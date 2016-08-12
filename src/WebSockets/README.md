配置文件中加入 SuperSocket.WebSocket 配置

<section name="superSocket" type="SuperSocket.SocketEngine.Configuration.SocketServiceConfig, SuperSocket.SocketEngine" />

<superSocket logFactory="WebSocketLogFactory" disablePerformanceDataCollector="true" >
  <servers>
    <server name="WebSocketServer"
            serverTypeName="WebSocketServer"
            maxConnectionNumber="1000"
            maxRequestLength="4096"
            sendTimeOut="300000">
      <listeners>
        <add ip="Any" port="10089" />
      </listeners>
    </server>
  </servers>
  <serverTypes>
    <add name="WebSocketServer"
         type="SuperSocket.WebSocket.WebSocketServer,SuperSocket.WebSocket" />
  </serverTypes>
  <logFactories>
      <add name="WebSocketLogFactory"
          type="Cloudwalk.BigDb.WebSockets.Logging.WebSocketLogFactory, Cloudwalk.BigDb.WebSockets" />
  </logFactories>
</superSocket>