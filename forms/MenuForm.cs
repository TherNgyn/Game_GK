﻿using System;
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
    public partial class MenuForm : Form
    {
        private PictureBox btnStart;
        private PictureBox btnExit;
        private PictureBox btnSound;

        private bool isMuted = false;

        public MenuForm()
        {
            InitializeComponent();
            InitUI();
            SoundManager.PlayMusic("Resources/Sound/menu_bgm.wav");


            this.FormClosing += MenuForm_FormClosing;

        }

        private void InitUI()
        {
            this.DoubleBuffered = true;
            this.BackgroundImage = Image.FromFile("Resources/menu_bg.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.ClientSize = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Start button
            btnStart = new PictureBox
            {
                Image = Image.FromFile("Resources/start_button.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnStart.Click += BtnStart_Click;

            // Exit button
            btnExit = new PictureBox
            {
                Image = Image.FromFile("Resources/exit_button.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Location = new Point(300, 350),
                Cursor = Cursors.Hand
            };
            btnExit.Click += (s, e) =>
            {
                if (MessageBox.Show("Bạn có chắc muốn thoát game?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SoundManager.StopMusic();
                    Environment.Exit(0);
                }
            };




            int buttonWidth = 200;
            int buttonHeight = 80;
            int buttonSpacing = 20;

            int centerX = (this.ClientSize.Width - buttonWidth) / 2;
            int startY = 180;

            btnStart.Size = new Size(buttonWidth, buttonHeight);
            btnStart.Location = new Point(centerX, startY);

            btnExit.Size = new Size(buttonWidth, buttonHeight);
            btnExit.Location = new Point(centerX, startY + buttonHeight + buttonSpacing);

            // Sound toggle
            btnSound = new PictureBox
            {
                Image = Image.FromFile("Resources/soundon_button.png"),
                Size = new Size(70, 70),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Location = new Point(this.ClientSize.Width - 95, 30),
                Cursor = Cursors.Hand
            };
            btnSound.Click += BtnSound_Click;

            this.Controls.Add(btnStart);
            this.Controls.Add(btnExit);
            this.Controls.Add(btnSound);
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            var intro = new DialogueForm(DialogueForm.DialogueState.Intro);
            intro.Show();
            this.Hide();
        }

        private void UpdateSoundIcon()
        {
            btnSound.Image = Image.FromFile(SoundManager.IsMuted
                ? "Resources/soundoff_button.png"
                : "Resources/soundon_button.png");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateSoundIcon();
        }

        private void BtnSound_Click(object sender, EventArgs e)
        {
            SoundManager.ToggleMute();
            UpdateSoundIcon();
        }


        private void MenuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát game không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                e.Cancel = true;
            }
            else
            {
                SoundManager.StopMusic();
                Environment.Exit(0);
            }
        }


        private void MenuForm_Load(object sender, EventArgs e)
        {

        }
    }
}
