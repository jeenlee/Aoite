using Aoite.Redis.Internal.IO;
using Aoite.Redis.Internal.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Aoite.Redis.Internal.Commands
{
    class RedisHashBytes : RedisCommand<Dictionary<string, byte[]>>
    {
        public RedisHashBytes(string command, params object[] args)
            : base(command, args) { }

        public override Dictionary<string, byte[]> Parse(RedisReader reader)
        {
            reader.ExpectType(RedisMessage.MultiBulk);
            long count = reader.ReadInt(false);
            var dict = new Dictionary<string, byte[]>(Convert.ToInt32(count / 2));
            string key = String.Empty;
            for(int i = 0; i < count; i++)
            {
                if(i % 2 == 0)
                    key = reader.ReadBulkString();
                else
                    dict[key] = reader.ReadBulk();
            }
            return dict;
        }
    }
    class RedisHash : RedisCommand<Dictionary<string, string>>
    {
        public RedisHash(string command, params object[] args)
            : base(command, args)
        { }

        public override Dictionary<string, string> Parse(RedisReader reader)
        {
            return ToDict(reader);
        }

        static Dictionary<string, string> ToDict(RedisReader reader)
        {
            reader.ExpectType(RedisMessage.MultiBulk);
            long count = reader.ReadInt(false);
            var dict = new Dictionary<string, string>();
            string key = String.Empty;
            for(int i = 0; i < count; i++)
            {
                if(i % 2 == 0)
                    key = reader.ReadBulkString();
                else
                    dict[key] = reader.ReadBulkString();
            }
            return dict;
        }

        public class Generic<T> : RedisCommand<T>
            where T : class
        {
            public Generic(string command, params object[] args)
                : base(command, args)
            { }

            public override T Parse(RedisReader reader)
            {
                return Serializer<T>.Deserialize(RedisHash.ToDict(reader));
            }
        }
    }
}
