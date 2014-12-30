//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Aoite.Redis
//{
//    public class RedisBase
//    {
//        private string _Host;
//        public string Host
//        {
//            get { return this._Host; }
//            set
//            {
//                if(string.IsNullOrEmpty(value)) throw new ArgumentNullException("host");
//                this._Host = value;
//            }
//        }

//        private int _Port;
//        public int Port
//        {
//            get { return this._Port; }
//            set
//            {
//                if(value < 1 || value > 65535) throw new ArgumentOutOfRangeException("port");
//                this._Port = value;
//            }
//        }

//        public override string ToString()
//        {
//            return this.Host + ":" + this.Port;
//        }
//    }
//    public class RedisInfo : RedisBase
//    {
//        public RedisNode Node { get; internal set; }
//    }

//    public class RedisNode : RedisBase
//    {
//        private int _Weight;
//        public int Weight
//        {
//            get { return this._Weight; }
//            set
//            {
//                if(value < 1) throw new ArgumentOutOfRangeException("weight");
//                this._Weight = value;
//            }
//        }

//        public DateTime? FaildTime { get; set; }

//        public RedisNode() { }
//        public RedisNode(string host, int port = RedisClient.DefaultPort, int weight = 1)
//        {
//            this.Host = host;
//            this.Port = port;
//            this.Weight = weight;
//        }

//        internal IEnumerable<RedisInfo> CreateInfos()
//        {
//            for(int i = 0; i < this._Weight; i++)
//            {
//                yield return new RedisInfo() { Host = this.Host, Port = this.Port, Node = this };
//            }
//        }
//    }

//    public class RedisBox
//    {
//        class RedisClientPool : ObjectPool<RedisClient>
//        {
//            private readonly static Exception AllShutdownException = new NotSupportedException("所有的 Redis 服务器均已崩溃。");
//            public override RedisClient Acquire()
//            {
//                Queue<RedisClient> faildClients = null;
//                RedisClient client;
//                while(true)
//                {
//                    client = base.Acquire();
//                    if(client.OwnerInfo.Node.FaildTime.HasValue)
//                    {
//                        if(faildClients == null) faildClients = new Queue<RedisClient>();
//                        faildClients.Enqueue(client);
//                        continue;
//                    }

//                    if(!client.Connected)
//                    {
//                        try
//                        {
//                            client.Connect(0);
//                        }
//                        catch(Exception)
//                        {
//                            client.OwnerInfo.Node.FaildTime = DateTime.Now;
//                            if(faildClients == null) faildClients = new Queue<RedisClient>();
//                            faildClients.Enqueue(client);
//                            continue;
//                        }
//                    }
//                    if(!string.IsNullOrEmpty(this._box.Password))
//                        client.Auth(this._box.Password).ThrowIfFailded();

//                    if(faildClients != null)
//                    {
//                        while(faildClients.Count > 0)
//                        {
//                            this.Release(faildClients.Dequeue());
//                        }
//                    }
//                    return client;
//                }
//            }

//            RedisNode[] _nodes;
//            LinkedList<RedisInfo> _linkedList;
//            LinkedListNode<RedisInfo> _current;
//            RedisBox _box;
//            public RedisClientPool(RedisBox box, RedisNode[] nodes)
//            {
//                this._box = box;
//                this._nodes = nodes;
//                this._linkedList = new LinkedList<RedisInfo>(from node in nodes
//                                                             from info in node.CreateInfos()
//                                                             select info);
//                this._current = this._linkedList.Last;
//                Ajob.Loop(Resume, 10000);
//            }
//            private void Resume(IAsyncJob job)
//            {
//                foreach(var node in _nodes)
//                {
//                    if(node.FaildTime.HasValue)
//                    {
//                        using(RedisClient client = new RedisClient(node.Host, node.Port))
//                        {

//                            try
//                            {
//                                client.Connect(500);
//                                if(client.Connected) node.FaildTime = null;
//                            }
//                            catch(Exception) { }
//                        }
//                    }
//                }
//            }

//            private readonly object SyncObject = new object();
//            protected override RedisClient OnCreateObject()
//            {
//                lock(SyncObject)
//                {
//                    int testIndex = 0;
//                    while(true)
//                    {
//                        this._current = this._current.Next == null ? this._current.List.First : this._current.Next;
//                        if(this._current.Value.Node.FaildTime.HasValue)
//                        {
//                            testIndex++;
//                            if(testIndex >= this._linkedList.Count) throw AllShutdownException;
//                            continue;
//                        }
//                        return new RedisClient(this._current.Value);
//                    }
//                }
//            }
//        }

//        public string Password { get; set; }

//        RedisClientPool _clients;
//        public RedisBox(string host, int port) : this(new RedisNode(host, port)) { }

//        public RedisBox(params RedisNode[] nodes)
//        {
//            if(nodes == null || nodes.Length == 0) throw new ArgumentNullException("nodes");
//            this._clients = new RedisClientPool(this, nodes);
//        }

//        /// <summary>
//        /// 执行。
//        /// </summary>
//        /// <param name="callback">回调函数。</param>
//        public virtual void Exec(Action<RedisClient> callback)
//        {
//            this._clients.AcquireRelease(callback);
//        }

//        /// <summary>
//        /// 执行。
//        /// </summary>
//        /// <typeparam name="TResult">返回的数据类型。</typeparam>
//        /// <param name="callback">回调函数。</param>
//        /// <returns>返回回调函数的返回值。</returns>
//        public virtual TResult Exec<TResult>(Func<RedisClient, TResult> callback)
//        {
//            return this._clients.AcquireRelease(callback);
//        }
//    }

//}
