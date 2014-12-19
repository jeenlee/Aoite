
using Aoite.ReflectionTest.SampleModel.Animals.Enumerations;
using Aoite.ReflectionTest.SampleModel.Animals.Interfaces;

namespace Aoite.ReflectionTest.SampleModel.Animals
{
    internal abstract class Reptile : Animal, ISlide
    {
        protected Reptile(Climate climateRequirements, MovementCapabilities movementCapabilities)
            : base(climateRequirements, movementCapabilities)
        {
        }

        protected Reptile(int id, Climate climateRequirements, MovementCapabilities movementCapabilities)
            : base(id, climateRequirements, movementCapabilities)
        {
        }

        public virtual double SlideDistance { get; protected set; }

        public abstract void Move(double distance);
    }
}
