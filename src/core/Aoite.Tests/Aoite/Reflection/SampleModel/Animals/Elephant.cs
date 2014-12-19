
using System;
using Aoite.ReflectionTest.SampleModel.Animals.Enumerations;

namespace Aoite.ReflectionTest.SampleModel.Animals
{
    internal class Elephant : Mammal
    {
#pragma warning disable 0169, 0649
        public int MethodInvoked { get; private set; }
#pragma warning restore 0169, 0649

        #region Constructors
        public Elephant()
            : base(Climate.Hot, MovementCapabilities.Land)
        {
        }
        #endregion

        #region Methods
        public void Eat()
        {
            MethodInvoked = 1;
        }
        public void Eat(string food)
        {
            MethodInvoked = 2;
        }
        public void Eat(int count)
        {
            MethodInvoked = 3;
        }
        public void Eat(int count, string food)
        {
            MethodInvoked = 4;
        }
        public void Eat(double count, string food, bool isHay)
        {
            MethodInvoked = 5;
        }

        public void Roar(int count)
        {
            MethodInvoked = 10;
        }
        public void Roar(int count, int volume)
        {
            MethodInvoked = 11;
        }
        public void Accept(char c)
        {
            MethodInvoked = 12;
        }
        public void AcceptParams(params string[] args)
        {
            MethodInvoked = 13;
        }
        #endregion
    }
}
