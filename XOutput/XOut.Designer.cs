namespace XOutput
{
    partial class XOut
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XOut));
            this.StartStopBtn = new System.Windows.Forms.Button();
            this.isExclusive = new System.Windows.Forms.CheckBox();
            this.controllerList = new System.Windows.Forms.CheckedListBox();
            this.settingsLabel = new System.Windows.Forms.LinkLabel();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.tipLabel = new System.Windows.Forms.Label();
            this.checkStartOnStartup = new System.Windows.Forms.CheckBox();
            this.checkToTrayOnStartup = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // StartStopBtn
            // 
            this.StartStopBtn.Location = new System.Drawing.Point(397, 316);
            this.StartStopBtn.Name = "StartStopBtn";
            this.StartStopBtn.Size = new System.Drawing.Size(75, 23);
            this.StartStopBtn.TabIndex = 0;
            this.StartStopBtn.Text = "Start";
            this.StartStopBtn.UseVisualStyleBackColor = true;
            this.StartStopBtn.Click += new System.EventHandler(this.StartStopBtn_Click);
            // 
            // isExclusive
            // 
            this.isExclusive.AutoSize = true;
            this.isExclusive.Location = new System.Drawing.Point(12, 320);
            this.isExclusive.Name = "isExclusive";
            this.isExclusive.Size = new System.Drawing.Size(75, 17);
            this.isExclusive.TabIndex = 9;
            this.isExclusive.Text = "Exclusivity";
            this.isExclusive.UseVisualStyleBackColor = true;
            this.isExclusive.CheckedChanged += new System.EventHandler(this.isExclusive_CheckedChanged);
            // 
            // controllerList
            // 
            this.controllerList.CheckOnClick = true;
            this.controllerList.FormattingEnabled = true;
            this.controllerList.Location = new System.Drawing.Point(12, 12);
            this.controllerList.Name = "controllerList";
            this.controllerList.Size = new System.Drawing.Size(460, 289);
            this.controllerList.TabIndex = 10;
            this.controllerList.MouseEnter += new System.EventHandler(this.controllerList_MouseEnter);
            this.controllerList.MouseLeave += new System.EventHandler(this.controllerList_MouseLeave);
            this.controllerList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.controllerList_MouseUp);
            // 
            // settingsLabel
            // 
            this.settingsLabel.AutoSize = true;
            this.settingsLabel.Location = new System.Drawing.Point(93, 321);
            this.settingsLabel.Name = "settingsLabel";
            this.settingsLabel.Size = new System.Drawing.Size(70, 13);
            this.settingsLabel.TabIndex = 11;
            this.settingsLabel.TabStop = true;
            this.settingsLabel.Text = "Control Panel";
            this.settingsLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.settingsLabel_LinkClicked);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "XOutput";
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // tipLabel
            // 
            this.tipLabel.AutoSize = true;
            this.tipLabel.BackColor = System.Drawing.SystemColors.Control;
            this.tipLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.tipLabel.Location = new System.Drawing.Point(170, 320);
            this.tipLabel.Name = "tipLabel";
            this.tipLabel.Size = new System.Drawing.Size(44, 13);
            this.tipLabel.TabIndex = 12;
            this.tipLabel.Text = "tipLabel";
            // 
            // checkStartOnStartup
            // 
            this.checkStartOnStartup.AutoSize = true;
            this.checkStartOnStartup.Location = new System.Drawing.Point(12, 354);
            this.checkStartOnStartup.Name = "checkStartOnStartup";
            this.checkStartOnStartup.Size = new System.Drawing.Size(98, 17);
            this.checkStartOnStartup.TabIndex = 13;
            this.checkStartOnStartup.Text = "Start on startup";
            this.checkStartOnStartup.UseVisualStyleBackColor = true;
            this.checkStartOnStartup.CheckedChanged += new System.EventHandler(this.checkStartOnStartup_CheckedChanged);
            // 
            // checkToTrayOnStartup
            // 
            this.checkToTrayOnStartup.AutoSize = true;
            this.checkToTrayOnStartup.Location = new System.Drawing.Point(139, 354);
            this.checkToTrayOnStartup.Name = "checkToTrayOnStartup";
            this.checkToTrayOnStartup.Size = new System.Drawing.Size(148, 17);
            this.checkToTrayOnStartup.TabIndex = 14;
            this.checkToTrayOnStartup.Text = "Minimize to tray on startup";
            this.checkToTrayOnStartup.UseVisualStyleBackColor = true;
            this.checkToTrayOnStartup.CheckedChanged += new System.EventHandler(this.checkToTrayOnStartup_CheckedChanged);
            // 
            // XOut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 383);
            this.Controls.Add(this.checkToTrayOnStartup);
            this.Controls.Add(this.checkStartOnStartup);
            this.Controls.Add(this.tipLabel);
            this.Controls.Add(this.settingsLabel);
            this.Controls.Add(this.controllerList);
            this.Controls.Add(this.isExclusive);
            this.Controls.Add(this.StartStopBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "XOut";
            this.Text = "XOutput";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.XOut_Closing);
            this.Load += new System.EventHandler(this.XOut_Load);
            this.Shown += new System.EventHandler(this.XOut_Shown);
            this.Resize += new System.EventHandler(this.XOut_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartStopBtn;

        private System.Windows.Forms.GroupBox[] boxes;
        private System.Windows.Forms.CheckBox[] checks;
        private System.Windows.Forms.Button[] options;
        private ControllerOptions optionsWindow;
        private ControllerManager controllerManager;
        private System.Windows.Forms.CheckBox isExclusive;
        private System.Windows.Forms.CheckedListBox controllerList;
        private System.Windows.Forms.LinkLabel settingsLabel;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Label tipLabel;
        private System.Windows.Forms.CheckBox checkStartOnStartup;
        private System.Windows.Forms.CheckBox checkToTrayOnStartup;
    }
}