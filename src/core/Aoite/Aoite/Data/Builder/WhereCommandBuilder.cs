using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Data
{
    internal class WhereCommandBuilder : CommandBuilderBase, IWhere
    {
        SelectCommandBuilder _selectCommandBuilder;
        public override ExecuteParameterCollection Parameters
        {
            get
            {
                return _selectCommandBuilder.Parameters;
            }
        }

        public override string CommandText
        {
            get
            {
                return _selectCommandBuilder.CommandText;
            }
        }

        internal WhereCommandBuilder(SelectCommandBuilder selectBuilder)
            : base(selectBuilder._engine)
        {
            this._selectCommandBuilder = selectBuilder;
            this._selectCommandBuilder._whereBuilder = new StringBuilder();
        }

        public IWhere Sql(string content)
        {
            this._selectCommandBuilder._whereBuilder.Append(content);
            return this;
        }

        public IWhere Parameter(string name, object value)
        {
            this._selectCommandBuilder.Parameter(name, value);
            return this;
        }

        public IWhere And()
        {
            if(this._selectCommandBuilder._whereBuilder.Length > 0) this.Sql(" AND ");
            return this;
        }

        public IWhere Or()
        {
            if(this._selectCommandBuilder._whereBuilder.Length > 0) this.Sql(" OR ");
            return this;
        }

        private void Append(string expression, string name, object value)
        {
            this.Sql(expression);
            this.Parameter(name, value);
        }

        private void Append<T>(string fieldName, string namePrefix, T[] values)
        {
            if(values == null || values.Length == 0) throw new ArgumentNullException("values");

            this.BeginGroup();
            int index = 0;
            fieldName = fieldName + "=";
            this.Append(fieldName + namePrefix + index, namePrefix + index, values[index]);
            for(index++; index < values.Length; index++)
            {
                this.Sql(" OR ");
                this.Append(fieldName + namePrefix + index, namePrefix + index, values[index]);
            }
            this.EndGroup();
        }

        private void Append<T>(bool isNotIn, string fieldName, string namePrefix, T[] values)
        {
            if(values == null || values.Length == 0) throw new ArgumentNullException("values");
            this.Sql(fieldName);
            this.Sql(isNotIn ? " NOT IN " : " IN ");
            this.BeginGroup();
            int index = 0;
            this.Append(namePrefix + index, namePrefix + index, values[index]);
            for(index++; index < values.Length; index++)
            {
                this.Sql(", ");
                this.Append(namePrefix + index, namePrefix + index, values[index]);
            }
            this.EndGroup();
        }

        public IWhere BeginGroup()
        {
            this.Sql("(");
            return this;
        }

        public IWhere BeginGroup(string expression, string name, object value)
        {
            this.BeginGroup();
            this.Append(expression, name, value);
            return this;
        }

        public IWhere EndGroup()
        {
            this.Sql(")");
            return this;
        }

        public override ExecuteCommand End()
        {
            return this._selectCommandBuilder.End();
        }

        public IWhere And(string expression)
        {
            return this.And().Sql(expression);
        }

        public IWhere And(string expression, string name, object value)
        {
            this.And();
            this.Append(expression, name, value);
            return this;
        }

        public IWhere Or(string expression)
        {
            return this.Or().Sql(expression);
        }
        public IWhere Or(string expression, string name, object value)
        {
            this.Or();
            this.Append(expression, name, value);
            return this;
        }

        public IWhere And<T>(string fieldName, string namePrefix, T[] values)
        {
            this.And();
            this.Append<T>(fieldName, namePrefix, values);
            return this;
        }

        public IWhere Or<T>(string fieldName, string namePrefix, T[] values)
        {
            this.Or();
            this.Append<T>(fieldName, namePrefix, values);
            return this;
        }

        public IWhere AndIn<T>(string fieldName, string namePrefix, T[] values)
        {
            this.And();
            this.Append(false, fieldName, namePrefix, values);
            return this;
        }
        public IWhere AndNotIn<T>(string fieldName, string namePrefix, T[] values)
        {
            this.And();
            this.Append(true, fieldName, namePrefix, values);
            return this;
        }

        public IWhere OrIn<T>(string fieldName, string namePrefix, T[] values)
        {
            this.Or();
            this.Append(false, fieldName, namePrefix, values);
            return this;
        }
        public IWhere OrNotIn<T>(string fieldName, string namePrefix, T[] values)
        {
            this.Or();
            this.Append(true, fieldName, namePrefix, values);
            return this;
        }

        public override ISelect OrderBy(params string[] fields)
        {
            return _selectCommandBuilder.OrderBy(fields);
        }
        public override ISelect GroupBy(params string[] fields)
        {
            return _selectCommandBuilder.GroupBy(fields);
        }
        public ISelect Select
        {
            get
            {
                return _selectCommandBuilder;
            }
        }


    }
}
