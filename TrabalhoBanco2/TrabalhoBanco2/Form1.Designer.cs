namespace TrabalhoBanco2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tbTransacaoXDadoLock = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbFilaDeEspera = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbTransAbortadas = new System.Windows.Forms.TextBox();
            this.tbTransEspera = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbFilaExecucao = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.nudTempoExecucao = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnLimpar = new System.Windows.Forms.Button();
            this.btnPassoAPasso = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tbSaida = new System.Windows.Forms.TextBox();
            this.tbCadastrar = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTempoExecucao)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(91, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(838, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Simulação de um gerente de controle de concorrência utilizando protocolo 2PL Bási" +
    "co";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.tbTransacaoXDadoLock);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.tbFilaDeEspera);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.tbTransAbortadas);
            this.groupBox1.Controls.Add(this.tbTransEspera);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbFilaExecucao);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.nudTempoExecucao);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnLimpar);
            this.groupBox1.Controls.Add(this.btnPassoAPasso);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbSaida);
            this.groupBox1.Controls.Add(this.tbCadastrar);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(5, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(1293, 553);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(1, 53);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 20);
            this.label2.TabIndex = 24;
            this.label2.Text = "História";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(976, 329);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(218, 20);
            this.label10.TabIndex = 23;
            this.label10.Text = "Transação x Dados Lock";
            // 
            // tbTransacaoXDadoLock
            // 
            this.tbTransacaoXDadoLock.BackColor = System.Drawing.Color.White;
            this.tbTransacaoXDadoLock.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTransacaoXDadoLock.Location = new System.Drawing.Point(908, 352);
            this.tbTransacaoXDadoLock.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbTransacaoXDadoLock.Multiline = true;
            this.tbTransacaoXDadoLock.Name = "tbTransacaoXDadoLock";
            this.tbTransacaoXDadoLock.ReadOnly = true;
            this.tbTransacaoXDadoLock.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbTransacaoXDadoLock.Size = new System.Drawing.Size(375, 159);
            this.tbTransacaoXDadoLock.TabIndex = 22;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(733, 151);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(131, 20);
            this.label9.TabIndex = 21;
            this.label9.Text = "Fila de Espera";
            // 
            // tbFilaDeEspera
            // 
            this.tbFilaDeEspera.BackColor = System.Drawing.Color.White;
            this.tbFilaDeEspera.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbFilaDeEspera.Location = new System.Drawing.Point(715, 175);
            this.tbFilaDeEspera.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbFilaDeEspera.Multiline = true;
            this.tbFilaDeEspera.Name = "tbFilaDeEspera";
            this.tbFilaDeEspera.ReadOnly = true;
            this.tbFilaDeEspera.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbFilaDeEspera.Size = new System.Drawing.Size(183, 340);
            this.tbFilaDeEspera.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(1107, 151);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(153, 20);
            this.label8.TabIndex = 19;
            this.label8.Text = "Trans. Abortadas";
            // 
            // tbTransAbortadas
            // 
            this.tbTransAbortadas.BackColor = System.Drawing.Color.White;
            this.tbTransAbortadas.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTransAbortadas.Location = new System.Drawing.Point(1100, 175);
            this.tbTransAbortadas.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbTransAbortadas.Multiline = true;
            this.tbTransAbortadas.Name = "tbTransAbortadas";
            this.tbTransAbortadas.ReadOnly = true;
            this.tbTransAbortadas.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbTransAbortadas.Size = new System.Drawing.Size(183, 147);
            this.tbTransAbortadas.TabIndex = 18;
            // 
            // tbTransEspera
            // 
            this.tbTransEspera.BackColor = System.Drawing.Color.White;
            this.tbTransEspera.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTransEspera.Location = new System.Drawing.Point(908, 175);
            this.tbTransEspera.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbTransEspera.Multiline = true;
            this.tbTransEspera.Name = "tbTransEspera";
            this.tbTransEspera.ReadOnly = true;
            this.tbTransEspera.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbTransEspera.Size = new System.Drawing.Size(183, 147);
            this.tbTransEspera.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(913, 151);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(158, 20);
            this.label7.TabIndex = 16;
            this.label7.Text = "Trans. em Espera";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(529, 151);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(153, 20);
            this.label6.TabIndex = 14;
            this.label6.Text = "Fila de Execução";
            // 
            // tbFilaExecucao
            // 
            this.tbFilaExecucao.BackColor = System.Drawing.Color.White;
            this.tbFilaExecucao.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbFilaExecucao.Location = new System.Drawing.Point(523, 175);
            this.tbFilaExecucao.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbFilaExecucao.Multiline = true;
            this.tbFilaExecucao.Name = "tbFilaExecucao";
            this.tbFilaExecucao.ReadOnly = true;
            this.tbFilaExecucao.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbFilaExecucao.Size = new System.Drawing.Size(183, 340);
            this.tbFilaExecucao.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(267, 523);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 20);
            this.label5.TabIndex = 12;
            this.label5.Text = "segundos";
            // 
            // nudTempoExecucao
            // 
            this.nudTempoExecucao.Location = new System.Drawing.Point(219, 523);
            this.nudTempoExecucao.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudTempoExecucao.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudTempoExecucao.Name = "nudTempoExecucao";
            this.nudTempoExecucao.Size = new System.Drawing.Size(40, 22);
            this.nudTempoExecucao.TabIndex = 11;
            this.nudTempoExecucao.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 523);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(182, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "Tempo Exec. Comando";
            // 
            // btnLimpar
            // 
            this.btnLimpar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpar.Location = new System.Drawing.Point(865, 519);
            this.btnLimpar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLimpar.Name = "btnLimpar";
            this.btnLimpar.Size = new System.Drawing.Size(131, 28);
            this.btnLimpar.TabIndex = 8;
            this.btnLimpar.Text = "Limpar";
            this.btnLimpar.UseVisualStyleBackColor = true;
            this.btnLimpar.Click += new System.EventHandler(this.btnLimpar_Click);
            // 
            // btnPassoAPasso
            // 
            this.btnPassoAPasso.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPassoAPasso.Location = new System.Drawing.Point(1009, 519);
            this.btnPassoAPasso.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPassoAPasso.Name = "btnPassoAPasso";
            this.btnPassoAPasso.Size = new System.Drawing.Size(131, 28);
            this.btnPassoAPasso.TabIndex = 7;
            this.btnPassoAPasso.Text = "Passo a Passo";
            this.btnPassoAPasso.UseVisualStyleBackColor = true;
            this.btnPassoAPasso.Click += new System.EventHandler(this.btnPassoAPasso_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(160, 151);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(183, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "Quadro de Execução";
            // 
            // tbSaida
            // 
            this.tbSaida.BackColor = System.Drawing.Color.White;
            this.tbSaida.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSaida.Location = new System.Drawing.Point(9, 175);
            this.tbSaida.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbSaida.Multiline = true;
            this.tbSaida.Name = "tbSaida";
            this.tbSaida.ReadOnly = true;
            this.tbSaida.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbSaida.Size = new System.Drawing.Size(504, 340);
            this.tbSaida.TabIndex = 4;
            // 
            // tbCadastrar
            // 
            this.tbCadastrar.AllowDrop = true;
            this.tbCadastrar.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.tbCadastrar.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCadastrar.Location = new System.Drawing.Point(5, 76);
            this.tbCadastrar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbCadastrar.Multiline = true;
            this.tbCadastrar.Name = "tbCadastrar";
            this.tbCadastrar.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbCadastrar.Size = new System.Drawing.Size(1277, 69);
            this.tbCadastrar.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1307, 564);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Trabalho de Banco de Dados 2 - Alexandre Prezzi, Felipe Bogo, Valdir Demoliner";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTempoExecucao)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbCadastrar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbSaida;
        private System.Windows.Forms.Button btnLimpar;
        private System.Windows.Forms.Button btnPassoAPasso;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudTempoExecucao;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbFilaExecucao;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbTransAbortadas;
        private System.Windows.Forms.TextBox tbTransEspera;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbFilaDeEspera;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbTransacaoXDadoLock;
        private System.Windows.Forms.Label label2;
    }
}

