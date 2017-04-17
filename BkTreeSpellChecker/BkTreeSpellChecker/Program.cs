using System;
using System.IO;
using BkTreeSpellChecker.StringMetrics;

namespace BkTreeSpellChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            // using levenshtein distance as a string metric  
            var bkTree = new BkTree.BkTree(new BkLevenshteinDistance());
            LoadTree(bkTree); // this takes some time to load -> TODO -> Some how this takes more time then building the tree on the fly

            // BUILDING THE TREE
            //BuildTree(bkTree);
            //BkTree.BkTree.SaveTree(bkTree, "savedGraph.gp");

            var result = bkTree.SpellCheck("cke", 2);
            Console.WriteLine(result.GetResult());
            Console.ReadLine();
        }

        // used to build tree
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

        private static void LoadTree(BkTree.BkTree bkTree)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Substring(0, path.IndexOf("bin", StringComparison.Ordinal)) + "WordList/savedGraph.gp";
            BkTree.BkTree.LoadTree(bkTree, path);
        }
    }
}
