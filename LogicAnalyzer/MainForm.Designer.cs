namespace LogicAnalyzer
{
    partial class MainForm
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
            this.cboDeviceConnected = new System.Windows.Forms.ComboBox();
            this.cboINEndpoint = new System.Windows.Forms.ComboBox();
            this.StartBtn = new System.Windows.Forms.Button();
            this.lblSuccess = new System.Windows.Forms.Label();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.wavePanel = new SharpGL.SceneControl();
            this.btnPanLeft = new System.Windows.Forms.Button();
            this.btnPanRight = new System.Windows.Forms.Button();
            this.btnZoomFull = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.wavePanel)).BeginInit();
            this.SuspendLayout();
            // 
            // cboDeviceConnected
            // 
            this.cboDeviceConnected.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDeviceConnected.DropDownWidth = 251;
            this.cboDeviceConnected.FormattingEnabled = true;
            this.cboDeviceConnected.Location = new System.Drawing.Point(13, 13);
            this.cboDeviceConnected.Name = "cboDeviceConnected";
            this.cboDeviceConnected.Size = new System.Drawing.Size(362, 21);
            this.cboDeviceConnected.TabIndex = 0;
            // 
            // cboINEndpoint
            // 
            this.cboINEndpoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboINEndpoint.FormattingEnabled = true;
            this.cboINEndpoint.Location = new System.Drawing.Point(13, 41);
            this.cboINEndpoint.Name = "cboINEndpoint";
            this.cboINEndpoint.Size = new System.Drawing.Size(362, 21);
            this.cboINEndpoint.TabIndex = 1;
            // 
            // StartBtn
            // 
            this.StartBtn.Location = new System.Drawing.Point(13, 68);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(75, 23);
            this.StartBtn.TabIndex = 2;
            this.StartBtn.Text = "Start";
            this.StartBtn.UseVisualStyleBackColor = true;
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // lblSuccess
            // 
            this.lblSuccess.AutoSize = true;
            this.lblSuccess.Location = new System.Drawing.Point(13, 99);
            this.lblSuccess.Name = "lblSuccess";
            this.lblSuccess.Size = new System.Drawing.Size(73, 13);
            this.lblSuccess.TabIndex = 3;
            this.lblSuccess.Text = "00000000000";
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Location = new System.Drawing.Point(175, 68);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(75, 23);
            this.btnZoomIn.TabIndex = 6;
            this.btnZoomIn.Text = "Zoom In";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Location = new System.Drawing.Point(94, 68);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(75, 23);
            this.btnZoomOut.TabIndex = 7;
            this.btnZoomOut.Text = "Zoom Out";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // wavePanel
            // 
            this.wavePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wavePanel.DrawFPS = false;
            this.wavePanel.Location = new System.Drawing.Point(16, 115);
            this.wavePanel.Name = "wavePanel";
            this.wavePanel.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL4_4;
            this.wavePanel.RenderContextType = SharpGL.RenderContextType.DIBSection;
            this.wavePanel.RenderTrigger = SharpGL.RenderTrigger.Manual;
            this.wavePanel.Size = new System.Drawing.Size(1405, 667);
            this.wavePanel.TabIndex = 8;
            this.wavePanel.OpenGLDraw += new SharpGL.RenderEventHandler(this.wavePanel_OpenGLDraw);
            this.wavePanel.DoubleClick += new System.EventHandler(this.wavePanel_DoubleClick);
            // 
            // btnPanLeft
            // 
            this.btnPanLeft.Location = new System.Drawing.Point(337, 68);
            this.btnPanLeft.Name = "btnPanLeft";
            this.btnPanLeft.Size = new System.Drawing.Size(26, 23);
            this.btnPanLeft.TabIndex = 9;
            this.btnPanLeft.Text = "<";
            this.btnPanLeft.UseVisualStyleBackColor = true;
            this.btnPanLeft.Click += new System.EventHandler(this.btnPanLeft_Click);
            // 
            // btnPanRight
            // 
            this.btnPanRight.Location = new System.Drawing.Point(369, 68);
            this.btnPanRight.Name = "btnPanRight";
            this.btnPanRight.Size = new System.Drawing.Size(26, 23);
            this.btnPanRight.TabIndex = 10;
            this.btnPanRight.Text = ">";
            this.btnPanRight.UseVisualStyleBackColor = true;
            this.btnPanRight.Click += new System.EventHandler(this.btnPanRight_Click);
            // 
            // btnZoomFull
            // 
            this.btnZoomFull.Location = new System.Drawing.Point(256, 68);
            this.btnZoomFull.Name = "btnZoomFull";
            this.btnZoomFull.Size = new System.Drawing.Size(75, 23);
            this.btnZoomFull.TabIndex = 11;
            this.btnZoomFull.Text = "Zoom Full";
            this.btnZoomFull.UseVisualStyleBackColor = true;
            this.btnZoomFull.Click += new System.EventHandler(this.btnZoomFull_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1433, 794);
            this.Controls.Add(this.btnZoomFull);
            this.Controls.Add(this.btnPanRight);
            this.Controls.Add(this.btnPanLeft);
            this.Controls.Add(this.wavePanel);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnZoomIn);
            this.Controls.Add(this.lblSuccess);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.cboINEndpoint);
            this.Controls.Add(this.cboDeviceConnected);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing_1);
            ((System.ComponentModel.ISupportInitialize)(this.wavePanel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboDeviceConnected;
        private System.Windows.Forms.ComboBox cboINEndpoint;
        private System.Windows.Forms.Button StartBtn;
        private System.Windows.Forms.Label lblSuccess;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private SharpGL.SceneControl wavePanel;
        private System.Windows.Forms.Button btnPanLeft;
        private System.Windows.Forms.Button btnPanRight;
        private System.Windows.Forms.Button btnZoomFull;
    }
}

