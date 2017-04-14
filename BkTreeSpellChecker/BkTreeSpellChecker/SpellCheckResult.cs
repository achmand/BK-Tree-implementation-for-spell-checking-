using System.Collections.Generic;

namespace BkTreeSpellChecker
{
    // results returned when a word is checked for spelling correction
    public sealed class SpellCheckResult
    {
        public string Result { get; set; } // result message
        public List<string> Suggestions { get; set; } // words suggested by the algorithm

        public SpellCheckResult()
        {
            Suggestions = new List<string>(10); // top 10 suggestions only 
        }
    }
}
