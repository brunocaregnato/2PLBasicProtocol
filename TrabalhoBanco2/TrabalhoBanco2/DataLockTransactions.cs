using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoBanco2
{
    enum LockDataType
	{
        Shared,
        Exclusive,
        Unlock,
        Commit
	}
    class DataLockTransactions
    {
        private String _dado;
        private Dictionary<int, Transactions> _listaTransacoes;
        private LockDataType _tipoLock;

        public String Dado { get { return _dado; } }

        public LockDataType TipoLock {
            get
            {
                return _tipoLock;
            }
            set
            {
                _tipoLock = value;
            }
        }

        public DataLockTransactions(String Dado, LockDataType LockDado)
        {
            _listaTransacoes = new Dictionary<int, Transactions>();
            _tipoLock = LockDado;
            _dado = Dado;
        }

        public void AdicionaTransacao(Transactions Transacao)
        {
            _listaTransacoes.Add(Transacao.TransactionNumber, Transacao);
        }

        public void RemoveTransacao(Transactions Transacao)
        {
            // Verifica se a transação possui o dado em lock
            if (_listaTransacoes.ContainsKey(Transacao.TransactionNumber))
            {
                // Verifica se o tipo de lock é exclusivo
                LockDataType LockDataType = Transacao.ReturnDataLockType(_dado);
                if (LockDataType == LockDataType.Exclusive)
                {
                    // Quando uma transação que possui um lock exclusivo é removida deve alterar o status do dado
                    // para compartilhado
                    _tipoLock = LockDataType.Shared;
                }
                _listaTransacoes.Remove(Transacao.TransactionNumber);
            }
        }

        public bool VerificaLockParaTransacao(int TransactionNumber)
        {
            if (_listaTransacoes.ContainsKey(TransactionNumber))
            {
                return true;
            }
            return false;
        }

        public int NumeroDeTransacoes()
        {
            return _listaTransacoes.Count;
        }

        public Transactions RetornaTransacao(int TransactionNumber)
        {
            if (VerificaLockParaTransacao(TransactionNumber))
            {
                return _listaTransacoes[TransactionNumber];
            }
            return null;
        }

        public int[] RetornaTransacoes()
        {
            return _listaTransacoes.Keys.ToArray<int>();
        }
    }
}
