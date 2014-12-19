
using System;
using Aoite.ReflectionTest.SampleModel.Animals.Enumerations;

namespace Aoite.ReflectionTest.SampleModel.Animals.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class ZoneAttribute : Attribute
    {
        public Zone Zone { get; set; }

        public ZoneAttribute(Zone zone)
        {
            Zone = zone;
        }
    }
}
