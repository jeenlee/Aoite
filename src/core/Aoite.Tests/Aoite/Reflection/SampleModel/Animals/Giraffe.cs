
using Aoite.ReflectionTest.SampleModel.Animals.Attributes;
using Aoite.ReflectionTest.SampleModel.Animals.Enumerations;
using Aoite.ReflectionTest.SampleModel.Animals.Interfaces;

namespace Aoite.ReflectionTest.SampleModel.Animals
{
    [Zone(Zone.Savannah)]
    internal class Giraffe : Mammal, ISwim
    {
        public Giraffe(int id, Climate climateRequirements, MovementCapabilities movementCapabilities)
            : base(id, climateRequirements, movementCapabilities)
        {
        }

        public Giraffe(Climate climateRequirements, MovementCapabilities movementCapabilities)
            : base(climateRequirements, movementCapabilities)
        {
        }

        #region ISwim Members
        double ISwim.SwimDistance
        {
            get { throw new System.NotImplementedException(); }
        }

        void ISwim.Move(double distance)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
