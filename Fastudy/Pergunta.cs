using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastudy
{
    public class Img
    {
        public string type { get; set; }
        public string path { get; set; }
    }

    public class Answers
    {
        public string letter { get; set; }
        public string value { get; set; }
    }

    public class Pergunta
    {
        public string title { get; set; }
        public string question { get; set; }
        public Img img { get; set; }
        public string opcionalQuestion { get; set; }
        public string correct { get; set; }
        public List<Answers> answers { get; set; }
    }
}
