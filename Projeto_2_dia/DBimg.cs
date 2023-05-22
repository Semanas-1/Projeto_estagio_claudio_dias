using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_2_dia
{
    public class DBimg
    {
        public DBimg(string url)
        {
            Id = -1;
            ProdutoId = -1;
            Url = url;
            Local = "";
        }
        public int Id {  get; set; }
        public int ProdutoId { get; set; }
        public string  Url { get; set; }
        public string Local { get; set;}
    }
}
