using System;
using BkTreeSpellChecker.StringMetrics;

namespace BkTreeSpellChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            // using levenshtein distance as a string metric  
            var bkTree = new BkTree.BkTree(new LevenshteinDistance());

            // load treeo (GC is much higher when loading from file)
            var treepath = AppDomain.CurrentDomain.BaseDirectory;
            treepath = treepath.Substring(0, treepath.IndexOf("bin", StringComparison.Ordinal)) + "WordList/savedGraph.gp";
            BkTree.BkTree.LoadTree(bkTree, treepath);

            // BUILDING THE TREE
            //var buildPath = AppDomain.CurrentDomain.BaseDirectory;
            //buildPath = buildPath.Substring(0, buildPath.IndexOf("bin", StringComparison.Ordinal)) +
            //            "WordList/dictionary.txt";
            //BkTree.BkTree.BuildTree(bkTree, buildPath);
            //BkTree.BkTree.SaveTree(bkTree, "savedGraph.gp"); UNCOMMENT TO SAVE TREE TO FILE 

            /* Section 6 – P3.2 */
            //var result = bkTree.SpellCheck("bookes", 2);
            //Console.WriteLine(result.GetResultText());

            /* Section 7 – P4.1 */
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.IndexOf("bin", StringComparison.Ordinal)) + "Ebooks/ZoneTherapyEBook.txt";
            var result = bkTree.TextSpellCheck(path, 1);
            Console.BufferHeight = 20000;
            Console.WriteLine(result);

            Console.ReadKey();
        }

     }
}
