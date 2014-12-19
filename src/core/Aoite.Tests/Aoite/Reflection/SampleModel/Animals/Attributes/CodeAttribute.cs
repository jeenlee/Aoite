
using System;

namespace Aoite.ReflectionTest.SampleModel.Animals.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    internal class CodeAttribute : Attribute
    {
        public string Code { get; set; }

        public CodeAttribute(string code)
        {
            Code = code;
        }
    }
}
