namespace QuickGenerator.UI.form
{
    partial class CreateClassfrm 
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Liberare le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnPackage = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dynamicCheck = new System.Windows.Forms.CheckBox();
            this.finalCheck = new System.Windows.Forms.CheckBox();
            this.rdbInternal = new System.Windows.Forms.RadioButton();
            this.rdbPublic = new System.Windows.Forms.RadioButton();
            this.btnCreateClass = new System.Windows.Forms.Button();
            this.chkClose = new System.Windows.Forms.CheckBox();
            this.btnSaveSetting = new System.Windows.Forms.Button();
            this.txtExtends = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInterface = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lstInterface = new System.Windows.Forms.ListBox();
            this.btnAddInterface = new System.Windows.Forms.Button();
            this.btnRemoveInterface = new System.Windows.Forms.Button();
            this.errorLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.constructorCheck = new System.Windows.Forms.CheckBox();
            this.superCheck = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtNameClass = new QuickGenerator.UI.form.TextBoxOnlyWord();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source Folder";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Enabled = false;
            this.txtFilePath.Location = new System.Drawing.Point(90, 21);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(341, 20);
            this.txtFilePath.TabIndex = 1;
            // 
            // btnPackage
            // 
            this.btnPackage.Location = new System.Drawing.Point(435, 21);
            this.btnPackage.Name = "btnPackage";
            this.btnPackage.Size = new System.Drawing.Size(28, 20);
            this.btnPackage.TabIndex = 4;
            this.btnPackage.Text = "...";
            this.btnPackage.UseVisualStyleBackColor = true;
            this.btnPackage.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dynamicCheck);
            this.groupBox1.Controls.Add(this.finalCheck);
            this.groupBox1.Controls.Add(this.rdbInternal);
            this.groupBox1.Controls.Add(this.rdbPublic);
            this.groupBox1.Location = new System.Drawing.Point(14, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(417, 66);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Modifiers";
            // 
            // dynamicCheck
            // 
            this.dynamicCheck.AutoSize = true;
            this.dynamicCheck.Location = new System.Drawing.Point(229, 30);
            this.dynamicCheck.Name = "dynamicCheck";
            this.dynamicCheck.Size = new System.Drawing.Size(65, 17);
            this.dynamicCheck.TabIndex = 23;
            this.dynamicCheck.Text = "dynamic";
            this.dynamicCheck.UseVisualStyleBackColor = true;
            // 
            // finalCheck
            // 
            this.finalCheck.AutoSize = true;
            this.finalCheck.Location = new System.Drawing.Point(346, 30);
            this.finalCheck.Name = "finalCheck";
            this.finalCheck.Size = new System.Drawing.Size(48, 17);
            this.finalCheck.TabIndex = 24;
            this.finalCheck.Text = "Final";
            this.finalCheck.UseVisualStyleBackColor = true;
            // 
            // rdbInternal
            // 
            this.rdbInternal.AutoSize = true;
            this.rdbInternal.Location = new System.Drawing.Point(111, 30);
            this.rdbInternal.Name = "rdbInternal";
            this.rdbInternal.Size = new System.Drawing.Size(60, 17);
            this.rdbInternal.TabIndex = 1;
            this.rdbInternal.Text = "Internal";
            this.rdbInternal.UseVisualStyleBackColor = true;
            // 
            // rdbPublic
            // 
            this.rdbPublic.AutoSize = true;
            this.rdbPublic.Checked = true;
            this.rdbPublic.Location = new System.Drawing.Point(16, 30);
            this.rdbPublic.Name = "rdbPublic";
            this.rdbPublic.Size = new System.Drawing.Size(54, 17);
            this.rdbPublic.TabIndex = 11;
            this.rdbPublic.TabStop = true;
            this.rdbPublic.Text = "Public";
            this.rdbPublic.UseVisualStyleBackColor = true;
            // 
            // btnCreateClass
            // 
            this.btnCreateClass.Location = new System.Drawing.Point(388, 442);
            this.btnCreateClass.Name = "btnCreateClass";
            this.btnCreateClass.Size = new System.Drawing.Size(75, 23);
            this.btnCreateClass.TabIndex = 4;
            this.btnCreateClass.Text = "OK";
            this.btnCreateClass.UseVisualStyleBackColor = true;
            this.btnCreateClass.Click += new System.EventHandler(this.btnCreateClass_Click);
            // 
            // chkClose
            // 
            this.chkClose.AutoSize = true;
            this.chkClose.Checked = true;
            this.chkClose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkClose.Location = new System.Drawing.Point(12, 448);
            this.chkClose.Name = "chkClose";
            this.chkClose.Size = new System.Drawing.Size(109, 17);
            this.chkClose.TabIndex = 8;
            this.chkClose.Text = "Close after create";
            this.chkClose.UseVisualStyleBackColor = true;
            // 
            // btnSaveSetting
            // 
            this.btnSaveSetting.Enabled = false;
            this.btnSaveSetting.Location = new System.Drawing.Point(286, 442);
            this.btnSaveSetting.Name = "btnSaveSetting";
            this.btnSaveSetting.Size = new System.Drawing.Size(84, 23);
            this.btnSaveSetting.TabIndex = 9;
            this.btnSaveSetting.Text = "Save Settings";
            this.btnSaveSetting.UseVisualStyleBackColor = true;
            this.btnSaveSetting.Click += new System.EventHandler(this.btnSaveSetting_Click);
            // 
            // txtExtends
            // 
            this.txtExtends.Location = new System.Drawing.Point(89, 176);
            this.txtExtends.Name = "txtExtends";
            this.txtExtends.Size = new System.Drawing.Size(341, 20);
            this.txtExtends.TabIndex = 2;
            this.txtExtends.TextChanged += new System.EventHandler(this.txtExtends_TextChanged_1);
            this.txtExtends.Enter += new System.EventHandler(this.txtExtends_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Class Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 15;
            this.label3.Tag = "0";
            this.label3.Text = "Extends";
            // 
            // txtInterface
            // 
            this.txtInterface.Location = new System.Drawing.Point(90, 210);
            this.txtInterface.Name = "txtInterface";
            this.txtInterface.Size = new System.Drawing.Size(341, 20);
            this.txtInterface.TabIndex = 3;
            this.txtInterface.Enter += new System.EventHandler(this.txtInterface_Enter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 210);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Interfaces";
            // 
            // lstInterface
            // 
            this.lstInterface.FormattingEnabled = true;
            this.lstInterface.Location = new System.Drawing.Point(90, 245);
            this.lstInterface.Name = "lstInterface";
            this.lstInterface.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstInterface.Size = new System.Drawing.Size(341, 108);
            this.lstInterface.TabIndex = 18;
            // 
            // btnAddInterface
            // 
            this.btnAddInterface.Location = new System.Drawing.Point(437, 245);
            this.btnAddInterface.Name = "btnAddInterface";
            this.btnAddInterface.Size = new System.Drawing.Size(26, 25);
            this.btnAddInterface.TabIndex = 19;
            this.btnAddInterface.Text = "+";
            this.btnAddInterface.UseVisualStyleBackColor = true;
            this.btnAddInterface.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnRemoveInterface
            // 
            this.btnRemoveInterface.Location = new System.Drawing.Point(437, 276);
            this.btnRemoveInterface.Name = "btnRemoveInterface";
            this.btnRemoveInterface.Size = new System.Drawing.Size(26, 26);
            this.btnRemoveInterface.TabIndex = 20;
            this.btnRemoveInterface.Text = "-";
            this.btnRemoveInterface.UseVisualStyleBackColor = true;
            this.btnRemoveInterface.Click += new System.EventHandler(this.btnRemoveInterface_Click);
            // 
            // errorLabel
            // 
            this.errorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.errorLabel.AutoSize = true;
            this.errorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.errorLabel.Location = new System.Drawing.Point(341, 61);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(0, 13);
            this.errorLabel.TabIndex = 26;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.constructorCheck);
            this.groupBox2.Controls.Add(this.superCheck);
            this.groupBox2.Location = new System.Drawing.Point(89, 370);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(340, 66);
            this.groupBox2.TabIndex = 29;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Code generation:";
            // 
            // constructorCheck
            // 
            this.constructorCheck.AutoSize = true;
            this.constructorCheck.Enabled = false;
            this.constructorCheck.Location = new System.Drawing.Point(21, 19);
            this.constructorCheck.Name = "constructorCheck";
            this.constructorCheck.Size = new System.Drawing.Size(225, 17);
            this.constructorCheck.TabIndex = 25;
            this.constructorCheck.Text = "Generate constructor matching base class";
            this.constructorCheck.UseVisualStyleBackColor = true;
            // 
            // superCheck
            // 
            this.superCheck.AutoSize = true;
            this.superCheck.Enabled = false;
            this.superCheck.Location = new System.Drawing.Point(21, 40);
            this.superCheck.Name = "superCheck";
            this.superCheck.Size = new System.Drawing.Size(235, 17);
            this.superCheck.TabIndex = 26;
            this.superCheck.Text = "Generate interface methods implementations";
            this.superCheck.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(5, 165);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(462, 2);
            this.label5.TabIndex = 30;
            // 
            // txtNameClass
            // 
            this.txtNameClass.Location = new System.Drawing.Point(90, 58);
            this.txtNameClass.Name = "txtNameClass";
            this.txtNameClass.Size = new System.Drawing.Size(231, 20);
            this.txtNameClass.TabIndex = 1;
            // 
            // CreateClassfrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 472);
            this.Controls.Add(this.txtNameClass);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.btnRemoveInterface);
            this.Controls.Add(this.btnAddInterface);
            this.Controls.Add(this.lstInterface);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtInterface);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtExtends);
            this.Controls.Add(this.btnSaveSetting);
            this.Controls.Add(this.chkClose);
            this.Controls.Add(this.btnCreateClass);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnPackage);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.label1);
            this.Name = "CreateClassfrm";
            this.Text = "Create   Class";
            this.Load += new System.EventHandler(this.CreateClassAS3frm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CreateClassAS3frm_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CreateClassfrm_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

       

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnPackage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbInternal;
        private System.Windows.Forms.RadioButton rdbPublic;
        private System.Windows.Forms.Button btnCreateClass;
        private System.Windows.Forms.CheckBox chkClose;
        private System.Windows.Forms.Button btnSaveSetting;
        private System.Windows.Forms.TextBox txtExtends;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInterface;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lstInterface;
        private System.Windows.Forms.Button btnAddInterface;
        private System.Windows.Forms.Button btnRemoveInterface;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.CheckBox dynamicCheck;
        private System.Windows.Forms.CheckBox finalCheck;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox constructorCheck;
        private System.Windows.Forms.CheckBox superCheck;
        private System.Windows.Forms.Label label5;
        private TextBoxOnlyWord txtNameClass;
    }
}