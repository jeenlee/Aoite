using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Aoite.Serialization.Json
{
    //二进制序列化还需要支持：DateTimeOffset、Uri
    class JsonWriter : JsonParserBase
    {
        public override string ToString()
        {
            return sb.ToString();
        }

        public JsonWriter() : this(new StringBuilder()) { }
        StringBuilder sb;
        public JsonWriter(StringBuilder builder)
        {
            if(builder == null) throw new ArgumentNullException("builder");
            this.sb = builder;
        }

        public void Clear()
        {
            this.sb.Clear();
        }
        public static string ToJson(object value)
        {
            var w = new JsonWriter();
            w.Write(value);
            return w.ToString();
        }
        public void Write(object value)
        {
            if(value == null)
            {
                this.WriteNull();
                return;
            }
            if(value is string)
            {
                this.WriteString((string)value);
                return;
            }
            if(value is Char)
            {
                this.WriteChar((char)value);
                return;
            }
            if(value is bool)
            {
                this.WriteBoolean((bool)value);
                return;
            }
            if(value is DateTime)
            {
                this.WriteDateTime((DateTime)value);
                return;
            }
            if(value is DateTimeOffset)
            {
                this.WriteDateTimeOffset((DateTimeOffset)value);
                return;
            }
            if(value is Guid)
            {
                this.WriteGuid((Guid)value);
                return;
            }
            if(value is double)
            {
                this.WriteDouble((double)value);
                return;
            }
            if(value is float)
            {
                this.WriteSingle((float)value);
                return;
            }
            if(value is Int64)
            {
                this.WriteString(value.ToString());
                return;
            }
            if(value is UInt64)
            {
                this.WriteString(value.ToString());
                return;
            }
            var type = value.GetType().GetNullableType();
            if(type.IsPrimitive || value is Decimal)
            {
                this.WritePrimitive(value);
                return;
            }
            if(type.IsEnum)
            {
                this.WriteEnum((Enum)value);
                return;
            }
            if(value is Result)
            {
                this.WriteResult((Result)value);
                return;
            }

            if(objectsInUse.IsValueCreated && objectsInUse.Value.ContainsKey(value)) throw new InvalidOperationException("对象 {0} 已被重复引用，在 JSON 序列化时不允许两个值表示相同对象。".Fmt(value.GetType().FullName));
            objectsInUse.Value.Add(value, null);
            if(objectsInUse.Value.Count > 1000) throw new OutOfMemoryException("对象太多了，最高只能限制 1000 个引用对象。");

            if(value is IDictionary)
            {
                this.WriteDictionary((IDictionary)value);
                return;
            }
            if(value is IEnumerable)
            {
                this.WriteEnumerable((IEnumerable)value);
                return;
            }
            this.WriteObject(value, type);
        }

        internal void WriteNull()
        {
            sb.Append("null");
        }
        internal void WriteString(string value)
        {
            //sb.Append('\"');
            sb.Append(HttpUtility.JavaScriptStringEncode(value, true));
            //sb.Append('\"');
        }
        internal void WriteChar(char value)
        {
            if(value == '\0') this.WriteNull();
            else this.WriteString(value.ToString());
        }
        internal void WriteBoolean(bool value)
        {
            sb.Append(value ? "true" : "false");
        }

        internal void WriteDateTime(DateTime value)
        {
            sb.Append("\"/Date(")
                .Append((value.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000)
                .Append(")/\"");
        }
        internal void WriteDateTimeOffset(DateTimeOffset value)
        {
            this.WriteDateTime(value.UtcDateTime);
        }
        internal void WriteGuid(Guid value)
        {
            sb.Append("\"").Append(value.ToString()).Append("\"");
        }
        internal void WriteDouble(Double value)
        {
            sb.Append(value.ToString("r", CultureInfo.InvariantCulture));
        }
        internal void WriteSingle(Single value)
        {
            sb.Append(value.ToString("r", CultureInfo.InvariantCulture));
        }
        internal void WritePrimitive(object value)
        {
            sb.Append(((IConvertible)value).ToString(CultureInfo.InvariantCulture));
        }
        internal void WriteEnum(Enum value)
        {
            var type = Enum.GetUnderlyingType(value.GetType());
            if(type == Types.Int64 || type == Types.UInt64) this.WriteString(value.ToString("D"));
            else sb.Append(value.ToString("D"));
        }

        internal void WriteDictionary(IDictionary value)
        {
            sb.Append('{');
            var isFirstElement = true;

            foreach(DictionaryEntry entry in value)
            {
                var key = entry.Key as string;
                if(key == null) throw new NotSupportedException("字典类型 {0} 错误，键只能是一个字符串类型。".Fmt(value.GetType().FullName));
                if(!isFirstElement) sb.Append(',');
                isFirstElement = false;

                WriteDictionaryKeyValue(key, entry.Value);
            }
            sb.Append('}');

        }

        private void WriteDictionaryKeyValue(string key, object value)
        {
            this.WriteString(key);
            sb.Append(':');
            this.Write(value);
        }

        internal void WriteEnumerable(IEnumerable value)
        {
            sb.Append('[');
            var isFirstElement = true;
            foreach(object o in value)
            {
                if(!isFirstElement) sb.Append(',');
                isFirstElement = false;
                this.Write(o);
            }
            sb.Append(']');
        }

        internal void WriteResult(Result result)
        {
            sb.Append('{');
            if(result.IsFailed)
            {
                sb.Append("\"status\":");
                sb.Append(result.Status);
                sb.Append(",\"message\":");
                this.WriteString(result.Exception.Message);
            }
            else
            {
                var value = result.GetValue();
                if(value != null)
                {
                    sb.Append("\"value\":");
                    this.Write(value);
                }
            }

            sb.Append('}');
        }

        internal void WriteObject(object value, Type type)
        {
            var isFirstElement = true;
            var mapper = Aoite.Data.EntityMapper.Create(type);
            sb.Append('{');
            foreach(var item in mapper.Properties)
            {
                if(item.IsIgnore) continue;
                if(!isFirstElement) sb.Append(',');
                isFirstElement = false;
                this.WriteString(item.Name.ToCamelCase());
                sb.Append(":");
                this.Write(item.GetValue(value));
            }
            sb.Append('}');
        }
    }
}
