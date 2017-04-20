using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using BkTreeSpellChecker.StringMetrics;

namespace BkTreeSpellChecker.BkTree
{
    // used as a result for the text check (.txt)
    public sealed class TextCheckResult
    {
        public List<int> Positions { get; set; }
        public string[] Suggestions { get; set; }

        public TextCheckResult()
        {
            Positions = new List<int>();
        }
    }

    // my implementation of the Burkhard-keller tree (metric tree)
    // bk-trees are usually used for spell checking 
    // for more info visit: https://en.wikipedia.org/wiki/BK-tree
    public sealed class BkTree
    {
        #region properties & variables

        // Properties used for text checking (.txt)
        private Hashtable _textChecker;
        private StringBuilder _stringBuilder;

        private readonly IBkMetricSpace<string> _stringMetric;
        private SpellCheckResult _spellCheckResult;
        private BkTreeNode<string> _root;
        private int _size;

        #endregion

        #region ctors

        // default constructor 
        public BkTree()
        {
            _stringMetric = new LevenshteinDistance();
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

            _spellCheckResult.SetObject(error, word);

            word = word.ToLower();
            SearchTree(_root, _spellCheckResult, word, error);
            return _spellCheckResult;
        }

        // TODO -> Take care of punctuation
        // spell checks a whole txt file, error margin must be specified for suggestions 
        public string TextSpellCheck(string path, int errorMargin)
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
                _spellCheckResult.ResetObject(true);
            }

            var wordSet = new HashSet<string>(); // holds a reference to words found in the text file 
            var lines = File.ReadAllLines(path);

            // each word has a position
            // a new line is also considered and its position is recorded to 
            var position = 0;
            _spellCheckResult.SetObject(errorMargin);

            foreach (var line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var words = line.ToLower().Split(' ');
                    for (var i = 0; i < words.Length; i++)
                    {
                        var w = words[i];
                        if (string.IsNullOrEmpty(w)) // white space found 
                        {
                            continue; // do no thing to not increment position 
                        }

                        if (wordSet.Contains(w))
                        {
                            position++;
                            if (_textChecker.ContainsKey(w))
                            {
                                var positions = ((TextCheckResult)_textChecker[w]).Positions;
                                positions.Add(position);
                            }

                            continue;
                        }

                        wordSet.Add(w);
                        position++;
                        SearchTree(_root, _spellCheckResult, w, errorMargin); // checking word 
                        if (!_spellCheckResult.Found)
                        {
                            var tmpCheckResult = new TextCheckResult { Suggestions = _spellCheckResult.GetResultCopy() };
                            tmpCheckResult.Positions.Add(position);
                            _textChecker.Add(w, tmpCheckResult);
                        }

                        _spellCheckResult.ResetObject(false); // reset spell check result without reseting the margin error
                    }
                }

                else // a new line is found increment position
                {
                    position++;
                }
            }

            return ParseTextChecker();
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

        // parses the _textChecker hashtable to a readable string 
        private string ParseTextChecker()
        {
            if (_stringBuilder == null)
            {
                _stringBuilder = new StringBuilder(); // init sb
            }

            _stringBuilder.Clear(); // clear sb

            // no text checker found 
            if (_textChecker == null)
            {
                throw new NullReferenceException("Text checker is set to null");
            }

            var total = _textChecker.Count;

            // no spelling mistakes 
            if (total == 0)
            {
                _stringBuilder.Append("No spelling mistakes found");
                return _stringBuilder.ToString();
            }

            _stringBuilder.Append($"Total incorrect words {total} \n******************************************\n\n");

            foreach (var key in _textChecker.Keys)
            {
                var result = (TextCheckResult)_textChecker[key];
                var positions = result.Positions;
                var suggestions = result.Suggestions;
                var suggest = suggestions != null;

                _stringBuilder.Append($"Word is -> {key} with {positions.Count} total occurrences." +
                                      $"\n\tFound at position/s: {string.Join("; ", positions)}" 
                                      + (suggest ? $"\n\tSuggestions: {string.Join("; ", suggestions.Where(s => s != null))}" : "\n\tNo suggestions")).Append("\n\n");
            }

            return _stringBuilder.ToString();
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