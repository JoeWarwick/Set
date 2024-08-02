
namespace SetImpl
{
    public interface IAVLTree<T> where T : IComparable<T>
    {
        void Delete(T value);
        T? Find(T value);
        void Insert(T value);
        string ToString();
    }
}