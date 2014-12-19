
using System;
using System.ComponentModel;
using Aoite.ReflectionTest.SampleModel.Animals.Enumerations;
using Aoite.ReflectionTest.SampleModel.Animals.Interfaces;

namespace Aoite.ReflectionTest.SampleModel.Animals
{
    internal class Snake : Reptile, ISwim, IBite
    {
        public Snake()
            : base(Climate.Hot, MovementCapabilities.Land)
        {
            HasDeadlyBite = true;
        }

        // regular member
        public bool HasDeadlyBite { get; private set; }

        // ISwim
        void ISwim.Move(double distance)
        {
            SwimDistance += distance;
        }
        public virtual double SwimDistance { get; private set; }

        // ISlide
        public override void Move([DefaultValue(100d)] double distance)
        {
            SlideDistance += distance;
        }
        public override double SlideDistance { get; protected set; }

        // IBite
        public bool Bite(Animal animal)
        {
            return HasDeadlyBite;
        }
    }
}
