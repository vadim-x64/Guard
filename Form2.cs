using NAudio.Wave;
using System;
using System.Drawing;
using System.Media;
using System.Reflection;
using System.Windows.Forms;

namespace Guard {
    public partial class Form2 : Form {
        private Form1 form1;
        private SoundPlayer player;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private Color label1FinalColor;
        private Color label2FinalColor;
        private Color label3FinalColor;
        float label1Opacity = 0.0f;
        float label2Opacity = 0.0f;
        float label3Opacity = 0.0f;
        int pictureBox1StartY;
        int pictureBox2StartY;
        float pictureBox3Opacity = 1.0f;
        Timer timer3;
        Timer labelFadeInTimer;

        public Form2(Form1 form1) {
            InitializeComponent();
            InitializePictureBoxesAndTimers();
            outputDevice = new WaveOutEvent();
            audioFile = new AudioFileReader(@"D:\Моя папка\Робочі\c#\Guard\bin\Debug\slide.mp3");
            audioFile.Volume = 1f;
            outputDevice.Init(audioFile);
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            this.form1 = form1;
        }

        private void InitializePictureBoxesAndTimers() {
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            pictureBox3.Visible = true;
            pictureBox3.BackColor = Color.White;
            Timer timer1And2 = new Timer();
            timer1And2.Interval = 10;
            timer1And2.Tick += Timer1And2_Tick;
            timer1And2.Start();
        }

        private void Timer1And2_Tick(object sender, EventArgs e) {
            bool pictureBox1Moved = false;
            bool pictureBox2Moved = false;
            if (pictureBox1.Top > pictureBox1StartY) {
                pictureBox1.Top -= 5;
                pictureBox1.Visible = true;
                pictureBox1Moved = true;
            }
            if (pictureBox2.Top < pictureBox2StartY) {
                pictureBox2.Top += 5;
                pictureBox2.Visible = true;
                pictureBox2Moved = true;
            }
            if (!pictureBox1Moved && !pictureBox2Moved) {
                ((Timer)sender).Stop();
                timer3 = new Timer();
                timer3.Interval = 50;
                timer3.Tick += Timer3_Tick;
                timer3.Start();
            }
        }

        private void Timer3_Tick(object sender, EventArgs e) {
            pictureBox3Opacity -= 0.05f;
            if (pictureBox3Opacity <= 0.0f) {
                pictureBox3Opacity = 0.0f;
                ((Timer)sender).Stop();
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                labelFadeInTimer = new Timer();
                labelFadeInTimer.Interval = 50;
                labelFadeInTimer.Tick += LabelFadeInTimer_Tick;
                labelFadeInTimer.Start();
            }
            pictureBox3.Invalidate();
        }

        private void LabelFadeInTimer_Tick(object sender, EventArgs e) {
            bool allLabelsFullyVisible = true;
            if (label1Opacity < 1.0f) {
                label1Opacity += 0.05f;
                if (label1Opacity > 1.0f) label1Opacity = 1.0f;
                allLabelsFullyVisible = false;
            }
            if (label2Opacity < 1.0f) {
                label2Opacity += 0.05f;
                if (label2Opacity > 1.0f) label2Opacity = 1.0f;
                allLabelsFullyVisible = false;
            }
            if (label3Opacity < 1.0f) {
                label3Opacity += 0.05f;
                if (label3Opacity > 1.0f) label3Opacity = 1.0f;
                allLabelsFullyVisible = false;
            }
            label1.ForeColor = Color.FromArgb(
                (int)(label1FinalColor.R * label1Opacity + 255 * (1 - label1Opacity)),
                (int)(label1FinalColor.G * label1Opacity + 255 * (1 - label1Opacity)),
                (int)(label1FinalColor.B * label1Opacity + 255 * (1 - label1Opacity))
            );
            label2.ForeColor = Color.FromArgb(
                (int)(label2FinalColor.R * label2Opacity + 255 * (1 - label2Opacity)),
                (int)(label2FinalColor.G * label2Opacity + 255 * (1 - label2Opacity)),
                (int)(label2FinalColor.B * label2Opacity + 255 * (1 - label2Opacity))
            );
            label3.ForeColor = Color.FromArgb(
                (int)(label3FinalColor.R * label3Opacity + 255 * (1 - label3Opacity)),
                (int)(label3FinalColor.G * label3Opacity + 255 * (1 - label3Opacity)),
                (int)(label3FinalColor.B * label3Opacity + 255 * (1 - label3Opacity))
            );
            if (allLabelsFullyVisible) {
                ((Timer)sender).Stop();
            }
        }

        private void PictureBox3_Paint(object sender, PaintEventArgs e) {
            using (var brush = new SolidBrush(Color.FromArgb((int)(255 * pictureBox3Opacity), Color.White))) {
                e.Graphics.FillRectangle(brush, pictureBox3.ClientRectangle);
            }
        }

        private void Form2_Load(object sender, EventArgs e) {
            label3.Text = $"v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}";
            pictureBox1StartY = pictureBox1.Top;
            pictureBox2StartY = pictureBox2.Top;
            pictureBox1.Top = this.ClientSize.Height;
            pictureBox2.Top = -pictureBox2.Height;
            pictureBox3.Paint += PictureBox3_Paint;
            outputDevice.Play();
            label1FinalColor = label1.ForeColor;
            label2FinalColor = label2.ForeColor;
            label3FinalColor = label3.ForeColor;
            label1.ForeColor = Color.FromArgb(255, 255, 255);
            label2.ForeColor = Color.FromArgb(255, 255, 255);
            label3.ForeColor = Color.FromArgb(255, 255, 255);
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Hide();
            form1.Show();
        }
    }
}