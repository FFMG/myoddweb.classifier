namespace myoddweb.classifier.forms
{
  partial class OptionsForm
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
      this.reCheckCategories = new System.Windows.Forms.CheckBox();
      this.ok = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.reCheckIfCtrl = new System.Windows.Forms.CheckBox();
      this.checkCategoryIfUnknown = new System.Windows.Forms.CheckBox();
      this.Categories = new System.Windows.Forms.Button();
      this.Magnets = new System.Windows.Forms.Button();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // reCheckCategories
      // 
      this.reCheckCategories.AutoSize = true;
      this.reCheckCategories.Location = new System.Drawing.Point(6, 19);
      this.reCheckCategories.Name = "reCheckCategories";
      this.reCheckCategories.Size = new System.Drawing.Size(278, 17);
      this.reCheckCategories.TabIndex = 0;
      this.reCheckCategories.Text = "Always re-check possible new category (can be slow)";
      this.reCheckCategories.UseVisualStyleBackColor = true;
      this.reCheckCategories.CheckedChanged += new System.EventHandler(this.reCheckCategories_CheckedChanged);
      // 
      // ok
      // 
      this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.ok.Location = new System.Drawing.Point(270, 120);
      this.ok.Name = "ok";
      this.ok.Size = new System.Drawing.Size(75, 23);
      this.ok.TabIndex = 1;
      this.ok.Text = "&Ok";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.ok_Click);
      // 
      // button1
      // 
      this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.button1.Location = new System.Drawing.Point(351, 120);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 2;
      this.button1.Text = "&Cancel";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.reCheckIfCtrl);
      this.groupBox1.Controls.Add(this.checkCategoryIfUnknown);
      this.groupBox1.Controls.Add(this.reCheckCategories);
      this.groupBox1.Location = new System.Drawing.Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(414, 102);
      this.groupBox1.TabIndex = 3;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Debug";
      // 
      // reCheckIfCtrl
      // 
      this.reCheckIfCtrl.AutoSize = true;
      this.reCheckIfCtrl.Location = new System.Drawing.Point(6, 65);
      this.reCheckIfCtrl.Name = "reCheckIfCtrl";
      this.reCheckIfCtrl.Size = new System.Drawing.Size(207, 17);
      this.reCheckIfCtrl.TabIndex = 2;
      this.reCheckIfCtrl.Text = "Always re-check if the Ctrl key is down";
      this.reCheckIfCtrl.UseVisualStyleBackColor = true;
      // 
      // checkCategoryIfUnknown
      // 
      this.checkCategoryIfUnknown.AutoSize = true;
      this.checkCategoryIfUnknown.Location = new System.Drawing.Point(23, 42);
      this.checkCategoryIfUnknown.Name = "checkCategoryIfUnknown";
      this.checkCategoryIfUnknown.Size = new System.Drawing.Size(169, 17);
      this.checkCategoryIfUnknown.TabIndex = 1;
      this.checkCategoryIfUnknown.Text = "If current category is unknown";
      this.checkCategoryIfUnknown.UseVisualStyleBackColor = true;
      // 
      // Categories
      // 
      this.Categories.Location = new System.Drawing.Point(18, 120);
      this.Categories.Name = "Categories";
      this.Categories.Size = new System.Drawing.Size(79, 23);
      this.Categories.TabIndex = 4;
      this.Categories.Text = "C&ategories ...";
      this.Categories.UseVisualStyleBackColor = true;
      this.Categories.Click += new System.EventHandler(this.Categories_Click);
      // 
      // Magnets
      // 
      this.Magnets.Location = new System.Drawing.Point(103, 120);
      this.Magnets.Name = "Magnets";
      this.Magnets.Size = new System.Drawing.Size(79, 23);
      this.Magnets.TabIndex = 5;
      this.Magnets.Text = "&Magnets ...";
      this.Magnets.UseVisualStyleBackColor = true;
      this.Magnets.Click += new System.EventHandler(this.Magnets_Click);
      // 
      // OptionsForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(438, 153);
      this.Controls.Add(this.Magnets);
      this.Controls.Add(this.Categories);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.ok);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "OptionsForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Options";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.CheckBox reCheckCategories;
    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.CheckBox checkCategoryIfUnknown;
    private System.Windows.Forms.CheckBox reCheckIfCtrl;
    private System.Windows.Forms.Button Categories;
    private System.Windows.Forms.Button Magnets;
  }
}