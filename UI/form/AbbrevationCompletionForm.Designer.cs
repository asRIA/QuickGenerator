namespace QuickGenerator.UI.form
{
    partial class AbbrevationCompletionForm
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

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.lsvCodeTemplate = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnModify = new System.Windows.Forms.Button();
            this.lstPlaceHolder = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAddLanguageExt = new System.Windows.Forms.Button();
            this.btnRemoveLanguage = new System.Windows.Forms.Button();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.txtAbbrevation = new QuickGenerator.UI.form.TextBoxOnlyWord();
            this.label3 = new System.Windows.Forms.Label();
            this.btnFormList = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtExpandesText = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkColorArgument = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lsvCodeTemplate
            // 
            this.lsvCodeTemplate.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lsvCodeTemplate.FullRowSelect = true;
            this.lsvCodeTemplate.GridLines = true;
            this.lsvCodeTemplate.Location = new System.Drawing.Point(9, 66);
            this.lsvCodeTemplate.MultiSelect = false;
            this.lsvCodeTemplate.Name = "lsvCodeTemplate";
            this.lsvCodeTemplate.Size = new System.Drawing.Size(710, 224);
            this.lsvCodeTemplate.TabIndex = 17;
            this.lsvCodeTemplate.UseCompatibleStateImageBehavior = false;
            this.lsvCodeTemplate.View = System.Windows.Forms.View.Details;
            this.lsvCodeTemplate.SelectedIndexChanged += new System.EventHandler(this.lsvCodeTemplate_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Abbreviation";
            this.columnHeader1.Width = 173;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Expanded Text";
            this.columnHeader2.Width = 523;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 310);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Abbreviation";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(725, 130);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(63, 27);
            this.btnRemove.TabIndex = 12;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Visible = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(725, 81);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(63, 27);
            this.btnAdd.TabIndex = 11;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(570, 570);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(65, 21);
            this.btnConfirm.TabIndex = 18;
            this.btnConfirm.Text = "Apply";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Visible = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(490, 570);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(65, 21);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnModify
            // 
            this.btnModify.Location = new System.Drawing.Point(725, 179);
            this.btnModify.Name = "btnModify";
            this.btnModify.Size = new System.Drawing.Size(63, 27);
            this.btnModify.TabIndex = 20;
            this.btnModify.Text = "Modify";
            this.btnModify.UseVisualStyleBackColor = true;
            this.btnModify.Visible = false;
            this.btnModify.Click += new System.EventHandler(this.btnModify_Click);
            // 
            // lstPlaceHolder
            // 
            this.lstPlaceHolder.FormattingEnabled = true;
            this.lstPlaceHolder.Location = new System.Drawing.Point(664, 379);
            this.lstPlaceHolder.Name = "lstPlaceHolder";
            this.lstPlaceHolder.Size = new System.Drawing.Size(133, 186);
            this.lstPlaceHolder.TabIndex = 21;
            this.lstPlaceHolder.DoubleClick += new System.EventHandler(this.lstPlaceHolder_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnAddLanguageExt);
            this.groupBox1.Controls.Add(this.btnRemoveLanguage);
            this.groupBox1.Controls.Add(this.cmbLanguage);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(697, 48);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Extension";
            // 
            // btnAddLanguageExt
            // 
            this.btnAddLanguageExt.Location = new System.Drawing.Point(231, 18);
            this.btnAddLanguageExt.Name = "btnAddLanguageExt";
            this.btnAddLanguageExt.Size = new System.Drawing.Size(28, 21);
            this.btnAddLanguageExt.TabIndex = 28;
            this.btnAddLanguageExt.Text = "+";
            this.btnAddLanguageExt.UseVisualStyleBackColor = true;
            this.btnAddLanguageExt.Click += new System.EventHandler(this.btnAddLanguageExt_Click);
            // 
            // btnRemoveLanguage
            // 
            this.btnRemoveLanguage.Location = new System.Drawing.Point(202, 18);
            this.btnRemoveLanguage.Name = "btnRemoveLanguage";
            this.btnRemoveLanguage.Size = new System.Drawing.Size(28, 21);
            this.btnRemoveLanguage.TabIndex = 27;
            this.btnRemoveLanguage.Text = "-";
            this.btnRemoveLanguage.UseVisualStyleBackColor = true;
            this.btnRemoveLanguage.Click += new System.EventHandler(this.btnRemoveLanguage_Click);
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.FormattingEnabled = true;
            this.cmbLanguage.Items.AddRange(new object[] {
            "as",
            "hx",
            "Other"});
            this.cmbLanguage.Location = new System.Drawing.Point(9, 19);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(169, 21);
            this.cmbLanguage.TabIndex = 26;
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.cmbLanguage_SelectedIndexChanged);
            // 
            // txtAbbrevation
            // 
            this.txtAbbrevation.Enabled = false;
            this.txtAbbrevation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAbbrevation.Location = new System.Drawing.Point(100, 305);
            this.txtAbbrevation.Name = "txtAbbrevation";
            this.txtAbbrevation.Size = new System.Drawing.Size(401, 23);
            this.txtAbbrevation.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(647, 379);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "<-";
            // 
            // btnFormList
            // 
            this.btnFormList.Location = new System.Drawing.Point(531, 305);
            this.btnFormList.Name = "btnFormList";
            this.btnFormList.Size = new System.Drawing.Size(104, 22);
            this.btnFormList.TabIndex = 30;
            this.btnFormList.Text = "Show Custom List";
            this.btnFormList.UseVisualStyleBackColor = true;
            this.btnFormList.Click += new System.EventHandler(this.btnFormList_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(661, 359);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Double Click to insert";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtExpandesText);
            this.groupBox2.Location = new System.Drawing.Point(9, 339);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(632, 228);
            this.groupBox2.TabIndex = 32;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Expanded Text";
            // 
            // txtExpandesText
            // 
            this.txtExpandesText.AcceptsTab = true;
            this.txtExpandesText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtExpandesText.Enabled = false;
            this.txtExpandesText.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExpandesText.Location = new System.Drawing.Point(3, 18);
            this.txtExpandesText.Name = "txtExpandesText";
            this.txtExpandesText.Size = new System.Drawing.Size(623, 204);
            this.txtExpandesText.TabIndex = 34;
            this.txtExpandesText.Text = "";
            this.txtExpandesText.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 572);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(447, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Alt + q = Show list Abbreviation | Alt + c = Show List of Custom List | Alt + a =" +
                " Show arguments";
            // 
            // chkColorArgument
            // 
            this.chkColorArgument.AutoSize = true;
            this.chkColorArgument.Location = new System.Drawing.Point(664, 572);
            this.chkColorArgument.Name = "chkColorArgument";
            this.chkColorArgument.Size = new System.Drawing.Size(102, 17);
            this.chkColorArgument.TabIndex = 34;
            this.chkColorArgument.Text = "Color arguments";
            this.chkColorArgument.UseVisualStyleBackColor = true;
            this.chkColorArgument.CheckedChanged += new System.EventHandler(this.chkColorArgument_CheckedChanged);
            // 
            // AbbrevationCompletionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 596);
            this.Controls.Add(this.chkColorArgument);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnFormList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtAbbrevation);
            this.Controls.Add(this.lstPlaceHolder);
            this.Controls.Add(this.btnModify);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.lsvCodeTemplate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.KeyPreview = true;
            this.Name = "AbbrevationCompletionForm";
            this.Text = "Abbreviations manager";
            this.Load += new System.EventHandler(this.frmAbbrevationCompletion_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmAbbrevationCompletion_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lsvCodeTemplate;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnModify;
        private System.Windows.Forms.ListBox lstPlaceHolder;
        private TextBoxOnlyWord txtAbbrevation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAddLanguageExt;
        private System.Windows.Forms.Button btnRemoveLanguage;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnFormList;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox txtExpandesText;
        private System.Windows.Forms.CheckBox chkColorArgument;
    }
}
