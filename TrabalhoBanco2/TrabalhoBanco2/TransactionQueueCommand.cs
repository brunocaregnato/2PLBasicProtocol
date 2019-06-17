using System.Collections.Generic;
using System.Linq;

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
                string primeiroComando = CheckCommand();
                string dado = primeiroComando.Substring(3, 1);
                return dado;
            }

            set { }
        }

        public void AddComamand(string Comando)
        {
            QueueCommands.Enqueue(Comando);
        }

        public string RemoveCommand()
        {
            return QueueCommands.Dequeue();
        }

        public string CheckCommand()
        {
            return QueueCommands.Peek();
        }

        public string[] ReturnQueueCommand()
        {
            return QueueCommands.ToArray<string>();
        }
    }
}
