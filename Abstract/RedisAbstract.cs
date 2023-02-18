using Redis.OM;
using LoggerSpace;
using Redis.OM.Searching;
using System.Linq.Expressions;
using StackExchange.Redis;

namespace RedisWorkOMSpace
{
    public sealed class RedisAbstract<T> : RedisAbstractBase<T> where T : class, new()
    {
        RedisConnectionProvider? provider = null;
        public RedisAbstract(Log log, RedisConnectionProvider? provider, string indexName)
        {
            this.log = log;
            Ini(provider, $"{indexName}-idx");
        }

        public T[]? GetSearchResult(Expression<Func<T, bool>>? expression = null)
        {
            T[]? result = null;
            if (list == null || expression==null) return null;

            try
            {
                var r = list.Where(expression);
                if (r != null)
                {
                    result = r.ToArray();
                }
                else
                    log.TraceInfo("GetSearchResult r is null");
            }
            catch (RedisException exp0)
            {
                log.Error($"GetSearchResult/{exp0.Message}");
            }
            catch (Exception exp)
            {
                log.Error($"GetSearchResult/{exp.Message}");
            }
            return result;
        }

        public void LoadData(T[] items)
        {
            if (items == null) return;
            if (provider == null) return;

            list = (RedisCollection<T>)provider.RedisCollection<T>();
            foreach (var item in items)
            {
                list.Insert(item);
            }
        }

        private void Ini(RedisConnectionProvider? provider_, string indexName)
        {
            try
            {
                if (provider_ == null) return;
                provider = provider_;

                CheckIndex(provider_, indexName);
            }
            catch (Exception exp)
            {
                log.Error($"Ini/{exp.Message}");
            }
        }
    }
}
