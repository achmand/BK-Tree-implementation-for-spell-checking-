using System;
using BkTreeSpellChecker.BkTree;

namespace BkTreeSpellChecker.StringMetrics
{
    // a levenshtein distance metric used in the bk tree
    public sealed class BkLevenshteinDistance : IBkMetricSpace<string>
    {
        // gets the distance between the two bk tree nodes
        public int GetDistance(BkTreeNode<string> sourceNode, BkTreeNode<string> targetNode)
        {
            if (sourceNode == null || targetNode == null)
            {
                throw new Exception("Nodes cannot be null.");
            }

            var source = sourceNode.GetElement();
            var target = targetNode.GetElement();

            var distance = LevenshteinDistance(source, target);
            return distance;
        }

        // gets the distance between the two bk tree nodes
        public int GetDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(target))
            {
                throw new Exception("Strings cannot be null.");
            }

            var distance = LevenshteinDistance(source, target);
            return distance;
        }
        
        // implementation of LevenshteinDistance 
        private static int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target) ? 0 : target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return source.Length;
            }

            if (source == target) // same word
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
