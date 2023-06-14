namespace client_application
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
            this.ip_textbox = new System.Windows.Forms.TextBox();
            this.port_textbox = new System.Windows.Forms.TextBox();
            this.name_textbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.connect_button = new System.Windows.Forms.Button();
            this.server_richtextbox = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.messages_textbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.send_button = new System.Windows.Forms.Button();
            this.disconnect_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ip_textbox
            // 
            this.ip_textbox.Location = new System.Drawing.Point(90, 56);
            this.ip_textbox.Name = "ip_textbox";
            this.ip_textbox.Size = new System.Drawing.Size(100, 20);
            this.ip_textbox.TabIndex = 0;
            // 
            // port_textbox
            // 
            this.port_textbox.Location = new System.Drawing.Point(90, 94);
            this.port_textbox.Name = "port_textbox";
            this.port_textbox.Size = new System.Drawing.Size(100, 20);
            this.port_textbox.TabIndex = 1;
            // 
            // name_textbox
            // 
            this.name_textbox.Location = new System.Drawing.Point(90, 133);
            this.name_textbox.Name = "name_textbox";
            this.name_textbox.Size = new System.Drawing.Size(100, 20);
            this.name_textbox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Server IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Server Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Name";
            // 
            // connect_button
            // 
            this.connect_button.Location = new System.Drawing.Point(61, 173);
            this.connect_button.Name = "connect_button";
            this.connect_button.Size = new System.Drawing.Size(75, 23);
            this.connect_button.TabIndex = 6;
            this.connect_button.Text = "Connect";
            this.connect_button.UseVisualStyleBackColor = true;
            this.connect_button.Click += new System.EventHandler(this.connect_button_Click);
            // 
            // server_richtextbox
            // 
            this.server_richtextbox.Location = new System.Drawing.Point(284, 56);
            this.server_richtextbox.Name = "server_richtextbox";
            this.server_richtextbox.ReadOnly = true;
            this.server_richtextbox.Size = new System.Drawing.Size(340, 256);
            this.server_richtextbox.TabIndex = 7;
            this.server_richtextbox.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(403, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Server Messages";
            // 
            // messages_textbox
            // 
            this.messages_textbox.Enabled = false;
            this.messages_textbox.Location = new System.Drawing.Point(406, 331);
            this.messages_textbox.Name = "messages_textbox";
            this.messages_textbox.Size = new System.Drawing.Size(133, 20);
            this.messages_textbox.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(345, 334);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Message";
            // 
            // send_button
            // 
            this.send_button.Enabled = false;
            this.send_button.Location = new System.Drawing.Point(436, 357);
            this.send_button.Name = "send_button";
            this.send_button.Size = new System.Drawing.Size(75, 23);
            this.send_button.TabIndex = 11;
            this.send_button.Text = "Send";
            this.send_button.UseVisualStyleBackColor = true;
            this.send_button.Click += new System.EventHandler(this.send_button_Click);
            // 
            // disconnect_button
            // 
            this.disconnect_button.Enabled = false;
            this.disconnect_button.Location = new System.Drawing.Point(151, 173);
            this.disconnect_button.Name = "disconnect_button";
            this.disconnect_button.Size = new System.Drawing.Size(75, 23);
            this.disconnect_button.TabIndex = 12;
            this.disconnect_button.Text = "Disconnect";
            this.disconnect_button.UseVisualStyleBackColor = true;
            this.disconnect_button.Click += new System.EventHandler(this.disconnect_button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 402);
            this.Controls.Add(this.disconnect_button);
            this.Controls.Add(this.send_button);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.messages_textbox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.server_richtextbox);
            this.Controls.Add(this.connect_button);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.name_textbox);
            this.Controls.Add(this.port_textbox);
            this.Controls.Add(this.ip_textbox);
            this.Name = "Form1";
            this.Text = "Tic-Tac-Toe Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ip_textbox;
        private System.Windows.Forms.TextBox port_textbox;
        private System.Windows.Forms.TextBox name_textbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button connect_button;
        private System.Windows.Forms.RichTextBox server_richtextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox messages_textbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button send_button;
        private System.Windows.Forms.Button disconnect_button;
    }
}

