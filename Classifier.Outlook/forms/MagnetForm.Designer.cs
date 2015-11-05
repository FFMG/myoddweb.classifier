namespace myoddweb.classifier.forms
{
  partial class MagnetForm
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
      this.Apply = new System.Windows.Forms.Button();
      this.Cancel = new System.Windows.Forms.Button();
      this.textName = new System.Windows.Forms.TextBox();
      this.labelMagnet = new System.Windows.Forms.Label();
      this.labelRule = new System.Windows.Forms.Label();
      this.comboBoxCategories = new System.Windows.Forms.ComboBox();
      this.comboBoxRules = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // Apply
      // 
      this.Apply.Location = new System.Drawing.Point(197, 135);
      this.Apply.Name = "Apply";
      this.Apply.Size = new System.Drawing.Size(75, 23);
      this.Apply.TabIndex = 0;
      this.Apply.Text = "Apply";
      this.Apply.UseVisualStyleBackColor = true;
      this.Apply.Click += new System.EventHandler(this.Apply_Click);
      // 
      // Cancel
      // 
      this.Cancel.Location = new System.Drawing.Point(116, 135);
      this.Cancel.Name = "Cancel";
      this.Cancel.Size = new System.Drawing.Size(75, 23);
      this.Cancel.TabIndex = 1;
      this.Cancel.Text = "Cancel";
      this.Cancel.UseVisualStyleBackColor = true;
      this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
      // 
      // textName
      // 
      this.textName.Location = new System.Drawing.Point(12, 29);
      this.textName.Name = "textName";
      this.textName.Size = new System.Drawing.Size(260, 20);
      this.textName.TabIndex = 4;
      // 
      // labelMagnet
      // 
      this.labelMagnet.AutoSize = true;
      this.labelMagnet.Location = new System.Drawing.Point(9, 9);
      this.labelMagnet.Name = "labelMagnet";
      this.labelMagnet.Size = new System.Drawing.Size(147, 13);
      this.labelMagnet.TabIndex = 2;
      this.labelMagnet.Text = "Magnet (email address/name)";
      // 
      // labelRule
      // 
      this.labelRule.AutoSize = true;
      this.labelRule.Location = new System.Drawing.Point(9, 92);
      this.labelRule.Name = "labelRule";
      this.labelRule.Size = new System.Drawing.Size(91, 13);
      this.labelRule.TabIndex = 3;
      this.labelRule.Text = "Move to Category";
      // 
      // comboBoxCategories
      // 
      this.comboBoxCategories.FormattingEnabled = true;
      this.comboBoxCategories.Location = new System.Drawing.Point(12, 108);
      this.comboBoxCategories.Name = "comboBoxCategories";
      this.comboBoxCategories.Size = new System.Drawing.Size(260, 21);
      this.comboBoxCategories.TabIndex = 5;
      // 
      // comboBoxRules
      // 
      this.comboBoxRules.FormattingEnabled = true;
      this.comboBoxRules.Location = new System.Drawing.Point(12, 68);
      this.comboBoxRules.Name = "comboBoxRules";
      this.comboBoxRules.Size = new System.Drawing.Size(260, 21);
      this.comboBoxRules.TabIndex = 7;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(9, 52);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(29, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "Rule";
      // 
      // MagnetForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(289, 168);
      this.Controls.Add(this.comboBoxRules);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.textName);
      this.Controls.Add(this.comboBoxCategories);
      this.Controls.Add(this.labelRule);
      this.Controls.Add(this.labelMagnet);
      this.Controls.Add(this.Cancel);
      this.Controls.Add(this.Apply);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MagnetForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Magnet";
      this.Load += new System.EventHandler(this.MagnetForm_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button Apply;
    private System.Windows.Forms.Button Cancel;
    private System.Windows.Forms.TextBox textName;
    private System.Windows.Forms.Label labelMagnet;
    private System.Windows.Forms.Label labelRule;
    private System.Windows.Forms.ComboBox comboBoxCategories;
    private System.Windows.Forms.ComboBox comboBoxRules;
    private System.Windows.Forms.Label label1;
  }
}