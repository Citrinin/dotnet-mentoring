using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstSymbol
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Type any string: ");
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Wrong input. String must be at least 1 char length");
                    continue;
                }
                var result = input[0];
                Console.WriteLine($"First char: {result}");
            }
        }
    }
}
