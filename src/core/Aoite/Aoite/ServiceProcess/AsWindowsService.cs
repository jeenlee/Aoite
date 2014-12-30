using Aoite.Net;
using System.ServiceProcess;

namespace Aoite.ServiceProcess
{
    /// <summary>
    /// 表示一个基于 <see cref="Aoite.Net.CommunicationBase"/> 的 Windows 服务。
    /// </summary>
    public class AsWindowsService : System.ServiceProcess.ServiceBase
    {
        /// <summary>
        /// 获取或设置通讯的基类。
        /// </summary>
        public ICommunication Communication { get; set; }

        /// <summary>
        /// 获取或设置指示向用户标识服务的友好名称。
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示是否应延迟启动该服务，直到运行其他自动启动的服务。
        /// </summary>
        public bool DelayedAutoStart { get; set; }
        /// <summary>
        /// 获取或设置服务的说明。
        /// </summary>
        public string Description { get; set; }

        private ServiceStartMode _StartType = ServiceStartMode.Automatic;
        /// <summary>
        /// 获取或设置指示启动此服务的方式和时间。
        /// </summary>
        public ServiceStartMode StartType { get { return this._StartType; } set { this._StartType = value; } }

        /// <summary>
        /// 初始化一个　<see cref="Aoite.ServiceProcess.AsWindowsService"/> 类的新实例。
        /// </summary>
        public AsWindowsService() { }

        /// <summary>
        /// 获取服务的安装程序。
        /// </summary>
        /// <returns>返回一个 <see cref="System.ServiceProcess.ServiceInstaller"/> 类的新实例。</returns>
        public virtual ServiceInstaller GetInstaller()
        {
            var serviceInstaller = new ServiceInstaller();
            serviceInstaller.ServiceName = this.ServiceName;
            serviceInstaller.DisplayName = this.DisplayName;
            serviceInstaller.DelayedAutoStart = this.DelayedAutoStart;
            serviceInstaller.Description = this.Description;
            serviceInstaller.StartType = this.StartType;

            return serviceInstaller;
        }

        /// <summary>
        /// 在下列情况下执行：在“服务控制管理器”(SCM) 向服务发送“开始”命令时，或者在操作系统启动时（对于自动启动的服务）。指定服务启动时采取的操作。
        /// </summary>
        /// <param name="args">启动命令传递的数据。</param>
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            var r = this.Communication.Open();
            if(r.IsFailed) throw r.Exception;
        }

        /// <summary>
        /// 方法于“服务控制管理器”(SCM) 将“停止”命令发送到服务时执行。指定服务停止运行时采取的操作。
        /// </summary>
        protected override void OnStop()
        {
            base.OnStop();
            this.Communication.Close();
        }
    }
}
