using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto_2_dia
{
    public class GPT
    {
        public event EventHandler<int> ProgressChanged;
        public static List<Produto> produto_GPT = new List<Produto>();
        List<Produto> ParseStringParaProdutos(string str)
        {
            var produto = produto_GPT;
            var linhas = str.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Produto produtoAtual = null;
            foreach (var linha in linhas)
            {
                if (linha.StartsWith("Nome:"))
                {
                    if (produtoAtual != null)
                    {
                        produto_GPT.Add(produtoAtual);
                    }

                    produtoAtual = new Produto();
                    produtoAtual.Nome = linha.Replace("Nome:", "").Trim();
                }
                else if (linha.StartsWith("Valor:"))
                {
                    produtoAtual.Preco = linha.Replace("Valor:", "").Trim();
                }
                else if (linha.StartsWith("Descrição:"))
                {
                    produtoAtual.Descricao = linha.Replace("Descrição:", "").Trim();
                }
            }

            // Adiciona o último produto se ele não tiver sido adicionado
            if (produtoAtual != null)
            {
                produto_GPT.Add(produtoAtual);
            }

            return produto;
        }
        public void GerarCategorias()
        {
            var logger = new Logger("main");
            logger.Log("Gerando categorias...");
            var openAIClient = new OpenAIClient(GPTKey.OPENAI_KEY);
            foreach (var produto in Program.listaProdutos)
            {
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    Messages =
                            {
                                new ChatMessage(ChatRole.User,$"Diz me baseado nestas categorias (Carros, motos e barcos/Imóveis/Bebé e Criança/Lazer/Telemóveis e Tablets/Agricultura/Animais/Desporto/Moda/Móveis, Casa e Jardim/Tecnologia/Emprego/Serviços/Equipamentos e Ferramentas) em qual delas se aplica um produto com este nome:{produto.Nome} e descrição:{produto.Descricao}, responde apenas com o nome da categoria nada mais." ),
                            }
                };
                var response = openAIClient.GetChatCompletions(
                     deploymentOrModelName: "gpt-3.5-turbo",
                     chatCompletionsOptions
                  );

                string tempm = "";
                foreach (var choice in response.Value.Choices)
                {
                    tempm += choice.Message.Content;
                }
                logger.Log($"Produto {produto.Nome} pertence à categoria {tempm}");
                produto.Categoria = tempm;
                Program.cont2 += 1;
                ProgressChanged?.Invoke(this, Program.cont2);
            }
            logger.Log("Categorias geradas");
        }
        public void separarProdutos()
        {
            var logger = new Logger("main");
            var openAIClient = new OpenAIClient(GPTKey.OPENAI_KEY);
            logger.Log("Separando produtos...");
            foreach (var produto in Program.listaProdutos)
            {
                try
                {
                    var chatCompletionsOptions = new ChatCompletionsOptions()
                    {
                        Messages =
                {
                new ChatMessage(ChatRole.User,$"Descrição do Produto: {produto.Descricao}\r\n\r\nA pergunta é: Existem vários produtos à venda nesta descrição,responde apenas com sim ou não?\r\n\r\nResposta: " ),
              }
                    };
                    var response = openAIClient.GetChatCompletions(
                         deploymentOrModelName: "gpt-3.5-turbo",
                         chatCompletionsOptions
                      );
                    string temp1 = "";
                    foreach (var choice in response.Value.Choices)
                    {
                        temp1 += choice.Message.Content;
                    }
                    produto.Categoria = temp1;
                    if (temp1.ToLower().Contains("sim"))
                    {
                        var chatCompletionsOptions2 = new ChatCompletionsOptions()
                        {
                            Messages =
                {
                new ChatMessage(ChatRole.User,$"Descrição do Produto: {produto.Descricao}\r\nUsando a descrição de produto acima extrai todos os sub-produtos correspondetes a este produto no formato seguinte:\r\nNome: Nome do produto\r\nValor: Valor do produto se diponível\r\nDescrição: Pequena descrição se possível\r\n----\r\nNome: xxxx\r\netc...\""),
              }
                        };
                        var response2 = openAIClient.GetChatCompletions(
                             deploymentOrModelName: "gpt-3.5-turbo",
                             chatCompletionsOptions2
                          );
                        string temp2 = "";
                        foreach (var choice in response2.Value.Choices)
                        {
                            temp2 += choice.Message.Content;
                        }
                        Program.temporario = temp2;
                        logger.Log($"Produto {produto.Nome} tem sub-produtos que são: {Program.temporario}");
                        ParseStringParaProdutos(Program.temporario);
                        //wait 20 miliseconds  
                        System.Threading.Thread.Sleep(20);
                    }
                }
                catch (System.AggregateException)
                {
                    logger.Log("Erro ao separar produtos");
                }
            }
            Program.listaProdutos.AddRange(produto_GPT);
            logger.Log($"Produtos separados {Program.listaProdutos.Count}");
        }
    }
}

