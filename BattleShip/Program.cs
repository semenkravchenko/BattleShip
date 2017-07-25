using System;
using System.Collections.Generic;
using System.Threading;

namespace BattleShip
{
    class Program
    {
        static void Main(string[] args)
        {
            Game battle = new Game();
            battle.Start();
        }
    }
}

