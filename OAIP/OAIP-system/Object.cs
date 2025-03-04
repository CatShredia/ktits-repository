namespace OAIP
{

    using static System.Console;

    class Object
    {
        public static ConsoleColor defaultForeground = ConsoleColor.Gray;
        public static ConsoleColor defaultBackground = ConsoleColor.Black;

        public static void PrintWithColor(string str, ConsoleColor foregrColor, ConsoleColor backgrColor)
        {
            ForegroundColor = foregrColor;
            BackgroundColor = backgrColor;
            Write(str);
            SetDefaultColor();
            Write("\n");
        }
        public static void SetDefaultColor()
        {
            ForegroundColor = defaultForeground;
            BackgroundColor = defaultBackground;
        }

        // пиолучение индекса символа, не зная регистр
        public static int GetLetterIndexIgnoreCase(char c)
        {
            char upperCased = char.ToUpper(c);

            if (char.IsLetter(upperCased))
            {
                return upperCased - 'A' + 1;
            }
            else
            {
                return -1;
            }
        }
    }
}