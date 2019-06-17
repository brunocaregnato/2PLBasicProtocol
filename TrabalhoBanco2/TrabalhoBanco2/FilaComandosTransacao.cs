using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoBanco2
{
    class FilaCommandsTransaction
    {
        private Transactions _Transaction;
        private Queue<String> _filaCommands;

        public FilaCommandsTransaction(Transactions Transaction)
        {
            _Transaction = Transaction;
            _filaCommands = new Queue<string>();
        }

        public String DadoEmEspera 
        {
            get
            {
                String firstCommand = CheckCommand();
                String dado = firstCommand.Substring(3, 1);
                return dado;
            }
        }

        public Transactions Transaction { get { return _Transaction; } }

        public int ContCommands { get { return _filaCommands.Count; } }

        public void AdicionaCommand(String Command)
        {
            _filaCommands.Enqueue(Command);
        }

        public String RemoveCommand()
        {
            return _filaCommands.Dequeue();
        }

        public String CheckCommand()
        {
            return _filaCommands.Peek();
        }

        public String[] RetornaFilaCommands()
        {
            return _filaCommands.ToArray<string>();
        }
    }
}
