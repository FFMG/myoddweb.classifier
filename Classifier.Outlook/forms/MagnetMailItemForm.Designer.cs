namespace myoddweb.classifier.forms
{
  partial class MagnetMailItemForm
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
      this.labelMagnet = new System.Windows.Forms.Label();
      this.labelRule = new System.Windows.Forms.Label();
      this.comboBoxCategories = new System.Windows.Forms.ComboBox();
      this.comboMagnetAndRules = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // Apply
      // 
      this.Apply.Location = new System.Drawing.Point(289, 95);
      this.Apply.Name = "Apply";
      this.Apply.Size = new System.Drawing.Size(75, 23);
      this.Apply.TabIndex = 0;
      this.Apply.Text = "Apply";
      this.Apply.UseVisualStyleBackColor = true;
      this.Apply.Click += new System.EventHandler(this.Apply_Click);
      // 
      // Cancel
      // 
      this.Cancel.Location = new System.Drawing.Point(208, 95);
      this.Cancel.Name = "Cancel";
      this.Cancel.Size = new System.Drawing.Size(75, 23);
      this.Cancel.TabIndex = 1;
      this.Cancel.Text = "Cancel";
      this.Cancel.UseVisualStyleBackColor = true;
      this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
      // 
      // labelMagnet
      // 
      this.labelMagnet.AutoSize = true;
      this.labelMagnet.Location = new System.Drawing.Point(9, 9);
      this.labelMagnet.Name = "labelMagnet";
      this.labelMagnet.Size = new System.Drawing.Size(43, 13);
      this.labelMagnet.TabIndex = 2;
      this.labelMagnet.Text = "Magnet";
      // 
      // labelRule
      // 
      this.labelRule.AutoSize = true;
      this.labelRule.Location = new System.Drawing.Point(9, 52);
      this.labelRule.Name = "labelRule";
      this.labelRule.Size = new System.Drawing.Size(91, 13);
      this.labelRule.TabIndex = 3;
      this.labelRule.Text = "Move to Category";
      // 
      // comboBoxCategories
      // 
      this.comboBoxCategories.FormattingEnabled = true;
      this.comboBoxCategories.Location = new System.Drawing.Point(12, 68);
      this.comboBoxCategories.Name = "comboBoxCategories";
      this.comboBoxCategories.Size = new System.Drawing.Size(352, 21);
      this.comboBoxCategories.TabIndex = 5;
      // 
      // comboMagnetAndRules
      // 
      this.comboMagnetAndRules.FormattingEnabled = true;
      this.comboMagnetAndRules.Location = new System.Drawing.Point(12, 28);
      this.comboMagnetAndRules.Name = "comboMagnetAndRules";
      this.comboMagnetAndRules.Size = new System.Drawing.Size(352, 21);
      this.comboMagnetAndRules.TabIndex = 7;
      // 
      // MagnetMailItemForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(375, 125);
      this.Controls.Add(this.comboMagnetAndRules);
      this.Controls.Add(this.comboBoxCategories);
      this.Controls.Add(this.labelRule);
      this.Controls.Add(this.labelMagnet);
      this.Controls.Add(this.Cancel);
      this.Controls.Add(this.Apply);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "MagnetMailItemForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Magnets";
      this.Load += new System.EventHandler(this.MagnetMailItemForm_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button Apply;
    private System.Windows.Forms.Button Cancel;
    private System.Windows.Forms.Label labelMagnet;
    private System.Windows.Forms.Label labelRule;
    private System.Windows.Forms.ComboBox comboBoxCategories;
    private System.Windows.Forms.ComboBox comboMagnetAndRules;
  }
}