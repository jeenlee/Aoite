using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Aoite.Data.Profilers
{
    /// <summary>
    /// 一个查询监控项。
    /// </summary>
    public class ProfilerItem
    {
        /// <summary>
        /// 获取或设置监控项输出的重复字节。
        /// </summary>
        public static char DefaultRepeatChar = '+';
        /// <summary>
        /// 获取或设置监控项输出重复字节的数量。
        /// </summary>
        public static int DefaultRepeatCount = 75;

        private string _EngineName;
        /// <summary>
        /// 获取引擎的名称。
        /// </summary>
        public string EngineName
        {
            get
            {
                return this._EngineName;
            }
        }

        private ExecuteType _Type;
        /// <summary>
        /// 获取查询的类型。
        /// </summary>
        public ExecuteType Type
        {
            get
            {
                return this._Type;
            }
        }

        private DateTime _BeginTime;
        /// <summary>
        /// 获取查询的开始时间。
        /// </summary>
        public DateTime BeginTime
        {
            get
            {
                return this._BeginTime;
            }
        }

        private DateTime _EndTime;
        /// <summary>
        /// 获取查询的结束时间。
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return this._EndTime;
            }
        }

        private string _CommandText;
        /// <summary>
        /// 获取查询的文本内容。
        /// </summary>
        public string CommandText
        {
            get
            {
                return this._CommandText;
            }
        }
        private Dictionary<string, string> _Parameters;
        /// <summary>
        /// 获取查询的参数集合。
        /// </summary>
        public Dictionary<string, string> Parameters
        {
            get
            {
                return this._Parameters;
            }
        }

        private string _Value;
        /// <summary>
        /// 获取查询的返回内容。
        /// </summary>
        public string Value
        {
            get
            {
                return this._Value;
            }
        }

        private string _ExceptionMessage;

        /// <summary>
        /// 获取查询的开始时间。
        /// </summary>
        public string ExceptionMessage
        {
            get
            {
                return this._ExceptionMessage;
            }
        }

        internal ProfilerItem() { this._BeginTime = DateTime.Now; }

        internal void Completed(ExecuteType type, IDbResult result)
        {
            this._EndTime = DateTime.Now;

            var command = result.Command;
            var engine = result.Engine;
            var owner = engine.Owner;
            this._EngineName = owner.Name ?? owner.ConnectionString;
            this._Type = type;
            this._CommandText = command.CommandText;

            var dict = new Dictionary<string, string>(command.Parameters.Count);
            foreach(DbParameter p in command.Parameters)
            {
                string name = owner.Injector.DescribeParameter(result.Engine, p);
                string value = (Convert.IsDBNull(p.Value) || p.Value == null) ? null : p.Value.ToString();
                dict.Add(name, value);

            }
            this._Parameters = dict;
            this._ExceptionMessage = result.IsSucceed ? null : result.Exception.Message;

            if(result.IsSucceed)
            {
                switch(type)
                {
                    case ExecuteType.Reader:
                        this._Value = "{DataReader}";
                        break;
                    case ExecuteType.DataSet:
                        var dataSet = (result.Value as DataSet);
                        this._Value = "找到 " + dataSet.Tables.Count + " 张表";
                        for(int i = 0; i < dataSet.Tables.Count; i++)
                        {
                            if(i == 0)
                            {
                                this._Value = "(";
                            }
                            else
                            {
                                this._Value += ",";
                            }
                            this._Value += dataSet.Tables[i].Rows.Count;
                        }
                        if(dataSet.Tables.Count > 0) this._Value += ")";

                        break;
                    case ExecuteType.Table:
                        var dataTable = (result.Value as DataTable);
                        this._Value = "找到 " + dataTable.Rows.Count + " 条记录";
                        break;
                    case ExecuteType.NoQuery:
                        this._Value = result.Value + " 行受影响";
                        break;
                    default:
                        this._Value = result.Value == null ? null : (result.Value.ToString() + "\t\t" + result.Value.GetType().ToString());
                        break;
                }
            }
        }

        private string _ToString;
        /// <summary>
        /// 返回字符串形式的监控项。
        /// </summary>
        public override string ToString()
        {
            if(this._ToString != null) return this._ToString;

            lock(this)
            {
                if(this._ToString != null) return this._ToString;

                StringBuilder descBuilder = new StringBuilder();
                descBuilder.Append(DefaultRepeatChar, DefaultRepeatCount); descBuilder.AppendLine();
                descBuilder.AppendLine("【查询信息】");
                descBuilder.Append("\t引擎：");
                descBuilder.AppendLine(this._EngineName);
                descBuilder.Append("\t开始：");
                descBuilder.AppendLine(this._BeginTime.ToString("yyyy-MM-dd HH:mm:ss ffffff"));
                descBuilder.Append("\t结束：");
                descBuilder.AppendLine(this._EndTime.ToString("yyyy-MM-dd HH:mm:ss ffffff"));
                descBuilder.Append("\t耗时：");
                descBuilder.Append((this._EndTime - this._BeginTime).TotalMilliseconds);
                descBuilder.AppendLine(" 毫秒");

                descBuilder.AppendLine();
                descBuilder.AppendLine("【查询语句】");
                descBuilder.Append('\t');
                descBuilder.AppendLine(this._CommandText);
                if(this._Parameters.Count > 0)
                {
                    descBuilder.AppendLine();
                    descBuilder.AppendLine("【参数集合】");
                    foreach(var item in this._Parameters)
                    {
                        descBuilder.Append('\t');
                        descBuilder.Append(item.Key);
                        descBuilder.Append(" = ");
                        descBuilder.AppendLine(item.Value);
                    }
                }
                descBuilder.AppendLine();
                if(this._ExceptionMessage == null)
                {
                    descBuilder.AppendLine("【返回内容】");
                    descBuilder.Append('\t');
                    descBuilder.AppendLine(this._Value);
                }
                else
                {
                    descBuilder.AppendLine("【异常信息】");
                    descBuilder.Append('\t');
                    descBuilder.AppendLine(this._ExceptionMessage);
                }
                descBuilder.Append(DefaultRepeatChar, DefaultRepeatCount); descBuilder.AppendLine();
                this._ToString = descBuilder.ToString();
            }
            return this._ToString;

        }
    }
}
