namespace myoddweb.classifier.forms
{
  partial class CategoryForm
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
      this.labelCategory = new System.Windows.Forms.Label();
      this.labelRule = new System.Windows.Forms.Label();
      this.comboBoxFolders = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // Apply
      // 
      this.Apply.Location = new System.Drawing.Point(197, 103);
      this.Apply.Name = "Apply";
      this.Apply.Size = new System.Drawing.Size(75, 23);
      this.Apply.TabIndex = 0;
      this.Apply.Text = "Apply";
      this.Apply.UseVisualStyleBackColor = true;
      this.Apply.Click += new System.EventHandler(this.Apply_Click);
      // 
      // Cancel
      // 
      this.Cancel.Location = new System.Drawing.Point(116, 103);
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
      // labelCategory
      // 
      this.labelCategory.AutoSize = true;
      this.labelCategory.Location = new System.Drawing.Point(9, 9);
      this.labelCategory.Name = "labelCategory";
      this.labelCategory.Size = new System.Drawing.Size(49, 13);
      this.labelCategory.TabIndex = 2;
      this.labelCategory.Text = "Category";
      // 
      // labelRule
      // 
      this.labelRule.AutoSize = true;
      this.labelRule.Location = new System.Drawing.Point(9, 57);
      this.labelRule.Name = "labelRule";
      this.labelRule.Size = new System.Drawing.Size(46, 13);
      this.labelRule.TabIndex = 3;
      this.labelRule.Text = "Move to";
      // 
      // comboBoxFolders
      // 
      this.comboBoxFolders.FormattingEnabled = true;
      this.comboBoxFolders.Location = new System.Drawing.Point(12, 76);
      this.comboBoxFolders.Name = "comboBoxFolders";
      this.comboBoxFolders.Size = new System.Drawing.Size(260, 21);
      this.comboBoxFolders.TabIndex = 5;
      // 
      // CategoryForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(289, 138);
      this.Controls.Add(this.textName);
      this.Controls.Add(this.comboBoxFolders);
      this.Controls.Add(this.labelRule);
      this.Controls.Add(this.labelCategory);
      this.Controls.Add(this.Cancel);
      this.Controls.Add(this.Apply);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "CategoryForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Category";
      this.Load += new System.EventHandler(this.CategoryForm_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button Apply;
    private System.Windows.Forms.Button Cancel;
    private System.Windows.Forms.TextBox textName;
    private System.Windows.Forms.Label labelCategory;
    private System.Windows.Forms.Label labelRule;
    private System.Windows.Forms.ComboBox comboBoxFolders;
  }
}