namespace Ground_base_software
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            label7 = new Label();
            button1 = new Button();
            button2 = new Button();
            TextBox1 = new TextBox();
            SSHoutput = new RichTextBox();
            Interupt = new Button();
            label13 = new Label();
            label14 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(214, 43);
            label1.Name = "label1";
            label1.Size = new Size(59, 25);
            label1.TabIndex = 0;
            label1.Text = "label1";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(73, 43);
            label2.Name = "label2";
            label2.Size = new Size(135, 25);
            label2.TabIndex = 1;
            label2.Text = "Signal strength:";
            label2.Click += label2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(73, 79);
            label3.Name = "label3";
            label3.Size = new Size(187, 25);
            label3.TabIndex = 2;
            label3.Text = "Messages per second:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(263, 79);
            label4.Name = "label4";
            label4.Size = new Size(59, 25);
            label4.TabIndex = 3;
            label4.Text = "label4";
            label4.Click += label4_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(230, 119);
            label5.Name = "label5";
            label5.Size = new Size(59, 25);
            label5.TabIndex = 7;
            label5.Text = "label5";
            label5.Click += label5_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(73, 119);
            label6.Name = "label6";
            label6.Size = new Size(151, 25);
            label6.TabIndex = 8;
            label6.Text = "CPU temperature:";
            label6.Click += label6_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(180, 154);
            label8.Name = "label8";
            label8.Size = new Size(59, 25);
            label8.TabIndex = 10;
            label8.Text = "label8";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(186, 188);
            label9.Name = "label9";
            label9.Size = new Size(59, 25);
            label9.TabIndex = 11;
            label9.Text = "label9";
            label9.Click += label9_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(73, 154);
            label10.Name = "label10";
            label10.Size = new Size(101, 25);
            label10.TabIndex = 12;
            label10.Text = "CPU usage:";
            label10.Click += label10_Click;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(73, 188);
            label11.Name = "label11";
            label11.Size = new Size(107, 25);
            label11.TabIndex = 13;
            label11.Text = "RAM usage:";
            label11.Click += label11_Click;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(73, 225);
            label12.Name = "label12";
            label12.Size = new Size(101, 25);
            label12.TabIndex = 14;
            label12.Text = "Airplane IP:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(186, 225);
            label7.Name = "label7";
            label7.Size = new Size(59, 25);
            label7.TabIndex = 15;
            label7.Text = "label7";
            // 
            // button1
            // 
            button1.Location = new Point(907, 970);
            button1.Name = "button1";
            button1.Size = new Size(235, 34);
            button1.TabIndex = 16;
            button1.Text = "Restart video reciever";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(666, 970);
            button2.Name = "button2";
            button2.Size = new Size(235, 34);
            button2.TabIndex = 17;
            button2.Text = "Open SSH";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // TextBox1
            // 
            TextBox1.Location = new Point(428, 933);
            TextBox1.Name = "TextBox1";
            TextBox1.Size = new Size(714, 31);
            TextBox1.TabIndex = 19;
            TextBox1.TextChanged += textBox1_TextChanged_1;
            TextBox1.KeyDown += textBox1_KeyDown;
            // 
            // SSHoutput
            // 
            SSHoutput.BackColor = SystemColors.ActiveCaptionText;
            SSHoutput.ForeColor = Color.FromArgb(0, 64, 0);
            SSHoutput.Location = new Point(428, 43);
            SSHoutput.Name = "SSHoutput";
            SSHoutput.ReadOnly = true;
            SSHoutput.Size = new Size(714, 884);
            SSHoutput.TabIndex = 20;
            SSHoutput.Text = "";
            // 
            // Interupt
            // 
            Interupt.Location = new Point(428, 970);
            Interupt.Name = "Interupt";
            Interupt.Size = new Size(235, 34);
            Interupt.TabIndex = 21;
            Interupt.Text = "Interrupt (Ctrl+C)";
            Interupt.UseVisualStyleBackColor = true;
            Interupt.Click += Interupt_Click;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(73, 259);
            label13.Name = "label13";
            label13.Size = new Size(155, 25);
            label13.TabIndex = 22;
            label13.Text = "Local UI response:";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(220, 259);
            label14.Name = "label14";
            label14.Size = new Size(69, 25);
            label14.TabIndex = 23;
            label14.Text = "label14";
            label14.Click += label14_Click_1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2421, 1440);
            Controls.Add(label14);
            Controls.Add(label13);
            Controls.Add(Interupt);
            Controls.Add(SSHoutput);
            Controls.Add(TextBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label7);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            Shown += Form1_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label7;
        private Button button1;
        private Button button2;
        private TextBox TextBox1;
        private RichTextBox SSHoutput;
        private Button Interupt;
        private Label label13;
        private Label label14;
    }
}
