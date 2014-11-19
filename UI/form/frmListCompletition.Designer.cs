namespace QuickGenerator.UI
{
    partial class frmListCompletition
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
            this.lstString = new System.Windows.Forms.ListBox();
            this.btnAddKey = new System.Windows.Forms.Button();
            this.btnRemoveKey = new System.Windows.Forms.Button();
            this.btnUP = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnInsertText = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lsName = new System.Windows.Forms.ListBox();
            this.txtNameList = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstString
            // 
            this.lstString.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstString.FormattingEnabled = true;
            this.lstString.ItemHeight = 15;
            this.lstString.Location = new System.Drawing.Point(21, 90);
            this.lstString.Name = "lstString";
            this.lstString.Size = new System.Drawing.Size(231, 214);
            this.lstString.TabIndex = 1;
            this.lstString.DoubleClick += new System.EventHandler(this.lstString_DoubleClick);
            // 
            // btnAddKey
            // 
            this.btnAddKey.Location = new System.Drawing.Point(151, 32);
            this.btnAddKey.Name = "btnAddKey";
            this.btnAddKey.Size = new System.Drawing.Size(36, 21);
            this.btnAddKey.TabIndex = 2;
            this.btnAddKey.Text = "+";
            this.btnAddKey.UseVisualStyleBackColor = true;
            this.btnAddKey.Click += new System.EventHandler(this.btnAddKey_Click);
            // 
            // btnRemoveKey
            // 
            this.btnRemoveKey.Location = new System.Drawing.Point(193, 33);
            this.btnRemoveKey.Name = "btnRemoveKey";
            this.btnRemoveKey.Size = new System.Drawing.Size(35, 21);
            this.btnRemoveKey.TabIndex = 3;
            this.btnRemoveKey.Text = "-";
            this.btnRemoveKey.UseVisualStyleBackColor = true;
            this.btnRemoveKey.Click += new System.EventHandler(this.btnRemoveKey_Click);
            // 
            // btnUP
            // 
            this.btnUP.Location = new System.Drawing.Point(258, 130);
            this.btnUP.Name = "btnUP";
            this.btnUP.Size = new System.Drawing.Size(49, 26);
            this.btnUP.TabIndex = 4;
            this.btnUP.Text = "Up";
            this.btnUP.UseVisualStyleBackColor = true;
            this.btnUP.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(258, 176);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(49, 29);
            this.btnDown.TabIndex = 5;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Times New Roman", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(23, 33);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(231, 23);
            this.textBox1.TabIndex = 6;
            // 
            // btnInsertText
            // 
            this.btnInsertText.Location = new System.Drawing.Point(260, 32);
            this.btnInsertText.Name = "btnInsertText";
            this.btnInsertText.Size = new System.Drawing.Size(49, 21);
            this.btnInsertText.TabIndex = 7;
            this.btnInsertText.Text = "+";
            this.btnInsertText.UseVisualStyleBackColor = true;
            this.btnInsertText.Click += new System.EventHandler(this.btnInsertText_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(258, 90);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(49, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "X";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lsName);
            this.groupBox1.Controls.Add(this.txtNameList);
            this.groupBox1.Controls.Add(this.btnAddKey);
            this.groupBox1.Controls.Add(this.btnRemoveKey);
            this.groupBox1.Location = new System.Drawing.Point(23, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(243, 313);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "List Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Double Click for Edit.";
            // 
            // lsName
            // 
            this.lsName.FormattingEnabled = true;
            this.lsName.Location = new System.Drawing.Point(6, 90);
            this.lsName.Name = "lsName";
            this.lsName.Size = new System.Drawing.Size(216, 212);
            this.lsName.TabIndex = 11;
            this.lsName.SelectedIndexChanged += new System.EventHandler(this.lsName_SelectedIndexChanged);
            this.lsName.DoubleClick += new System.EventHandler(this.lsName_DoubleClick);
            // 
            // txtNameList
            // 
            this.txtNameList.Location = new System.Drawing.Point(6, 33);
            this.txtNameList.Name = "txtNameList";
            this.txtNameList.Size = new System.Drawing.Size(139, 20);
            this.txtNameList.TabIndex = 12;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.lstString);
            this.groupBox2.Controls.Add(this.btnUP);
            this.groupBox2.Controls.Add(this.btnInsertText);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.btnDown);
            this.groupBox2.Location = new System.Drawing.Point(295, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(322, 313);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "List";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Double Click for Edit.";
            // 
            // frmListCompletition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 342);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmListCompletition";
            this.Text = "frmListCompletition";
            this.Load += new System.EventHandler(this.frmListCompletition_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmListCompletition_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstString;
        private System.Windows.Forms.Button btnAddKey;
        private System.Windows.Forms.Button btnRemoveKey;
        private System.Windows.Forms.Button btnUP;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnInsertText;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lsName;
        private System.Windows.Forms.TextBox txtNameList;
        private System.Windows.Forms.Label label2;
    }
}