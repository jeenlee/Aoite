using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Data
{
    internal class SelectCommandBuilder : CommandBuilderBase, ISelect
    {
        private string _fromTables;
        private List<string> _select, _orderby, _groupby;
        internal WhereCommandBuilder _whereCommandBuilder;
        internal StringBuilder _whereBuilder;
        private ExecuteParameterCollection _parameters;
        public override ExecuteParameterCollection Parameters
        {
            get
            {
                if(_parameters == null) _parameters = new ExecuteParameterCollection(0);
                return this._parameters;
            }
        }

        public ISelect Parameter(string name, object value)
        {
            this.Parameters.Add(name, value);
            return this;
        }

        public override string CommandText
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("SELECT ");
                if(this._select == null) builder.Append("*");
                else this.AppendFields(builder, this._select.ToArray());

                builder.Append(" FROM ");
                builder.Append(this._fromTables);
                if(this._whereBuilder != null && this._whereBuilder.Length > 0)
                {
                    builder.Append(" WHERE ");
                    builder.Append(_whereBuilder.ToString());
                }
                if(this._groupby != null)
                {
                    builder.Append(" GROUP BY ");
                    this.AppendFields(builder, this._groupby.ToArray());
                }
                if(this._orderby != null)
                {
                    builder.Append(" ORDER BY ");
                    this.AppendFields(builder, this._orderby.ToArray());
                }
                return builder.ToString();
            }
        }

        internal SelectCommandBuilder(IDbEngine engine) : base(engine) { }

        public ISelect Select(params string[] fields)
        {
            if(fields == null || fields.Length == 0) return this;
            if(_select == null) _select = new List<string>(fields.Length);
            _select.AddRange(fields);
            return this;
        }

        public ISelect From(string fromTables)
        {
            this._fromTables = fromTables;
            return this;
        }

        public ISelect OrderBy(params string[] fields)
        {
            if(fields == null || fields.Length == 0) return this;
            if(_orderby == null) _orderby = new List<string>(fields.Length);
            _orderby.AddRange(fields);
            return this;
        }

        public ISelect GroupBy(params string[] fields)
        {
            if(fields == null || fields.Length == 0) return this;
            if(_groupby == null) _groupby = new List<string>(fields.Length);
            _groupby.AddRange(fields);
            return this;
        }

        public IWhere Where()
        {
            if(_whereCommandBuilder == null) _whereCommandBuilder = new WhereCommandBuilder(this);
            return _whereCommandBuilder;
        }

        public IWhere Where(string expression)
        {
            return this.Where().And(expression);
        }


        public IWhere Where<T>(string fieldName, string namePrefix, T[] values)
        {
            return this.Where().And<T>(fieldName, namePrefix, values);
        }

        public IWhere Where(string expression, string name, object value)
        {
            return this.Where().And(expression, name, value);
        }

        public IWhere WhereIn<T>(string fieldName, string namePrefix, T[] values)
        {
            return this.Where().AndIn<T>(fieldName, namePrefix, values);
        }
        public IWhere WhereNotIn<T>(string fieldName, string namePrefix, T[] values)
        {
            return this.Where().AndNotIn<T>(fieldName, namePrefix, values);
        }


        private void AppendFields(StringBuilder builder, string[] fields)
        {
            if(fields == null || fields.Length == 0) return;
            builder.Append(fields[0]);
            for(int i = 1; i < fields.Length; i++)
            {
                builder.Append(", ");
                builder.Append(fields[i]);
            }
        }

        public override ExecuteCommand End()
        {
            if(this._parameters != null) return new ExecuteCommand(this.CommandText, this._parameters);
            return new ExecuteCommand(this.CommandText);
        }
    }
}
