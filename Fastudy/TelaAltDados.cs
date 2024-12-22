using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.Data.Common;
using System.IO;

namespace Fastudy
{
    public partial class TelaAltDados : Form
    {
        private int tipo;
        public string novoDado = "";
        private DialogResult res;

        public TelaAltDados(int tipo, string usuario)
        {
            InitializeComponent();
            this.tipo = tipo;
            switch (tipo)
            {
                case 1: nomeEmailAlt(usuario, "Novo nome de usuario:"); break;
                case 2: nomeEmailAlt(usuario, "Novo email:"); break;
                case 3: senha(usuario); break;
                case 4: imgUsu(usuario); break;
            }
        }

        private void nomeEmailAlt(string usuario, string lblTxt) {
            this.Size = new Size(350, 140);
            FuncoesInterface.geraBarra(this, this.Width);
            Label l = new Label();
            l.Name = "label1";
            l.Text = lblTxt;
            l.AutoSize = false;
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.Size = new Size(171, 23);
            l.ForeColor = Color.Black;
            l.BackColor = Color.Transparent;
            l.Font = new Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            l.Location = new Point(90, 40);
            TextBox t = new TextBox();
            t.Name = "txtEmailNomeUsu";
            t.Size = new Size(140, 20);
            t.MaxLength = lblTxt == "Novo email:" ? 200 : 10;
            t.Location = new Point(105, 70);
            this.Controls.Add(t);
            this.Controls.Add(l);
            string coluna = "";
            if (lblTxt == "Novo email:") coluna = "email";
            else coluna = "nome_usuario";
            botoesNomeEmail(t, usuario, coluna);
        }

