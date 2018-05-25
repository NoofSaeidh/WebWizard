using Abp.Dependency;
using Abp.Domain.Repositories;
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
    public class LongRunCancellationWorker : PeriodicBackgroundWorkerBase, ILongRunCancellationWorker, ISingletonDependency
    {
        private readonly Dictionary<string, CancellationTokenSource> _tokenSources;
        private readonly IRepository<LongRunInfo, string> _longRunInfoRepo;
        public LongRunCancellationWorker(AbpTimer abpTimer,
            IRepository<LongRunInfo, string> longRunInfoRepo)
            : base(abpTimer)
        {
            _tokenSources = new Dictionary<string, CancellationTokenSource>(32);
            _longRunInfoRepo = longRunInfoRepo;
            Tokens = new TokensDictionary(_tokenSources);
            // 10 seconds
            // todo: move to configurations
            Timer.Period = 10000;
        }

        public IReadOnlyDictionary<string, CancellationToken> Tokens { get; }

        public void PushLongRun(string longRunId)
        {
            if (_tokenSources.ContainsKey(longRunId))
                throw new ArgumentException($"Long run with id {longRunId} already watched.");
            _tokenSources.TryAdd(longRunId, new CancellationTokenSource());
        }


        protected override void DoWork()
        {
            var toRemove = new List<string>(_tokenSources.Count);
            Parallel.ForEach(_tokenSources, async s =>
            {
                var status = await CheckLongRunHealth(s.Key);
                if (status)
                    return;

                toRemove.Add(s.Key);
                s.Value.Cancel();
            });
            toRemove.ForEach(x => _tokenSources.Remove(x));
        }

        protected async Task<bool> CheckLongRunHealth(string longRunId)
        {
            try
            {
                var longRunInfo = await _longRunInfoRepo.GetAsync(longRunId);
                if (longRunInfo.LongRunStatus != LongRunStatus.Queued
                    && longRunInfo.LongRunStatus != LongRunStatus.InProcess)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private class TokensDictionary : IReadOnlyDictionary<string, CancellationToken>
        {
            private readonly IDictionary<string, CancellationTokenSource> _tokenSources;

            public TokensDictionary(IDictionary<string, CancellationTokenSource> sources)
            {
                _tokenSources = sources;
            }

            public CancellationToken this[string key] => _tokenSources[key].Token;

            public IEnumerable<string> Keys
                => ((IReadOnlyDictionary<string, CancellationTokenSource>)_tokenSources).Keys;

            public IEnumerable<CancellationToken> Values
                => ((IReadOnlyDictionary<string, CancellationTokenSource>)_tokenSources).Values.Select(t => t.Token);

            public int Count => _tokenSources.Count;

            public bool ContainsKey(string key)
            {
                return _tokenSources.ContainsKey(key);
            }

            private IEnumerable<KeyValuePair<string, CancellationToken>> Enumerate()
            {
                foreach (var source in _tokenSources)
                {
                    yield return new KeyValuePair<string, CancellationToken>(
                        source.Key,
                        source.Value.Token
                    );
                }
            }

            public IEnumerator<KeyValuePair<string, CancellationToken>> GetEnumerator()
            {
                return Enumerate().GetEnumerator();
            }

            public bool TryGetValue(string key, out CancellationToken value)
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
