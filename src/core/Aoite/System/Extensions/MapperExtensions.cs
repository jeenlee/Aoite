using Aoite.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个映射器的扩展方法。
    /// </summary>
    public static class MapperExtensions
    {
        /// <summary>
        /// 将指定的数据行填充到实体。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="row">数据行。</param>
        /// <returns>返回一个映射目标的新实例。</returns>
        public static IMapTo From(this TypeMapper mapper, DataRow row)
        {
            if(mapper == null) throw new ArgumentNullException("mapper");
            if(row == null) throw new ArgumentNullException("row");

            return new DataRowToObjectMapper()
            {
                Mapper = mapper,
                FromValue = row
            };
        }
        /// <summary>
        /// 将指定的数据读取器填充到实体。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="reader">数据读取器。</param>
        /// <returns>返回一个映射目标的新实例。</returns>
        public static IMapTo From(this TypeMapper mapper, IDataReader reader)
        {
            if(mapper == null) throw new ArgumentNullException("mapper");
            if(reader == null) throw new ArgumentNullException("reader");

            return new DataReaderToObjectMapper()
            {
                Mapper = mapper,
                FromValue = reader
            };
        }
        /// <summary>
        /// 将指定的实体填充到数据行。
        /// </summary>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体。</param>
        /// <returns>返回一个映射目标的新实例。</returns>
        public static IMapTo<DataRow> From(this TypeMapper mapper, object entity)
        {
            if(mapper == null) throw new ArgumentNullException("mapper");
            if(entity == null) throw new ArgumentNullException("entity");

            return new ObjectToRowMapper()
            {
                Mapper = mapper,
                FromValue = entity
            };
        }
    }
}
