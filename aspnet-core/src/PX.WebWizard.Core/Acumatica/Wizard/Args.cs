using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PX.WebWizard.Acumatica.Wizard
{
    [Serializable]
    public class Args : IDictionary<string, object>
    {
        private Type _cachedType;
        private readonly Dictionary<string, object> _items;

        private readonly PropertyInfo[] _cachedProperties;
        // may contains duplicated values (from Property name and from Argument attribute)
        private readonly Dictionary<string, PropertyInfo> _cachedPropertiesByKeys;

        private static readonly Dictionary<Type, (PropertyInfo[], Dictionary<string, PropertyInfo>)>
            _cachedPropertiesByType = new Dictionary<Type, (PropertyInfo[], Dictionary<string, PropertyInfo>)>();

        public Args()
        {
            _cachedType = GetType();
            _items = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            lock (((ICollection)_cachedPropertiesByType).SyncRoot)
            {
                if (!_cachedPropertiesByType.ContainsKey(_cachedType))
                {
                    _cachedProperties = _cachedType
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(prop => prop.CanRead && prop.CanWrite
                                    && prop.GetCustomAttribute<NotArgumentAttribute>() == null
                                    && prop.GetIndexParameters().Length == 0)
                        .ToArray();

                    PropertiesCount = _cachedProperties.Length;

                    var tmpCollection = _cachedProperties
                        .Where(prop => prop.GetCustomAttribute<ArgumentAttribute>() != null)
                        .Select(prop =>
                        {
                            var arg = prop.GetCustomAttribute<ArgumentAttribute>();
                            return (prop, arg);
                        });

                    _cachedPropertiesByKeys = tmpCollection
                        .Where(p => p.arg != null && p.arg.Name != null)
                        .Select(p => (p.arg.Name, p.prop))
                        .Union(tmpCollection
                            .Select(p => (p.prop.Name, p.prop)))
                        .ToDictionary(p => p.Name, p => p.prop);

                    _cachedPropertiesByType.Add(_cachedType,
                        (_cachedProperties, _cachedPropertiesByKeys));
                }
                else
                {
                    (_cachedProperties, _cachedPropertiesByKeys) = _cachedPropertiesByType[_cachedType];
                    PropertiesCount = _cachedProperties.Length;
                }
            }
        }

        public object this[string key]
        {
            get
            {
                if (this.TryGetValue(key, out var propRes))
                    return propRes;

                throw new KeyNotFoundException();
            }
            set
            {
                if (!TrySetPropertyValue(key, value))
                {
                    _items[key] = value;
                }
            }
        }

        [NotArgument]
        public int Count => PropertiesCount + _items.Count;


        public bool TryGetValue(string key, out object value)
        {
            if (TryGetPropertyValue(key, out value, out _, out _))
            {
                return true;
            }

            if (_items.TryGetValue(key, out value))
            {
                return true;
            }

            value = null;
            return false;
        }

        public void Add(string key, object value)
        {
            if (GetProperty(key) != null)
                throw new InvalidOperationException($"Cannot add element with key {key}, because property with such key already exists.");
            _items.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return ContainsPropertyKey(key) || _items.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            if (GetProperty(key) != null)
                return false;
            return _items.Remove(key);
        }

        public void Clear()
        {
            ClearProperties();
            _items.Clear();
        }

        public string Serialize()
        {
            var sb = new StringBuilder();
            foreach (var prop in _cachedProperties)
            {
                if (TryGetPropertyValue(prop, out var value, out var outkey, out var argType))
                    sb.Append(SerializeArgument(outkey, value, argType));
                sb.Append(" ");
            }
            foreach (var item in _items)
            {
                sb.Append(SerializeArgument(item.Key, item.Value));
                sb.Append(" ");
            }

            return sb.ToString().Trim();
        }

        public string SerializeArgument(string argumentName)
        {
            if (TryGetPropertyValue(argumentName, out var value, out var outkey, out var argType))
            {
                argumentName = outkey;
            }
            return SerializeArgument(argumentName, this[argumentName], argType);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        protected int PropertiesCount { get; }

        protected bool ContainsPropertyKey(string key)
        {
            return GetProperty(key) != null;
        }

        protected void ClearProperties()
        {
            foreach (var prop in _cachedProperties)
            {
                try
                {
                    prop.SetValue(this, null);
                }
                catch { continue; }
            }
        }

        protected bool TryGetPropertyValue(string key, out object value, out string outkey, out ArgumentType argumentType)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var prop = GetProperty(key);
            outkey = null;
            value = null;
            argumentType = default;

            if (prop != null)
            {
                return TryGetPropertyValue(prop, out value, out outkey, out argumentType);
            }
            return false;
        }

        protected bool TryGetPropertyValue(PropertyInfo property, out object value, out string outkey, out ArgumentType argumentType)
        {
            try
            {
                value = property.GetValue(this);
                outkey = GetPropertyKey(property, out argumentType);
                return true;
            }
            catch
            {
                argumentType = default;
                outkey = null;
                value = null;
                return false;
            }
        }

        protected bool TrySetPropertyValue(string key, object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var prop = GetProperty(key);

            if (prop != null)
            {
                try
                {
                    prop.SetValue(this, value);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        protected string SerializeArgument(string key, object value, ArgumentType type = ArgumentType.Value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null) return null;
            switch (type)
            {
                case ArgumentType.Value:
                    return $"-{key}:\"{value}\"";
                case ArgumentType.Object:
                    return $"-{key}:\"{SerializeObject(value)}\"";
                case ArgumentType.Enumerable:
                {
                    if (!(value is IEnumerable e))
                        throw new InvalidOperationException($"Value {value} is not IEnumerable.");
                    var res = new StringBuilder();
                    foreach (var item in e)
                    {
                        res.Append($"-{key}:\"{item}\" ");
                    }
                    return res.ToString().Trim();
                }
                case ArgumentType.Object | ArgumentType.Enumerable:
                {
                    if (!(value is IEnumerable e))
                        throw new InvalidOperationException($"Value {value} is not IEnumerable.");
                    var res = new StringBuilder();
                    foreach (var item in e)
                    {
                        res.Append($"-{key}:\"{SerializeObject(item)}\" ");
                    }
                    return res.ToString().Trim();
                }
                default:
                    throw new NotSupportedException($"{nameof(ArgumentType)} {type} is not supported.");
            }

        }

        private string SerializeObject(object value)
        {
            var props = value.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
            var res = new StringBuilder();
            foreach (var prop in props)
            {
                object propValue = null;
                try
                {
                    propValue = prop.GetValue(value);
                }
                catch
                {
                    continue;
                }
                //todo: attributes to properties of object to have different names
                if (propValue != null)
                    res.Append($"{prop.Name}={propValue};");
            }
            return res.ToString().Trim();
        }

        private PropertyInfo GetProperty(string key)
        {
            if (_cachedPropertiesByKeys.TryGetValue(key, out var result))
                return result;
            return null;
        }

        private string GetPropertyKey(PropertyInfo property, out ArgumentType argumentType)
        {
            var arg = property.GetCustomAttribute<ArgumentAttribute>();
            if (arg == null)
            {
                argumentType = default;
                return property.Name;
            }
            argumentType = arg.ArgumentType;
            return arg.Name ?? property.Name;
        }

        #region Explicit Dictionary Implementation

        ICollection<string> IDictionary<string, object>.Keys => _cachedPropertiesByKeys.Keys.Union(_items.Keys).ToArray();

        ICollection<object> IDictionary<string, object>.Values => _cachedProperties.Union(_items.Values).ToArray();

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            if (!TrySetPropertyValue(item.Key, item.Value))
                ((ICollection<KeyValuePair<string, object>>)_items).Add(item);
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            if (TryGetPropertyValue(item.Key, out var value, out _, out _))
            {
                return item.Value == value;
            }
            return ((ICollection<KeyValuePair<string, object>>)_items).Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, $"{arrayIndex} is less than 0.");

            if (Count > array.Length - arrayIndex)
                throw new ArgumentException($"The number of elements in the source {nameof(ICollection<KeyValuePair<string, object>>)}" +
                    $"is greater than the available space from {nameof(arrayIndex)} to the end of the destination array.");

            var i = arrayIndex;
            foreach (var item in this)
            {
                array[i] = item;
                i++;
            }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            if (GetProperty(item.Key) != null)
                return false;
            return ((ICollection<KeyValuePair<string, object>>)_items).Remove(item);
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        public override string ToString()
        {
            return Serialize();
        }

        public struct Enumerator : IEnumerator<KeyValuePair<string, object>>
        {
            private Args _args;
            private IEnumerator<KeyValuePair<string, object>> _propEnumerator;
            private IEnumerator<KeyValuePair<string, object>> _dictEnumerator;
            private bool _usePropEnum;

            internal Enumerator(Args args)
            {
                _args = args;
                _usePropEnum = true;
                _propEnumerator = args
                    ._cachedProperties
                    .Select(p =>
                    {
                        args.TryGetPropertyValue(p, out var value, out var outkey, out _);
                        return new KeyValuePair<string, object>(outkey, value);
                    })
                    .Where(x => x.Key != null)
                    .GetEnumerator();
                _dictEnumerator = args._items.GetEnumerator();
            }

            public bool MoveNext()
            {
                if (_usePropEnum)
                {
                    if (_propEnumerator.MoveNext())
                        return true;
                    else
                        _usePropEnum = false;
                }
                return _dictEnumerator.MoveNext();
            }

            public KeyValuePair<string, object> Current
            {
                get
                {
                    if (_usePropEnum)
                        return _propEnumerator.Current;
                    return _dictEnumerator.Current;
                }
            }

            public void Dispose()
            {
            }

            object IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                _usePropEnum = true;
                _propEnumerator.Reset();
                _dictEnumerator.Reset();
            }
        }
    }
}
