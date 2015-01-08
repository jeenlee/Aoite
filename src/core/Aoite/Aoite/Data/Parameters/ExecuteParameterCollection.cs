using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据源交互的执行命令参数集合。
    /// </summary>
    [Serializable]
    public class ExecuteParameterCollection : ICollection<ExecuteParameter>
    {
        private readonly static Type ExecuteParameterType = typeof(ExecuteParameter);
        private readonly Dictionary<string, ExecuteParameter> _innerDict;

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        public ExecuteParameterCollection() : this(4) { }

        /// <summary>
        /// 指定初始容量初始化一个 <see cref="Aoite.Data.ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="capacity">集合可包含的初始元素数。</param>
        public ExecuteParameterCollection(int capacity)
        {
            this._innerDict = new Dictionary<string, ExecuteParameter>(capacity, StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public ExecuteParameterCollection(object objectInstance)
            : this()
        {
            this.AddObject(objectInstance);
        }

        /// <summary>
        /// 指定参数集合初始化一个 <see cref="Aoite.Data.ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="keysAndValues">应当是 <see cref="System.String"/> / <see cref="System.Object"/> 的字典集合。</param>
        public ExecuteParameterCollection(params object[] keysAndValues)
            : this(keysAndValues == null ? 0 : keysAndValues.Length / 2)
        {
            if(keysAndValues == null) return;

            if(keysAndValues.Length % 2 != 0) throw new ArgumentException("参数长度无效！长度必须为 2 的倍数。", "keysAndValues");

            for(int i = 0; i < keysAndValues.Length; )
            {
                this.Add(keysAndValues[i++].ToString(), keysAndValues[i++]);
            }
        }

        /// <summary>
        /// 指定参数数组初始化一个 <see cref="Aoite.Data.ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="source">参数数组。</param>
        public ExecuteParameterCollection(params ExecuteParameter[] source)
            : this(source == null ? 0 : source.Length)
        {
            foreach(var item in source)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// 指定原参数集合初始化一个 <see cref="Aoite.Data.ExecuteParameterCollection"/> 类的新实例。
        /// </summary>
        /// <param name="source">原参数集合。</param>
        public ExecuteParameterCollection(ExecuteParameterCollection source)
            : this(source == null ? 0 : source.Count)
        {
            foreach(var item in source)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// 获取或设置指定参数名称的参数内容。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>获取一个 <see cref="Aoite.Data.ExecuteParameter"/> 的参数实例。</returns>
        public ExecuteParameter this[string name]
        {
            get
            {
                ExecuteParameter value;
                this._innerDict.TryGetValue(name, out value);
                return value;
            }
            set
            {
                this._innerDict[name] = value;
            }
        }

        /// <summary>
        /// 获取指定参数索引的参数内容。
        /// </summary>
        /// <param name="index">参数索引。</param>
        /// <returns>获取一个 <see cref="Aoite.Data.ExecuteParameter"/> 的参数实例。</returns>
        public ExecuteParameter this[int index]
        {
            get
            {
                return this._innerDict.Values.ElementAtOrDefault(index);
            }
        }

        /// <summary>
        /// 指定解析一个任意对象，添加到集合中。
        /// </summary>
        /// <param name="objectInstance">一个任意对象。</param>
        public void AddObject(object objectInstance)
        {
            if(objectInstance == null) return;

            var objType = objectInstance.GetType();
            if(objType == ExecuteParameterType)
            {
                this.Add(objectInstance as ExecuteParameter);
                return;
            }

            var mapper = TypeMapper.Create(objType);
            object value;
            foreach(var prop in mapper.Properties)
            {
                if(prop.IsIgnore) continue;

                value = prop.GetValue(objectInstance);
                if(prop.Property.PropertyType == ExecuteParameterType) this.Add(value as ExecuteParameter);
                else this.Add(prop.Name, value);
            }
        }

        void ICollection<ExecuteParameter>.Add(ExecuteParameter parameter) { this.Add(parameter); }

        /// <summary>
        /// 指定一个 <see cref="Aoite.Data.ExecuteParameter"/> 实例，添加到集合中。
        /// </summary>
        /// <param name="parameter">要添加的 <see cref="Aoite.Data.ExecuteParameter"/> 实例。</param>
        public ExecuteParameterCollection Add(ExecuteParameter parameter)
        {
            if(parameter == null) throw new ArgumentNullException("parameter");
            var name = parameter.Name;
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("parameter.Name");
            if(this._innerDict.ContainsKey(name)) throw new ArgumentException("已存在此名称 " + name + " 的参数！", "parameter.Name");
            parameter.Owner = this;
            this._innerDict.Add(name, parameter);
            return this;
        }

        /// <summary>
        /// 指定原 <see cref="System.Data.Common.DbParameter"/> 实例，添加到集合中。
        /// </summary>
        /// <param name="sourceParameter">原 <see cref="System.Data.Common.DbParameter"/> 实例。可以为 null，表示未确定数据。</param>
        public ExecuteParameterCollection Add(DbParameter sourceParameter)
        {
            this.Add(new ExecuteDbParameter(sourceParameter));
            return this;
        }

        /// <summary>
        /// 尝试获取指定名称的值。
        /// </summary>
        /// <param name="name">指定的名称。</param>
        /// <param name="value">获取的值。</param>
        /// <returns>获取成功将返回 true，否则返回 false。</returns>
        public bool TryGetValue(string name, out object value)
        {
            var p = this[name];
            if(p == null)
            {
                value = null;
                return false;
            }
            value = p.Value;
            return true;
        }

        internal void ChangedName(string oldName, string newName)
        {
            ExecuteParameter p;
            if(!this._innerDict.TryGetValue(oldName, out p)) throw new Exception("没在当前参数集合找到参数 " + oldName);
            if(this._innerDict.ContainsKey(newName)) throw new Exception("已存在同名参数 " + newName);

            this._innerDict.Remove(oldName);
            this._innerDict.Add(newName, p);
        }

        /// <summary>
        /// 指定输出的参数名，添加到集合中。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        public ExecuteParameterCollection Add(string name)
        {
            return this.Add(new ExecuteParameter(name));
        }

        /// <summary>
        /// 指定输出参数的名称和参数的类型，添加到集合中。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        public ExecuteParameterCollection Add(string name, DbType type)
        {
            return this.Add(new ExecuteParameter(name, type));
        }

        /// <summary>
        /// 指定参数名和参数值，添加到集合中。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        public ExecuteParameterCollection Add(string name, object value)
        {
            return this.Add(new ExecuteParameter(name, value));
        }

        /// <summary>
        /// 指定内容生成项，并添加到集合中。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="direction">指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        public ExecuteParameterCollection Add(string name, object value, ParameterDirection direction)
        {
            return this.Add(new ExecuteParameter(name, value, direction));
        }

        /// <summary>
        /// 指定内容生成项，并添加到集合中。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        /// <param name="direction">指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        public ExecuteParameterCollection Add(string name, object value, DbType type, ParameterDirection direction)
        {
            return this.Add(new ExecuteParameter(name, value, type, direction));
        }

        /// <summary>
        /// 指定内容生成项，并添加到集合中。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        public ExecuteParameterCollection Add(string name, object value, DbType type)
        {
            return this.Add(new ExecuteParameter(name, value, type));
        }

        /// <summary>
        /// 指定内容生成项，并添加到集合中。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        /// <param name="size">参数的长度。</param>
        public ExecuteParameterCollection Add(string name, object value, DbType type, int size)
        {
            return this.Add(new ExecuteParameter(name, value, type, size));
        }

        /// <summary>
        /// 指定内容生成项，并添加到集合中。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        /// <param name="size">参数的长度。</param>
        /// <param name="direction">指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        public ExecuteParameterCollection Add(string name, object value, DbType type, int size, ParameterDirection direction)
        {
            return this.Add(new ExecuteParameter(name, value, type, size, direction));
        }

        /// <summary>
        /// 移除指定参数名的 <see cref="Aoite.Data.ExecuteParameter"/> 项。
        /// </summary>
        /// <param name="name">参数名。</param>
        /// <returns>如果已从集合中成功移除项，则为 true；否则为 false。如果在集合中没有找到项，该方法也会返回 false。</returns>
        public bool Remove(string name)
        {
            ExecuteParameter p;
            if(this._innerDict.TryGetValue(name, out p))
            {
                p.Owner = null;
                return this._innerDict.Remove(name);
            }
            return false;
        }

        /// <summary>
        /// 移除指定的 <see cref="Aoite.Data.ExecuteParameter"/> 项。
        /// </summary>
        /// <param name="parameter">要移除的 <see cref="Aoite.Data.ExecuteParameter"/>。</param>
        /// <returns>如果已从集合中成功移除项，则为 true；否则为 false。如果在集合中没有找到项，该方法也会返回 false。</returns>
        public bool Remove(ExecuteParameter parameter)
        {
            if(parameter == null) throw new ArgumentNullException("parameter");
            var name = parameter.Name;
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("parameter.Name");

            return this.Remove(name);
        }

        /// <summary>
        /// 从集合中移除所有项。
        /// </summary>
        public void Clear()
        {
            foreach(var item in this._innerDict.Values)
            {
                item.Owner = null;
            }
            this._innerDict.Clear();
        }

        /// <summary>
        /// 确定集合是否包含特定的参数名。
        /// </summary>
        /// <param name="name">参数名。</param>
        /// <returns>如果在集合中找到项，则为 true；否则为 false。</returns>
        public bool Contains(string name)
        {
            if(name == null) throw new ArgumentNullException("name");
            return this._innerDict.ContainsKey(name);
        }

        /// <summary>
        /// 确定集合是否包含特定的参数 <see cref="Aoite.Data.ExecuteParameter"/>。
        /// </summary>
        /// <param name="parameter">要查找的 <see cref="Aoite.Data.ExecuteParameter"/>。</param>
        /// <returns>如果在集合中找到项，则为 true；否则为 false。</returns>
        public bool Contains(ExecuteParameter parameter)
        {
            if(parameter == null) throw new ArgumentNullException("parameter");
            return this.Contains(parameter.Name);
        }

        /// <summary>
        /// 从特定的 <see cref="System.Array"/> 索引开始，将集合的元素复制到一个 <see cref="System.Array"/> 中。
        /// </summary>
        /// <param name="parameters">作为从集合复制的元素的目标位置的一维 <see cref="System.Array"/>。<see cref="System.Array"/> 必须具有从零开始的索引。</param>
        /// <param name="arrayIndex"><paramref name="parameters"/> 中从零开始的索引，从此处开始复制。</param>
        public void CopyTo(ExecuteParameter[] parameters, int arrayIndex)
        {
            this._innerDict.Values.CopyTo(parameters, arrayIndex);
        }

        /// <summary>
        /// 获取集合中包含的元素数。
        /// </summary>
        public int Count { get { return this._innerDict.Count; } }

        bool ICollection<ExecuteParameter>.IsReadOnly { get { return false; } }

        IEnumerator<ExecuteParameter> IEnumerable<ExecuteParameter>.GetEnumerator()
        {
            return this._innerDict.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._innerDict.Values.GetEnumerator();
        }

    }
}
