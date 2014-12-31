using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示内存中数据的一个分页表。
    /// </summary>

    [Serializable]
    [DefaultEvent("RowChanging")]
    [DefaultProperty("TableName")]
    [DesignTimeVisible(false)]
    [Editor("Microsoft.VSDesigner.Data.Design.DataTableEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [ToolboxItem(false)]
    [System.Xml.Serialization.XmlSchemaProvider("GetDataTableSchema")]
    public class AoiteTable : DataTable
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.AoiteTable"/> 类的新实例。
        /// </summary>
        public AoiteTable() { }

        /// <summary>
        /// 使用 <see cref="System.Runtime.Serialization.SerializationInfo"/> 和 <see cref="System.Runtime.Serialization.StreamingContext"/> 初始化 <see cref="Aoite.Data.AoiteTable"/> 类的新实例。
        /// </summary>
        /// <param name="info">将对象序列化或反序列化所需的数据。</param>
        /// <param name="context">给定序列化流的源和目的地。</param>
        protected AoiteTable(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            TotalRowCount = info.GetInt64("TotalRowCount");
        }

        /// <summary>
        /// 获取数据的总行数。
        /// </summary>
        public long TotalRowCount { get; internal set; }

        /// <summary>
        /// 用序列化 <see cref="Aoite.Data.AoiteTable"/> 所需的数据填充序列化信息对象。
        /// </summary>
        /// <param name="info">一个 <see cref="System.Runtime.Serialization.SerializationInfo"/> 对象，它包含与 <see cref="Aoite.Data.AoiteTable"/> 关联的序列化数据。</param>
        /// <param name="context">一个 <see cref="System.Runtime.Serialization.StreamingContext"/> 对象，它包含与 <see cref="Aoite.Data.AoiteTable"/> 关联的序列化流的源和目标。</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("TotalRowCount", TotalRowCount);
        }
    }
}
