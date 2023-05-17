using Projeto_2_dia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_2_dia
{
    public class Produto : INotifyPropertyChanged
    {
        private string _descricao = "";
        private string _urlimagem = "";
        public string Nome { get; set; }
        public string Descricao
        {
            get { return this._descricao; }
            set { this._descricao = value; NotifyPropertyChanged(); }
        }
        public string Preco { get; set; }
        public string Localizacao { get; set; }
        public string UrlImagem
        {
            get { return this._urlimagem; }
            set { this._urlimagem = value; NotifyPropertyChanged(); }
        }

       
        public string link2 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public List<string> Urls = new List<string>();

    }
}
