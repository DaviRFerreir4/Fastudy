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
    public partial class TelaFechar : Form
    {
        Form f;

        public TelaFechar(Form f)
        {
            InitializeComponent();
            this.f = f;
            FuncoesInterface.geraBarra(this, this.Width);
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnDeslogar_Click(object sender, EventArgs e)
        {
            f.Close();
            this.Close();
        }
    }
}
