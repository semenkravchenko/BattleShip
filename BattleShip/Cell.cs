namespace BattleShip
{
    public class Cell // клетка. должна уметь сказать, как себя рисовать
    {
        public bool isVisible; //потом поменять
        public CellState State;

        public string CellRepresentation // рисовать в консоли будем char'ами или string'ами
        {
            get
            {
                switch (State)
                {
                    case CellState.Missed: return ".";
                    case CellState.Shotdown: return "X";
                    case CellState.Ship:
                        return isVisible ? "O" : " "; 
                    case CellState.Empty: return " ";
                    default: return "";
                }
            }
        }
    }
}
