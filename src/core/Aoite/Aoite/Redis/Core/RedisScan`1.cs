using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    internal class RedisScan<T> : IEnumerable<T>
    {
        private const long DefaultScanCount = 10;

        private long _cursor;
        private string _pattern;
        private long _count;
        private IRedisClient _client;
        private string _command;
        private string _key;
        private long _parseItemCount;
        private Func<string, object[], RedisCommand<T>> _commandFactory;

        public RedisScan(IRedisClient client, string command, string key, long cursor, string pattern, long count
            , Func<string, object[], RedisCommand<T>> commandFactory, long parseItemCount = 1)
        {
            this._client = client;
            this._command = command;
            this._key = key;
            this._cursor = cursor;
            this._pattern = pattern;
            this._count = count;
            this._commandFactory = commandFactory;
            this._parseItemCount = parseItemCount;
        }

        private bool CreateScanStack(Queue<T> stack, ref long cursor)
        {
            var args = new List<object>();
            if(this._key != null) args.Add(this._key);
            args.Add(cursor);
            if(this._pattern != null) args.AddRange(new[] { "MATCH", this._pattern });
            if(this._count != DefaultScanCount) args.AddRange(new object[] { "COUNT", this._count });

            var command = new RedisArray.Scan<T>(this._commandFactory(this._command, args.ToArray()), this._parseItemCount);
            var keys = this._client.Execute(command);
            if(keys == null || keys.Length == 0)
            {
                cursor = -1;
                return false;
            }
            cursor = command.Cursor;
            if(cursor == 0) cursor = -1;

            foreach(var key in keys) stack.Enqueue(key);
            return true;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this, this._cursor);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal class Enumerator : IEnumerator<T>
        {
            Queue<T> _stack = null;
            RedisScan<T> _scan;
            private T _current;
            private long _cursor;
            public long Cursor
            {
                get
                {
                    // -1 是自己假定，枚举器已经没必要继续往下跑了，因为为 0 时，可能是首次枚举
                    if(this._cursor == -1) return 0;
                    return this._cursor;
                }
            }
            public T[] Items
            {
                get
                {
                    if(this._stack == null) return new T[0];
                    return this._stack.ToArray();
                }
            }
            public Enumerator(RedisScan<T> scan, long cursor)
            {
                this._scan = scan;
                this._cursor = cursor;
                this._stack = new Queue<T>((int)scan._count + 1);
            }

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            public T Current { get { return this._current; } }

            public void Dispose()
            {
                if(this._stack != null) this._stack.Clear();
                this._stack = null;
                this._scan = null;
            }

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
            object System.Collections.IEnumerator.Current { get { return this.Current; } }

            public bool MoveNext()
            {
                if(this._stack.Count == 0 && (this._cursor == -1 || !this._scan.CreateScanStack(this._stack, ref this._cursor)))
                    return false;
                this._current = this._stack.Dequeue();
                return true;
            }

            public void Reset()
            {
                if(this._stack != null) this._stack.Clear();
                this._scan._cursor = 0;
            }
        }
    }
}
