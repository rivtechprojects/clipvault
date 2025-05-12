using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace ClipVault.Tests
{
    public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner?.Execute(expression) 
                ?? throw new InvalidOperationException("Inner query provider is null.");
        }

        public TResult Execute<TResult>(Expression expression)
        {

            // Handle MethodCallExpression
            if (expression is MethodCallExpression methodCallExpression)
            {
                var result = Expression.Lambda(methodCallExpression).Compile().DynamicInvoke();

                if (result is null)
                {
                    throw new InvalidOperationException("The method call expression evaluated to null, which cannot be assigned to a non-nullable type.");
                }

                // Wrap the result in a Task if TResult is a Task type
                if (typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var taskResultType = typeof(TResult).GetGenericArguments()[0];
                    var taskFromResultMethod = typeof(Task).GetMethod("FromResult")?.MakeGenericMethod(taskResultType);

                    if (taskFromResultMethod == null)
                    {
                        throw new InvalidOperationException("Unable to find Task.FromResult method via reflection.");
                    }

                    var taskResult = taskFromResultMethod.Invoke(null, [result]);

                    if (taskResult is null)
                    {
                        throw new InvalidOperationException("Task.FromResult returned null, which cannot be assigned to a non-nullable type.");
                    }

                    return (TResult)taskResult;
                }

                return (TResult)result;
            }

            // Compile and execute the expression if it's a LambdaExpression
            if (expression is LambdaExpression lambdaExpression)
            {
                var compiledLambda = lambdaExpression.Compile();
                var result = compiledLambda.DynamicInvoke() ?? throw new InvalidOperationException("Null cannot be assigned to a non-nullable type.");
                return (TResult)result;
            }

            // Otherwise, delegate to the inner query provider
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            // Use the synchronous Execute method to simulate asynchronous behavior
            return Task.FromResult(Execute<TResult>(expression)).Result;
        }
    }

    public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
        {
        }

        public TestAsyncEnumerable(Expression expression) : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public T Current => _inner.Current;
    }
}
