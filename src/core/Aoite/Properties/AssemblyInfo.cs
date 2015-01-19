using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的常规信息通过以下
// 特性集控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle(Aoite.AoiteInfo.Product + " " + Aoite.AoiteInfo.Version)]
[assembly: AssemblyDescription(Aoite.AoiteInfo.Description)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(Aoite.AoiteInfo.Company)]
[assembly: AssemblyProduct(Aoite.AoiteInfo.Product)]
[assembly: AssemblyCopyright(Aoite.AoiteInfo.Copyright)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid(Aoite.AoiteInfo.Guid)]

// 程序集的版本信息由下面四个值组成:
//
//      主版本
//      次版本
//      内部版本号
//      修订号
//
// 可以指定所有这些值，也可以使用“内部版本号”和“修订号”的默认值，
// 方法是按如下所示使用“*”:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(Aoite.AoiteInfo.AssemblyVersion)]
[assembly: AssemblyFileVersion(Aoite.AoiteInfo.AssemblyVersion)]
#if DEBUG
[assembly: InternalsVisibleTo("Aoite.Tests,PublicKey="
+ "00240000048000009400000006020000002400005253413100040000010001005d95cfd539e6d0"
+ "702b5f60592328a570d7df30f7a8426e7cec04cf0c722f00a420e623a0d81d5dd44032cff3d860"
+ "844eeeed057ece8c2dc99a05b945dc7f892aae2d584faaff512b80190367863554bed506654919"
+ "1e07a5a70f87e1da3a1944ddbb4fbffaa71d47bde5240fb14496874197412ae47bc26b864fdb86"
+ "fc2340a2")]

[assembly: InternalsVisibleTo("Aoite.Redis.Tests,PublicKey="
+ "00240000048000009400000006020000002400005253413100040000010001005d95cfd539e6d0"
+ "702b5f60592328a570d7df30f7a8426e7cec04cf0c722f00a420e623a0d81d5dd44032cff3d860"
+ "844eeeed057ece8c2dc99a05b945dc7f892aae2d584faaff512b80190367863554bed506654919"
+ "1e07a5a70f87e1da3a1944ddbb4fbffaa71d47bde5240fb14496874197412ae47bc26b864fdb86"
+ "fc2340a2")]
#endif