using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using Chess;

namespace ChessConsole
{
    /// <summary>
    /// This program is used for running performance profiling.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var a = (int)200;
            Console.WriteLine($"{a}: {Convert.ToString(a, 2)}");
            a <<= 1;
            Console.WriteLine($"{a}: {Convert.ToString(a, 2)}");

            Console.ReadLine();
        }

        private void TestPerformance()
        {
            var testClass = new EngineTests();
            try {
                testClass.Setup();
                testClass.TestGamePerformance();
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }
    }
}
