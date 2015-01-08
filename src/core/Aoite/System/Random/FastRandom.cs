using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个快速的随机数实现。
    /// </summary>
    public sealed class FastRandom
    {
        // The +1 ensures NextDouble doesn't generate 1.0
        const double REAL_UNIT_INT = 1.0 / ((double)int.MaxValue + 1.0);
        const double REAL_UNIT_UINT = 1.0 / ((double)uint.MaxValue + 1.0);
        const uint Y = 842502087, Z = 3579807591, W = 273326509;

        /// <summary>
        /// 获取随机数的唯一实例。
        /// </summary>
        public readonly static FastRandom Instance = new FastRandom();
        uint x, y, z, w;

        #region Constructors

        /// <summary>
        /// 使用与时间相关的默认种子值，初始化 <see cref="System.FastRandom"/> 类的新实例。
        /// </summary>
        public FastRandom()
        {
            Reinitialise(Environment.TickCount);
        }

        /// <summary>
        /// 使用指定的种子值初始化 <see cref="System.FastRandom"/> 类的新实例。
        /// </summary>
        /// <param name="seed">用来计算伪随机数序列起始值的数字。如果指定的是负数，则使用其绝对值。</param>
        public FastRandom(int seed)
        {
            Reinitialise(seed);
        }

        #endregion

        #region Public Methods [Reinitialisation]

        /// <summary>
        /// 重新加载种子值。
        /// </summary>
        /// <param name="seed">用来计算伪随机数序列起始值的数字。如果指定的是负数，则使用其绝对值。</param>
        public void Reinitialise(int seed)
        {
            // The only stipulation stated for the xorshift RNG is that at least one of
            // the seeds x,y,z,w is non-zero. We fulfill that requirement by only allowing
            // resetting of the x seed
            x = (uint)seed;
            y = Y;
            z = Z;
            w = W;
        }

        #endregion

        #region Public Methods [System.Random functionally equivalent methods]

        /// <summary>
        /// 返回非负随机数。 
        /// </summary>
        /// <returns>大于等于零且小于 <see cref="System.Int32.MaxValue"/> 的 32 位带符号整数。</returns>
        public int Next()
        {
            uint t = (x ^ (x << 11));
            x = y; y = z; z = w;
            w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

            // Handle the special case where the value int.MaxValue is generated. This is outside of 
            // the range of permitted values, so we therefore call Next() to try again.
            uint rtn = w & 0x7FFFFFFF;
            if(rtn == 0x7FFFFFFF)
                return Next();
            return (int)rtn;
        }

        /// <summary>
        /// 返回一个小于所指定最大值的非负随机数。
        /// </summary>
        /// <param name="maxValue">要生成的随机数的上限（随机数不能取该上限值）。<paramref name="maxValue"/> 必须大于或等于零。</param>
        /// <returns>大于等于零且小于 <paramref name="maxValue"/> 的 32 位带符号整数，即：返回值的范围通常包括零但不包括 <paramref name="maxValue"/>。不过，如果 <paramref name="maxValue"/> 等于零，则返回 <paramref name="maxValue"/>。</returns>
        public int Next(int maxValue)
        {
            if(maxValue < 0) throw new ArgumentOutOfRangeException("maxValue", maxValue, "maxValue 小于零。");

            uint t = (x ^ (x << 11));
            x = y; y = z; z = w;

            // The explicit int cast before the first multiplication gives better performance.
            // See comments in NextDouble.
            return (int)((REAL_UNIT_INT * (int)(0x7FFFFFFF & (w = (w ^ (w >> 19)) ^ (t ^ (t >> 8))))) * maxValue);
        }

        /// <summary>
        /// 返回一个指定范围内的随机数。
        /// </summary>
        /// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
        /// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。<paramref name="maxValue"/> 必须大于或等于 <paramref name="minValue"/>。</param>
        /// <returns>一个大于等于 <paramref name="minValue"/> 且小于 <paramref name="maxValue"/> 的 32 位带符号整数，即：返回的值范围包括 <paramref name="minValue"/> 但不包括 <paramref name="maxValue"/>。如果 <paramref name="minValue"/> 等于 <paramref name="maxValue"/>，则返回 <paramref name="minValue"/>。</returns>
        public int Next(int minValue, int maxValue)
        {
            if(minValue > maxValue)
                throw new ArgumentOutOfRangeException("maxValue", maxValue, "minValue 大于 maxValue");

            uint t = (x ^ (x << 11));
            x = y; y = z; z = w;

            // The explicit int cast before the first multiplication gives better performance.
            // See comments in NextDouble.
            int range = maxValue - minValue;
            if(range < 0)
            {	// If range is <0 then an overflow has occured and must resort to using long integer arithmetic instead (slower).
                // We also must use all 32 bits of precision, instead of the normal 31, which again is slower.	
                return minValue + (int)((REAL_UNIT_UINT * (double)(w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)))) * (double)((long)maxValue - (long)minValue));
            }

            // 31 bits of precision will suffice if range<=int.MaxValue. This allows us to cast to an int and gain
            // a little more performance.
            return minValue + (int)((REAL_UNIT_INT * (double)(int)(0x7FFFFFFF & (w = (w ^ (w >> 19)) ^ (t ^ (t >> 8))))) * (double)range);
        }

        internal readonly static char[] UpperCaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        internal readonly static char[] LowerCaseCharacters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        internal readonly static char[] NumericCharacters = "0123456789".ToCharArray();
        internal readonly static char[] SpecialCharacters = ",.;:?!/@#$%^&()=+*-_{}[]<>|~".ToCharArray();

        /// <summary>
        /// 返回一个固定长度随机字符串。
        /// </summary>
        /// <param name="length">随机字符串的长度。</param>
        /// <param name="type">随机字符串的类型。</param>
        /// <returns>返回一个固定长度的随机字符串。</returns>
        public string NextString(int length, CharacterType type = CharacterType.Default)
        {
            if(length < 1) throw new ArgumentOutOfRangeException("length");
            char[] chars = new char[length];
            List<char> charsArray = new List<char>(40);
            if(type.HasFlag(CharacterType.UpperCase)) charsArray.AddRange(UpperCaseCharacters);
            if(type.HasFlag(CharacterType.LowerCase)) charsArray.AddRange(LowerCaseCharacters);
            if(type.HasFlag(CharacterType.Numeric)) charsArray.AddRange(NumericCharacters);
            if(type.HasFlag(CharacterType.Special)) charsArray.AddRange(SpecialCharacters);

            for(int i = 0; i < length; i++)
            {
                chars[i] = charsArray[this.Next(0, charsArray.Count)];
            }
            return new string(chars);
        }

        /// <summary>
        /// 返回一个指定范围内长度的随机字符串。
        /// </summary>
        /// <param name="minLength">返回的随机字符串长度的下界（可取该下界值）。</param>
        /// <param name="maxLength">返回的随机字符串长度的上界（不能取该上界值）。<paramref name="maxLength"/> 必须大于或等于 <paramref name="minLength"/>。</param>
        /// <param name="type">随机字符串的类型。</param>
        /// <returns>一个字符串长度大于等于 <paramref name="minLength"/> 且小于 <paramref name="maxLength"/> 的字符串。</returns>
        public string NextString(int minLength, int maxLength, CharacterType type = CharacterType.Default)
        {
            if(minLength < 1) throw new ArgumentOutOfRangeException("minLength");
            if(maxLength < 1) throw new ArgumentOutOfRangeException("maxLength");
            if(minLength > maxLength) throw new ArgumentOutOfRangeException("maxLength", maxLength, "minLength 大于 maxLength");
            return this.NextString(this.Next(minLength, maxLength), type);

        }
        /// <summary>
        /// 返回一个介于 0.0 和 1.0 之间的随机数。
        /// </summary>
        /// <returns>大于等于 0.0 并且小于 1.0 的双精度浮点数。</returns>
        public double NextDouble()
        {
            uint t = (x ^ (x << 11));
            x = y; y = z; z = w;

            // Here we can gain a 2x speed improvement by generating a value that can be cast to 
            // an int instead of the more easily available uint. If we then explicitly cast to an 
            // int the compiler will then cast the int to a double to perform the multiplication, 
            // this final cast is a lot faster than casting from a uint to a double. The extra cast
            // to an int is very fast (the allocated bits remain the same) and so the overall effect 
            // of the extra cast is a significant performance improvement.
            //
            // Also note that the loss of one bit of precision is equivalent to what occurs within 
            // System.Random.
            return (REAL_UNIT_INT * (int)(0x7FFFFFFF & (w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)))));
        }


        /// <summary>
        /// 用随机数填充指定字节数组的元素。
        /// </summary>
        /// <param name="buffer">包含随机数的字节数组。</param>
        public void NextBytes(byte[] buffer)
        {
            if(buffer == null) throw new ArgumentNullException("buffer");

            // Fill up the bulk of the buffer in chunks of 4 bytes at a time.
            uint x = this.x, y = this.y, z = this.z, w = this.w;
            int i = 0;
            uint t;
            for(int bound = buffer.Length - 3; i < bound; )
            {
                // Generate 4 bytes. 
                // Increased performance is achieved by generating 4 random bytes per loop.
                // Also note that no mask needs to be applied to zero out the higher order bytes before
                // casting because the cast ignores thos bytes. Thanks to Stefan Trosch黷z for pointing this out.
                t = (x ^ (x << 11));
                x = y; y = z; z = w;
                w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

                buffer[i++] = (byte)w;
                buffer[i++] = (byte)(w >> 8);
                buffer[i++] = (byte)(w >> 16);
                buffer[i++] = (byte)(w >> 24);
            }

            // Fill up any remaining bytes in the buffer.
            if(i < buffer.Length)
            {
                // Generate 4 bytes.
                t = (x ^ (x << 11));
                x = y; y = z; z = w;
                w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

                buffer[i++] = (byte)w;
                if(i < buffer.Length)
                {
                    buffer[i++] = (byte)(w >> 8);
                    if(i < buffer.Length)
                    {
                        buffer[i++] = (byte)(w >> 16);
                        if(i < buffer.Length)
                        {
                            buffer[i] = (byte)(w >> 24);
                        }
                    }
                }
            }
            this.x = x; this.y = y; this.z = z; this.w = w;
        }


        /// <summary>
        /// A version of NextBytes that uses a pointer to set 4 bytes of the byte buffer in one operation
        /// thus providing a nice speedup. The loop is also partially unrolled to allow out-of-order-execution,
        /// this results in about a x2 speedup on an AMD Athlon. Thus performance may vary wildly on different CPUs
        /// depending on the number of execution units available.
        /// 
        /// Another significant speedup is obtained by setting the 4 bytes by indexing pDWord (e.g. pDWord[i++]=w)
        /// instead of adjusting it dereferencing it (e.g. *pDWord++=w).
        /// 
        /// Note that this routine requires the unsafe compilation flag to be specified and so is commented out by default.
        /// </summary>
        /// <param name="buffer">包含随机数的字节数组。</param>
        public unsafe void NextBytesUnsafe(byte[] buffer)
        {
            if(buffer.Length % 8 != 0)
                throw new ArgumentException("缓冲区的长度必须能被 8 整除！", "buffer");

            uint x = this.x, y = this.y, z = this.z, w = this.w;

            fixed(byte* pByte0 = buffer)
            {
                uint* pDWord = (uint*)pByte0;
                for(int i = 0, len = buffer.Length >> 2; i < len; i += 2)
                {
                    uint t = (x ^ (x << 11));
                    x = y; y = z; z = w;
                    pDWord[i] = w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

                    t = (x ^ (x << 11));
                    x = y; y = z; z = w;
                    pDWord[i + 1] = w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
                }
            }

            this.x = x; this.y = y; this.z = z; this.w = w;
        }

        #endregion

        #region Public Methods [Methods not present on System.Random]

        /// <summary>
        /// 返回非负的 <see cref="System.UInt32.MaxValue"/> 随机数。 
        /// </summary>
        /// <returns>大于等于零且小于 <see cref="System.UInt32.MaxValue"/> 的 32 位无符号整数。</returns>
        public uint NextUInt()
        {
            uint t = (x ^ (x << 11));
            x = y; y = z; z = w;
            return (w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)));
        }

        /// <summary>
        /// 返回非负的 <see cref="System.Int32.MaxValue"/> 随机数。 
        /// </summary>        
        /// <returns>大于等于零且小于 <see cref="System.Int32.MaxValue"/> 的 32 位有符号整数。</returns>  
        public int NextInt()
        {
            uint t = (x ^ (x << 11));
            x = y; y = z; z = w;
            return (int)(0x7FFFFFFF & (w = (w ^ (w >> 19)) ^ (t ^ (t >> 8))));
        }

        // Buffer 32 bits in bitBuffer, return 1 at a time, keep track of how many have been returned
        // with bitBufferIdx.
        uint bitBuffer;
        uint bitMask = 1;

        /// <summary>
        /// 返回一个随机的布尔值。
        /// </summary>
        /// <returns>一个 <see cref="System.Boolean"/> 的随机值。</returns>
        public bool NextBool()
        {
            if(bitMask == 1)
            {
                // Generate 32 more bits.
                uint t = (x ^ (x << 11));
                x = y; y = z; z = w;
                bitBuffer = w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

                // Reset the bitMask that tells us which bit to read next.
                bitMask = 0x80000000;
                return (bitBuffer & bitMask) == 0;
            }

            return (bitBuffer & (bitMask >>= 1)) == 0;
        }

        #endregion
    }

    /// <summary>
    /// 表示一个字符串的类型。
    /// </summary>
    [Serializable]
    [Flags]
    public enum CharacterType
    {
        /// <summary>
        /// 默认类型。表示 A-Z 和 0-9。
        /// </summary>
        Default = UpperCase | Numeric,
        /// <summary>
        /// 大写字符。表示 A-Z。
        /// </summary>
        UpperCase = 1,
        /// <summary>
        /// 小写字符。表示 a-z。
        /// </summary>
        LowerCase = 2,
        /// <summary>
        /// 数字字符。表示 0-9。
        /// </summary>
        Numeric = 4,
        /// <summary>
        /// 特殊字符。表示“,.;:?!/@#$%^&amp;()=+*-_{}[]&lt;&gt;>|~”之一。
        /// </summary>
        Special = 8
    }
}
