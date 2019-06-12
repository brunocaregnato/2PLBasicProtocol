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
        private Protocolo2PLBasico _protocolo2PBasico;
        public Form1()
        {
            InitializeComponent();
            tbCadastrar.Text = "w1[x]-r2[y]-w2[y]-c2-w1[y]-c1";
            _protocolo2PBasico = new Protocolo2PLBasico();
            
            
        }

        private void btnExecutar_Click(object sender, EventArgs e)
        {
            _protocolo2PBasico = new Protocolo2PLBasico();
           // _protocolo2PBasico.AnalisarHistoria(tbCadastrar.Text, tbSaida,tbFilaExecucao,tbTransEspera,tbTransAbortadas,tbFilaDeEspera,tbTransacaoXDadoLock, (int)nudTempoExecucao.Value);
        }

        private void btnPassoAPasso_Click(object sender, EventArgs e)
        {
            //_protocolo2PBasico.AnalisarHistoria(tbCadastrar.Text, tbSaida, tbFilaExecucao,tbTransEspera, tbTransAbortadas,tbFilaDeEspera,tbTransacaoXDadoLock);
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            _protocolo2PBasico = new Protocolo2PLBasico();
        }
    }
}
