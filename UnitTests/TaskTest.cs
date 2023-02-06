using ArqInf.Controllers;
using ArqInf.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ArchInfTest
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> input)
        {
            return new NotInDbSet<T>(input);
        }
    }

    public class NotInDbSet<T> : IQueryable<T>, IAsyncEnumerable<T>, IEnumerable<T>, IEnumerable
    {
        private readonly List<T> _innerCollection;

        public NotInDbSet(IEnumerable<T> innerCollection)
        {
            _innerCollection = innerCollection.ToList();
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return new AsyncEnumerator(GetEnumerator());
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public class AsyncEnumerator : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;
            public AsyncEnumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public ValueTask DisposeAsync()
            {
                return new ValueTask();
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_enumerator.MoveNext());
            }

            public T Current => _enumerator.Current;
        }

        public Type ElementType => typeof(T);
        public Expression Expression => Expression.Empty();
        public IQueryProvider Provider => new EnumerableQuery<T>(Expression);
    }
    /*public class TaskControllerTest
    {
        [Fact]
        public async Task Index_CanLoadFromContext()
        {
            var options = new DbContextOptionsBuilder<ArchInfComIdentityContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;
            var context = new ArchInfComIdentityContext(options);

            var mockTaskManager = new Mock<TaskManager<Task>>(new Mock<IUserStore<Task>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<Task>>().Object,
                new IUserValidator<Task>[0],
                new IPasswordValidator<Task>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<TaskManager<Task>>>().Object);

            IList<Task> listaTasks = new List<Task>
            {
                new Task()
                {
                    Name = "Make Form Detail"
                },
                new Task()
                {
                    Name = "Make Login"
                },
                new Task()
                {
                    Name = "Test Login"
                }
            };

            var Tasks = listaTasks.AsAsyncQueryable();
            mockTaskManager.Setup(r => r.Tasks)
                .Returns(Tasks);


            var controller = new TasksController(context, mockTaskManager.Object);
            mockTaskManager.Setup(r => r.GetUserAsync(controller.User)).ReturnsAsync(listaTasks[0]);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Task>>(
                viewResult.ViewData.Model);
            Assert.NotNull(model);
            Assert.Equal(3, model.Count());
        }
		
		[Fact]
        public async Task Edit_Task_Name()
        {
            var mockTaskManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);


            mockTaskManager.Setup(r => r.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string UserId, string oldTask, string newTask) => null);

            var controller = new GestaoTasksController(_context, mockTaskManager.Object);

            var result = await controller.Edit(newTask);

            Assert.IsType<NotFoundResult>(result);
        }
    }*/
}
