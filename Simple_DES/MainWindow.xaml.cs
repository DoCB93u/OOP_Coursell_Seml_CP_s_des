// Не забудь УБРАТЬ ЛИШНИЕ БИБЛИОТЕКИ
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
                resultBuilder.Append(S_DES.Cipher(c, key));
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
        private static int[] P10_arr = { 2,4,1,6,3,9,0,8,7,5 }; //Перестановка Р10: { 3 5 2 7 4 10 1 9 8 6 }
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

        public static BitArray IntToBit (int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
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

        public static BitArray P10(BitArray bitArray)
        {
            int length = bitArray.Length;
            BitArray bitArray_temp = new BitArray(length);

            for(int i = 0; i < 10;  i++)
            {
                bitArray_temp[i] = bitArray[P10_arr[i]];  
            }

            return bitArray_temp;
        }

        public static BitArray CSlideLeft (BitArray bitArray)
        {
            int length = bitArray.Length;
            BitArray bitArray_temp = new BitArray(length);

            bool firstBit = bitArray[0];

            for (int i = 1; i < length; i++)
            {
                bitArray_temp[i - 1] = bitArray[i];
            }

            bitArray_temp[length - 1] = firstBit;

            return bitArray_temp;
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

        public static string Cipher(char ch, int key)
        {
            BitArray bitArray_char = new BitArray(8);
            BitArray bitArray_key = new BitArray(10);

            bitArray_key = IntToBit(key);
            bitArray_key = Reverse(bitArray_key);
            bitArray_char = CharToBit(ch);
            bitArray_char = Reverse(bitArray_char);

            bitArray_key = CSlideLeft(bitArray_key);
     
            return BitToString(bitArray_char);
        }
    }

}
