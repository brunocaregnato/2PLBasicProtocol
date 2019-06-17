using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TrabalhoBanco2
{
    class Protocol2PLBasic
    {
        private Dictionary<string, DataLockTransactions> TransactionDataLock { get; set; }
        private Dictionary<int, Transactions> TransactionsList { get; set; }
        private TextBox OutPut { get; set; }
        private Queue<string> ExecutionRow { get; set; }
        private Dictionary<int, TransactionQueueCommand> WaitingTransactionsList { get; set; }

        public Protocol2PLBasic()
        {
            TransactionsList = new Dictionary<int, Transactions>();
            TransactionDataLock = new Dictionary<string, DataLockTransactions>();
            WaitingTransactionsList = new Dictionary<int, TransactionQueueCommand>();
        }

        // exemplo: w1(x)-r2(y)-w2(y)-c2-w1(y)
        public void History(string entryTransaction, TextBox output, TextBox RunningRow, TextBox WaitingTransaction, TextBox AbortedTransaction, TextBox Queue, TextBox DataLock)
        {
            string[] transactionList;

            // Quebra a string de entrada em um vetor de string
            transactionList = entryTransaction.Split('-');
            OutPut = output;
            
            if (ExecutionRow == null) ExecutionRow = new Queue<string>(transactionList);
            
            if (ExecutionRow.Count > 0)
            {
                RunCommand(ExecutionRow.Dequeue());
                CheckWaitingQueue();
                RunningRow.Text = RetornaRunningRow();
                WaitingTransaction.Text = ReturnWaitingTransactions();
                AbortedTransaction.Text = ReturnTransactionsAborteds();
                Queue.Text = ReturnQueue();
                DataLock.Text = ReturnDataLock();
                SetFinalPoint(OutPut);
                RunningRow.Refresh();
                WaitingTransaction.Refresh();
                AbortedTransaction.Refresh();
                DataLock.Refresh();
                SetFinalPoint(Queue);
            }
            else
            {
                OutPut.Text += "\r\nExecução finalizada.";
                SetFinalPoint(OutPut);
            }
        }
        
        private void CheckWaitingQueue()
        {
            // Analisa primeiro comando de cada transação.
            List<int> excludes = new List<int>();
            foreach (var TransactionNumber in WaitingTransactionsList.Keys)
            {
                TransactionQueueCommand commandsTransactions = WaitingTransactionsList[TransactionNumber];
                if (!commandsTransactions.Transaction.LockType.Equals(TransactionTypeLock.Aborted))
                {
                    string firstCommand = commandsTransactions.CheckCommand();
                    LockDataType LockDataType = ReturnLockType(firstCommand.Substring(0, 1));

                    FreeData freeData = ReturnFreeData(commandsTransactions.Transaction, LockDataType, commandsTransactions.WaitingData);

                    // Se o dado estiver disponivel remove todos os dados da fila e adiciona novamente à fila de execução
                    int RemovedTransaction = 0;
                    string ciclo = "";
                    string retorno = "";
                    switch (freeData)
                    {
                        case FreeData.FreeData:
                        case FreeData.SharedTransactionsData:
                        case FreeData.OtherSharedTransactionsData:

                            Queue<string> tempQueue = new Queue<string>();

                            // Remove tudo que está na fila de execução para adicionar a fila de espera na frente
                            while (ExecutionRow.Count > 0) tempQueue.Enqueue(ExecutionRow.Dequeue());
                            
                            while (commandsTransactions.QueueCommands.Count > 0)
                            {
                                AddOutPut("Comando: " + commandsTransactions.CheckCommand() + " adicionado à fila de execução.");
                                ExecutionRow.Enqueue(commandsTransactions.RemoveCommand());
                            }

                            // Adiciona os commandos de volta a fila de execução
                            while (tempQueue.Count > 0) ExecutionRow.Enqueue(tempQueue.Dequeue());
                            
                            commandsTransactions.Transaction.LockType = TransactionTypeLock.Executing;

                            // Adiciona à lista de excluídos para remover no final
                            excludes.Add(TransactionNumber);
                            break;

                        case FreeData.ExclusiveOtherTransactionsData:

                            // Realiza validação para analisar se a transação entrou em deadlock
                            retorno = CheckDeadLock(TransactionNumber, commandsTransactions.WaitingData);
                            RemovedTransaction = int.Parse(retorno.Substring(0, retorno.IndexOf("|")));

                            ciclo = retorno.Substring(retorno.IndexOf("|") + 1);
                            if (RemovedTransaction > -1)
                            {
                                excludes.Add(RemovedTransaction);
                                // Desaloca dados da transação.
                                string msg = "";
                                Transactions Transactions = TransactionsList[RemovedTransaction];

                                // Realiza unlock do dado e retira da lista de dados utilizados.
                                string[] usedData = Transactions.ReturnDataUsed();
                                msg = "************* DEAD LOCK DETECTADO ************* " + "\r\n";
                                msg += ciclo + "\r\n";
                                msg += "Transação " + RemovedTransaction + " eliminada, pois foi a que executou menos comandos.\r\n";
                                AddOutPut(msg);
                                foreach (var dado in usedData)
                                {
                                    // Adiciona saída unclok para dados liberados
                                    AddOutPut(CreateLock(LockDataType.Unlock, Transactions.TransactionNumber, dado, Transactions.ReturnDataLockType(dado)));
                                    // Remove dado e transação da lista de dados locks
                                    RemoveDataLocksList(Transactions, dado);
                                    // Remove dado da lista de transações
                                    Transactions.RemoveData(dado);
                                }
                                Transactions.LockType = TransactionTypeLock.Aborted;
                            }
                            break;
                    }
                }
            }

            foreach (var transactionNumber in excludes) WaitingTransactionsList.Remove(transactionNumber);
        }

        private string CheckDeadLock(int TransactionNumber, string Dado)
        {
            List<int> transacoesWaiting = new List<int>();
            transacoesWaiting.Add(TransactionNumber);

            return CheckDeadLock(TransactionNumber, Dado, transacoesWaiting);
        }

        private string CheckDeadLock(int TransactionNumber, string Dado, List<int> TransacoesWaiting)
        {
            string retorno = "-1|x";
            DataLockTransactions TransactionsUsingData;

            // Se não existir o dado é pq a transação já foi abortada
            if (TransactionDataLock.ContainsKey(Dado))
            {
                TransactionsUsingData = TransactionDataLock[Dado];
                foreach (var transactionNumber in TransactionsUsingData.ReturnTransactions())
                {
                    // Não verifica a própria transação
                    if (!transactionNumber.Equals(TransactionNumber))
                    {
                        // Adiciona na lista e verifica se ocorreu dead lock.
                        if (TransactionsList[TransactionNumber].LockType.Equals(TransactionTypeLock.Waiting))
                        {
                            if (TransacoesWaiting.Contains(TransactionNumber))
                            {
                                // Elimina deadlock
                                int eliminatedTransaction = RemoveDeadLock(TransacoesWaiting);
                                retorno = eliminatedTransaction + "|" + ReturnTransactionsCycle(TransacoesWaiting, eliminatedTransaction);
                            }
                            else
                            {
                                TransacoesWaiting.Add(TransactionNumber);
                                // Retorna dado que a transação está espera.
                                TransactionQueueCommand commandsTransactions = WaitingTransactionsList[TransactionNumber];
                                // Verifica pendencias desta transação.
                                retorno = CheckDeadLock(TransactionNumber, commandsTransactions.WaitingData, TransacoesWaiting);
                            }
                        }
                    }
                }
            }
            return retorno;
        }

        public string ReturnTransactionsCycle(List<int> transactionList, int TransactionNumber)
        {
            string ciclo = "";

            foreach (var transactionNumber in transactionList) ciclo += "t" + transactionNumber + "=>";
            
            ciclo += "t" + TransactionNumber;
            return ciclo;
        }

        public int RemoveDeadLock(List<int> TransacoesWaiting)
        {
            int removedTransaction = 0;
            int menorNumeroExecucoes = 999999999;
            Transactions transactions;

            foreach (var TransactionNumber in TransacoesWaiting)
            {
                transactions = TransactionsList[TransactionNumber];

                if (transactions.ExecutedCommands < menorNumeroExecucoes)
                {
                    menorNumeroExecucoes = transactions.ExecutedCommands;
                    removedTransaction = TransactionNumber;
                }
            }

            return removedTransaction;

        }

        // Utilizado para retorna o tipo de disponibilidade de um dado acessado pelo comando de uma transação
        private enum FreeData
        {
            FreeData,
            ExclusiveOtherTransactionsData,
            SharedTransactionsData,
            OtherSharedTransactionsData,
        }

        private FreeData ReturnFreeData(Transactions Transactions, LockDataType LockDataType, string Dado)
        {
            // Verifica se o dado está lockado para alguma sessão
            if (TransactionDataLock.ContainsKey(Dado))
            {
                DataLockTransactions usedData = TransactionDataLock[Dado];
                // Verifica se está lockado para a transação que está tentando acessar.
                if (usedData.CheckLockForTransaction(Transactions.TransactionNumber))
                {
                    /* Se o tipo de dado for restrito deve ter lock exclusivo, neste caso
                     não pode estar lockado para outras transações */
                    if (LockDataType.Equals(LockDataType.Exclusive) && usedData.NumberOfTransactions() > 1)
                        return FreeData.ExclusiveOtherTransactionsData;
                    
                    else
                        return FreeData.SharedTransactionsData;                    
                }

                // Verifica se tem lock exclusivo para outra sessão.
                else
                {
                    /* Adiciona à lista de espera quando estiver lock exclusivo para outra transação
                     Ou quando a transação pediu lock exclusivo */
                    if (usedData.LockType.Equals(LockDataType.Exclusive) || LockDataType.Equals(LockDataType.Exclusive))
                        return FreeData.ExclusiveOtherTransactionsData;
                    
                    else
                        return FreeData.OtherSharedTransactionsData;                    
                }
            }
            // Quandoo dado não está sendo utilizado por nenhuma sessão
            else
                return FreeData.FreeData;            
        }

        private void SetFinalPoint(TextBox TextBox)
        {
            TextBox.SelectionStart = TextBox.Text.Length;
            TextBox.ScrollToCaret();
            TextBox.Refresh();
        }


        private void RunCommand(string Command)
        {
            Transactions transactions = null;
            // Extrai Tipo de dado
            string commandType = Command.Substring(0, 1);

            // retorna o tipo de dado
            LockDataType LockDataType = ReturnLockType(commandType);

            // Extrai número da transação
            int transactionNumber = int.Parse(Command.Substring(1, 1));

            // Adiciona transação com status inicial executando
            transactions = AddTransactions(transactionNumber, TransactionTypeLock.Executing);

            // Extrai nome do dado
            string dado = "";

            if (!LockDataType.Equals(LockDataType.Commit))
                dado = Command.Substring(3, 1);
            
            // Tratamento para comando conforme o status da transação.
            if (transactions.LockType.Equals(TransactionTypeLock.Executing))
                SolvingExecutingTransactions(LockDataType, dado, transactions, Command);
            
            // Quando a transação estiver waiting, adiciona o comando à fila de espera
            else if (transactions.LockType.Equals(TransactionTypeLock.Waiting))
                AddCommandWaitingQueue(transactions, Command);
            
            else
                AddOutPut("Transação " + transactions.TransactionNumber + " Aborted. Command " + Command + " ignorado.");
            
        }

        public void AddCommandWaitingQueue(Transactions Transactions, string Command)
        {
            // Adiciona a transação à fila quando ainda não existir
            TransactionQueueCommand transactionQueueCommand = null;
            if (!WaitingTransactionsList.ContainsKey(Transactions.TransactionNumber))
            {
                Transactions.LockType = TransactionTypeLock.Waiting;
                transactionQueueCommand = new TransactionQueueCommand(Transactions);
                WaitingTransactionsList.Add(Transactions.TransactionNumber, transactionQueueCommand);
            }
            // Retorna a fila de Commands da transação.
            else
                transactionQueueCommand = WaitingTransactionsList[Transactions.TransactionNumber];

            // Adiciona Command à fila de espera da transação.
            transactionQueueCommand.AddComamand(Command);
            AddOutPut("Comando: " + Command + " adicionado à fila de espera.");

        }

        public void SolvingExecutingTransactions(LockDataType LockDataType, string Dado, Transactions Transactions, string Command)
        {
            DataLockTransactions usedData;

            // Verifica se o dado está disponível
            if (LockDataType.Equals(LockDataType.Commit))
            {
                /* Quando for commit, verifica se o status da transação esta Waiting.
                 se não estiver percorre a lista de dados lockadas pela transação e libera todas
                 e dispara o commit
                 Se estiver Waiting adiciona o comando commit a lista de espera */
                if (Transactions.LockType.Equals(TransactionTypeLock.Waiting))
                    AddCommandWaitingQueue(Transactions, Command);
                
                // libera lock dados e realiza commit.
                else if (!Transactions.LockType.Equals(TransactionTypeLock.Aborted))
                {
                    // Realiza unlock do dado e retira da lista de dados utilizados.
                    string[] usedDataVet = Transactions.ReturnDataUsed();
                    foreach (var dado in usedDataVet)
                    {
                        // Adiciona saída unclok para dados liberados
                        AddOutPut(CreateLock(LockDataType.Unlock, Transactions.TransactionNumber, dado, Transactions.ReturnDataLockType(dado)));

                        // Remove dado e transação da lista de dados locks
                        RemoveDataLocksList(Transactions, dado);

                        // Remove dado da lista de transações
                        Transactions.RemoveData(dado);
                    }
                    // Incrementa Commands executados da transação
                    Transactions.ExecutedCommands++;

                    // Adiciona saída para commit.
                    AddOutPut(Command);
                }
            }
            // Verifica disponibidade do dado para a transação e Command
            else
            {
                //DadoLockTransactions usedData;
                FreeData freeData = ReturnFreeData(Transactions, LockDataType, Dado);
                // Dado não foi lockado por nenhuma transação
                if (freeData.Equals(FreeData.FreeData))
                {
                    usedData = new DataLockTransactions(Dado, LockDataType);
                    TransactionDataLock.Add(Dado, usedData);
                    TransactionDataLock[Dado].AddTransaction(Transactions);
                    Transactions.AddData(Dado, LockDataType);
                    // Adiciona lock.
                    AddOutPut(CreateLock(LockDataType, Transactions.TransactionNumber, Dado, null));
                    // Incrementa Commands executados da transação
                    Transactions.ExecutedCommands++;
                    // Adiciona Command lido
                    AddOutPut(Command);
                }
                // Dado já esta lockado para a transação, mas como shared
                else if (freeData.Equals(FreeData.SharedTransactionsData))
                {
                    usedData = TransactionDataLock[Dado];
                    // Faz upgrade no lock se necessário.
                    if (UpgradeLock(usedData, LockDataType, Transactions))
                        AddOutPut(CreateLock(LockDataType, Transactions.TransactionNumber, Dado, null));
                    
                    // Incrementa Commands executados da transação
                    Transactions.ExecutedCommands++;
                    // Adiciona Command lido
                    AddOutPut(Command);
                }
                // Dado está com lock shared em outra transação
                else if (freeData.Equals(FreeData.OtherSharedTransactionsData))
                {   
                    // Adiciona o dado à lista de dados da transação
                    Transactions.AddData(Dado, LockDataType);
                    TransactionDataLock[Dado].AddTransaction(Transactions);
                    // Adiciona lock.
                    AddOutPut(CreateLock(LockDataType, Transactions.TransactionNumber, Dado, null));
                    // Incrementa Commands executados da transação
                    Transactions.ExecutedCommands++;
                    // Adiciona Command lido
                    AddOutPut(Command);
                }
                /* Dado está com lock exclusivo em outra transação
                   ou precisa de lock exclusivo para a transação atual */
                else if (freeData.Equals(FreeData.ExclusiveOtherTransactionsData))
                    AddCommandWaitingQueue(Transactions, Command);
                
            }
        }

        private void RemoveDataLocksList(Transactions Transactions, string Dado)
        {
            DataLockTransactions dadoLockTransactions = TransactionDataLock[Dado];
            if (dadoLockTransactions != null)
            {
                dadoLockTransactions.RemoveTransaction(Transactions);

                /* Se não existe mais nenhuma transação utilizando o dado
                   então remove da lista de dados utilizados. */
                if (dadoLockTransactions.NumberOfTransactions().Equals(0))
                    TransactionDataLock.Remove(Dado);
                
            }
        }

        // Adiciona a transação à lista de transações.
        private Transactions AddTransactions(int TransactionNumber, TransactionTypeLock LockTransactions)
        {
            Transactions transaction = null;
            if (!TransactionsList.ContainsKey(TransactionNumber))
            {
                transaction = new Transactions(TransactionNumber, LockTransactions);
                TransactionsList.Add(TransactionNumber, transaction);
            }
            else
                transaction = TransactionsList[TransactionNumber];
            
            return transaction;
        }

        private LockDataType ReturnLockType(string Tipo)
        {
            LockDataType lockDataType = LockDataType.Unlock;

            if (Tipo.Equals("w"))
                lockDataType = LockDataType.Exclusive;
            
            else if(Tipo.Equals("r"))
                lockDataType = LockDataType.Shared;
            
            else if (Tipo.Equals("c"))
                lockDataType = LockDataType.Commit;
            
            return lockDataType;
        }

        private string CreateLock(LockDataType LockDataType, int TransactionNumber, string Dado, LockDataType? LockDataTypeUnlock)
        {
            string outPut;

            // formata tipo de lock
            if (LockDataType.Equals(LockDataType.Unlock))
            {
                outPut = "u";
                if (LockDataTypeUnlock.Equals(LockDataType.Exclusive))
                    outPut += "x";
                
                else
                    outPut += "h";
                
            }
            else
            {
                outPut = "l";
                if (LockDataType.Equals(LockDataType.Exclusive))
                    outPut += "x";
                
                else
                    outPut += "h";
                
            }
            // adiciona o número da transação ao lock
            outPut += TransactionNumber;
            outPut += "[" + Dado + "]";

            return outPut;
        }

        private void AddOutPut(string outPut)
        {
            if (!outPut.Equals(""))
            {
                if (OutPut.Text.Equals(""))
                    OutPut.Text = outPut;
                
                else
                    OutPut.Text += "\r\n" + outPut;                
            }
        }

        private bool UpgradeLock(DataLockTransactions Dado, LockDataType LockType, Transactions Transactions)
        {
            if (LockType.Equals(LockDataType.Exclusive) && !Dado.LockType.Equals(LockDataType.Exclusive))
            {
                Dado.LockType = LockDataType.Exclusive;
                Transactions.ChangeDataLockType(Dado.Data, LockDataType.Exclusive);
                return true;
            }

            return false;
        }

        public string RetornaRunningRow()
        {
            string retorno = "";
            foreach (var command in ExecutionRow)
            {
                if (retorno.Equals(""))
                    retorno = command;
                
                else
                    retorno += "\r\n" + command;                
            }

            return retorno;
        }

        public string ReturnWaitingTransactions()
        {
            string retorno = "";
            foreach (var transactions in TransactionsList.Values)
            {
                if (transactions.LockType.Equals(TransactionTypeLock.Waiting))
                {
                    if (!retorno.Equals(""))
                        retorno += "\r\n";
                    
                    retorno += "Transação " + transactions.TransactionNumber;
                    var filaCommands = WaitingTransactionsList[transactions.TransactionNumber];
                    retorno += ": " + filaCommands.WaitingData;
                }
            }
            return retorno;
        }

        public string ReturnTransactionsAborteds()
        {
            string retorno = "";
            foreach (var transactions in TransactionsList.Values)
            {
                if (transactions.LockType.Equals(TransactionTypeLock.Aborted))
                {
                    if (retorno.Equals(""))
                        retorno = "Transação " + transactions.TransactionNumber;
                    
                    else
                        retorno += "\r\n" + "Transação " + transactions.TransactionNumber;                    
                }
            }

            return retorno;
        }

        public string ReturnQueue()
        {
            string retorno = "";

            foreach (var commandsTransactions in WaitingTransactionsList.Values)
            {
                if (!retorno.Equals(""))
                    retorno += "\r\n";
                
                retorno += "Transação: " + commandsTransactions.Transaction.TransactionNumber + "\r\n";
                string[] commands = commandsTransactions.ReturnQueueCommand();

                foreach (var command in commands) retorno += command + "\r\n";                
            }

            return retorno;
        }

        public string ReturnDataLock()
        {
            string retorno = "";
            foreach (var transactions in TransactionsList.Values)
            {
                string[] usedData = transactions.ReturnDataUsed();
                if (usedData.Count() > 0)
                {
                    if (!retorno.Equals("")) retorno += "\r\n";
                    
                    retorno += "Transação" + transactions.TransactionNumber + ": ";

                    for (int i = 0; i < usedData.Count(); i++)
                    {
                        retorno += transactions.ReturnDataLockType(usedData[i]) + "(" + usedData[i] + ")";

                        if (i < usedData.Count() - 1)
                            retorno += ",";
                    }
                }
            }

            return retorno;
        }
    }
}
