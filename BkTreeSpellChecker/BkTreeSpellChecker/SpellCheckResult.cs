using System.Collections.Generic;
using System.Linq;

namespace BkTreeSpellChecker
{
    // results returned when a word is checked for spelling correction
    public sealed class SpellCheckResult
    {
        public string Word { get; set; }
        public bool Found { get; set; }
        public List<string> Suggestions { get; set; } // words suggested by the algorithm

        public SpellCheckResult()
        {
            Suggestions = new List<string>(10); // top 10 suggestions only 
        }

        public string GetResult()
        {
            var result = Found ? "Correct" : $"Incorrect - Suggestions: {string.Join(",", Suggestions.Select(p => p.ToString()).ToArray())}";
            return result;
        }

        public void ResetObject()
        {
            Suggestions.Clear();
            Found = false;
            Word = string.Empty;
        }
    }
}