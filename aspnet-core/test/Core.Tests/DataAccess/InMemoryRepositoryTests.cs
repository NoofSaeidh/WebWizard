using AcWebTool.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AcWebTool.Core.Tests.DataAccess
{
    public class InMemoryRepositoryTests
    {
        [Fact]
        public void Insert_Works()
        {
            // Arrange
            var repo = new InMemoryRepository<TestEntity>();
            repo.Clear();
            // Act & Assert
            repo.Insert(new TestEntity { Message = "test" });
        }

        [Fact]
        public void GetById_Works()
        {
            // Arrange
            var repo = new InMemoryRepository<TestEntity>();
            repo.Clear();
            repo.Insert(new TestEntity { Id = "id", Message = "test" });
            // Act
            var entity = repo.Get("id");
            // Assert
            Assert.Equal("id", entity.Id);
            Assert.Equal("test", entity.Message);
        }

        [Fact]
        public void Get_WithNull_ReturnsAll()
        {
            // Arrange
            var repo = new InMemoryRepository<TestEntity>();
            repo.Clear();
            repo.Insert(new TestEntity { Message = "1" });
            repo.Insert(new TestEntity { Message = "2" });
            repo.Insert(new TestEntity { Message = "3" });

            // Act
            var all = repo.Get((System.Linq.Expressions.Expression<Func<TestEntity, bool>>)null);
            // Assert
            Assert.Equal(3, all.Count());
        }

        [Fact]
        public void Get_WithPredicate_Works()
        {
            // Arrange
            var repo = new InMemoryRepository<TestEntity>();
            repo.Clear();
            repo.Insert(new TestEntity { Message = "1" });
            repo.Insert(new TestEntity { Message = "2" });
            repo.Insert(new TestEntity { Message = "3" });

            // Act
            var all = repo.Get(e=>e.Message == "3");
            // Assert
            Assert.Single(all);
            Assert.Equal("3", all.First().Message);
        }

        [Fact]
        public void Update_ById_Works()
        {
            // Arrange
            var repo = new InMemoryRepository<TestEntity>();
            repo.Clear();
            repo.Insert(new TestEntity { Id = "1", Message = "1" });

            // Act
            repo.Update(new TestEntity { Id = "1", Message = "test" });

            // Assert
            Assert.Equal("test", repo.Get("1").Message);
        }

        [Fact]
        public void Upsert_WorksAsInsert()
        {
            // Arrange
            var repo = new InMemoryRepository<TestEntity>();
            repo.Clear();

            // Act
            repo.Upsert(new TestEntity { Id = "1", Message = "test" });

            // Assert
            Assert.Equal("test", repo.Get("1").Message);
        }

        [Fact]
        public void Upsert_WorksAsUpdate()
        {
            // Arrange
            var repo = new InMemoryRepository<TestEntity>();
            repo.Clear();
            repo.Insert(new TestEntity { Id = "1", Message = "1" });

            // Act
            repo.Upsert(new TestEntity { Id = "1", Message = "test" });

            // Assert
            Assert.Equal("test", repo.Get("1").Message);
        }

        [Fact]
        public void Delete_Works()
        {
            // Arrange
            var repo = new InMemoryRepository<TestEntity>();
            repo.Clear();
            repo.Insert(new TestEntity { Id = "1", Message = "1" });

            // Act
            repo.Delete("1");

            // Assert
            Assert.Empty(repo.Get((System.Linq.Expressions.Expression<Func<TestEntity, bool>>)null));
        }

        [Fact]
        public void DeleteById_Works()
        {
            // Arrange
            var repo = new InMemoryRepository<TestEntity>();
            repo.Clear();
            repo.Insert(new TestEntity { Id = "1", Message = "1" });

            // Act
            repo.Delete("1");

            // Assert
            Assert.Empty(repo.Get((System.Linq.Expressions.Expression<Func<TestEntity, bool>>)null));
        }

        [Fact]
        public void MultipleRepos_WorksWithSingleDataSource()
        {
            // Arrange
            var repo1 = new InMemoryRepository<TestEntity>();
            var repo2 = new InMemoryRepository<TestEntity>();
            repo1.Clear();
            repo2.Clear();

            repo1.Insert(new TestEntity { Message = "1" });
            repo2.Insert(new TestEntity { Message = "2" });

            // Act
            var all1 = repo1.Get((System.Linq.Expressions.Expression<Func<TestEntity, bool>>)null).ToArray();
            var all2 = repo2.Get((System.Linq.Expressions.Expression<Func<TestEntity, bool>>)null).ToArray();
            // Assert
            Assert.Equal(2, all1.Length);
            Assert.Equal(2, all2.Length);
            Assert.Contains(all1, p => p.Message == "1");
            Assert.Contains(all1, p => p.Message == "2");
            Assert.Contains(all2, p => p.Message == "1");
            Assert.Contains(all2, p => p.Message == "2");
        }
    }
}
