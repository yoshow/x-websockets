using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.IO;

using X3Platform;
using X3Platform.Configuration;
using X3Platform.Services;
using X3Platform.Services.Configuration;
using X3Platform.Util;

namespace X3Platform.WebSockets.ServiceObservers
{
    /// <summary>WebSocket 服务监听器</summary>
    public class WebSocketServiceObserver : IServiceObserver
    {
        #region 属性:Name
        private string m_Name;

        public string Name
        {
            get { return m_Name; }
        }
        #endregion

        private bool running = false;

        /// <summary>是否正在运行</summary>
        public bool IsRunning
        {
            get { return running; }
        }

        #region 属性:Sleeping
        public bool Sleeping
        {
            get { return (nextRunTime > DateTime.Now) ? true : false; }
        }
        #endregion

        public WebSocketServiceObserver()
            : this("WebSocketService", string.Empty)
        {
        }

        public WebSocketServiceObserver(string name, string args)
        {
            this.m_Name = name;

            Reload();
        }

        private ServicesConfiguration configuration = null;

        /// <summary>上一次执行的结束时间</summary>
        private DateTime nextRunTime = DateHelper.DefaultTime;

        // 单位(小时)
        private int runTimeInterval = 1;

        // 跟踪运行时间
        private bool trackRunTime = false;

        public void Reload()
        {
            this.configuration = ServicesConfigurationView.Instance.Configuration;

            this.trackRunTime = ServicesConfigurationView.Instance.TrackRunTime;

            this.nextRunTime = this.configuration.Observers[this.Name].NextRunTime;
        }

        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            if (running)
                return;

            try
            {
                if (nextRunTime < DateTime.Now)
                {
                    nextRunTime = DateTime.Now.AddHours(runTimeInterval);

                    this.configuration.Observers[this.Name].NextRunTime = nextRunTime;

                    ServicesConfigurationView.Instance.Save();
                }
            }
            catch (Exception ex)
            {
                // 发生异常是, 记录异常信息  并把运行标识为false.

                KernelContext.Log.Info(ex);

                EventLogHelper.Write(string.Format("{0}服务发生异常信息\r\n{1}", this.Name, ex.ToString()));

                running = false;
            }
        }

        WebSocketService service = new WebSocketService();

        public void Start()
        {
            service.Start();
        }

        public void Close()
        {
            EventLogHelper.Write(string.Format("{0}服务正在停止。", this.Name));

            service.Stop();
        }
    }
}