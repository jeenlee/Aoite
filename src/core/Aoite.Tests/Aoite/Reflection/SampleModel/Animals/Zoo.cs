
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Aoite.ReflectionTest.SampleModel.Animals
{
    internal sealed class Zoo : Collection<Animal>
    {
        private const int FirstId = 1000;
        private static int nextId = FirstId;
        private readonly string name;
        private string alias;

        public Zoo(string name)
        {
            this.name = name;
            alias = name;
        }

        public IEnumerable<T> Animals<T>()
        {
            return this.Where(a => a is T).Cast<T>();
        }

        public void RegisterClass(string @class, string _section, string __name, int size)
        {
        }

        public string Name
        {
            get { return name; }
        }
        public string Alias
        {
            set { alias = value; }
        }

        public static int NextId
        {
            get { return ++nextId; }
        }
    }
}
