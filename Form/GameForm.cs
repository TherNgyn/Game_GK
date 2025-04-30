using GameNinjaSchool_GK.Character;
using GameNinjaSchool_GK.Controller;
using GameNinjaSchool_GK.GameNinjaSchool_GK;
using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;


namespace GameNinjaSchool_GK
{
    public partial class GameForm : Form
    {
        Ninja ninja = new Ninja();
        CanhGame canhGame = new CanhGame();
        private Image background;

        public GameForm()
        {
            try
            {
                InitializeComponent();
                try
                {
                    //Man 1 
                    canhGame.LoadLevel(1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải level: {ex.Message}");
                }

                // Load background
                try
                {
                    background = Image.FromFile("Assets/Background/BG4-01-01.png");
                }
                catch (Exception ex)
                {
                    
                    background = new Bitmap(800, 600);
                    using (Graphics g = Graphics.FromImage(background))
                    {
                        g.Clear(Color.LightBlue);
                    }
                }

                try
                {
                    ninja = new Ninja();
                  
                    if (canhGame.ChuongNgai.Count > 0)
                    {
                        GameObject ground = canhGame.ChuongNgai[0];

                        ninja.X = 50;
                        // Vi tri ninja = dinh chuongngai - chieu cao của ninja = đỉnh của ninja
                        ninja.Y = Math.Max(ninja.Y, ground.Y - ninja.height);
                        // Chọn mức ground (đất để nhân vật đứng)
                        MoveController.SetGroundLevel(ground.Y);
                    }
                    else
                    {
                        ninja.X = 50;
                        ninja.Y = 350;
                        MoveController.SetGroundLevel(600);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải nhân vật: {ex.Message}");
                }

             
                this.KeyPreview = true;
               
                if (timerGame == null)
                {
                    timerGame = new System.Windows.Forms.Timer();
                    // tốc độ 
                    timerGame.Interval = 16;
                    timerGame.Tick += timerGame_Tick;
                }
                timerGame.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi động Game: {ex.Message}");
            }
        }
     /*   private void InitializeTimer()
        {
            timerGame = new System.Windows.Forms.Timer();
            timerGame.Interval = 16; // Approx 60 FPS
            timerGame.Tick += new EventHandler(timerGame_Tick);
            timerGame.Enabled = true;
        }*/

        // Vẽ hình nhân vật, bg, canhGame
        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // background
            g.DrawImage(background, 0, 0, this.Width, this.Height);

            //  game objects
            for (int i = 0; i < canhGame.Obj.Count; i++)
            {
                
                g.DrawImage(canhGame.Obj[i].Image, canhGame.Obj[i].X, canhGame.Obj[i].Y, canhGame.Obj[i].Width, canhGame.Obj[i].Height);
            }
                // ninja
                g.DrawImage(ninja.Image, new Rectangle(ninja.X, ninja.Y, ninja.width, ninja.height));

            // Draw HUD (Health, MP, EXP)
            DrawHUD(g);
        }

        private void DrawHUD(Graphics g)
        {
            // Draw HP bar
            g.FillRectangle(Brushes.Red, 20, 20, ninja.HP * 2, 20);
            g.DrawRectangle(Pens.Black, 20, 20, 200, 20);
            g.DrawString($"HP: {ninja.HP}/100", new Font("Arial", 12), Brushes.White, 25, 20);

            // Draw MP bar
            g.FillRectangle(Brushes.Blue, 20, 50, ninja.MP * 2, 20);
            g.DrawRectangle(Pens.Black, 20, 50, 200, 20);
            g.DrawString($"MP: {ninja.MP}/100", new Font("Arial", 12), Brushes.White, 25, 50);

            // Draw EXP bar
            int expNeeded = ninja.Level * 100; // Example formula
            float expPercentage = (float)ninja.EXP / expNeeded;
            g.FillRectangle(Brushes.Green, 20, 80, (int)(expPercentage * 200), 20);
            g.DrawRectangle(Pens.Black, 20, 80, 200, 20);
            g.DrawString($"EXP: {ninja.EXP}/{expNeeded} (Lv.{ninja.Level})", new Font("Arial", 12), Brushes.White, 25, 80);
        }

        private void timerGame_Tick(object sender, EventArgs e)
        {
            MoveController.Update(ninja);
            CheckVaCham();

            // Vẽ lại
            Invalidate();
        }

        private void CheckVaCham()
        {
            Rectangle ninjaRect = new Rectangle(ninja.X, ninja.Y, ninja.width, ninja.height);
            bool oTrenChuongNgai = false;

            for (int i = 0; i < canhGame.ChuongNgai.Count; i++)
            {
                var chuongngai = canhGame.ChuongNgai[i];  
                Rectangle chuongngaiRect = chuongngai.GetBounds();
                bool laOTren = false;
                // nhảy 
                if (ninja.SpeedY >= 0) 
                {
                    // Kiểm tra xem bottom của ninja có ở trên chuongngai 
                   laOTren = ninja.Y + ninja.height >= chuongngai.Y - 5 && ninja.Y + ninja.height <= chuongngai.Y + 15;

                    bool laDungVaoChuongNgai = ninja.X + ninja.width > chuongngai.X && ninja.X < chuongngai.X + chuongngai.Width;

                    if (laOTren && laDungVaoChuongNgai)
                    {
                        ninja.Y = chuongngai.Y - ninja.height;
                        ninja.Jump = false;
                        ninja.Falling = false;
                        ninja.SpeedY = 0;
                        oTrenChuongNgai = true;
                        break;  
                    }
                }
            }

            if (!oTrenChuongNgai && !ninja.Jump)
            {
                ninja.Falling = true;
            }

            //  Nếu ninja rớt xuống => die
            // Chiều cao của form + 50 => Y die
            int deathY = this.Height + 50;
            if (ninja.Y > deathY)
            {
                ninja.HP = 0;
                timerGame.Enabled = false;
                MessageBox.Show("Thua, nhân vật đã bị rơi");
                ResetGame();
            }
            // Tăng exp
            foreach (var collectible in canhGame.VatPhamThuThap.ToList())
            {
                Rectangle collectibleRect = collectible.GetBounds();

                if (ninjaRect.IntersectsWith(collectibleRect))
                {
                    if (collectible.Type == "exp")
                    {
                        ninja.EXP += 10;
                        CheckLevelUp();
                    }
                    canhGame.VatPhamThuThap.Remove(collectible);
                    canhGame.Obj.Remove(collectible);
                }
            }
        }

        private void CheckLevelUp()
        {
            int expNeeded = ninja.Level*50;
            if (ninja.EXP >= expNeeded)
            {
                ninja.Level++;
                ninja.EXP -= expNeeded;
                ninja.HP = 100; 
                ninja.MP = 100;

                MessageBox.Show($"Thăng cấp! Bạn đã qua cấp {ninja.Level}!");
            }
        }
        private void ResetGame()
        {
            ninja = new Ninja();
            ninja.X = 50;
            ninja.Y = 500;
            canhGame.LoadLevel(1);
            timerGame.Enabled = true;
        }

        private void GameForm_Load(object sender, EventArgs e)
        {

        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            MoveController.KeyDown(ninja, e);
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            MoveController.KeyUp(ninja, e);
        }
        private void GameForm_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
