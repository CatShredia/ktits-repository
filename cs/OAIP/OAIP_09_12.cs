namespace OAIP
{
    internal class OAIP_09_12
    {
        public bool isDevoperEdition;
        public OAIP_09_12(bool isDevoperEdition)
        {
            this.isDevoperEdition = isDevoperEdition;


            int choise = 1;
            while (choise != 0)
            {
                Console.WriteLine("Введите нужное задание, для выхода: 0");
                choise = Convert.ToInt32(Console.ReadLine());

                switch (choise)
                {
                    case 0:
                        Console.WriteLine("Выход");
                        break;
                    case 1:
                        Console.WriteLine("Тестовый Char");
                        charTest();
                        break;
                    default:
                        System.Console.WriteLine("Нет такого задания!");
                        break;
                }
            }
        }

        public void charTest()
        {
            char charTestPer = 'k';
            Console.WriteLine(charTestPer);
            Console.WriteLine((int)charTestPer);
        }
    }
}