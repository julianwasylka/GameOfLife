using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameOfLife.WPF.Models
{
    public class GameBoard
    {
        public string GameRules { get; set; } = "B3/S23";
        public int Width { get; set; } = 50;
        public int Height { get; set; } = 50;
        public HashSet<Point> AliveCells { get; private set; } = new HashSet<Point>();

        // TODO
        public StepResult CalculateNextStep()
        {
            throw new NotImplementedException();
        }

        public void GenerateRandom()
        {
            throw new NotImplementedException();
        }

        public void ClearBoard()
        {
            throw new NotImplementedException();
        }
    }
}