using Azure.AI.OpenAI;
using OpenQA.Selenium.DevTools.V111.WebAuthn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto_2_dia
{
    public class GPT
    {
        public event EventHandler<int> ProgressChanged;
        public static List<Produto> produto_GPT = new List<Produto>();
        List<Produto> ParseStringParaProdutos(string str, Produto produto2)
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
                        produtoAtual.Localizacao = produto2.Localizacao;
                        produtoAtual.Categoria = produto2.Categoria;
                        produtoAtual.link2 = produto2.link2;
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
                produtoAtual.Localizacao = produto2.Localizacao;
                produtoAtual.Categoria = produto2.Categoria;
                produtoAtual.link2 = produto2.link2;
                produto_GPT.Add(produtoAtual);
            }

            return produto;
        }
        public void GerarCategorias()
        {
            List<Thread> threads = new List<Thread>();
            var logger = new Logger("main");
            logger.Log("Gerando categorias...");

            List<List<int>> listdiv = Program.listaProdutos.SplitList(13);
            foreach (List<int> list in listdiv)
            {
                var thread = new Thread(() =>
                {
                    var openAIClient = new OpenAIClient(GPTKey.OPENAI_KEY);
                    foreach (var produto in list)
                    {
                        GPTcategorias(produto, openAIClient, logger);
                    }
                });
                threads.Add(thread);
                thread.Start();
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            logger.Log("Categorias geradas com sucesso!");
        }
        public void GPTcategorias(int i, OpenAIClient openAIClient, Logger logger)
        {
            var produto = Program.listaProdutos[i];
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
        public void GPTseparar(int i, OpenAIClient openAIClient, Logger logger)
        {
            try
            {
                var produto = Program.listaProdutos[i];
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
                    ParseStringParaProdutos(Program.temporario, produto);
                    //wait 20 miliseconds  
                    System.Threading.Thread.Sleep(20);
                }
                
            }
            catch(Exception ex)
            {
                logger.Log($"Erro ao separar produtos: {ex.Message}");
            }  
        }
            public void separarProdutos()
            {
                var logger = new Logger("main");
                logger.Log("Separando produtos...");
                List<List<int>> listdiv = Program.listaProdutos.SplitList(13);
                List<Thread> threads = new List<Thread>();
                foreach (List<int> list in listdiv)
                {
                    var thread = new Thread(() =>
                    {
                        var openAIClient = new OpenAIClient(GPTKey.OPENAI_KEY);
                        foreach (var produto in list)
                        {
                            GPTseparar(produto, openAIClient, logger);
                        }
                    });
                    thread.Start();
                    threads.Add(thread);
                }
                foreach (var thread in threads)
                {
                    thread.Join();
                }
                logger.Log("Produtos separados com sucesso!");
            }
        }
    }


