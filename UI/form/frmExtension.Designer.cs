namespace QuickGenerator.UI.form
{
    partial class NameExtensionForm
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
            this.textBoxOnlyWord1 = new QuickGenerator.UI.form.TextBoxOnlyWord();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxOnlyWord1
            // 
            this.textBoxOnlyWord1.Location = new System.Drawing.Point(12, 12);
            this.textBoxOnlyWord1.Name = "textBoxOnlyWord1";
            this.textBoxOnlyWord1.Size = new System.Drawing.Size(100, 20);
            this.textBoxOnlyWord1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(119, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(36, 20);
            this.button1.TabIndex = 2;
            this.button1.Text = "add";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmExtension
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(164, 48);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxOnlyWord1);
            this.Name = "frmExtension";
            this.Load += new System.EventHandler(this.frmExtension_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBoxOnlyWord textBoxOnlyWord1;
        private System.Windows.Forms.Button button1;
    }
}