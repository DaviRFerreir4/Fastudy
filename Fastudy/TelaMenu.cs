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
using System.Text.Json;

namespace Fastudy
{
    public partial class TelaMenu : Form
    {
        public string dados = "";
        private int cdgMateria;
        private int cdgConteudo;
        private int mult = 0;
        private int qtdPerg = 0;
        private int ultPer = 0;

        public TelaMenu(string dados)
        {
            InitializeComponent();
            FuncoesInterface.geraBarra(this, this.Width);
            this.dados = dados;
            insereUsuario(dados, 1);
        }

        private void insereUsuario(string usuario, int inicio = 0)
        {
            try
            {
                NpgsqlConnection conn = FuncoesBancoDeDados.conecta();
                conn.Open();
                string sql = "select nome_usuario, nome_completo, nome_escolar, email from tb_usuarios a join tb_escolas b " +
                    "on a.cdg_escolar = b.cdg_escolar where nome_usuario = '" + usuario + "';";
                NpgsqlDataReader dados = FuncoesBancoDeDados.select(sql, conn);
                dados.Read();
                lbNomeUsuario.Text = dados["nome_usuario"].ToString();
                lblNomeUsuTela.Text = dados["nome_usuario"].ToString();
                lblNomeCompleto.Text = dados["nome_completo"].ToString();
                lblInstituicao.Text = dados["nome_escolar"].ToString();
                lblEmail.Text = dados["email"].ToString();
                try
                {
                    string caminho = "..\\..\\Imagens\\Imagens Usuarios\\imagem " + dados["nome_usuario"] + ".png";
                    pnFotoUsuario.BackgroundImage = Image.FromFile(caminho);
                    pnImgUsuarioTela.BackgroundImage = Image.FromFile(caminho);
                } catch (Exception er) { pnImgUsuarioTela.BackgroundImage = Properties.Resources.imgUsuarioPadrao; pnFotoUsuario.BackgroundImage = Properties.Resources.imgUsuarioPadrao; }
                conn.Close();
                conn.Open();
                sql = "select informacoes ->> 'firstStatus' as fstat, informacoes ->> 'status' as stat from tb_per_respondidas where nome_usuario = '" + usuario +"';";
                dados = FuncoesBancoDeDados.select(sql, conn);
                int perResp = 0;
                int perCorr = 0;
                int perCorrFirst = 0;
                while (dados.Read())
                {
                    perResp++;
                    if (dados["fstat"].ToString() == "correta") perCorrFirst++;
                    if (dados["stat"].ToString() == "correta") perCorr++;
                }
                conn.Close();
                conn.Open();
                sql = "select count(cdg_pergunta) from tb_perguntas;";
                dados = FuncoesBancoDeDados.select(sql, conn);
                dados.Read();
                int totalPer = int.Parse(dados[0].ToString());
                conn.Close();
                lblPerRes.Text = perResp + "/" + totalPer;
                lblPerResCor.Text = perCorr + "/" + totalPer;
                lblPerCorFirst.Text = perCorrFirst + "/" + totalPer;
            } catch (Exception er) {  }
            if (inicio != 0) trocaTela("inicial");
        }

        private void lblTelaInicial_MouseEnter(object sender, EventArgs e) { FuncoesInterface.mudaCorTexto(lblTelaInicial, 1); }
        private void lblTelaInicial_MouseLeave(object sender, EventArgs e) { FuncoesInterface.mudaCorTexto(lblTelaInicial, 0); }
        private void lblExercicios_MouseEnter(object sender, EventArgs e) { FuncoesInterface.mudaCorTexto(lblExercicios, 1); }
        private void lblExercicios_MouseLeave(object sender, EventArgs e) { FuncoesInterface.mudaCorTexto(lblExercicios, 0); }
        private void lblSobreNos_MouseEnter(object sender, EventArgs e) { FuncoesInterface.mudaCorTexto(lblSobreNos, 1); }
        private void lblSobreNos_MouseLeave(object sender, EventArgs e) { FuncoesInterface.mudaCorTexto(lblSobreNos, 0); }

