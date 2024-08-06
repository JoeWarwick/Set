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
            return new AVLTreeQueryable<T>(this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new AVLTreeQueryable<T>(this, expression) as IQueryable<TElement>;

        public object Execute(Expression expression)
        {
            // Ensure the expression does not contain any non-evaluable components
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return Execute<IEnumerable<T>>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var visitor = new AVLTreeExpressionVisitor<T>(_tree);
            if(expression is ConstantExpression constantExpression)
            {
                return constantExpression.Value is TResult result ? result : default;
            }
            var modifiedExpression = visitor.Visit(expression);

            // Check if the modified expression is a ConstantExpression
            if (modifiedExpression is ConstantExpression constantExpression2)
            {
                // If it's a constant expression, try to get the value out of it
                return constantExpression2.Value is TResult result ? result : default;
            }

            if (typeof(TResult).IsAssignableFrom(modifiedExpression.Type))
            {
                // For direct matches of TResult type
                return (TResult)ExecuteInner(modifiedExpression);
            }

            // Evaluate the enumerable, if it's an IEnumerable<T> type
            return (TResult)ExecuteInner(modifiedExpression);
        }

        private object ExecuteInner(Expression expression)
        {
            // Here we execute the inner evaluator
            var lambda = Expression.Lambda(expression);
            var compiledLambda = lambda.Compile();
            return compiledLambda.DynamicInvoke();
        }
    }

    public class AVLTreeExpressionVisitor<T> : ExpressionVisitor where T : IComparable<T>
    {
        private readonly AVLTree<T>? _tree;

        public AVLTreeExpressionVisitor(AVLTree<T>? tree)
        {
            _tree = tree;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable))
            {
                switch (node.Method.Name)
                {
                    case "Where": return VisitWhere(node);
                    case "Select": return VisitSelect(node);
                    case "Count": return VisitCount(node);
                }
            }
            return base.VisitMethodCall(node);
        }

        private Expression VisitWhere(MethodCallExpression node)
        {
            var source = (IEnumerable<T>?)CompileAndInvoke(node.Arguments[0]);
            var lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
            var predicate = (Func<T, bool>)lambda.Compile();
            var filteredItems = source?.Where(predicate);
            return Expression.Constant(filteredItems);
        }

        private Expression VisitSelect(MethodCallExpression node)
        {
            var source = (IEnumerable<T>?)CompileAndInvoke(node.Arguments[0]);
            var lambda = (LambdaExpression)StripQuotes(node.Arguments[1]);
            var selector = (Func<T, T>)lambda.Compile();
            var selectedItems = source?.Select(selector);
            return Expression.Constant(selectedItems);
        }

        private Expression VisitCount(MethodCallExpression node)
        {
            var source = CompileAndInvoke(node.Arguments[0]) as IEnumerable<T>;
            var count = source?.Count();
            return Expression.Constant(count);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            // Check if the node holds an IQueryable
            if (typeof(IQueryable).IsAssignableFrom(node.Type))
            {
                // Safely cast to IQueryable<T>
                var queryable = node.Value as IQueryable<T>;
                if (queryable != null)
                {
                    // Execute the query and return it as a constant expression
                    var result = queryable.Provider.Execute(queryable.Expression);
                    return Expression.Constant(result);
                }
            }
            return base.VisitConstant(node);
        }

        private AVLTreeQueryable<T>? CompileAndInvoke(Expression expression)
        {
            var lambda = Expression.Lambda(expression);
            var compiledLambda = lambda.Compile();
            var res = (AVLTreeQueryable<T>?)compiledLambda.DynamicInvoke();
            return res;
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

    public class AVLTreeQueryable<T> : IQueryable<T> where T : IComparable<T>
    {
        internal IQueryProvider _provider;
        internal Expression _expression;

        public AVLTreeQueryable(AVLTree<T> tree)
        {
            if (tree == null) throw new ArgumentNullException(nameof(tree));
            _provider = new AVLTreeQueryProvider<T>(tree);
            _expression = Expression.Constant(this);
        }

        public AVLTreeQueryable(IQueryProvider provider, Expression expression)
        {
            this._provider = provider;
            this._expression = expression;
        }

        public Type ElementType => typeof(T);

        public Expression Expression => _expression;

        public IQueryProvider Provider => _provider;

        public IEnumerator<T> GetEnumerator() {
            try
            {
                // Execute the expression to get an enumerable result.
                var result = _provider.Execute<IEnumerable<T>>(_expression);

                // Ensure result is not null and return its enumerator. Otherwise return a new enumerator for an empty collection.
                return result?.GetEnumerator() ?? Enumerable.Empty<T>().GetEnumerator();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Enumerable.Empty<T>().GetEnumerator(); // Return an empty enumerator on exception.
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
