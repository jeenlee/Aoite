using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System
{
    public class DataSecurityTests
    {
        [Fact()]
        public void Test()
        {
            Assert.Equal("E10ADC3949BA59ABBE56E057F20F883E", DataSecurity.Crypto(SecurityAlgorithms.MD5, "123456"));
            Assert.Equal("7C4A8D09CA3762AF61E59520943DC26494F8941B", DataSecurity.Crypto(SecurityAlgorithms.SHA1, "123456"));
            Assert.Equal("8D969EEF6ECAD3C29A3A629280E686CF0C3F5D5A86AFF3CA12020C923ADC6C92", DataSecurity.Crypto(SecurityAlgorithms.SHA256, "123456"));
            Assert.Equal("0A989EBC4A77B56A6E2BB7B19D995D185CE44090C13E2984B7ECC6D446D4B61EA9991B76A4C2F04B1B4D244841449454", DataSecurity.Crypto(SecurityAlgorithms.SHA384, "123456"));
            Assert.Equal("BA3253876AED6BC22D4A6FF53D8406C6AD864195ED144AB5C87621B6C233B548BAEAE6956DF346EC8C17F5EA10F35EE3CBC514797ED7DDD3145464E2A0BAB413", DataSecurity.Crypto(SecurityAlgorithms.SHA512, "123456"));
        }
    }
}
