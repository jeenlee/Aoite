using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Ioc
{
    public interface IService1 { }
    public interface IService2 { }
    public interface IService_NotFound { }
    class LastMapperTestModel
    {
        public int BaseValue1 { get; set; }
        public int BaseValue2 { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public LastMapperTestModel(int baseValue1, [LastMapping] int value2, int baseValue2, [LastMapping] int value1)
        {
            this.BaseValue1 = baseValue1;
            this.BaseValue2 = baseValue2;
            this.Value1 = value1;
            this.Value2 = value2;
        }
    }
    public class DefaultService1 : IService1 { }
    public class DefaultService2 : IService2 { }
    public class XService2 : IService2 { }

    public interface IValueService
    {
        int Value1 { get; set; }
        string Value2 { get; set; }
        bool Value3 { get; set; }
    }
    public class ValueService1 : IValueService
    {
        public int Value1 { get; set; }
        public string Value2 { get; set; }
        public bool Value3 { get; set; }

        public ValueService1(int value1, string value2, bool value3)
        {
            this.Value1 = value1;
            this.Value2 = value2;
            this.Value3 = value3;
        }
    }
    public class ValueService2 : IValueService
    {
        public int Value1 { get; set; }
        public string Value2 { get; set; }
        public bool Value3 { get; set; }

        public ValueService2(int value1, string value2, bool value3)
        {
            this.Value1 = value1;
            this.Value2 = value2;
            this.Value3 = value3;
        }
    }


    [DefaultMapping(typeof(DefaultMappingService2))]
    public interface IDefaultMappingService { }
    public class DefaultMappingService : IDefaultMappingService { }
    public class DefaultMappingService2 : IDefaultMappingService { }
}
