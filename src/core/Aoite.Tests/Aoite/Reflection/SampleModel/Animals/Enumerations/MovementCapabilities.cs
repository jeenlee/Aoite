
using System;

namespace Aoite.ReflectionTest.SampleModel.Animals.Enumerations
{
    [Flags]
    internal enum MovementCapabilities
    {
        Land = 1,
        Water = 2,
        Air = 4
    }
}
