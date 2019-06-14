using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoBanco2
{
    class TransactionQueueCommand
    {
        public Transactions Transaction { get; private set; }
        public Queue<string> QueueCommands { get; private set; }

        public TransactionQueueCommand(Transactions Transacao)
        {
            Transaction = Transacao;
            QueueCommands = new Queue<string>();
        }

        public string WaitingData 
        {
            get
            {
                String primeiroComando = ConsultaComando();
                String dado = primeiroComando.Substring(3, 1);
                return dado;
            }

            set { }
        }

        public Transactions Transacao { get { return Transaction; } }

        public int ContComandos { get { return QueueCommands.Count; } }

        public void AdicionaComando(String Comando)
        {
            QueueCommands.Enqueue(Comando);
        }

        public String RemoveComando()
        {
            return QueueCommands.Dequeue();
        }

        public String ConsultaComando()
        {
            return QueueCommands.Peek();
        }

        public String[] RetornaFilaComandos()
        {
            return QueueCommands.ToArray<string>();
        }
    }
}
