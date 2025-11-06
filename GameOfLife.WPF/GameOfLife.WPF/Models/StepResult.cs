using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife.WPF.Models
{
    public record StepResult(int Born, int Died);
}
