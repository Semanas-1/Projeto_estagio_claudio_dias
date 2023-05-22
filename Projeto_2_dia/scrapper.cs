using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using System.Net.Http;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using System.Data.SQLite;
using System.Net;
using System.Security.Policy;

namespace Projeto_2_dia
{

    public class scrapper
    {
        public event EventHandler<int> ProgressChanged;
        public event EventHandler<int> ProgressMaximum;
        public static List<string> listaUrl = new List<string>();
        public static List<Produto> newChunk;
        public scrapper()
        {
            this.ChromeDriverService = ChromeDriverService.CreateDefaultService();
            this.ChromeDriverService.HideCommandPromptWindow = true;
        }
        public IWebDriver driver { get; set; }
        public ChromeDriverService ChromeDriverService { get; set; }
        public void load()
        {
            var options = new ChromeOptions();
            options.AddArguments("headless");
            driver = new ChromeDriver(ChromeDriverService, options);
        }
        public void analisarUrls()
        {
            using (var connection = new SQLiteConnection("Data Source=Basedados.db"))
            {
                connection.Open();
                string selectQuery = "SELECT url FROM Produtos";
                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaUrl.Add(reader.GetString(reader.GetOrdinal("url")));
                        }
                    }
                }
                connection.Close();
            }
        }
        public void Buscarlistaprod(int pagina)
        {
            
            var urlsite = pagina < 2 ? Program.urlpesquisa : Program.urlpesquisa + "?page=" + pagina;
            try
            {
                driver.Navigate().GoToUrl(urlsite);
            }
            catch (OpenQA.Selenium.WebDriverArgumentException)
            { 
            MessageBox.Show("Insira um url","Erro detetado no input",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            var cards = driver.FindElements(By.CssSelector("[data-cy='l-card']"));
            for (int i = 0; i < cards.Count; i++)
            {
                if (i >= 10) break;
                try
                {
                    var card = cards[i];
                    Produto P = new Produto();
                    var link = card.FindElement(By.TagName("a"));
                    P.link2 = link.GetAttribute("href");
                    bool linkFound = false;
                    for(i=0;i<listaUrl.Count;i++)
                    {

                        if (listaUrl[i] == P.link2)
                        {
                            linkFound = true;
                            break;
                        }
                    }
                    if (linkFound)
                    {
                        break; 
                    }
                    var nome = card.FindElement(By.TagName("h6"));
                    P.Nome = nome.Text;
                    var preco = card.FindElement(By.CssSelector("[data-testid='ad-price']"));
                    P.Preco = preco.Text;
                    var local = card.FindElement(By.CssSelector("[data-testid='location-date']"));
                    P.Localizacao = local.Text;
                    Program.listaProdutos.Add(P);
                    Program.cont2 += 1;
                    ProgressChanged?.Invoke(this, Program.cont2);
                }
                catch (OpenQA.Selenium.WebDriverException e)
                {
                    MessageBox.Show(e.Message);
                }
            }

        }
        private void BuscarImg(int i, IWebDriver tdriver)
        {

            var produto = Program.listaProdutos[i];
            tdriver.Navigate().GoToUrl(produto.link2);
            var cards = tdriver.FindElements(By.CssSelector("[class='swiper-wrapper'] img"));
            foreach (var card in cards)
            {
                var url = card.GetAttribute("src");
                Program.Urls.Add(url);
                produto.DBimgs.Add(new DBimg(url));
            }
        }
        public void Buscardetalhesprod(int i, IWebDriver tdriver)
        {
            try
            {
                var produto = Program.listaProdutos[i];
                tdriver.Navigate().GoToUrl(produto.link2);
                var img = tdriver.FindElement(By.TagName("img"));
                produto.UrlImagem = img.GetAttribute("src");
                BuscarImg(i, tdriver);
                var descri = tdriver.FindElement(By.CssSelector("[class='css-bgzo2k er34gjf0']"));
                produto.Descricao = descri.Text;
                Program.cont2 += 1;
                ProgressChanged?.Invoke(this, Program.cont2);
            }
            catch (OpenQA.Selenium.WebDriverException)
            {

            }
        }
        public void Buscardetalhes()
        {
            List<List<int>> listdiv = Program.listaProdutos.SplitList(13);
            List<Thread> threads = new List<Thread>();
            List<IWebDriver> drivers = new List<IWebDriver>();
            foreach (List<int> list in listdiv)
            {
                var thread = new Thread(() =>
                {
                    var options = new ChromeOptions();
                    options.AddArguments("headless");
                    var tdriver = new ChromeDriver(ChromeDriverService, options);
                    drivers.Add(tdriver);
                    foreach (var produto in list)
                    {
                        Buscardetalhesprod(produto, tdriver);
                    }
                });
                threads.Add(thread);
                thread.Start();
            }
            var thread2 = new Thread(() =>
            {
                foreach (var thread in threads)
                {
                    thread.Join();
                }
                foreach (var driver in drivers)
                {
                    try
                    {
                        driver.Close();
                        driver.Dispose();
                        driver.Quit();
                        EnviarBDimg();
                        EnviarBD();
                        Program.cont1++;
                    }
                    catch (OpenQA.Selenium.WebDriverException) { }
                    catch (System.NullReferenceException) { }
                }

            });
            thread2.Start();

        }
        public void EnviarBDimg()
        {
            List<string> validUrls = new List<string>();
            foreach (var url in Program.Urls)
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                {
                    validUrls.Add(url);
                }
            }
            foreach (var imageUrl in validUrls)
            {
                using (WebClient client = new WebClient())
                {

                    string imageFileName = Path.GetFileName(imageUrl);


                    string imageFilePath = Path.Combine("Images", imageFileName);


                    client.DownloadFile(imageUrl, imageFilePath + ".png");
                }
            }
          
            foreach(var produto in Program.listaProdutos)
            {
                foreach(var img in produto.DBimgs)
                {
                   if(!Uri.TryCreate(img.Url, UriKind.Absolute, out Uri uri))
                    {
                        continue;
                    }
                    using (WebClient client = new WebClient())
                    {

                        string imageFileName = Path.GetFileName(img.Url);


                        string imageFilePath = Path.Combine("Images", imageFileName);


                        client.DownloadFile(img.Url, imageFilePath + ".png");
                        img.Local = imageFilePath + ".png";
                    }
                }
            }

        }
        public void EnviarBD()
        {
            using (var connection = new SQLiteConnection("Data Source=Basedados.db"))
            {
                connection.Open();
                foreach (var produto in Program.listaProdutos)
                {
                    string insertProdutoQuery = "INSERT INTO Produtos (Nome, Descricao, Localidade, Preco, url) VALUES (@Nome, @Descricao, @Localidade, @Preco, @url)";
                    using (SQLiteCommand insertProdutoCommand = new SQLiteCommand(insertProdutoQuery, connection))
                    {
                        insertProdutoCommand.Parameters.AddWithValue("@Nome", produto.Nome);
                        insertProdutoCommand.Parameters.AddWithValue("@Descricao", produto.Descricao);
                        insertProdutoCommand.Parameters.AddWithValue("@Localidade", produto.Localizacao);
                        insertProdutoCommand.Parameters.AddWithValue("@Preco", produto.Preco);
                        insertProdutoCommand.Parameters.AddWithValue("@url", produto.link2);
                        insertProdutoCommand.ExecuteNonQuery();
                    }
                    using (SQLiteCommand com = new SQLiteCommand("SELECT last_insert_rowid()", connection))
                    {
                        var id = com.ExecuteScalar();
                        produto.Id = id.ToString();
                    }
                }
                foreach (var produto in Program.listaProdutos)
                {
                    foreach (var img in produto.DBimgs)
                    {
                        string insertImagemQuery = "INSERT INTO Imagens (Url,ProdutoId,Local) VALUES (@Url,@ProdutoId,@Local)";
                        using (SQLiteCommand insertImagemCommand = new SQLiteCommand(insertImagemQuery, connection))
                        {
                            insertImagemCommand.Parameters.AddWithValue("@Url", img.Url);
                            insertImagemCommand.Parameters.AddWithValue("@ProdutoId", produto.Id);
                            insertImagemCommand.Parameters.AddWithValue("@Local",img.Local);
                            insertImagemCommand.ExecuteNonQuery();
                            //img.ProdutoId = produto.Id;
                        }
                    }
                }
            }
        }
        public void load2()
        {
            using (var connection = new SQLiteConnection("Data Source=Basedados.db"))
            {
                connection.Open();


                string selectQuery = "SELECT * FROM Produtos";

                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        int cont4= 0;
                        while (reader.Read() && cont4 < 52)
                        {
                             Produto produto = new Produto();
                             produto.Nome = reader.GetString(reader.GetOrdinal("Nome"));
                             produto.Descricao = reader.GetString(reader.GetOrdinal("Descricao"));
                             produto.Localizacao = reader.GetString(reader.GetOrdinal("Localidade"));
                             produto.Preco =reader.GetString(reader.GetOrdinal("Preco"));
                             produto.link2 = reader.GetString(reader.GetOrdinal("url"));
                             Program.listaProdutos_mostrar.Add(produto);
                             cont4++;
                        }
                    }
                }
                connection.Close();
            }
        }
    }
}
