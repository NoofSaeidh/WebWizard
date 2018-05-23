using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.DataAccess
{
    public class JsonStorageRepository<T> : FileStorageRepository<T> where T : IEntity
    {
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = new JsonConverter[]
            {
                new StringEnumConverter()
            }
        };

        public JsonStorageRepository(string fileLocation) : base(fileLocation)
        {
        }

        protected override string GetFileName(string id)
        {
            //todo: some characters cannot be used
            return id + ".json";
        }

        protected override T Parse(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, jsonSerializerSettings);
        }

        protected override string Serialize(T value)
        {
            return JsonConvert.SerializeObject(value, jsonSerializerSettings);
        }
    }
}
