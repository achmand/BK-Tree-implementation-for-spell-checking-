using System;
using System.Collections.Generic;

namespace BkTreeSpellChecker.StringMetrics
{
    // a damerau levenshtein distance metric used in the bk tree
    // for most info visit: https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance
    // similiar to levenshtein but includes transpositions 
    // NOTE: that for the optimal string alignment distance, the triangle inequality does not hold 
    // (this implementation does not use optimal string alignment)
    // code taken from: https://github.com/wolfgarbe/symspell/blob/master/symspell.cs (Modified pieces of the code) 
    public sealed class DamerauLevenshteinDistance : BaseBkMetricSpace
    {
        public override int ComputeDistance(string source, string target)
        {
            var m = source.Length;
            var n = target.Length;
            var h = new int[m + 2, n + 2];

            var inf = m + n;
            h[0, 0] = inf;
            for (var i = 0; i <= m; i++)
            {
                h[i + 1, 1] = i;
                h[i + 1, 0] = inf;
            }

            for (var j = 0; j <= n; j++)
            {
                h[1, j + 1] = j; h[0, j + 1] = inf;
            }

            var sd = new SortedDictionary<char, int>();
            foreach (var letter in source + target)
            {
                if (!sd.ContainsKey(letter))
                {
                    sd.Add(letter, 0);
                }
            }

            for (var i = 1; i <= m; i++)
            {
                var db = 0;
                for (var j = 1; j <= n; j++)
                {
                    var i1 = sd[target[j - 1]];
                    var j1 = db;

                    if (source[i - 1] == target[j - 1])
                    {
                        h[i + 1, j + 1] = h[i, j];
                        db = j;
                    }
                    else
                    {
                        h[i + 1, j + 1] = Math.Min(h[i, j], Math.Min(h[i + 1, j], h[i, j + 1])) + 1;
                    }

                    h[i + 1, j + 1] = Math.Min(h[i + 1, j + 1], h[i1, j1] + (i - i1 - 1) + 1 + (j - j1 - 1));
                }

                sd[source[i - 1]] = i;
            }

            return h[m + 1, n + 1];
        }
    }
}
