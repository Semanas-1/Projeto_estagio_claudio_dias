using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto_2_dia
{
    public partial class TextBoxWithPlaceholder : TextBox
    {
        public string Placeholder { get; set; }
        public System.Drawing.Color PlaceholderColor { get; set; }

       bool isPlaceholderVisible = false;
        public new string Text
        {
            get
            {
                if (isPlaceholderVisible)
                {
                    return "";
                }
                else
                {
                    return base.Text;
                }
            }
            set
            {
                base.Text = value;
            }
        }


        public TextBoxWithPlaceholder() : base()
        {
            InitializeComponent();

            GotFocus += TextBoxWithPlaceholder_GotFocus;
            LostFocus += TextBoxWithPlaceholder_LostFocus;
            
            if (string.IsNullOrEmpty(Text))
            {
                base.Text = Placeholder;
                ForeColor = PlaceholderColor;
                isPlaceholderVisible = true;
            }
        }

        private void TextBoxWithPlaceholder_LostFocus(object sender, EventArgs e)
        {
            // if the textbox is empty, show the placeholder
            if (string.IsNullOrEmpty(Text))
            {
                base.Text = Placeholder;
                ForeColor = PlaceholderColor;
                isPlaceholderVisible = true;
            }
        }

        private void TextBoxWithPlaceholder_GotFocus(object sender, EventArgs e)
        {
            // if the placeholder is visible, hide it
            if (isPlaceholderVisible)
            {
                base.Text = "";
                ForeColor = System.Drawing.Color.Black;
                isPlaceholderVisible = false;
            }
        }

        public TextBoxWithPlaceholder(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
