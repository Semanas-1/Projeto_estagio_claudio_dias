using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto_2_dia
{
    public class GPT
    {
        public event EventHandler<int> ProgressChanged;
        public void GerarCategorias()
        {
            var openAIClient = new OpenAIClient("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");
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
                produto.Categoria = tempm;
                Program.cont2 += 1;
                ProgressChanged?.Invoke(this, Program.cont2);
            }
        }
        public void separarProdutos()
        {
            var openAIClient = new OpenAIClient("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");
            foreach (var produto in Program.listaProdutos)
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
                if (temp1 == "sim")
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
                }
            }
        }
    }
}
