using Redis.OM.Searching;
using Redis.OM;
using LoggerSpace;

namespace RedisWorkOMSpace
{
    public class RedisAbstractBase<T> where T : class, new()
    {
        protected RedisCollection<T>? list = null;
        protected Log log = new(true);

        public bool Add(T item)
        {
            var result = false;
            if (list == null) return result;
            try
            {
                list.Insert(item);
                result = true;
            }
            catch (Exception exp)
            {
                log.Error($"Add/{exp.Message}");
            }
            return result;
        }

        public bool Delete(T item)
        {
            var result = false;
            if (list == null) return result;
            try
            {
                list.Delete(item);

                result = true;
            }
            catch (Exception exp)
            {
                log.Error($"Delete/{exp.Message}");
            }
            return result;
        }

        protected void CheckIndex(RedisConnectionProvider provider, string indexName)
        {
            if (string.IsNullOrEmpty(indexName)) return;
            indexName= indexName.ToLower();
            var info = provider.Connection.Execute("FT._LIST").ToArray().Select(x => x.ToString());
            if (info.All(x => x != indexName))
            {
                provider.Connection.CreateIndex(typeof(T));
            }
        }
    }
}
