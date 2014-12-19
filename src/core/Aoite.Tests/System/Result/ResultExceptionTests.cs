using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System
{
    public class ResultExceptionTests
    {
        [Fact()]
        public void Constructor_String_Int32()
        {
            var message = "test";
            var status = 5;
            var ex = new ResultException(message, status);
            Assert.Equal(message, ex.Message);
            Assert.Equal(status, ex.Status);
            Assert.Throws<ArgumentOutOfRangeException>(() => new ResultException("", 0));
        }

        [Fact()]
        public void get_Status()
        {
            var ex = new ResultException(5);
            Assert.Equal(5, ex.Status);
            Assert.Equal("错误 5", ex.Message);
        }

        [Fact()]
        public void get_Message()
        {
            var ex = new ResultException("test");
            Assert.Equal(ResultStatus.Failed, ex.Status);
            Assert.Equal("test", ex.Message);
        }

        [Fact()]
        public void ToResult()
        {
            var ex = new ResultException(5);
            var r = ex.ToResult();

            Assert.Equal(5, r.Status);
            Assert.Equal("错误 5", r.Message);
            Assert.Equal(ex, r.Exception);
        }

        [Fact()]
        public void ToResultT()
        {
            var ex = new ResultException(5);
            var r = ex.ToResult<Uri>();

            Assert.Equal(5, r.Status);
            Assert.Equal("错误 5", r.Message);
            Assert.Equal(ex, r.Exception);
        }

        [Fact()]
        public void ToCustomResult()
        {
            var ex = new ResultException(5);
            var r = ex.ToCustomResult(typeof(Result<Uri>));
            Assert.Equal(5, r.Status);
            Assert.Equal("错误 5", r.Message);
            Assert.Equal(ex, r.Exception);
        }

        [Fact()]
        public void ToCustomResultT()
        {
            var ex = new ResultException(5);
            var r = ex.ToCustomResult<Result<Uri>>();
            Assert.Equal(5, r.Status);
            Assert.Equal("错误 5", r.Message);
            Assert.Equal(ex, r.Exception);
        }
    }
}
