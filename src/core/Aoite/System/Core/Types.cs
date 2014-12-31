using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 基本数据类型的集合。
    /// </summary>
    public static class Types
    {
        #region 框架类型

        //TODO 未完成。需要完成 Aoite.Data 模块
        ///// <summary>
        ///// 表示 <see cref="Aoite.Data.DbEngineManager"/> 的类型。
        ///// </summary>
        //public static readonly Type DbEngineManager = typeof(Aoite.Data.DbEngineManager);
        ///// <summary>
        ///// 表示 <see cref="System.IDbEngine"/> 的类型。
        ///// </summary>
        //public static readonly Type IDbEngine = typeof(IDbEngine);
        /// <summary>
        /// 表示 <see cref="System.Result&lt;TValue&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GResult = typeof(System.Result<>);
        /// <summary>
        /// 表示 <see cref="System.Result"/> 的类型。
        /// </summary>
        public static readonly Type Result = typeof(System.Result);

        #endregion

        #region 其他类型

        /// <summary>
        /// 表示 <see cref="System.Convert"/> 的类型。
        /// </summary>
        public static readonly Type Convert = typeof(System.Convert);
        /// <summary>
        /// 表示 <see cref="System.Delegate"/> 的类型。
        /// </summary>
        public static readonly Type Delegate = typeof(System.Delegate);
        /// <summary>
        /// 表示 <see cref="System.Enum"/> 的类型。
        /// </summary>
        public static readonly Type Enum = typeof(Enum);
        /// <summary>
        /// 表示 <see cref="System.Uri"/> 的类型。
        /// </summary>
        public static readonly Type Uri = typeof(Uri);
        /// <summary>
        /// 表示 <see cref="System.Exception"/> 的类型。
        /// </summary>
        public static readonly Type Exception = typeof(System.Exception);
        /// <summary>
        /// 表示 <see cref="System.IConvertible"/> 的类型。
        /// </summary>
        public static readonly Type IConvertible = typeof(IConvertible);
        /// <summary>
        /// 表示 <see cref="System.IDisposable"/> 的类型。
        /// </summary>
        public static readonly Type IDisposable = typeof(System.IDisposable);
        /// <summary>
        /// 表示 <see cref="System.IO.MemoryStream"/> 的类型。
        /// </summary>
        public static readonly Type MemoryStream = typeof(System.IO.MemoryStream);
        /// <summary>
        /// 表示 <see cref="System.IO.Stream"/> 的类型。
        /// </summary>
        public static readonly Type Stream = typeof(System.IO.Stream);
        /// <summary>
        /// 表示 <see cref="System.Nullable&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type Nullable = typeof(Nullable<>);
        /// <summary>
        /// 表示 <see cref="System.Text.RegularExpressions.Regex"/> 的类型。
        /// </summary>
        public static readonly Type Regex = typeof(System.Text.RegularExpressions.Regex);
        /// <summary>
        /// 表示 void 的类型。
        /// </summary>
        public static readonly Type Void = typeof(void);
        /// <summary>
        /// 表示 <see cref="System.Type"/> 的类型。
        /// </summary>
        public static readonly Type Type = typeof(Type);
        /// <summary>
        /// 表示 <see cref="System.Type"/>[] 的类型。
        /// </summary>
        public static readonly Type TypeArray = typeof(Type[]);

        #endregion

        #region 集合类型

        /// <summary>
        /// 表示 <see cref="System.Collections.ArrayList"/> 的类型。
        /// </summary>
        public static readonly Type ArrayList = typeof(System.Collections.ArrayList);
        /// <summary>
        /// 表示 <see cref="System.Array"/> 的类型。
        /// </summary>
        public static readonly Type Array = typeof(System.Array);
        /// <summary>
        /// 表示 <see cref="System.Collections.IEnumerator"/> 的类型。
        /// </summary>
        public static readonly Type IEnumerator = typeof(System.Collections.IEnumerator);
        /// <summary>
        /// 表示 <see cref="System.Collections.Generic.IEnumerable&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type IGEnumerable = typeof(System.Collections.Generic.IEnumerable<>);
        /// <summary>
        /// 表示 <see cref="System.Collections.Hashtable"/> 的类型。
        /// </summary>
        public static readonly Type Hashtable = typeof(System.Collections.Hashtable);
        /// <summary>
        /// 表示 <see cref="System.Collections.Generic.HashSet&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GHashSet = typeof(System.Collections.Generic.HashSet<>);
        /// <summary>
        /// 表示 <see cref="System.Collections.Generic.Queue&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GQueue = typeof(System.Collections.Generic.Queue<>);
        /// <summary>
        /// 表示 <see cref="System.Collections.Generic.Stack&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GStack = typeof(System.Collections.Generic.Stack<>);
        /// <summary>
        /// 表示 <see cref="System.Collections.Generic.Dictionary&lt;TKey, TValue&gt;"/> 的类型。
        /// </summary>
        public static readonly Type HybridDictionary = typeof(System.Collections.Specialized.HybridDictionary);
        /// <summary>
        /// 表示 <see cref="System.Collections.Concurrent.ConcurrentDictionary&lt;TKey, TValue&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GConcurrentDictionary = typeof(System.Collections.Concurrent.ConcurrentDictionary<,>);
        /// <summary>
        /// 表示 <see cref="System.Collections.Generic.Dictionary&lt;TKey, TValue&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GDictionary = typeof(System.Collections.Generic.Dictionary<,>);
        /// <summary>
        /// 表示 <see cref="System.Collections.Generic.IDictionary&lt;TKey, TValue&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GIDictionary = typeof(System.Collections.Generic.IDictionary<,>);
        /// <summary>
        /// 表示 <see cref="System.Collections.Generic.IList&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GIList = typeof(System.Collections.Generic.IList<>);
        /// <summary>
        /// 表示 <see cref="System.Collections.Generic.List&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GList = typeof(System.Collections.Generic.List<>);
        /// <summary>
        /// 表示 <see cref="System.Collections.ICollection"/> 的类型。
        /// </summary>
        public static readonly Type ICollection = typeof(System.Collections.ICollection);
        /// <summary>
        /// 表示 <see cref="System.Collections.ObjectModel.Collection&lt;T&gt;"/> 的类型。
        /// </summary>
        public static readonly Type GCollection = typeof(System.Collections.ObjectModel.Collection<>);
        /// <summary>
        /// 表示 <see cref="System.Collections.IDictionary"/> 的类型。
        /// </summary>
        public static readonly Type IDictionary = typeof(System.Collections.IDictionary);
        /// <summary>
        /// 表示 <see cref="System.Collections.IList"/> 的类型。
        /// </summary>
        public static readonly Type IList = typeof(System.Collections.IList);

        #endregion

        #region 数据类型

        /// <summary>
        /// 表示 <see cref="System.Data.DataSet"/> 的类型。
        /// </summary>
        public static readonly Type DataSet = typeof(System.Data.DataSet);
        /// <summary>
        /// 表示 <see cref="System.Data.DataTable"/> 的类型。
        /// </summary>
        public static readonly Type DataTable = typeof(System.Data.DataTable);
        /// <summary>
        /// 表示 <see cref="System.Data.DataRow"/> 的类型。
        /// </summary>
        public static readonly Type DataRow = typeof(System.Data.DataRow);
        /// <summary>
        /// 表示 <see cref="System.Data.DataColumn"/> 的类型。
        /// </summary>
        public static readonly Type DataColumn = typeof(System.Data.DataColumn);

        #endregion

        #region 基础类型

        /// <summary>
        /// 表示 <see cref="System.Object"/> 的类型。
        /// </summary>
        public static readonly Type Object = typeof(System.Object);
        /// <summary>
        /// 表示 <see cref="System.Object"/> 数组的类型。
        /// </summary>
        public static readonly Type ObjectArray = typeof(System.Object[]);
        /// <summary>
        /// 表示 <see cref="System.Object"/> 的类型（ref）。
        /// </summary>
        public static readonly Type RefObject = Object.MakeByRefType();
        /// <summary>
        /// 表示 <see cref="System.String"/> 的类型。
        /// </summary>
        public static readonly Type String = typeof(System.String);
        /// <summary>
        /// 表示 <see cref="System.Text.StringBuilder"/> 的类型。
        /// </summary>
        public static readonly Type StringBuilder = typeof(System.Text.StringBuilder);
        /// <summary>
        /// 表示 <see cref="System.DBNull"/> 的类型。
        /// </summary>
        public static readonly Type DBNull = typeof(DBNull);
        /// <summary>
        /// 表示 <see cref="System.Byte"/> 数组的类型。
        /// </summary>
        public static readonly Type ByteArray = typeof(System.Byte[]);
        /// <summary>
        /// 表示 <see cref="System.Char"/> 数组数组的类型。
        /// </summary>
        public static readonly Type CharArray = typeof(System.Char[]);

        #region 值类型

        /// <summary>
        /// 表示 <see cref="System.Boolean"/> 的类型。
        /// </summary>
        public static readonly Type Boolean = typeof(System.Boolean);
        /// <summary>
        /// 表示 <see cref="System.Byte"/> 的类型。
        /// </summary>
        public static readonly Type Byte = typeof(System.Byte);
        /// <summary>
        /// 表示 <see cref="System.Char"/> 的类型。
        /// </summary>
        public static readonly Type Char = typeof(System.Char);
        /// <summary>
        /// 表示 <see cref="System.DateTime"/> 的类型。
        /// </summary>
        public static readonly Type DateTime = typeof(System.DateTime);
        /// <summary>
        /// 表示 <see cref="System.DateTimeOffset"/> 的类型。
        /// </summary>
        public static readonly Type DateTimeOffset = typeof(System.DateTimeOffset);
        /// <summary>
        /// 表示 <see cref="System.Decimal"/> 的类型。
        /// </summary>
        public static readonly Type Decimal = typeof(System.Decimal);
        /// <summary>
        /// 表示 <see cref="System.Double"/> 的类型。
        /// </summary>
        public static readonly Type Double = typeof(System.Double);
        /// <summary>
        /// 表示 <see cref="System.Guid"/> 的类型。
        /// </summary>
        public static readonly Type Guid = typeof(System.Guid);
        /// <summary>
        /// 表示 <see cref="System.Int16"/> 的类型。
        /// </summary>
        public static readonly Type Int16 = typeof(System.Int16);
        /// <summary>
        /// 表示 <see cref="System.Int32"/> 的类型。
        /// </summary>
        public static readonly Type Int32 = typeof(System.Int32);
        /// <summary>
        /// 表示 <see cref="System.Int64"/> 的类型。
        /// </summary>
        public static readonly Type Int64 = typeof(System.Int64);
        /// <summary>
        /// 表示 <see cref="System.SByte"/> 的类型。
        /// </summary>
        public static readonly Type SByte = typeof(System.SByte);
        /// <summary>
        /// 表示 <see cref="System.Single"/> 的类型。
        /// </summary>
        public static readonly Type Single = typeof(System.Single);
        /// <summary>
        /// 表示 <see cref="System.TimeSpan"/> 的类型。
        /// </summary>
        public static readonly Type TimeSpan = typeof(System.TimeSpan);
        /// <summary>
        /// 表示 <see cref="System.UInt16"/> 的类型。
        /// </summary>
        public static readonly Type UInt16 = typeof(System.UInt16);
        /// <summary>
        /// 表示 <see cref="System.UInt32"/> 的类型。
        /// </summary>
        public static readonly Type UInt32 = typeof(System.UInt32);
        /// <summary>
        /// 表示 <see cref="System.UInt64"/> 的类型。
        /// </summary>
        public static readonly Type UInt64 = typeof(System.UInt64);

        #endregion

        #endregion

        /// <summary>
        /// 表示 True 的字符串形式。
        /// </summary>
        public static readonly string[] TrueStrings = { "true", "是", "校验", "checked", "1", "yes", "selected", "ok" };
        /// <summary>
        /// 表示 Flase 的字符串形式。
        /// </summary>
        public static readonly string[] FlaseStrings = { "flase", "否", "非校验", "unchecked", "0", "no", "unselected", "cancel", string.Empty };

        /// <summary>
        /// 表示浮点数的数据类型集合。
        /// </summary>
        public static readonly Type[] NumberFloatTypes = { Single, Double, Decimal };

        /// <summary>
        /// 表示数字的数据类型集合。
        /// </summary>
        public static readonly Type[] NumberTypes = {SByte,Byte, UInt16, UInt32, UInt64
                                                     ,Int16, Int32, Int64
                                                     , Single,Double,Decimal };
    }
}
