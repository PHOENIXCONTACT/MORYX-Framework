namespace DataModelWizard
{
    partial class InputForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._modelName = new System.Windows.Forms.TextBox();
            this._usesInheritance = new System.Windows.Forms.CheckBox();
            this.OKButton = new System.Windows.Forms.Button();
            this._baseModel = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._baseNamespace = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this._comboBox1 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this._docuLink = new System.Windows.Forms.LinkLabel();
            this._wizardDocu = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(273, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please enter the following information";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Name of model: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Uses Inheritance: ";
            // 
            // _modelName
            // 
            this._modelName.Location = new System.Drawing.Point(128, 46);
            this._modelName.Name = "_modelName";
            this._modelName.Size = new System.Drawing.Size(158, 20);
            this._modelName.TabIndex = 3;
            this._modelName.Text = "MyModel";
            // 
            // _usesInheritance
            // 
            this._usesInheritance.AutoSize = true;
            this._usesInheritance.Location = new System.Drawing.Point(128, 102);
            this._usesInheritance.Name = "_usesInheritance";
            this._usesInheritance.Size = new System.Drawing.Size(15, 14);
            this._usesInheritance.TabIndex = 4;
            this._usesInheritance.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(211, 196);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 9;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // _baseModel
            // 
            this._baseModel.Location = new System.Drawing.Point(129, 128);
            this._baseModel.Name = "_baseModel";
            this._baseModel.Size = new System.Drawing.Size(157, 20);
            this._baseModel.TabIndex = 11;
            this._baseModel.Text = "Base";
            this._baseModel.TextChanged += new System.EventHandler(this._baseModel_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Inherited model: ";
            // 
            // _baseNamespace
            // 
            this._baseNamespace.Location = new System.Drawing.Point(128, 155);
            this._baseNamespace.Name = "_baseNamespace";
            this._baseNamespace.Size = new System.Drawing.Size(158, 20);
            this._baseNamespace.TabIndex = 13;
            this._baseNamespace.Text = "Marvin.Base.Model";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 158);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Inherited namespace: ";
            // 
            // _comboBox1
            // 
            this._comboBox1.FormattingEnabled = true;
            this._comboBox1.Items.AddRange(new object[] {
            "PostgreSQL",
            "SQL Server"});
            this._comboBox1.Location = new System.Drawing.Point(129, 73);
            this._comboBox1.Name = "_comboBox1";
            this._comboBox1.Size = new System.Drawing.Size(157, 21);
            this._comboBox1.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Server type: ";
            // 
            // _docuLink
            // 
            this._docuLink.AutoSize = true;
            this._docuLink.Location = new System.Drawing.Point(14, 185);
            this._docuLink.Name = "_docuLink";
            this._docuLink.Size = new System.Drawing.Size(134, 13);
            this._docuLink.TabIndex = 16;
            this._docuLink.TabStop = true;
            this._docuLink.Text = "DataModel-Documentation";
            this._docuLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._docuLink_LinkClicked);
            // 
            // _wizardDocu
            // 
            this._wizardDocu.AutoSize = true;
            this._wizardDocu.Location = new System.Drawing.Point(14, 206);
            this._wizardDocu.Name = "_wizardDocu";
            this._wizardDocu.Size = new System.Drawing.Size(115, 13);
            this._wizardDocu.TabIndex = 17;
            this._wizardDocu.TabStop = true;
            this._wizardDocu.Text = "Wizard-Documentation";
            this._wizardDocu.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._wizardDocu_LinkClicked);
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 232);
            this.Controls.Add(this._wizardDocu);
            this.Controls.Add(this._docuLink);
            this.Controls.Add(this.label6);
            this.Controls.Add(this._comboBox1);
            this.Controls.Add(this._baseNamespace);
            this.Controls.Add(this.label5);
            this.Controls.Add(this._baseModel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this._usesInheritance);
            this.Controls.Add(this._modelName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "InputForm";
            this.Text = "DataModel Wizard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _modelName;
        private System.Windows.Forms.CheckBox _usesInheritance;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.TextBox _baseModel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _baseNamespace;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox _comboBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel _docuLink;
        private System.Windows.Forms.LinkLabel _wizardDocu;
    }
}