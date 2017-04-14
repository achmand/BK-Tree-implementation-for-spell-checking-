using System;

namespace BkTreeSpellChecker.StringMetrics
{
    // different types of string metrics
    // for more info visit -> https://en.wikipedia.org/wiki/String_metric
    public static class StringMetrics
    {
        #region Levenshtein Distance

        // implementation of Levenshtein Distance (aka Edit distance)
        // distance between two strings is defined as the minimum 
        // number of edits needed to transform one string into the other
        // for more info visit -> https://en.wikipedia.org/wiki/Levenshtein_distance
        // code taken from: https://en.wikibooks.org/wiki/Algorithm_Implementation/Strings/Levenshtein_distance#C.23
        // modified some pieces of code
        public static int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target) ? 0 : target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            if (source.Length > target.Length)
            {
                var temp = target;
                target = source;
                source = temp;
            }

            var m = target.Length;
            var n = source.Length;
            var distance = new int[2, m + 1];

            // Initialize the distance 'matrix'
            for (var j = 1; j <= m; j++)
            {
                distance[0, j] = j;
            }

            var currentRow = 0;
            for (var i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                var previousRow = currentRow ^ 1;
                for (var j = 1; j <= m; j++)
                {
                    var cost = target[j - 1] == source[i - 1] ? 0 : 1;
                    distance[currentRow, j] = Math.Min(Math.Min(
                        distance[previousRow, j] + 1,
                        distance[currentRow, j - 1] + 1),
                        distance[previousRow, j - 1] + cost);
                }
            }

            return distance[currentRow, m];
        }

        #endregion

        #region Hamming distance

        // implementation of Hamming distance
        // measures the minimum number of substitutions required to change one string into the other
        // unlike the Edit distance, this only limited to substitutions 
        // it fundamentally assumes that both strings are of the same length 
        // for more info visit -> https://en.wikipedia.org/wiki/Hamming_distance
        public static int HammingDistance(string source, string target)
        {
            return 0; // TODO -> Implement hamming distance !
        }

        #endregion
    }
}
