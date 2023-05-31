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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Diagnostics;
using Microsoft.Extensions.Azure;

namespace Projeto_2_dia
{

    public class scrapper
    {
        public event EventHandler<int> ProgressChanged;
        public event EventHandler<int> ProgressMaximum;
        public static List<string> listaUrl = new List<string>();
        public static List<Produto> newChunk;


        private void GPTbar(object sender, int progress)
        {
            ProgressChanged?.Invoke(sender, progress);
        }

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
        public void Buscarlistaprod(int pagina,Logger logger)
        {

            var urlsite = pagina < 2 ? Program.urlpesquisa : Program.urlpesquisa + "?page=" + pagina;
            try
            {
                driver.Navigate().GoToUrl(urlsite);
            }
            catch (OpenQA.Selenium.WebDriverArgumentException)
            {
                MessageBox.Show("Insira um url", "Erro detetado no input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            var cards = driver.FindElements(By.CssSelector("[data-cy='l-card']"));
            ProgressMaximum?.Invoke(this, cards.Count * 2);
            bool moveToNextCard = false;
            logger.Log("A iniciar a busca de produtos");
            for (int i = 0; i < cards.Count; i++)
            {
                //if (i >= 10) break;
                try
                {
                    var card = cards[i];
                    Produto P = new Produto();
                    var link = card.FindElement(By.TagName("a"));
                    P.link2 = link.GetAttribute("href");
                    bool linkFound = false;
                    for (int j = 0; j < listaUrl.Count; j++)
                    {
                        if (listaUrl[j] == P.link2)
                        {
                            linkFound = true;
                            moveToNextCard = true;
                            break;
                        }
                    }
                    if (linkFound)
                    {
                        Program.cont2 += 1;
                        ProgressChanged?.Invoke(this, Program.cont2);
                    }
                    else
                    {
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
                    if (moveToNextCard)
                    {
                        moveToNextCard = false;
                        continue;
                    }
                }
                catch (OpenQA.Selenium.WebDriverException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            logger.Log($"Busca de produtos terminada, encontrados {Program.listaProdutos.Count} produtos");
        }
        private void BuscarImg(int i, IWebDriver tdriver, Logger logger)
        {
            var produto = Program.listaProdutos[i];
            tdriver.Navigate().GoToUrl(produto.link2);

            // Get element with class swiper-wrapper
            var wrapper = tdriver.FindElement(By.CssSelector("[class='swiper-wrapper']"));
            // Get all images inside the wrapper
            var imgs = wrapper.FindElements(By.TagName("img"));
            foreach (var img in imgs)
            {
                var url = img.GetAttribute("src");
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri validUri))
                {
                    Program.Urls.Add(url);
                    DBimg dbImg = new DBimg(url);
                    produto.DBimgs.Add(dbImg);
                    Program.dbImgQueue.Enqueue(dbImg);
                    logger.Log($"Imagem {url} adicionada ao produto {produto.Nome}");
                }
                else
                {
                    logger.Log($"Imagem {url} nao adicionada ao produto {produto.Nome} ({produto.link2})", LogType.Warning);
                }
            }
        }
        public void Buscardetalhesprod(int i, IWebDriver tdriver, Logger logger)
        {
            try
            {
                var produto = Program.listaProdutos[i];
                tdriver.Navigate().GoToUrl(produto.link2);
                var img = tdriver.FindElement(By.TagName("img"));
                produto.UrlImagem = img.GetAttribute("src");
                BuscarImg(i, tdriver, logger);
                var descri = tdriver.FindElement(By.CssSelector("[class='css-bgzo2k er34gjf0']"));
                produto.Descricao = descri.Text;
                Program.cont2 += 1;
                logger.Log($"Produto {produto.Nome} analisado (desricao e imagens)");
                ProgressChanged?.Invoke(this, Program.cont2);
            }
            catch (OpenQA.Selenium.WebDriverException)
            {
                logger.Log($"Erro ao analisar produto {Program.listaProdutos[i].Nome}", LogType.Error);
            }
        }
        public void Buscardetalhes()
        {
            List<List<int>> listdiv = Program.listaProdutos.SplitList(13);
            List<Thread> threads = new List<Thread>();
            List<IWebDriver> drivers = new List<IWebDriver>();

            var count = 1;
            foreach (List<int> list in listdiv)
            {
                var thread = new Thread(() =>
                {
                    var logger = new Logger($"split-list-{count}");
                    var options = new ChromeOptions();
                    options.AddArguments("headless");
                    var tdriver = new ChromeDriver(ChromeDriverService, options);
                    drivers.Add(tdriver);
                    foreach (var produto in list)
                    {
                        Buscardetalhesprod(produto, tdriver, logger);
                    }
                });
                threads.Add(thread);
                thread.Start();
                count++;
            }
            var thread3 = new Thread(() =>
            {
                var logger = new Logger("Download");
                DownloadThread(logger);
            });
            thread3.Start();

            var thread2 = new Thread(() =>
            {
                var logger = new Logger("end");
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
                        ProgressMaximum?.Invoke(this, (Program.listaProdutos.Count * 4) + Program.Urls.Count);
                        Program.cont2 = 0;
                        ProgressChanged?.Invoke(this, Program.cont2);
                        GPT gpt = new GPT();
                        gpt.ProgressChanged += GPTbar;
                        gpt.GerarCategorias();
                        gpt.separarProdutos();
                        Program.noMoreItems= true;
                        thread3.Join();
                        EnviarBD(logger);
                        logger.Log($"Analise completa clique no botão para analisar outra pagina");
                        ProgressMaximum?.Invoke(this, 1);
                        ProgressChanged?.Invoke(this, 1);
                        MessageBox.Show("Analise completa clique no botão para analisar outra pagina", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch (OpenQA.Selenium.WebDriverException)
                    {
                        logger.Log($"Erro ao fechar o driver", LogType.Error);
                    }
                    catch (System.NullReferenceException)
                    {
                     logger.Log($"Erro ao fechar o driver", LogType.Error);
                    }
                }
                Program.cont1++;
            });
            thread2.Start();

        }
        
        public void EnviarBD(Logger logger)
        {
            using (var connection = new SQLiteConnection("Data Source=Basedados.db"))
            {
                connection.Open();
                foreach (var produto in Program.listaProdutos)
                {
                    string insertProdutoQuery = "INSERT INTO Produtos (Nome, Descricao, Localidade, Preco, Url,Categoria) VALUES (@Nome, @Descricao, @Localidade, @Preco, @Url, @Categoria)";
                    using (SQLiteCommand insertProdutoCommand = new SQLiteCommand(insertProdutoQuery, connection))
                    {
                        insertProdutoCommand.Parameters.AddWithValue("@Nome", produto.Nome);
                        insertProdutoCommand.Parameters.AddWithValue("@Descricao", produto.Descricao);
                        insertProdutoCommand.Parameters.AddWithValue("@Localidade", produto.Localizacao);
                        insertProdutoCommand.Parameters.AddWithValue("@Preco", produto.Preco);
                        insertProdutoCommand.Parameters.AddWithValue("@Url", produto.link2);
                        insertProdutoCommand.Parameters.AddWithValue("@Categoria", produto.Categoria);
                        insertProdutoCommand.ExecuteNonQuery();
                    }
                    using (SQLiteCommand com = new SQLiteCommand("SELECT last_insert_rowid()", connection))
                    {
                        var id = com.ExecuteScalar();
                        produto.Id = id.ToString();
                    }
                    Program.cont2 += 1;
                    ProgressChanged?.Invoke(this, Program.cont2);
                    logger.Log($"Produto {produto.Nome} guardado na base de dados com o id {produto.Id}");
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
                            insertImagemCommand.Parameters.AddWithValue("@Local", img.Local);
                            insertImagemCommand.ExecuteNonQuery();
                            img.ProdutoId = Convert.ToUInt16(produto.Id);
                        }
                        using (SQLiteCommand com = new SQLiteCommand("SELECT last_insert_rowid()", connection))
                        {
                            var id = com.ExecuteScalar();
                            img.Id = int.Parse(id.ToString());
                        }
                        logger.Log($"Imagem {img.Url} pertencendo ao produto {produto.Nome} guardada na base de dados com o id {img.Id}");
                        
                    }
                    Program.cont2 += 1;
                    ProgressChanged?.Invoke(this, Program.cont2);
                }
            }
        }
        public void load2()
        {
            var logger = new Logger("main");

            using (var connection = new SQLiteConnection("Data Source=Basedados.db"))
            {
                connection.Open();
                logger.Log("SQLite Connection opened");


                string selectQuery = "SELECT * FROM Produtos ";
                string selectQuery2 = "SELECT * FROM Imagens WHERE ProdutoId=@ProdutoId ";
                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        int cont4 = 0;
                        while (reader.Read() && cont4 < 52)
                        {
                            Produto produto = new Produto();
                            produto.Nome = reader.GetString(reader.GetOrdinal("Nome"));
                            produto.Descricao = reader.GetString(reader.GetOrdinal("Descricao"));
                            produto.Localizacao = reader.GetString(reader.GetOrdinal("Localidade"));
                            produto.Preco = reader.GetString(reader.GetOrdinal("Preco"));
                            produto.link2 = reader.GetString(reader.GetOrdinal("url"));
                            produto.Id = reader.GetDecimal(reader.GetOrdinal("Id")).ToString();

                            logger.Log($"Loaded product with ID {produto.Id}");

                            using (SQLiteCommand selectCommand2 = new SQLiteCommand(selectQuery2, connection))
                            {
                                selectCommand2.Parameters.AddWithValue("@ProdutoId", produto.Id);
                                using (SQLiteDataReader reader2 = selectCommand2.ExecuteReader())
                                {

                                    while (reader2.Read())
                                    {
                                        var yes = new DBimg(produto.link2);
                                        yes.Local = reader2.GetString(reader2.GetOrdinal("Local"));
                                        yes.Id = reader2.GetInt32(reader2.GetOrdinal("Id"));
                                        yes.ProdutoId = reader2.GetInt32(reader2.GetOrdinal("ProdutoId"));
                                        produto.DBimgs.Add(yes);

                                        logger.Log($"Loaded image with ID {yes.Id} for product with ID {produto.Id}");
                                    }
                                }
                            }
                            Program.listaProdutos_mostrar.Add(produto);
                        }
                    }
                }
                connection.Close();
            }
        }
        public void DownloadThread(Logger logger)
        {
            while (true)
            {
                if (Program.noMoreItems && Program.dbImgQueue.Count == 0)
                {
                    break;
                }

                if (Program.dbImgQueue.Count > 0)
                {
                    var img = Program.dbImgQueue.Dequeue();
                    if (!Uri.TryCreate(img.Url, UriKind.Absolute, out Uri uri))
                    {
                        continue;
                    }
                    using (WebClient client = new WebClient())
                    {

                        string imageFileName = Program.GenerateRandomString(16);


                        string imageFilePath = Path.Combine("Images", imageFileName);

                        if (!Directory.Exists("Images"))
                        {
                            Directory.CreateDirectory("Images");
                        }

                        client.DownloadFile(img.Url, imageFilePath + ".png");
                        img.Local = imageFilePath + ".png";
                        logger.Log($"Imagem {img.Url} foi guardada em {img.Local}");
                    }
                }
                else
                {
                    Thread.Sleep(20);  // Pause the thread for 20 milliseconds
                }
            }
        }

    }
}
