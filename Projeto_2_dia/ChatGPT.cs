using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto_2_dia
{
    public partial class ChatGPT : Form
    {
        
        private List<Message> messages = new List<Message>();
        private int cont = 0;
        private List<Produto> ProdGPT = new List<Produto>();
        private string Temp2 = "";
        public ChatGPT()
        {
            InitializeComponent();
        }
        static List<string> DividePalavras(string frase)
        {
            List<string> palavras = new List<string>();
            string[] palavrasSeparadas = frase.Split(' ');

            foreach (string palavra in palavrasSeparadas)
            {
                string palavraLimpa = palavra.Trim(new char[] { ' ', ',', '.', '!', '?' });
                if (!string.IsNullOrEmpty(palavraLimpa))
                {
                    palavras.Add(palavraLimpa);
                }
            }
            return palavras;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var message = new Message();
            message.Pergunta = textBox1.Text;
            List<string> ids = new List<string>();
            var openAIClient = new OpenAIClient(GPTKey.OPENAI_KEY);
            List<string> TempP = DividePalavras(textBox1.Text);
            using (var connection = new SQLiteConnection("Data Source=Basedados.db"))
            {
                connection.Open();
                foreach (string palavra in TempP)
                {
                    string selectQuery = "SELECT * FROM Produtos WHERE instr(LOWER(Nome),LOWER(@Palavra)) > 0 OR instr(LOWER(Categoria),LOWER(@Palavra)) > 0 OR instr(LOWER(Localidade),LOWER(@Palavra)) > 0 OR instr(LOWER(Descricao),LOWER(@Palavra)) > 0";
                    using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@Palavra", palavra);
                        using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Produto produto = new Produto();
                                produto.Id = reader.GetInt32(reader.GetOrdinal("Id")).ToString();
                                ids.Add(produto.Id);
                                if(ids.Contains(produto.Id))
                                {
                                    continue;
                                }
                                produto.Nome = reader.GetString(reader.GetOrdinal("Nome"));
                                produto.Descricao = reader.GetString(reader.GetOrdinal("Descricao"));
                                produto.Localizacao = reader.GetString(reader.GetOrdinal("Localidade"));
                                produto.Preco = reader.GetString(reader.GetOrdinal("Preco"));
                                produto.Categoria = reader.GetString(reader.GetOrdinal("Categoria"));
                                ProdGPT.Add(produto);
                            }
                        }
                    }
                }
                foreach (var produto in ProdGPT)
                {
                        Temp2 += $" {produto.Localizacao}, {produto.Descricao},  {produto.Categoria}, {produto.Preco}, {produto.Nome}";
                }
                message.Pergunta = textBox1.Text.Trim();
                message.DatePergunta = DateTime.Now.ToString("h:mm:ss tt");
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    Messages =
             {   
                new ChatMessage (ChatRole.System, $"You are Cloud, You help the user find products based on the provided data base information.\r\n\r\nDatabase information:{Temp2}"),
                new ChatMessage(ChatRole.User,message.Pergunta ),
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
                message.DateResposta = DateTime.Now.ToString("h:mm:ss tt");
                message.Content = tempm;
                messages.Add(message);
                var tempv = messages[cont];
                Font boldFont = new Font(richTextBox1.Font, FontStyle.Bold);
                richTextBox1.SelectionFont = boldFont;
                richTextBox1.AppendText($"Informação base de dados:");
                richTextBox1.SelectionFont = richTextBox1.Font;
                richTextBox1.AppendText($"{Temp2}\r");
                richTextBox1.SelectionFont = boldFont;
                richTextBox1.AppendText($"Horas: ");
                richTextBox1.SelectionFont = richTextBox1.Font;
                richTextBox1.AppendText($"{tempv.DatePergunta}\r");
                richTextBox1.SelectionFont = boldFont;
                richTextBox1.AppendText($"Utilizador: ");
                richTextBox1.SelectionFont = richTextBox1.Font;
                richTextBox1.AppendText($"{tempv.Pergunta}\r");
                richTextBox1.SelectionFont = boldFont;
                richTextBox1.AppendText($"Horas: ");
                richTextBox1.SelectionFont = richTextBox1.Font;
                richTextBox1.AppendText($"{tempv.DateResposta}\r");
                richTextBox1.SelectionFont = boldFont;
                richTextBox1.AppendText($"ChatGPT: ");
                richTextBox1.SelectionFont = richTextBox1.Font;
                richTextBox1.AppendText($"{tempv.Content}\r");
                ProdGPT.Clear();
                Temp2 = "";

            }
        }
    }
}
