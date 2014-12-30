using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个包含 <see cref="System.Data.Common.DbParameter"/> 实例的执行命令参数。
    /// </summary>
    public class ExecuteDbParameter : ExecuteParameter
    {
        private DbParameter _SourceParameter;
        /// <summary>
        /// 获取或设置原 <see cref="System.Data.Common.DbParameter"/> 实例。
        /// </summary>
        public DbParameter SourceParameter { get { return this._SourceParameter; } set { this._SourceParameter = value; } }

        #region Properties

        /// <summary>
        /// 获取或设置参数的 <see cref="System.Data.DbType"/>。
        /// </summary>
        public override DbType Type { get { return this._SourceParameter.DbType; } set { this._SourceParameter.DbType = value; } }

        /// <summary>
        /// 获取或设置参数的长度。
        /// <para>当值小于零时，默认值是从参数值推导出的。</para>
        /// </summary>
        public override int Size { get { return this._SourceParameter.Size; } set { this._SourceParameter.Size = value; } }

        /// <summary>
        /// 获取或设置一个值，该值指示参数是只可输入、只可输出、双向还是存储过程返回值参数。
        /// </summary>
        public override ParameterDirection Direction { get { return this._SourceParameter.Direction; } set { this._SourceParameter.Direction = value; } }

        /// <summary>
        /// 获取或设置参数的名称。
        /// </summary>
        public override string Name
        {
            get
            {
                return this._SourceParameter.ParameterName;
            }
            set
            {
                if(string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                if(this._SourceParameter.ParameterName.iEquals(value)) return;
                if(this.Owner != null) this.Owner.ChangedName(this._SourceParameter.ParameterName, value);
                this._SourceParameter.ParameterName = value;
            }
        }

        /// <summary>
        /// 获取或设置参数的值。
        /// </summary>
        public override object Value { get { return this._SourceParameter.Value; } set { this._SourceParameter.Value = value ?? DBNull.Value; } }

        #endregion

        /// <summary>
        /// 指定原 <see cref="System.Data.Common.DbParameter"/> 实例，初始化一个 <see cref="Aoite.Data.ExecuteDbParameter"/> 类的新实例。
        /// </summary>
        /// <param name="sourceParameter">原 <see cref="System.Data.Common.DbParameter"/> 实例。</param>
        public ExecuteDbParameter(DbParameter sourceParameter)
        {
            if(sourceParameter == null) throw new ArgumentNullException("sourceParameter");
            this._SourceParameter = sourceParameter;
        }

        /// <summary>
        /// 指定 <see cref="System.Data.Common.DbCommand"/>，生成一个 <see cref="System.Data.Common.DbParameter"/>。
        /// </summary>
        /// <param name="command">一个 <see cref="System.Data.Common.DbCommand"/>。</param>
        /// <returns>返回一个已生成的 <see cref="System.Data.Common.DbParameter"/>。</returns>
        public override DbParameter CreateParameter(DbCommand command)
        {
            return this._SourceParameter;
        }

        /// <summary>
        /// 创建作为当前实例副本的新对象。
        /// </summary>
        /// <returns>作为此实例副本的新对象。</returns>
        public override object Clone()
        {
            return new ExecuteDbParameter((this._SourceParameter as ICloneable).Clone() as DbParameter);
        }
    }
}
