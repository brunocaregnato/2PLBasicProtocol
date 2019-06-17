using System.Collections.Generic;
using System.Linq;

namespace TrabalhoBanco2
{
    enum TransactionTypeLock
    {
        // Transação que está executando
        Executing,
        // Transação que está esperando o dado utilizado por outra transação
        Waiting,
        // Transação que foi abortada
        Aborted
    }

    class Transactions
    {
        public int TransactionNumber { get; private set; }
        public Dictionary<string, LockDataType> DataUsed { get; private set; }
        public int ExecutedCommands { get; set; }
        public TransactionTypeLock LockType { get; set; }

        public Transactions(int NumeroTransaction, TransactionTypeLock TipoLock)
        {
            TransactionNumber = NumeroTransaction;
            DataUsed = new Dictionary<string, LockDataType>();

        }

        public void AddData(string Dado, LockDataType LockDataType)
        {
            if (!DataUsed.ContainsKey(Dado))
                DataUsed.Add(Dado,LockDataType);
            
        }

        public void RemoveData(string Dado)
        {
            if (DataUsed.ContainsKey(Dado))
                DataUsed.Remove(Dado);            
        }

        public string[] ReturnDataUsed()
        {
            return DataUsed.Keys.ToArray();
        }

        public LockDataType ReturnDataLockType(string Dado)
        {
            if (DataUsed.ContainsKey(Dado))
                return DataUsed[Dado];
            
            else
                return LockDataType.Unlock;
        }

        public void ChangeDataLockType(string Dado, LockDataType NovoLockDataType)
        {
            if (DataUsed.ContainsKey(Dado))
                DataUsed[Dado] = NovoLockDataType;
            
        }
    }
}