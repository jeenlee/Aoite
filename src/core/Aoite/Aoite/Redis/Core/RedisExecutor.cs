using Aoite.Net;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Aoite.Redis
{
    class RedisExecutor
    {
        const char CR = '\r', LF = '\n';
        const char MultiBulkChar = (char)RedisReplyType.MultiBulk;
        const char BulkChar = (char)RedisReplyType.Bulk;

        private IConnector _connector;
        public RedisExecutor(IConnector connector)
        {
            if(connector == null) throw new ArgumentNullException("connector");
            this._connector = connector;
        }

        public void AssertType(RedisReplyType expectedType)
        {
            var actualType = this.ReadType();
            if((int)actualType == -1)
                throw new RedisIOException("意外结束的流，预期回复类型：'{0}'。".Fmt(expectedType));
            this.AssertType(expectedType, actualType);
        }
        public void AssertType(RedisReplyType expectedType, RedisReplyType actualType)
        {
            if(actualType != expectedType)
                throw new RedisProtocolException("意外的回复类型：'{0}'，预期回复类型：'{1}'。".Fmt(actualType, expectedType));

        }
        public void AssertBytesRead(long expecting, long actual)
        {
            if(actual != expecting)
                throw new RedisProtocolException(String.Format("预期 {0} bytes；实际 {1} bytes", expecting, actual));
        }

        public void AssertSize(long expectedSize)
        {
            long size = ReadInteger(false);
            AssertSize(expectedSize, size);
        }

        public void AssertSize(long expectedSize, long actualSize)
        {
            if(actualSize != expectedSize)
                throw new RedisProtocolException("预期 " + expectedSize + " 个元素，实际只有 " + actualSize + " 个元素。");
        }

        #region Read

        public string ReadLine()
        {
            var sb = new StringBuilder();
            char c;
            int crIndex = -1;
            var stream = this._connector.ReadStream;
            while(true)
            {
                c = (char)stream.ReadByte();
                if(c == CR) crIndex = sb.Length;
                else if(c == LF && (crIndex == sb.Length)) break;
                else sb.Append(c);
            }
            return sb.ToString();
        }
        private void ReadCRLF() // TODO: remove hardcoded
        {
            var stream = this._connector.ReadStream;
            var r = stream.ReadByte();
            var n = stream.ReadByte();
            if(r != CR && n != LF)
                throw new RedisProtocolException("预期回复 'CRLF'，实际回复字节：{0}, {1}".Fmt(r, n));
        }

        public RedisReplyType ReadType(bool throwException = true)
        {
            var stream = this._connector.ReadStream;
            var type = (RedisReplyType)stream.ReadByte();
            if(type == RedisReplyType.Error && throwException) throw new RedisReplyException(this.ReadLine());
            return type;
        }

        private const string OK_STATUS = "OK";
        public Result ReadStatus(bool checkType = true, string statusText = OK_STATUS)
        {
            if(checkType)
            {
                var type = this.ReadType(false);
                if(type == RedisReplyType.Error) return this.ReadLine();
            }
            var s = this.ReadLine();
            if(s == statusText) return Result.Successfully;
            return new Result(s);
        }

        public long ReadInteger(bool checkType = true)
        {
            if(checkType) this.AssertType(RedisReplyType.Integer);

            var line = this.ReadLine();
            return Int64.Parse(line);
        }

        public byte[] ReadBulk(bool checkType = true)
        {
            if(checkType) this.AssertType(RedisReplyType.Bulk);
            var size = (int)this.ReadInteger(false);
            if(size == -1) return null;

            var bulk = new byte[size];
            int bytes_read = 0, bytes_remaining = size;
            var stream = this._connector.ReadStream;

            while(bytes_read < size)
                bytes_read += stream.Read(bulk, bytes_read, size - bytes_read);

            this.AssertBytesRead(size, bytes_read);
            this.ReadCRLF();
            return bulk;
        }

        public string ReadBulkString(bool checkType = true)
        {
            byte[] bulk = ReadBulk(checkType);
            if(bulk == null) return null;
            return GA.UTF8.GetString(bulk);
        }

        public object[] ReadMultiBulk(bool checkType = true)
        {
            if(checkType) this.AssertType(RedisReplyType.MultiBulk);
            var count = this.ReadInteger(false);
            if(count == -1) return null;
            object[] values = new object[count];
            for(long i = 0; i < count; i++)
            {
                values[i] = this.ReadObject();
            }
            return values;
        }

        public object ReadObject()
        {
            RedisReplyType type = ReadType();
            switch(type)
            {
                case RedisReplyType.Bulk:
                    return ReadBulk(false);

                case RedisReplyType.Integer:
                    return ReadInteger(false);

                case RedisReplyType.MultiBulk:
                    return ReadMultiBulk(false);

                case RedisReplyType.Status:
                    return ReadStatus(false);
            }
            throw new RedisProtocolException("意外的回复类型： " + type);
        }

        #endregion

        #region Write

        private void WriteBytes(StreamWriter writer, byte[] value)
        {
            // $<参数 1 的字节数量> CR LF
            writer.Write(BulkChar);
            writer.Write(value.Length.ToString());
            writer.Write(CR);
            writer.Write(LF);

            // <参数 1 的数据> CR LF
            writer.Flush();
            writer.BaseStream.Write(value, 0, value.Length);
            writer.Write(CR);
            writer.Write(LF);
        }

        public T Write<T>(T command) where T : RedisCommand
        {
            var parts = command.Command.Split(' ');
            var argumentCount = parts.Length + command.Arguments.Length;
            var encoding = GA.UTF8;
            using(var commandStream = new MemoryStream())
            {
                using(var writer = new StreamWriter(commandStream, encoding))
                {
                    // *<参数数量> CR LF
                    writer.Write(MultiBulkChar);
                    writer.Write(argumentCount.ToString());
                    writer.Write(CR);
                    writer.Write(LF);

                    foreach(var part in parts)
                    {
                        this.WriteBytes(writer, encoding.GetBytes(part));
                    }

                    foreach(var arg in command.Arguments)
                    {
                        if(arg == null) throw new RedisIOException("参数不能为空。");
                        else if(arg is BinaryValue)
                        {
                            var value = (arg as BinaryValue);
                            if(!value.HasValue()) throw new RedisIOException("System.BinaryValue 包含了无效值。");
                            this.WriteBytes(writer, value.ByteArray);
                        }
                        else if(arg is byte[])
                        {
                            this.WriteBytes(writer, arg as byte[]);
                        }
                        else
                        {
                            var type = arg.GetType();
                            if(type == Types.Boolean)
                            {
                                this.WriteBytes(writer, encoding.GetBytes(((bool)arg) ? "1" : "0"));
                            }
                            else if(type.IsPrimitive || type == Types.Decimal || type == Types.String)
                            {
                                var str = Convert.ToString(arg, CultureInfo.InvariantCulture);
                                this.WriteBytes(writer, encoding.GetBytes(str));
                            }
                            else throw new RedisIOException("参数不支持数据类型 {0}。".Fmt(arg.GetType().FullName));
                        }
                    }
                    writer.Flush();
                    commandStream.Seek(0, SeekOrigin.Begin);
                    commandStream.CopyTo(this._connector.WriteStream);
                }
            }
            return command;
        }

        public T Execute<T>(RedisCommand<T> command)
        {
            return this.Write(command).Parse(this);
        }

        #endregion
    }
}
