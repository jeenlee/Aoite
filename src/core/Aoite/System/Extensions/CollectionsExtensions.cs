//using Aoite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Collections.Generic
{
    /// <summary>
    /// 提供用于集合的实用工具方法。
    /// </summary>
    public static class CollectionsExtensions
    {
        /// <summary>
        /// 将指定的集合转换为实体的数据集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="collection">集合。</param>
        /// <param name="totalCount">数据的总行数。</param>
        /// <returns>返回一个实体的数据集合。。</returns>
        public static GridData<TEntity> ToGrid<TEntity>(this IEnumerable<TEntity> collection, long totalCount = 0)
        {
            if(collection == null) throw new ArgumentNullException("collection");
            return new GridData<TEntity>() { Rows = collection.ToArray(), Total = totalCount };
        }
        /* TODO：未完成。需要完成 EntityMapper 模块。
        /// <summary>
        /// 将指定的集合转换为数据表格。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="collection">集合。</param>
        /// <param name="totalCount">数据的总行数。</param>
        /// <returns>返回一张表。</returns>
        public static AoiteTable ToTable<TEntity>(this IEnumerable<TEntity> collection, long totalCount = 0)
        {
            if(collection == null) throw new ArgumentNullException("collection");
            var mp = EntityMapper.Instance<TEntity>.Mapper;
            AoiteTable table = new AoiteTable() { TableName = mp.Name };
            foreach(var p in mp.Properties) table.Columns.Add(p.Name, p.Property.PropertyType);
            table.BeginLoadData();
            foreach(var item in collection)
            {
                var row = table.NewRow();
                mp.FillRow(item, row);
                table.Rows.Add(row);
            }
            table.EndLoadData();
            table.TotalRowCount = totalCount > 0 ? totalCount : table.Rows.Count;
            return table;
        }
         */

        /// <summary>
        /// 获取与指定的键相关联的值。
        /// </summary>
        /// <typeparam name="TKey">字典中的键的类型。</typeparam>
        /// <typeparam name="TValue">字典中的值的类型。</typeparam>
        /// <param name="dict">字典。</param>
        /// <param name="key">字典的键。</param>
        /// <returns>如果字典包含具有指定键的元素则返回对应的值，否则返回默认值。</returns>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            if(dict == null) throw new ArgumentNullException("dict");
            TValue value;
            if(dict.TryGetValue(key, out value)) return value;
            return default(TValue);
        }

        /// <summary>
        /// 判定指定的字节数组是否匹配。
        /// </summary>
        /// <param name="b1">第一个字节数组。</param>
        /// <param name="b2">比较的字节数组。</param>
        /// <returns>如果匹配返回 true，否则返回 false。</returns>
        public static bool EqualsBytes(this byte[] b1, byte[] b2)
        {
            if(b1 == null) return b2 == null;
            if(b2 == null) return false;
            var length = b1.LongLength;
            if(length != b2.LongLength) return false;
            if(length > 1024) memcmp(b1, b2, length);
            return EqualsBytesShort(b1, b2);
        }

        static unsafe bool EqualsBytesShort(byte[] b1, byte[] b2)
        {
            fixed(byte* p1 = b1, p2 = b2)
            {
                byte* x1 = p1, x2 = p2;
                int l = b1.Length;
                for(int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                    if(*((long*)x1) != *((long*)x2)) return false;
                if((l & 4) != 0) { if(*((int*)x1) != *((int*)x2)) return false; x1 += 4; x2 += 4; }
                if((l & 2) != 0) { if(*((short*)x1) != *((short*)x2)) return false; x1 += 2; x2 += 2; }
                if((l & 1) != 0) if(*((byte*)x1) != *((byte*)x2)) return false;
                return true;
            }
        }
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);

        /// <summary>
        /// 随机取出指定枚举的一部分元素。
        /// </summary>
        /// <param name="e">枚举。</param>
        /// <param name="max">固定枚举元素数，为 0 则随机。</param>
        public static IEnumerable<T> RandomAny<T>(this IEnumerable<T> e, int max = 0)
        {
            if(e == null) throw new ArgumentNullException("e");

            long tick = DateTime.Now.Ticks;
            List<T> list = new List<T>(e);
            if(max < 1) max = FastRandom.Instance.Next(1, list.Count);
            if(max > list.Count) throw new ArgumentOutOfRangeException("max");

            for(int i = 0; i < max; i++)
            {
                var index = FastRandom.Instance.Next(0, list.Count);
                var item = list[index];
                list.Remove(item);
                yield return item;
            }
        }

        /// <summary>
        /// 随机取出指定枚举的一个元素。
        /// </summary>
        /// <param name="e">枚举。</param>
        public static T RandomOne<T>(this IEnumerable<T> e)
        {
            if(e == null) throw new ArgumentNullException("e");
            return RandomAny(e, 1).First();
        }

        /// <summary>
        /// 把集合中的所有元素放入一个字符串。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="items">原集合。</param>
        /// <param name="callback">回调函数。</param>
        /// <param name="separator">分隔符。</param>
        /// <param name="start">起始文本。如果集合为空，不包含此数据。</param>
        /// <param name="end">结尾文本。如果集合为空，不包含此数据。</param>
        /// <returns>拼接后的字符串。</returns>
        public static string Join<T>(this IEnumerable<T> items, Func<T, string> callback, string separator = ",", string start = null, string end = null)
        {
            Text.StringBuilder builder = new Text.StringBuilder();
            foreach(var item in items)
            {
                if(builder.Length > 0) builder.Append(separator);
                builder.Append(callback(item));
            }
            if(builder.Length == 0) return string.Empty;
            if(start != null) builder.Insert(0, start);
            if(end != null) builder.Append(end);
            return builder.ToString();
        }

        /// <summary>
        /// 把集合中的所有元素放入一个字符串。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="items">原集合。</param>
        /// <param name="separator">分隔符。</param>
        /// <param name="start">起始文本。如果集合为空，不包含此数据。</param>
        /// <param name="end">结尾文本。如果集合为空，不包含此数据。</param>
        /// <returns>拼接后的字符串。</returns>
        public static string Join<T>(this IEnumerable<T> items, string separator = ",", string start = null, string end = null)
        {
            return Join(items, t => Convert.ToString(t), separator, start, end);
        }

        /// <summary>
        /// 搜索指定的对象，并返回整个 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 中第一个匹配项的索引。
        /// </summary>
        /// <typeparam name="T">列表元素的类型。</typeparam>
        /// <param name="collection">要搜索的从零开始的 <see cref="System.Collections.Generic.IList&lt;T&gt;"/>。</param>
        /// <param name="value">要在 <paramref name="collection"/> 中查找的对象。</param>
        /// <param name="comparer">一个对值进行比较的相等比较器。</param>
        /// <returns>如果在整个 <paramref name="collection"/> 中找到 <paramref name="value"/> 的匹配项，则为第一个匹配项的从零开始的索引；否则为 -1。</returns>
        public static int IndexOf<T>(this IList<T> collection
            , T value
            , IEqualityComparer<T> comparer)
        {
            return IndexOf<T>(collection, value, 0, collection.Count, comparer);
        }

        /// <summary>
        /// 搜索指定的对象，并返回 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 中从指定索引到最后一个元素这部分元素中第一个匹配项的索引。
        /// </summary>
        /// <typeparam name="T">列表元素的类型。</typeparam>
        /// <param name="collection">要搜索的从零开始的 <see cref="System.Collections.Generic.IList&lt;T&gt;"/>。</param>
        /// <param name="value">要在 <paramref name="collection"/> 中查找的对象。</param>
        /// <param name="startIndex">从零开始的搜索的起始索引。</param>
        /// <param name="comparer">一个对值进行比较的相等比较器。</param>
        /// <returns>如果在 <paramref name="collection"/> 中从 <paramref name="startIndex"/> 到最后一个元素这部分元素中找到 <paramref name="value"/> 的匹配项，则为第一个匹配项的从零开始的索引；否则为 -1。</returns>
        public static int IndexOf<T>(this IList<T> collection
            , T value
            , int startIndex
            , IEqualityComparer<T> comparer)
        {
            return IndexOf<T>(collection, value, startIndex, collection.Count, comparer);
        }

        /// <summary>
        /// 搜索指定的对象，并返回 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 中从指定索引开始包含指定个元素的这部分元素中第一个匹配项的索引。
        /// </summary>
        /// <typeparam name="T">列表元素的类型。</typeparam>
        /// <param name="collection">要搜索的从零开始的 <see cref="System.Collections.Generic.IList&lt;T&gt;"/>。</param>
        /// <param name="value">要在 <paramref name="collection"/> 中查找的对象。</param>
        /// <param name="startIndex">从零开始的搜索的起始索引。</param>
        /// <param name="count">要搜索的部分中的元素数。</param>
        /// <param name="comparer">一个对值进行比较的相等比较器。</param>
        /// <returns>如果在 <paramref name="collection"/> 中从 <paramref name="startIndex"/> 开始、包含 <paramref name="count"/> 所指定的元素个数的这部分元素中，找到 <paramref name="value"/> 的匹配项，则为第一个匹配项的从零开始的索引；否则为 -1。</returns>
        public static int IndexOf<T>(this IList<T> collection
            , T value
            , int startIndex
            , int count
            , IEqualityComparer<T> comparer)
        {
            if(collection == null) throw new ArgumentNullException("collection");
            if((startIndex < 0) || (startIndex > collection.Count)) throw new ArgumentOutOfRangeException("startIndex");
            if((count < 0) || (count > (collection.Count - startIndex))) throw new ArgumentOutOfRangeException("count");
            int num = startIndex + count;

            for(int i = startIndex; i < num; i++)
            {
                if(comparer.Equals(collection[i], value)) return i;
            }
            return -1;
        }
        /// <summary>
        /// 搜索指定的对象，并返回整个 <see cref="System.Array"/> 中第一个匹配项的索引。
        /// </summary>
        /// <typeparam name="T">数组元素的类型。</typeparam>
        /// <param name="array">要搜索的从零开始的一维 <see cref="System.Array"/>。</param>
        /// <param name="value">要在 <paramref name="array"/> 中查找的对象。</param>
        /// <param name="comparer">一个对值进行比较的相等比较器。</param>
        /// <returns>如果在整个 <paramref name="array"/> 中找到 <paramref name="value"/> 的匹配项，则为第一个匹配项的从零开始的索引；否则为 -1。</returns>
        public static int IndexOf<T>(this T[] array
            , T value
            , IEqualityComparer<T> comparer)
        {
            return IndexOf<T>(array, value, 0, array.Length, comparer);
        }

        /// <summary>
        /// 搜索指定的对象，并返回 <see cref="System.Array"/> 中从指定索引到最后一个元素这部分元素中第一个匹配项的索引。
        /// </summary>
        /// <typeparam name="T">数组元素的类型。</typeparam>
        /// <param name="array">要搜索的从零开始的一维 <see cref="System.Array"/>。</param>
        /// <param name="value">要在 <paramref name="array"/> 中查找的对象。</param>
        /// <param name="startIndex">从零开始的搜索的起始索引。</param>
        /// <param name="comparer">一个对值进行比较的相等比较器。</param>
        /// <returns>如果在 <paramref name="array"/> 中从 <paramref name="startIndex"/> 到最后一个元素这部分元素中找到 <paramref name="value"/> 的匹配项，则为第一个匹配项的从零开始的索引；否则为 -1。</returns>
        public static int IndexOf<T>(this T[] array
            , T value
            , int startIndex
            , IEqualityComparer<T> comparer)
        {
            return IndexOf<T>(array, value, startIndex, array.Length, comparer);
        }

        /// <summary>
        /// 搜索指定的对象，并返回 <see cref="System.Array"/> 中从指定索引开始包含指定个元素的这部分元素中第一个匹配项的索引。
        /// </summary>
        /// <typeparam name="T">数组元素的类型。</typeparam>
        /// <param name="array">要搜索的从零开始的一维<see cref="System.Array"/>。</param>
        /// <param name="value">要在<paramref name="array"/>中查找的对象。</param>
        /// <param name="startIndex">从零开始的搜索的起始索引。</param>
        /// <param name="count">要搜索的部分中的元素数。</param>
        /// <param name="comparer">一个对值进行比较的相等比较器。</param>
        /// <returns>如果在<paramref name="array"/>中从 <paramref name="startIndex"/> 开始、包含 <paramref name="count"/> 所指定的元素个数的这部分元素中，找到 <paramref name="value"/> 的匹配项，则为第一个匹配项的从零开始的索引；否则为 -1。</returns>
        public static int IndexOf<T>(this T[] array
            , T value
            , int startIndex
            , int count
            , IEqualityComparer<T> comparer)
        {
            if(array == null) throw new ArgumentNullException("array");
            if((startIndex < 0) || (startIndex > array.Length)) throw new ArgumentOutOfRangeException("startIndex");
            if((count < 0) || (count > (array.Length - startIndex))) throw new ArgumentOutOfRangeException("count");
            int num = startIndex + count;

            for(int i = startIndex; i < num; i++)
            {
                if(comparer.Equals(array[i], value)) return i;
            }
            return -1;
        }

        /// <summary>
        /// 搜索指定的对象，并返回整个 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 中第一个匹配项的索引。
        /// </summary>
        /// <typeparam name="T">列表元素的类型。</typeparam>
        /// <param name="collection">要搜索的从零开始的 <see cref="System.Collections.Generic.IList&lt;T&gt;"/>。</param>
        /// <param name="value">要在 <paramref name="collection"/> 中查找的对象。</param>
        /// <returns>如果在整个 <paramref name="collection"/> 中找到 <paramref name="value"/> 的匹配项，则为第一个匹配项的从零开始的索引；否则为 -1。</returns>
        public static int IndexOf<T>(this T[] collection
            , T value)
        {
            return Array.IndexOf(collection, value);
        }

    }
}
