using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示一个 Redis 的实体集合。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public class RedisCollection<TEntity>
    {
        static readonly Aoite.Data.IEntityMapper Mapper;
        static readonly Aoite.Data.PropertyMapper KeyProperty;
        static readonly string HashKey;
        const string SequenceHashKeyName = "$Sequence";
        static RedisCollection()
        {
            Mapper = Aoite.Data.EntityMapper.Instance<TEntity>.Mapper;
            var keys = (from mp in Mapper.Properties where mp.Column.IsPrimaryKey select mp).ToArray();
            if(keys.Length == 0) keys = (from mp in Mapper.Properties where string.Equals(mp.Name, "id", StringComparison.OrdinalIgnoreCase) select mp).ToArray();
            if(keys.Length == 0) throw new NotSupportedException("找不到 {0} 的主键。".Fmt(Mapper.Type.FullName));
            if(keys.Length != 1) throw new NotSupportedException("类型 {0} 不允许多个主键。".Fmt(Mapper.Type.FullName));
            KeyProperty = keys[0];
            HashKey = "$E<" + Mapper.Type.FullName + ">";
        }

        RedisClient _client;
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Redis.RedisCollection&lt;TEntity&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="client">客户端。</param>
        public RedisCollection(RedisClient client)
        {
            if(client == null) throw new ArgumentNullException("client");
            this._client = client;
        }

        /// <summary>
        /// 获取指定递增量的实体序列。
        /// </summary>
        /// <param name="increment">递增量。</param>
        /// <returns>返回新的序列号。</returns>
        public long GetNextSequence(long increment = 1)
        {
            if(increment < 2)
                return this._client.HIncr(SequenceHashKeyName, HashKey);
            return this._client.HIncrBy(SequenceHashKeyName, HashKey, increment);
        }

        /// <summary>
        /// 重置实体的序列。
        /// </summary>
        public void ResetSequence()
        {
            this._client.HDel(SequenceHashKeyName, HashKey);
        }

        private string GetKeyString(object key)
        {
            if(key == null) throw new ArgumentNullException("key");
            return key.ToString();
        }
        /// <summary>
        /// 设置一个实体。
        /// </summary>
        /// <param name="entity">设置的实体。</param>
        public void Set(TEntity entity)
        {
            if(entity == null) throw new ArgumentNullException("entity");
            var key = KeyProperty.GetValue(entity);
            this._client.HSet(HashKey, this.GetKeyString(key), Serializer.Quickly.FastWriteBytes(entity));
        }

        /// <summary>
        /// 获取指定键的实体。
        /// </summary>
        /// <param name="key">实体的编。</param>
        /// <returns>一个实体或 null 值。</returns>
        public TEntity Get(object key)
        {
            var bytes = this._client.HGetBytes(HashKey, this.GetKeyString(key));
            if(bytes == null) return default(TEntity);
            return Serializer.Quickly.FastReadBytes<TEntity>(bytes);
        }
        /// <summary>
        /// 删除指定键的实体。
        /// </summary>
        /// <param name="key">待删除的键。</param>
        public void Remove(object key)
        {
            this._client.HDel(HashKey, this.GetKeyString(key));
        }
        /// <summary>
        /// 批量删除指定键的实体。
        /// </summary>
        /// <param name="keys">待删除的键列表。</param>
        public void Remove(params object[] keys)
        {
            this._client.HDel(HashKey, keys.Select(this.GetKeyString).ToArray());
        }
        /// <summary>
        /// 判断当前集合是否包含指定键的实体。
        /// </summary>
        /// <param name="key">实体的编。</param>
        /// <returns>包含返回 true，否则返回 false。</returns>
        public bool Contains(object key)
        {
            return this._client.HExists(HashKey, Convert.ToString(key));
        }
        /// <summary>
        /// 清空集合。
        /// </summary>
        public void Clear()
        {
            this._client.Del(HashKey);
        }

        /// <summary>
        /// 获取所有的实体。
        /// </summary>
        /// <returns>一个实体枚举器。</returns>
        public IEnumerable<TEntity> GetAll()
        {
            var values = this._client.HGetAllBytes(HashKey).Values;
            return from value in values
                   select Serializer.Quickly.FastReadBytes<TEntity>(value);
        }

        /// <summary>
        /// 获取实体的元素数。
        /// </summary>
        public long Count { get { return this._client.HLen(HashKey); } }
    }
}
