using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fastudy
{
    public static class FuncoesInterface
    {
        public static void geraBarra(Form f, int width)
        {
            void fecha(object sender, EventArgs e)
            {
                if (f.Name == "TelaMenu")
                {
                    TelaFechar tela = new TelaFechar(f);
                    tela.ShowDialog();
                }
                else if (f.Name == "TelaLogin")
                {
                    Environment.Exit(0);
                }
                else
                {
                    f.Close();
                }
            }

            void minimiza(object sender, EventArgs e)
            {
                f.WindowState = FormWindowState.Minimized;
            }

            Button btnFecha = new Button();
            Button btnMinimiza = new Button();
            Panel barra = new Panel();

            btnFecha.Name = "btnFecha";
            btnFecha.Location = new Point(width - 36, 2);
            btnFecha.Size = new Size(28, 20);
            btnFecha.FlatStyle = FlatStyle.Flat;
            btnFecha.BackColor = Color.Red;
            btnFecha.Text = "X";
            btnFecha.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            btnFecha.Click += new EventHandler(fecha);

            btnMinimiza.Name = "btnMinimiza";
            btnMinimiza.Location = new Point(width - 68, 2);
            btnMinimiza.Size = new Size(28, 20);
            btnMinimiza.FlatStyle = FlatStyle.Flat;
            btnMinimiza.BackColor = Color.LightSkyBlue;
            btnMinimiza.Text = "-";
            btnMinimiza.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
            btnMinimiza.Click += new EventHandler(minimiza);

            barra.Name = "pnlBarra";
            barra.Size = new Size(width, 26);
            barra.Location = new Point(0, 0);
            barra.BackColor = SystemColors.GradientActiveCaption;
            barra.Controls.Add(btnFecha);
            if (f.Name == "TelaMenu" || f.Name == "TelaLogin") barra.Controls.Add(btnMinimiza);


            f.Controls.Add(barra);
        }

        public static void mudaCorTexto (Label l, int n)
        {
            if (n == 1)
            {
                l.ForeColor = Color.FromArgb(1, 0, 123, 255);
            } else if (n == 0)
            {
                l.ForeColor = Color.Black;
            }
        }
    }
}
