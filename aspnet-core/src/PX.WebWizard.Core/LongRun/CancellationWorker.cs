using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PX.WebWizard.LongRun
{
    [UnitOfWork(IsDisabled = true)]
    public class CancellationWorker : ICancellationWorker, ISingletonDependency
    {
        private readonly Dictionary<object, CancellationTokenSource> _tokenSources;
        public CancellationWorker()
        {
            _tokenSources = new Dictionary<object, CancellationTokenSource>(32);
            Tokens = new TokensDictionary(_tokenSources);
        }

        public IReadOnlyDictionary<object, CancellationToken> Tokens { get; }

        public CancellationToken Add(object id)
        {
            if (_tokenSources.ContainsKey(id))
                throw new ArgumentException("Token with this key already exists.");
            var tokenSource = new CancellationTokenSource();
            _tokenSources[id] = tokenSource;
            return tokenSource.Token;
        }

        public CancellationToken AddOrGet(object id)
        {
            if (_tokenSources.TryGetValue(id, out var value))
                return value.Token;
            var tokenSource = new CancellationTokenSource();
            _tokenSources[id] = tokenSource;
            return tokenSource.Token;
        }

        public bool RequestCancellation(object id, bool throwOnFirstException = false)
        {
            if (_tokenSources.TryGetValue(id, out var source))
            {
                try
                {
                    source.Cancel(throwOnFirstException);
                    return true;
                }
                catch /*Invalid opertaion exception or Aggregate exception? */
                {
                    return false;
                }
            }
            return false;
        }

        public bool Remove(object id)
        {
            return _tokenSources.Remove(id);
        }

        private class TokensDictionary : IReadOnlyDictionary<object, CancellationToken>
        {
            private readonly IDictionary<object, CancellationTokenSource> _tokenSources;

            public TokensDictionary(IDictionary<object, CancellationTokenSource> sources)
            {
                _tokenSources = sources;
            }

            public CancellationToken this[object key] => _tokenSources[key].Token;

            public IEnumerable<object> Keys
                => ((IReadOnlyDictionary<object, CancellationTokenSource>)_tokenSources).Keys;

            public IEnumerable<CancellationToken> Values
                => ((IReadOnlyDictionary<object, CancellationTokenSource>)_tokenSources).Values.Select(t => t.Token);

            public int Count => _tokenSources.Count;

            public bool ContainsKey(object key)
            {
                return _tokenSources.ContainsKey(key);
            }

            private IEnumerable<KeyValuePair<object, CancellationToken>> Enumerate()
            {
                foreach (var source in _tokenSources)
                {
                    yield return new KeyValuePair<object, CancellationToken>(
                        source.Key,
                        source.Value.Token
                    );
                }
            }

            public IEnumerator<KeyValuePair<object, CancellationToken>> GetEnumerator()
            {
                return Enumerate().GetEnumerator();
            }

            public bool TryGetValue(object key, out CancellationToken value)
            {
                var result = _tokenSources.TryGetValue(key, out var source);
                if (!result)
                {
                    value = default;
                    return false;
                }
                value = source.Token;
                return true;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
