using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoBanco2
{
    
    class Transactions
    {
        public int Transaction { get; private set; }
        public Dictionary<string, TipoLockDado> DataUsed { get; private set; }
        public int ComandosExecutados { get; set; }
        public TransactionTypeLock LockType
        {
            get => LockType;
            set => LockType = value; 
        }

        public Transactions(int NumeroTransacao, TransactionTypeLock TipoLock)
        {
            Transaction = NumeroTransacao;
            DataUsed = new Dictionary<string, TipoLockDado>();

        }

        public void AdicionarDado(String Dado, TipoLockDado TipoLockDado)
        {
            if (!DataUsed.ContainsKey(Dado))
            {
                DataUsed.Add(Dado,TipoLockDado);
            }
        }

        public void RemoverDado(String Dado)
        {
            if (DataUsed.ContainsKey(Dado))
            {
                DataUsed.Remove(Dado);
            }
        }

        public String[] RetornaDadosUtilizados()
        {
            return DataUsed.Keys.ToArray<String>();
        }

        public TipoLockDado RetornaTipoLockDado(String Dado)
        {
            if (DataUsed.ContainsKey(Dado))
            {
                return DataUsed[Dado];
            }
            else
            {
                return TipoLockDado.Unlock;
            }
        }

        public void AlterarTipoLockDado(String Dado, TipoLockDado NovoTipoLockDado)
        {
            if (DataUsed.ContainsKey(Dado))
            {
                DataUsed[Dado] = NovoTipoLockDado;
            }
        }
    }

    enum TransactionTypeLock
    {
        // Transação que está executando
        Executing,
        // Transação que está esperando o dado utilizado por outra transação
        Waiting,
        // Transação que foi abortada
        Aborted
    }
}
