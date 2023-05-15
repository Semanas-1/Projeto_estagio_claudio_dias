﻿using OpenQA.Selenium.Chrome;
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

namespace Projeto_2_dia
{
    public class scrapper
    {
        public event EventHandler<int> ProgressChanged;
        public event EventHandler<int> ProgressMaximum;
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
        public void close() 
        {
            driver.Close();
            driver.Dispose();
        }
        public void Buscardados(int pagina) 
        {
                ProgressMaximum?.Invoke(this,104);
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
                    catch(OpenQA.Selenium.WebDriverException e)  
                   { 
                    MessageBox.Show(e.Message);
                   }
                }
            var threadParameters2 = new System.Threading.ThreadStart(delegate {
                    var options = new ChromeOptions();
                    options.AddArguments("headless");
                    var tdriver = new ChromeDriver(ChromeDriverService, options);
                    var tent = 0;
                    for (int i = 0; i < Program.listaProdutos.Count/2; i++)
                    {
                    try
                     {
                        var produto = Program.listaProdutos[i];
                        tdriver.Navigate().GoToUrl(produto.link2);
                        var img = tdriver.FindElement(By.TagName("img"));
                        produto.UrlImagem = img.GetAttribute("src");
                        var descri = tdriver.FindElement(By.CssSelector("[class='css-bgzo2k er34gjf0']"));
                        produto.Descricao = descri.Text;
                        Program.cont2 += 1;
                        ProgressChanged?.Invoke(this, Program.cont2);
                        tent = 0;
                     }
                    catch (OpenQA.Selenium.WebDriverException ) { }
                    catch(Exception e)
                    {
                        tent++;
                        if(tent<3)
                        {
                            i--;
                        }
                    }
                    }
                try
                {
                    tdriver.Close();
                    tdriver.Dispose();
                }
                catch(OpenQA.Selenium.WebDriverException) { }
            });
            var threadParameters3 = new System.Threading.ThreadStart(delegate {
                    var options = new ChromeOptions();
                    options.AddArguments("headless");
                    var tdriver = new ChromeDriver(ChromeDriverService, options);
                     var tent = 0;
                for (int i2 = Program.listaProdutos.Count / 2; i2 < Program.listaProdutos.Count; i2++)
                {
                    try
                    {
                        var produto = Program.listaProdutos[i2];
                        tdriver.Navigate().GoToUrl(produto.link2);
                        var img = tdriver.FindElement(By.TagName("img"));
                        produto.UrlImagem = img.GetAttribute("src");
                        var descri = tdriver.FindElement(By.CssSelector("[class='css-bgzo2k er34gjf0']"));
                        produto.Descricao = descri.Text;
                        Program.cont2 += 1;
                        ProgressChanged?.Invoke(this, Program.cont2);
                        tent = 0;
                    }
                    catch (OpenQA.Selenium.WebDriverException) { }
                    catch (Exception e)
                    {
                        tent++;
                        if (tent < 3)
                        {
                            i2--;
                        }
                    }
                }
                try
                {
                    tdriver.Close();
                    tdriver.Dispose();
                }
                catch (OpenQA.Selenium.WebDriverException) { }
            });
                var thread3 = new System.Threading.Thread(threadParameters2);
                var thread4 = new System.Threading.Thread(threadParameters3);
                thread3.Start();
                thread4.Start();
        }
    }
}