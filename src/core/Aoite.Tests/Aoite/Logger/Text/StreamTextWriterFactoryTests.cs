using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Logger.Text
{
    public class StreamTextWriterFactoryTests : IDisposable
    {
        private readonly static string LogFolder = GA.FullPath("Logs");
        void IDisposable.Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            GA.IO.DeleteDirectory(LogFolder)
                .Wait(500);
        }

        [Fact()]
        public void Test1()
        {
            var now = new DateTime(2015, 1, 1);
            const string logText = "abcdefg";
            StreamTextWriterFactory writerFactory = new StreamTextWriterFactory(new DayLogPathFactory());
            writerFactory.NowGetter = () => now;
            writerFactory.Process(writer =>
            {
                writer.Write(logText);
            });
            var path = GA.FullPath(LogFolder, "2015年01月", "01.log");
            Assert.Equal(logText, GA.IO.ShareReadAllText(path, Encoding.UTF8));
        }

        [Fact()]
        public void Test2()
        {
            var now = new DateTime(2015, 2, 1);
            StreamTextWriterFactory writerFactory = new StreamTextWriterFactory(new DayLogPathFactory());
            TextLogger logger = new TextLogger(writerFactory, new DefaultLogDescriptor());
            writerFactory.NowGetter = () => now;
            logger.Write(new LogItem()
            {
                Time = now,
                Message = "测试内容。",
                Type = LogType.Info
            });
            System.Threading.Thread.Sleep(1001);
            var path = GA.FullPath(LogFolder, "2015年02月", "01.log");
            Assert.Equal(now.ToString("HH:mm:ss.ffff") + " [消息] 测试内容。\r\n", GA.IO.ShareReadAllText(path, Encoding.UTF8));
        }
    }
}
