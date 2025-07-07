using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ground_base_software
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public string ConnectingText
        {
            get => label1.Text;
            set => label1.Text = value;
        }

        private Label ConnectingLabel;

        private void InitializeComponent()
        {
            ConnectingLabel = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // ConnectingLabel
            // 
            ConnectingLabel.Font = new Font("Segoe UI", 20F);
            ConnectingLabel.ForeColor = Color.Green;
            ConnectingLabel.Location = new Point(46, 35);
            ConnectingLabel.Name = "ConnectingLabel";
            ConnectingLabel.Size = new Size(699, 53);
            ConnectingLabel.TabIndex = 0;
            ConnectingLabel.Text = "Connecting...";
            ConnectingLabel.Click += ConnectingLabel_Click;
            // 
            // label1
            // 
            label1.Location = new Point(46, 104);
            label1.Name = "label1";
            label1.Size = new Size(691, 170);
            label1.TabIndex = 1;
            label1.Text = "IP";
            // 
            // Form2
            // 
            ClientSize = new Size(781, 321);
            Controls.Add(label1);
            Controls.Add(ConnectingLabel);
            Name = "Form2";
            Load += Form2_Load;
            ResumeLayout(false);
        }

        private Label label1;

        private void ConnectingLabel_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
