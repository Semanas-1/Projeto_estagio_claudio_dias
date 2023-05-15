using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto_2_dia
{
    public partial class Controlo_2_dia : UserControl
    {
        private Produto produto;
       
        public Controlo_2_dia(Produto prod)
        {
            InitializeComponent();
            this.produto = prod;
            richTextBox1.Text = produto.Descricao;
            label1.Text = produto.Nome;
            label2.Text = produto.Preco + ", " + produto.Localizacao;
            pictureBox1.ImageLocation = produto.UrlImagem;
        }
        
        private void Controlo_2_dia_Load(object sender, EventArgs e)
        {
            
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
           
            
            
        }
    }
}