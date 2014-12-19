
using Aoite.ReflectionTest.SampleModel.Animals.Enumerations;

namespace Aoite.ReflectionTest.SampleModel.Animals
{
    internal abstract class Mammal : Animal
    {
        protected Mammal(Climate climateRequirements, MovementCapabilities movementCapabilities)
            : base(climateRequirements, movementCapabilities)
        {
        }

        protected Mammal(int id, Climate climateRequirements, MovementCapabilities movementCapabilities)
            : base(id, climateRequirements, movementCapabilities)
        {
        }
    }
}
