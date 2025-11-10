using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife.WPF.Models
{
    public class Pattern
    {
        public string Name { get; set; }
        public HashSet<Point> Points { get; set; }
    }

    public static class PatternLibrary
    {
        public static List<Pattern> GetPatterns()
        {
            var patterns = new List<Pattern>();

            patterns.Add(new Pattern
            {
                Name = "Szybowiec",
                Points = new HashSet<Point>
                {
                    new Point(1, 0),
                    new Point(2, 1),
                    new Point(0, 2), 
                    new Point(1, 2),
                    new Point(2, 2)
                }
            });

            patterns.Add(new Pattern
            {
                Name = "Blinker",
                Points = new HashSet<Point>
                {
                    new Point(0, 0),
                    new Point(1, 0),
                    new Point(2, 0)
                }
            });

            patterns.Add(new Pattern
            {
                Name = "Blok",
                Points = new HashSet<Point>
                {
                    new Point(0, 0),
                    new Point(1, 0),
                    new Point(0, 1),
                    new Point(1, 1)
                }
            });

            patterns.Add(new Pattern
            {
                Name = "Krokodyl",
                Points = new HashSet<Point>()
                {
                    new Point(0, 1),
                    new Point(1, 1),
                    new Point(2, 0),
                    new Point(2, 2),
                    new Point(3, 1),
                    new Point(4, 1),
                    new Point(5, 1),
                    new Point(6, 1),
                    new Point(7, 0),
                    new Point(7, 2),
                    new Point(8, 1),
                    new Point(9, 1)

                }
            });

            patterns.Add(new Pattern
            {
                Name = "Fontanna",
                Points = new HashSet<Point>
                {
                    new Point(1, 1), new Point(4, 1),
                    new Point(1, 2), new Point(4, 2),
                    new Point(2, 1), new Point(5, 1),
                    new Point(2, 2), new Point(5, 2),
                    new Point(2, 3), new Point(4, 3),
                    new Point(2, 4), new Point(4, 4),
                    new Point(2, 5), new Point(4, 5),
                    new Point(1, 6), new Point(5, 6),
                    new Point(0, 6), new Point(6, 6),
                    new Point(0, 5), new Point(6, 5),
                    new Point(0, 4), new Point(6, 4),

                }
            });

            return patterns;
        }
    }
}
