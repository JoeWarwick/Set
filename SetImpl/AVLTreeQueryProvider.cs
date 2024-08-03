using System.Collections;
using System.Linq.Expressions;
using System.Xml.Linq;


namespace SetImpl
{
    public class AVLTreeExpression<T> : Expression where T: IComparable<T>
    {
        private readonly AVLTree<T> tree;

        public AVLTreeExpression(AVLTree<T> tree)
        {
            this.tree = tree;
        }


        public override Type Type => typeof(IQueryable<T>);

        // **This is the missing part:**
        public override ExpressionType NodeType => ExpressionType.Extension;

        // Override the ToString() method for debugging
        public override string ToString()
        {
            return "AVLTreeExpression";
        }
    }

    // Query provider for the AVL tree
    public class AVLTreeQueryProvider<T> : IQueryProvider where T : IComparable<T>
    {
        private readonly AVLTree<T> tree;

        public AVLTreeQueryProvider(AVLTree<T> tree)
        {
            this.tree = tree;
        }

        // Execute a query
        public object Execute(Expression expression)
        {
            // Handle different types of queries (e.g., Where, Select, OrderBy)
            // This implementation assumes a simple "Where" clause for demonstration purposes
            if (expression is MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.Method.Name == "Where")
                {
                    // Extract the lambda expression from the Where clause
                    var source = methodCallExpression.Arguments[1];
                    LambdaExpression? lambda;
                    if (source.NodeType == ExpressionType.Quote)
                        lambda = ((UnaryExpression)source).Operand as LambdaExpression;
                    else
                        lambda = (LambdaExpression)methodCallExpression.Arguments[1];

                    // Create a list to store the results
                    List<T> results = new List<T>();

                    // Traverse the AVL tree using an in-order traversal
                    InOrderTraversal(this.tree.root, results, lambda);

                    // Return the results as an IEnumerable
                    return results.AsQueryable();
                }
            }

            // Unsupported expression type
            throw new NotImplementedException();
        }

        // Perform in-order traversal of the AVL tree
        private void InOrderTraversal(AVLTreeNode<T>? node, List<T> results, LambdaExpression? lambda)
        {
            if (node == null)
            {
                return;
            }

            // Recursively traverse the left subtree
            InOrderTraversal(node.Left, results, lambda);

            // Check if the current node satisfies the filter condition
            if (lambda != null && lambda.Compile().DynamicInvoke(node.Value) is bool condition && condition)
            {
                results.Add(node.Value);
            }

            // Recursively traverse the right subtree
            InOrderTraversal(node.Right, results, lambda);
        }

        // Not implemented for this example
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        // Not implemented for this example
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            // Create a new AVLTreeExpression for the new query
            var newExpression = new AVLTreeExpression<T>(tree);

            // Return an IQueryable<TElement> based on the new expression
            return new AVLTreeQueryable<TElement>(this, expression, newExpression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }

        
    }

    public class AVLTreeQueryable<TElement> : IQueryable<TElement>
    {
        private IQueryProvider provider;
        private Expression expression;
        private Expression treeExpression;

        public AVLTreeQueryable(IQueryProvider provider, Expression expression, Expression treeExpression)
        {
            this.provider = provider;
            this.expression = expression;
            this.treeExpression = treeExpression;
        }

        public Type ElementType => typeof(TElement);

        public Expression Expression => expression;

        public Expression TreeExpression => treeExpression;

        public IQueryProvider Provider => provider;

        public IEnumerator<TElement> GetEnumerator()
        {
            var res = (IQueryable<TElement>?)provider.Execute(expression);
            if(res != null) return res.GetEnumerator();
            return new List<TElement>().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
