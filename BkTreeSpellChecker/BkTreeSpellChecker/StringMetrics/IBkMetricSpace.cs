using BkTreeSpellChecker.BkTree;

namespace BkTreeSpellChecker.StringMetrics
{
    // an interface specifying the method signature
    // used to get the distance between two bk tree nodes 
    public interface IBkMetricSpace<T>
    {
        double GetDistance(BkTreeNode<T> source, BkTreeNode<T> target);
        double GetDistance(T source, T target);
    }
}
