using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System
{
    public class ResultTests
    {
        private T AssertSuccess<T>(T r)
            where T : Result
        {
            Assert.NotNull(r);
            Assert.Equal(0, r.Status);
            Assert.Equal(null, r.Exception);
            Assert.Equal(null, r.Message);
            Assert.True(r.IsSucceed);
            Assert.False(r.IsFailed);
            Assert.Equal(Result.SuccessedString, r.ToString());
            return r;
        }
        private T AssertFail<T>(T r, int status, string message)
            where T : Result
        {
            Assert.NotNull(r);
            Assert.Equal(status, r.Status);
            Assert.Equal(message, r.Exception.Message);
            Assert.Equal(message, r.Message);
            Assert.False(r.IsSucceed);
            Assert.True(r.IsFailed);
            Assert.Equal(message, r.ToString());
            return r;
        }
        private T AssertFail<T>(T r, int status, Exception exception)
            where T : Result
        {
            Assert.NotNull(r);
            Assert.Equal(status, r.Status);
            Assert.Equal(exception, r.Exception);
            Assert.Equal(exception.Message, r.Message);
            Assert.False(r.IsSucceed);
            Assert.True(r.IsFailed);
            Assert.Equal(exception.Message, r.ToString());
            return r;
        }

        [Fact()]
        public void Constructor_Empty()
        {
            AssertSuccess(new Result());
        }

        [Fact()]
        public void Constructor_Exception_Int32()
        {
            var exception = new Exception("test");
            var status = 5;
            AssertFail(new Result(exception, status), status, exception);
        }

        [Fact()]
        public void Constructor_ExceptionNull_Int32()
        {
            Exception exception = null;
            var status = 5;
            AssertSuccess(new Result(exception, status));
        }

        [Fact()]
        public void Constructor_String_Int32()
        {
            var message = "test";
            var status = 5;
            AssertFail(new Result(message, status), status, message);
        }

        [Fact()]
        public void Constructor_StringNull_Int32()
        {
            string message = null;
            var status = 5;
            AssertSuccess(new Result(message, status));
        }

        [Fact()]
        public void Implicit_Exception()
        {
            var exception = new Exception("test");
            AssertFail((Result)exception, -1, exception);
        }

        [Fact()]
        public void Implicit_ExceptionNull()
        {
            Exception exception = null;
            AssertSuccess((Result)exception);
        }

        [Fact()]
        public void Implicit_String()
        {
            var message = "test";
            AssertFail((Result)message, -1, message);
        }

        [Fact()]
        public void Implicit_StringNull()
        {
            string message = null;
            AssertSuccess((Result)message);
        }


    }
}
