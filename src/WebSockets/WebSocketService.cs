namespace X3Platform.WebSockets
{
    using System;
    using System.Linq;

    using Common.Logging;

    using SuperSocket.SocketBase;
    using SuperSocket.SocketEngine;
    using SuperSocket.WebSocket;

    using X3Platform.Json;
    using X3Platform.Util;

    public class WebSocketService
    {
        private ILog logger = LogManager.GetLogger(typeof(WebSocketService));
        
        // web socket server 
        private IBootstrap m_Bootstrap;

        private WebSocketServer m_WebSocketServer;
        
        public void Start()
        {
            logger.Info("Starting WebSocket.");

            m_Bootstrap = BootstrapFactory.CreateBootstrap();

            if (!m_Bootstrap.Initialize())
            {
                return;
            }

            var socketServer = m_Bootstrap.AppServers.FirstOrDefault(s => s.Name.Equals("WebSocketServer")) as WebSocketServer;

            socketServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(socketServer_NewMessageReceived);
            socketServer.NewSessionConnected += socketServer_NewSessionConnected;
            socketServer.SessionClosed += socketServer_SessionClosed;

            m_WebSocketServer = socketServer;

            // start the socket
            m_Bootstrap.Start();

            logger.Info("Started WebSocket.");
        }

        public void Stop()
        {
            // shut down the scheduler
            logger.Info("Stopping WebSocket.");

            if (m_Bootstrap != null)
                m_Bootstrap.Stop();

            logger.Info("Stopped WebSocket.");
        }

        void socketServer_NewMessageReceived(WebSocketSession session, string message)
        {
            JsonData data = JsonMapper.ToObject(message);

            string command = JsonHelper.GetDataValue(data, "command");

            if (command == "echo")
            {
                data["text"] = session.Cookies["accountName"] + " " + DateTime.Now.ToString("HH:mm:ss") + " " + JsonHelper.GetDataValue(data, "text");
            }
            
            Send(session, data.ToJson());
        }

        void socketServer_NewSessionConnected(WebSocketSession session)
        {
            // SendToAll("[系统] " + session.Cookies["accountName"] + " 已连接.");
            Send(session, "{\"command\":\"echo\", \"text\":\"" + session.Cookies["accountName"] + " 已连接.\", \"widget\":\"[系统]\"}");
        }

        void socketServer_SessionClosed(WebSocketSession session, CloseReason reason)
        {
            if (reason == CloseReason.ServerShutdown)
                return;
            // 
            Send(session, "{\"command\":\"echo\", \"text\":\"" + session.Cookies["accountName"] + " 已断开.\", \"widget\":\"[系统]\"}");
        }

        void Send(WebSocketSession session, string response)
        {
            if (this.m_WebSocketServer == null) return;

            var sessions = m_WebSocketServer.GetAllSessions();

            if (sessions == null) return;

            JsonData data = JsonMapper.ToObject(response);

            string widget = JsonHelper.GetDataValue(data, "widget");

            string command = JsonHelper.GetDataValue(data, "command");

            string text = JsonHelper.GetDataValue(data, "text");

            foreach (var item in sessions)
            {
                if (item.Cookies["widget"] != null && session.Cookies["widget"].IndexOf(widget) > -1)
                {

                }

                item.Send(response);
            }
        }
    }
}
