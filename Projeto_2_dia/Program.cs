using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto_2_dia
{
    internal static class Program
    {
        public static List<Produto> listaProdutos = new List<Produto>();
        public static List<Produto> listaProdutos_mostrar = new List<Produto>();
        public static List<string> Urls = new List<string>();
        public static int cont1 = 1;
        public static int cont2 = 0;
        public static string urlpesquisa = "";
        private static Random random = new Random();
        private static string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        public static List<List<int>> SplitList<T>(this List<T> me, int size)
        {
            var meIndexes = me.Select((_, i) => i).ToList();
            var list = new List<List<int>>();
            for (int i = 0; i < me.Count; i += size)
                list.Add(meIndexes.GetRange(i, Math.Min(size, me.Count - i)));
            return list;
        }
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        public static string GenerateRandomString(int length)
        {
            char[] stringChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = characters[random.Next(characters.Length)];
            }
            return new String(stringChars);
        }
    }
}
