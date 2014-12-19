
using System;
using System.ComponentModel;
using Aoite.Reflection;
using Aoite.ReflectionTest.SampleModel.Animals.Attributes;
using Aoite.ReflectionTest.SampleModel.Animals.Enumerations;

namespace Aoite.ReflectionTest.SampleModel.Animals
{
    [Zone(Zone.Savannah)]
    [Serializable]
    internal class Lion : Mammal
    {
#pragma warning disable 0169, 0649
        [Code("Field")]
        private DateTime lastMealTime = DateTime.MinValue;

        [Code("ReadWrite Property")]
        [DefaultValue("Simba")]
        public string Name { get; private set; }

        [Code("ReadOnly Property")]
        public bool IsHungry { get { return DateTime.Now.AddHours(-12) > lastMealTime; } }

        public int ConstructorInstanceUsed { get; private set; }
#pragma warning restore 0169, 0649

        #region Constructors
        public Lion()
            : this(typeof(Lion).Property("Name").Attribute<DefaultValueAttribute>().Value.ToString())
        {
            ConstructorInstanceUsed = 1;
        }

        [Code("A")]
        public Lion(string name)
            : base(Climate.Hot, MovementCapabilities.Land)
        {
            Name = name;
            ConstructorInstanceUsed = 2;
        }

        public Lion(int id)
            : this(id, typeof(Lion).Property("Name").Attribute<DefaultValueAttribute>().Value.ToString())
        {
            ConstructorInstanceUsed = 3;
        }

        public Lion(int id, string name)
            : base(id, Climate.Hot, MovementCapabilities.Land)
        {
            Name = name;
            ConstructorInstanceUsed = 4;
        }
        #endregion
    }
}
