using System;
using System.IO;
using System.Data;
using System.Linq;
using NAudio.Wave;
using System.Media;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Guard {
    public partial class Form1 : Form {
        private List<string> selectedPhotos = new List<string>();
        private bool messagesRepeat = false;
        private ContextMenuStrip contextMenuStrip;
        private bool helpWindow = false;
        private SoundPlayer player;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private bool isMaximized = true;

        public Form1() {
            InitializeComponent();
            InitializeContextMenu();
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.SelectionMode = SelectionMode.MultiExtended;
            listBox1.DrawItem += new DrawItemEventHandler(listBox1_DrawItem);
            listBox1.MouseDown += new MouseEventHandler(listBox1_MouseDown);
            listBox1.MouseDoubleClick += new MouseEventHandler(listBox1_MouseDoubleClick);
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            outputDevice = new WaveOutEvent();
            audioFile = new AudioFileReader(@"D:\Моя папка\Робочі\c#\Guard\bin\Debug\sound.mp3");
            audioFile.Volume = 0.2f;
            outputDevice.Init(audioFile);
            this.StartPosition = FormStartPosition.CenterScreen;
            button5.Visible = false;
            button5.Click += new EventHandler(button5_Click);
            this.SizeChanged += Form1_SizeChanged;
        }

        private async void ShowHelpWindow() {
            if (!helpWindow) {
                helpWindow = true;
                Rectangle screenRectangle = Screen.PrimaryScreen.WorkingArea;
                int x = (screenRectangle.Width - 400) / 2;
                int y = (screenRectangle.Height - 200) / 2;
                Form helpForm = new Form();
                helpForm.FormBorderStyle = FormBorderStyle.None;
                helpForm.StartPosition = FormStartPosition.Manual;
                helpForm.Size = new Size(400, 200);
                helpForm.Location = new Point(x, y);
                helpForm.BackColor = Color.LightGoldenrodYellow;
                helpForm.Padding = new Padding(2);
                helpForm.Paint += (sender, e) => {
                    int borderWidth = 2;
                    using (Pen pen = new Pen(Color.Black, borderWidth)) {
                        e.Graphics.DrawRectangle(pen, new Rectangle(borderWidth / 2, borderWidth / 2, helpForm.ClientSize.Width - borderWidth, helpForm.ClientSize.Height - borderWidth));
                    }
                };
                Label label = new Label();
                label.Text = "Перш, ніж користуватися програмою, рекомендуємо Вам переглянути\nрозділ 'Довідка'";
                label.Font = new Font("Bookman Old Style", 14);
                label.AutoSize = false;
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Size = new Size(380, 100);
                label.Location = new Point((helpForm.Width - label.Width) / 2, (helpForm.Height - label.Height) / 2);
                helpForm.Controls.Add(label);
                Button logo = new Button();
                logo.Text = "ℹ️";
                logo.FlatAppearance.MouseOverBackColor = Color.Transparent;
                logo.FlatAppearance.MouseDownBackColor = Color.Transparent;
                logo.ForeColor = Color.Red;
                logo.BackColor = Color.Transparent;
                logo.Font = new Font("Bookman Old Style", 24);
                logo.Size = new Size(50, 50);
                logo.Location = new Point(3, -1);
                logo.FlatStyle = FlatStyle.Flat;
                logo.FlatAppearance.BorderSize = 0;
                helpForm.Controls.Add(logo);
                logo.BringToFront();
                Button closeButton = new Button();
                closeButton.Text = "⨉";
                closeButton.FlatAppearance.MouseOverBackColor = Color.Red;
                closeButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(192, 0, 0);
                closeButton.FlatAppearance.BorderSize = 0;
                closeButton.BackColor = Color.Transparent;
                closeButton.Font = new Font("Bookman Old Style", 16);
                closeButton.Size = new Size(35, 35);
                closeButton.Location = new Point(363, 2);
                closeButton.TextAlign = ContentAlignment.MiddleCenter;
                closeButton.FlatStyle = FlatStyle.Flat;
                closeButton.Cursor = Cursors.Hand;
                closeButton.Click += async (sender, e) => {
                    await CloseHelpWindow(helpForm);
                };
                helpForm.Controls.Add(closeButton);
                closeButton.BringToFront();
                helpForm.Opacity = 0;
                helpForm.Show();
                await FadeIn(helpForm);
                audioFile.Position = 0;
                outputDevice.Play();
            }
        }

        private async Task CloseHelpWindow(Form helpForm) {
            await FadeOut(helpForm);
            helpForm.Close();
            helpWindow = false;
        }

        private async Task FadeIn(Form form) {
            while (form.Opacity < 1.0) {
                await Task.Delay(10);
                form.Opacity += 0.05;
            }
        }

        private async Task FadeOut(Form form) {
            while (form.Opacity > 0) {
                await Task.Delay(10);
                form.Opacity -= 0.05;
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e) {
            ShowHelpWindow();
        }

        private void InitializeContextMenu() {
            contextMenuStrip = new ContextMenuStrip();
            var openWithMenuItem = new ToolStripMenuItem("Відкрити за допомогою...");
            openWithMenuItem.Click += OpenWithMenuItem_Click;
            contextMenuStrip.Items.Add(openWithMenuItem);
            listBox1.ContextMenuStrip = contextMenuStrip;
        }

        private void OpenWithMenuItem_Click(object sender, EventArgs e) {
            if (listBox1.SelectedIndices.Count == 0) {
                MessageBox.Show("Будь ласка, виберіть файл перед відкриттям!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else if (listBox1.SelectedIndices.Count > 1) {
                MessageBox.Show("Будь ласка, виберіть тільки один файл перед відкриттям!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else {
                ListBoxItem selectedItem = listBox1.SelectedItem as ListBoxItem;
                if (selectedItem != null) {
                    string selectedFile = selectedItem.FullPath;
                    OpenWith(selectedFile);
                } else {
                    MessageBox.Show("Виникла помилка при виборі файлу. Спробуйте ще раз!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OpenWith(string filePath) {
            OpenFileDialog openFileDialog = new OpenFileDialog() {
                Filter = "Виконувані файли (*.exe)|*.exe",
                Title = "Виберіть програму"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                string programPath = openFileDialog.FileName;
                try {
                    string fileUri = new Uri(filePath).AbsoluteUri;
                    System.Diagnostics.Process.Start(programPath, fileUri);
                } catch (Exception ex) {
                    MessageBox.Show($"Не вдалось відкрити файл: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                int index = listBox1.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches) {
                    listBox1.SelectedIndex = index;
                } else {
                    listBox1.ClearSelected();
                }
            } else if (e.Button == MouseButtons.Right) {
                if (listBox1.Items.Count == 0) {
                    MessageBox.Show("Список порожній. Додайте файли для перегляду!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                int index = listBox1.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches) {
                    listBox1.SelectedIndex = index;
                    contextMenuStrip.Show(Cursor.Position);
                } else {
                    listBox1.ClearSelected();
                }
            }
        }

        private bool IsImageFile(string fileName) {
            string extension = Path.GetExtension(fileName).ToLower();
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp";
        }

        private bool IsVideoFile(string fileName) {
            string extension = Path.GetExtension(fileName).ToLower();
            return extension == ".mp4" || extension == ".mkv" || extension == ".avi" || extension == ".mov" || extension == ".wmv";
        }

        private void UpdatePhotoList() {
            listBox1.BeginUpdate();
            listBox1.Items.Clear();
            foreach (var photo in selectedPhotos) {
                listBox1.Items.Add(new ListBoxItem {
                    Text = this.WindowState == FormWindowState.Maximized ? photo : shortenPath(photo),
                    FullPath = photo,
                    IsDuplicate = IsDuplicate(photo)
                });
            }
            listBox1.EndUpdate();
        }

        private string shortenPath(string path) {
            if (path.Length <= 25) return path;
            return path.Substring(0, 25) + "...";
        }

        private bool IsDuplicate(string photo) {
            return selectedPhotos.Count(p => GetFileSignature(p) == GetFileSignature(photo)) > 1;
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e) {
            if (e.Index < 0) return;
            ListBoxItem item = (ListBoxItem)listBox1.Items[e.Index];
            e.DrawBackground();
            Graphics g = e.Graphics;
            Brush textBrush;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
                textBrush = Brushes.White;
            } else {
                textBrush = item.IsDuplicate ? Brushes.Red : Brushes.Black;
            }
            string itemText = $"{e.Index + 1}. {item.Text}";
            SizeF textSize = g.MeasureString(itemText, e.Font);
            float textX = e.Bounds.Left + 10;
            float textY = e.Bounds.Top + (e.Bounds.Height - textSize.Height) / 2;
            g.DrawString(itemText, e.Font, textBrush, textX, textY);
            // e.DrawFocusRectangle();
        }

        private List<List<string>> FindDuplicateGroups(List<string> photos) {
            var duplicateGroups = new List<List<string>>();
            var uniqueHashes = new HashSet<string>();
            var duplicateHashes = new Dictionary<string, string>();
            foreach (var photo in photos) {
                string photoHash = GetFileSignature(photo);
                if (!uniqueHashes.Add(photoHash)) {
                    if (!duplicateHashes.ContainsKey(photoHash)) {
                        duplicateHashes[photoHash] = photo;
                    }
                }
            }
            foreach (var hash in duplicateHashes.Keys) {
                var group = photos.Where(photo => GetFileSignature(photo) == hash).ToList();
                if (group.Count > 1) {
                    duplicateGroups.Add(group);
                }
            }
            return duplicateGroups;
        }

        private void RemoveAllDuplicates(List<List<string>> duplicateGroups) {
            foreach (var group in duplicateGroups) {
                for (int i = 1; i < group.Count; i++) {
                    selectedPhotos.Remove(group[i]);
                    File.Delete(group[i]);
                }
            }
        }

        private string GetFileSignature(string filePath) {
            try {
                using (var stream = File.OpenRead(filePath)) {
                    long fileSize = new FileInfo(filePath).Length;
                    byte[] fileContent = new byte[Math.Min(fileSize, 16384)];
                    stream.Read(fileContent, 0, fileContent.Length);
                    using (var md5 = System.Security.Cryptography.MD5.Create()) {
                        byte[] hashBytes = md5.ComputeHash(fileContent);
                        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"Сталася помилка: {ex.Message}");
                return null;
            }
        }

        private void RenameFile(string filePath, Form form1) {
            var renameForm = new Form3(filePath, this);
            form1.Hide();
            while (true) {
                if (renameForm.ShowDialog() == DialogResult.OK) {
                    string newFileName = renameForm.NewFileName;
                    if (newFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) {
                        MessageBox.Show("Назва файлу містить невірні символи.\nБудь ласка, повторіть спробу!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                    string directory = Path.GetDirectoryName(filePath);
                    string newFilePath = Path.Combine(directory, newFileName + Path.GetExtension(filePath));
                    if (File.Exists(newFilePath)) {
                        MessageBox.Show("Файл з такою назвою вже є. Будь ласка, введіть інше ім'я!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                    try {
                        File.Move(filePath, newFilePath);
                        if (selectedPhotos.Contains(filePath)) {
                            selectedPhotos[selectedPhotos.IndexOf(filePath)] = newFilePath;
                        }
                        UpdatePhotoList();
                        break;
                    } catch (Exception ex) {
                        MessageBox.Show($"Сталася помилка під час перейменування файлу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } else {
                    break;
                }
            }
            form1.Show();
        }

        private void перевіритиToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!selectedPhotos.Any()) {
                MessageBox.Show("Спочатку додайте фото або відео до робочої області!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var duplicateGroups = FindDuplicateGroups(selectedPhotos);
            if (duplicateGroups.Any()) {
                var duplicates = duplicateGroups.SelectMany(g => g).ToList();
                button5.Visible = true;
            } else {
                MessageBox.Show("У списку не знайдено жодного дубліката!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void перейменуватиToolStripMenuItem_Click(object sender, EventArgs e) {
            if (listBox1.SelectedIndex == -1) {
                MessageBox.Show("Будь ласка, виберіть файл для перейменування!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (listBox1.SelectedIndices.Count > 1) {
                MessageBox.Show("Будь ласка, виберіть тільки один файл для\nперейменування!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string selectedFile = selectedPhotos[listBox1.SelectedIndex];
            RenameFile(selectedFile, this);
        }

        private void очиститиToolStripMenuItem_Click(object sender, EventArgs e) {
            if (listBox1.Items.Count == 0) {
                MessageBox.Show("Немає файлів для очищення поля!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            listBox1.Items.Clear();
            selectedPhotos.Clear();
            button5.Visible = false;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (selectedPhotos.Count == 0) {
                MessageBox.Show("Будь ласка, додайте файли перед їх збереженням!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (var folderDialog = new FolderBrowserDialog()) {
                folderDialog.Description = "Виберіть місце для збереження файлів!";
                if (folderDialog.ShowDialog() == DialogResult.OK) {
                    string destinationFolder = folderDialog.SelectedPath;
                    SaveFiles(destinationFolder);
                }
            }
        }

        private void SaveFiles(string destinationFolder) {
            try {
                int savedFileCount = 0;
                foreach (var file in selectedPhotos) {
                    string fileName = Path.GetFileName(file);
                    string destinationPath = Path.Combine(destinationFolder, fileName);
                    if (File.Exists(destinationPath)) {
                        var result = MessageBox.Show($"Файл з назвою: {fileName} вже існує. Хочете його замінити?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.No) {
                            continue;
                        }
                    }
                    try {
                        File.Copy(file, destinationPath, true);
                        savedFileCount++;
                    } catch (IOException ex) when ((ex.HResult & 0x0000FFFF) == 0x20) {
                        MessageBox.Show("Файли успішно збережено!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        continue;  
                    }
                }
                if (savedFileCount > 0) {
                    MessageBox.Show("Файли успішно збережено!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } catch (Exception ex) {
                MessageBox.Show($"Не вдалось зберегти файли: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            if (listBox1.Items.Count == 0) {
                MessageBox.Show("У списку немає файлів для видалення!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else if (listBox1.SelectedIndices.Count > 0) {
                var result = MessageBox.Show("Ви впевнені, що хочете видалити виділені файли? В такому разі їх буде ліквідовано назавжди!", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) {
                    foreach (int index in listBox1.SelectedIndices) {
                        string photoPath = selectedPhotos[index];
                        try {
                            File.Delete(photoPath);
                        } catch (Exception ex) {
                            MessageBox.Show($"Не вдалося видалити {photoPath}: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    selectedPhotos.RemoveAll(photo => listBox1.SelectedItems.Cast<ListBoxItem>().Select(item => item.FullPath).Contains(photo));
                    UpdatePhotoList();
                }
            } else {
                var result = MessageBox.Show("Ви впевнені, що хочете видалити всі файли? В такому разі їх буде ліквідовано назавжди!", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) {
                    foreach (var photoPath in selectedPhotos.ToList()) {
                        try {
                            File.Delete(photoPath);
                        } catch (Exception ex) {
                            MessageBox.Show($"Не вдалося видалити {photoPath}: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    listBox1.Items.Clear();
                    selectedPhotos.Clear();
                    button5.Visible = false;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                if (!messagesRepeat) {
                    var confirm = MessageBox.Show("Дійсно бажаєте вийти з програми?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm == DialogResult.Yes) {
                        Close();
                    } else if (confirm == DialogResult.No) {
                        messagesRepeat = true;
                    }
                } else {
                    messagesRepeat = false;
                }
            } else if (e.KeyCode == Keys.Delete) {
                if (!messagesRepeat) {
                    deleteToolStripMenuItem_Click(sender, e);
                    messagesRepeat = true;
                } else {
                    messagesRepeat = false;
                }
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e) {
            int index = listBox1.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches) {
                ListBoxItem item = (ListBoxItem)listBox1.Items[index];
                if (!messagesRepeat) {
                    MessageBox.Show($"Повний шлях до файлу\n{item.FullPath}", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    messagesRepeat = true;
                } else {
                    messagesRepeat = false;
                }
            }
        }

        private void інформаціяToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Hide();
            Form4 form4 = new Form4(this);
            form4.ShowDialog();
        }

        private void проРозробникаToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Hide();
            Form2 form2 = new Form2(this);
            form2.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e) {
            var confirm = MessageBox.Show("Дійсно бажаєте вийти з програми?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes) {
                Close();
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Maximized) {
                this.WindowState = FormWindowState.Normal;
                CenterForm();
            } else {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void CenterForm() {
            var screen = Screen.FromControl(this);
            var workingArea = screen.WorkingArea;
            int newX = (workingArea.Width - this.Width) / 2;
            int newY = (workingArea.Height - this.Height) / 2;
            this.Location = new Point(newX, newY);
        }

        private void button4_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button5_Click(object sender, EventArgs e) {
            var duplicateGroups = FindDuplicateGroups(selectedPhotos);
            RemoveAllDuplicates(duplicateGroups);
            UpdatePhotoList();
            button5.Visible = false;
        }

        private void Form1_SizeChanged(object sender, EventArgs e) {
            UpdatePhotoList();
        }

        private void додатиToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Усі файли|*.*|Фото (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Відео (*.mp4;*.mkv;*.avi;*.mov;*.wmv)|*.mp4;*.mkv;*.avi;*.mov;*.wmv";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                var selectedFiles = openFileDialog.FileNames;
                foreach (var file in selectedFiles) {
                    if (!IsImageFile(file) && !IsVideoFile(file)) {
                        MessageBox.Show($"Файл '{Path.GetFileName(file)}' не є фото/відео матеріалом і не може бути додано!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (selectedFiles.Length < 2) {
                    MessageBox.Show("Будь ласка, виберіть принаймні 2 фото/відео!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                int photoCount = selectedFiles.Count(file => IsImageFile(file));
                int videoCount = selectedFiles.Length - photoCount;
                if (photoCount > 0 && videoCount > 0) {
                    MessageBox.Show("Виберіть файли тільки одного типу за раз: або всі фото або всі відео!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var alreadyAddedFiles = selectedFiles.Where(file => selectedPhotos.Contains(file)).ToList();
                var newFiles = selectedFiles.Where(file => !selectedPhotos.Contains(file)).ToList();
                if (alreadyAddedFiles.Any()) {
                    var fileNamesWithNewLines = string.Join(Environment.NewLine + Environment.NewLine, alreadyAddedFiles.Select(file => Path.GetFileName(file)));
                    MessageBox.Show($"Файли:\n\n{fileNamesWithNewLines}\n\nвже наявні у цьому списку!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                selectedPhotos.AddRange(newFiles);
                UpdatePhotoList();
            }
        }

        private void вихідToolStripMenuItem_Click_1(object sender, EventArgs e) {
            var confirm = MessageBox.Show("Дійсно бажаєте вийти з програми?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes) {
                Close();
            }
        }
    }
}