        private void trocaTela(string tela)
        {
            pnTelaBemVindo.Visible = false;
            pnTelaExercicios.Visible = false;
            pnTelaPerguntasRandom.Visible = false;
            pnTelaDisciplinas.Visible = false;
            pnTelaSobreNos.Visible = false;
            pnTelaUsuario.Visible = false;
            switch (tela)
            {
                case "inicial": pnTelaBemVindo.Visible = true; break;
                case "exercicios": pnTelaExercicios.Visible = true; break;
                case "exerRandom": pnTelaPerguntasRandom.Visible = true; break;
                case "disciplinas": pnTelaDisciplinas.Visible = true; break;
                case "sobre nos": pnTelaSobreNos.Visible = true; break;
                case "usuario": pnTelaUsuario.Visible = true; insereUsuario(dados); break;
                default: break;
            }
        }

        // tela usuario
        private void pnFotoUsuario_Click(object sender, EventArgs e) { trocaTela("usuario"); }
        private void lbNomeUsuario_Click(object sender, EventArgs e) { trocaTela("usuario"); }

        // tela inicial
        private void lblTelaInicial_Click(object sender, EventArgs e) { trocaTela("inicial"); }

        //tela de escolha de exercicios (normais ou randomicos)
        private void lblExercicios_Click(object sender, EventArgs e) { trocaTela("exercicios"); }

        //tela explicando sobre o projeto
        private void lblSobreNos_Click(object sender, EventArgs e) { trocaTela("sobre nos"); }

        //tela com as disciplinas para escolher perguntas
        private void pnPerguntas_Click(object sender, EventArgs e)
        {
            mult = 0;
            cdgMateria = 0;
            pnTelaDisciplinas.Controls.Clear();
            trocaTela("disciplinas");
            try
            {
                NpgsqlConnection conn = FuncoesBancoDeDados.conecta();
                conn.Open();
                string sql = "select * from tb_materias order by cdg_materia;";
                NpgsqlDataReader dados = FuncoesBancoDeDados.select(sql, conn);
                int cont = 0;
                while (dados.Read())
                {
                    cont = int.Parse(dados["cdg_materia"].ToString());
                    Panel p = new Panel();
                    p.Name = "pnDisc" + cont;
                    p.Size = new Size(160, 160);
                    int x = (cont % 3 == 1) ? 119 : (cont % 3 == 0) ? 540 : 340;
                    int y = (cont <= 3) ? 14 : (cont > 6) ? 389 : 204;
                    p.Location = new Point(x, y);
                    p.BackColor = Color.Transparent;
                    p.BackgroundImage = Image.FromFile("..\\..\\Imagens\\Imagens Disciplinas\\imagem" + cont + ".png");
                    p.BackgroundImageLayout = ImageLayout.Stretch;
                    p.Click += new EventHandler(this.pnDisc_Click);
                    Label l = new Label();
                    l.Name = "lblDisc" + cont;
                    l.Text = dados["nome_materia"].ToString();
                    l.AutoSize = false;
                    l.Size = new Size(170, 20);
                    l.Location = new Point(x, y + 160);
                    l.Font = new Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                    l.ForeColor = Color.White;
                    l.TextAlign = ContentAlignment.MiddleCenter;
                    l.Click += new EventHandler(this.lblDisc_Click);
                    pnTelaDisciplinas.Controls.Add(p);
                    pnTelaDisciplinas.Controls.Add(l);
                }
                conn.Close();
            } catch (Exception er) {  }
        }

        //função de criar os conteudo
        public void pnDisc_Click(object sender, EventArgs e)
        {
            Panel p = sender as Panel;
            this.cdgMateria = int.Parse(p.Name.Substring(6, 1));
            criaConteudo();
        }

        public void lblDisc_Click(object sender, EventArgs e)
        {
            Label l = sender as Label;
            this.cdgMateria = int.Parse(l.Name.Substring(7, 1));
            criaConteudo();
        }

