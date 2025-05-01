using GameNinjaSchool_GK.Character;
using GameNinjaSchool_GK.Controller;

using System;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Reflection.Emit;
using System.Windows.Forms;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;


namespace GameNinjaSchool_GK
{
    public partial class GameForm : Form
    {
        Ninja ninja = new Ninja();
        private List<GameObject> ChuongNgai = new List<GameObject>();
        private List<GameObject> VatPhamThuThap = new List<GameObject>();
        private List<GameObject> GayChet = new List<GameObject>();
        private List<GameObject> Obj = new List<GameObject>();
        private LevelLoader levelLoader;
        private Image background;

        public GameForm()
        {
            try
            {
                InitializeComponent();
                
                levelLoader = new LevelLoader(ChuongNgai, VatPhamThuThap, GayChet, Obj);
                try
                {
                    levelLoader.LoadLevel(1);
                    background = levelLoader.GetBackgroundForLevel(1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải level: {ex.Message}");
                    background = new Bitmap(800, 600);
                    using (Graphics g = Graphics.FromImage(background))
                    {
                        g.Clear(Color.LightBlue);
                    }
                }
                try
                {
                    ninja = new Ninja();
                    if (ChuongNgai.Count > 0)
                    {
                        GameObject ground = ChuongNgai[0];

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

        // Vẽ hình nhân vật, bg
        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(background, 0, 0, this.Width, this.Height);
            for (int i = 0; i < Obj.Count; i++)
            {
                g.DrawImage(Obj[i].Image, Obj[i].X, Obj[i].Y, Obj[i].Width, Obj[i].Height);
            }
            if (ninja.TurnAround)
            {
                GraphicsState state = g.Save();
                g.TranslateTransform(ninja.X + ninja.width/2, ninja.Y + ninja.height/2);
                g.ScaleTransform(-1, 1);
                g.TranslateTransform(-(ninja.X + ninja.width/2), -(ninja.Y + ninja.height/2));

                g.DrawImage(ninja.Image, ninja.X, ninja.Y, ninja.width, ninja.height);

                g.Restore(state);
            }
            else
            {
                g.DrawImage(ninja.Image, ninja.X, ninja.Y, ninja.width, ninja.height);
            }

            VeHME(g);
        }

        private HashSet<GameObject> expObj = new HashSet<GameObject>();
        private void CheckVaCham()
        {
            Rectangle ninjaRect = new Rectangle(ninja.X, ninja.Y, ninja.width, ninja.height);
            bool oTrenChuongNgai = false;
            int expNeeded = ninja.Level * 50;
            for (int i = 0; i < ChuongNgai.Count; i++)
            {
                var chuongngai = ChuongNgai[i];
                Rectangle chuongngaiRect = chuongngai.GetBounds();
                bool laOTren = false;
                // nhảy 
                if (ninja.SpeedY > 0)
                {
                    if (ninja.SpeedY > 0)
                    {
                        int yVaCham = chuongngai.Y;
                        if (chuongngai.Type == "water")
                        {
                            yVaCham += 50;
                        }
                        if(chuongngai.Type =="chuongngaiMoveDa")
                        {
                            yVaCham += 50;
                        }
                        
                        laOTren = ninja.Y + ninja.height >= yVaCham - 5 && ninja.Y + ninja.height <= yVaCham + 15;
                        bool laDungVaoChuongNgai = ninja.X + ninja.width > chuongngai.X && ninja.X < chuongngai.X + chuongngai.Width;

                        if (laOTren && laDungVaoChuongNgai)
                        {
                            ninja.Y = yVaCham - ninja.height;
                            ninja.Jump = false;
                            ninja.Falling = false;
                            ninja.SpeedY = 0;
                            oTrenChuongNgai = true;

                            if (chuongngai is MovingGameObject movingObj && movingObj.ngang)
                            {
                                ninja.X += movingObj.SpeedX * movingObj.DirectionX;
                            }
                            else
                                if (chuongngai is MovingGameObject movingObjDoc && movingObjDoc.doc)
                                {
                                    ninja.Y += movingObjDoc.SpeedY * movingObjDoc.DirectionY;
                                }

                            if (oTrenChuongNgai && chuongngai.Type == "chuongngai" && !expObj.Contains(chuongngai))
                            {
                                expObj.Add(chuongngai);
                                int chuongNgaiCount = ChuongNgai.Count(obj => obj.Type == "chuongngai");
                                if (chuongNgaiCount > 0)
                                    ninja.EXP += (expNeeded / chuongNgaiCount)+2;
                               /* else
                                    ninja.EXP += 5;*/
                                CheckLevelUp();
                            }
                            break;
                        }
                    }
                }
            }

            if (!oTrenChuongNgai && !ninja.Jump)
            {
                ninja.Falling = true;
            }

            // Nếu ninja rớt xuống => die
            int deathY = this.Height + 50;
            if (ninja.Y > deathY)
            {
                ninja.HP = 0;
                timerGame.Enabled = false;
                MessageBox.Show("Thua, nhân vật đã bị rơi");
                ResetGame();
            }
        }

        private void VeHME(Graphics g)
        {
            // Thanh HP 
            g.FillRectangle(Brushes.Red, 50, 20, ninja.HP * 2, 25);
            g.DrawRectangle(Pens.DarkRed, 50, 20, 200, 25);
            g.DrawString($"HP:", new Font("Cambria", 12), Brushes.DarkRed, 5, 20);
            g.DrawString($"{ninja.HP}/100", new Font("Cambria", 12), Brushes.White, 60, 20);

            // Thanh MP 
            g.FillRectangle(Brushes.Blue, 50, 50, ninja.MP * 2, 25);
            g.DrawRectangle(Pens.DarkBlue, 50, 50, 200, 25);
            g.DrawString($"MP:", new Font("Cambria", 12), Brushes.DarkSlateBlue, 5, 50);
            g.DrawString($"{ninja.MP}/100", new Font("Cambria", 12), Brushes.White, 60, 50);

            // Thanh Exp 
            int expNeeded = ninja.Level * 50;
            float expPercentage = (float)ninja.EXP / expNeeded;
            g.FillRectangle(Brushes.Green, 50, 80, (int)(expPercentage * 200), 25);
            g.DrawRectangle(Pens.DarkGreen, 50, 80, 200, 25);
            g.DrawString($"EXP:", new Font("Cambria", 12), Brushes.DarkGreen, 5, 80);
            g.DrawString($"{ninja.EXP}/{expNeeded} (Lv.{ninja.Level})", new Font("Cambria", 12), Brushes.White, 60, 80);
        }

        private void timerGame_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Obj.Count; i++)
            {
                if (Obj[i] is MovingGameObject movingObj)
                {
                    movingObj.Move();
                }
            }
            MoveController.Update(ninja);
            CheckVaCham();

            // Vẽ lại
            Invalidate();
        }

        private void CheckLevelUp()
        {
            int expNeeded = ninja.Level * 50;


            if (ninja.EXP >= expNeeded )
            {
                ninja.Level++;
                ninja.EXP -= expNeeded;
                ninja.HP = 100;
                ninja.MP = 100;

                if (ninja.Level == 4)
                {
                    MessageBox.Show($"Chúc mừng! Bạn đã đạt cấp cao nhất (Cấp {ninja.Level}) và hoàn thành tất cả các màn chơi!");
                }
                else
                {
                    MessageBox.Show($"Thăng cấp! Bạn đã qua cấp {ninja.Level}!");
                }

                try
                {     
                    ChuongNgai.Clear();
                    VatPhamThuThap.Clear();
                    GayChet.Clear();
                    Obj.Clear();           
                    int nextLevel = Math.Min(ninja.Level, 3); 
                    levelLoader.LoadLevel(nextLevel);
                    background = levelLoader.GetBackgroundForLevel(nextLevel);

                    if (ChuongNgai.Count > 0)
                    {
                        GameObject ground = ChuongNgai[0];
                        ninja.X = 50;
                        ninja.Y = Math.Max(ninja.Y, ground.Y - ninja.height);
                        MoveController.SetGroundLevel(ground.Y);
                    }

                    expObj.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải level: {ex.Message}");
                }
            }
        }
        private void ResetGame()
        {
            ninja = new Ninja();
            ninja.X = 50;
            ninja.Y = 500;
            ChuongNgai.Clear();
            VatPhamThuThap.Clear();
            GayChet.Clear();
            Obj.Clear();
            levelLoader.LoadLevel(1);
            background = levelLoader.GetBackgroundForLevel(1);
            expObj.Clear();
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
