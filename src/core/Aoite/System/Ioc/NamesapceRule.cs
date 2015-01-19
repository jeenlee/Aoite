using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个命名空间的规则。
    /// </summary>
    public class NamesapceRule
    {
        private string _Expression;
        /// <summary>
        /// 获取一个命名空间的规则表达式。
        /// </summary>
        public string Expression { get { return this._Expression; } }

        private NamesapceRuleMode _Mode;
        /// <summary>
        /// 获取一个命名空间的规则模式。
        /// </summary>
        public NamesapceRuleMode Mode { get { return this._Mode; } }

        internal NamesapceRule(string expression)
        {
            if(expression.EndsWith("*"))
            {
                expression = expression.RemoveEnds();
                this._Mode = NamesapceRuleMode.StartsWith;
            }
            else if(expression.StartsWith("*"))
            {
                expression = expression.Remove(0, 1);
                this._Mode = NamesapceRuleMode.EndsWith;
            }

            if(string.IsNullOrEmpty(expression)) throw new ArgumentOutOfRangeException("expression");

            this._Expression = expression;
        }

        internal bool IsMatch(string @namespace)
        {
            bool isEquals = @namespace.iEquals(this._Expression);
            switch(this._Mode)
            {
                case NamesapceRuleMode.StartsWith:
                    return isEquals || @namespace.iStartsWith(this._Expression);

                case NamesapceRuleMode.EndsWith:
                    return isEquals || @namespace.iEndsWith(this._Expression);

                default:
                    return isEquals;
            }
        }
    }
}
