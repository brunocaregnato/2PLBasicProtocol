using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrabalhoBanco2
{
    public partial class Form1 : Form
    {
        private Protocol2PLBasic _protocolo2PBasico;
        public Form1()
        {
            InitializeComponent();
            tbCadastrar.Text = "w1[x]-r2[y]-w2[y]-c2-w1[y]-c1";
            _protocolo2PBasico = new Protocol2PLBasic();
        }

        private void BtnPassoAPasso_Click(object sender, EventArgs e)
        {
            _protocolo2PBasico.History(tbCadastrar.Text, tbSaida, tbFilaExecucao, tbTransEspera, tbTransAbortadas, tbFilaDeEspera, tbTransacaoXDadoLock);
        }

        private void BtnLimpar_Click(object sender, EventArgs e)
        {
            _protocolo2PBasico = new Protocol2PLBasic();
            tbSaida.Text = "";
            tbFilaExecucao.Text = "";
            tbTransEspera.Text = "";
            tbTransAbortadas.Text = "";
            tbFilaDeEspera.Text = "";
            tbTransacaoXDadoLock.Text = "";
        }
    }
}
