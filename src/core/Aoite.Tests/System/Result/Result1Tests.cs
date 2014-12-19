using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System
{
    public class Result1Tests
    {
        private T AssertSuccess<T>(T r, string valueString = Result.NullValueString)
            where T : Result
        {
            Assert.NotNull(r);
            Assert.Equal(0, r.Status);
            Assert.Equal(null, r.Exception);
            Assert.Equal(null, r.Message);
            Assert.True(r.IsSucceed);
            Assert.False(r.IsFailed);
            Assert.Equal(valueString, r.ToString());
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
            var r = AssertSuccess(new Result<Uri>());
            Assert.Null(r.GetValue());
            Assert.Equal(typeof(Uri), r.GetValueType());
        }
        [Fact()]
        public void Constructor_T()
        {
            Uri value = new Uri("http://www.baidu.com");
            var r = AssertSuccess((Result<Uri>)value, value.ToString());
            Assert.Equal(value, r.Value);
            Assert.Equal(value, r.UnsafeValue);
            Assert.Equal(value, r.GetValue());
            Assert.Equal(typeof(Uri), r.GetValueType());
        }

        [Fact()]
        public void UnsafeValueT_Throw_Exception()
        {
            Assert.Throws<Exception>(() => new Result<Uri>(new Exception("test"), 5).UnsafeValue);
            Assert.Throws<ResultException>(() => new Result<Uri>("test", 5).UnsafeValue);
        }

        [Fact()]
        public void Constructor_Exception_Int32()
        {
            var exception = new Exception("test");
            var status = 5;
            AssertFail(new Result<Uri>(exception, status), status, exception);
        }

        [Fact()]
        public void Constructor_ExceptionNull_Int32()
        {
            Exception exception = null;
            var status = 5;

            AssertSuccess(new Result<Uri>(exception, status)).ToString();
        }

        [Fact()]
        public void Constructor_String_Int32()
        {
            var message = "test";
            var status = 5;
            AssertFail(new Result<Uri>(message, status), status, message);
        }

        [Fact()]
        public void Constructor_StringNull_Int32()
        {
            string message = null;
            var status = 5;
            AssertSuccess(new Result<Uri>(message, status));
        }

        [Fact()]
        public void set_Value()
        {
            var value = new Uri("http://www.baidu.com");
            Result<Uri> r = new Result<Uri>();
            r.Value = value;
            AssertSuccess(r, value.ToString());
        }

        [Fact()]
        public void SetValue()
        {
            var value = new Uri("http://www.baidu.com");
            Result<Uri> r = new Result<Uri>();
            r.SetValue(value);
            AssertSuccess(r, value.ToString());
            Assert.Throws<InvalidCastException>(() => r.SetValue(""));
        }
        [Fact()]
        public void Implicit_Exception()
        {
            var exception = new Exception("test");
            AssertFail((Result<Uri>)exception, -1, exception);
        }

        [Fact()]
        public void Implicit_ExceptionNull()
        {
            Exception exception = null;
            AssertSuccess((Result<Uri>)exception);
        }

        [Fact()]
        public void Implicit_String()
        {
            var message = "test";
            AssertFail((Result<Uri>)message, -1, message);
        }

        [Fact()]

        public void Implicit_StringNull()
        {
            string message = null;
            AssertSuccess((Result<Uri>)message);
        }

        [Fact()]

        public void Implicit_T()
        {
            var value = new Uri("http://www.baidu.com");
            AssertSuccess((Result<Uri>)value, value.ToString());
        }

    }
}
