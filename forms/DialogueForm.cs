using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameNinjaSchool_GK.forms
{
    public partial class DialogueForm : Form
    {
        public enum DialogueState { Intro, AfterBoss }

        private DialogueState state;
        private int currentDialogue = 0;
        private (string speaker, string text)[] dialogues;

        private Panel panelDialogue;
        private Label lblDialogue;
        private Button btnNext;

        private Panel panelEnd;
        private Label lblEnd;
        private Button btnMenu;

        private PictureBox btnSoundToggle;
        private PictureBox btnBack;

        private string fullText = "";
        private int textIndex = 0;
        private System.Windows.Forms.Timer typeTimer;
        private bool isConfirmedClose = false;

        public DialogueForm(DialogueState mode)
        {
            state = mode;
            this.DoubleBuffered = true;
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Dialogue";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.FormClosing += DialogueForm_FormClosing;

            this.BackgroundImage = LoadImage(state == DialogueState.Intro ? "start_dialogue_bg.png" : "end_dialogue_bg.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            InitializeComponent();
            if (!SoundManager.IsMuted)
                SoundManager.StopMusic();
                SoundManager.PlayMusic("Resources/Sound/dialogue_bgm.wav");
        }

        private void InitializeComponent()
        {
            panelDialogue = new Panel
            {
                Size = new Size(420, 50),
                Location = new Point(60, 320),
                BackColor = Color.FromArgb(220, 255, 255, 255)
            };
            lblDialogue = new Label
            {
                AutoSize = false,
                Size = new Size(360, 35),
                Location = new Point(10, 10),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft
            };
            btnNext = new Button
            {
                Text = "→",
                Location = new Point(375, 12),
                Size = new Size(40, 25)
            };
            btnNext.Click += BtnNext_Click;
            panelDialogue.Controls.Add(lblDialogue);
            panelDialogue.Controls.Add(btnNext);
            this.Controls.Add(panelDialogue);

            panelEnd = new Panel
            {
                Size = new Size(400, 200),
                Location = new Point(200, 150),
                BackColor = Color.FromArgb(220, 240, 240, 240),
                Visible = false
            };
            lblEnd = new Label
            {
                Text = "Bạn đã giải cứu công chúa!",
                AutoSize = false,
                Size = new Size(380, 60),
                Location = new Point(10, 10),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnMenu = new Button
            {
                Text = "Về Menu Chính",
                Size = new Size(150, 40),
                BackColor = Color.Transparent,
                Location = new Point(125, 80)
            };
            btnMenu.Click += BtnMenu_Click;

            panelEnd.Controls.Add(lblEnd);
            panelEnd.Controls.Add(btnMenu);
            this.Controls.Add(panelEnd);

            int margin = 10;
            int buttonSize = 50;

            btnBack = new PictureBox
            {
                Size = new Size(buttonSize, buttonSize),
                Location = new Point(margin, margin),
                Image = LoadImage("back_button.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnBack.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn có chắc muốn quay về menu?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    isConfirmedClose = true;
                    new MenuForm().Show();
                    this.Close();
                }
            };
            this.Controls.Add(btnBack);


            btnSoundToggle = new PictureBox
            {
                Size = new Size(buttonSize, buttonSize),
                Location = new Point(this.ClientSize.Width - buttonSize - margin, margin),
                Image = LoadImage(SoundManager.IsMuted ? "soundoff_button.png" : "soundon_button.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnSoundToggle.Click += (s, e) =>
            {
                SoundManager.ToggleMute();
                btnSoundToggle.Image = LoadImage(SoundManager.IsMuted ? "soundoff_button.png" : "soundon_button.png");
            };
            this.Controls.Add(btnSoundToggle);

            this.Resize += (s, e) =>
            {
                btnSoundToggle.Location = new Point(this.ClientSize.Width - buttonSize - margin, margin);
            };

            typeTimer = new System.Windows.Forms.Timer();
            typeTimer.Interval = 30;
            typeTimer.Tick += TypeTimer_Tick;

            this.Load += DialogueForm_Load;
        }

        private void DialogueForm_Load(object sender, EventArgs e)
        {
            if (state == DialogueState.Intro)
            {
                dialogues = new[]
                {
                    ("lead", "Ninja! Công chúa đã bị bắt cóc!"),
                    ("lead", "Chúng ta cần ngươi giúp đỡ!"),
                    ("lead", "Hãy lên đường ngay đi!")
                };
            }
            else
            {
                dialogues = new[]
                {
                    ("lead", "Ngươi đã đánh bại Boss!"),
                    ("lead", "Cảm ơn ngươi đã giải cứu công chúa!"),
                    ("lead", "Ngôi làng này mãi mãi biết ơn ngươi")
                };
            }
            currentDialogue = 0;
            ShowDialogue();
        }

        private void TypeTimer_Tick(object sender, EventArgs e)
        {
            if (textIndex < fullText.Length)
            {
                lblDialogue.Text += fullText[textIndex];
                textIndex++;
            }
            else
            {
                typeTimer.Stop();
            }
        }

        private void ShowDialogue()
        {
            fullText = dialogues[currentDialogue].text;
            textIndex = 0;
            lblDialogue.Text = "";
            typeTimer.Start();
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (typeTimer.Enabled)
            {
                typeTimer.Stop();
                lblDialogue.Text = fullText;
                return;
            }

            currentDialogue++;
            if (currentDialogue < dialogues.Length)
            {
                ShowDialogue();
            }
            else
            {
                panelDialogue.Visible = false;
                if (state == DialogueState.Intro)
                {
                    isConfirmedClose = true;
                    new GameForm().Show();
                    this.Close();
                }
                else
                {
                    panelEnd.Visible = true;
                }
            }
        }

        private void DialogueForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isConfirmedClose)
            {
                if (MessageBox.Show("Bạn có chắc muốn thoát game?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    isConfirmedClose = true;
                    SoundManager.StopMusic();
                    Environment.Exit(0);
                }
            }
        }

        private void BtnMenu_Click(object sender, EventArgs e)
        {
            isConfirmedClose = true;
            new MenuForm().Show();
            this.Close();
        }

        private Image LoadImage(string filename)
        {
            string path = Path.Combine("Resources", filename);
            return File.Exists(path) ? Image.FromFile(path) : null;
        }
    }
}