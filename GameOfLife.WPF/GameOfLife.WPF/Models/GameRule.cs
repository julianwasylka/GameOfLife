using System;
using System.Collections.Generic;
using System.Linq;

namespace GameOfLife.WPF.Models
{
    public class GameRule
    {
        public HashSet<int> BirthRules { get; private set; }
        public HashSet<int> SurviveRules { get; private set; }

        private GameRule()
        {
            BirthRules = new HashSet<int>();
            SurviveRules = new HashSet<int>();
        }

        public bool CanBeBorn(int neighbors)
        {
            return BirthRules.Contains(neighbors);
        }

        public bool CanSurvive(int neighbors)
        {
            return SurviveRules.Contains(neighbors);
        }

        public static GameRule Parse(string ruleString)
        {
            var newRule = new GameRule();

            if (string.IsNullOrWhiteSpace(ruleString))
            {
                return Parse("B3/S23");
            }

            try
            {
                // Split into Birth and Survive
                var parts = ruleString.Split('/');
                if (parts.Length != 2) throw new FormatException("Rule string must contain a '/'.");


                var birthPart = parts[0].Trim().ToUpper();
                if (birthPart.StartsWith("B") && birthPart.Length > 1)
                {
                    foreach (char c in birthPart.Substring(1))
                    {
                        if (int.TryParse(c.ToString(), out int n))
                            newRule.BirthRules.Add(n);
                    }
                }

                var survivePart = parts[1].Trim().ToUpper();
                if (survivePart.StartsWith("S") && survivePart.Length > 1)
                {
                    foreach (char c in survivePart.Substring(1))
                    {
                        if (int.TryParse(c.ToString(), out int n))
                            newRule.SurviveRules.Add(n);
                    }
                }

                return newRule;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing rule string: {ex.Message}. Reverting to default.");
                return Parse("B3/S23");
            }
        }

        public override string ToString()
        {
            var b = string.Join("", BirthRules.OrderBy(x => x));
            var s = string.Join("", SurviveRules.OrderBy(x => x));
            return $"B{b}/S{s}";
        }
    }
}