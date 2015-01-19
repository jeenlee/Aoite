using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Core
{
    public class LogTests
    {
        private readonly StringBuilder Builder = new StringBuilder();
        public LogTests()
        {
            var logger = Log.Logger as Aoite.Logger.TextLogger;
            logger.Asynchronous = false;
            var ctwf = logger.TextWriterFactory as Aoite.Logger.CustomTextWriterFactory;
            StringWriter writer = new StringWriter(Builder);
            ctwf.CreateWriter = () => writer;
        }
        private void Equal(string expected)
        {
            List<string> actualList = new List<string>();
            using(var reader = new StringReader(Builder.ToString()))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    if(line.Length == 0) continue;
                    actualList.Add(line.ToString().Substring(14));
                }
            }

            Assert.Equal(expected, actualList.ToArray().Reverse().Join("\r\n"));
            Builder.Clear();
        }

        [Fact()]
        public void LoggerTest()
        {
            Log.Info("一个消息。");
            Equal("[消息] 一个消息。");
            Log.Warn("一个警告。");
            Equal("[警告] 一个警告。");
            Log.Error("一个错误。");
            Equal("[错误] 一个错误。");
        }

        [Fact()]
        public void ContextTest()
        {
            Task[] tasks = new Task[10];
            for(int i = 0; i < 10; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    using(var context = Log.Context)
                    {
                        context.Info("一个消息。");
                        context.Warn("一个警告。");
                        context.Error("一个错误。");
                    }
                });
            }
            Task.WaitAll(tasks);
            string[] expecteds = new string[10];
            for(int i = 0; i < 10; i++)
            {
                expecteds[i] = "[消息] 一个消息。\r\n[警告] 一个警告。\r\n[错误] 一个错误。";
            }
            Equal(expecteds.Join("\r\n"));
        }
    }
}
