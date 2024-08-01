
namespace SetImpl
{
    public interface IAVLTree<T> where T : IComparable<T>
    {
        void Delete(T value);
        AVLTreeNode<T>? Find(T value);
        void Insert(T value);
        string ToString();
    }
}