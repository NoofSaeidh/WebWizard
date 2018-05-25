using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.Keys
{
    public class KeyGenerator : IKeyGenerator, ISingletonDependency
    {
        private static Lazy<Dictionary<Type, MethodInfo>> _methods = new Lazy<Dictionary<Type, MethodInfo>>(() =>
        {
            return typeof(KeyGenerator)
                .GetMethods()
                .Where(m => !m.ContainsGenericParameters
                         && m.Name.StartsWith(nameof(Generate)))
                .ToDictionary(m => m.ReturnType, m => m);
        });

        public T Generate<T>()
        {
            if (!_methods.Value.TryGetValue(typeof(T), out var method))
                throw new Exception($"Type {typeof(T)} is not supported.");
            try
            {
                return (T)method.Invoke(this, null);
            }
            catch(Exception e)
            {
                throw new Exception($"Cannot create insance of type {typeof(T)}.", e);
            }
        }
        public Guid GenerateGuid() => Guid.NewGuid();
        // todo: generate like online services do
        public string GenerateString() => GenerateGuid().ToString("N");
    }
}
