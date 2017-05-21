using System;
using BkTreeSpellChecker.BkTree;

namespace BkTreeSpellChecker.StringMetrics
{
    // base class for bk metric spaces
    public abstract class BaseBkMetricSpace : IBkMetricSpace<string>
    {
        #region properties and variables 

        public double RateOfChange { get; protected set; }

        #endregion

        #region public methods 

        // gets the distance between the two bk tree nodes
        public double GetDistance(BkTreeNode<string> sourceNode, BkTreeNode<string> targetNode)
        {
            if (sourceNode == null || targetNode == null)
            {
                throw new Exception("Nodes cannot be null.");
            }

            var source = sourceNode.GetElement();
            var target = targetNode.GetElement();

            var distance = ComputeDistance(source, target);
            return distance;
        }

        // gets the distance between the two bk tree nodes
        public double GetDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(target))
            {
                throw new Exception("Strings cannot be null.");
            }

            var distance = ComputeDistance(source, target);
            return distance;
        }

        // implementation
        public abstract double ComputeDistance(string target, string source);

        #endregion
    }
}