        private void botoesNomeEmail(object dado, string usuario, string coluna)
        {
            void btnOk_Click(object sender, EventArgs e)
            {
                TextBox t = dado as TextBox;
                string novoDado = t.Text;
                if (novoDado == "")
                {
                    MessageBox.Show("Informe o novo nome do usuário antes de continuar.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        NpgsqlConnection conn = FuncoesBancoDeDados.conecta();
                        conn.Open();
                        string sql;
                        if (coluna == "email") {
                            sql = "update tb_usuarios set " + coluna + " = '" + novoDado + "' where nome_usuario = '" + usuario + "';";
                        } else
                        {
                            sql = "select * from tb_usuarios where nome_usuario = '" + usuario + "';";
                            DbDataReader dados = FuncoesBancoDeDados.select(sql, conn);
                            dados.Read();
                            sql = "insert into tb_usuarios values ('" + dados["nome_completo"].ToString() + "', '" + novoDado + "', " + dados["cdg_escolar"].ToString() + ", '" + dados["email"].ToString() + "', '" + dados["senha"].ToString() + "');";
                            conn.Close();
                            conn.Open();
                            int foi = FuncoesBancoDeDados.insUpDel(sql, conn);
                            if (foi <= 0)
                            {
                                MessageBox.Show("Ocorreu um erro durante a troca de nome de usuário, por favor tente novamente mais tarde.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            conn.Close();
                            conn.Open();
                            sql = "update tb_per_respondidas set nome_usuario = '" + novoDado + "' where nome_usuario = '" + usuario + "';";
                            foi = FuncoesBancoDeDados.insUpDel(sql, conn);
                            if (foi <= 0)
                            {
                                MessageBox.Show("Ocorreu um erro durante a troca de nome de usuário, por favor tente novamente mais tarde.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            conn.Close();
                            conn.Open();
                            sql = "delete from tb_usuarios where nome_usuario = '" + usuario + "';";
                        }
                        int deu = FuncoesBancoDeDados.insUpDel(sql, conn);
                        if (deu > 0)
                        {
                            if (coluna == "nome_usuario")
                            {
                                string path = "..\\..\\Imagens\\Imagens Usuarios\\imagem " + usuario + ".png";
                                string newPath = "..\\..\\Imagens\\Imagens Usuarios\\imagem " + novoDado + ".png";
                                if (File.Exists(path))
                                {
                                    File.Move(path, newPath);
                                }
                            }
                            MessageBox.Show(coluna == "email" ? "Email alterado com sucesso." : "Nome de usuário alterado com sucesso.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.novoDado = novoDado;
                        }
                        else { MessageBox.Show("Ocorreu um erro durante a tentativa de alteração do " + coluna == "email" ? "email" : "nome de usuário" + ", por favor tente mais tarde.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        conn.Close();
                        this.Close();
                    }
                    catch (Exception er)
                    {
                        if (er.Message.Contains("duplicate key value violates unique constraint")) MessageBox.Show("Erro, esse nome de usuário já existe, por favor, escolha outro nome de usuário", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //else MessageBox.Show(er.Message);
                    }
                }
            }

            void btnCanc_Click(object sender, EventArgs e)
            {
                this.Close();
            }

            Button b1 = new Button();
            Button b2 = new Button();
            b1.Name = "btnOk";
            b2.Name = "btnCanc";
            b1.Text = "Salvar alterações";
            b2.Text = "Cancelar";
            b1.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            b2.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            b1.TextAlign = ContentAlignment.MiddleCenter;
            b2.TextAlign = ContentAlignment.MiddleCenter;
            b1.Size = new Size(100, 28);
            b2.Size = new Size(75, 28);
            b1.Location = new Point(68, 100);
            b2.Location = new Point(203, 100);

            b1.Click += new EventHandler(btnOk_Click);
            b2.Click += new EventHandler(btnCanc_Click);

            this.Controls.Add(b1);
            this.Controls.Add(b2);
        }

        private void senha(string usuario) {
            this.Size = new Size(330, 240);
            FuncoesInterface.geraBarra(this, this.Width);

            Label l1 = new Label();
            Label l2 = new Label();
            Label l3 = new Label();
            l1.Name = "lblSenAtu";
            l2.Name = "lblNovSen";
            l3.Name = "lblConfSen";
            l1.Text = "Senha atual:";
            l2.Text = "Nova senha:";
            l3.Text = "Confirmar senha:";
            l1.Font = new Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            l2.Font = new Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            l3.Font = new Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            l1.AutoSize = false;
            l2.AutoSize = false;
            l3.AutoSize = false;
            l1.TextAlign = ContentAlignment.MiddleCenter;
            l2.TextAlign = ContentAlignment.MiddleCenter;
            l3.TextAlign = ContentAlignment.MiddleCenter;
            l1.Size = new Size(100, 25);
            l2.Size = new Size(100, 25);
            l3.Size = new Size(134, 25);
            l1.Location = new Point(115, 35);
            l2.Location = new Point(115, 85);
            l3.Location = new Point(98, 135);
            TextBox t1 = new TextBox();
            TextBox t2 = new TextBox();
            TextBox t3 = new TextBox();
            t1.Name = "txtSenAtu";
            t2.Name = "txtNovSen";
            t3.Name = "txtConfSen";
            t1.PasswordChar = '*';
            t2.PasswordChar = '*';
            t3.PasswordChar = '*';
            t1.Size = new Size(110, 25);
            t2.Size = new Size(110, 25);
            t3.Size = new Size(110, 25);
            t1.Location = new Point(110, 60);
            t2.Location = new Point(110, 110);
            t3.Location = new Point(110, 160);

            void confirmar(object sender, EventArgs e)
            {
                if (t1.Text == "")
                {
                    MessageBox.Show("Informe sua senha atual para prosseguir", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } else if (t2.Text == "")
                {
                    MessageBox.Show("Informe a nova senha para prosseguir", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } else if (t3.Text == "")
                {
                    MessageBox.Show("Confirme sua nova senha para prosseguir", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } else
                {
                    string senha = "";
                    try
                    {
                        NpgsqlConnection conn = FuncoesBancoDeDados.conecta();
                        conn.Open();
                        string sql = "select senha from tb_usuarios where nome_usuario = '" + usuario + "';";
                        DbDataReader res = FuncoesBancoDeDados.select(sql, conn);
                        res.Read();
                        senha = res["senha"].ToString();
                        conn.Close();
                        if (senha == FuncoesGerais.encriptar(t1.Text))
                        {
                            if (t2.Text.Equals(t3.Text))
                            {
                                try
                                {
                                    conn.Open();
                                    sql = "update tb_usuarios set senha = '" + FuncoesGerais.encriptar(t2.Text) + "' where nome_usuario = '" + usuario + "';";
                                    int deu = FuncoesBancoDeDados.insUpDel(sql, conn);
                                    if (deu > 0)
                                    {
                                        MessageBox.Show("Senha alterada com sucesso.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Ocorreu um erro durante a tentativa de alteração da senha, por favor tente mais tarde.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    conn.Close();
                                    this.Close();
                                }
                                catch (Exception er) {  }
                            }
                            else
                            {
                                MessageBox.Show("Erro, por favor confira se sua nova senha está redigitada corretamente.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Erro, por favor confira se sua senha atual está digitada corretamente.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } catch (Exception er) {  }
                }
            }

            void cancelar(object sender, EventArgs e)
            {
                this.Close();
            }

            Button b1 = new Button();
            Button b2 = new Button();
            b1.Name = "btnConf";
            b2.Name = "btnCanc";
            b1.Text = "Confirmar";
            b2.Text = "Cancelar";
            b1.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            b2.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            b1.Size = new Size(75, 25);
            b2.Size = new Size(75, 25);
            b1.Location = new Point(65, 195);
            b2.Location = new Point(190, 195);
            b1.Click += new EventHandler(confirmar);
            b2.Click += new EventHandler(cancelar);
            this.Controls.Add(l1);
            this.Controls.Add(l2);
            this.Controls.Add(l3);
            this.Controls.Add(t1);
            this.Controls.Add(t2);
            this.Controls.Add(t3);
            this.Controls.Add(b1);
            this.Controls.Add(b2);
        }

        private void imgUsu(string usuario) {
            this.Size = new Size(330, 150);
            FuncoesInterface.geraBarra(this, this.Width);
            string path = "..\\..\\Imagens\\Imagens Usuarios\\imagem " + usuario + ".png";
            OpenFileDialog opnFileImgUsuario = new OpenFileDialog();
            opnFileImgUsuario.Filter = "Imagens (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF";
            opnFileImgUsuario.FileName = "";
            Panel p = new Panel();
            p.Name = "pnFoto";
            p.Size = new Size(40, 40);
            p.Location = new Point(30, 45);
            p.BackgroundImageLayout = ImageLayout.Stretch;

            void cancelar(object sender, EventArgs e)
            {
                this.Close();
            }

            void confirmar(object sender, EventArgs e)
            {
                if (res == DialogResult.OK && opnFileImgUsuario.FileName != "")
                {
                    if (File.Exists(path)) File.Delete(path);
                    Image imagem = Image.FromFile(opnFileImgUsuario.FileName);
                    imagem.Save(path);
                    this.novoDado = path;
                    MessageBox.Show("Foto alterada com sucesso!", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                } else
                {
                    MessageBox.Show("Escolha uma imagem antes de enviar", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            void enviar(object sender, EventArgs e)
            {
                res = opnFileImgUsuario.ShowDialog();
                if (res == DialogResult.OK) p.BackgroundImage = Image.FromFile(opnFileImgUsuario.FileName);
                p.BackColor = Color.Transparent;
            }

            void excluir(object sender, EventArgs e)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    MessageBox.Show("Sua imagem de usuário foi restaurada para a padrão", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                } else
                {
                    DialogResult result = MessageBox.Show("Você já está utilizando a imagem de usuário padrão, deseja troca-la?", "Resultado", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No) this.Close();
                }
            }

            Button b1 = new Button();
            Button b2 = new Button();
            Button b3 = new Button();
            Button b4 = new Button();
            b1.Name = "btnEnviaImg";
            b1.Text = "Enviar imagem";
            b1.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            b1.TextAlign = ContentAlignment.MiddleCenter;
            b1.Size = new Size(75, 44);
            b1.Location = new Point(75, 45);
            b1.Click += new EventHandler(enviar);
            b2.Name = "btnExcluiImg";
            b2.Text = "Excluir minha foto";
            b2.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            b2.TextAlign = ContentAlignment.MiddleCenter;
            b2.Size = new Size(95, 44);
            b2.Location = new Point(195, 45);
            b2.Click += new EventHandler(excluir);
            b3.Name = "btnConfir";
            b3.Text = "Confirmar";
            b3.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            b3.TextAlign = ContentAlignment.MiddleCenter;
            b3.Size = new Size(75, 30);
            b3.Location = new Point(65, 105);
            b3.Click += new EventHandler(confirmar);
            b4.Name = "btnCanc";
            b4.Text = "Cancelar";
            b4.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            b4.TextAlign = ContentAlignment.MiddleCenter;
            b4.Size = new Size(75, 30);
            b4.Location = new Point(175, 105);
            b4.Click += new EventHandler(cancelar);
            this.Controls.Add(p);
            this.Controls.Add(b1);
            this.Controls.Add(b2);
            this.Controls.Add(b3);
            this.Controls.Add(b4);
        }
    }
}
