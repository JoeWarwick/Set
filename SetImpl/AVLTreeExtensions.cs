using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static int DeleteWhere<T>(this AVLTree<T> tree, Func<T, bool> predicate) where T : IComparable<T>
        {
            var toDelete = new List<T>();
            foreach (var item in tree)
            {
                if (predicate(item))
                    toDelete.Add(item);
            }
            int count = 0;
            foreach (var item in toDelete)
            {
                if (tree.Delete(item))
                    count++;
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
