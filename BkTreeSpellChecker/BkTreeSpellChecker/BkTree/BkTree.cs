using System.Collections.Generic;
using BkTreeSpellChecker.StringMetrics;

namespace BkTreeSpellChecker.BkTree
{
    // my implementation of the Burkhard-keller tree (metric tree)
    // bk-trees are usually used for spell checking 
    // for more info visit: https://en.wikipedia.org/wiki/BK-tree
    public sealed class BkTree
    {
        #region properties & variables

        private readonly IBkMetricSpace<string> _stringMetric;
        private BkTreeNode<string> _root;

        #endregion

        #region ctors

        // default constructor 
        public BkTree()
        {
            _stringMetric = new BkLevenshteinDistance();
        }

        // inject metric space method 
        public BkTree(IBkMetricSpace<string> bkMetricSpace)
        {
            _stringMetric = bkMetricSpace;
        }

        #endregion

        // adding node to tree 
        public void AddNode(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return;
            }

            word = word.ToLower();
            if (_root == null)
            {
                _root = new BkTreeNode<string>(word); // set the new root node
                return;
            }

            var currentNode = _root;
            var distance = _stringMetric.GetDistance(currentNode.GetElement(), word);
            if (distance == 0) // word already exist 
            {
                return;
            }

            currentNode.AddNode(distance, new BkTreeNode<string>(word), _stringMetric);
        }

        // returns the spell check result 
        // suggestions with a margin of error as specified,
        // would be returned (only top 10)
        public SpellCheckResult SpellCheck(string word, int error)
        {
            var spellCheck = new SpellCheckResult();
            word = word.ToLower();
            SearchTree(_root, spellCheck.Suggestions, word, error);
            return spellCheck;
        }

        private void SearchTree(BkTreeNode<string> root, ICollection<string> suggestions, string word, int distance)
        {
            var currentDistance = _stringMetric.GetDistance(root.GetElement(), word);
            var minDistance = currentDistance - distance;
            var maxDistance = currentDistance + distance;

            if (currentDistance <= distance)
            {
                suggestions.Add(root.GetElement());
            }

            if (root.GetChildren() == null)
            {
                return;
            }

            foreach (var dist in root.GetChildren().Keys)
            {
                if (minDistance > dist || dist > maxDistance)
                {
                    continue;
                }

                var node = root.GetChildren()[dist];
                SearchTree(node, suggestions, word, distance);
            }
        }
    }
}
