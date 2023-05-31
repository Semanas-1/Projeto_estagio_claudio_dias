using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto_2_dia
{
    public partial class FormLogs : Form
    {
        public FormLogs()
        {
            InitializeComponent();
        }
        private void LogBoxLogic()
        {
            string input = searchInput.Text;
            string logbox = LogBox.Text;
            int firstIndex = logbox.IndexOf(input);
            int nextIndex = logbox.IndexOf(input, LogBox.SelectionStart + LogBox.SelectionLength);


            // make all occurences of the search term yellow highlight
            LogBox.SelectAll();
            LogBox.SelectionBackColor = Color.White;

            var occurences = Regex.Matches(logbox, input);
            foreach (Match m in occurences)
            {
                LogBox.Select(m.Index, input.Length);
                LogBox.SelectionBackColor = Color.Yellow;
            }

            if (nextIndex >= 0)
            {
                LogBox.Select(nextIndex, input.Length);
                LogBox.ScrollToCaret();
                // make selection a darker tone of yellow
                LogBox.SelectionBackColor = Color.DarkOrange;

            }
            else if (firstIndex >= 0)
            {
                LogBox.Select(firstIndex, input.Length);
                LogBox.ScrollToCaret();
                // make selection a darker tone of yellow
                LogBox.SelectionBackColor = Color.DarkOrange;
            }
            label1.Text = $"Found {occurences.Count} occurences of \"{input}\"";
        }

        private void FormLogs_Load(object sender, EventArgs e)
        {
            ShowLogs();
            Program.LogAdded += Program_LogAdded;
        }
        private void AddLogToLogBox(Log log)
        {
            // make sure to run on the richtextox's thread
            if (LogBox.InvokeRequired)
            {
                LogBox.Invoke(new Action(() => AddLogToLogBox(log)));
                return;
            }

            if (log.Type == LogType.Info)
            {
                LogBox.SelectionColor = Color.Black;
            }
            else if (log.Type == LogType.Error)
            {
                LogBox.SelectionColor = Color.Red;
            }
            else if (log.Type == LogType.Warning)
            {
                LogBox.SelectionColor = Color.Orange;
            }
            try
            {
                LogBox.AppendText($"[{log.Time}] [{log.ThreadName}] {log.Message}\n");
            }
            catch (Exception )
            {
                
            }

            // If the richtextbox is scrolled to the bottom, scroll it down so the new line is visible.
            // If not, we let it be
            if (LogBox.SelectionStart >= LogBox.TextLength - 1)
            {
                try
                {
                    LogBox.ScrollToCaret();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private void Program_LogAdded(object sender, Log log)
        {
            AddLogToLogBox(log);
        }

        private void ShowLogs()
        {
            // clear the textbox
            LogBox.Clear();

            foreach (var log in Program.logs)
            {
                AddLogToLogBox(log);
            }

            // Scroll the richtextbox down
            LogBox.ScrollToCaret();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            LogBoxLogic();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FormLogs_KeyDown(object sender, KeyEventArgs e)
        {
            //if the user presses enter , search 
            if (e.KeyCode == Keys.Enter)
            {
                LogBoxLogic();
            }
        }
    }
}
