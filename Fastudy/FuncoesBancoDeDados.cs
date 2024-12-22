using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace Fastudy
{
    public static class FuncoesBancoDeDados
    {
        public static NpgsqlConnection conecta()
        {
            string conexao = "Server=localhost;Port=5432;User Id=postgres;Password=password;Database=fastudy";
            NpgsqlConnection conn = new NpgsqlConnection(conexao);
            return conn;
        }

        public static NpgsqlConnection conecta(string servidor, string porta, string usuario, string senha, string banco)
        {
            string conexao = "Server=" + servidor + ";Port=" + porta + ";User Id=" + usuario + ";Password=" + senha + ";Database=" + banco; ;
            NpgsqlConnection conn = new NpgsqlConnection(conexao);
            return conn;
        }

        public static NpgsqlDataReader select(string query, NpgsqlConnection conn)
        {
            NpgsqlDataReader dados = null;
            if (conn.State == ConnectionState.Open)
            {
                NpgsqlCommand res = new NpgsqlCommand(query, conn);
                dados = res.ExecuteReader();
            }
            return dados;
        }

        public static int insUpDel(string query, NpgsqlConnection conn)
        {
            NpgsqlCommand dados = new NpgsqlCommand(query, conn);
            return dados.ExecuteNonQuery();
        }
    }
}
