//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;

//namespace Aoite.ServiceModel
//{
//    /// <summary>
//    /// 表示一个验证的异常信息。
//    /// </summary>
//    public class ValidationException : Exception
//    {
//        private List<ValidationResult> _Results;
//        /// <summary>
//        /// 获取验证的结果集合。
//        /// </summary>
//        public List<ValidationResult> Results { get { return this._Results; } }

//        static string CreateMessage(List<ValidationResult> results)
//        {
//            StringBuilder builder = new StringBuilder();
//            builder.Append(results[0].ErrorMessage);
//            for(int i = 1; i < results.Count; i++)
//            {
//                builder.AppendLine().Append(results[i].ErrorMessage);
//            }
//            return builder.ToString();
//        }

//        internal ValidationException(List<ValidationResult> results)
//            : base(CreateMessage(results))
//        {
//            this._Results = results;
//        }
//    }
//}
