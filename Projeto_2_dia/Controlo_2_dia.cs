﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Projeto_2_dia
{
    public partial class Controlo_2_dia : UserControl
    {
        private Produto produto;
        private int imagemCont = 0;

        public Controlo_2_dia(Produto prod)
        {
            InitializeComponent();
            this.produto = prod;
            carregarDados();
            this.produto.PropertyChanged += Produto_PropertyChanged;
        }

        private void Produto_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            carregarDados();
        }

        public void carregarDados()
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action(() =>
            {
                mostrar();
            }));
            }
            else { mostrar(); }
        }
        public void mostrar()
        {
            richTextBox1.Text = produto.Descricao;
            label1.Text = produto.Nome;
            label2.Text = produto.Preco + ", " + produto.Localizacao;
            if (produto.DBimgs.Count > 0) 
            {
                pictureBox1.ImageLocation = produto.DBimgs[0].Local;
            }
            label3.Text = produto.DBimgs.Count > 1 ? $"1/{produto.DBimgs.Count}" : "";
        }
        private void Controlo_2_dia_Load(object sender, EventArgs e)
        {


        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (produto.DBimgs.Count < 2)
                return;
            int proxCont = (this.imagemCont+1)%this.produto.DBimgs.Count;
            this.imagemCont = proxCont;
            pictureBox1.ImageLocation = this.produto.DBimgs[imagemCont].Local;
            label3.Text =$"{proxCont + 1}/{produto.DBimgs.Count }";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (produto.DBimgs.Count < 2)
                return;
            int proxCont = (this.imagemCont - 1 + this.produto.DBimgs.Count) % this.produto.DBimgs.Count;
            this.imagemCont = proxCont;
            pictureBox1.ImageLocation = this.produto.DBimgs[imagemCont].Local;
            label3.Text = $"{proxCont + 1}/{produto.DBimgs.Count }";
        }
    }
}