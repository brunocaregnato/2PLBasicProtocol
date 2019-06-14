using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoBanco2
{
    
    class Transactions
    {
        public int TransactionNumber { get; private set; }
        public Dictionary<string, LockDataType> DataUsed { get; private set; }
        public int ExecutedCommands { get; set; }
        public TransactionTypeLock LockType
        {
            get => LockType;
            set => LockType = value; 
        }

        public Transactions(int NumeroTransacao, TransactionTypeLock TipoLock)
        {
            TransactionNumber = NumeroTransacao;
            DataUsed = new Dictionary<string, LockDataType>();

        }

        public void AddData(string Dado, LockDataType LockDataType)
        {
            if (!DataUsed.ContainsKey(Dado))
            {
                DataUsed.Add(Dado,LockDataType);
            }
        }

        public void RemoveData(String Dado)
        {
            if (DataUsed.ContainsKey(Dado))
            {
                DataUsed.Remove(Dado);
            }
        }

        public String[] ReturnDataUsed()
        {
            return DataUsed.Keys.ToArray<String>();
        }

        public LockDataType ReturnDataLockType(String Dado)
        {
            if (DataUsed.ContainsKey(Dado))
            {
                return DataUsed[Dado];
            }
            else
            {
                return LockDataType.Unlock;
            }
        }

        public void ChangeDataLockType(String Dado, LockDataType NovoLockDataType)
        {
            if (DataUsed.ContainsKey(Dado))
            {
                DataUsed[Dado] = NovoLockDataType;
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
