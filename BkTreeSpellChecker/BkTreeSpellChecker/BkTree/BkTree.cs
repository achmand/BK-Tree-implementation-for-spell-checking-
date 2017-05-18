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

        private readonly char[] _specialCharacters = { ',', '?', '"', '!', '.', ')', '(' };

        private HashSet<string> GlobalWordSet { get; }

        #endregion

        #region ctors

        // default constructor 
        public BkTree(bool useWordSet = false)
        {
            _stringMetric = new LevenshteinDistance();
            _size = 0;

            if (useWordSet)
            {
                GlobalWordSet = new HashSet<string>();
            }
        }

        // inject metric space method 
        public BkTree(IBkMetricSpace<string> bkMetricSpace, bool useWordSet = false)
        {
            _stringMetric = bkMetricSpace;
            _size = 0;

            if (useWordSet)
            {
                GlobalWordSet = new HashSet<string>();
            }
        }

        #endregion

        #region public methods

        // returns the size 
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

            // set the new root node
            if (_root == null)
            {
                _root = new BkTreeNode<string>(word);
                _size++;
                return;
            }

            var currentNode = _root;
            var distance = _stringMetric.GetDistance(currentNode.GetElement(), word);

            // word already exist
            if (distance == 0)
            {
                return;
            }

            GlobalWordSet?.Add(word);
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

        // spell checks a whole txt file, error margin must be specified for suggestions 
        // Without the use of a global hashset/hashtable
        public string TextSpellCheck(string path, int errorMargin)
        {
            if (_textChecker == null)
            {
                _textChecker = new Hashtable(); // TODO -> if already exists clear to be reused
            }

            if (_spellCheckResult == null)
            {
                _spellCheckResult = new SpellCheckResult();
            }
            else
            {
                _spellCheckResult.ResetObject(true);
            }

            // holds a reference to words found in the text file (little hack here)
            var wordSet = new HashSet<string>();
            var lines = File.ReadAllLines(path); // used lines instead of text to cater for large text files 

            // each word has a position
            // a new line is also considered and its position is recorded to 
            var position = 0;
            _spellCheckResult.SetObject(errorMargin);

            for (int x = 0; x < lines.Length; x++)
            {
                var line = lines[x];
                if (!string.IsNullOrEmpty(line))
                {
                    var words = line.ToLower().Split(' ');
                    for (var i = 0; i < words.Length; i++)
                    {
                        var w = words[i];
                        if (string.IsNullOrEmpty(w))
                        {
                            continue;
                        }

                        w = w.Trim(_specialCharacters);
                        position++;

                        if (wordSet.Contains(w))
                        {
                            if (_textChecker.ContainsKey(w))
                            {
                                var positions = ((TextCheckResult)_textChecker[w]).Positions;
                                positions.Add(position);
                            }

                            continue;
                        }

                        wordSet.Add(w);
                        SearchTree(_root, _spellCheckResult, w, errorMargin);
                        if (!_spellCheckResult.Found)
                        {
                            var tmpCheckResult = new TextCheckResult { Suggestions = _spellCheckResult.GetResultCopy() };
                            tmpCheckResult.Positions.Add(position);
                            _textChecker.Add(w, tmpCheckResult);
                        }

                        // reset spell check result without reseting the margin error
                        _spellCheckResult.ResetObject(false);
                    }
                }
                else
                {
                    position++;
                }
            }

            return ParseTextChecker();
        }

        // heuristic added 
        public string TextSpellCheckHeuristic(string path, int errorMargin)
        {
            if (GlobalWordSet == null)
            {
                return TextSpellCheck(path, errorMargin);
            }

            if (_textChecker == null)
            {
                _textChecker = new Hashtable(); // TODO -> if already exists clear to be reused
            }

            if (_spellCheckResult == null)
            {
                _spellCheckResult = new SpellCheckResult();
            }
            else
            {
                _spellCheckResult.ResetObject(true);
            }

            var position = 0;
            var lines = File.ReadAllLines(path); 
            _spellCheckResult.SetObject(errorMargin);

            for (int x = 0; x < lines.Length; x++)
            {
                var line = lines[x];
                if (!string.IsNullOrEmpty(line))
                {
                    var words = line.ToLower().Split(' ');
                    for (var i = 0; i < words.Length; i++)
                    {
                        var w = words[i];
                        if (string.IsNullOrEmpty(w))
                        {
                            continue;
                        }

                        w = w.Trim(_specialCharacters);
                        position++;

                        if (GlobalWordSet.Contains(w))
                        {
                            continue;
                        }

                        if (_textChecker.ContainsKey(w))
                        {
                            var positions = ((TextCheckResult)_textChecker[w]).Positions;
                            positions.Add(position);
                            continue;
                        }

                        SearchTree(_root, _spellCheckResult, w, errorMargin);
                        if (!_spellCheckResult.Found)
                        {
                            var tmpCheckResult = new TextCheckResult { Suggestions = _spellCheckResult.GetResultCopy() };
                            tmpCheckResult.Positions.Add(position);
                            _textChecker.Add(w, tmpCheckResult);
                        }

                        _spellCheckResult.ResetObject(false);
                    }
                }
                else
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
        public static void BuildTree(BkTree bkTree, string path = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                var buildPath = AppDomain.CurrentDomain.BaseDirectory;
                path = buildPath.Substring(0, buildPath.IndexOf("bin", StringComparison.Ordinal)) +
                            "WordList/dictionary.txt";
            }

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