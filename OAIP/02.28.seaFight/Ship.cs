// Класс Ship представляет собой корабль с палубами. Он содержит словарь Decks, который хранит палубы корабля, 
// и целочисленное поле LiveDeck, которое указывает количество неуничтоженных палуб. 
// Конструктор инициализирует словарь Decks пустым массивом.
namespace OAIP
{
    using static System.Console;

    class Ship
    {
        // палубы корабля
        public Dictionary<int, int[]> Decks;
        // Количество неуничтоженных палуб
        public int LiveDeck;
        public Ship()
        {
            Decks = new Dictionary<int, int[]>();
        }
    }
}