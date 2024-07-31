
namespace SetImpl
{
    public interface IAVLTree<T> where T : IComparable
    {
        void Delete(T value);
        AVLTreeNode<T>? Find(T value);
        AVLTreeNode<T>? Insert(T value);
        string ToString();
    }
}