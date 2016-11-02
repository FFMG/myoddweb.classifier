namespace myoddweb.viewer.forms
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
      this.buttonPaste = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // webBrowserDetails
      // 
      this.webBrowserDetails.Location = new System.Drawing.Point(0, 41);
      this.webBrowserDetails.MinimumSize = new System.Drawing.Size(20, 20);
      this.webBrowserDetails.Name = "webBrowserDetails";
      this.webBrowserDetails.Size = new System.Drawing.Size(535, 375);
      this.webBrowserDetails.TabIndex = 0;
      // 
      // buttonPaste
      // 
      this.buttonPaste.Location = new System.Drawing.Point(460, 12);
      this.buttonPaste.Name = "buttonPaste";
      this.buttonPaste.Size = new System.Drawing.Size(75, 23);
      this.buttonPaste.TabIndex = 1;
      this.buttonPaste.Text = "&Paste";
      this.buttonPaste.UseVisualStyleBackColor = true;
      // 
      // DetailsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(547, 428);
      this.Controls.Add(this.buttonPaste);
      this.Controls.Add(this.webBrowserDetails);
      this.Name = "DetailsForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Details";
      this.Load += new System.EventHandler(this.OnLoad);
      this.Resize += new System.EventHandler(this.DetailsForm_Resize);
      this.ResumeLayout(false);

    }
    #endregion

    private System.Windows.Forms.WebBrowser webBrowserDetails;
    private System.Windows.Forms.Button buttonPaste;
  }
}

