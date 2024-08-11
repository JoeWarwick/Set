using System.Collections.Concurrent;

namespace SetImpl
{
    public static class AVLTreeExtensions
    {

        public static IEnumerable<T> Where<T>(this AVLTree<T> tree, Func<T, bool> predicate) where T : IComparable<T>
        {
            foreach (var item in tree)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<TResult> Select<T, TResult>(this AVLTree<T> tree, Func<T, TResult> selector)
            where T : IComparable<T>
        {
            foreach (var item in tree)
            {
                yield return selector(item);
            }
        }

        public static async Task<int> DeleteWhere<T>(this AVLTree<T> tree, Func<T, bool> predicate) where T : IComparable<T>
        {
            var toDelete = new ConcurrentBag<T>();

            await Parallel.ForEachAsync(tree, async (item, _) =>
            {
                if (predicate(item))
                    toDelete.Add(item);
            });

            int count = 0;
            foreach (var item in toDelete)
            {
                if (tree.Delete(item))
                    Interlocked.Increment(ref count);
            }

            return count;
        }

        // You can add more custom methods here if needed

        public static AVLTree<T> ToAVLTree<T>(this IEnumerable<T> source) where T : IComparable<T>
        {
            var newTree = new AVLTree<T>();
            foreach (var item in source)
            {
                newTree.Insert(item);
            }
            return newTree;
        }
    }

}
