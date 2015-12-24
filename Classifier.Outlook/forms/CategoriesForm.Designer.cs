namespace myoddweb.classifier.forms
{
  partial class CategoriesForm
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
      this.listCategories = new System.Windows.Forms.ListView();
      this.New = new System.Windows.Forms.Button();
      this.Edit = new System.Windows.Forms.Button();
      this.Delete = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // ok
      // 
      this.ok.Location = new System.Drawing.Point(516, 273);
      this.ok.Name = "ok";
      this.ok.Size = new System.Drawing.Size(75, 23);
      this.ok.TabIndex = 0;
      this.ok.Text = "&Ok";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.Ok_Click);
      // 
      // listCategories
      // 
      this.listCategories.FullRowSelect = true;
      this.listCategories.GridLines = true;
      this.listCategories.Location = new System.Drawing.Point(12, 12);
      this.listCategories.Name = "listCategories";
      this.listCategories.Size = new System.Drawing.Size(579, 255);
      this.listCategories.TabIndex = 1;
      this.listCategories.UseCompatibleStateImageBehavior = false;
      this.listCategories.View = System.Windows.Forms.View.List;
      this.listCategories.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listCategories_ItemSelectionChanged);
      this.listCategories.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listCategories_MouseDoubleClick);
      // 
      // New
      // 
      this.New.Location = new System.Drawing.Point(174, 273);
      this.New.Name = "New";
      this.New.Size = new System.Drawing.Size(75, 23);
      this.New.TabIndex = 2;
      this.New.Text = "&New...";
      this.New.UseVisualStyleBackColor = true;
      this.New.Click += new System.EventHandler(this.New_Click);
      // 
      // Edit
      // 
      this.Edit.Location = new System.Drawing.Point(12, 273);
      this.Edit.Name = "Edit";
      this.Edit.Size = new System.Drawing.Size(75, 23);
      this.Edit.TabIndex = 3;
      this.Edit.Text = "&Edit...";
      this.Edit.UseVisualStyleBackColor = true;
      this.Edit.Click += new System.EventHandler(this.Edit_Click);
      // 
      // Delete
      // 
      this.Delete.Location = new System.Drawing.Point(93, 273);
      this.Delete.Name = "Delete";
      this.Delete.Size = new System.Drawing.Size(75, 23);
      this.Delete.TabIndex = 5;
      this.Delete.Text = "Delete";
      this.Delete.UseVisualStyleBackColor = true;
      this.Delete.Click += new System.EventHandler(this.Delete_Click);
      // 
      // CategoriesForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(603, 308);
      this.Controls.Add(this.Delete);
      this.Controls.Add(this.Edit);
      this.Controls.Add(this.New);
      this.Controls.Add(this.listCategories);
      this.Controls.Add(this.ok);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "CategoriesForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Category";
      this.Load += new System.EventHandler(this.CategoriesForm_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.ListView listCategories;
    private System.Windows.Forms.Button New;
    private System.Windows.Forms.Button Edit;
    private System.Windows.Forms.Button Delete;
  }
}