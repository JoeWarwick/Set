using System.Collections;
using System.Linq.Expressions;

namespace SetImpl
{
    public class AVLTreeQueryProvider<T>: IQueryProvider where T : IComparable<T>
    {
        private readonly AVLTree<T> _tree;

        public AVLTreeQueryProvider(AVLTree<T> tree)
        {
            _tree = tree;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = expression.Type.GetGenericArguments()[0];
            return (IQueryable)Activator.CreateInstance(typeof(AVLTreeQueryable<>).MakeGenericType(elementType), new object[] { this, expression });
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AVLTreeQueryable<TElement>((IQueryProvider)this, expression);
        }

        public object Execute(Expression expression)
        {
            return Execute<IEnumerable<T>>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var isEnumerable = typeof(TResult).Name == "IEnumerable`1";
            return (TResult)Execute(expression, isEnumerable);
        }

        private object? Execute(Expression expression, bool isEnumerable)
        {
            var visitor = new ExpressionTreeVisitor();
            var lambda = Expression.Lambda<Func<T, bool>>(visitor.Visit(expression), visitor.Parameter);
            var predicate = lambda.Compile();

            var result = _tree.Where(predicate);

            if (!isEnumerable)
            {
                return result.FirstOrDefault();
            }
            return result;
        }

        private class ExpressionTreeVisitor : ExpressionVisitor
        {
            public ParameterExpression Parameter { get; } = Expression.Parameter(typeof(T), "x");

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Where")
                {
                    var lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
                    return Expression.Invoke(lambda, Parameter);
                }
                return base.VisitMethodCall(node);
            }

            private static Expression StripQuotes(Expression e)
            {
                while (e.NodeType == ExpressionType.Quote)
                {
                    e = ((UnaryExpression)e).Operand;
                }
                return e;
            }
        }
    }

    public class AVLTreeQueryable<T> : IQueryable<T>
    {
        private IQueryProvider _provider;
        private Expression _expression;

        public AVLTreeQueryable(IQueryProvider provider, Expression expression)
        {
            this._provider = provider;
            this._expression = expression;
        }

        public Type ElementType => typeof(T);

        public Expression Expression => _expression;

        public IQueryProvider Provider => _provider;

        public IEnumerator<T> GetEnumerator()
        {
            return ((_provider.Execute<IEnumerable<T>>(_expression)) ?? Enumerable.Empty<T>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
