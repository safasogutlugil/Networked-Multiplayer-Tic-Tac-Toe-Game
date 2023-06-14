namespace server_application
{
    partial class Form1
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
            this.port_textbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listen_button = new System.Windows.Forms.Button();
            this.server_richtextbox = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.start_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // port_textbox
            // 
            this.port_textbox.Location = new System.Drawing.Point(67, 43);
            this.port_textbox.Name = "port_textbox";
            this.port_textbox.Size = new System.Drawing.Size(100, 20);
            this.port_textbox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port";
            // 
            // listen_button
            // 
            this.listen_button.Location = new System.Drawing.Point(194, 40);
            this.listen_button.Name = "listen_button";
            this.listen_button.Size = new System.Drawing.Size(75, 23);
            this.listen_button.TabIndex = 2;
            this.listen_button.Text = "Listen";
            this.listen_button.UseVisualStyleBackColor = true;
            this.listen_button.Click += new System.EventHandler(this.listen_button_Click);
            // 
            // server_richtextbox
            // 
            this.server_richtextbox.Location = new System.Drawing.Point(67, 97);
            this.server_richtextbox.Name = "server_richtextbox";
            this.server_richtextbox.ReadOnly = true;
            this.server_richtextbox.Size = new System.Drawing.Size(500, 352);
            this.server_richtextbox.TabIndex = 4;
            this.server_richtextbox.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(285, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Messages";
            // 
            // start_button
            // 
            this.start_button.Enabled = false;
            this.start_button.Location = new System.Drawing.Point(307, 40);
            this.start_button.Name = "start_button";
            this.start_button.Size = new System.Drawing.Size(75, 23);
            this.start_button.TabIndex = 6;
            this.start_button.Text = "Start Game";
            this.start_button.UseVisualStyleBackColor = true;
            this.start_button.Click += new System.EventHandler(this.start_button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 479);
            this.Controls.Add(this.start_button);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.server_richtextbox);
            this.Controls.Add(this.listen_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.port_textbox);
            this.Name = "Form1";
            this.Text = "Tic-Tac-Toe Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox port_textbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button listen_button;
        private System.Windows.Forms.RichTextBox server_richtextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button start_button;
    }
}

