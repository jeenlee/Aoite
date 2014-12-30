using System;
using System.Collections.Generic;
using System.Text;
namespace Aoite
{
    /// <summary>
    /// Aoite 框架的程序集描述。
    /// </summary>
    public static class AoiteInfo
    {
        //<开发阶段>
        //- Pre-alpha：功能不完整的版本。
        //- Alpha：仍然需要测试，其功能亦未完善。
        //- Beta：第一个对外公开的软件版本，是由公众参与的测试阶段。
        //- RC：最终产品的候选版本，如果未出现问题则可发布成为正式版本。
        //<发布阶段>
        //- RTM：可用于生产环境的版本，又称为正式版。
        //- Stable：修复问题，扩展功能，优化性能。

        internal const string Life = "Beta";
        internal const string Name = "Aoite";

        /// <summary> 
        /// 程序集的产品名称。
        /// </summary>
        public const string Product = Name + " " + Life;
        /// <summary> 
        /// 获取简写化的版本号。
        /// </summary>
        public const string Version = "2.0";
        /// <summary> 
        /// 程序集详细的版本。
        /// </summary>
        public const string AssemblyVersion = Version + ".0.0";
        /// <summary>
        /// 公司。
        /// </summary>
        public const string Company = Name + " Co.,";
        /// <summary> 
        /// 版权。
        /// </summary>
        public const string Copyright = "Copyright © " + Name + " 2014 All Right Reserved";
        /// <summary> 
        /// 程序集的简单描述。
        /// </summary>
        public const string Description = Name + "[A]ny[o]ne [it]ems suit[e] for .NET Framwork 4.0 Runtime 开发套件，详见：http://www.aoite.com。";
        /// <summary> 
        /// 程序集的唯一标识。
        /// </summary>
        public const string Guid = "01c9c125-67fb-46af-b50a-0c96bdcb3775";

        /// <summary> 
        /// Aoite Windows Forms 基础套件。
        /// </summary>
        public static class Windows
        {
            /// <summary>
            /// 程序集的产品名称。
            /// </summary>
            public const string Product = AoiteInfo.Name + " Windows " + Life;
            /// <summary>
            /// 程序集的唯一标识。
            /// </summary>
            public const string Guid = "14d7026d-bd80-4191-8b87-56046b7ad731";
            /// <summary> 
            /// 程序集详细的版本。
            /// </summary>
            public const string AssemblyVersion = Version + ".0.0";
            /// <summary>
            /// 程序集的简单描述。
            /// </summary>
            public const string Description = AoiteInfo.Name + " Windows Forms 基础套件。";
        }

        /// <summary> 
        /// Aoite ASP.NET Web 基础套件。
        /// </summary>
        public static class Web
        {
            /// <summary>
            /// 程序集的产品名称。
            /// </summary>
            public const string Product = AoiteInfo.Name + " Web " + Life;
            /// <summary>
            /// 程序集的简单描述。
            /// </summary>
            public const string Description = AoiteInfo.Name + " ASP.NET Web 基础套件。";
            /// <summary> 
            /// 程序集详细的版本。
            /// </summary>
            public const string AssemblyVersion = Version + ".0.0";
            /// <summary> 
            /// 程序集的唯一标识。
            /// </summary>
            public const string Guid = "03356e94-5514-49d5-8eb2-092bef66b743";
        }
    }
}