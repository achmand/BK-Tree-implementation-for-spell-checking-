using System;

namespace BkTreeSpellChecker.StringMetrics
{
    // a levenshtein distance metric used in the bk tree
    // for more info visit: https://en.wikipedia.org/wiki/Levenshtein_distance
    // uses single-character edits to calculate distance
    // code taken from: https://rosettacode.org/wiki/Levenshtein_distance (Modified pieces of the code) 
    public sealed class LevenshteinDistance : BaseBkMetricSpace
    {
        public override double ComputeDistance(string target, string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target) ? 0 : target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            if (source == target) 
            {
                return 0;
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
    }
}
