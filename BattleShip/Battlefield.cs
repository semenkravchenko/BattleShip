using System;
using System.Collections.Generic;

namespace BattleShip
{
    public class Battlefield
    {
        private int _size; // размер поля
        private Cell[,] _field; // поле как массив клеток
        private List<Ship> _ships = new List<Ship>();
        private int _totalAliveShipsCount;

        public Battlefield(int size, bool isVisible)
        {
            _size = size;
            _field = new Cell[size, size];

            for (int i = 0; i < _size; i++)   // инициализируем массив объектов Cell
                for (int j = 0; j < _size; j++)
                {
                    _field[i, j] = new Cell();
                    _field[i, j].isVisible = isVisible;
                }
        }

        public bool IsEveryOneDead
        {
            get
            {
                return _totalAliveShipsCount == 0;
            }
        } //свойство. остались ли живые корабли

        public bool Shoot(int x, int y)
        {
            Logger.WriteDebug("Выстрел сделан в клетку " + x + "," + y);
            if (_field[x, y].State == CellState.Empty)
            {
                _field[x, y].State = CellState.Missed;
                Logger.WriteDebug("Мимо");
                return false;
            }

            if (_field[x, y].State == CellState.Shotdown || _field[x, y].State == CellState.Missed)
            {
                Logger.WriteDebug("В клетку " + x + ", " + y + " уже стреляли ранее");
                return true;
            }

            if (_field[x, y].State == CellState.Ship)                                            
            {
                foreach (var currentShip in _ships)
                {
                    if (currentShip.CheckShot(x, y))
                    {
                        _field[x, y].State = CellState.Shotdown;

                        if (!currentShip.isAlive) //если корабль помер, рисуем ореол                        
                        {
                            _totalAliveShipsCount--;
                            foreach (var dot in currentShip.AureoleDots)
                            {
                                if ((dot.X < 0 || dot.Y < 0) || (dot.X >= _size || dot.Y >= _size))
                                    continue;
                                if (_field[dot.X, dot.Y].State == CellState.Shotdown)
                                    continue;
                                _field[dot.X, dot.Y].State = CellState.Missed;
                            }
                        }
                        return true; 
                    }                    
                }
            }

            Logger.WriteError("Мы не должны были увидеть это. Выстрел ушёл в невероятное место.");
            return false;

        }

        public bool CreateShip(int shipSize, int xStart, int yStart, int xEnd, int yEnd) // метод проверки кораблей. должно принимать координаты и возвращать Ship
        {
            if (shipSize <= 0 || xStart < 0 || yStart < 0 || xEnd < 0 || yEnd < 0) //проверяем, пересекают ли корабли 0
            {
                Logger.WriteInfo("Входные данные должны быть > 0");
                return false;
            }

            if (xStart >= _size || yStart >= _size || xEnd >= _size || yEnd >= _size)
            {
                Logger.WriteInfo("Улетели за границы поля.");
                return false;
            }

            if (xStart != xEnd && yStart != yEnd) // проверка формы корабля
            {
                Logger.WriteInfo("Корабль должен располагаться по прямой.");
                return false;
            }

            if ((shipSize - 1) == Math.Abs(xEnd - xStart) || (shipSize - 1) == Math.Abs(yEnd - yStart)) //проверяем, что размер соотносится с координатами.
                Logger.WriteInfo("Всё хорошо.");
            else
            {
                Logger.WriteInfo("Размер корабля не соотносится с его координатами");
                return false;
            }

            Ship currentShip = new Ship(xStart, yStart, xEnd, yEnd);

            foreach (Ship ship in _ships)
                foreach (var coordinate in ship.AureoleDots)
                    foreach (var shipCoord in currentShip.ShipCoordinates)
                        if (shipCoord.X == coordinate.X && shipCoord.Y == coordinate.Y)
                        {
                            Logger.WriteInfo("Нельзя ставить корабли впритык.");
                            return false;
                        }

            Logger.WriteInfo("Корабль размером " + shipSize + " успешно создан.");

            _ships.Add(currentShip);
            _totalAliveShipsCount++;

            foreach (var currentCoord in currentShip.ShipCoordinates)
                _field[currentCoord.X, currentCoord.Y].State = CellState.Ship; //рисуем корабль на поле
            return true;
        }

        public void Draw() //дикий ASCII арт
        {
            Console.Write("  "); //отступ перед горизонтальной строкой цифр

            for (int i = 0; i < _size; i++) // горизонтальная строка цифр
                Console.Write(" " + i);

            Console.WriteLine();

            for (int i = 0; i < _size; i++)
            {
                Console.Write("  "); // отступ перед началом горизонтальных разделительных линий 

                for (int k = 0; k < _size; k++) // горизонтальные разделительные линии
                    Console.Write(" -");

                Console.WriteLine();
                Console.Write(i + " "); //вертикальный столбец цифр

                for (int j = 0; j < _size; j++)
                {
                    //if (_field[j, i].isVisible == true)
                    Console.Write("|" + _field[j, i].CellRepresentation); // выводим содержимое всех клеток. индекы j,i, иначе поле перевернётся                       
                }

                Console.WriteLine("|"); //вертикальные разделительные линии
            }

            Console.Write("  "); //отступ перед последней горизонтальной разделительной линией

            for (int i = 0; i < _size; i++)
                Console.Write(" -"); //последняя горизонтальная разделительная линия 

            Console.WriteLine();
        }
    }
}
