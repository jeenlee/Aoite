using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Collections.Generic
{
    /// <summary>
    /// 表示一致性哈希算法的集合。
    /// </summary>
    /// <typeparam name="TNode">哈希节点的数据类型。</typeparam>
    public class ConsistentHash<TNode>
    {
        private SortedDictionary<int, TNode> _circle = new SortedDictionary<int, TNode>();
        private int _weight;
        private int[] _keys = new int[0];

        /// <summary>
        /// 初始化一个 <see cref="System.Collections.Generic.ConsistentHash&lt;TNode&gt;"/> 类的新实例。
        /// </summary>
        public ConsistentHash() : this(new TNode[0]) { }
        /// <summary>
        /// 指定默认节点的集合，初始化一个 <see cref="System.Collections.Generic.ConsistentHash&lt;TNode&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="nodes">默认节点的集合。</param>
        public ConsistentHash(IEnumerable<TNode> nodes) : this(nodes, 100) { }

        /// <summary>
        /// 指定默认节点的集合和权重值，初始化一个 <see cref="System.Collections.Generic.ConsistentHash&lt;TNode&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="nodes">默认节点的集合。</param>
        /// <param name="weight">节点权重值。</param>
        public ConsistentHash(IEnumerable<TNode> nodes, int weight)
        {
            if(nodes == null) throw new ArgumentNullException("nodes");
            if(weight < 1) throw new ArgumentOutOfRangeException("weight");
            this._weight = weight;

            foreach(TNode node in nodes)
            {
                this.Add(node, false);
            }
            this._keys = _circle.Keys.ToArray();
        }

        /// <summary>
        /// 添加新的节点。
        /// </summary>
        /// <param name="node">节点。</param>
        public void Add(TNode node)
        {
            if(node == null) throw new ArgumentNullException("node");
            this.Add(node, true);
        }

        private void Add(TNode node, bool updateKeyArray)
        {
            for(int i = 0; i < _weight; i++)
            {
                var hash = BetterHash(node.GetHashCode().ToString() + i);
                this._circle[hash] = node;
            }

            if(updateKeyArray)
            {
                this._keys = this._circle.Keys.ToArray();
            }
        }

        /// <summary>
        /// 移除一个节点。
        /// </summary>
        /// <param name="node">节点。</param>
        /// <returns>成功返回 true，否则返回 false。</returns>
        public bool Remove(TNode node)
        {
            if(node == null) throw new ArgumentNullException("node");
            for(int i = 0; i < _weight; i++)
            {
                int hash = BetterHash(node.GetHashCode().ToString() + i);
                if(!this._circle.Remove(hash)) return false;
            }
            this._keys = _circle.Keys.ToArray();
            return true;
        }

        //return the index of first item that >= val.
        //if not exist, return 0;
        //ay should be ordered array.
        int First_ge(int[] ay, int val)
        {
            int begin = 0;
            int end = ay.Length - 1;

            if(ay[end] < val || ay[0] > val)
            {
                return 0;
            }

            int mid = begin;
            while(end - begin > 1)
            {
                mid = (end + begin) / 2;
                if(ay[mid] >= val)
                {
                    end = mid;
                }
                else
                {
                    begin = mid;
                }
            }

            if(ay[begin] > val || ay[end] < val)
            {
                throw new Exception("不应该发生的错误。");
            }

            return end;
        }

        /// <summary>
        /// 获取指定键的节点。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回一个节点。</returns>
        public TNode GetNode(string key)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            int hash = BetterHash(key);

            int first = First_ge(this._keys, hash);

            return this._circle[this._keys[first]];
        }
        
        /// <summary>
        /// 获取指定键的节点。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回一个节点。</returns>
        public TNode GetNode(byte[] key)
        {
            if(key == null) throw new ArgumentNullException("key");

            int hash = (int)MurmurHash2.Hash(key);
            int first = First_ge(this._keys, hash);
            return this._circle[this._keys[first]];
        }

        //default String.GetHashCode() can't well spread strings like "1", "2", "3"
        static int BetterHash(string key)
        {
            //return KetamaHash.Utils.HashAlgorithm.hash(KetamaHash.Utils.HashAlgorithm.computeMd5(key),0);
            uint hash = MurmurHash2.Hash(Encoding.ASCII.GetBytes(key));
            return (int)hash;
        }
    }
    class MurmurHash2
    {
        public static UInt32 Hash(Byte[] data)
        {
            return Hash(data, 0xc58f1a7b);
        }
        const UInt32 m = 0x5bd1e995;
        const Int32 r = 24;

        [StructLayout(LayoutKind.Explicit)]
        struct BytetoUInt32Converter
        {
            [FieldOffset(0)]
            public Byte[] Bytes;

            [FieldOffset(0)]
            public UInt32[] UInts;
        }

        public static UInt32 Hash(Byte[] data, UInt32 seed)
        {
            Int32 length = data.Length;
            if(length == 0)
                return 0;
            UInt32 h = seed ^ (UInt32)length;
            Int32 currentIndex = 0;
            // array will be length of Bytes but contains Uints
            // therefore the currentIndex will jump with +1 while length will jump with +4
            UInt32[] hackArray = new BytetoUInt32Converter { Bytes = data }.UInts;
            while(length >= 4)
            {
                UInt32 k = hackArray[currentIndex++];
                k *= m;
                k ^= k >> r;
                k *= m;

                h *= m;
                h ^= k;
                length -= 4;
            }
            currentIndex *= 4; // fix the length
            switch(length)
            {
                case 3:
                    h ^= (UInt16)(data[currentIndex++] | data[currentIndex++] << 8);
                    h ^= (UInt32)data[currentIndex] << 16;
                    h *= m;
                    break;
                case 2:
                    h ^= (UInt16)(data[currentIndex++] | data[currentIndex] << 8);
                    h *= m;
                    break;
                case 1:
                    h ^= data[currentIndex];
                    h *= m;
                    break;
                default:
                    break;
            }

            // Do a few final mixes of the hash to ensure the last few
            // bytes are well-incorporated.

            h ^= h >> 13;
            h *= m;
            h ^= h >> 15;

            return h;
        }
    }
}
