using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Logger.Text
{
    public class DefaultLogDescriptorTests
    {
        [Fact()]
        public void Test1()
        {
            DefaultLogDescriptor desc = new DefaultLogDescriptor();
            var now = DateTime.Now;
            var str = desc.Describe(Log.Logger, new LogItem()
            {
                Time = now,
                Message = "测试内容。",
                Type = LogType.Info
            });
            Assert.Equal(now.ToString("HH:mm:ss.ffff") + " [消息] 测试内容。", str);
        }
    }
}
