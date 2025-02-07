namespace OAIP
{

    using static System.Console;
    using static OAIP_Arrays;

    class CollectionsPractise : Object
    {

        public double CONST = Math.Pow(10, 5);

        public CollectionsPractise(bool isDevelopEdition)
        {
            Random random = new Random();
            WriteLine("Введите N");
            int N = Convert.ToInt32(ReadLine());

            List<int> list = new List<int>();

            for (int i = 0; i < N; i++)
            {
                list.Add(random.Next(Convert.ToInt32(CONST) * -1, Convert.ToInt32(CONST)));
            }

            WriteArray(list, "list-random");

            var result = list.Select((value, index) => new { value, index })
                            .Where(x => x.index % 3 == 0)
                            .Select(x => x.value);
            
            foreach (var number in result)
            {
                WriteLine(number);
            }
        }
    }
}