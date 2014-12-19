
using System;
using Aoite.ReflectionTest.SampleModel.Animals.Attributes;

namespace Aoite.ReflectionTest.SampleModel.Animals.Enumerations
{
    [Flags]
    [Code("Temperature")]
    internal enum Climate
    {
        [Code("Hot")]
        Hot = 1,
        [Code("Cold")]
        Cold = 2,
        [Code("Any")]
        Any = Hot | Cold
    }
}
