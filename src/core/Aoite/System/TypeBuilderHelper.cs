using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace System
{
    internal class TypeBuilderHelper
    {
        internal const PropertyAttributes PublicProperty = PropertyAttributes.HasDefault;
        private static readonly Dictionary<Type, OpCode> LdindOpCodeTypeMap;
        private static readonly Dictionary<Type, OpCode> StindOpCodeTypeMap;

        internal static readonly AppDomain Domain = System.Threading.Thread.GetDomain();
        internal static readonly AssemblyBuilder Assembly = Domain.DefineDynamicAssembly(new AssemblyName("AOITE_Assembly"), AssemblyBuilderAccess.Run);//AndSave,GA.AppDirectory
        internal static readonly ModuleBuilder Module = Assembly.DefineDynamicModule("AOITE_Module", true);
        static TypeBuilderHelper()
        {
            // 加载指定索引（在这之前已经 POP）的值作为 XXX 类型读取出来
            LdindOpCodeTypeMap = new Dictionary<Type, OpCode>(12);
            LdindOpCodeTypeMap.Add(typeof(Boolean), OpCodes.Ldind_I1);
            LdindOpCodeTypeMap.Add(typeof(Byte), OpCodes.Ldind_U1);
            LdindOpCodeTypeMap.Add(typeof(SByte), OpCodes.Ldind_I1);
            LdindOpCodeTypeMap.Add(typeof(Int16), OpCodes.Ldind_I2);
            LdindOpCodeTypeMap.Add(typeof(UInt16), OpCodes.Ldind_U2);
            LdindOpCodeTypeMap.Add(typeof(Int32), OpCodes.Ldind_I4);
            LdindOpCodeTypeMap.Add(typeof(UInt32), OpCodes.Ldind_U4);
            LdindOpCodeTypeMap.Add(typeof(Int64), OpCodes.Ldind_I8);
            LdindOpCodeTypeMap.Add(typeof(UInt64), OpCodes.Ldind_I8);
            LdindOpCodeTypeMap.Add(typeof(Char), OpCodes.Ldind_U2);
            LdindOpCodeTypeMap.Add(typeof(Double), OpCodes.Ldind_R8);
            LdindOpCodeTypeMap.Add(typeof(Single), OpCodes.Ldind_R4);

            // 同上，只不过作为 存储。
            StindOpCodeTypeMap = new Dictionary<Type, OpCode>(12);
            StindOpCodeTypeMap.Add(typeof(Boolean), OpCodes.Stind_I1);
            StindOpCodeTypeMap.Add(typeof(Byte), OpCodes.Stind_I1);
            StindOpCodeTypeMap.Add(typeof(SByte), OpCodes.Stind_I1);
            StindOpCodeTypeMap.Add(typeof(Int16), OpCodes.Stind_I2);
            StindOpCodeTypeMap.Add(typeof(UInt16), OpCodes.Stind_I2);
            StindOpCodeTypeMap.Add(typeof(Int32), OpCodes.Stind_I4);
            StindOpCodeTypeMap.Add(typeof(UInt32), OpCodes.Stind_I4);
            StindOpCodeTypeMap.Add(typeof(Int64), OpCodes.Stind_I8);
            StindOpCodeTypeMap.Add(typeof(UInt64), OpCodes.Stind_I8);
            StindOpCodeTypeMap.Add(typeof(Char), OpCodes.Stind_I2);
            StindOpCodeTypeMap.Add(typeof(Double), OpCodes.Stind_R8);
            StindOpCodeTypeMap.Add(typeof(Single), OpCodes.Stind_R4);

        }

        private static void LoadOrStoreIndexByType(ILGenerator mIL, Type type, Dictionary<Type, OpCode> dict, OpCode def)
        {
            OpCode code;
            if(dict.TryGetValue(type, out code))
            {
                mIL.Emit(code);
            }
            else
            {
                mIL.Emit(def, type);// 对其他值类型进行存储或加载
            }
        }

        public static void LoadIndexByType(ILGenerator mIL, Type type)
        {
            LoadOrStoreIndexByType(mIL, type, LdindOpCodeTypeMap, OpCodes.Ldobj);
        }

        public static void StoreIndexByType(ILGenerator mIL, Type type)
        {
            LoadOrStoreIndexByType(mIL, type, StindOpCodeTypeMap, OpCodes.Stobj);
        }
    }
}
