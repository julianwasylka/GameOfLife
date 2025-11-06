using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife.WPF.Models
{
    public class GameState
    {
        public int GenerationCount { get; private set; }
        public int TotalCellsBorn { get; private set; }
        public int TotalCellsDied { get; private set; }

        public void Update(StepResult result)
        {
            GenerationCount++;
            TotalCellsBorn += result.Born;
            TotalCellsDied += result.Died;
        }

        public void Reset()
        {
            GenerationCount = 0;
            TotalCellsBorn = 0;
            TotalCellsDied = 0;
        }
    }
}
