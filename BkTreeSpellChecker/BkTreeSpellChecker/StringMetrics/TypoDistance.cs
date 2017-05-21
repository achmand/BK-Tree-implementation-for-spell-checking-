using System;
using System.Collections.Generic;

namespace BkTreeSpellChecker.StringMetrics
{
    public enum TouchLayoutType
    {
        Small,
        Standard
    }
    public enum AdjacentSide
    {
        Up,
        Down,
        Left,
        Right,
        DownLeft,
        DownRight,
        UpLeft,
        UpRight
    }

    // my implementation of key distance
    public sealed class KeyDistance
    {
        #region properties and variables 

        private const double SDist = 0.25D;
        private const double MDist = 0.50D;

        private const int TotalKeys = 28;
        private readonly TouchLayoutType _touchLayoutType;
        private readonly Dictionary<char, Dictionary<char, double>> _adjacentKeys;

        private readonly char[][] _qwertyLayout =
        {
            new[] {'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p'},
            new[] {'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l'},
            new[] {'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.'}
        };

        #endregion

        #region constructors 

        public KeyDistance(TouchLayoutType touchLayoutType)
        {
            _touchLayoutType = touchLayoutType;
            _adjacentKeys = new Dictionary<char, Dictionary<char, double>>(TotalKeys);
            BuildAdjKeys();
        }

        #endregion

        #region private methods 

        // TODO -> Add other layout
        private void BuildAdjKeys()
        {
            if (_touchLayoutType == TouchLayoutType.Standard)
            {
                for (var x = 0; x < _qwertyLayout.Length; x++)
                {
                    var layer = _qwertyLayout[x];
                    for (var y = 0; y < layer.Length; y++)
                    {
                        var key = layer[y];
                        BuildStandard(layer, key, x, y);
                    }
                }
            }
        }

        private void BuildStandard(IReadOnlyCollection<char> layer, char key, int x, int y)
        {
            _adjacentKeys[key] = new Dictionary<char, double>();
            if (y > 0 && y < layer.Count - 1)
            {
                var left = GetAdjacent(AdjacentSide.Left, x, y);
                _adjacentKeys[key].Add(left, SDist);

                var right = GetAdjacent(AdjacentSide.Right, x, y);
                _adjacentKeys[key].Add(right, SDist);

                if (x > 1)
                {
                    return;
                }

                var down = GetAdjacent(AdjacentSide.Down, x, y);
                _adjacentKeys[key].Add(down, MDist);

                var downL = GetAdjacent(AdjacentSide.DownLeft, x, y);
                _adjacentKeys[key].Add(downL, MDist);
            }
            else
            {
                var isFirst = y == 0;
                var side = isFirst ? AdjacentSide.Right : AdjacentSide.Left;
                var down = isFirst && x == 0 ? AdjacentSide.Down : AdjacentSide.DownLeft;

                if (x == 1)
                {
                    down = AdjacentSide.Down;
                }

                var sideKey = GetAdjacent(side, x, y);
                _adjacentKeys[key].Add(sideKey, SDist);

                if (x <= 1)
                {
                    var downKey = GetAdjacent(down, x, y);
                    _adjacentKeys[key].Add(downKey, MDist);
                }

                if (x != 1 || isFirst)
                {
                    return;
                }

                var downL = GetAdjacent(AdjacentSide.DownLeft, x, y);
                _adjacentKeys[key].Add(downL, MDist);
            }
        }


        private char GetAdjacent(AdjacentSide adjacentSide, int x, int y)
        {
            switch (adjacentSide)
            {
                case AdjacentSide.Left:
                    return _qwertyLayout[x][y - 1];
                case AdjacentSide.Right:
                    return _qwertyLayout[x][y + 1];
                case AdjacentSide.Down:
                    return _qwertyLayout[x + 1][y];
                case AdjacentSide.Up:
                    return _qwertyLayout[x - 1][y];
                case AdjacentSide.DownLeft:
                    return _qwertyLayout[x + 1][y - 1];
                case AdjacentSide.DownRight:
                    return _qwertyLayout[x + 1][y + 1];
                case AdjacentSide.UpLeft:
                    return _qwertyLayout[x - 1][y - 1];
                case AdjacentSide.UpRight:
                    return _qwertyLayout[x - 1][y + 1];
                default:
                    throw new ArgumentOutOfRangeException(nameof(adjacentSide), adjacentSide, null);
            }
        }

        #endregion

        #region public methods

        public double GetTypoDistance(char source, char target)
        {
            if (source == target)
            {
                return 0;
            }

            if (!_adjacentKeys.ContainsKey(source))
            {
                return 1;
            }

            var sourceAdj = _adjacentKeys[source];
            if (sourceAdj.ContainsKey(target))
            {
                return sourceAdj[target];
            }

            if (!_adjacentKeys.ContainsKey(target))
            {
                return 1;
            }

            var targetAdj = _adjacentKeys[target];
            return targetAdj.ContainsKey(source) ? targetAdj[source] : 1;
        }

        #endregion
    }

    // Using a weighted Damerau Levenshtein 
    // setting weights to each key and its adjacent keys
    public sealed class TypoDistance : BaseBkMetricSpace
    {
        #region properties and variables 

        private KeyDistance KeyDistance { get; }

        #endregion

        #region ctors

        public TypoDistance()
        {
            RateOfChange = 0.25D;
            
            // using a standard touch keyboard
            KeyDistance = new KeyDistance(TouchLayoutType.Standard);
        }

        #endregion

        #region public methods 

        // Source: http://www.csharpstar.com/csharp-string-distance-algorithm/
        // Code modifed from source + added cost of typo mistakes 
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

            var bounds = new { Height = source.Length + 1, Width = target.Length + 1 };
            var matrix = new double[bounds.Height, bounds.Width];
            for (var height = 0; height < bounds.Height; height++)
            {
                matrix[height, 0] = height;
            }

            for (var width = 0; width < bounds.Width; width++)
            {
                matrix[0, width] = width;
            }

            for (var height = 1; height < bounds.Height; height++)
            {
                for (var width = 1; width < bounds.Width; width++)
                {
                    //check cost of hitting adjacent key instead of key
                    var cost = source[height - 1] == target[width - 1] ? 0 : KeyDistance.GetTypoDistance(source[height - 1], target[width - 1]);
                    var insertion = matrix[height, width - 1] + 1;
                    var deletion = matrix[height - 1, width] + cost;
                    var substitution = matrix[height - 1, width - 1] + cost;

                    var distance = Math.Min(insertion, Math.Min(deletion, substitution));
                    if (height > 1 && width > 1 && source[height - 1] == target[width - 2] && source[height - 2] == target[width - 1])
                    {
                        distance = Math.Min(distance, matrix[height - 2, width - 2] + cost);
                    }
                    matrix[height, width] = distance;
                }
            }

            return matrix[bounds.Height - 1, bounds.Width - 1];
        }

        #endregion
    }
}
