// Apertus Brain Orchestrator — panel sterowania dla floty CODEX/Lokisia
// Łączy: mcp-gateway (HTTP:3845), IRC bus (TCP:6667), countdown do rozprawy IV RNs 301/26
using System;
using System.Windows.Forms;
using ADD.Tools;
using Newtonsoft.Json.Linq;

namespace ADD
{
    public partial class Orchestrator : Form
    {
        private MCPClient _mcp;
        private IRCFleet _irc;

        // Rozprawa apelacyjna IV RNs 301/26 — SO Gdańsk, 09.12.2026 godz. 14:45
        private static readonly DateTime HearingTime = new DateTime(2026, 12, 9, 14, 45, 0);

        public Orchestrator()
        {
            InitializeComponent();
            UpdateCountdown();
            tmrCountdown.Start();
        }

        private void UpdateCountdown()
        {
            var remaining = HearingTime - DateTime.Now;
            if (remaining.TotalSeconds <= 0)
            {
                lblCountdown.Text = "ROZPRAWA TRWA";
                lblCountdown.ForeColor = System.Drawing.Color.Red;
                return;
            }

            lblCountdown.Text = string.Format("{0}d {1}h {2}m",
                (int)remaining.TotalDays,
                remaining.Hours,
                remaining.Minutes);
        }

        private void tmrCountdown_Tick(object sender, EventArgs e)
        {
            UpdateCountdown();
        }

        private void btnConnectMCP_Click(object sender, EventArgs e)
        {
            if (_mcp != null && _mcp.IsConnected)
            {
                _mcp.Dispose();
                _mcp = null;
                UpdateMCPStatus(false, "Rozłączono");
                return;
            }

            AppendLog("MCP: łączę z localhost:3845...");
            _mcp = new MCPClient("localhost", 3845);

            var health = _mcp.CheckHealth();
            if (health == null)
            {
                AppendLog("MCP: gateway niedostępny — uruchom: cd /home/user/codex/mcp-gateway && npm start");
                UpdateMCPStatus(false, "Niedostępny");
                _mcp.Dispose();
                _mcp = null;
                return;
            }

            AppendLog("MCP: health OK — " + health);

            if (_mcp.Initialize())
            {
                UpdateMCPStatus(true, "Połączono");
                AppendLog("MCP: sesja aktywna");
            }
            else
            {
                AppendLog("MCP: błąd inicjalizacji — " + (_mcp.LastError ?? "unknown"));
                UpdateMCPStatus(false, "Błąd init");
                _mcp.Dispose();
                _mcp = null;
            }
        }

        private void btnConnectIRC_Click(object sender, EventArgs e)
        {
            if (_irc != null && _irc.IsConnected)
            {
                _irc.Dispose();
                _irc = null;
                UpdateIRCStatus(false, "Rozłączono");
                return;
            }

            AppendLog("IRC: łączę z localhost:6667...");
            _irc = new IRCFleet("APERTUS-BRAIN", "localhost", 6667);
            _irc.MessageReceived += IRC_MessageReceived;
            _irc.RawLineReceived += IRC_RawLineReceived;

            if (_irc.Connect())
            {
                UpdateIRCStatus(true, "Połączono");
                _irc.Join("#codex");
                _irc.Join("#fleet");
                AppendLog("IRC: dołączono do #codex #fleet");
            }
            else
            {
                AppendLog("IRC: błąd — " + (_irc.LastError ?? "unknown"));
                UpdateIRCStatus(false, "Błąd");
                _irc.Dispose();
                _irc = null;
            }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            if (_mcp == null || !_mcp.IsConnected)
            {
                AppendLog("Query: MCP nie podłączony");
                return;
            }

            var tool = cmbTool.Text.Trim();
            var action = txtAction.Text.Trim();
            var payload = txtPayload.Text.Trim();

            if (string.IsNullOrEmpty(tool) || string.IsNullOrEmpty(action))
            {
                AppendLog("Query: podaj tool i action");
                return;
            }

            AppendLog(string.Format("Query → {0}.{1} ...", tool, action));
            btnQuery.Enabled = false;

            JObject parameters = null;
            if (!string.IsNullOrEmpty(payload))
            {
                var providerVal = cmbProvider.Text.Trim();
                parameters = new JObject();
                if (!string.IsNullOrEmpty(providerVal)) parameters["provider"] = providerVal;
                parameters["params"] = new JObject { ["prompt"] = payload };
            }

            var result = _mcp.CallTool(tool, action, parameters);
            btnQuery.Enabled = true;

            if (result == null)
            {
                AppendLog("Query: błąd — " + (_mcp.LastError ?? "null response"));
                return;
            }

            AppendLog("Response:");
            AppendLog(result.ToString(Newtonsoft.Json.Formatting.Indented));
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            rtbOutput.Clear();
        }

        private void IRC_MessageReceived(string nick, string channel, string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => IRC_MessageReceived(nick, channel, text)));
                return;
            }
            AppendLog(string.Format("IRC [{0}] <{1}>: {2}", channel, nick, text));
        }

        private void IRC_RawLineReceived(string line)
        {
            // Log only non-PING lines to reduce noise
            if (!line.StartsWith("PING", StringComparison.Ordinal))
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => AppendLog("IRC raw: " + line)));
                    return;
                }
                // Filter out server noise — only log user-visible events
                if (line.Contains("JOIN") || line.Contains("PART") || line.Contains("NOTICE"))
                    AppendLog("IRC: " + line);
            }
        }

        private void UpdateMCPStatus(bool connected, string status)
        {
            lblMCPStatus.Text = "MCP: " + status;
            lblMCPStatus.ForeColor = connected
                ? System.Drawing.Color.LimeGreen
                : System.Drawing.Color.OrangeRed;
            btnConnectMCP.Text = connected ? "Rozłącz MCP" : "Połącz MCP";
        }

        private void UpdateIRCStatus(bool connected, string status)
        {
            lblIRCStatus.Text = "IRC: " + status;
            lblIRCStatus.ForeColor = connected
                ? System.Drawing.Color.LimeGreen
                : System.Drawing.Color.OrangeRed;
            btnConnectIRC.Text = connected ? "Rozłącz IRC" : "Połącz IRC";
        }

        private void AppendLog(string text)
        {
            var ts = DateTime.Now.ToString("HH:mm:ss");
            rtbOutput.AppendText(string.Format("[{0}] {1}\n", ts, text));
            rtbOutput.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _mcp?.Dispose();
            _irc?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
