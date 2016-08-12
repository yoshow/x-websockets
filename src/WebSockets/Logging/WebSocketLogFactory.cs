using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SuperSocket.SocketBase.Logging;

namespace X3Platform.WebSockets.Logging
{
    /// <summary>
    /// Console log factory
    /// </summary>
    public class WebSocketLogFactory : ILogFactory
    {
        /// <summary>
        /// Gets the log by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ILog GetLog(string name)
        {
            return new WebSocketLog(Common.Logging.LogManager.GetLogger(name));
        }
    }
}