        public void criaConteudo()
        {
            cdgConteudo = 0;
            pnTelaDisciplinas.Controls.Clear();
            try
            {
                NpgsqlConnection conn = FuncoesBancoDeDados.conecta();
                conn.Open();
                string sql = "select nome_conteudo, cdg_conteudo from tb_conteudos where cdg_materia = " +
                    this.cdgMateria + " limit 12 offset " + (0 + (12 * mult)) + ";";
                DbDataReader dados = FuncoesBancoDeDados.select(sql, conn);
                int cont = 1 + (12 * mult);
                int i = 0;
                while (dados.Read())
                {
                    i++;
                    Panel p = new Panel();
                    p.Name = "pnCont" + dados["cdg_conteudo"].ToString();
                    p.Size = new Size(185, 150);
                    int x = (i % 4 == 1) ? 23 : (i % 4 == 2) ? 228 : (i % 4 == 3) ? 433 : 638;
                    int y = (i <= 4) ? 81 : (i > 8) ? 401 : 241;
                    p.Location = new Point(x, y);
                    p.BackColor = Color.Transparent;
                    p.Click += new EventHandler(this.pnCont_Click);
                    Label l = new Label();
                    l.Name = "lblCont" + dados["cdg_conteudo"].ToString();
                    l.Text = dados["nome_conteudo"].ToString();
                    l.AutoSize = false;
                    l.Size = new Size(175, 140);
                    l.Location = new Point(5, 5);
                    l.Font = new Font("Comic Sans MS", 18F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                    l.ForeColor = Color.White;
                    l.TextAlign = ContentAlignment.MiddleCenter;
                    l.Click += new EventHandler(this.lblCont_Click);
                    p.Controls.Add(l);
                    pnTelaDisciplinas.Controls.Add(p);
                    cont++;
                }
                if (mult != 0)
                {
                    Label volta = new Label();
                    volta.Name = "lblVoltaCont";
                    volta.Text = "<";
                    volta.Font = new Font("Comic Sans MS", 26F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                    volta.ForeColor = Color.White;
                    volta.AutoSize = false;
                    volta.Size = new Size(40, 40);
                    volta.Location = new Point(35, 10);
                    volta.Click += new EventHandler(this.lblVoltaCont_Click);
                    pnTelaDisciplinas.Controls.Add(volta);
                }
                conn.Close();
                conn.Open();
                sql = "select nome_conteudo from tb_conteudos where cdg_materia = " +
                    this.cdgMateria + " limit 12 offset " + (0 + (12 * (mult + 1))) + ";";
                dados = FuncoesBancoDeDados.select(sql, conn);
                if (dados.HasRows)
                {
                    Label vai = new Label();
                    vai.Name = "lblVaiCont";
                    vai.Text = ">";
                    vai.Font = new Font("Comic Sans MS", 26F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                    vai.ForeColor = Color.White;
                    vai.AutoSize = false;
                    vai.Size = new Size(40, 40);
                    vai.Location = new Point(pnTelaDisciplinas.Width - vai.Width - 35, 10);
                    vai.Click += new EventHandler(this.lblVaiCont_Click);
                    pnTelaDisciplinas.Controls.Add(vai);
                }
            }
            catch (Exception er) {  }
        }

        private void lblVoltaCont_Click(object sender, EventArgs e)
        {
            this.mult--;
            criaConteudo();
        }

        private void lblVaiCont_Click(object sender, EventArgs e)
        {
            this.mult++;
            criaConteudo();
        }

        private void pnCont_Click(object sender, EventArgs e)
        {
            pnTelaDisciplinas.Controls.Clear();
            Panel p = sender as Panel;
            this.cdgConteudo = int.Parse(p.Name.Substring(6, p.Name.Length - 6));
            criaPergunta();
        }

        private void lblCont_Click(object sender, EventArgs e)
        {
            pnTelaDisciplinas.Controls.Clear();
            Label l = sender as Label;
            this.cdgConteudo = int.Parse(l.Name.Substring(7, l.Name.Length - 7));
            criaPergunta();
        }

        private void criaPergunta()
        {
            try
            {
                NpgsqlConnection conn = FuncoesBancoDeDados.conecta();
                conn.Open();
                string sql = "select dados_pergunta, cdg_pergunta from tb_perguntas where cdg_conteudo = " + this.cdgConteudo + " order by cdg_pergunta;";
                DbDataReader dados = FuncoesBancoDeDados.select(sql, conn);
                if (dados.HasRows)
                {
                    dados.Read();
                    passaPergunta(dados, pnTelaDisciplinas);
                }
                else
                {
                    MessageBox.Show("baianorkk");
                    criaConteudo();
                }
            }
            catch (Exception er) {  }
        }

        private void passaPergunta(DbDataReader dados, Panel pan, DialogResult res = DialogResult.No)
        {
            Pergunta p;
            var readOnlySpan = new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(dados[0].ToString()));
            p = JsonSerializer.Deserialize<Pergunta>(readOnlySpan);
            montaPergunta(p, dados, pan, res);
        }

        private void montaPergunta(Pergunta perg, DbDataReader dados, Panel pan, DialogResult res = DialogResult.No)
        {
            int cdgPer = int.Parse(dados["cdg_pergunta"].ToString());
            int temPer = 0;

            void btnImagem_Click(object sender, EventArgs e)
            {
                Form f = new Form();
                f.Text = "";
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MaximizeBox = false;
                f.MinimizeBox = false;
                f.StartPosition = FormStartPosition.CenterScreen;
                Image i = Image.FromFile(perg.img.path);
                f.BackgroundImage = i;
                f.Size = new Size(i.Width + 13, i.Height + 37);
                f.ShowDialog();
            }

            void lblPassaPer_Click(object sender, EventArgs e)
            {
                pan.Controls.Clear();
                if (pan.Name == "pnTelaDisciplinas") passaPergunta(dados, pnTelaDisciplinas);
                else exerRandom(res);
            }

            int y = 359;
            RadioButton alt;
            RadioButton alt1 = null;
            RadioButton alt2 = null;
            RadioButton alt3 = null;
            RadioButton alt4 = null;
            RadioButton alt5 = null;
            for (int i = 0; i < perg.answers.Count; i++)
            {
                alt = new RadioButton();
                alt.Name = "rdAlt" + perg.answers[i].letter;
                alt.Text = perg.answers[i].letter + ") " + perg.answers[i].value;
                alt.Tag = perg.answers[i].letter;
                alt.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                alt.TextAlign = ContentAlignment.MiddleLeft;
                alt.BackColor = Color.Transparent;
                alt.ForeColor = Color.White;
                alt.AutoSize = false;
                alt.Size = new Size(700, 44);
                alt.Location = new Point(32, y);
                y += 44;
                switch (i)
                {
                    case 0: alt1 = alt; break;
                    case 1: alt2 = alt; break;
                    case 2: alt3 = alt; break;
                    case 3: alt4 = alt; break;
                    case 4: alt5 = alt; break;
                }
                pan.Controls.Add(alt);
            }

            void gravaResposta(int status)
            {
                try
                {
                    NpgsqlConnection conn = FuncoesBancoDeDados.conecta();
                    conn.Open();
                    string sql = "select informacoes ->> 'firstStatus' as fstat from tb_per_respondidas where nome_usuario = '" + this.dados + "' and cdg_pergunta = " + cdgPer + ";";
                    DbDataReader dadosSel = FuncoesBancoDeDados.select(sql, conn);
                    if (!dadosSel.HasRows)
                    {
                        conn.Close();
                        conn.Open();
                        string json = "{\"firstStatus\": \"" + (status == 1 ? "correta" : "incorreta") + "\", \"status\": \"" + (status == 1 ? "correta" : "incorreta") + "\"}";
                        sql = "insert into tb_per_respondidas values ('" + this.dados + "', " + cdgMateria + ", " + cdgConteudo + ", " + cdgPer + ", '" + json + "');";
                        int deu = FuncoesBancoDeDados.insUpDel(sql, conn);
                        if (!(deu > 0))
                        {
                            MessageBox.Show("Ocorreu algum erro ao gravar sua resposta, se quiser grava-las, por favor, tente mais tarde.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } else
                    {
                        dadosSel.Read();
                        string fstat = dadosSel["fstat"].ToString();
                        conn.Close();
                        conn.Open();
                        string json = "{\"firstStatus\": \"" + fstat + "\", \"status\": \"" + (status == 1 ? "correta" : "incorreta") + "\"}";
                        sql = "update tb_per_respondidas set informacoes = '" + json + "' where nome_usuario = '" + this.dados + "' and cdg_pergunta = " + cdgPer + ";";
                        int deu = FuncoesBancoDeDados.insUpDel(sql, conn);
                        if (!(deu > 0))
                        {
                            MessageBox.Show("Ocorreu algum erro ao gravar sua resposta, se quiser grava-las, por favor, tente mais tarde.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    conn.Close();
                } catch (Exception er) {  }
            }
            
            void decisao(string dec)
            {
                if (dec == "voltar" && pan.Name == "pnTelaDisciplinas")
                {
                    pan.Controls.Clear();
                    criaConteudo();
                } else if (dec == "voltar" && pan.Name == "pnTelaPerguntasRandom")
                {
                    pan.Controls.Clear();
                    trocaTela("exercicios");
                } else if (dec == "continuar" && pan.Name == "pnTelaDisciplinas")
                {
                    pan.Controls.Clear();
                    if (temPer == 1) {
                        passaPergunta(dados, pnTelaDisciplinas);
                    } else
                    {
                        MessageBox.Show("Você finalizou todas as questões.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        criaConteudo();
                    }
                } else if (dec == "continuar" && pan.Name == "pnTelaPerguntasRandom")
                {
                    pan.Controls.Clear();
                    exerRandom(res);
                }
            }

            void btnResponder_Click(object sender, EventArgs e)
            {
                if (alt1.Checked)
                {
                    TelaResPerg f;
                    if (perg.correct == "A")
                    {
                        gravaResposta(1);
                        alt1.BackColor = Color.Lime;
                        f = new TelaResPerg(1);
                        f.ShowDialog();
                        alt1.BackColor = Color.Transparent;
                        decisao(f.decisao);
                    } else {
                        gravaResposta(0);
                        alt1.BackColor = Color.Red;
                        f = new TelaResPerg(0);
                        f.ShowDialog();
                        alt1.BackColor = Color.Transparent;
                        decisao(f.decisao);
                    }
                } else if (alt2.Checked)
                {
                    TelaResPerg f;
                    if (perg.correct == "B")
                    {
                        gravaResposta(1);
                        alt2.BackColor = Color.Lime;
                        f = new TelaResPerg(1);
                        f.ShowDialog();
                        alt2.BackColor = Color.Transparent;
                        decisao(f.decisao);
                    }
                    else {
                        gravaResposta(0);
                        alt2.BackColor = Color.Red;
                        f = new TelaResPerg(0);
                        f.ShowDialog();
                        alt2.BackColor = Color.Transparent;
                        decisao(f.decisao);
                    }
                } else if (alt3.Checked)
                {
                    TelaResPerg f;
                    if (perg.correct == "C")
                    {
                        gravaResposta(1);
                        alt3.BackColor = Color.Lime;
                        f = new TelaResPerg(1);
                        f.ShowDialog();
                        alt3.BackColor = Color.Transparent;
                        decisao(f.decisao);
                    } else
                    {
                        gravaResposta(0);
                        alt3.BackColor = Color.Red;
                        f = new TelaResPerg(0);
                        f.ShowDialog();
                        alt3.BackColor = Color.Transparent;
                        decisao(f.decisao);
                    }
                } else if (alt4.Checked)
                {
                    TelaResPerg f;
                    if (perg.correct == "D")
                    {
                        gravaResposta(1);
                        alt4.BackColor = Color.Lime;
                        f = new TelaResPerg(1);
                        f.ShowDialog();
                        alt4.BackColor = Color.Transparent;
                        decisao(f.decisao);
                    } else
                    {
                        gravaResposta(0);
                        alt4.BackColor = Color.Red;
                        f = new TelaResPerg(0);
                        f.ShowDialog();
                        alt4.BackColor = Color.Transparent;
                        decisao(f.decisao);
                    }
                } else if (alt5 != null) {
                    if (alt5.Checked)
                    {
                        TelaResPerg f;
                        if (perg.correct == "E")
                        {
                            gravaResposta(1);
                            alt5.BackColor = Color.Lime;
                            f = new TelaResPerg(1);
                            f.ShowDialog();
                            alt5.BackColor = Color.Transparent;
                            decisao(f.decisao);
                        } else
                        {
                            gravaResposta(0);
                            alt5.BackColor = Color.Red;
                            f = new TelaResPerg(0);
                            f.ShowDialog();
                            alt5.BackColor = Color.Transparent;
                            decisao(f.decisao);
                        }
                    } else { MessageBox.Show("Escolha uma alternativa para continuar.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                } else
                {
                    MessageBox.Show("Escolha uma alternativa para continuar.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
            Label titulo = new Label();
            titulo.Name = "lblTitulo";
            titulo.Text = perg.title;
            titulo.TextAlign = ContentAlignment.MiddleLeft;
            titulo.Font = new Font("Comic Sans MS", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            titulo.ForeColor = Color.White;
            titulo.Location = new Point(12, 9);
            Label pergunta = new Label();
            pergunta.Name = "lblPergunta";
            pergunta.Text = perg.question;
            pergunta.TextAlign = ContentAlignment.MiddleLeft;
            pergunta.Font = new Font("Comic Sans MS", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            pergunta.ForeColor = Color.White;
            pergunta.AutoSize = false;
            pergunta.Size = new Size(784, 144);
            pergunta.Location = new Point(32, 30);
            if (perg.opcionalQuestion != "")
            {
                Label opPergunta = new Label();
                opPergunta.Name = "lblOpPergunta";
                opPergunta.Text = perg.opcionalQuestion;
                opPergunta.TextAlign = ContentAlignment.MiddleLeft;
                opPergunta.Font = new Font("Comic Sans MS", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                opPergunta.ForeColor = Color.White;
                opPergunta.AutoSize = false;
                opPergunta.Size = new Size(784, 124);
                opPergunta.Location = new Point(32, 225);
                pan.Controls.Add(opPergunta);
            }
            if (res == DialogResult.Yes)
            {
                Label matCont = new Label();
                matCont.Name = "lblMatCont";
                try
                {
                    NpgsqlConnection conn = FuncoesBancoDeDados.conecta();
                    conn.Open();
                    string sql = "select nome_materia, nome_conteudo from tb_perguntas tb1" +
                        " join tb_conteudos tb2 on tb1.cdg_conteudo = tb2.cdg_conteudo" +
                        " join tb_materias tb3 on tb1.cdg_materia = tb3.cdg_materia" +
                        " where cdg_pergunta = " + cdgPer + ";";
                    DbDataReader dadosMatCont = FuncoesBancoDeDados.select(sql, conn);
                    dadosMatCont.Read();
                    matCont.Text = dadosMatCont["nome_materia"].ToString() + " - " + dadosMatCont["nome_conteudo"].ToString();
                } catch (Exception er) { }
                matCont.TextAlign = ContentAlignment.MiddleCenter;
                matCont.Font = new Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                matCont.ForeColor = Color.White;
                matCont.AutoSize = true;
                matCont.Location = new Point((pan.Width/2) - (matCont.Width/2) - 40, 20);
                pan.Controls.Add(matCont);
            }
            if (dados.Read() || pan.Name == "pnTelaPerguntasRandom")
            {
                temPer = 1;
                Label passa = new Label();
                passa.Name = "lblPassaPer";
                passa.Text = ">";
                passa.TextAlign = ContentAlignment.MiddleLeft;
                passa.Font = new Font("Comic Sans MS", 19F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                passa.ForeColor = Color.White;
                passa.AutoSize = false;
                passa.Size = new Size(25, 34);
                passa.Location = new Point(776, 182);
                passa.Click += new EventHandler(lblPassaPer_Click);
                pan.Controls.Add(passa);
            }
            Button resposta = new Button();
            resposta.Name = "btnResponder";
            resposta.Text = "Enviar resposta";
            resposta.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            resposta.TextAlign = ContentAlignment.MiddleCenter;
            resposta.FlatStyle = FlatStyle.Flat;
            resposta.BackColor = Color.DodgerBlue;
            resposta.ForeColor = Color.White;
            resposta.Size = new Size(80, 56);
            resposta.Location = new Point(740, 412);
            resposta.Click += new EventHandler(btnResponder_Click);
            if (perg.img.path != "")
            {
                Button imagem = new Button();
                imagem.Name = "btnImagem";
                imagem.Text = "Exibir imagem";
                imagem.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                imagem.TextAlign = ContentAlignment.MiddleCenter;
                imagem.FlatStyle = FlatStyle.Flat;
                imagem.BackColor = Color.DodgerBlue;
                imagem.ForeColor = Color.White;
                imagem.Size = new Size(140, 34);
                imagem.Location = new Point(354, 182);
                imagem.Click += new EventHandler(btnImagem_Click);
                pan.Controls.Add(imagem);
            }
            pan.Controls.Add(titulo);
            pan.Controls.Add(pergunta);
            pan.Controls.Add(resposta);
        }

        //tela com perguntas randomicas
        private void pnRandom_Click(object sender, EventArgs e)
        {
            DialogResult res = DialogResult.Yes;//MessageBox.Show("Deseja saber qual é a matéria e o conteúdo da questão?", "Responda para prosseguir", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            trocaTela("exerRandom");
            pnTelaPerguntasRandom.Controls.Clear();
            try
            {
                NpgsqlConnection conn = FuncoesBancoDeDados.conecta();
                conn.Open();
                string sql = "select count(cdg_pergunta) from tb_perguntas;";
                DbDataReader dados = FuncoesBancoDeDados.select(sql, conn);
                dados.Read();
                this.qtdPerg = int.Parse(dados[0].ToString());
                conn.Close();
                exerRandom(res);
            }
            catch (Exception er) {  }
        }

        private void exerRandom(DialogResult res)
        {
            try
            {
                Random r = new Random();
                int cdgPer;
                while ((cdgPer = r.Next(1, qtdPerg + 1)) == ultPer)
                {
                    continue;
                }
                ultPer = cdgPer;
                NpgsqlConnection conn = FuncoesBancoDeDados.conecta();
                conn.Open();
                string sql = "select dados_pergunta, cdg_pergunta from tb_perguntas where cdg_pergunta = " + cdgPer + ";";
                DbDataReader dados = FuncoesBancoDeDados.select(sql, conn);
                dados.Read();
                passaPergunta(dados, pnTelaPerguntasRandom, res);
            }
            catch (Exception er) {  }
        }

        private void pnAltNomeUsuario_Click(object sender, EventArgs e)
        {
            pnFotoUsuario.BackgroundImage.Dispose();
            pnImgUsuarioTela.BackgroundImage.Dispose();
            TelaAltDados t = new TelaAltDados(1, dados);
            t.ShowDialog();
            if (t.novoDado != "")
            {
                this.dados = t.novoDado;
            }
            insereUsuario(dados);
        }

        private void pnAltEmail_Click(object sender, EventArgs e)
        {
            TelaAltDados t = new TelaAltDados(2, dados);
            t.ShowDialog();
            insereUsuario(dados);
        }

        private void btnAltSenha_Click(object sender, EventArgs e)
        {
            TelaAltDados t = new TelaAltDados(3, dados);
            t.ShowDialog();
        }

        private void pnAltImgUsu_Click(object sender, EventArgs e)
        {
            pnFotoUsuario.BackgroundImage.Dispose();
            pnImgUsuarioTela.BackgroundImage.Dispose();
            TelaAltDados t = new TelaAltDados(4, dados);
            t.ShowDialog();
            insereUsuario(dados);
        }
    }
}
