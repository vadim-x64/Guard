using System;
using System.IO;
using System.Windows.Forms;

namespace Guard {
    public partial class Form3 : Form {
        public string NewFileName { get; private set; }
        public Form1 form1;

        public Form3(string filePath, Form1 form1) {
            InitializeComponent();
            textBox1.Text = Path.GetFileNameWithoutExtension(filePath);
            label2.Text = Path.GetExtension(filePath);
            this.form1 = form1;
        }

        private void button1_Click(object sender, EventArgs e) {
            NewFileName = textBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(NewFileName)) {
                MessageBox.Show("Будь ласка, введіть назву файлу!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            this.Hide();
            form1.Show();
        }
    }
}