
using System;

namespace Aoite.ReflectionTest.SampleModel.Animals.Interfaces
{
    internal interface ISwim
    {
        double SwimDistance { get; }

        void Move(double distance);
    }
}
