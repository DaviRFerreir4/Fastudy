using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fastudy
{
    public partial class TelaResPerg : Form
    {
        public string decisao;
        public TelaResPerg(int status)
        {
            InitializeComponent();
            FuncoesInterface.geraBarra(this, this.Width);
            if (status == 1) {
                label1.Text = "Parabéns, você acertou a questão, agora o que deseja fazer?";
            } else
            {
                label1.Text = "Que pena, você errou a questão, agora o que deseja fazer?";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.decisao = "voltar";
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.decisao = "ficar";
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.decisao = "continuar";
            this.Close();
        }
    }
}
