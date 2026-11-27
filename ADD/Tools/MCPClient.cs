// MCP Gateway client — HTTP bridge from Apertus to CODEX mcp-gateway (port 3845)
// Protocol: JSON-RPC 2.0 over StreamableHTTP (MCP spec 2024-11-05)
using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ADD.Tools
{
    public class MCPClient : IDisposable
    {
        private readonly string _baseUrl;
        private readonly string _healthUrl;
        private string _sessionId;
        private int _msgId;

        public bool IsConnected { get; private set; }
        public string LastError { get; private set; }

        public MCPClient(string host = "localhost", int port = 3845)
        {
            _baseUrl = string.Format("http://{0}:{1}/mcp", host, port);
            _healthUrl = string.Format("http://{0}:{1}/health", host, port);
        }

        public bool Initialize()
        {
            var req = new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = ++_msgId,
                ["method"] = "initialize",
                ["params"] = new JObject
                {
                    ["protocolVersion"] = "2024-11-05",
                    ["capabilities"] = new JObject(),
                    ["clientInfo"] = new JObject { ["name"] = "Apertus", ["version"] = "1.0" }
                }
            };

            string sessionId;
            var resp = Post(req.ToString(), null, out sessionId);
            if (resp == null) return false;

            _sessionId = sessionId;

            // Notify gateway that initialization is complete
            var notif = new JObject
            {
                ["jsonrpc"] = "2.0",
                ["method"] = "notifications/initialized"
            };
            string dummy;
            Post(notif.ToString(), _sessionId, out dummy);

            IsConnected = true;
            return true;
        }

        // Call a lokis_* meta-tool. Returns parsed JSON result or null on failure.
        public JObject CallTool(string toolName, string action, JObject parameters = null)
        {
            if (!IsConnected)
            {
                LastError = "Not connected. Call Initialize() first.";
                return null;
            }

            var args = new JObject { ["action"] = action };
            if (parameters != null)
                foreach (var prop in parameters.Properties())
                    args[prop.Name] = prop.Value;

            var req = new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = ++_msgId,
                ["method"] = "tools/call",
                ["params"] = new JObject
                {
                    ["name"] = toolName,
                    ["arguments"] = args
                }
            };

            string dummy;
            var resp = Post(req.ToString(), _sessionId, out dummy);
            if (resp == null) return null;

            try { return JObject.Parse(resp); }
            catch (Exception ex) { LastError = ex.Message; return null; }
        }

        // Check if the gateway process is running. Returns health JSON string or null.
        public string CheckHealth()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(_healthUrl);
                request.Method = "GET";
                request.Timeout = 3000;
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
        }

        private string Post(string body, string sessionId, out string newSessionId)
        {
            newSessionId = null;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(_baseUrl);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json, text/event-stream";
                request.Timeout = 15000;
                if (sessionId != null)
                    request.Headers["mcp-session-id"] = sessionId;

                var bytes = Encoding.UTF8.GetBytes(body);
                request.ContentLength = bytes.Length;
                using (var stream = request.GetRequestStream())
                    stream.Write(bytes, 0, bytes.Length);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    newSessionId = response.Headers["mcp-session-id"];
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
        }

        public void Dispose()
        {
            if (_sessionId != null && IsConnected)
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(_baseUrl);
                    request.Method = "DELETE";
                    request.Headers["mcp-session-id"] = _sessionId;
                    request.Timeout = 3000;
                    request.GetResponse().Dispose();
                }
                catch { }
                IsConnected = false;
            }
        }
    }
}
