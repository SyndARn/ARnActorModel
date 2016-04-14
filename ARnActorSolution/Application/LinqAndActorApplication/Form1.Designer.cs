namespace LinqAndActorApplication
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
            this.LinqOperation = new System.Windows.Forms.Button();
            this.tbSource = new System.Windows.Forms.TextBox();
            this.tbTarget = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LinqOperation
            // 
            this.LinqOperation.Location = new System.Drawing.Point(86, 193);
            this.LinqOperation.Name = "LinqOperation";
            this.LinqOperation.Size = new System.Drawing.Size(101, 23);
            this.LinqOperation.TabIndex = 0;
            this.LinqOperation.Text = "LinqOperation";
            this.LinqOperation.UseVisualStyleBackColor = true;
            this.LinqOperation.Click += new System.EventHandler(this.LinqOperation_Click);
            // 
            // tbSource
            // 
            this.tbSource.Location = new System.Drawing.Point(261, 25);
            this.tbSource.Multiline = true;
            this.tbSource.Name = "tbSource";
            this.tbSource.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbSource.Size = new System.Drawing.Size(412, 172);
            this.tbSource.TabIndex = 1;
            this.tbSource.WordWrap = false;
            // 
            // tbTarget
            // 
            this.tbTarget.Location = new System.Drawing.Point(261, 215);
            this.tbTarget.Multiline = true;
            this.tbTarget.Name = "tbTarget";
            this.tbTarget.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbTarget.Size = new System.Drawing.Size(412, 172);
            this.tbTarget.TabIndex = 2;
            this.tbTarget.WordWrap = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 422);
            this.Controls.Add(this.tbTarget);
            this.Controls.Add(this.tbSource);
            this.Controls.Add(this.LinqOperation);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LinqOperation;
        private System.Windows.Forms.TextBox tbSource;
        private System.Windows.Forms.TextBox tbTarget;
    }
}

