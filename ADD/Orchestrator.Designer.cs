namespace ADD
{
    partial class Orchestrator
    {
        private System.ComponentModel.IContainer components = null;

        // Status group (top-left)
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.Label lblCountdownTitle;
        private System.Windows.Forms.Label lblCountdown;
        private System.Windows.Forms.Label lblHearingDate;
        private System.Windows.Forms.Label lblMCPStatus;
        private System.Windows.Forms.Label lblIRCStatus;
        private System.Windows.Forms.Button btnConnectMCP;
        private System.Windows.Forms.Button btnConnectIRC;

        // Query group (top-right)
        private System.Windows.Forms.GroupBox grpQuery;
        private System.Windows.Forms.Label lblTool;
        private System.Windows.Forms.ComboBox cmbTool;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.TextBox txtAction;
        private System.Windows.Forms.Label lblProvider;
        private System.Windows.Forms.ComboBox cmbProvider;
        private System.Windows.Forms.Label lblPayload;
        private System.Windows.Forms.TextBox txtPayload;
        private System.Windows.Forms.Button btnQuery;

        // Output group (bottom)
        private System.Windows.Forms.GroupBox grpOutput;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Button btnClear;

        // Timers
        private System.Windows.Forms.Timer tmrCountdown;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.lblCountdownTitle = new System.Windows.Forms.Label();
            this.lblCountdown = new System.Windows.Forms.Label();
            this.lblHearingDate = new System.Windows.Forms.Label();
            this.lblMCPStatus = new System.Windows.Forms.Label();
            this.lblIRCStatus = new System.Windows.Forms.Label();
            this.btnConnectMCP = new System.Windows.Forms.Button();
            this.btnConnectIRC = new System.Windows.Forms.Button();

            this.grpQuery = new System.Windows.Forms.GroupBox();
            this.lblTool = new System.Windows.Forms.Label();
            this.cmbTool = new System.Windows.Forms.ComboBox();
            this.lblAction = new System.Windows.Forms.Label();
            this.txtAction = new System.Windows.Forms.TextBox();
            this.lblProvider = new System.Windows.Forms.Label();
            this.cmbProvider = new System.Windows.Forms.ComboBox();
            this.lblPayload = new System.Windows.Forms.Label();
            this.txtPayload = new System.Windows.Forms.TextBox();
            this.btnQuery = new System.Windows.Forms.Button();

            this.grpOutput = new System.Windows.Forms.GroupBox();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.btnClear = new System.Windows.Forms.Button();

            this.tmrCountdown = new System.Windows.Forms.Timer(this.components);

            this.grpStatus.SuspendLayout();
            this.grpQuery.SuspendLayout();
            this.grpOutput.SuspendLayout();
            this.SuspendLayout();

            // ─── grpStatus ───────────────────────────────────────────
            this.grpStatus.Location = new System.Drawing.Point(10, 10);
            this.grpStatus.Size = new System.Drawing.Size(375, 220);
            this.grpStatus.Text = "System Status";
            this.grpStatus.ForeColor = System.Drawing.Color.White;

            this.lblCountdownTitle.Location = new System.Drawing.Point(10, 22);
            this.lblCountdownTitle.Size = new System.Drawing.Size(355, 18);
            this.lblCountdownTitle.Text = "Rozprawa IV RNs 301/26 — SO Gdańsk 09.12.2026 14:45";
            this.lblCountdownTitle.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblCountdownTitle.ForeColor = System.Drawing.Color.Silver;

            this.lblCountdown.Location = new System.Drawing.Point(10, 44);
            this.lblCountdown.Size = new System.Drawing.Size(355, 48);
            this.lblCountdown.Text = "...";
            this.lblCountdown.Font = new System.Drawing.Font("Consolas", 24F, System.Drawing.FontStyle.Bold);
            this.lblCountdown.ForeColor = System.Drawing.Color.Orange;

            this.lblHearingDate.Location = new System.Drawing.Point(10, 96);
            this.lblHearingDate.Size = new System.Drawing.Size(355, 16);
            this.lblHearingDate.Text = "Alicja (10, CBT §374) + Aleksandra (8, ASD F84.5) — 17+ m-cy bez kontaktu";
            this.lblHearingDate.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblHearingDate.ForeColor = System.Drawing.Color.LightCoral;

            this.lblMCPStatus.Location = new System.Drawing.Point(10, 118);
            this.lblMCPStatus.Size = new System.Drawing.Size(200, 20);
            this.lblMCPStatus.Text = "MCP: Rozłączony";
            this.lblMCPStatus.ForeColor = System.Drawing.Color.OrangeRed;

            this.lblIRCStatus.Location = new System.Drawing.Point(10, 140);
            this.lblIRCStatus.Size = new System.Drawing.Size(200, 20);
            this.lblIRCStatus.Text = "IRC: Rozłączony";
            this.lblIRCStatus.ForeColor = System.Drawing.Color.OrangeRed;

            this.btnConnectMCP.Location = new System.Drawing.Point(10, 165);
            this.btnConnectMCP.Size = new System.Drawing.Size(170, 44);
            this.btnConnectMCP.Text = "Połącz MCP";
            this.btnConnectMCP.Click += new System.EventHandler(this.btnConnectMCP_Click);

            this.btnConnectIRC.Location = new System.Drawing.Point(195, 165);
            this.btnConnectIRC.Size = new System.Drawing.Size(170, 44);
            this.btnConnectIRC.Text = "Połącz IRC";
            this.btnConnectIRC.Click += new System.EventHandler(this.btnConnectIRC_Click);

            this.grpStatus.Controls.Add(this.lblCountdownTitle);
            this.grpStatus.Controls.Add(this.lblCountdown);
            this.grpStatus.Controls.Add(this.lblHearingDate);
            this.grpStatus.Controls.Add(this.lblMCPStatus);
            this.grpStatus.Controls.Add(this.lblIRCStatus);
            this.grpStatus.Controls.Add(this.btnConnectMCP);
            this.grpStatus.Controls.Add(this.btnConnectIRC);

            // ─── grpQuery ────────────────────────────────────────────
            this.grpQuery.Location = new System.Drawing.Point(395, 10);
            this.grpQuery.Size = new System.Drawing.Size(390, 220);
            this.grpQuery.Text = "Fleet Query (MCP)";
            this.grpQuery.ForeColor = System.Drawing.Color.White;

            this.lblTool.Location = new System.Drawing.Point(10, 22);
            this.lblTool.Size = new System.Drawing.Size(80, 20);
            this.lblTool.Text = "Tool:";
            this.lblTool.ForeColor = System.Drawing.Color.Silver;

            this.cmbTool.Location = new System.Drawing.Point(90, 20);
            this.cmbTool.Size = new System.Drawing.Size(290, 24);
            this.cmbTool.Items.AddRange(new object[] {
                "lokis_ai", "lokis_mail", "lokis_calendar",
                "lokis_chat", "lokis_docs", "lokis_project",
                "lokis_commerce", "lokis_design"
            });
            this.cmbTool.SelectedIndex = 0;

            this.lblAction.Location = new System.Drawing.Point(10, 54);
            this.lblAction.Size = new System.Drawing.Size(80, 20);
            this.lblAction.Text = "Action:";
            this.lblAction.ForeColor = System.Drawing.Color.Silver;

            this.txtAction.Location = new System.Drawing.Point(90, 52);
            this.txtAction.Size = new System.Drawing.Size(290, 24);
            this.txtAction.Text = "inference";

            this.lblProvider.Location = new System.Drawing.Point(10, 86);
            this.lblProvider.Size = new System.Drawing.Size(80, 20);
            this.lblProvider.Text = "Provider:";
            this.lblProvider.ForeColor = System.Drawing.Color.Silver;

            this.cmbProvider.Location = new System.Drawing.Point(90, 84);
            this.cmbProvider.Size = new System.Drawing.Size(290, 24);
            this.cmbProvider.Items.AddRange(new object[] {
                "ollama", "groq", "deepseek", "gemini", "gpt", "grok",
                "gmail", "google", "slack", "gdrive", "linear"
            });
            this.cmbProvider.SelectedIndex = 0;

            this.lblPayload.Location = new System.Drawing.Point(10, 118);
            this.lblPayload.Size = new System.Drawing.Size(80, 20);
            this.lblPayload.Text = "Prompt:";
            this.lblPayload.ForeColor = System.Drawing.Color.Silver;

            this.txtPayload.Location = new System.Drawing.Point(90, 116);
            this.txtPayload.Size = new System.Drawing.Size(290, 24);
            this.txtPayload.Text = "Co jest priorytetem na dziś?";

            this.btnQuery.Location = new System.Drawing.Point(90, 155);
            this.btnQuery.Size = new System.Drawing.Size(290, 50);
            this.btnQuery.Text = "Wyślij Query do Floty";
            this.btnQuery.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);

            this.grpQuery.Controls.Add(this.lblTool);
            this.grpQuery.Controls.Add(this.cmbTool);
            this.grpQuery.Controls.Add(this.lblAction);
            this.grpQuery.Controls.Add(this.txtAction);
            this.grpQuery.Controls.Add(this.lblProvider);
            this.grpQuery.Controls.Add(this.cmbProvider);
            this.grpQuery.Controls.Add(this.lblPayload);
            this.grpQuery.Controls.Add(this.txtPayload);
            this.grpQuery.Controls.Add(this.btnQuery);

            // ─── grpOutput ───────────────────────────────────────────
            this.grpOutput.Location = new System.Drawing.Point(10, 240);
            this.grpOutput.Size = new System.Drawing.Size(775, 370);
            this.grpOutput.Text = "Output / Log";
            this.grpOutput.ForeColor = System.Drawing.Color.White;

            this.rtbOutput.Location = new System.Drawing.Point(5, 20);
            this.rtbOutput.Size = new System.Drawing.Size(765, 310);
            this.rtbOutput.BackColor = System.Drawing.Color.Black;
            this.rtbOutput.ForeColor = System.Drawing.Color.LightGreen;
            this.rtbOutput.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;

            this.btnClear.Location = new System.Drawing.Point(5, 334);
            this.btnClear.Size = new System.Drawing.Size(100, 30);
            this.btnClear.Text = "Wyczyść";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);

            this.grpOutput.Controls.Add(this.rtbOutput);
            this.grpOutput.Controls.Add(this.btnClear);

            // ─── tmrCountdown ─────────────────────────────────────────
            this.tmrCountdown.Interval = 60000;
            this.tmrCountdown.Tick += new System.EventHandler(this.tmrCountdown_Tick);

            // ─── Orchestrator Form ────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 622);
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.Text = "Apertus Brain Orchestrator — IV RNs 301/26";
            this.MinimumSize = new System.Drawing.Size(811, 661);

            this.Controls.Add(this.grpStatus);
            this.Controls.Add(this.grpQuery);
            this.Controls.Add(this.grpOutput);

            this.grpStatus.ResumeLayout(false);
            this.grpQuery.ResumeLayout(false);
            this.grpOutput.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
