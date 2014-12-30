using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个实体的映射器。
    /// </summary>
    public interface IEntityMapper : IMapper
    {
        /// <summary>
        /// 获取或设置表的特性。
        /// </summary>
        TableAttribute Table { get; set; }
        /// <summary>
        /// 获取实体的类型。
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// 获取实体的属性映射集合。
        /// </summary>
        IEnumerable<PropertyMapper> Properties { get; }
        /// <summary>
        /// 指定属性名，判断指定的属性是否存在。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        bool ContainsProperty(string propertyName);
        /// <summary>
        /// 获取指定属性名称的属性映射。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        PropertyMapper this[string propertyName] { get; }
        /// <summary>
        /// 获取实体的属性映射集合的元素数。
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 将指定的实体填充到数据行。
        /// </summary>
        /// <param name="form">实例。</param>
        /// <param name="to">数据行。</param>
        void FillRow(object form, DataRow to);
        /// <summary>
        /// 将指定的数据行填充到实体。
        /// </summary>
        /// <param name="form">数据行。</param>
        /// <param name="to">实例。</param>
        void FillEntity(DataRow form, object to);
        /// <summary>
        /// 将指定的数据读取器填充到实体。
        /// </summary>
        /// <param name="form">数据读取器。</param>
        /// <param name="to">实例。</param>
        void FillEntity(IDataReader form, object to);
        /// <summary>
        /// 指定实体创建一个插入的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询。</returns>
        ExecuteCommand CreateInsertCommand(IDbEngine engine, object entity, string tableName = null);
        /// <summary>
        /// 指定实体创建一个更新的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个命令。</returns>
        ExecuteCommand CreateUpdateCommand(IDbEngine engine, object entity, string tableName = null);
        /// <summary>
        /// 指定实体创建一个删除的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entityOrPKValue">实体的实例对象（引用类型）或一个主键的值（值类型）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个命令。</returns>
        ExecuteCommand CreateDeleteCommand(IDbEngine engine, object entityOrPKValue, string tableName = null);
    }
}
