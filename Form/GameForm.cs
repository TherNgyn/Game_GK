using GameNinjaSchool_GK.Character;
using GameNinjaSchool_GK.Controller;
using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace GameNinjaSchool_GK
{
    public partial class GameForm : Form
    {
        Player player = new Player();

        public GameForm()
        {
            InitializeComponent();
           
        }

        private void GameForm_Load(object sender, EventArgs e)
        {

        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
           
            Graphics g = e.Graphics;
            e.Graphics.Clear(Color.SkyBlue);
            Image bg = Image.FromFile("Assets/Background/BG4-01-01.png");
            g.DrawImage(bg, 0, 0, this.Width, this.Height);
            g.DrawImage(player.Image, player.X, player.Y);

        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            MoveController.KeyDown(player, e);
        }
        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            MoveController.KeyUp(player, e);
        }
        private void timerGame_Tick(object sender, EventArgs e)
        {
            MoveController.Update(player);
            Invalidate();
        }
    }
}
