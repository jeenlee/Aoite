using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Logger.Text
{
    public class LogPathFactoriesTests : IDisposable
    {
        private readonly static string LogFolder = GA.FullPath("Logs");
        void IDisposable.Dispose()
        {
            GA.IO.DeleteDirectory(LogFolder)
                .Wait(500);
        }

        [Fact()]
        public void DayLogPathFactoryTest()
        {
            DayLogPathFactory factory = new DayLogPathFactory();
            var now = new DateTime(2015, 3, 1);
            Assert.False(factory.IsCreated(now));
            Assert.Equal(GA.FullPath(LogFolder, "2015年03月", "01.log"), factory.CreatePath(now, LogFolder, ".log"));
            Assert.True(factory.IsCreated(now));
            now = now.AddDays(1);

            Assert.False(factory.IsCreated(now));
            Assert.Equal(GA.FullPath(LogFolder, "2015年03月", "02.log"), factory.CreatePath(now, LogFolder, ".log"));
            Assert.True(factory.IsCreated(now));
        }

        [Fact()]
        public void HourLogPathFactoryTest()
        {
            HourLogPathFactory factory = new HourLogPathFactory();
            var now = new DateTime(2015, 1, 5, 3, 0, 0);
            Assert.False(factory.IsCreated(now));
            Assert.Equal(GA.FullPath(LogFolder, "2015年01月","05", "03.log"), factory.CreatePath(now, LogFolder, ".log"));
            Assert.True(factory.IsCreated(now));
            now = now.AddDays(1);

            Assert.False(factory.IsCreated(now));
            Assert.Equal(GA.FullPath(LogFolder, "2015年01月", "06", "03.log"), factory.CreatePath(now, LogFolder, ".log"));
            Assert.True(factory.IsCreated(now));

            now = now.AddHours(1);

            Assert.False(factory.IsCreated(now));
            Assert.Equal(GA.FullPath(LogFolder, "2015年01月", "06", "04.log"), factory.CreatePath(now, LogFolder, ".log"));
            Assert.True(factory.IsCreated(now));
        }

    }
}
