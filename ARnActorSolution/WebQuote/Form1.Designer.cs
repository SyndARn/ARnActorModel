namespace WebQuote
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbCCY1 = new System.Windows.Forms.TextBox();
            this.tbCCY2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tbQuote = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbCCY1
            // 
            this.tbCCY1.Location = new System.Drawing.Point(82, 73);
            this.tbCCY1.Name = "tbCCY1";
            this.tbCCY1.Size = new System.Drawing.Size(100, 20);
            this.tbCCY1.TabIndex = 0;
            this.tbCCY1.Text = "EUR";
            // 
            // tbCCY2
            // 
            this.tbCCY2.Location = new System.Drawing.Point(227, 73);
            this.tbCCY2.Name = "tbCCY2";
            this.tbCCY2.Size = new System.Drawing.Size(100, 20);
            this.tbCCY2.TabIndex = 1;
            this.tbCCY2.Text = "USD";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(92, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbQuote
            // 
            this.tbQuote.Location = new System.Drawing.Point(90, 135);
            this.tbQuote.Multiline = true;
            this.tbQuote.Name = "tbQuote";
            this.tbQuote.Size = new System.Drawing.Size(463, 110);
            this.tbQuote.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 261);
            this.Controls.Add(this.tbQuote);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbCCY2);
            this.Controls.Add(this.tbCCY1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbCCY1;
        private System.Windows.Forms.TextBox tbCCY2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbQuote;
    }
}

