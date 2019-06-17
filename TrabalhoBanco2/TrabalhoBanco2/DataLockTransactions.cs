using System.Collections.Generic;
using System.Linq;

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
        private readonly Dictionary<int, Transactions> _transactionList;
        public string Data { get; private set; }
        public LockDataType LockType { get; set; }

        public DataLockTransactions(string data, LockDataType lockDado)
        {
            _transactionList = new Dictionary<int, Transactions>();
            LockType = lockDado;
            Data = data;
        }

        public void AddTransaction(Transactions Transaction)
        {
            _transactionList.Add(Transaction.TransactionNumber, Transaction);
        }

        public void RemoveTransaction(Transactions Transaction)
        {
            // Verifica se a transação possui o dado em lock
            if (_transactionList.ContainsKey(Transaction.TransactionNumber))
            {
                // Verifica se o tipo de lock é exclusivo
                LockDataType LockDataType = Transaction.ReturnDataLockType(Data);

                /* Quando uma transação que possui um lock exclusivo é removida deve alterar o status do dado
                   para compartilhado */
                if (LockDataType.Equals(LockDataType.Exclusive))
                    LockType = LockDataType.Shared;
                
                _transactionList.Remove(Transaction.TransactionNumber);
            }
        }

        public bool CheckLockForTransaction(int TransactionNumber)
        {
            if (_transactionList.ContainsKey(TransactionNumber))
                return true;
            
            return false;
        }

        public int NumberOfTransactions()
        {
            return _transactionList.Count;
        }

        public int[] ReturnTransactions()
        {
            return _transactionList.Keys.ToArray();
        }
    }
}
