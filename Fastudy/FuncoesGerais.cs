using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Fastudy
{
    public static class FuncoesGerais
    {
        public static string encriptar(string str)
        {
            str += "senha";
            string strEncripitada = "";
            SHA256Managed SHhash = new SHA256Managed();
            for (int i = 0; i < 4; i++)
            {
                byte[] HashValue = Encoding.UTF8.GetBytes(str);
                HashValue = SHhash.ComputeHash(HashValue);
                foreach (byte b in HashValue)
                {
                    strEncripitada += String.Format("{0:x2}", b);
                }
                str = strEncripitada;
                strEncripitada = "";
            }
            return str;
        }
    }
}
