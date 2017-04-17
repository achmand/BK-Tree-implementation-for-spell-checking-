using System;
using System.IO;
using BkTreeSpellChecker.StringMetrics;

namespace BkTreeSpellChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var bkTree = new BkTree.BkTree(new BkLevenshteinDistance()); // using levenshtein distance as a string metric  

            // BUILDING THE TREE
            BuildTree(bkTree);

            var result = bkTree.SpellCheck("cae", 2);
            Console.WriteLine(result.GetResult());
            Console.ReadLine();
        }


        private static void BuildTree(BkTree.BkTree bkTree)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.IndexOf("bin", StringComparison.Ordinal)) + "WordList/dictionary.txt";
            var words = File.ReadAllLines(path);

            if (words.Length <= 0)
            {
                return;
            }

            foreach (var t in words)
            {
                bkTree.AddNode(t);
            }
        }
    }
}
