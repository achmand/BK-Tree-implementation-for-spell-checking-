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

        private readonly char[][] _qwertyLayout = {
            new[] {'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p'},
            new[] {'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l'},
            new[] { 'z', 'x', 'c', 'v', 'b', 'n', 'm' , ',', '.'}
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
                    for (var y = 0; y < layer.Length - 1; y++)
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

            var sourceAdj = _adjacentKeys[source];
            if (sourceAdj.ContainsKey(target))
            {
                return sourceAdj[target];
            }

            var targetAdj = _adjacentKeys[target];
            return targetAdj.ContainsKey(source) ? targetAdj[source] : 1;
        }

        #endregion
    }

    public sealed class TypoDistance : BaseBkMetricSpace
    {

        public override double ComputeDistance(string target, string source)
        {
            throw new System.NotImplementedException();
        }
    }
}
