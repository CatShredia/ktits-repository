namespace OAIP
{
    using static System.Console;

    class Player : Object
    {
        public Dictionary<int, int> CountOfShip = new Dictionary<int, int>
        {
            // сколько палуб в корабле, количество у игрока
            { 1, 4 },
            { 2, 3 },
            { 3, 2 },
            { 4, 1 },
        };
    }
}