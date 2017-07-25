using System;
using System.Collections.Generic;

namespace BattleShip
{
    public class Ship
    {
        public bool isAlive = true;
        private int _deckCounter; // счётчик живых палуб
        public List<Coordinate> AureoleDots = new List<Coordinate>(); //list структур координат запретной зоны корабля
        public List<Coordinate> ShipCoordinates = new List<Coordinate>(); //list структур координат корабля

        public Ship(int xStart, int yStart, int xEnd, int yEnd) //конструктор корабля
        {
            for (int x = xStart; x <= xEnd; x++)
                for (int y = yStart; y <= yEnd; y++)
                    ShipCoordinates.Add(new Coordinate(x, y));

            _deckCounter = ShipCoordinates.Count;

            CalculateAureole();
        }

        public bool CheckShot(int x, int y) //должны возвращать состояние попал\не попал\убил. 
        {
            foreach (var currentShipCoord in ShipCoordinates)
                if (currentShipCoord.X == x && currentShipCoord.Y == y)
                {
                    Logger.WriteDebug("В корабль попали!");
                    _deckCounter--; //уменьшаем счётчик живых палуб                

                    if (_deckCounter == 0) //если после попадания палубы закончились, объявляем корабль убитым
                    {
                        Logger.WriteDebug("Корабль потоплен.");
                        isAlive = false;
                    }
                    return true;
                }

            return false;
        }

        private void CalculateAureole() //рассчитываем ореол (запретную зону) для стрельбы
        {
            foreach (var currantCoord in ShipCoordinates)
            {
                for (int k = currantCoord.X - 1; k <= currantCoord.X + 1; k++)
                    for (int j = currantCoord.Y - 1; j <= currantCoord.Y + 1; j++)
                    {
                        AureoleDots.Add(new Coordinate(k, j));
                    }
            }
        }
    }
}
