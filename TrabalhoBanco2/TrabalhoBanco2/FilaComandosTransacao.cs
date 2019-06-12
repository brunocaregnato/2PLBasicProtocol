using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoBanco2
{
    class FilaComandosTransacao
    {
        private Transactions _transacao;
        private Queue<String> _filaComandos;

        public FilaComandosTransacao(Transactions Transacao)
        {
            _transacao = Transacao;
            _filaComandos = new Queue<string>();
        }

        public String DadoEmEspera 
        {
            get
            {
                String primeiroComando = ConsultaComando();
                String dado = primeiroComando.Substring(3, 1);
                return dado;
            }
        }

        public Transactions Transacao { get { return _transacao; } }

        public int ContComandos { get { return _filaComandos.Count; } }

        public void AdicionaComando(String Comando)
        {
            _filaComandos.Enqueue(Comando);
        }

        public String RemoveComando()
        {
            return _filaComandos.Dequeue();
        }

        public String ConsultaComando()
        {
            return _filaComandos.Peek();
        }

        public String[] RetornaFilaComandos()
        {
            return _filaComandos.ToArray<string>();
        }
    }
}
