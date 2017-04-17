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
        private int _size;
        private SpellCheckResult _spellCheckResult;

        #endregion

        #region ctors

        // default constructor 
        public BkTree()
        {
            _stringMetric = new BkLevenshteinDistance();
            _size = 0;
        }

        // inject metric space method 
        public BkTree(IBkMetricSpace<string> bkMetricSpace)
        {
            _stringMetric = bkMetricSpace;
            _size = 0;
        }

        #endregion

        public int GetSize()
        {
            return _size;
        }

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
                _size++;
                return;
            }

            var currentNode = _root;
            var distance = _stringMetric.GetDistance(currentNode.GetElement(), word);
            if (distance == 0) // word already exist 
            {
                return;
            }

            _size += currentNode.AddNode(distance, new BkTreeNode<string>(word), _stringMetric);
        }

        // returns the spell check result 
        // suggestions with a margin of error as specified,
        // would be returned (only top 10)
        public SpellCheckResult SpellCheck(string word, int error)
        {
            if (_spellCheckResult == null)
            {
                _spellCheckResult = new SpellCheckResult();
            }

            _spellCheckResult.ResetObject();
            
            word = word.ToLower();
            SearchTree(_root, _spellCheckResult, word, error);
            return _spellCheckResult;
        }

        private void SearchTree(BkTreeNode<string> root, SpellCheckResult spellCheck, string word, int distance)
        {
            if (spellCheck.Found) // word is found stop recurssion
            {
                return;
            }

            var currentDistance = _stringMetric.GetDistance(root.GetElement(), word);
            var minDistance = currentDistance - distance;
            var maxDistance = currentDistance + distance;

            if (currentDistance == 0)
            {
                spellCheck.Found = true;
                return;
            }

            if (currentDistance <= distance)
            {
                spellCheck.Suggestions.Add(root.GetElement());
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
                SearchTree(node, spellCheck, word, distance);
            }
        }
    }
}
