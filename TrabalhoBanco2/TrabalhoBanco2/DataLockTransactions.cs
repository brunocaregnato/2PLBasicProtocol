using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoBanco2
{
    enum TipoLockDado
	{
        Shared,
        Exclusive,
        Unlock,
        Commit
	}
    class DadoLockTransacao
    {
        private String _dado;
        private Dictionary<int, Transactions> _listaTransacoes;
        private TipoLockDado _tipoLock;

        public String Dado { get { return _dado; } }

        public TipoLockDado TipoLock {
            get
            {
                return _tipoLock;
            }
            set
            {
                _tipoLock = value;
            }
        }

        public DadoLockTransacao(String Dado, TipoLockDado LockDado)
        {
            _listaTransacoes = new Dictionary<int, Transactions>();
            _tipoLock = LockDado;
            _dado = Dado;
        }

        public void AdicionaTransacao(Transactions Transacao)
        {
            _listaTransacoes.Add(Transacao.NumeroTransacao, Transacao);
        }

        public void RemoveTransacao(Transactions Transacao)
        {
            // Verifica se a transação possui o dado em lock
            if (_listaTransacoes.ContainsKey(Transacao.NumeroTransacao))
            {
                // Verifica se o tipo de lock é exclusivo
                TipoLockDado tipoLockDado = Transacao.RetornaTipoLockDado(_dado);
                if (tipoLockDado == TipoLockDado.Exclusive)
                {
                    // Quando uma transação que possui um lock exclusivo é removida deve alterar o status do dado
                    // para compartilhado
                    _tipoLock = TipoLockDado.Shared;
                }
                _listaTransacoes.Remove(Transacao.NumeroTransacao);
            }
        }

        public bool VerificaLockParaTransacao(int NumeroTransacao)
        {
            if (_listaTransacoes.ContainsKey(NumeroTransacao))
            {
                return true;
            }
            return false;
        }

        public int NumeroDeTransacoes()
        {
            return _listaTransacoes.Count;
        }

        public Transactions RetornaTransacao(int NumeroTransacao)
        {
            if (VerificaLockParaTransacao(NumeroTransacao))
            {
                return _listaTransacoes[NumeroTransacao];
            }
            return null;
        }

        public int[] RetornaTransacoes()
        {
            return _listaTransacoes.Keys.ToArray<int>();
        }
    }
}
