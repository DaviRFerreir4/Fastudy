using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data.Common;
using Npgsql;

namespace Fastudy
{
    public partial class TelaLogin : Form
    {
        public TelaLogin()
        {
            InitializeComponent();
            FuncoesInterface.geraBarra(this, this.Width);
            txtLogin.Focus();
            txtLogin.Select();
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            if (txtLogin.Text == "")
            {
                MessageBox.Show("Digite seu usuário antes de continuar", "Usuário não informado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtLogin.Text = "";
            }
            else if (txtCodigoEscolar.Text == "")
            {
                MessageBox.Show("Digite seu código escolar antes de continuar", "Código escolar não informado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCodigoEscolar.Text = "";
            }
            else if (txtSenha.Text == "")
            {
                MessageBox.Show("Digite sua senha antes de continuar", "Senha não informado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                NpgsqlConnection conn = FuncoesBancoDeDados.conecta();
                try
                {
                    conn.Open();
                    string sql = "select * from tb_usuarios where nome_usuario = '" + txtLogin.Text + "' and senha = '"
                    + FuncoesGerais.encriptar(txtSenha.Text) + "' and cdg_escolar = " + txtCodigoEscolar.Text + ";";
                    NpgsqlDataReader data = FuncoesBancoDeDados.select(sql, conn);
                    if (data.HasRows)
                    {
                        data.Read();
                        string dados = "";
                        dados = data["nome_usuario"].ToString();
                        TelaMenu f = new TelaMenu(dados);
                        this.Hide();
                        conn.Close();
                        f.ShowDialog();
                        this.Show();
                        txtLogin.Text = "";
                        txtCodigoEscolar.Text = "";
                        txtSenha.Text = "";
                    } else
                    {
                        MessageBox.Show("Algum dos seus dados está incorreto, por favor, confira seu usuário " +
                            "e sua senha novamente.", "Usuário ou senha incorretos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        conn.Close();
                    }
                } catch (Exception er) {
                    MessageBox.Show("Esse aplicativo necessita estar conectado a rede para funcionar, por favor, confira se sua conexão está normal.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtCodigoEscolar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8) e.Handled = true;
        }
    }
}
