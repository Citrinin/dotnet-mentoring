using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConvertStringLib
{
    /// <summary>
    /// Class for converting strings
    /// </summary>
    public class ConvertString
    {
        /// <summary>
        /// Converts string to integer
        /// </summary>
        /// <param name="source">Source string</param>
        /// <returns>Integer value that string contains</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public int ConvertToInt(string source)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentNullException(nameof(source), "String must be not null");
            }

            var regex = new Regex("^[+|-]?[\\d]+$");

            if (!regex.IsMatch(source))
            {
                throw new ArgumentException($"String must contain only numbers or sign symbol. \"{source}\" is vrong value", nameof(source));
            }

            bool positive;

            switch (source[0])
            {
                case '-':
                {
                    positive = false;
                    source = source.Substring(1);
                    break;
                }
                case '+':
                {
                    positive = true;
                    source = source.Substring(1);
                    break;
                }
                default:
                {
                    positive = true;
                    break;
                }
            }

            try
            {
                checked
                {
                    return positive
                        ? source.Select((t, i) =>
                                (int)char.GetNumericValue(t) * (int)Math.Pow(10, source.Length - 1 - i))
                            .Sum()
                        : source.Select((t, i) =>
                                -(int)char.GetNumericValue(t) * (int)Math.Pow(10, source.Length - 1 - i))
                            .Sum();
                }
            }
            catch (OverflowException e)
            {
                throw new ArgumentException($"String \"{source}\" is to long to be int32 value", e);
            }
        }
    }
}

