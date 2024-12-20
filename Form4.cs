using System;
using System.Windows.Forms;

namespace Guard {
    public partial class Form4 : Form {
        private string[] phrases = new string[] {
             "Доброго дня, Вас вітає GuardMedia!",
             "Ознайомтесь з інструкцією для подальшого користування!",
             "Бажаємо гарного настрою 😉"
        };

        private Form1 form1;
        private int currentPhraseIndex = 0;
        private int currentIndex = 0;
        private bool cursorVisible = true;
        private bool isDeleting = false;
        private Timer typingTimer;
        private Timer delayTimer;
        private Timer cursorTimer;

        public Form4(Form1 form1) {
            InitializeComponent();
            InitializeTimers();
            StartInitialDelay();
            this.form1 = form1;
        }

        private void InitializeTimers() {
            typingTimer = new Timer();
            typingTimer.Interval = 100;
            typingTimer.Tick += new EventHandler(TypingTimerTick);
            delayTimer = new Timer();
            delayTimer.Interval = 1000;
            delayTimer.Tick += new EventHandler(DelayTimerTick);
            cursorTimer = new Timer();
            cursorTimer.Interval = 500;
            cursorTimer.Tick += new EventHandler(CursorTimerTick);
            cursorTimer.Start();
        }

        private void StartInitialDelay() {
            Timer initialDelayTimer = new Timer();
            initialDelayTimer.Interval = 1000;
            initialDelayTimer.Tick += (sender, e) => {
                initialDelayTimer.Stop();
                typingTimer.Start();
            };
            initialDelayTimer.Start();
        }

        private void TypingTimerTick(object sender, EventArgs e) {
            if (!isDeleting) {
                if (currentIndex < phrases[currentPhraseIndex].Length) {
                    label1.Text = phrases[currentPhraseIndex].Substring(0, currentIndex + 1) + (cursorVisible ? "|" : " ");
                    currentIndex++;
                } else {
                    delayTimer.Start();
                    typingTimer.Stop();   
                }
            } else {
                if (currentIndex > 0) {
                    label1.Text = phrases[currentPhraseIndex].Substring(0, currentIndex - 1) + (cursorVisible ? "|" : " ");
                    currentIndex--;
                } else {
                    delayTimer.Start();
                    typingTimer.Stop();
                }
            }
        }

        private void DelayTimerTick(object sender, EventArgs e) {
            delayTimer.Stop();
            if (!isDeleting) {
                isDeleting = true;
            } else {
                isDeleting = false;
                currentPhraseIndex = (currentPhraseIndex + 1) % phrases.Length;
            }
            typingTimer.Start();
        }

        private void CursorTimerTick(object sender, EventArgs e) {
            cursorVisible = !cursorVisible;
            if (isDeleting || currentIndex < phrases[currentPhraseIndex].Length) {
                label1.Text = phrases[currentPhraseIndex].Substring(0, currentIndex) + (cursorVisible ? "|" : " ");
            } else {
                label1.Text = phrases[currentPhraseIndex] + (cursorVisible ? "|" : " ");
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            this.Hide();
            form1.Show();
        }
    }
}