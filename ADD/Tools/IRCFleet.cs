// IRC fleet bus client — connects Apertus to the CODEX IRC bus (localhost:6667)
// Fleet coordinates 6+ AI agents via IRC channels; Apertus joins as APERTUS-BRAIN
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ADD.Tools
{
    public class IRCFleet : IDisposable
    {
        private readonly string _host;
        private readonly int _port;
        private TcpClient _tcp;
        private StreamReader _reader;
        private StreamWriter _writer;
        private Thread _readThread;
        private bool _running;

        public string Nick { get; }
        public bool IsConnected { get { return _tcp != null && _tcp.Connected && _running; } }
        public string LastError { get; private set; }

        // nick = channel, message text
        public event Action<string, string, string> MessageReceived;
        public event Action<string> RawLineReceived;

        public IRCFleet(string nick = "APERTUS-BRAIN", string host = "localhost", int port = 6667)
        {
            Nick = nick;
            _host = host;
            _port = port;
        }

        public bool Connect()
        {
            try
            {
                _tcp = new TcpClient();
                _tcp.Connect(_host, _port);
                _tcp.ReceiveTimeout = 120000;
                _tcp.SendTimeout = 5000;

                var stream = _tcp.GetStream();
                _reader = new StreamReader(stream, Encoding.UTF8);
                _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true, NewLine = "\r\n" };

                _writer.WriteLine("NICK " + Nick);
                _writer.WriteLine("USER " + Nick + " 0 * :Apertus Orchestrator Brain");

                _running = true;
                _readThread = new Thread(ReadLoop) { IsBackground = true, Name = "IRCFleetRead" };
                _readThread.Start();
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public void Join(string channel)
        {
            if (IsConnected) _writer.WriteLine("JOIN " + channel);
        }

        public void Send(string target, string message)
        {
            if (IsConnected) _writer.WriteLine("PRIVMSG " + target + " :" + message);
        }

        // Broadcast a query to the fleet channel and return immediately.
        // Format: APERTUS:QUERY:<tool>:<action>:<payload>
        public void QueryFleet(string channel, string tool, string action, string payload)
        {
            Send(channel, string.Format("APERTUS:QUERY:{0}:{1}:{2}", tool, action, payload));
        }

        private void ReadLoop()
        {
            while (_running)
            {
                try
                {
                    var line = _reader.ReadLine();
                    if (line == null) break;

                    var handler = RawLineReceived;
                    if (handler != null) handler(line);

                    HandleLine(line);
                }
                catch (IOException) { break; }
                catch { break; }
            }
            _running = false;
        }

        private void HandleLine(string line)
        {
            // PING :server → PONG :server  (keeps connection alive)
            if (line.StartsWith("PING", StringComparison.Ordinal))
            {
                _writer.WriteLine(line.Replace("PING", "PONG"));
                return;
            }

            // :nick!user@host PRIVMSG #channel :text
            var privIdx = line.IndexOf(" PRIVMSG ", StringComparison.Ordinal);
            if (privIdx < 0) return;

            var prefix = line.Substring(0, privIdx);
            var rest = line.Substring(privIdx + 9); // skip " PRIVMSG "
            var spaceIdx = rest.IndexOf(' ');
            if (spaceIdx < 0) return;

            var nick = prefix.TrimStart(':');
            var bangIdx = nick.IndexOf('!');
            if (bangIdx >= 0) nick = nick.Substring(0, bangIdx);

            var target = rest.Substring(0, spaceIdx);
            var text = rest.Substring(spaceIdx + 1).TrimStart(':');

            var msgHandler = MessageReceived;
            if (msgHandler != null) msgHandler(nick, target, text);
        }

        public void Dispose()
        {
            _running = false;
            try { _tcp?.Close(); } catch { }
        }
    }
}
