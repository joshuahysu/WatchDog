﻿namespace WatchDog
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.serverState = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.startTimer1 = new System.Windows.Forms.Timer(this.components);
            this.AP = new System.Windows.Forms.Label();
            this.errorCode = new System.Windows.Forms.Label();
            this.webState = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 4000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // serverState
            // 
            this.serverState.AutoSize = true;
            this.serverState.Font = new System.Drawing.Font("新細明體", 20F);
            this.serverState.Location = new System.Drawing.Point(17, 51);
            this.serverState.Name = "serverState";
            this.serverState.Size = new System.Drawing.Size(117, 34);
            this.serverState.TabIndex = 0;
            this.serverState.Text = "Starting";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(190, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "time";
            // 
            // startTimer1
            // 
            this.startTimer1.Enabled = true;
            this.startTimer1.Interval = 1000;
            this.startTimer1.Tick += new System.EventHandler(this.startTimer1_Tick);
            // 
            // AP
            // 
            this.AP.AutoSize = true;
            this.AP.Font = new System.Drawing.Font("新細明體", 20F);
            this.AP.Location = new System.Drawing.Point(17, 9);
            this.AP.Name = "AP";
            this.AP.Size = new System.Drawing.Size(56, 34);
            this.AP.TabIndex = 2;
            this.AP.Text = "AP";
            // 
            // errorCode
            // 
            this.errorCode.AutoSize = true;
            this.errorCode.Location = new System.Drawing.Point(20, 98);
            this.errorCode.Name = "errorCode";
            this.errorCode.Size = new System.Drawing.Size(64, 15);
            this.errorCode.TabIndex = 3;
            this.errorCode.Text = "errorCode";
            // 
            // webState
            // 
            this.webState.AutoSize = true;
            this.webState.Font = new System.Drawing.Font("新細明體", 20F);
            this.webState.Location = new System.Drawing.Point(187, 9);
            this.webState.Name = "webState";
            this.webState.Size = new System.Drawing.Size(68, 34);
            this.webState.TabIndex = 4;
            this.webState.Text = "web";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 296);
            this.Controls.Add(this.webState);
            this.Controls.Add(this.errorCode);
            this.Controls.Add(this.AP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverState);
            this.Name = "Form1";
            this.Text = "watch dog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label serverState;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer startTimer1;
        private System.Windows.Forms.Label AP;
        private System.Windows.Forms.Label errorCode;
        private System.Windows.Forms.Label webState;
    }
}

