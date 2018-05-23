using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AcWebTool.Core.Tests.DataAccess
{
    public class JsonStorageFixture : IDisposable
    {
        public JsonStorageFixture()
        {
            Location = Path.Combine(Directory.GetCurrentDirectory(), nameof(JsonStorageFixture));
            if(Directory.Exists(Location))
            {
                Directory.Delete(Location, true);
            }
            Directory.CreateDirectory(Location);
        }

        public string Location { get; }

        public void Dispose()
        {
            Directory.Delete(Location, true);
        }
    }

    [CollectionDefinition("JsonStorage", DisableParallelization = true)]
    public class JsonStorageCollection : ICollectionFixture<JsonStorageFixture> { }
}
