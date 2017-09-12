namespace myoddweb.classifier.forms
{
  partial class LogForm
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
      this.ok = new System.Windows.Forms.Button();
      this.listLog = new System.Windows.Forms.ListView();
      this.SuspendLayout();
      // 
      // ok
      // 
      this.ok.Location = new System.Drawing.Point(581, 359);
      this.ok.Name = "ok";
      this.ok.Size = new System.Drawing.Size(75, 23);
      this.ok.TabIndex = 0;
      this.ok.Text = "&Ok";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.Ok_Click);
      // 
      // listLog
      // 
      this.listLog.FullRowSelect = true;
      this.listLog.GridLines = true;
      this.listLog.Location = new System.Drawing.Point(12, 12);
      this.listLog.Name = "listLog";
      this.listLog.Size = new System.Drawing.Size(644, 341);
      this.listLog.TabIndex = 1;
      this.listLog.UseCompatibleStateImageBehavior = false;
      this.listLog.View = System.Windows.Forms.View.List;
      // 
      // LogForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(668, 394);
      this.Controls.Add(this.listLog);
      this.Controls.Add(this.ok);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "LogForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Log";
      this.Load += new System.EventHandler(this.LogForm_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.ListView listLog;
  }
}