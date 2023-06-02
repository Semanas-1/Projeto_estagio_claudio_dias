using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_2_dia
{
    public class ProdutoEmbed
    {
        public string ProdutoId { get; set; }
        public Embeddings Embeddings { get; set; }

    }
}
