using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    abstract class MapperBase<TTo> : IMapTo<TTo>, IMapTo
        where TTo : class
    {
        public TypeMapper Mapper;
        public TTo ToValue;

        public void To(TTo to)
        {
            if(to == null) throw new ArgumentNullException("to");
            this.ToValue = to;
            this.Fill();
        }

        TEntity IMapTo.To<TEntity>(TEntity to)
        {
            To(to as TTo);
            return to;
        }

        protected abstract void Fill();

        protected void WarningNotFound(string fullName, string fieldName)
        {
            GA.TraceWarning("填充数据时候发生错误，无法在 {0} 找到字段 {1} 对应的属性成员。", fullName, fieldName);
        }

        protected void SetPropertyValue(string name, Type valueType, object value)
        {
            var pMapper = this.Mapper[name];

            if(pMapper == null)
            {
                WarningNotFound(ToValue.GetType().FullName, name);
                return;
            }
            var property = pMapper.Property;
            var propertyType = property.PropertyType;

            if(Convert.IsDBNull(value)) value = pMapper.TypeDefaultValue;
            else if(valueType != propertyType)
            {
                try
                {
                    value = propertyType.ChangeType(value);
                }
                catch(Exception ex)
                {
                    throw new InvalidCastException("列 {0} 无法将值类型 {1} 转换为类型 {2}。".Fmt(name, valueType.FullName, propertyType.FullName), ex);
                }
            }

            pMapper.SetValue(ToValue, value);
        }
    }

}
