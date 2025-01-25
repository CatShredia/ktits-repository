namespace OAIP
{
    using static System.Console;
    class OAIP_Strings
    {
        public static string ReadMoreLines()
        {
            WriteLine("Многострочный ввод: ");
            string str = "";

            while (true)
            {
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    break;
                }
                else
                {
                    str += input + "\n";
                }

            }

            str = str.Substring(0, str.Length - 1);

            return str;
        }
    }
}