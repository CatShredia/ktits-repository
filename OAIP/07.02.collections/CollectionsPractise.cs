namespace OAIP
{

    using static System.Console;

    class CollectionsPractise : Object
    {
        public CollectionsPractise(bool isDevelopEdition) {
            WriteLine("В");
            int number = Convert.ToInt32(ReadLine());

            List<int> list = new List<int>{number}; 
        }
    }
}