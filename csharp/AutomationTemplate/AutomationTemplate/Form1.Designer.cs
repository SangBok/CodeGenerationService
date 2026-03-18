namespace AutomationTemplate
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnLoadBt = new System.Windows.Forms.Button();
            this.btnStartBt = new System.Windows.Forms.Button();
            this.btnStopBt = new System.Windows.Forms.Button();
            this.lblBtStatus = new System.Windows.Forms.Label();
            this.txtBtPath = new System.Windows.Forms.TextBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnLoadBt
            // 
            this.btnLoadBt.Location = new System.Drawing.Point(12, 12);
            this.btnLoadBt.Name = "btnLoadBt";
            this.btnLoadBt.Size = new System.Drawing.Size(90, 28);
            this.btnLoadBt.TabIndex = 0;
            this.btnLoadBt.Text = "BT 로드...";
            this.btnLoadBt.UseVisualStyleBackColor = true;
            this.btnLoadBt.Click += new System.EventHandler(this.btnLoadBt_Click);
            // 
            // btnStartBt
            // 
            this.btnStartBt.Location = new System.Drawing.Point(108, 12);
            this.btnStartBt.Name = "btnStartBt";
            this.btnStartBt.Size = new System.Drawing.Size(90, 28);
            this.btnStartBt.TabIndex = 1;
            this.btnStartBt.Text = "Start";
            this.btnStartBt.UseVisualStyleBackColor = true;
            this.btnStartBt.Click += new System.EventHandler(this.btnStartBt_Click);
            // 
            // btnStopBt
            // 
            this.btnStopBt.Location = new System.Drawing.Point(204, 12);
            this.btnStopBt.Name = "btnStopBt";
            this.btnStopBt.Size = new System.Drawing.Size(90, 28);
            this.btnStopBt.TabIndex = 2;
            this.btnStopBt.Text = "Stop";
            this.btnStopBt.UseVisualStyleBackColor = true;
            this.btnStopBt.Click += new System.EventHandler(this.btnStopBt_Click);
            // 
            // lblBtStatus
            // 
            this.lblBtStatus.AutoSize = true;
            this.lblBtStatus.Location = new System.Drawing.Point(310, 19);
            this.lblBtStatus.Name = "lblBtStatus";
            this.lblBtStatus.Size = new System.Drawing.Size(89, 12);
            this.lblBtStatus.TabIndex = 3;
            this.lblBtStatus.Text = "BT 상태: Idle";
            // 
            // txtBtPath
            // 
            this.txtBtPath.Location = new System.Drawing.Point(12, 46);
            this.txtBtPath.Name = "txtBtPath";
            this.txtBtPath.ReadOnly = true;
            this.txtBtPath.Size = new System.Drawing.Size(956, 21);
            this.txtBtPath.TabIndex = 4;
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(12, 78);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(956, 510);
            this.txtLog.TabIndex = 5;
            this.txtLog.WordWrap = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 600);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.txtBtPath);
            this.Controls.Add(this.lblBtStatus);
            this.Controls.Add(this.btnStopBt);
            this.Controls.Add(this.btnStartBt);
            this.Controls.Add(this.btnLoadBt);
            this.Name = "Form1";
            this.Text = "AutomationTemplate - BT Runner";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoadBt;
        private System.Windows.Forms.Button btnStartBt;
        private System.Windows.Forms.Button btnStopBt;
        private System.Windows.Forms.Label lblBtStatus;
        private System.Windows.Forms.TextBox txtBtPath;
        private System.Windows.Forms.TextBox txtLog;
    }
}

