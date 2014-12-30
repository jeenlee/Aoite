using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示实例创建的委托。
    /// </summary>
    /// <param name="lastMappingArguments">后期绑定的参数列表。</param>
    /// <returns>返回一个实例。</returns>
    public delegate object InstanceCreatorCallback(object[] lastMappingArguments);
    /// <summary>
    /// 通过 <see cref="System.Object&lt;T&gt;"/> 获取实例时，动态设置后期映射的参数值数组。
    /// </summary>
    /// <param name="type">当前依赖注入与控制反转的数据类型。</param>
    /// <returns>返回后期映射的参数值数组。</returns>
    public delegate object[] LastMappingHandler(Type type);
    /// <summary>
    /// 表示映射解析的事件委托。
    /// </summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">事件参数。</param>
    public delegate void MapResolveEventHandler(object sender, MapResolveEventArgs e);
}
