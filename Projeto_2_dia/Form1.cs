using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using Projeto_2_dia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Text.Encodings;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;


namespace Projeto_2_dia
{
    
    public partial class Form1 : Form
    {
        public IWebDriver driver { get; set; }
        public ChromeDriverService ChromeDriverService { get; set; }
        public int cont2 = 0;
       
        public Form1()
        {
            InitializeComponent();
            this.ChromeDriverService=ChromeDriverService.CreateDefaultService();
            this.ChromeDriverService.HideCommandPromptWindow = true;
        }
        public void updatebar()
        {
            progressBar1.Value = cont2;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            var options = new ChromeOptions();
            options.AddArguments("headless");
            driver = new ChromeDriver(ChromeDriverService, options);
        }
        
       public void Buscardados(int pagina)
        {
            if (progressBar1.InvokeRequired)
            {
                Action define = delegate { progressBar1.Value = Program.listaProdutos.Count; };
                progressBar1.Invoke(define);
            }
            if (pagina < 2)
            {
                driver.Navigate().GoToUrl("https://www.olx.pt/tecnologia-e-informatica/videojogos-consolas/");
                var cards = driver.FindElements(By.CssSelector("[data-cy='l-card']"));


                foreach (var card in cards)
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
                    cont2 += 1;
                    if (progressBar1.InvokeRequired)
                    {
                        Action define = delegate { updatebar(); };
                        progressBar1.Invoke(define);
                    }

                }
                
                var threadParameters2 = new System.Threading.ThreadStart(delegate {
                    var options = new ChromeOptions();
                    options.AddArguments("headless");
                    var tdriver = new ChromeDriver(ChromeDriverService,options);
                    for (int i = 0; i < 22; i++) {
                        var produto = Program.listaProdutos[i];
                        tdriver.Navigate().GoToUrl(produto.link2);
                        var img = tdriver.FindElement(By.TagName("img"));
                        produto.UrlImagem = img.GetAttribute("src");
                        var descri = tdriver.FindElement(By.CssSelector("[class='css-bgzo2k er34gjf0']"));
                        produto.Descricao = descri.Text;
                        cont2 += 1;
                        if (progressBar1.InvokeRequired)
                        {
                            Action define = delegate { updatebar(); };
                            progressBar1.Invoke(define);
                        }
                    }
                tdriver.Close();
                    tdriver.Dispose();
                });
                var thread4 = new System.Threading.Thread(threadParameters2);
                var threadParameters3 = new System.Threading.ThreadStart(delegate {
                    var options = new ChromeOptions();
                    options.AddArguments("headless");
                    var tdriver = new ChromeDriver(ChromeDriverService, options);
                    for (int i2 = 21; i2 < 52; i2++)
                    {
                        var produto = Program.listaProdutos[i2];
                        tdriver.Navigate().GoToUrl(produto.link2);
                        var img = tdriver.FindElement(By.TagName("img"));
                        produto.UrlImagem = img.GetAttribute("src");
                        var descri = tdriver.FindElement(By.CssSelector("[class='css-bgzo2k er34gjf0']"));
                        produto.Descricao = descri.Text;
                        cont2 += 1;
                        if (progressBar1.InvokeRequired)
                        {
                            Action define = delegate { updatebar(); };
                            progressBar1.Invoke(define);
                        }
                    }
                    tdriver.Close();
                    tdriver.Dispose();
                });
                var thread3 = new System.Threading.Thread(threadParameters2);
                thread3.Start();
                thread4.Start();
            }
            else
            {
                driver.Navigate().GoToUrl("https://www.olx.pt/tecnologia-e-informatica/videojogos-consolas/?page=" + Program.cont1.ToString());
                var cards = driver.FindElements(By.CssSelector("[data-cy='l-card']"));


                foreach (var card in cards)
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
                    if (progressBar1.InvokeRequired)
                    {
                        Action define = delegate { updatebar(); };
                        progressBar1.Invoke(define);
                    }
                }
                var threadParameters2 = new System.Threading.ThreadStart(delegate {
                    var options = new ChromeOptions();
                    options.AddArguments("headless");
                    var tdriver = new ChromeDriver(ChromeDriverService, options);
                    for (int i = 0; i < 22; i++)
                    {
                        var produto = Program.listaProdutos[i];
                        tdriver.Navigate().GoToUrl(produto.link2);
                        var img = tdriver.FindElement(By.TagName("img"));
                        produto.UrlImagem = img.GetAttribute("src");
                        var descri = tdriver.FindElement(By.CssSelector("[class='css-bgzo2k er34gjf0']"));
                        produto.Descricao = descri.Text;
                        cont2 += 1;
                        if (progressBar1.InvokeRequired)
                        {
                            Action define = delegate { updatebar(); };
                            progressBar1.Invoke(define);
                        }
                    }
                    tdriver.Close();
                    tdriver.Dispose();
                });
                var thread4 = new System.Threading.Thread(threadParameters2);
                var threadParameters3 = new System.Threading.ThreadStart(delegate {
                    var options = new ChromeOptions();
                    options.AddArguments("headless");
                    var tdriver = new ChromeDriver(ChromeDriverService, options);
                    for (int i2 = 21; i2 < 52; i2++)
                    {
                        var produto = Program.listaProdutos[i2];
                        tdriver.Navigate().GoToUrl(produto.link2);
                        var img = tdriver.FindElement(By.TagName("img"));
                        produto.UrlImagem = img.GetAttribute("src");
                        var descri = tdriver.FindElement(By.CssSelector("[class='css-bgzo2k er34gjf0']"));
                        produto.Descricao = descri.Text;
                        cont2 += 1;
                        if (progressBar1.InvokeRequired)
                        {
                            Action define = delegate { updatebar(); };
                            progressBar1.Invoke(define);
                        }
                    }
                    tdriver.Close();
                    tdriver.Dispose();
                });
                var thread3 = new System.Threading.Thread(threadParameters2);
                thread3.Start();
                thread4.Start();
            }
        }

        private void controlo_2_dia1_Load(object sender, EventArgs e)
        {
           
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var threadParameters = new System.Threading.ThreadStart(delegate { Buscardados(Program.cont1);  });
            var thread2 = new System.Threading.Thread(threadParameters);
            thread2.Start();


        }

        private void button3_Click(object sender, EventArgs e)
        {
            Controlo_2_dia[] controlo_2_Dias = new Controlo_2_dia[52];
            for (int i = 1; i < 52; i++)
            {
                controlo_2_Dias[i]=new Controlo_2_dia(Program.listaProdutos[i]);

                flowLayoutPanel1.Controls.Add(controlo_2_Dias[i]);
            }
        }

        

        private void button5_Click(object sender, EventArgs e)
        {
            
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string jsonString = JsonSerializer.Serialize(Program.listaProdutos);
                string jsonFilePath = saveFileDialog1.FileName;
                using (FileStream fileStream = new FileStream(jsonFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {

                    byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

                    fileStream.Write(jsonBytes, 0, jsonBytes.Length);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK) 
            {
                string jsonFilePath = openFileDialog1.FileName;
                using (FileStream fileStream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read))
                {
                    TextReader reader = new StreamReader(fileStream);
                    var data = reader.ReadToEnd();
                    reader.Close();
                    Program.listaProdutos = JsonSerializer.Deserialize<List<Produto>>(data);
                }
            }
        }
        static int ParseNumber(string input)
        {
           
            string pattern = @"\d+";

          
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                
                return int.Parse(match.Value);
            }
            else
            {
                
                throw new ArgumentException("No number found in the input string.");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int max = 0;
            int min = 9999999;
            foreach (var produto in Program.listaProdutos)
            {
                int i = ParseNumber(produto.Preco);
                if (i > max)
                { max = i; }
                if (i < min)
                { min = i; }
            }
            MessageBox.Show($"Max: {max}\r Min: {min}");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            driver.Close();
            driver.Dispose();
        }
    }
}
