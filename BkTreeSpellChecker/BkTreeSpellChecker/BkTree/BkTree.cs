using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BkTreeSpellChecker.StringMetrics;

namespace BkTreeSpellChecker.BkTree
{
    public sealed class TextCheckResult
    {
        public List<int> Positions { get; set; }
        public string[] Suggestions { get; set; }
    }

    // my implementation of the Burkhard-keller tree (metric tree)
    // bk-trees are usually used for spell checking 
    // for more info visit: https://en.wikipedia.org/wiki/BK-tree
    public sealed class BkTree
    {
        #region properties & variables

        private Hashtable _textChecker;

        private readonly IBkMetricSpace<string> _stringMetric;
        private SpellCheckResult _spellCheckResult;
        private BkTreeNode<string> _root;
        private int _size;

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

        #region public methods

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
        public SpellCheckResult SpellCheck(string word, int error)
        {
            if (_spellCheckResult == null)
            {
                _spellCheckResult = new SpellCheckResult();
            }

            _spellCheckResult.SetObject(word, error);

            word = word.ToLower();
            SearchTree(_root, _spellCheckResult, word, error);
            return _spellCheckResult;
        }

        // spell checks a whole txt file, error margin must be specified for suggestions 
        public void TextSpellCheck(string path, int errorMargin)
        {
            if (_textChecker == null)
            {
                _textChecker = new Hashtable();
            }

            if (_spellCheckResult == null)
            {
                _spellCheckResult = new SpellCheckResult();
            }

            else
            {
                _spellCheckResult.ResetObject();
            }

            var wordSet = new HashSet<string>(); // change in the distinction
            var lines = File.ReadAllLines(path);

            // each word has a position
            // a new line is also considered and its position is recorded to 
            var position = 0;

            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var words = line.ToLower().Split(' ');
                    foreach (var w in words)
                    {
                        if (wordSet.Contains(w))
                        {
                            position++;

                            if (_textChecker.ContainsKey(w))
                            {
                                var positions = (TextCheckResult)_textChecker[w];
                                positions.Positions.Add(position);
                            }

                            continue;
                        }

                        wordSet.Add(w);
                        position++;
                        SearchTree(_root, _spellCheckResult, w, errorMargin); // checking word 
                        if (!_spellCheckResult.Found)
                        {
                            _textChecker.Add(w, new TextCheckResult { Positions = new List<int>(position), Suggestions = _spellCheckResult.GetResultArray() });
                        }

                        _spellCheckResult.ResetObject();
                    }
                }
                else // a new line is found increment position
                {
                    position++;
                }
            }

        }

        #endregion

        #region private methods

        // BK-Trees have a search time of O(log n)
        private void SearchTree(BkTreeNode<string> root, SpellCheckResult spellCheck, string word, int distance)
        {
            // word is found stop recurssion
            if (spellCheck.Found)
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
                if (spellCheck.Suggestions.ContainsKey(currentDistance))
                {
                    spellCheck.Suggestions[currentDistance].Add(root.GetElement());
                }
                else
                {
                    spellCheck.Suggestions.Add(currentDistance, new List<string> { root.GetElement() });
                }
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

        #endregion

        #region static methods

        // TODO -> Serialize the whole tree instead of the root only as there are other properties which need to be persisted like size !!!

        public static void SaveTree(BkTree tree, string filename)
        {
            using (Stream file = File.Open(filename, FileMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(file, tree._root);
            }
        }

        public static void LoadTree(BkTree tree, string filename)
        {
            using (Stream file = File.Open(filename, FileMode.Open))
            {
                var binaryFormatter = new BinaryFormatter();
                var node = binaryFormatter.Deserialize(file) as BkTreeNode<string>;
                tree._root = node;
            }
        }

        // used to build tree
        public static void BuildTree(BkTree bkTree, string path)
        {
            var words = File.ReadAllLines(path);
            if (words.Length <= 0)
            {
                return;
            }

            foreach (var t in words)
            {
                bkTree.AddNode(t);
            }
        }

        #endregion
    }
}