using System;
using BkTreeSpellChecker.StringMetrics;

namespace BkTreeSpellChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var bkTree = new BkTree.BkTree(new BkLevenshteinDistance());
            bkTree.AddNode("book");
            bkTree.AddNode("books");
            bkTree.AddNode("boo");
            bkTree.AddNode("boon");
            bkTree.AddNode("cook");
            bkTree.AddNode("cake");
            bkTree.AddNode("cape");
            bkTree.AddNode("cart");

            var result = bkTree.SpellCheck("boa", 2);
            Console.ReadLine();
        }
    }
}
