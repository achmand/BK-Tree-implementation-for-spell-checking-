using System;
using BkTreeSpellChecker.StringMetrics;

namespace BkTreeSpellChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            #region section 6 

            /*
            var bkTree = new BkTree.BkTree(new LevenshteinDistance());
            BkTree.BkTree.BuildTree(bkTree);
            var result = bkTree.SpellCheck("bookes", 2);
            Console.WriteLine(result.GetResultText());
            */

            #endregion

            #region section 7 
            /*
            var bkTree = new BkTree.BkTree(new LevenshteinDistance());
            BkTree.BkTree.BuildTree(bkTree);

            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.IndexOf("bin", StringComparison.Ordinal)) + "Ebooks/ZoneTherapyEBook.txt";
            var result = bkTree.TextSpellCheck(path, 1);
            Console.BufferHeight = 20000;
            Console.WriteLine(result);
            */
            #endregion

            #region section 8

            // using typo metric for touch interfaces 
            /*
            var bkTreeTypo = new BkTree.BkTree(new TypoDistance()); 
            BkTree.BkTree.BuildTree(bkTreeTypo);
            var result = bkTreeTypo.SpellCheck("Eome", 1);
            Console.WriteLine($"Typo Distance: {result.GetResultText()}");

            var bkTree = new BkTree.BkTree(new LevenshteinDistance());
            BkTree.BkTree.BuildTree(bkTree);
            var result2 = bkTree.SpellCheck("Eome", 1);
            Console.WriteLine($"Levenshtein Distance: {result2.GetResultText()}");

            result = bkTreeTypo.SpellCheck("ehllo", 1);
            Console.WriteLine($"Typo Distance: {result.GetResultText()}");
            result2 = bkTree.SpellCheck("ehllo", 1);
            Console.WriteLine($"Levenshtein Distance: {result2.GetResultText()}");
            */

            #endregion

            #region section 9 

            /*
            var bkTreeA = new BkTree.BkTree(new LevenshteinDistance());
            BkTree.BkTree.BuildTree(bkTreeA);

            var bkTreeB = new BkTree.BkTree(new LevenshteinDistance(), true); // heuristic
            BkTree.BkTree.BuildTree(bkTreeB);

            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.IndexOf("bin", StringComparison.Ordinal)) + "Ebooks/ZoneTherapyEBook.txt";

            var stopWatch = new Stopwatch();

            stopWatch.Start();
            bkTreeA.TextSpellCheck(path, 1);
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed.TotalSeconds + " s");
            stopWatch.Reset();

            stopWatch.Start();
            bkTreeB.TextSpellCheckHeuristic(path, 1);
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed.TotalSeconds + " s");
            stopWatch.Reset();
            
            //Console.BufferHeight = 20000;
            //Console.WriteLine(resultA);
            //Console.WriteLine(resultB);
            */

            #endregion

            Console.ReadKey();
        }
    }
}
