using System.Collections.Generic;
using BkTreeSpellChecker.StringMetrics;

namespace BkTreeSpellChecker.BkTree
{
    // node used in the Bk tree
    public class BkTreeNode<T>
    {
        #region properties & variables

        private readonly T _element;
        private Dictionary<int, BkTreeNode<T>> Children { get; set; }

        #endregion

        #region ctors

        // default ctor 
        public BkTreeNode()
        {
        }

        // with params 
        public BkTreeNode(T element)
        {
            _element = element;
        }

        #endregion

        #region public methods 

        // returns the element found in the node 
        public T GetElement()
        {
            return _element;
        }

        public Dictionary<int, BkTreeNode<T>> GetChildren()
        {
            return Children;
        }

        // adds a new child node 
        public void AddNode(int distance, BkTreeNode<T> node, IBkMetricSpace<T> bkMetricSpace)
        {
            if (Children == null)
            {
                Children = new Dictionary<int, BkTreeNode<T>>(); // init dictionary 
            }

            // a child with the same distance is already in place
            if (Children.ContainsKey(distance))
            {
                var tmpNode = Children[distance];
                distance = bkMetricSpace.GetDistance(tmpNode.GetElement(), node.GetElement());
                tmpNode.AddNode(distance, node, bkMetricSpace);
                return;
            }

            Children.Add(distance, node); // add the new node as a child
        }

        #endregion
    }
}
