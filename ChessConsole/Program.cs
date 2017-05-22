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
            //TestPerformance();
            TestEnginePerformance();
            Console.ReadLine();
        }

        private static void TestPerformance()
        {
            var testClass = new EngineTests();
            try {
                testClass.Setup();
                var watch = Stopwatch.StartNew();
                testClass.TestGamePerformance();
                //testClass.Perft();
                Console.WriteLine(watch.ElapsedMilliseconds);
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        private static void TestEnginePerformance()
        {
            var testClass = new EngineTestsDeepening();
            try
            {
                testClass.Setup();
                var watch = Stopwatch.StartNew();
                //testClass.TestGamePerformance();
                testClass.TestBestMoveBlackToPlay();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

