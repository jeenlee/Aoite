using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System
{
    public class AjobTests
    {
        [Fact]
        public void OnceTest()
        {
            bool isRunning = false;
            var token = Ajob.Once(j => isRunning = true, 1000);
            token.Wait();
            Assert.True(isRunning);
        }

        [Fact]
        public void OnceExceptionTest()
        {
            Assert.Throws<AggregateException>(() =>
            {
                var token = Ajob.Once(j =>
                {
                    throw new ArgumentNullException();
                });
                token.Wait();
            });
        }

        [Fact]
        public void OnceNonBlockingTest()
        {
            bool isRunning = false;
            Ajob.Once(j => isRunning = true).Wait();
            Assert.True(isRunning);
        }

        [Fact]
        public void OnceCancelTest()
        {
            var token = Ajob.Once(j => j.Delay(5000), 1000);
            if(token.Wait(1000)) Assert.Fail();
            token.Cancel();
        }

        [Fact]
        public void LoopTest()
        {
            int testCount = 0;
            var token = Ajob.Loop(j => testCount++, 300);
            if(!token.Wait(1000)) token.Cancel();
            Assert.Equal(3, testCount);
        }
    }
}
