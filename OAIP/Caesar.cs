// Этот класс реализует шифр Цезаря для шифрования строк на русском языке. 
// Пользователь может ввести период шифрования и строку, которую нужно зашифровать. 
// Программа поддерживает преобразование букв в нижний регистр и выводит информацию о смене регистра.

using System;
namespace OAIP
{
    internal class Caesar
    {
        public string alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        public string str;
        public char[] newStr;
        public int period = 1;
        public Caesar(bool isDevoperEdition)
        {
            Console.WriteLine("Шифр Цезаря");
            while (period != 0)
            {
                Console.WriteLine("Введите период, для выхода: 0");
                period = Convert.ToInt32(Console.ReadLine());
                if (period == 0) {
                    break;
                }
                Console.WriteLine("Введите Строку");
                str = Console.ReadLine();
                // str = "Привет";
                newStr = str.ToCharArray();
                alphabet += alphabet.ToLower();
                Encrypt(isDevoperEdition);
                Console.WriteLine(newStr);
            }
        }
        private void Encrypt(bool isDevoperEdition)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (alphabet.IndexOf(str[i]) + period >= alphabet.Length)
                {
                    newStr[i] = alphabet[period - 1];
                    if (isDevoperEdition
                    )
                    {
                        Console.WriteLine(newStr[i].ToString().ToUpper());
                        Console.WriteLine(newStr[i].ToString());
                    }
                    if (newStr[i].ToString().ToLower() != newStr[i].ToString())
                    {
                        Console.WriteLine("смена");
                        newStr[i] = Char.ToLower(newStr[i]);
                    }
                }
                else
                {
                    newStr[i] = alphabet[alphabet.IndexOf(str[i]) + period];
                }
            }
        }
    }
}