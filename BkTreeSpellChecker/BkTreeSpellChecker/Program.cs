using System;
using System.Text.RegularExpressions;
using BkTreeSpellChecker.StringMetrics;

namespace BkTreeSpellChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            // using levenshtein distance as a string metric  
            var bkTree = new BkTree.BkTree(new BkLevenshteinDistance());

            // loading tree
            var treepath = AppDomain.CurrentDomain.BaseDirectory;
            treepath = treepath.Substring(0, treepath.IndexOf("bin", StringComparison.Ordinal)) + "WordList/savedGraph.gp";
            BkTree.BkTree.LoadTree(bkTree, treepath);

            /* Section 6 – P3.2 */

            // BUILDING THE TREE
            //var buildPath = AppDomain.CurrentDomain.BaseDirectory;
            //buildPath = buildPath.Substring(0, buildPath.IndexOf("bin", StringComparison.Ordinal)) + "WordList/dictionary.txt";
            //BkTree.BkTree.BuildTree(bkTree, buildPath);
            //BkTree.BkTree.SaveTree(bkTree, "savedGraph.gp");

            //var result = bkTree.SpellCheck("bookes", 2);
            //Console.WriteLine(result.GetResultText());
            //Console.ReadLine();



            /* Section 7 – P4.1 */
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.IndexOf("bin", StringComparison.Ordinal)) + "Ebooks/ZoneTherapyEBook.txt";
            bkTree.TextSpellCheck(path, 1);
            Console.ReadKey();
        }
    }
}
