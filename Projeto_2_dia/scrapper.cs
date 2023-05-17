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

namespace Projeto_2_dia
{

    public class scrapper
    {
        public event EventHandler<int> ProgressChanged;
        public event EventHandler<int> ProgressMaximum;

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

        public void Buscarlistaprod(int pagina)
        {
            ProgressMaximum?.Invoke(this, 104);
            var urlsite = pagina < 2 ? "https://www.olx.pt/tecnologia-e-informatica/videojogos-consolas/" : "https://www.olx.pt/tecnologia-e-informatica/videojogos-consolas/?page=" + pagina;
            driver.Navigate().GoToUrl(urlsite);
            var cards = driver.FindElements(By.CssSelector("[data-cy='l-card']"));
            foreach (var card in cards)
            {
                try
                {
                    Produto P = new Produto();
                    var nome = card.FindElement(By.TagName("h6"));
                    P.Nome = nome.Text;
                    var preco = card.FindElement(By.CssSelector("[data-testid='ad-price']"));
                    P.Preco = preco.Text;
                    var local = card.FindElement(By.CssSelector("[data-testid='location-date']"));
                    P.Localizacao = local.Text;
                    var link = card.FindElement(By.TagName("a"));
                    P.link2 = link.GetAttribute("href");
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
                    }
                    catch (OpenQA.Selenium.WebDriverException) { }
                    catch (System.NullReferenceException) { }
                }

            });
            thread2.Start();
        }
    }
}
