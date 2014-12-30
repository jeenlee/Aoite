using System.Data;
using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 表示以自定义数据集对象作为返回值的查询结果。
    /// </summary>
    /// <typeparam name="TDataSet">数据集的类型。</typeparam>
    public class DataSetResult<TDataSet> : CanUpdateDbResultBase<TDataSet>
        where TDataSet : DataSet
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.DataSetResult&lt;TDataSet&gt;"/> 类的新实例。
        /// </summary>
        public DataSetResult() { }

        /// <summary>
        /// 获取一个值，表示值是否已被修改。
        /// </summary>
        public override bool IsChanged
        {
            get { return this._Value != null && this._Value.GetChanges() != null; }
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void OnDispose()
        {
            if(this._Value != null)
            {
                this._Value.Clear();
                this._Value.Dispose();
                this._Value = null;
            }
            base.OnDispose();
        }

        /// <summary>
        /// 更新结果的返回值。
        /// </summary>
        /// <param name="dataAdapter">查询结果对应的数据适配器。</param>
        /// <returns>返回更新的行数。如果返回 0 则表示当前返回值并改动，如果返回小于 0 的整数，则表示更新发生了异常。</returns>
        protected override int UpdateValue(DbDataAdapter dataAdapter)
        {
            var changes = this._Value.GetChanges();
            var i = 0;
            if(changes != null)
            {
                i = dataAdapter.Update(changes);
                this._Value.AcceptChanges();
            }
            return i;
        }
    }
}
