
namespace SetImpl
{
    public interface IAVLTree<T> where T : IComparable<T>
    {
        void Delete(T value);
        T? Find(T value);
        T? Get(int index);
        int IndexOf(T value);
        void Insert(T value);
        string ToString();
    }
}