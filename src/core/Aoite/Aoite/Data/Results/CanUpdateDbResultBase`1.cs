using System;
using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个可更新的查询结果。
    /// </summary>
    /// <typeparam name="TValue">返回值的类型。</typeparam>
    public abstract class CanUpdateDbResultBase<TValue> : DbResult<TValue>, ICanUpdateDbResult
    {
        #region Constructors

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.CanUpdateDbResultBase&lt;TValue&gt;"/> 类的新实例。
        /// </summary>
        internal CanUpdateDbResultBase() { }

        #endregion Constructors

        /// <summary>
        /// 获取一个值，表示值是否已被修改。
        /// </summary>
        public abstract bool IsChanged { get; }

        /// <summary>
        /// 提交当前返回值，更新数据源对应的记录。
        /// <para>如果在更新过程中发生了异常，将会通知当前查询结果的 <see cref="System.IResult.Exception"/> 属性。</para>
        /// </summary>  
        /// <returns>返回更新的行数。如果返回一个 0 则表示当前返回值并改动，如果返回一个小于 0 的整数，则表示更新发生了异常。</returns>
        public int Update()
        {
            return this.Update(false);
        }

        /// <summary>
        /// 提交当前返回值，更新数据源对应的记录。
        /// <para>如果在更新过程中发生了异常，将会通知当前查询结果的 <see cref="System.IResult.Exception"/> 属性。</para>
        /// </summary>  
        /// <param name="continueUpdateOnError">指示在行更新过程中遇到错误时是否生成异常。如果要继续更新而不生成异常，则为 true，否则为 false。</param>
        /// <returns>返回更新的行数。如果返回一个 0 则表示当前返回值并改动，如果返回一个小于 0 的整数，则表示更新发生了异常。</returns>
        public int Update(bool continueUpdateOnError)
        {
            this.ToSuccessed();
            try
            {
                var factory = this.Engine.Owner.Injector.Factory;
                using(var dataAdapter = factory.CreateDataAdapter())
                {
                    var command = this.Command;
                    var cloneObject = (command as ICloneable);
                    if(cloneObject != null) command = cloneObject.Clone() as DbCommand;

                    dataAdapter.SelectCommand = command;
                    dataAdapter.ContinueUpdateOnError = continueUpdateOnError;
                    using(var cb = factory.CreateCommandBuilder())
                    {
                        cb.DataAdapter = dataAdapter;
                        return this.UpdateValue(dataAdapter);
                    }
                }
            }
            catch(System.Data.DBConcurrencyException)
            {
                this.ToFailded("更新失败，当前数据已被其他用户修改！");
            }
            catch(Exception ex)
            {
                this.ToFailded(ex);
            }
            return -1;
        }

        /// <summary>
        /// 更新结果的返回值。
        /// </summary>
        /// <param name="dataAdapter">查询结果对应的数据适配器。</param>
        /// <returns>返回更新的行数。如果返回一个 0 则表示当前返回值并改动，如果返回一个小于 0 的整数，则表示更新发生了异常。</returns>
        protected abstract int UpdateValue(DbDataAdapter dataAdapter);
    }
}
