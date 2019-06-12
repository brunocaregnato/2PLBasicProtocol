using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TrabalhoBanco2
{
    /// <summary>
    /// Define o tipo de execução utilizado
    /// </summary>
    class Protocolo2PLBasico
    {

        private Dictionary<int, Transactions> _transactions;

        public Protocolo2PLBasico()
        {
            _transactions = new Dictionary<int, Transactions>();
        }
    }
}
