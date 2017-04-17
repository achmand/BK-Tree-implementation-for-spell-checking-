using System;
using System.Collections.Generic;
using BkTreeSpellChecker.StringMetrics;

namespace BkTreeSpellChecker.BkTree
{
    [Serializable]
    public class BkTreeNode<T> // node used in the Bk tree
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
        // returns 0 if node is not added, 1 if added 
        public int AddNode(int distance, BkTreeNode<T> node, IBkMetricSpace<T> bkMetricSpace)
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

                if (distance == 0) // same word
                {
                    return 0; // not added
                }

                tmpNode.AddNode(distance, node, bkMetricSpace);
                return 1;
            }

            Children.Add(distance, node); // add the new node as a child
            return 1;
        }

        #endregion
    }
}
