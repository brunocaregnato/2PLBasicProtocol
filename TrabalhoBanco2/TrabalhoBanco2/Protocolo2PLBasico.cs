using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TrabalhoBanco2
{
    /// <summary>
    /// Define o tipo de execução utilizado
    /// </summary>
    class Protocolo2PLBasico
    {
        private Dictionary<String, DataLockTransactions> TransactionDataLock { get; set; }
        private Dictionary<int, Transactions> TransactionsList { get; set; }
        private  TextBox OutPut { get; set; }
        private Queue<string> ExecutionRow { get; set; }
        private Dictionary<int, TransactionQueueCommand> WaitingTransactionsList { get; set; }

        // exemplo: w1(x)-r2(y)-w2(y)-c2-w1(y)
        public void History(string entryTransaction, TextBox output, TextBox RunningRow, TextBox WaitingTransaction, TextBox AbortedTransaction, TextBox Queue, TextBox DataLock)
        {
            String[] transactionList;

            // Quebra a string de entrada em um vetor de string
            transactionList = entryTransaction.Split('-');
            OutPut = output;
            
            if (ExecutionRow == null) ExecutionRow = new Queue<string>(transactionList);
            
            if (ExecutionRow.Count > 0)
            {
                ExecutaComando(ExecutionRow.Dequeue());
                AnalisaFilaEspera();
                RunningRow.Text = RetornaRunningRow();
                WaitingTransaction.Text = RetornaTransacoesEmEspera();
                AbortedTransaction.Text = RetornaTransacoesAborteds();
                Queue.Text = RetornaQueue();
                DataLock.Text = RetornaDataLock();
                PosicionaCursorFinal(OutPut);
                RunningRow.Refresh();
                WaitingTransaction.Refresh();
                AbortedTransaction.Refresh();
                DataLock.Refresh();
                PosicionaCursorFinal(Queue);
            }
            else
            {
                OutPut.Text += "\r\nExecução finalizada.";
                PosicionaCursorFinal(OutPut);
            }
        }
        
        private void AnalisaFilaEspera()
        {
            // Analisa primeiro comando de cada transação.
            List<int> excluidos = new List<int>();
            foreach (int TransactionNumber in WaitingTransactionsList.Keys)
            {
                TransactionQueueCommand comandosTransactions = WaitingTransactionsList[TransactionNumber];
                if (comandosTransactions.Transaction.LockType != TransactionTypeLock.Aborted)
                {
                    String primeiroComando = comandosTransactions.ConsultaComando();
                    LockDataType LockDataType = RetornaLockType(primeiroComando.Substring(0, 1));

                    DisponibilidadeDado dispDado = RetornaDisponibilidadeDado(comandosTransactions.Transaction, LockDataType, comandosTransactions.WaitingData);

                    // Se o dado estiver disponivel remove todos os dados da fila e adiciona novamente à fila de execução
                    int TransactionsEliminada = 0;
                    string ciclo = "";
                    string retorno = "";
                    switch (dispDado)
                    {
                        case DisponibilidadeDado.DadoDisponivel:
                        case DisponibilidadeDado.DadoSharedTransactions:
                        case DisponibilidadeDado.DadoSharedOutraTransactions:
                            Queue<String> filaTemp = new Queue<string>();
                            // Remove tudo que está na fila de execução para adicionar a fila de espera na frente
                            while (ExecutionRow.Count > 0)
                            {
                                filaTemp.Enqueue(ExecutionRow.Dequeue());
                            }
                            while (comandosTransactions.ContComandos > 0)
                            {
                                AddOutPut("Comando: " + comandosTransactions.ConsultaComando() + " adicionado à fila de execução.");
                                ExecutionRow.Enqueue(comandosTransactions.RemoveComando());
                            }
                            // Adiciona comandos de volta a fila de execução
                            while (filaTemp.Count > 0)
                            {
                                ExecutionRow.Enqueue(filaTemp.Dequeue());
                            }
                            comandosTransactions.Transaction.LockType = TransactionTypeLock.Executing;
                            // Adiciona à lista de excluídos para remover no final
                            excluidos.Add(TransactionNumber);
                            break;
                        case DisponibilidadeDado.DadoExclusiveOutraTransactions:
                            // Realiza validação para analisar se a transação entrou em deadlock
                            retorno = VerificaDeadLock(TransactionNumber, comandosTransactions.WaitingData);
                            TransactionsEliminada = int.Parse(retorno.Substring(0, retorno.IndexOf("|")));
                            ciclo = retorno.Substring(retorno.IndexOf("|") + 1);
                            if (TransactionsEliminada > -1)
                            {
                                excluidos.Add(TransactionsEliminada);
                                // Desaloca dados da transação.
                                String msg = "";
                                Transactions Transactions = TransactionsList[TransactionsEliminada];
                                // Realiza unlock do dado e retira da lista de dados utilizados.
                                String[] dadosUtilizados = Transactions.ReturnDataUsed();
                                msg = "************* DEAD LOCK DETECTADO ************* " + "\r\n";
                                msg += ciclo + "\r\n";
                                msg += "Transação " + TransactionsEliminada + " eliminada, pois foi a que executou menos comandos.\r\n";
                                AddOutPut(msg);
                                foreach (String dado in dadosUtilizados)
                                {
                                    // Adiciona saída unclok para dados liberados
                                    AddOutPut(GeraLock(LockDataType.Unlock, Transactions.TransactionNumber, dado, Transactions.ReturnDataLockType(dado)));
                                    // Remove dado e transação da lista de dados locks
                                    RemoveDadoListaLocks(Transactions, dado);
                                    // Remove dado da lista de transações
                                    Transactions.RemoveData(dado);
                                }
                                Transactions.LockType = TransactionTypeLock.Aborted;
                            }
                            break;
                    }
                }
            }
            foreach (int TransactionNumber in excluidos)
            {
                WaitingTransactionsList.Remove(TransactionNumber);
            }
        }

        private string VerificaDeadLock(int TransactionNumber, String Dado)
        {
            List<int> transacoesWaiting = new List<int>();
            transacoesWaiting.Add(TransactionNumber);
            return VerificaDeadLock(TransactionNumber, Dado, transacoesWaiting);
        }

        private string VerificaDeadLock(int TransactionNumber, String Dado, List<int> TransacoesWaiting)
        {
            int eliminatedTransaction = -1;
            string retorno = "-1|x";
            DataLockTransactions TransacoesUsandoDado;
            // Se não existir o dado é pq a transação já foi Aborted
            if (TransactionDataLock.ContainsKey(Dado))
            {
                TransacoesUsandoDado = TransactionDataLock[Dado];
                foreach (int transactionNumber in TransacoesUsandoDado.RetornaTransacoes())
                {
                    // Não verifica a própria transação
                    if (transactionNumber != TransactionNumber)
                    {
                        // Adiciona na lista e verifica se ocorreu dead lock.
                        if (TransactionsList[TransactionNumber].LockType == TransactionTypeLock.Waiting)
                        {
                            if (TransacoesWaiting.Contains(TransactionNumber))
                            {
                                // Elimina dead lock
                                eliminatedTransaction = EliminaDeadLock(TransacoesWaiting);
                                retorno = eliminatedTransaction + "|" + RetornaCicloTransacoes(TransacoesWaiting, eliminatedTransaction);
                            }
                            else
                            {
                                TransacoesWaiting.Add(TransactionNumber);
                                // Retorna dado que a transação está Waiting.
                                TransactionQueueCommand ComandosTransactions = WaitingTransactionsList[TransactionNumber];
                                // Verifica pendencias desta transação.
                                retorno = VerificaDeadLock(TransactionNumber, ComandosTransactions.WaitingData, TransacoesWaiting);
                            }
                        }
                    }
                }
            }
            return retorno;
        }

        public String RetornaCicloTransacoes(List<int> transactionList, int TransactionNumber)
        {
            string ciclo = "";
            foreach (int transactionNumber in transactionList)
            {
                ciclo += "t" + transactionNumber + "=>";
            }
            ciclo += "t" + TransactionNumber;
            return ciclo;
        }

        public int EliminaDeadLock(List<int> TransacoesWaiting)
        {
            int TransactionsEliminada = 0;
            int menorNumeroExecucoes = 999999999;
            Transactions Transactions;
            foreach (int TransactionNumber in TransacoesWaiting)
            {
                Transactions = TransactionsList[TransactionNumber];
                if (Transactions.ExecutedCommands < menorNumeroExecucoes)
                {
                    menorNumeroExecucoes = Transactions.ExecutedCommands;
                    TransactionsEliminada = TransactionNumber;
                }
            }
            return TransactionsEliminada;

        }

        // Utilizado para retorna o tipo de disponibilidade de um dado acessado pelo comando de uma transação
        private enum DisponibilidadeDado
        {
            DadoDisponivel,
            DadoExclusiveOutraTransactions,
            DadoSharedTransactions,
            DadoSharedOutraTransactions,
        }

        private DisponibilidadeDado RetornaDisponibilidadeDado(Transactions Transactions, LockDataType LockDataType, String Dado)
        {
            // Verifica se o dado está lockado para alguma sessão
            if (TransactionDataLock.ContainsKey(Dado))
            {
                DataLockTransactions dadoUtilizado = TransactionDataLock[Dado];
                // Verifica se está lockado para a transação que está tentando acessar.
                if (dadoUtilizado.VerificaLockParaTransacao(Transactions.TransactionNumber))
                {
                    // Se o tipo de dado for estrita deve ter lock exclusivo, neste caso
                    // não pode estar lockado para outras transações
                    if (LockDataType == LockDataType.Exclusive && dadoUtilizado.NumeroDeTransacoes() > 1)
                    {
                        // deve adicionar na lista de espera.
                        return DisponibilidadeDado.DadoExclusiveOutraTransactions;
                    }
                    else
                    {
                        return DisponibilidadeDado.DadoSharedTransactions;
                    }
                }
                // Verifica se tem lock exclusivo para outra sessão.
                else
                {
                    // Adiciona à lista de espera quando estiver lock exclusivo para outra transação
                    // Ou quando a transação pediu lock exclusivo
                    if (dadoUtilizado.TipoLock == LockDataType.Exclusive || LockDataType == LockDataType.Exclusive)
                    {
                        return DisponibilidadeDado.DadoExclusiveOutraTransactions;
                    }
                    else
                    {
                        return DisponibilidadeDado.DadoSharedOutraTransactions;
                    }
                }
            }
            // Quandoo dado não está sendo utilizado por nenhuma sessão
            else
            {
                return DisponibilidadeDado.DadoDisponivel;
            }
        }

        private void PosicionaCursorFinal(TextBox TextBox)
        {
            //TextBox.Focus();
            TextBox.SelectionStart = TextBox.Text.Length;
            TextBox.ScrollToCaret();
            TextBox.Refresh();
        }


        private void ExecutaComando(String Comando)
        {
            Transactions Transactions = null;
            // Extrai Tipo de dado
            String tipoComando = Comando.Substring(0, 1);
            // retorna o tipo de dado
            LockDataType LockDataType = RetornaLockType(tipoComando);
            // Extrai número da transação
            int TransactionNumber = int.Parse(Comando.Substring(1, 1));
            // Adiciona transação com status inicial executando
            Transactions = AdicionaTransactions(TransactionNumber, TransactionTypeLock.Executing);
            // Extrai nome do dado
            string dado = "";
            if (LockDataType != LockDataType.Commit)
            {
                dado = Comando.Substring(3, 1);
            }
            // Tratamento para comando conforme o status da transação.
            if (Transactions.LockType == TransactionTypeLock.Executing)
            {
                TratamentoTransactionsExecutando(LockDataType, dado, Transactions, Comando);
            }
            // Quando a transação estiver Waiting, adiciona o comando à fila de espera
            else if (Transactions.LockType == TransactionTypeLock.Waiting)
            {
                AdicionarComandoFilaEspera(Transactions, Comando);
            }
            else
            {
                AddOutPut("Transação " + Transactions.TransactionNumber + " Aborted. Comando " + Comando + " ignorado.");
            }
        }

        public void AdicionarComandoFilaEspera(Transactions Transactions, String Comando)
        {
            // Adiciona a transação à fila quando ainda não existir
            TransactionQueueCommand TransactionQueueCommand = null;
            if (!WaitingTransactionsList.ContainsKey(Transactions.TransactionNumber))
            {
                Transactions.LockType = TransactionTypeLock.Waiting;
                TransactionQueueCommand = new TransactionQueueCommand(Transactions);
                WaitingTransactionsList.Add(Transactions.TransactionNumber, TransactionQueueCommand);
            }
            // Retorna a fila de comandos da transação.
            else
            {
                TransactionQueueCommand = WaitingTransactionsList[Transactions.TransactionNumber];
            }
            // Adiciona comando à fila de espera da transação.
            TransactionQueueCommand.AdicionaComando(Comando);
            AddOutPut("Comando: " + Comando + " adicionado à fila de espera.");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LockDataType">Utilizado pra indicar qual o tipo de lock está sobre o dado</param>
        /// <param name="Dado">Dado sendo utilizado pela transação</param>
        /// <param name="Transactions">Transação que executou o comando</param>
        /// <param name="Comando">Comando executado pela transação</param>
        public void TratamentoTransactionsExecutando(LockDataType LockDataType, String Dado, Transactions Transactions, String Comando)
        {
            DataLockTransactions dadoUtilizado;
            // Verifica se o dado está disponível
            if (LockDataType == LockDataType.Commit)
            {
                // Quando for commit, verifica se o status da transação esta Waiting.
                // se não estiver percorre a lista de dados lockadas pela transação e libera todas
                // e dispara o commit
                // Se estiver Waiting adiciona o comnado commit a lista de espera
                if (Transactions.LockType == TransactionTypeLock.Waiting)
                {
                    AdicionarComandoFilaEspera(Transactions, Comando);
                }
                // libera lock dados e realiza commit.
                else if (Transactions.LockType != TransactionTypeLock.Aborted)
                {
                    // Realiza unlock do dado e retira da lista de dados utilizados.
                    String[] dadosUtilizados = Transactions.ReturnDataUsed();
                    foreach (String dado in dadosUtilizados)
                    {
                        // Adiciona saída unclok para dados liberados
                        AddOutPut(GeraLock(LockDataType.Unlock, Transactions.TransactionNumber, dado, Transactions.ReturnDataLockType(dado)));
                        // Remove dado e transação da lista de dados locks
                        RemoveDadoListaLocks(Transactions, dado);
                        // Remove dado da lista de transações
                        Transactions.RemoveData(dado);
                    }
                    // Incrementa comandos executados da transação
                    Transactions.ExecutedCommands++;
                    // Adiciona saída para commit.
                    AddOutPut(Comando);
                }
            }
            // Verifica disponibidade do dado para a transação e comando
            else
            {
                //DadoLockTransactions dadoUtilizado;
                DisponibilidadeDado dispDado = RetornaDisponibilidadeDado(Transactions, LockDataType, Dado);
                // Dado não foi lockado por nenhuma transação
                if (dispDado == DisponibilidadeDado.DadoDisponivel)
                {
                    dadoUtilizado = new DataLockTransactions(Dado, LockDataType);
                    TransactionDataLock.Add(Dado, dadoUtilizado);
                    TransactionDataLock[Dado].AdicionaTransacao(Transactions);
                    Transactions.AddData(Dado, LockDataType);
                    // Adiciona lock.
                    AddOutPut(GeraLock(LockDataType, Transactions.TransactionNumber, Dado, null));
                    // Incrementa comandos executados da transação
                    Transactions.ExecutedCommands++;
                    // Adiciona comando lido
                    AddOutPut(Comando);
                }
                // Dado já esta lockado para a transação, mas como shared
                else if (dispDado == DisponibilidadeDado.DadoSharedTransactions)
                {
                    dadoUtilizado = TransactionDataLock[Dado];
                    // Faz upgrade no lock se necessário.
                    if (UpgradeLock(dadoUtilizado, LockDataType, Transactions))
                    {
                        AddOutPut(GeraLock(LockDataType, Transactions.TransactionNumber, Dado, null));
                    }
                    // Incrementa comandos executados da transação
                    Transactions.ExecutedCommands++;
                    // Adiciona comando lido
                    AddOutPut(Comando);
                }
                // Dado está com lock shared em outra transação
                else if (dispDado == DisponibilidadeDado.DadoSharedOutraTransactions)
                {
                    dadoUtilizado = TransactionDataLock[Dado];
                    // Adiciona transação a lista de dados lockados
                    //TransactionDataLock.Add(Dado, dadoUtilizado);
                    // Adiciona o dado à lista de dados da transação
                    Transactions.AddData(Dado, LockDataType);
                    TransactionDataLock[Dado].AdicionaTransacao(Transactions);
                    // Adiciona lock.
                    AddOutPut(GeraLock(LockDataType, Transactions.TransactionNumber, Dado, null));
                    // Incrementa comandos executados da transação
                    Transactions.ExecutedCommands++;
                    // Adiciona comando lido
                    AddOutPut(Comando);
                }
                // Dado está com lock exclusivo em outra transação
                // ou precisa de lock exclusivo para a transação atual
                else if (dispDado == DisponibilidadeDado.DadoExclusiveOutraTransactions)
                {
                    AdicionarComandoFilaEspera(Transactions, Comando);
                }
            }
        }

        // TODO
        // Remover no final se não ocorre erro no método atual
        public void TratamentoTransactionsExecutando_old(LockDataType LockDataType, String Dado, Transactions Transactions, String Comando)
        {
            DataLockTransactions dadoUtilizado;
            // Verifica se o dado está disponível
            if (LockDataType == LockDataType.Commit)
            {
                // Quando for commit, verifica se o status da transação esta Waiting.
                // se não estiver percorre a lista de dados lockadas pela transação e libera todas
                // e dispara o commit
                // Se estiver Waiting adiciona o comnado commit a lista de espera
                //w1(x)-r2(y)-w2(y)-c2-w1(y)-c1
                if (Transactions.LockType == TransactionTypeLock.Waiting)
                {
                    AdicionarComandoFilaEspera(Transactions, Comando);
                }
                // libera lock dados e realiza commit.
                else
                {
                    // Realiza unlock do dado e retira da lista de dados utilizados.
                    String[] dadosUtilizados = Transactions.ReturnDataUsed();
                    foreach (String dado in dadosUtilizados)
                    {
                        // Adiciona saída unclok para dados liberados
                        AddOutPut(GeraLock(LockDataType.Unlock, Transactions.TransactionNumber, dado, Transactions.ReturnDataLockType(dado)));
                        // Remove dado e transação da lista de dados locks
                        RemoveDadoListaLocks(Transactions, dado);
                        // Remove dado da lista de transações
                        Transactions.RemoveData(dado);
                    }
                    // Incrementa comandos executados da transação
                    Transactions.ExecutedCommands++;
                    // Adiciona saída para commit.
                    AddOutPut(Comando);
                }
            }
            else if (TransactionDataLock.ContainsKey(Dado))
            {
                dadoUtilizado = dadoUtilizado = TransactionDataLock[Dado];
                // Verifica se está lockado para a transação que está tentando acessar.
                if (dadoUtilizado.VerificaLockParaTransacao(Transactions.TransactionNumber))
                {
                    // Se o tipo de dado for estrita deve ter lock exclusivo, neste caso
                    // não pode estar lockado para outras transações
                    if (LockDataType == LockDataType.Exclusive && dadoUtilizado.NumeroDeTransacoes() > 1)
                    {
                        // deve adicionar na lista de espera.
                        AdicionarComandoFilaEspera(Transactions, Comando);
                    }
                    else
                    {
                        // Faz upgrade no lock se necessário.
                        if (UpgradeLock(dadoUtilizado, LockDataType, Transactions))
                        {
                            AddOutPut(GeraLock(LockDataType, Transactions.TransactionNumber, Dado, null));
                        }
                        // Incrementa comandos executados da transação
                        Transactions.ExecutedCommands++;
                        // Adiciona comando lido
                        AddOutPut(Comando);
                    }
                }
                // Verifica se tem lock exclusivo para outra sessão.
                else
                {
                    // Adiciona à lista de espera quando estiver lock exclusivo para outra transação
                    // Ou quando a transação pediu lock exclusivo
                    if (dadoUtilizado.TipoLock == TrabalhoBanco2.LockDataType.Exclusive || LockDataType == LockDataType.Exclusive)
                    {
                        AdicionarComandoFilaEspera(Transactions, Comando);
                    }
                    else
                    {
                        // Adiciona transação a lista de dados lockados
                        TransactionDataLock.Add(Dado, dadoUtilizado);
                        // Adiciona o dado à lista de dados da transação
                        Transactions.AddData(Dado, LockDataType);
                        // Adiciona lock.
                        AddOutPut(GeraLock(LockDataType, Transactions.TransactionNumber, Dado, null));
                        // Incrementa comandos executados da transação
                        Transactions.ExecutedCommands++;
                        // Adiciona comando lido
                        AddOutPut(Comando);
                    }
                }
            }
            else
            {
                // Quando o tipo de dado não existe na lista de lock
                // Adiciona o dado e gera a saída com lock
                dadoUtilizado = new DataLockTransactions(Dado, LockDataType);
                TransactionDataLock.Add(Dado, dadoUtilizado);
                TransactionDataLock[Dado].AdicionaTransacao(Transactions);
                Transactions.AddData(Dado, LockDataType);
                // Adiciona lock.
                AddOutPut(GeraLock(LockDataType, Transactions.TransactionNumber, Dado, null));
                // Incrementa comandos executados da transação
                Transactions.ExecutedCommands++;
                // Adiciona comando lido
                AddOutPut(Comando);
            }
        }

        private void RemoveDadoListaLocks(Transactions Transactions, String Dado)
        {
            DataLockTransactions dadoLockTransactions = TransactionDataLock[Dado];
            if (dadoLockTransactions != null)
            {
                dadoLockTransactions.RemoveTransacao(Transactions);
                // Se não existe mais nenhuma transação utilizando o dado
                // então remove da lista de dados utilizados.
                if (dadoLockTransactions.NumeroDeTransacoes() == 0)
                {
                    TransactionDataLock.Remove(Dado);
                }
            }
        }

        // Adiciona a transação à lista de transações.
        private Transactions AdicionaTransactions(int TransactionNumber, TransactionTypeLock LockTransactions)
        {
            Transactions Transactions = null;
            if (!TransactionsList.ContainsKey(TransactionNumber))
            {
                Transactions = new Transactions(TransactionNumber, LockTransactions);
                TransactionsList.Add(TransactionNumber, Transactions);
            }
            else
            {
                Transactions = TransactionsList[TransactionNumber];
            }
            return Transactions;
        }

        private LockDataType RetornaLockType(String Tipo)
        {
            LockDataType tipoDadoLock = LockDataType.Unlock;
            if (Tipo == "w")
            {
                tipoDadoLock = LockDataType.Exclusive;
            }
            else if (Tipo == "r")
            {
                tipoDadoLock = LockDataType.Shared;
            }
            else if (Tipo == "c")
            {
                tipoDadoLock = LockDataType.Commit;
            }
            return tipoDadoLock;
        }

        private string GeraLock(LockDataType LockDataType, int TransactionNumber, String Dado, LockDataType? LockDataTypeUnlock)
        {
            string OutPut;
            // formata tipo de lock
            if (LockDataType == LockDataType.Unlock)
            {
                OutPut = "u";
                if (LockDataTypeUnlock == LockDataType.Exclusive)
                {
                    OutPut += "x";
                }
                else
                {
                    OutPut += "h";
                }
            }
            else
            {
                OutPut = "l";
                if (LockDataType == LockDataType.Exclusive)
                {
                    OutPut += "x";
                }
                else
                {
                    OutPut += "h";
                }
            }
            // adiciona o número da transação ao lock
            OutPut += TransactionNumber;
            OutPut += "[" + Dado + "]";
            return OutPut;
        }

        private void AddOutPut(string outPut)
        {
            if (outPut != "")
            {
                if (OutPut.Text == "")
                {
                    OutPut.Text = outPut;
                }
                else
                {
                    OutPut.Text += "\r\n" + outPut;
                }
            }
        }

        private bool UpgradeLock(DataLockTransactions Dado, LockDataType LockType, Transactions Transactions)
        {
            if (LockType == LockDataType.Exclusive && Dado.TipoLock != LockDataType.Exclusive)
            {
                Dado.TipoLock = LockDataType.Exclusive;
                Transactions.ChangeDataLockType(Dado.Dado, LockDataType.Exclusive);
                return true;
            }
            return false;
        }

        public Protocolo2PLBasico()
        {
            TransactionsList = new Dictionary<int, Transactions>();
            TransactionDataLock = new Dictionary<string, DataLockTransactions>();
            WaitingTransactionsList = new Dictionary<int, TransactionQueueCommand>();
        }

        public String RetornaRunningRow()
        {
            String retorno = "";
            foreach (string comando in ExecutionRow)
            {
                if (retorno == "")
                {
                    retorno = comando;
                }
                else
                {
                    retorno += "\r\n" + comando;
                }
            }
            return retorno;
        }

        public String RetornaTransacoesEmEspera()
        {
            String retorno = "";
            foreach (Transactions Transactions in TransactionsList.Values)
            {
                if (Transactions.LockType == TransactionTypeLock.Waiting)
                {
                    if (retorno != "")
                    {
                        retorno += "\r\n";
                    }
                    retorno += "Transação " + Transactions.TransactionNumber;
                    TransactionQueueCommand filaComandos = WaitingTransactionsList[Transactions.TransactionNumber];
                    retorno += ": " + filaComandos.WaitingData;
                }
            }
            return retorno;
        }

        public String RetornaTransacoesAborteds()
        {
            String retorno = "";
            foreach (Transactions Transactions in TransactionsList.Values)
            {
                if (Transactions.LockType == TransactionTypeLock.Aborted)
                {
                    if (retorno == "")
                    {
                        retorno = "Transação " + Transactions.TransactionNumber;
                    }
                    else
                    {
                        retorno += "\r\n" + "Transação " + Transactions.TransactionNumber;
                    }
                }
            }
            return retorno;
        }

        public String RetornaQueue()
        {
            String retorno = "";

            foreach (TransactionQueueCommand comandosTransactions in WaitingTransactionsList.Values)
            {
                if (retorno != "")
                {
                    retorno += "\r\n";
                }
                retorno += "Transação: " + comandosTransactions.Transaction.TransactionNumber + "\r\n";
                string[] comandos = comandosTransactions.RetornaFilaComandos();
                foreach (String comando in comandos)
                {
                    retorno += comando + "\r\n";
                }
            }

            return retorno;
        }

        public String RetornaDataLock()
        {
            String retorno = "";
            foreach (Transactions Transactions in TransactionsList.Values)
            {
                string[] DadosUtilizados = Transactions.ReturnDataUsed();
                if (DadosUtilizados.Count() > 0)
                {
                    if (retorno != "")
                    {
                        retorno += "\r\n";
                    }
                    retorno += "Transação" + Transactions.TransactionNumber + ": ";
                    for (int i = 0; i < DadosUtilizados.Count(); i++)
                    {
                        retorno += Transactions.ReturnDataLockType(DadosUtilizados[i]) + "(" + DadosUtilizados[i] + ")";
                        if (i < DadosUtilizados.Count() - 1)
                        {
                            retorno += ",";
                        }
                    }
                }
            }
            return retorno;
        }
    }
}
