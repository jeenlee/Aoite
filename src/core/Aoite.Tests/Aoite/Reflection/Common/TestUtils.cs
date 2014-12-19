
namespace Aoite.ReflectionTest.Common
{
    public static class TestUtils
    {
        public static string FirstCharUpper(this string str)
        {
            if(string.IsNullOrEmpty(str))
            {
                return str;
            }
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

        public static string FirstCharLower(this string str)
        {
            if(string.IsNullOrEmpty(str))
            {
                return str;
            }
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }
    }
}