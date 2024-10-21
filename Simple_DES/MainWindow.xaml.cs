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
        private static int key;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ResultButton_Click(object sender, RoutedEventArgs e)
        {
            data = Default_TextBox.Text.ToCharArray();
            try
            {
                int.TryParse(Key_TextBox.Text, out key);
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
        private readonly static int[] P10_arr = { 2 ,4 ,1 ,6 ,3 ,9 ,0 ,8 ,7 ,5 }; //Перестановка Р10: { 3 5 2 7 4 10 1 9 8 6 }
        private readonly static int[] P8_arr = { 5, 2, 6, 3, 7, 4, 9, 8 }; //Перестановка Р8: { 6 3 7 4 8 5 10 9 - - }
        private readonly static int[] IP_arr = { 1, 5, 2, 0, 3, 7, 4, 6 }; //Перестановка IP: { 2 6 3 1 4 8 5 7 }
        private readonly static int[] E_P_arr = { 3, 0, 1, 2, 1, 2, 3, 0 }; //E/P: { 4 1 2 3 2 3 4 1}
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
            BitArray temp_bitArray = new BitArray(10);

            for(int i = 0; i < 10; i++)
            {
                temp_bitArray[i] = bitArray[i];
            }

            return temp_bitArray;
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

            for(int i = 0; i < length;  i++)
            {
                bitArray_temp[i] = bitArray[P10_arr[i]];  
            }

            return bitArray_temp;
        }

        public static BitArray P8(BitArray bitArray)
        {
            int length = bitArray.Length;
            BitArray bitArray_temp = new BitArray(length - 2);

            for(int i = 0; i < length - 2; i++)
            {
                bitArray_temp[i] = bitArray[P8_arr[i]];
            }

            return bitArray_temp;
        }

        public static BitArray IP(BitArray bitArray)
        {
            int length = bitArray.Length;
            BitArray bitArray_temp = new BitArray(length);

            for(int i = 0; i < length; i++)
            {
                bitArray_temp[i] = bitArray[IP_arr[i]];
            }

            return bitArray_temp;
        }

        public static BitArray E_P (BitArray bitArray)
        {
            int length = bitArray.Length;
            BitArray bitArray_temp = new BitArray(length*2);

            for(int i = 0; i < length*2; i++)
            {
                bitArray_temp[i] = bitArray[E_P_arr[i]];
            }

            return bitArray_temp;
        }

        public static BitArray CSlideLeft (BitArray bitArray)
        {
            int length = bitArray.Length;
            BitArray bitArray_temp = new BitArray(length);
            int halfLength = length / 2;

            if (length % 2 != 0)
            {
                throw new ArgumentException("BitArray length must be even.");
            }

            // Циклический сдвиг первой половины
            bool firstBitFirstHalf = bitArray[0];
            for (int i = 1; i < halfLength; i++)
            {
                bitArray_temp[i - 1] = bitArray[i];
            }
            bitArray_temp[halfLength - 1] = firstBitFirstHalf;

            // Циклический сдвиг второй половины
            bool firstBitSecondHalf = bitArray[halfLength];
            for (int i = halfLength + 1; i < length; i++)
            {
                bitArray_temp[i - 1] = bitArray[i];
            }
            bitArray_temp[length - 1] = firstBitSecondHalf;

            return bitArray_temp; // Возвращаем измененный массив
        }

        public static BitArray Reverse(BitArray bitArray)
        {
            int length = bitArray.Length;
            BitArray bitArray_temp = new BitArray(length);

            for (int i = 0; i < length; i++)
            {
                bitArray_temp[i] = bitArray[length - 1 - i];
            }

            return bitArray_temp;
        }

        public static void FeistelAlgorithm(BitArray bitArray, BitArray key)
        {
            int length = bitArray.Length;
            BitArray bitArrayLeft = new BitArray(length/2);
            BitArray bitArrayRight = new BitArray(length/2);

            for (int i = 0; i < length; i++)
            {
                if(i < length/2)
                {
                    bitArrayLeft[i] = bitArray[i];
                }

                else
                {
                    bitArrayRight[i - length/2] = bitArray[i];
                }
            }

            bitArrayRight = E_P(bitArrayRight); //bitArrayRigth теперь длинною 8
        }

        public static string Cipher(char ch, int key)
        {
            BitArray bitArray_char = new BitArray(8);
            BitArray bitArray_key = new BitArray(10);

            //Подготовка массива битов для финальных ключей, юзнем больше памяти чем дозволено, но повысим читабельность 
            BitArray bitArray_key_final1 = new BitArray(8);
            BitArray bitArray_key_final2 = new BitArray(8);

            //Подготовка массива битов к работе
            bitArray_key = IntToBit(key);
            bitArray_key = Reverse(bitArray_key);
            
            //Перестановка Р10
            bitArray_key = P10(bitArray_key);
            //Циклический сдвиг влево
            bitArray_key = CSlideLeft(bitArray_key);
            //Перестановка Р8
            bitArray_key_final1 = P8(bitArray_key);

            //Двойной циклический сдвиг влево
            bitArray_key = CSlideLeft(bitArray_key);
            bitArray_key = CSlideLeft(bitArray_key);
            //Перестановка Р8
            bitArray_key_final2 = P8(bitArray_key);

            //Подготовка массива битов к работе
            bitArray_char = CharToBit(ch);
            bitArray_char = Reverse(bitArray_char);

            //Перестановка IP
            bitArray_char = IP(bitArray_char);
            FeistelAlgorithm(bitArray_char, bitArray_key_final1);

            return BitToString(bitArray_char);
        }
    }
}
