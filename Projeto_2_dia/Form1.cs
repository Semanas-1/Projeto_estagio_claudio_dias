﻿using OpenQA.Selenium;
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
using System.Diagnostics;
using System.Net;


namespace Projeto_2_dia
{

    public partial class Form1 : Form
    {
        private scrapper scrape;
        public GPT gpt = new GPT();
        private void scrape_progresschanged(object sender, int progress)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action(() => progressBar1.Value = progress));
            }
            else
            {
                progressBar1.Value = progress;
            }
        }
        private void scrape_max(object sender, int max)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action(() => progressBar1.Maximum = max));
            }
            else
            {
                progressBar1.Maximum = max;
            }
        }

        private void FlowLoad()
        {
            if (Program.listaProdutos_mostrar.Count == 0)
            {
                return;
            }   

            flowLayoutPanel1.Controls.Clear();
            Controlo_2_dia[] controlo_2_Dias = new Controlo_2_dia[52];
            for (int i = 1; i < 52; i++)
            {
                controlo_2_Dias[i] = new Controlo_2_dia(Program.listaProdutos_mostrar[i]);
                try
                {
                    flowLayoutPanel1.Controls.Add(controlo_2_Dias[i]);
                }catch (System.IndexOutOfRangeException) { }
            }
        }
        public Form1()
        {
            InitializeComponent();
            scrape = new scrapper();
            scrape.ProgressChanged += scrape_progresschanged;
            scrape.ProgressMaximum += scrape_max;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            scrape.load();
            scrape.load2();
            scrape.CreateEmbedings();
            FlowLoad();
        }


        private void controlo_2_dia1_Load(object sender, EventArgs e)
        {

        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (progressBar1.InvokeRequired)
            {
                Action define = delegate { progressBar1.Maximum = Program.listaProdutos.Count * 2; };
                progressBar1.Invoke(define);
            }
            if (progressBar1.InvokeRequired)
            {
                Action define = delegate { progressBar1.Value = 0; };
                progressBar1.Invoke(define);
            }
            Program.urlpesquisa = textBox1.Text;
            var threadParameters = new System.Threading.ThreadStart(delegate {
                var logger = new Logger("Pesquisa");
                scrape.analisarUrls(); scrape.Buscarlistaprod(Program.cont1,logger); scrape.Buscardetalhes(); });
            var thread2 = new System.Threading.Thread(threadParameters);
            thread2.Start(); 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string jsonFilePath = saveFileDialog1.FileName;
                string folderPath = Path.GetDirectoryName(jsonFilePath);
                string jsonFileName = Path.GetFileName(jsonFilePath);
                string imagesFolder = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(jsonFileName));

                
                Directory.CreateDirectory(imagesFolder);
                List<string> validUrls = new List<string>();

                foreach (var url in Program.Urls)
                {
                    if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                    {
                        validUrls.Add(url);
                    }
                }
                progressBar2.Maximum = validUrls.Count;
                int cont3 = 0;
                foreach (var imageUrl in validUrls)
                {
                    using (WebClient client = new WebClient())
                    {
                        
                        string imageFileName = Path.GetFileName(imageUrl);

                       
                        string imageFilePath = Path.Combine(imagesFolder, imageFileName);

                        
                        client.DownloadFile(imageUrl, imageFilePath + ".png");
                    }
                    cont3++;
                    progressBar2.Value = cont3;
                }

                
                string jsonString = JsonSerializer.Serialize(Program.listaProdutos);

                
                using (FileStream fileStream = new FileStream(jsonFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
                    fileStream.Write(jsonBytes, 0, jsonBytes.Length);
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
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
            try
            {
                scrape.driver.Close();
                scrape.driver.Dispose();
            }
            catch (OpenQA.Selenium.WebDriverArgumentException){ }
        }

        private void startNewFormLogs()
        {

            FormLogs formLogs = new FormLogs();
            formLogs.Show();


            formLogs.FormClosed += (s, args) =>
            {
                formLogs.Dispose();
            };

            System.Windows.Forms.Application.Run(formLogs);
        }

        private void openLogsExternalButton_Click(object sender, EventArgs e)
        {
            // Open formLogs in it's own thread so it doesn't block this UI
            // when the multiple threads are working on different things

            var thread = new Thread(startNewFormLogs);
            thread.Start();

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            ChatGPT chatGPT = new ChatGPT();
            chatGPT.Show();
        }
    }
}