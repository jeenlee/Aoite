using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System
{
    public class FastRandomTests
    {
        [Fact]
        public void GenerateNumberTest()
        {
            var min = 5; var max = 10;
            FastRandom fr = new FastRandom();
            for(int i = 0; i < 10000; i++)
            {
                var v = fr.Next(min, max);
                if(v < min || v >= max) Assert.Fail(v.ToString());
            }
        }

        [Fact]
        public void GenerateStringTest()
        {
            var min = 5; var max = 10;
            FastRandom fr = new FastRandom();
            for(int i = 0; i < 10000; i++)
            {
                var v = fr.NextString(min, max);
                if(v.Length < min || v.Length >= max) Assert.Fail(v.ToString());
            }
        }

        [Fact]
        public void GenerateStringTypeTests()
        {
            FastRandom fr = new FastRandom();
            CharacterType[] types = new CharacterType[] { CharacterType.LowerCase, CharacterType.UpperCase, CharacterType.Numeric };
            Func<char, bool>[] actions = new Func<char, bool>[] { Char.IsLower, Char.IsUpper, Char.IsNumber };
            for(int i = 0; i < types.Length; i++)
            {
                for(int j = 0; j < 1000; j++)
                {
                    var s = fr.NextString(5, types[i]);
                    Assert.Equal(5, s.Length);
                    foreach(var c in s)
                    {
                        Assert.True(actions[i](c));
                    }
                }
            }
            {
                var s = fr.NextString(5, CharacterType.Special);
                Assert.Equal(5, s.Length);
                foreach(var c in s)
                {
                    Assert.True(FastRandom.SpecialCharacters.Contains(c));
                }
            }
            
        }
    }
}
