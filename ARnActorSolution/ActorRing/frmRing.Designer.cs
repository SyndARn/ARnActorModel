namespace ActorRing
{
    partial class frmRing
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbRingSize = new System.Windows.Forms.TextBox();
            this.tbMessageQuantity = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btStartStop = new System.Windows.Forms.Button();
            this.lblDuration = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter a ring size";
            // 
            // tbRingSize
            // 
            this.tbRingSize.Location = new System.Drawing.Point(228, 43);
            this.tbRingSize.Name = "tbRingSize";
            this.tbRingSize.Size = new System.Drawing.Size(100, 20);
            this.tbRingSize.TabIndex = 1;
            this.tbRingSize.Text = "100";
            // 
            // tbMessageQuantity
            // 
            this.tbMessageQuantity.Location = new System.Drawing.Point(228, 73);
            this.tbMessageQuantity.Name = "tbMessageQuantity";
            this.tbMessageQuantity.Size = new System.Drawing.Size(100, 20);
            this.tbMessageQuantity.TabIndex = 3;
            this.tbMessageQuantity.Text = "100";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Enter quantity of message";
            // 
            // btStartStop
            // 
            this.btStartStop.Location = new System.Drawing.Point(66, 129);
            this.btStartStop.Name = "btStartStop";
            this.btStartStop.Size = new System.Drawing.Size(75, 23);
            this.btStartStop.TabIndex = 4;
            this.btStartStop.Text = "Start";
            this.btStartStop.UseVisualStyleBackColor = true;
            this.btStartStop.Click += new System.EventHandler(this.btStartStop_Click);
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(228, 138);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(53, 13);
            this.lblDuration.TabIndex = 5;
            this.lblDuration.Text = "Duration :";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 272);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.btStartStop);
            this.Controls.Add(this.tbMessageQuantity);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbRingSize);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Actor Ring Application";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbRingSize;
        private System.Windows.Forms.TextBox tbMessageQuantity;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btStartStop;
        private System.Windows.Forms.Label lblDuration;
    }
}

