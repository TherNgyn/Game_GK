using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameNinjaSchool_GK.forms
{
    public partial class PauseMenuForm : Form
    {
        private GameForm parentGameForm;

        

        public PauseMenuForm(GameForm parent)
        {
            InitializeComponent();
            this.parentGameForm = parent;
            this.Load += PauseMenuForm_Load;

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackgroundImage = Image.FromFile("Resources/pausemenu_bg.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Size = new Size(300, 300);
        }

        private void PauseMenuForm_Load(object sender, EventArgs e)
        {
            int btnW = 180;
            int btnH = 50;
            int centerX = (this.Width - btnW) / 2;
            int marginTop = 30;

            // Buy
            Button btnBuy = new Button();
            btnBuy.BackgroundImage = Image.FromFile("Resources/buy_button.png");
            btnBuy.BackgroundImageLayout = ImageLayout.Stretch;
            btnBuy.Size = new Size(btnW, btnH);
            btnBuy.Location = new Point(centerX, marginTop);
            StyleButton(btnBuy);
            btnBuy.Click += BtnBuy_Click;
            this.Controls.Add(btnBuy);

            // Sound
            Button btnSound = new Button();
            btnSound.BackgroundImage = Image.FromFile(SoundManager.IsMuted ? "Resources/soundoff_button.png" : "Resources/soundon_button.png");
            btnSound.BackgroundImageLayout = ImageLayout.Stretch;
            btnSound.Size = new Size(30, 30);
            btnSound.Location = new Point(this.Width - 40, 10);
            StyleButton(btnSound);
            btnSound.Click += (s, e) =>
            {
                SoundManager.ToggleMute();
                btnSound.BackgroundImage = Image.FromFile(SoundManager.IsMuted ? "Resources/soundoff_button.png" : "Resources/soundon_button.png");
            };
            this.Controls.Add(btnSound);

            // Exit
            Button btnExit = new Button();
            btnExit.BackgroundImage = Image.FromFile("Resources/exit_button.png");
            btnExit.BackgroundImageLayout = ImageLayout.Stretch;
            btnExit.Size = new Size(btnW, btnH);
            btnExit.Location = new Point(centerX, marginTop + 2 * (btnH + 10));
            StyleButton(btnExit);
            btnExit.Click += (s, e) =>
            {
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát game không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                { 
                    parentGameForm.IsConfirmedExit = true;
                    Application.Exit();   
                }
            };
            this.Controls.Add(btnExit);
             

            //back
            Button btnBack = new Button();
            btnBack.BackgroundImage = Image.FromFile("Resources/back_button.png");
            btnBack.BackgroundImageLayout = ImageLayout.Stretch;
            btnBack.Size = new Size(30, 30);
            btnBack.Location = new Point(10, 10);
            StyleButton(btnBack);
            btnBack.Click += (s, e) => { this.Close(); };
            this.Controls.Add(btnBack);

            //Quit
            Button btnQuit = new Button();
            btnQuit.BackgroundImage = Image.FromFile("Resources/quit_button.png");
            btnQuit.BackgroundImageLayout = ImageLayout.Stretch;
            btnQuit.Size = new Size(btnW, btnH);
            btnQuit.Location = new Point(centerX, marginTop + btnH + 10);
            StyleButton(btnQuit);
            btnQuit.Click += (s, e) =>
            {
                DialogResult result = MessageBox.Show("Bạn có chắc muốn quay về menu?", "Xác nhận", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    SoundManager.StopMusic(); 
                    parentGameForm.IsReturningToMenu = true;
                    this.Close();
                    parentGameForm.Close();
                    new MenuForm().Show();
                }
            };

            this.Controls.Add(btnQuit); 
        }

        private void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.BackColor = Color.Transparent;
            btn.UseVisualStyleBackColor = false;
            btn.Text = "";

        }

       


        private void BtnBuy_Click(object sender, EventArgs e)
        {
            BuyForm buyForm = new BuyForm(parentGameForm);
            buyForm.ShowDialog();
        }
        
 
    }
}
