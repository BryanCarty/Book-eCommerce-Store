using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

/**
 * 
 *  Taken from Zefirrat on GitHub to resolve issues with mocking DbContexts
 *  
 *  Source: https://gist.github.com/Zefirrat/a04658c827ba3ebffe03fda48d53ea11
 * 
 */
namespace Book_eCommerce_Store.Test
{
    internal class MockDbContextAsynced<TDbContext>
    {
        private readonly TDbContext _mock;
        public TDbContext Object => _mock;

        public MockDbContextAsynced()
        {
            _mock = Activator.CreateInstance<TDbContext>();
        }

        public void AddDbSetData<TEntity>(List<TEntity> data) where TEntity : class
        {
            var mockSet = new Mock<DbSet<TEntity>>();
            mockSet.As<IAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator(CancellationToken.None))
                .Returns(new TestAsyncEnumerator<TEntity>(data.AsQueryable().GetEnumerator()));
            mockSet.As<IQueryable<TEntity>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<TEntity>(data.AsQueryable().Provider));
            mockSet.As<IQueryable<TEntity>>()
                .Setup(m => m.Expression)
                .Returns(data.AsQueryable().Expression);
            mockSet.As<IQueryable<TEntity>>()
                .Setup(m => m.ElementType)
                .Returns(data.AsQueryable().ElementType);
            mockSet.As<IQueryable<TEntity>>()
                .Setup(m => m.GetEnumerator())
                .Returns(data.AsQueryable().GetEnumerator());
            mockSet
                .Setup(m => m.AddRange(It.IsAny<IEnumerable<TEntity>>()))
                .Callback((IEnumerable<TEntity> entities) =>
                {
                    var collection = entities as List<TEntity> ?? entities.ToList();
                    data.AddRange(collection);
                });
            mockSet
                .Setup(m => m.Add(It.IsAny<TEntity>()))
                .Callback((TEntity entity) =>
                {
                    data.Add(entity);
                });

            var entityType = data.GetType().GetGenericArguments()[0];
            var propertySet = _mock.GetType()
                .GetProperties()
                .First(p => p.PropertyType.IsGenericType
                            && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)
                            && p.PropertyType.GetGenericArguments()[0] == entityType)
                .SetMethod;

            propertySet.Invoke(_mock, new[] { mockSet.Object });
        }
    }

    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
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
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }


        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(
                    name: nameof(IQueryProvider.Execute),
                    genericParameterCount: 1,
                    types: new[] { typeof(Expression) })
                .MakeGenericMethod(expectedResultType)
                .Invoke(this, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                ?.MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { executionResult });
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable()
                .GetEnumerator());
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestAsyncQueryProvider<T>(this); }
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken()) =>
            GetEnumerator();
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(Task.FromResult(_inner.MoveNext()));

        public T Current
        {
            get
            {
                return _inner.Current;
            }
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }

        public ValueTask DisposeAsync() => new ValueTask(Task.Run(Dispose));
    }
}
