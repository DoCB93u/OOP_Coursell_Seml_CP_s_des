using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simple_DES
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private char[] data;
        private int key;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ResultButton_Click(object sender, RoutedEventArgs e)
        {
            data = Default_TextBox.Text.ToCharArray();
            try
            {
                int.TryParse(Default_TextBox.Text, out key);
            } catch(Exception ex) { MessageBox.Show("Произошла ошибка: " + ex.Message); }
                

            StringBuilder resultBuilder = new StringBuilder();

            foreach (char c in data)
            {
                resultBuilder.Append(S_DES.Cipher(c));
            }

            Result_TextBlock.Text = resultBuilder.ToString();
        }

        public char[] GetData()
        {
            return data;
        }
    }

    public static class S_DES
    {
        public static string CharToBitString(char ch) //потом убрать
        {
            byte[] bytes = Encoding.ASCII.GetBytes(new char[] { ch });
            BitArray bitArray = new BitArray(bytes);
            StringBuilder bitString = new StringBuilder();

            foreach (bool bit in bitArray)
            {
                bitString.Append(bit ? '1' : '0');
            }

            return bitString.ToString();
        }

        public static BitArray CharToBit(char ch)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(new char[] { ch });
            BitArray bitArray = new BitArray(bytes);
            return bitArray;
        }

        public static string BitToString(BitArray bitArray)
        {
            StringBuilder bitString = new StringBuilder();

            foreach (bool bit in bitArray)
            {
                bitString.Append(bit ? '1' : '0');
            }

            return bitString.ToString();
        }

        public static BitArray Reverse(BitArray bitArray)
        {
            int length = bitArray.Length;
            BitArray bitArray_temp = new BitArray(length);

            for (int i = 0; i < 8; i++)
            {
                bitArray_temp[i] = bitArray[length - 1 - i];
            }
            return bitArray_temp;
        }

        public static string Cipher(char ch)
        {
            BitArray bitArray = CharToBit(ch);
            bitArray = Reverse(bitArray);
     
            return BitToString(bitArray);
        }
    }

}
