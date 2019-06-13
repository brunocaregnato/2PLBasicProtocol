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
        private Dictionary<int, TransactionCommandQueue> WaitingTransactionsList { get; set; }

        public Protocolo2PLBasico()
        {
            TransactionsList = new Dictionary<int, Transactions>();

        }

        // exemplo: w1(x)-r2(y)-w2(y)-c2-w1(y)
        private void History(string entryTransaction, TextBox output)
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
                FilaEmExecucao.Text = RetornaFilaEmExecucao();
                TransEmEspera.Text = RetornaTransacoesEmEspera();
                TransAbortadas.Text = RetornaTransacoesAbortadas();
                FilaDeEspera.Text = RetornaFilaDeEspera();
                TransactionsXDadoLock.Text = RetornaTransactionsXDadoLock();
                PosicionaCursorFinal(OutPut);
                FilaEmExecucao.Refresh();
                TransEmEspera.Refresh();
                TransAbortadas.Refresh();
                TransactionsXDadoLock.Refresh();
                PosicionaCursorFinal(FilaDeEspera);
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
            foreach (int numeroTransactions in WaitingTransactionsList.Keys)
            {
                FilaComandosTransactions comandosTransactions = WaitingTransactionsList[numeroTransactions];
                if (comandosTransactions.Transactions.LockType != TipoLockTransactions.Abortada)
                {
                    String primeiroComando = comandosTransactions.ConsultaComando();
                    TipoLockDado tipoLockDado = RetornaTipoLock(primeiroComando.Substring(0, 1));

                    DisponibilidadeDado dispDado = RetornaDisponibilidadeDado(comandosTransactions.Transactions, tipoLockDado, comandosTransactions.DadoEmEspera);

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
                            comandosTransactions.Transactions.TipoLock = TipoLockTransactions.Executando;
                            // Adiciona à lista de excluídos para remover no final
                            excluidos.Add(numeroTransactions);
                            break;
                        case DisponibilidadeDado.DadoExclusiveOutraTransactions:
                            // Realiza validação para analisar se a transação entrou em deadlock
                            retorno = VerificaDeadLock(numeroTransactions, comandosTransactions.DadoEmEspera);
                            TransactionsEliminada = int.Parse(retorno.Substring(0, retorno.IndexOf("|")));
                            ciclo = retorno.Substring(retorno.IndexOf("|") + 1);
                            if (TransactionsEliminada > -1)
                            {
                                excluidos.Add(TransactionsEliminada);
                                // Desaloca dados da transação.
                                String msg = "";
                                Transactions Transactions = TransactionsList[TransactionsEliminada];
                                // Realiza unlock do dado e retira da lista de dados utilizados.
                                String[] dadosUtilizados = Transactions.RetornaDadosUtilizados();
                                msg = "************* DEAD LOCK DETECTADO ************* " + "\r\n";
                                msg += ciclo + "\r\n";
                                msg += "Transação " + TransactionsEliminada + " eliminada, pois foi a que executou menos comandos.\r\n";
                                AddOutPut(msg);
                                foreach (String dado in dadosUtilizados)
                                {
                                    // Adiciona saída unclok para dados liberados
                                    AddOutPut(GeraLock(TipoLockDado.Unlock, Transactions.NumeroTransactions, dado, Transactions.RetornaTipoLockDado(dado)));
                                    // Remove dado e transação da lista de dados locks
                                    RemoveDadoListaLocks(Transactions, dado);
                                    // Remove dado da lista de transações
                                    Transactions.RemoverDado(dado);
                                }
                                Transactions.TipoLock = TipoLockTransactions.Abortada;
                            }
                            break;
                    }
                }
            }
            foreach (int numeroTransactions in excluidos)
            {
                WaitingTransactionsList.Remove(numeroTransactions);
            }
        }

        private string VerificaDeadLock(int NumeroTransactions, String Dado)
        {
            List<int> transacoesEsperando = new List<int>();
            transacoesEsperando.Add(NumeroTransactions);
            return VerificaDeadLock(NumeroTransactions, Dado, transacoesEsperando);
        }

        private string VerificaDeadLock(int NumeroTransactions, String Dado, List<int> TransacoesEsperando)
        {
            int TransactionsEliminada = -1;
            string retorno = "-1|x";
            DataLockTransactions TransacoesUsandoDado;
            // Se não existir o dado é pq a transação já foi abortada
            if (TransactionDataLock.ContainsKey(Dado))
            {
                TransacoesUsandoDado = TransactionDataLock[Dado];
                foreach (int numeroTransactions in TransacoesUsandoDado.RetornaTransacoes())
                {
                    // Não verifica a própria transação
                    if (numeroTransactions != NumeroTransactions)
                    {
                        // Adiciona na lista e verifica se ocorreu dead lock.
                        if (TransactionsList[numeroTransactions].TipoLock == TipoLockTransactions.Esperando)
                        {
                            if (TransacoesEsperando.Contains(numeroTransactions))
                            {
                                // Elimina dead lock
                                TransactionsEliminada = EliminaDeadLock(TransacoesEsperando);
                                retorno = TransactionsEliminada + "|" + RetornaCicloTransacoes(TransacoesEsperando, TransactionsEliminada);
                            }
                            else
                            {
                                TransacoesEsperando.Add(numeroTransactions);
                                // Retorna dado que a transação está esperando.
                                FilaComandosTransactions ComandosTransactions = WaitingTransactionsList[numeroTransactions];
                                // Verifica pendencias desta transação.
                                retorno = VerificaDeadLock(numeroTransactions, ComandosTransactions.DadoEmEspera, TransacoesEsperando);
                            }
                        }
                    }
                }
            }
            return retorno;
        }

        public String RetornaCicloTransacoes(List<int> transactionList, int NumeroTransactions)
        {
            string ciclo = "";
            foreach (int numeroTransactions in transactionList)
            {
                ciclo += "t" + numeroTransactions + "=>";
            }
            ciclo += "t" + NumeroTransactions;
            return ciclo;
        }

        public int EliminaDeadLock(List<int> TransacoesEsperando)
        {
            int TransactionsEliminada = 0;
            int menorNumeroExecucoes = 999999999;
            Transactions Transactions;
            foreach (int numeroTransactions in TransacoesEsperando)
            {
                Transactions = TransactionsList[numeroTransactions];
                if (Transactions.ComandosExecutados < menorNumeroExecucoes)
                {
                    menorNumeroExecucoes = Transactions.ComandosExecutados;
                    TransactionsEliminada = numeroTransactions;
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

        private DisponibilidadeDado RetornaDisponibilidadeDado(Transactions Transactions, TipoLockDado TipoLockDado, String Dado)
        {
            // Verifica se o dado está lockado para alguma sessão
            if (TransactionDataLock.ContainsKey(Dado))
            {
                DataLockTransactions dadoUtilizado = TransactionDataLock[Dado];
                // Verifica se está lockado para a transação que está tentando acessar.
                if (dadoUtilizado.VerificaLockParaTransactions(Transactions.NumeroTransactions))
                {
                    // Se o tipo de dado for estrita deve ter lock exclusivo, neste caso
                    // não pode estar lockado para outras transações
                    if (TipoLockDado == TipoLockDado.Exclusive && dadoUtilizado.NumeroDeTransacoes() > 1)
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
                    if (dadoUtilizado.TipoLock == TrabalhoBanco2.TipoLockDado.Exclusive || TipoLockDado == TipoLockDado.Exclusive)
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
            TipoLockDado tipoLockDado = RetornaTipoLock(tipoComando);
            // Extrai número da transação
            int numeroTransactions = int.Parse(Comando.Substring(1, 1));
            // Adiciona transação com status inicial executando
            Transactions = AdicionaTransactions(numeroTransactions, TipoLockTransactions.Executando);
            // Extrai nome do dado
            string dado = "";
            if (tipoLockDado != TipoLockDado.Commit)
            {
                dado = Comando.Substring(3, 1);
            }
            // Tratamento para comando conforme o status da transação.
            if (Transactions.TipoLock == TipoLockTransactions.Executando)
            {
                TratamentoTransactionsExecutando(tipoLockDado, dado, Transactions, Comando);
            }
            // Quando a transação estiver esperando, adiciona o comando à fila de espera
            else if (Transactions.TipoLock == TipoLockTransactions.Esperando)
            {
                AdicionarComandoFilaEspera(Transactions, Comando);
            }
            else
            {
                AddOutPut("Transação " + Transactions.NumeroTransactions + " abortada. Comando " + Comando + " ignorado.");
            }
        }

        public void AdicionarComandoFilaEspera(Transactions Transactions, String Comando)
        {
            // Adiciona a transação à fila quando ainda não existir
            FilaComandosTransactions filaComandosTransactions = null;
            if (!WaitingTransactionsList.ContainsKey(Transactions.NumeroTransactions))
            {
                Transactions.TipoLock = TipoLockTransactions.Esperando;
                filaComandosTransactions = new FilaComandosTransactions(Transactions);
                WaitingTransactionsList.Add(Transactions.NumeroTransactions, filaComandosTransactions);
            }
            // Retorna a fila de comandos da transação.
            else
            {
                filaComandosTransactions = WaitingTransactionsList[Transactions.NumeroTransactions];
            }
            // Adiciona comando à fila de espera da transação.
            filaComandosTransactions.AdicionaComando(Comando);
            AddOutPut("Comando: " + Comando + " adicionado à fila de espera.");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TipoLockDado">Utilizado pra indicar qual o tipo de lock está sobre o dado</param>
        /// <param name="Dado">Dado sendo utilizado pela transação</param>
        /// <param name="Transactions">Transação que executou o comando</param>
        /// <param name="Comando">Comando executado pela transação</param>
        public void TratamentoTransactionsExecutando(TipoLockDado TipoLockDado, String Dado, Transactions Transactions, String Comando)
        {
            DataLockTransactions dadoUtilizado;
            // Verifica se o dado está disponível
            if (TipoLockDado == TipoLockDado.Commit)
            {
                // Quando for commit, verifica se o status da transação esta esperando.
                // se não estiver percorre a lista de dados lockadas pela transação e libera todas
                // e dispara o commit
                // Se estiver esperando adiciona o comnado commit a lista de espera
                if (Transactions.TipoLock == TipoLockTransactions.Esperando)
                {
                    AdicionarComandoFilaEspera(Transactions, Comando);
                }
                // libera lock dados e realiza commit.
                else if (Transactions.TipoLock != TipoLockTransactions.Abortada)
                {
                    // Realiza unlock do dado e retira da lista de dados utilizados.
                    String[] dadosUtilizados = Transactions.RetornaDadosUtilizados();
                    foreach (String dado in dadosUtilizados)
                    {
                        // Adiciona saída unclok para dados liberados
                        AddOutPut(GeraLock(TipoLockDado.Unlock, Transactions.NumeroTransactions, dado, Transactions.RetornaTipoLockDado(dado)));
                        // Remove dado e transação da lista de dados locks
                        RemoveDadoListaLocks(Transactions, dado);
                        // Remove dado da lista de transações
                        Transactions.RemoverDado(dado);
                    }
                    // Incrementa comandos executados da transação
                    Transactions.ComandosExecutados++;
                    // Adiciona saída para commit.
                    AddOutPut(Comando);
                }
            }
            // Verifica disponibidade do dado para a transação e comando
            else
            {
                //DadoLockTransactions dadoUtilizado;
                DisponibilidadeDado dispDado = RetornaDisponibilidadeDado(Transactions, TipoLockDado, Dado);
                // Dado não foi lockado por nenhuma transação
                if (dispDado == DisponibilidadeDado.DadoDisponivel)
                {
                    dadoUtilizado = new DataLockTransactions(Dado, TipoLockDado);
                    TransactionDataLock.Add(Dado, dadoUtilizado);
                    TransactionDataLock[Dado].AdicionaTransactions(Transactions);
                    Transactions.AdicionarDado(Dado, TipoLockDado);
                    // Adiciona lock.
                    AddOutPut(GeraLock(TipoLockDado, Transactions.NumeroTransactions, Dado, null));
                    // Incrementa comandos executados da transação
                    Transactions.ComandosExecutados++;
                    // Adiciona comando lido
                    AddOutPut(Comando);
                }
                // Dado já esta lockado para a transação, mas como shared
                else if (dispDado == DisponibilidadeDado.DadoSharedTransactions)
                {
                    dadoUtilizado = TransactionDataLock[Dado];
                    // Faz upgrade no lock se necessário.
                    if (UpgradeLock(dadoUtilizado, TipoLockDado, Transactions))
                    {
                        AddOutPut(GeraLock(TipoLockDado, Transactions.NumeroTransactions, Dado, null));
                    }
                    // Incrementa comandos executados da transação
                    Transactions.ComandosExecutados++;
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
                    Transactions.AdicionarDado(Dado, TipoLockDado);
                    TransactionDataLock[Dado].AdicionaTransactions(Transactions);
                    // Adiciona lock.
                    AddOutPut(GeraLock(TipoLockDado, Transactions.NumeroTransactions, Dado, null));
                    // Incrementa comandos executados da transação
                    Transactions.ComandosExecutados++;
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
        public void TratamentoTransactionsExecutando_old(TipoLockDado TipoLockDado, String Dado, Transactions Transactions, String Comando)
        {
            DataLockTransactions dadoUtilizado;
            // Verifica se o dado está disponível
            if (TipoLockDado == TipoLockDado.Commit)
            {
                // Quando for commit, verifica se o status da transação esta esperando.
                // se não estiver percorre a lista de dados lockadas pela transação e libera todas
                // e dispara o commit
                // Se estiver esperando adiciona o comnado commit a lista de espera
                //w1(x)-r2(y)-w2(y)-c2-w1(y)-c1
                if (Transactions.TipoLock == TipoLockTransactions.Esperando)
                {
                    AdicionarComandoFilaEspera(Transactions, Comando);
                }
                // libera lock dados e realiza commit.
                else
                {
                    // Realiza unlock do dado e retira da lista de dados utilizados.
                    String[] dadosUtilizados = Transactions.RetornaDadosUtilizados();
                    foreach (String dado in dadosUtilizados)
                    {
                        // Adiciona saída unclok para dados liberados
                        AddOutPut(GeraLock(TipoLockDado.Unlock, Transactions.NumeroTransactions, dado, Transactions.RetornaTipoLockDado(dado)));
                        // Remove dado e transação da lista de dados locks
                        RemoveDadoListaLocks(Transactions, dado);
                        // Remove dado da lista de transações
                        Transactions.RemoverDado(dado);
                    }
                    // Incrementa comandos executados da transação
                    Transactions.ComandosExecutados++;
                    // Adiciona saída para commit.
                    AddOutPut(Comando);
                }
            }
            else if (TransactionDataLock.ContainsKey(Dado))
            {
                dadoUtilizado = dadoUtilizado = TransactionDataLock[Dado];
                // Verifica se está lockado para a transação que está tentando acessar.
                if (dadoUtilizado.VerificaLockParaTransactions(Transactions.NumeroTransactions))
                {
                    // Se o tipo de dado for estrita deve ter lock exclusivo, neste caso
                    // não pode estar lockado para outras transações
                    if (TipoLockDado == TipoLockDado.Exclusive && dadoUtilizado.NumeroDeTransacoes() > 1)
                    {
                        // deve adicionar na lista de espera.
                        AdicionarComandoFilaEspera(Transactions, Comando);
                    }
                    else
                    {
                        // Faz upgrade no lock se necessário.
                        if (UpgradeLock(dadoUtilizado, TipoLockDado, Transactions))
                        {
                            AddOutPut(GeraLock(TipoLockDado, Transactions.NumeroTransactions, Dado, null));
                        }
                        // Incrementa comandos executados da transação
                        Transactions.ComandosExecutados++;
                        // Adiciona comando lido
                        AddOutPut(Comando);
                    }
                }
                // Verifica se tem lock exclusivo para outra sessão.
                else
                {
                    // Adiciona à lista de espera quando estiver lock exclusivo para outra transação
                    // Ou quando a transação pediu lock exclusivo
                    if (dadoUtilizado.TipoLock == TrabalhoBanco2.TipoLockDado.Exclusive || TipoLockDado == TipoLockDado.Exclusive)
                    {
                        AdicionarComandoFilaEspera(Transactions, Comando);
                    }
                    else
                    {
                        // Adiciona transação a lista de dados lockados
                        TransactionDataLock.Add(Dado, dadoUtilizado);
                        // Adiciona o dado à lista de dados da transação
                        Transactions.AdicionarDado(Dado, TipoLockDado);
                        // Adiciona lock.
                        AddOutPut(GeraLock(TipoLockDado, Transactions.NumeroTransactions, Dado, null));
                        // Incrementa comandos executados da transação
                        Transactions.ComandosExecutados++;
                        // Adiciona comando lido
                        AddOutPut(Comando);
                    }
                }
            }
            else
            {
                // Quando o tipo de dado não existe na lista de lock
                // Adiciona o dado e gera a saída com lock
                dadoUtilizado = new DataLockTransactions(Dado, TipoLockDado);
                TransactionDataLock.Add(Dado, dadoUtilizado);
                TransactionDataLock[Dado].AdicionaTransactions(Transactions);
                Transactions.AdicionarDado(Dado, TipoLockDado);
                // Adiciona lock.
                AddOutPut(GeraLock(TipoLockDado, Transactions.NumeroTransactions, Dado, null));
                // Incrementa comandos executados da transação
                Transactions.ComandosExecutados++;
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
        private Transactions AdicionaTransactions(int NumeroTransactions, TipoLockTransactions LockTransactions)
        {
            Transactions Transactions = null;
            if (!TransactionsList.ContainsKey(NumeroTransactions))
            {
                Transactions = new Transactions(NumeroTransactions, LockTransactions);
                TransactionsList.Add(NumeroTransactions, Transactions);
            }
            else
            {
                Transactions = TransactionsList[NumeroTransactions];
            }
            return Transactions;
        }

        private TipoLockDado RetornaTipoLock(String Tipo)
        {
            TipoLockDado tipoDadoLock = TipoLockDado.Unlock;
            if (Tipo == "w")
            {
                tipoDadoLock = TipoLockDado.Exclusive;
            }
            else if (Tipo == "r")
            {
                tipoDadoLock = TipoLockDado.Shared;
            }
            else if (Tipo == "c")
            {
                tipoDadoLock = TipoLockDado.Commit;
            }
            return tipoDadoLock;
        }

        private string GeraLock(TipoLockDado TipoLockDado, int NumeroTransactions, String Dado, TipoLockDado? TipoLockDadoUnlock)
        {
            string OutPut;
            // formata tipo de lock
            if (TipoLockDado == TipoLockDado.Unlock)
            {
                OutPut = "u";
                if (TipoLockDadoUnlock == TipoLockDado.Exclusive)
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
                if (TipoLockDado == TipoLockDado.Exclusive)
                {
                    OutPut += "x";
                }
                else
                {
                    OutPut += "h";
                }
            }
            // adiciona o número da transação ao lock
            OutPut += NumeroTransactions;
            OutPut += "[" + Dado + "]";
            return OutPut;
        }

        private void AddOutPut(String OutPut)
        {
            if (OutPut != "")
            {
                if (_OutPut.Text == "")
                {
                    _OutPut.Text = OutPut;
                }
                else
                {
                    _OutPut.Text += "\r\n" + OutPut;
                }
            }
        }

        private bool UpgradeLock(DataLockTransactions Dado, TipoLockDado TipoLock, Transactions Transactions)
        {
            if (TipoLock == TipoLockDado.Exclusive && Dado.TipoLock != TipoLockDado.Exclusive)
            {
                Dado.TipoLock = TipoLockDado.Exclusive;
                Transactions.AlterarTipoLockDado(Dado.Dado, TipoLockDado.Exclusive);
                return true;
            }
            return false;
        }

        public Protocolo2PLBasico()
        {
            TransactionsList = new Dictionary<int, Transactions>();
            TransactionDataLock = new Dictionary<string, DataLockTransactions>();
            WaitingTransactionsList = new Dictionary<int, FilaComandosTransactions>();
        }

        public String RetornaFilaEmExecucao()
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
                if (Transactions.TipoLock == TipoLockTransactions.Esperando)
                {
                    if (retorno != "")
                    {
                        retorno += "\r\n";
                    }
                    retorno += "Transação " + Transactions.NumeroTransactions;
                    FilaComandosTransactions filaComandos = WaitingTransactionsList[Transactions.NumeroTransactions];
                    retorno += ": " + filaComandos.DadoEmEspera;
                }
            }
            return retorno;
        }

        public String RetornaTransacoesAbortadas()
        {
            String retorno = "";
            foreach (Transactions Transactions in TransactionsList.Values)
            {
                if (Transactions.TipoLock == TipoLockTransactions.Abortada)
                {
                    if (retorno == "")
                    {
                        retorno = "Transação " + Transactions.NumeroTransactions;
                    }
                    else
                    {
                        retorno += "\r\n" + "Transação " + Transactions.NumeroTransactions;
                    }
                }
            }
            return retorno;
        }

        public String RetornaFilaDeEspera()
        {
            String retorno = "";

            foreach (FilaComandosTransactions comandosTransactions in WaitingTransactionsList.Values)
            {
                if (retorno != "")
                {
                    retorno += "\r\n";
                }
                retorno += "Transação: " + comandosTransactions.Transactions.NumeroTransactions + "\r\n";
                string[] comandos = comandosTransactions.RetornaFilaComandos();
                foreach (String comando in comandos)
                {
                    retorno += comando + "\r\n";
                }
            }

            return retorno;
        }

        public String RetornaTransactionsXDadoLock()
        {
            String retorno = "";
            foreach (Transactions Transactions in TransactionsList.Values)
            {
                string[] DadosUtilizados = Transactions.RetornaDadosUtilizados();
                if (DadosUtilizados.Count() > 0)
                {
                    if (retorno != "")
                    {
                        retorno += "\r\n";
                    }
                    retorno += "Transação" + Transactions.NumeroTransactions + ": ";
                    for (int i = 0; i < DadosUtilizados.Count(); i++)
                    {
                        retorno += Transactions.RetornaTipoLockDado(DadosUtilizados[i]) + "(" + DadosUtilizados[i] + ")";
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
