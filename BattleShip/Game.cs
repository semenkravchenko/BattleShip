using System;
using System.Collections.Generic;
using System.Threading;

namespace BattleShip
{
    public class Game
    {
        List<Restriction> currentRestrictions = new List<Restriction> { new Restriction(4, 1), new Restriction(3, 2), new Restriction(2, 3), new Restriction(1, 4) };

        public void Start()
        {            
            Console.WriteLine("Морской бой начнётся, стоит Вам нажать любую клавишу. \nПожалуйста, будьте с ним аккуратны.");
            Console.ReadKey();
            Console.WriteLine("Игроку расставить корабли рандомом? \"Y\" - да, \"N\" - нет:");
            bool randomPlacement = Console.ReadLine().ToLower() == "y";

            CoordinateShips(_playerField, randomPlacement);            
            CoordinateShips(_enemyField, true);

            ShowTime();
        }

        private Battlefield _enemyField = new Battlefield(10, false);
        private Battlefield _playerField = new Battlefield(10, true);

        public bool PlaceShipRandomly(Battlefield currentField, int shipSize)
        {
            int xEnd, yEnd;

            Logger.WriteInfo("Ставим корабль рандомом."); 
            Random rndCoord = new Random();
            int xStart = rndCoord.Next(0, 10);
            int yStart = rndCoord.Next(0, 10);
            int formFactor = rndCoord.Next(0, 2);
            if (formFactor == 0)
            {
                xEnd = xStart + shipSize - 1;
                yEnd = yStart;
            }
            else
            {
                xEnd = xStart;
                yEnd = yStart + shipSize - 1;
            }

            return currentField.CreateShip(shipSize, xStart, yStart, xEnd, yEnd);
        }

        public bool PlaceShipManually(Battlefield currentField, int shipSize)
        {
            string inputrow;
            string[] inputSplit;
            int xStart;
            int yStart;
            int xEnd;
            int yEnd;

                if (shipSize == 1)
                {
                    Console.WriteLine("Введите координаты однопалубного корабля:");
                    inputrow = Console.ReadLine();
                    inputSplit = inputrow.Split(',');

                    xStart = Convert.ToInt32(inputSplit[0]);
                    yStart = Convert.ToInt32(inputSplit[1]);
                    xEnd = xStart;
                    yEnd = yStart;
                }

                else
                {
                    Console.WriteLine("Введите координаты начала и конца корабля размером {0} через запятые:", shipSize);
                    inputrow = Console.ReadLine();
                    inputSplit = inputrow.Split(',');

                    xStart = Convert.ToInt32(inputSplit[0]);
                    yStart = Convert.ToInt32(inputSplit[1]);
                    xEnd = Convert.ToInt32(inputSplit[2]);
                    yEnd = Convert.ToInt32(inputSplit[3]);
                }

                return currentField.CreateShip(shipSize, xStart, yStart, xEnd, yEnd);
        }

        private class Restriction
        {
            public int ShipSize { get; private set; }

            public int Quantity { get; private set; }

            public Restriction(int shipSize, int quantity)
            {
                ShipSize = shipSize;
                Quantity = quantity;
            }
        }

        private void CoordinateShips(Battlefield currentField, bool randomPlacement)
        {
            foreach (var currentRestrictionPair in currentRestrictions)
                for (int i = 0; i < currentRestrictionPair.Quantity; i++)
                    CoordinateShip(currentField, currentRestrictionPair.ShipSize, randomPlacement); 
        }

        private void CoordinateShip(Battlefield currentField, int shipSize, bool randomPlacement)
        {
            bool placementResult = false;

            while (!placementResult)
            {                    
                if (randomPlacement)
                    placementResult = PlaceShipRandomly(currentField, shipSize);
                else
                    placementResult = PlaceShipManually(currentField, shipSize);
            }
        }
        

        public bool ManualShooting(Battlefield currentField) //возвращаем попал\не попал для метода "передача хода".
        {
            Console.WriteLine("Введите координаты обстреливаемой точки через запятую.");
            string inputrow = Console.ReadLine();
            string[] inputSplit = inputrow.Split(',');
            int x = Convert.ToInt32(inputSplit[0]);
            int y = Convert.ToInt32(inputSplit[1]);

            return currentField.Shoot(x, y);
        }

        public bool AutomaticShooting(Battlefield currentField)
        {
            Console.WriteLine("Нажмите любую клавишу, чтобы дать противнику выстрелить.");
            Console.ReadKey();

            Random rndShooting = new Random();
            int x = rndShooting.Next(0, 10);
            int y = rndShooting.Next(0, 10);

            return currentField.Shoot(x, y);
        }

        private void redrawSpace()
        {
            Console.Clear();

            _playerField.Draw();
            _enemyField.Draw();

            Logger.Flush();
        }

        private void ShowTime()
        {
            bool isPlayerTurn = true;
            bool isComputerTurn = false;

            redrawSpace();

            while (!_enemyField.IsEveryOneDead && !_playerField.IsEveryOneDead)
            {
                if (isPlayerTurn)
                {
                    Logger.WriteDebug("--- Ход игрока ---");

                    isPlayerTurn = ManualShooting(_enemyField);
                    //isPlayerTurn = AutomaticShooting(_enemyField); //война рандомов
                    isComputerTurn = !isPlayerTurn;
                }
                else if (isComputerTurn)
                {
                    Logger.WriteDebug("--- Ход противника ---");

                    isComputerTurn = AutomaticShooting(_playerField);
                    isPlayerTurn = !isComputerTurn;
                }

                redrawSpace();
            }

            if (_enemyField.IsEveryOneDead)
                Console.WriteLine("Все корабли Enemy были расстреляны. Поздравляем Player с победой.");
            else
                Console.WriteLine("Все корабли Player были расстреляны. Поздравляем Enemy с победой.");

            Console.ReadKey();
        }
    }
}
