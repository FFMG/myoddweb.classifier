﻿namespace myoddweb.classifier.forms
{
  partial class DetailsForm
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
      this.webBrowserDetails = new System.Windows.Forms.WebBrowser();
      this.SuspendLayout();
      // 
      // webBrowserDetails
      // 
      this.webBrowserDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.webBrowserDetails.Location = new System.Drawing.Point(0, 0);
      this.webBrowserDetails.MinimumSize = new System.Drawing.Size(20, 20);
      this.webBrowserDetails.Name = "webBrowserDetails";
      this.webBrowserDetails.Size = new System.Drawing.Size(284, 261);
      this.webBrowserDetails.TabIndex = 0;
      // 
      // DetailsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 261);
      this.Controls.Add(this.webBrowserDetails);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MinimizeBox = false;
      this.Name = "DetailsForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Details";
      this.Load += new System.EventHandler(this.OnLoad);
      this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.WebBrowser webBrowserDetails;
  }
}