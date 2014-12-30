using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace Aoite.ServiceProcess
{
    /// <summary>
    /// 表示一个基于 <see cref="Aoite.Net.CommunicationBase"/> 的 Windows 服务的安装程序。
    /// </summary>
    [System.ComponentModel.RunInstaller(true)]
    public abstract class AsWindowsServiceInstaller : Installer
    {
        private Dictionary<string, AsWindowsService> _Services;
        /// <summary>
        /// 获取服务列表。
        /// </summary>
        public AsWindowsService[] Services { get { return this._Services.Values.ToArray(); } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.ServiceProcess.AsWindowsServiceInstaller"/> 类的新实例。
        /// </summary>
        public AsWindowsServiceInstaller()
        {
            var services = this.OnCreateServices();
            if(services == null || services.Length == 0) throw new ArgumentNullException("services");
            this._Services = services.ToDictionary<AsWindowsService, string>(s => s.ServiceName);
            var processInstaller = new ServiceProcessInstaller();
            processInstaller.Account = ServiceAccount.LocalSystem;
            this.Installers.Add(processInstaller);

            foreach(var service in services)
            {
                this.Installers.Add(service.GetInstaller());
            }
        }

        /// <summary>
        /// 创建服务列表时发生。
        /// </summary>
        /// <returns>返回一个服务的列表。</returns>
        protected abstract AsWindowsService[] OnCreateServices();

        private IEnumerable<ServiceController> GetServiceControllers()
        {
            foreach(var service in ServiceController.GetServices())
            {
                if(!this._Services.ContainsKey(service.ServiceName)) continue;
                yield return service;
            }
        }

        private void StopAllServices()
        {
            foreach(var service in this.GetServiceControllers())
            {
                if(service.Status == ServiceControllerStatus.Running)
                {
                    Console.WriteLine("正在停止服务 {0} ({1}) ...", service.ServiceName, service.DisplayName);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    Console.WriteLine("服务 {0} ({1}) 已被停止...", service.ServiceName, service.DisplayName);
                }
            }
        }

        private void StartAllServices()
        {
            foreach(var service in this.GetServiceControllers())
            {
                if(service.Status == ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine("正在启动服务 {0} ({1}) ...", service.ServiceName, service.DisplayName);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    Console.WriteLine("服务 {0} ({1}) 启动成功...", service.ServiceName, service.DisplayName);
                }
            }
        }

        /// <summary>
        /// 运行于控制台应用程序。
        /// </summary>
        public void RunAsConsoleApplication()
        {
            if(Environment.UserInteractive)
            {
                Console.Title = "服务管理器";
                if(!GA.IsAdministrator)
                {
                    Console.WriteLine("非管理员方式运行程序，在弹出的系统对话框中，请选择“是”...");
                    GA.RunAsAdministrator();
                    return;
                }

                var old = Console.ForegroundColor;
                while(true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("请输入执行命令：");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    var commands = GA.ToCommandLines(Console.ReadLine());
                    if(commands == null | commands.Length == 0) return;
                    var command = commands[0];
                    //var args = commands.Skip(1).ToArray();
                    if(string.IsNullOrEmpty(command)) return;
                    if(command[0] == '/' && command[0] == '-') command = command.Remove(0, 1);
                    Console.ForegroundColor = old;

                    switch(command.ToLower())
                    {
                        case "clear":
                            Console.Clear();
                            break;
                        case "help":
                        case "?":
                            Console.WriteLine("\t-i：");
                            Console.WriteLine("\t\t安装所有服务。");
                            Console.WriteLine("\t-u：");
                            Console.WriteLine("\t\t卸载所有服务。");
                            Console.WriteLine("\t-show：");
                            Console.WriteLine("\t\t显示所有服务的状态。");
                            Console.WriteLine("\t-start：");
                            Console.WriteLine("\t\t启动所有服务。");
                            Console.WriteLine("\t-stop：");
                            Console.WriteLine("\t\t停止所有服务。");
                            Console.WriteLine("\t-help 或 -?：");
                            Console.WriteLine("\t\t帮助说明。");
                            Console.WriteLine("\t-exit：");
                            Console.WriteLine("\t\t退出程序。");
                            break;
                        case "exit":
                            return;
                        case "show":
                            var ss = this.GetServiceControllers().ToArray();
                            if(ss.Length == 0)
                            {
                                Console.WriteLine("没有安装的服务...");
                                break;
                            }
                            foreach(var service in ss)
                            {
                                Console.WriteLine("\t服务标识名称：{0}", service.ServiceName);
                                Console.WriteLine("\t服务友好名称：{0}", service.DisplayName);
                                Console.WriteLine("\t计算机的名称：{0}", service.MachineName);
                                Console.WriteLine("\t服务运行状态：{0}", service.Status.ToString());
                                Console.WriteLine(new string('-', 20));
                            }
                            break;
                        case "start":
                            this.StartAllServices();
                            break;
                        case "stop":
                            this.StopAllServices();
                            break;
                        case "i":
                            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetEntryAssembly().Location });
                            this.StartAllServices();
                            break;
                        case "u":
                            this.StopAllServices();
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetEntryAssembly().Location });
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("无法识别命令：{0}，请注意全角或半角（是否打开了输入法？）...", command);
                            Console.ForegroundColor = old;
                            break;
                    }
                    Console.WriteLine();
                }
            }
            else
            {

                System.ServiceProcess.ServiceBase.Run(this.Services);
            }

        }
    }

}
