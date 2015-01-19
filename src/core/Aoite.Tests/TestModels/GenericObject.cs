using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestModels
{
    public class GenericObject<T>
    {
        public GenericObject()
        {
        }

        public GenericObject(T data)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}
