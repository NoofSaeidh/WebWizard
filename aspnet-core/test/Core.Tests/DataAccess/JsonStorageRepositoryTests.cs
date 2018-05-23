using AcWebTool.Core.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AcWebTool.Core.Tests.DataAccess
{
    [Collection("JsonStorage")]
    public class JsonStorageRepositoryTests
    {
        private readonly string _location;
        public JsonStorageRepositoryTests(JsonStorageFixture fixture)
        {
            _location = fixture.Location;
        }

        [Fact]
        public void Get_ById_Works()
        {
            // Arrange
            var entity = new TestEntity
            {
                Id = NewId(),
                Message = "message"
            };
            var file = GetFullFileName(entity.Id);
            var text = JsonConvert.SerializeObject(entity);
            File.WriteAllText(file, text);
            var repository = GetRepository();
            // Act
            var result = repository.Get(entity.Id);
            // Assert
            Assert.Equal(entity, result);
        }

        [Fact]
        public void Insert_WithId_Works()
        {
            // Arrange
            var entity = new TestEntity
            {
                Id = NewId(),
                Message = "message"
            };
            var repository = GetRepository();
            // Act
            repository.Insert(entity);
            // Assert
            Assert.Equal(entity, repository.Get(entity.Id));
        }

        [Fact]
        public void Insert_WithoutId_Works_AndFillId()
        {
            // Arrange
            var entity = new TestEntity
            {
                Message = "message"
            };
            var repository = GetRepository();
            // Act
            repository.Insert(entity);
            // Assert
            Assert.NotNull(entity.Id);
            Assert.Equal(entity, repository.Get(entity.Id));
        }

        [Fact]
        public void Update_Works()
        {
            // Arrange
            var entity = new TestEntity
            {
                Message = "message"
            };
            var repository = GetRepository();
            repository.Insert(entity);
            entity.Message = "new message";
            // Act
            repository.Update(entity);
            // Assert
            Assert.Equal(entity, repository.Get(entity.Id));
        }

        [Fact]
        public void Upsert_AsInsert_WithoutId_Works()
        {
            // Arrange
            var entity = new TestEntity
            {
                Message = "message"
            };
            var repository = GetRepository();
            // Act
            repository.Upsert(entity);
            // Assert
            Assert.NotNull(entity.Id);
            Assert.Equal(entity, repository.Get(entity.Id));
        }

        [Fact]
        public void Upsert_AsInsert_WithId_Works()
        {
            // Arrange
            var entity = new TestEntity
            {
                Id = NewId(),
                Message = "message"
            };
            var repository = GetRepository();
            // Act
            repository.Upsert(entity);
            // Assert
            Assert.Equal(entity, repository.Get(entity.Id));
        }

        [Fact]
        public void Upsert_AsUpdate_Works()
        {
            // Arrange
            var entity = new TestEntity
            {
                Message = "message"
            };
            var repository = GetRepository();
            repository.Insert(entity);
            entity.Message = "new message";
            // Act
            repository.Upsert(entity);
            // Assert
            Assert.Equal(entity, repository.Get(entity.Id));
        }

        [Fact]
        public void Delete_ById_Works()
        {
            // Arrange
            var entity = new TestEntity
            {
                Message = "message"
            };
            var repository = GetRepository();
            repository.Insert(entity);
            // Act
            repository.Delete(entity.Id);
            // Assert
            Assert.False(File.Exists(GetFullFileName(entity.Id)));
        }

        [Fact]
        public void Clear_Works()
        {
            // Arrange
            var repository = GetRepository();
            //just for sure that it exists
            repository.Insert(new TestEntity());
            // Act
            repository.Clear();
            // Assert
            Assert.Empty(Directory.EnumerateFiles(_location));
        }

        [Fact]
        public void GetAll_Works()
        {
            // Arrange
            var repository = GetRepository();
            // just for sure nothing in folder:
            repository.Clear();
            
            repository.Insert(new TestEntity());
            repository.Insert(new TestEntity());
            repository.Insert(new TestEntity());
            // Act
            var result = repository.GetAll();
            // Assert
            Assert.Equal(3, result.Count());
            Assert.All(result, e => Assert.NotNull(e.Id));
        }


        private string NewId() => Guid.NewGuid().ToString("N");
        private string GetFullFileName(string id) => Path.Combine(_location, id + ".json");
        private JsonStorageRepository<TestEntity> GetRepository() => new JsonStorageRepository<TestEntity>(_location);
    }
}
