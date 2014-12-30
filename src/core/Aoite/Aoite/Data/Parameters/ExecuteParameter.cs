using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据源交互的执行命令参数。
    /// </summary>
    [Serializable]
    public class ExecuteParameter : ICloneable
    {
        #region Properties

        private DbType? _Type;
        /// <summary>
        /// 获取或设置参数的 <see cref="System.Data.DbType"/>。
        /// <para>默认值为 <see cref="System.Data.DbType.String"/>。</para>
        /// </summary>
        public virtual DbType Type { get { return this._Type ?? DbType.String; } set { this._Type = value; } }

        private int _Size = -1;
        /// <summary>
        /// 获取或设置参数的长度。
        /// <para>当值小于零时，默认值是从参数值推导出的。</para>
        /// </summary>
        public virtual int Size { get { return this._Size; } set { this._Size = value; } }

        private ParameterDirection _Direction = ParameterDirection.Input;
        /// <summary>
        /// 获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。
        /// <para>默认值为 <see cref="ParameterDirection.Input"/>。</para>
        /// </summary>
        public virtual ParameterDirection Direction { get { return this._Direction; } set { this._Direction = value; } }

        private string _Name;
        /// <summary>
        /// 获取或设置参数的名称。
        /// </summary>
        public virtual string Name
        {
            get { return this._Name; }
            set
            {
                if(string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                if(this._Name.iEquals(value)) return;
                if(this._Owner != null) this._Owner.ChangedName(this._Name, value);
                this._Name = value;
            }
        }

        private object _Value;
        /// <summary>
        /// 获取或设置参数的值。
        /// </summary>
        public virtual object Value
        {
            get { return Convert.IsDBNull(this._Value) ? null : this._Value; }
            set { this._Value = value ?? DBNull.Value; }
        }

        [NonSerialized]
        private ExecuteParameterCollection _Owner;
        /// <summary>
        /// 获取参数所在的集合列表。
        /// </summary>
        public ExecuteParameterCollection Owner
        {
            get
            {
                return this._Owner;
            }
            internal set
            {
                this._Owner = value;
            }
        }

        #endregion

        #region Ctors

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        public ExecuteParameter() { }

        /// <summary>
        /// 指定输出参数的名称，初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        public ExecuteParameter(string name)
            : this(name, null, null, -1, ParameterDirection.Output) { }

        /// <summary>
        /// 指定输出参数的名称和参数的类型，初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        public ExecuteParameter(string name, DbType type)
            : this(name, null, type, -1, ParameterDirection.Output) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        public ExecuteParameter(string name, object value)
            : this(name, value, null, -1, ParameterDirection.Input) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="direction">指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        public ExecuteParameter(string name, ParameterDirection direction)
            : this(name, DBNull.Value, null, -1, direction) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="direction">指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        public ExecuteParameter(string name, ParameterDirection direction, DbType type)
            : this(name, DBNull.Value, type, -1, direction) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="direction">指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        /// <param name="size">参数的长度。</param>
        public ExecuteParameter(string name, ParameterDirection direction, DbType type, int size)
            : this(name, DBNull.Value, type, size, direction) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="direction">指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        public ExecuteParameter(string name, object value, ParameterDirection direction)
            : this(name, value, null, -1, direction) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        public ExecuteParameter(string name, object value, DbType type)
            : this(name, value, type, -1, ParameterDirection.Input) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        /// <param name="size">参数的长度。</param>
        public ExecuteParameter(string name, object value, DbType type, int size)
            : this(name, value, type, size, ParameterDirection.Input) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        /// <param name="direction">指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        public ExecuteParameter(string name, object value, DbType type, ParameterDirection direction)
            : this(name, value, (DbType?)type, -1, direction) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        /// <param name="size">参数的长度。</param>
        /// <param name="direction">指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        public ExecuteParameter(string name, object value, DbType type, int size, ParameterDirection direction)
            : this(name, value, (DbType?)type, size, direction) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteParameter"/> 类的新实例。
        /// </summary>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <param name="type">参数的 <see cref="System.Data.DbType"/>。</param>
        /// <param name="size">参数的长度。</param>
        /// <param name="direction">指示参数是只可输入、只可输出、双向还是存储过程返回值参数。</param>
        private ExecuteParameter(string name, object value, DbType? type, int size, ParameterDirection direction)
        {
            this.Name = name;
            this.Value = value;
            this._Type = type;
            this._Size = size;
            this._Direction = direction;
        }

        #endregion

        /// <summary>
        /// 指定 <see cref="System.Data.Common.DbCommand"/>，生成一个 <see cref="System.Data.Common.DbParameter"/>。
        /// </summary>
        /// <param name="command">一个 <see cref="System.Data.Common.DbCommand"/>。</param>
        /// <returns>返回一个已生成的 <see cref="System.Data.Common.DbParameter"/>。</returns>
        public virtual DbParameter CreateParameter(DbCommand command)
        {
            if(command == null) throw new ArgumentNullException("command");
            var adorner = this._Value as IParameterAdorner;
            if(adorner != null) return adorner.Render(command, this);

            var parameter = command.CreateParameter();
            parameter.ParameterName = this._Name;
            parameter.Value = this._Value;
            if(this._Type.HasValue) parameter.DbType = this._Type.Value;
            if(this._Direction != ParameterDirection.Input) parameter.Direction = this._Direction;
            if(this._Size > -1) parameter.Size = this._Size;
            return parameter;
        }

        /// <summary>
        /// 创建作为当前实例副本的新对象。
        /// </summary>
        /// <returns>作为此实例副本的新对象。</returns>
        public virtual object Clone()
        {
            return new ExecuteParameter(this._Name, this._Value, this._Type, this._Size, this._Direction);
        }
    }

}
