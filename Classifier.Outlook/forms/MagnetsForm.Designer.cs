namespace myoddweb.classifier.forms
{
  partial class MagnetsForm
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
      this.Ok = new System.Windows.Forms.Button();
      this.ListMagnets = new System.Windows.Forms.ListView();
      this.New = new System.Windows.Forms.Button();
      this.Edit = new System.Windows.Forms.Button();
      this.Delete = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // Ok
      // 
      this.Ok.Location = new System.Drawing.Point(516, 273);
      this.Ok.Name = "Ok";
      this.Ok.Size = new System.Drawing.Size(75, 23);
      this.Ok.TabIndex = 0;
      this.Ok.Text = "&Ok";
      this.Ok.UseVisualStyleBackColor = true;
      this.Ok.Click += new System.EventHandler(this.Ok_Click);
      // 
      // ListMagnets
      // 
      this.ListMagnets.FullRowSelect = true;
      this.ListMagnets.GridLines = true;
      this.ListMagnets.Location = new System.Drawing.Point(12, 12);
      this.ListMagnets.Name = "ListMagnets";
      this.ListMagnets.Size = new System.Drawing.Size(579, 255);
      this.ListMagnets.TabIndex = 1;
      this.ListMagnets.UseCompatibleStateImageBehavior = false;
      this.ListMagnets.View = System.Windows.Forms.View.List;
      this.ListMagnets.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListMagnets_ItemSelectionChanged);
      this.ListMagnets.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListMagnets_MouseDoubleClick);
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
      this.Controls.Add(this.ListMagnets);
      this.Controls.Add(this.Ok);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "CategoriesForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Category";
      this.Load += new System.EventHandler(this.MagnetsForm_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button Ok;
    private System.Windows.Forms.ListView ListMagnets;
    private System.Windows.Forms.Button New;
    private System.Windows.Forms.Button Edit;
    private System.Windows.Forms.Button Delete;
  }
}