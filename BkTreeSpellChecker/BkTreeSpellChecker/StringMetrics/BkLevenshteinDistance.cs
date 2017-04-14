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

            var distance = StringMetrics.LevenshteinDistance(source, target);
            return distance;
        }

        // gets the distance between the two bk tree nodes
        public int GetDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(target))
            {
                throw new Exception("Strings cannot be null.");
            }

            var distance = StringMetrics.LevenshteinDistance(source, target);
            return distance;
        }
    }
}
