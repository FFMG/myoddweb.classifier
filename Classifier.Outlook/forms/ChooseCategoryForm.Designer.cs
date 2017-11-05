namespace myoddweb.classifier.forms
{
  partial class ChooseCategoryForm
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
      this.btnSelect = new System.Windows.Forms.Button();
      this.Cancel = new System.Windows.Forms.Button();
      this.checkBoxDontAskAgain = new System.Windows.Forms.CheckBox();
      this.comboBoxCategories = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // btnSelect
      // 
      this.btnSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnSelect.Location = new System.Drawing.Point(161, 39);
      this.btnSelect.Name = "btnSelect";
      this.btnSelect.Size = new System.Drawing.Size(75, 23);
      this.btnSelect.TabIndex = 3;
      this.btnSelect.Text = "Select";
      this.btnSelect.UseVisualStyleBackColor = true;
      this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
      // 
      // Cancel
      // 
      this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Cancel.Location = new System.Drawing.Point(242, 39);
      this.Cancel.Name = "Cancel";
      this.Cancel.Size = new System.Drawing.Size(75, 23);
      this.Cancel.TabIndex = 4;
      this.Cancel.Text = "&Cancel";
      this.Cancel.UseVisualStyleBackColor = true;
      // 
      // checkBoxDontAskAgain
      // 
      this.checkBoxDontAskAgain.AutoSize = true;
      this.checkBoxDontAskAgain.Location = new System.Drawing.Point(12, 43);
      this.checkBoxDontAskAgain.Name = "checkBoxDontAskAgain";
      this.checkBoxDontAskAgain.Size = new System.Drawing.Size(117, 17);
      this.checkBoxDontAskAgain.TabIndex = 5;
      this.checkBoxDontAskAgain.Text = "Don\'t ask me again";
      this.checkBoxDontAskAgain.UseVisualStyleBackColor = true;
      // 
      // comboBoxCategories
      // 
      this.comboBoxCategories.FormattingEnabled = true;
      this.comboBoxCategories.Location = new System.Drawing.Point(68, 12);
      this.comboBoxCategories.Name = "comboBoxCategories";
      this.comboBoxCategories.Size = new System.Drawing.Size(249, 21);
      this.comboBoxCategories.TabIndex = 6;
      this.comboBoxCategories.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(9, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(49, 13);
      this.label1.TabIndex = 7;
      this.label1.Text = "Category";
      // 
      // ChooseCategoryForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(329, 74);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.comboBoxCategories);
      this.Controls.Add(this.checkBoxDontAskAgain);
      this.Controls.Add(this.Cancel);
      this.Controls.Add(this.btnSelect);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ChooseCategoryForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Choose Category";
      this.Load += new System.EventHandler(this.ChooseCategoryForm_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Button btnSelect;
    private System.Windows.Forms.Button Cancel;
    private System.Windows.Forms.CheckBox checkBoxDontAskAgain;
    private System.Windows.Forms.ComboBox comboBoxCategories;
    private System.Windows.Forms.Label label1;
  }
}