namespace OAIP
{
    using static System.Console;

    class SeaFight : Object {
        public SeaFight(bool isDevelop) {

            // Level enemyLevel = new Level("противника");

            WriteLine();

            Level playerLevel = new Level("игрока");
            
            ReadKey();
        }
    }
}   