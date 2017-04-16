using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Predictive
{
    static class AffixUtils
    {
        public static bool TryMatch(string affix, string patternToMatch, double minValue, double maxValue, out double normalizedWeight)
        {
            Match m = Regex.Match(affix, patternToMatch);
            if (m.Success)
            {
                if (double.TryParse(m.Groups[1].Value, out double result))
                {
                    if (minValue == maxValue)
                    {
                        //Set it to 100% when there is no range.
                        normalizedWeight = 1;
                    }
                    else
                    {
                        //Reserve 25% for the fact that a value exists, even if the value is low
                        normalizedWeight = 0.25;

                        var fromMin = (result - minValue);
                        normalizedWeight += (fromMin / (maxValue - minValue)) * 0.75;
                    }
                }
                else
                {
                    //Diagnostics:
                    Console.WriteLine($"Explicit matched but value could not be parsed to double: {m.Groups[0].Value} in {affix}.");
                    normalizedWeight = 0;
                }
            }
            else
            {
                normalizedWeight = 0;
            }
            return m.Success;
        }
    }
}
