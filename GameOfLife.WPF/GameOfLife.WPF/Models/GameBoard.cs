using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife.WPF.Models
{
    public class GameBoard
    {
        public GameRule Rules { get; set; }
        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;
        public HashSet<Point> AliveCells { get; private set; } = new HashSet<Point>();

        private Dictionary<Point, int> _currentNeighbours = new Dictionary<Point, int>();
        private Random _random = new Random();

        public StepResult CalculateNextStep()
        {
            _currentNeighbours.Clear();

            //check each adjacent cell
            foreach (var cell in AliveCells)
            {
                AddNeighbour(cell.X-1, cell.Y);
                AddNeighbour(cell.X-1, cell.Y+1);
                AddNeighbour(cell.X-1, cell.Y-1);
                AddNeighbour(cell.X, cell.Y+1);
                AddNeighbour(cell.X, cell.Y-1);
                AddNeighbour(cell.X+1, cell.Y);
                AddNeighbour(cell.X+1, cell.Y+1);
                AddNeighbour(cell.X+1, cell.Y-1);
            }

            int currentBorn = 0, currentDied = 0;
            HashSet<Point> nextGeneration = new HashSet<Point>();

            // check if alive cells will survive
            foreach (var cell in AliveCells)
            {
                _currentNeighbours.TryGetValue(cell, out int count);

                if (Rules.CanSurvive(count)) nextGeneration.Add(cell);
                else currentDied++;
            }

            // check if new cells can be born
            foreach (var entry in _currentNeighbours)
            {
                var cell = entry.Key;
                var count = entry.Value;

                if (!AliveCells.Contains(cell))
                {
                    if (Rules.CanBeBorn(count))
                    {
                        nextGeneration.Add(cell);
                        currentBorn++;
                    }
                }
            }

            AliveCells = nextGeneration;
            return new StepResult(currentBorn, currentDied);
        }

        private bool CheckBoundaries(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height) return true;
            return false;
        }

        private void AddNeighbour(int x, int y)
        {
            if (CheckBoundaries(x, y))
            {
                Point p = new Point(x, y);
                _currentNeighbours.TryGetValue(p, out int currentCount);
                _currentNeighbours[p] = currentCount + 1;
            }
        }

        public void GenerateRandom(double density = 0.2)
        {
            ClearBoard();

            if (density < 0) density = 0;
            else if (density > 1) density = 1;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (_random.NextDouble() < density)
                    {
                        AliveCells.Add(new Point(x, y));
                    }
                }
            }
        }

        public void Resize(int newWidth, int newHeight)
        {
            Width = newWidth;
            Height = newHeight;

            var cellsToRemove = AliveCells.Where(cell =>
                cell.X >= Width || cell.Y >= Height
            ).ToList();

            foreach (var cell in cellsToRemove)
            {
                AliveCells.Remove(cell);
            }
        }

        public void ClearBoard()
        {
            AliveCells.Clear();
        }
    }
}