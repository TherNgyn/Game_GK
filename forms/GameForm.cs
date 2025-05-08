using GameNinjaSchool_GK.Character;
using GameNinjaSchool_GK.Controller;

using System;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Reflection.Emit;
using System.Windows.Forms;
using GameNinjaSchool_GK.forms;


using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using System.Drawing.Printing;
using System.IO;


namespace GameNinjaSchool_GK
{
    public partial class GameForm : Form
    {
        public bool IsConfirmedExit = false;
        public bool IsReturningToMenu = false;
        public Ninja ninja = new Ninja();
        public List<GameObject> ChuongNgai = new List<GameObject>();
        public List<GameObject> VatPhamThuThap = new List<GameObject>();
        public List<GameObject> GayChet = new List<GameObject>();
        public List<GameObject> Obj = new List<GameObject>();
        public LevelLoader levelLoader;
        private Image background;
        private PictureBox btnMenu;
        GameObject chuongngai;
        int nextLevel;
        int expPerElement = 0;
       
        bool isAttacking = false; // Cờ trạng thái tấn công của Player
        int attackFrameIndex = 0; // Chỉ số frame animation tấn công Player
        int attackFrameDelay = 0; // Bộ đếm thời gian chuyển frame tấn công Player
        List<Image> playerAttackFrames; // Danh sách frame animation tấn công Player

        // Đạn của Player (Phi tiêu)
        Bitmap shurikenImage; // Ảnh phi tiêu (dùng Bitmap để dễ thao tác)
        List<Bullet> bullets = new List<Bullet>(); // Danh sách các phi tiêu
        bool bulletCreated = false; // Cờ để chỉ tạo 1 phi tiêu mỗi lần tấn công

        // Kẻ địch và Boss
        public List<Enemy> enemies = new List<Enemy>(); // Danh sách tất cả kẻ địch (bao gồm cả Boss cuối)
        private Boss finalBoss = null; // Đối tượng Boss cuối (được lưu riêng để dễ kiểm tra trạng thái game)

        // Đạn và Skill của Boss
        List<BossBullet> bossBullets = new List<BossBullet>(); // Danh sách đạn của Boss
        List<EnergyColumn> energyColumns = new List<EnergyColumn>(); // Danh sách cột năng lượng (Skill 1 Boss)

        // Vật phẩm
        List<MoneyItem> moneyItems = new List<MoneyItem>(); // Danh sách vật phẩm tiền
        Image coinImage; // Ảnh vật phẩm tiền

        // Ảnh đơn khác
        Image minionGoblinImage; // Ảnh Minion (nếu dùng ảnh đơn hoặc làm fallback)
        Image bossBulletImage;      // Ảnh đạn Boss
        Image columnIndicatorImage; // Ảnh vệt sáng cột năng lượng Skill 1
        Image columnActiveImage;    // Ảnh cột năng lượng đầy đủ Skill 1

        // --- Các Dictionary chứa frame animation theo hướng và trạng thái ---
        // Key ngoài: AnimationState (Enum)
        // Key trong: bool (True = FacingRight, False = FacingLeft)
        // Value: List<Image> (Danh sách các frame ảnh)
        internal Dictionary<AnimationState, Dictionary<bool, List<Image>>> goblinAnimationFrames = new Dictionary<AnimationState, Dictionary<bool, List<Image>>>(); // Frame animation cho kẻ địch loại Goblin
        internal Dictionary<AnimationState, Dictionary<bool, List<Image>>> bossAnimationFrames; // Frame animation cho Boss
        // Có thể thêm các Dictionary cho các loại kẻ địch khác nếu có

        // --- Giới hạn số lượng đối tượng hoạt động (Tùy chọn) ---
        private const int MAX_PLAYER_BULLETS = 50;  // Giới hạn số lượng phi tiêu của Player
        private const int MAX_BOSS_BULLETS = 100; // Giới hạn số lượng đạn của Boss
        private const int MAX_ENEMIES = 30;     // Giới hạn số lượng kẻ địch (bao gồm Minions)
        private const int MAX_MONEY_ITEMS = 40;   // Giới hạn số lượng vật phẩm tiền
        private bool useObjectLimits = false; // Đặt true để bật giới hạn số lượng đối tượng
        // -------------------------------------------

        // --- Timer Game chính ---

        // Định nghĩa kích thước vẽ các đối tượng (Nếu muốn kích thước vẽ khác kích thước ảnh gốc)
        private const int SHURIKEN_DRAW_WIDTH = 30;
        private const int SHURIKEN_DRAW_HEIGHT = 30;
        //private const int PLAYER_DRAW_WIDTH = 120; // Kích thước vẽ Player
        //private const int PLAYER_DRAW_HEIGHT = 180;
        private const int MONEY_ITEM_DRAW_WIDTH = 40; // Kích thước vẽ vật phẩm tiền (Điều chỉnh nếu cần)
        private const int MONEY_ITEM_DRAW_HEIGHT = 40; // Điều chỉnh nếu cần
        // Kích thước vẽ Enemy/Boss/BossBullet/EnergyColumn sẽ lấy từ thuộc tính Width/Height của object

        // Random object (cho các phép tính ngẫu nhiên)
        private Random random = new Random(); 


        public GameForm()
        {
            this.Load += GameForm_Load;

            this.FormClosing += GameForm_FormClosing;


            try
            {
                InitializeComponent();
               
                SoundManager.StopMusic();
                SoundManager.PlayMusic("Resources/Sound/game_bgm.wav");

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
                levelLoader = new LevelLoader(ChuongNgai, VatPhamThuThap, GayChet, Obj, enemies);
                try
                {
                    levelLoader.LoadLevel(ninja.Level);
                    background = levelLoader.GetBackgroundForLevel(ninja.Level);
                    
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
                InitializeExpValues();

                this.KeyPreview = true;

                if (timerGame == null)
                {
                    timerGame = new System.Windows.Forms.Timer();
                    // tốc độ 
                    timerGame.Interval = 16;
                    timerGame.Tick += timerGame_Tick;
                }
                //MessageBox.Show(ninja.X.ToString());
                //MessageBox.Show(ninja.Y.ToString());

                //button Menu - pause game

                btnMenu = new PictureBox
                {

                    Size = new Size(64, 64),
                    Location = new Point(this.ClientSize.Width - 74, 10),
                    Image = Image.FromFile("Resources/menu_button.png"),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand
                };

               

                btnMenu.Click += BtnMenu_Click;
                this.Controls.Add(btnMenu);





            timerGame.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi động Game: {ex.Message}");
            }
        }
        private void InitializeExpValues()
        {
            int expNeeded = ninja.Level * 50;
            int chuongNgaiCount = ChuongNgai.Count(obj => obj.Type == "chuongngai" || obj.Type == "chuongngaiMove");
            expPerElement =((expNeeded - enemies.Count*10)/chuongNgaiCount)+1;
        }
        // --- Các phương thức Add đối tượng (được Boss gọi) ---
        public void AddEnergyColumn(EnergyColumn column)
        {
            // Kiểm tra giới hạn nếu dùng
            if (useObjectLimits && energyColumns.Count >= MAX_ENEMIES) return; // Sử dụng giới hạn Enemy chung cho cột năng lượng nếu không có giới hạn riêng
            energyColumns.Add(column);
        }

        public void AddBossBullet(BossBullet bullet)
        {
            // Kiểm tra giới hạn nếu dùng
            if (useObjectLimits && bossBullets.Count >= MAX_BOSS_BULLETS) return;
            bossBullets.Add(bullet);
        }

        public void AddEnemy(Enemy newEnemy) // Phương thức cho Skill 3
        {
            // Kiểm tra giới hạn nếu dùng
            if (useObjectLimits && enemies.Count >= MAX_ENEMIES) return;
            enemies.Add(newEnemy);
        }
        // ----------------------------------------------------
        // Vẽ hình nhân vật, bg
        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(background, 0, 0, this.Width, this.Height);
            for (int i = 0; i < Obj.Count; i++)
            {
                g.DrawImage(Obj[i].Image, Obj[i].X, Obj[i].Y, Obj[i].Width, Obj[i].Height);
                // Vẽ nhãn "Chướng Ngại"
                if (Obj[i].ShowLabel)
                {
                    using (Font labelFont = new Font("Cambria", 12, FontStyle.Bold))
                    using (Brush textBrush = new SolidBrush(Color.LightGoldenrodYellow))
                    using (Brush shadowBrush = new SolidBrush(Color.DarkSlateGray))

                    {
                        // Tính toán vị trí nhãn
                        Point textPoint = new Point(Obj[i].X + Obj[i].Width/2 - 60, Obj[i].Y + Obj[i].Height+5);
                        g.DrawString(Obj[i].Label, labelFont, shadowBrush, textPoint.X + 1, textPoint.Y + 1);
                        g.DrawString(Obj[i].Label, labelFont, textBrush, textPoint);
                    }
                }
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
            VeEnemies(g);
            VeMoneyItem(g);
            VeBulletPlayer(g);
            VeBossBullet(g);
            VeEnergyColumn(g);
        }
        public void VeEnemies(Graphics g)
        {
            Rectangle clientRect = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = enemies[i];
                if (enemy.IsAlive) //Chỉ vẽ nếu còn sống
                {
                    // Sử dụng CurrentAnimationFrame để vẽ ảnh động theo trạng thái và hướng
                    Image currentFrame = enemy.CurrentAnimationFrame;
                    if (currentFrame != null)
                    {
                        Rectangle enemyRect = new Rectangle((int)enemy.X, (int)enemy.Y, enemy.Width, enemy.Height); // Sử dụng kích thước từ Enemy object
                                                                                                                    // Kiểm tra va chạm với clientRect nếu bạn đang áp dụng tối ưu hóa "chỉ vẽ trong màn hình"
                        if (clientRect.IntersectsWith(enemyRect))
                        {
                            bool flipHorizontal = false;
                            g.DrawImage(currentFrame, enemy.X, enemy.Y, enemy.Width, enemy.Height);
                            
                            { 
                              // Vẽ thanh máu kẻ địch/Boss (vẽ cùng vị trí với kẻ địch)
                                float hpBarHeight = 5;
                                float hpBarY = enemy.Y - hpBarHeight - 2;
                                g.DrawRectangle(Pens.Black, enemy.X, hpBarY, enemy.Width, hpBarHeight);
                                float hpBarWidth = (float)enemy.HP / enemy.MaxHP * enemy.Width;
                                g.FillRectangle(Brushes.Red, enemy.X, hpBarY, hpBarWidth, hpBarHeight);
                            }
                        }
                    }
                }
            }
        }

        public void VeMoneyItem(Graphics g)
        {
            Rectangle clientRect = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            // --- Vẽ các vật phẩm tiền ---
            for (int i = moneyItems.Count - 1; i >= 0; i--) // Vòng lặp ngược
            {
                MoneyItem item = moneyItems[i];
                if (!item.Collected) // Chỉ vẽ nếu chưa thu thập
                {
                    Rectangle itemRect = new Rectangle((int)item.X, (int)item.Y, item.Width, item.Height); // Sử dụng kích thước từ MoneyItem object
                    if (clientRect.IntersectsWith(itemRect)) // Chỉ vẽ nếu vật phẩm trong màn hình
                    {
                        if (item.Image != null)
                        {
                            g.DrawImage(item.Image, item.X, item.Y, item.Width/2, item.Height/2);
                        }
                    }
                }
            }
            // ---------------------------
        }

        private void CollectRemainingMoney()
        {
            // Kiểm tra để đảm bảo danh sách tiền và đối tượng player đã tồn tại
            if (moneyItems == null || ninja == null)
            {
                System.Diagnostics.Debug.WriteLine("CollectRemainingMoney called but moneyItems list or player object is null.");
                return; // Không thể thu thập nếu danh sách hoặc player là null
            }

            int totalCollectedValue = 0;

            // Duyệt qua tất cả vật phẩm tiền hiện có trong danh sách
            // Việc duyệt và tính tổng là an toàn, không cần copy danh sách nếu không xóa trong vòng lặp này.
            foreach (var item in moneyItems)
            {
                // Giả sử MoneyItem có thuộc tính 'Value' chứa giá trị tiền
                totalCollectedValue += item.Value;
            }

            // Cộng tổng giá trị tiền đã thu thập vào tiền của người chơi
            ninja.Money += totalCollectedValue;

            System.Diagnostics.Debug.WriteLine($"Collected {totalCollectedValue} remaining money items. Player total money: {ninja.Money}"); // Ghi log để kiểm tra

            // Xóa tất cả các vật phẩm tiền khỏi danh sách
            moneyItems.Clear(); // <-- Xóa hết các vật phẩm tiền
        }
        public void VeBulletPlayer(Graphics g)
        {
            Rectangle clientRect = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);

            // --- Vẽ các phi tiêu (Player) ---
            for (int i = bullets.Count - 1; i >= 0; i--) // Vòng lặp ngược
            {
                Bullet bullet = bullets[i];
                // Sử dụng kích thước vẽ (SHURIKEN_DRAW_WIDTH, SHURIKEN_DRAW_HEIGHT)
                Rectangle bulletRect = new Rectangle((int)bullet.X, (int)bullet.Y, SHURIKEN_DRAW_WIDTH, SHURIKEN_DRAW_HEIGHT);
                if (clientRect.IntersectsWith(bulletRect)) // Chỉ vẽ nếu phi tiêu trong màn hình
                {
                    if (bullet.Img != null)
                    {
                        // Sử dụng hàm vẽ xoay cho phi tiêu
                        DrawRotatedImage(g, bullet.Img, bullet.X, bullet.Y, bullet.Angle, SHURIKEN_DRAW_WIDTH, SHURIKEN_DRAW_HEIGHT);
                    }
                }
            }
            // ---------------------------
        }

        public void VeBossBullet(Graphics g)
        {
            Rectangle clientRect = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);

            // --- Vẽ đạn của Boss ---
            for (int i = bossBullets.Count - 1; i >= 0; i--) // Vòng lặp ngược
            {
                BossBullet bossBullet = bossBullets[i];
                if (bossBullet.IsActive) // Chỉ vẽ nếu còn hoạt động
                {
                    Rectangle bossBulletRect = new Rectangle((int)bossBullet.X, (int)bossBullet.Y, bossBullet.Width, bossBullet.Height); // Sử dụng kích thước từ BossBullet object
                    if (clientRect.IntersectsWith(bossBulletRect)) // Chỉ vẽ nếu đạn Boss trong màn hình
                    {
                        if (bossBullet.Image != null)
                        {
                            // Vẽ đạn Boss
                            g.DrawImage(bossBullet.Image, bossBullet.X, bossBullet.Y, bossBullet.Width, bossBullet.Height);
                            // Nếu đạn Boss có xoay và thuộc tính Angle:
                            // DrawRotatedImage(g, bossBullet.Image, bossBullet.X, bossBullet.Y, bossBullet.Angle, bossBullet.Width, bossBullet.Height);
                        }
                    }
                }
            }
            // ---------------------------
        }

        public void VeEnergyColumn(Graphics g)
        {
            Rectangle clientRect = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            // --- Vẽ các cột năng lượng ---
            for (int i = energyColumns.Count - 1; i >= 0; i--) // Duyệt ngược
            {
                EnergyColumn column = energyColumns[i];
                // Chỉ vẽ nếu cột chưa kết thúc
                if (!column.IsFinished)
                {
                    // Sử dụng hình ảnh khác nhau tùy thuộc vào trạng thái
                    Image imageToDraw = column.IsActive ? column.ActiveImage : column.IndicatorImage;

                    if (imageToDraw != null)
                    {
                        Rectangle columnRect = new Rectangle((int)column.X, (int)column.Y, column.Width-30, column.Height); // Sử dụng kích thước từ EnergyColumn object
                        if (clientRect.IntersectsWith(columnRect)) // Chỉ vẽ nếu cột trong màn hình
                        {
                            g.DrawImage(imageToDraw, column.X, column.Y, column.Width, column.Height);
                        }
                    }
                }
            }
        }

        private void DrawRotatedImage(Graphics g, Image image, float x, float y, float angle, int width, int height)
        {
            // Lưu trạng thái Graphics hiện tại
            GraphicsState state = g.Save();

            g.TranslateTransform(x + width / 2, y + height / 2); // Di chuyển gốc tọa độ đến tâm đối tượng
            g.RotateTransform(angle); // Xoay theo góc
            g.TranslateTransform(-(x + width / 2), -(y + height / 2)); // Di chuyển gốc tọa độ về lại vị trí cũ ban đầu

            // Vẽ ảnh tại vị trí ban đầu (vì gốc tọa độ đã bị dịch chuyển và xoay)
            g.DrawImage(image, x, y, width, height);

            // Khôi phục trạng thái Graphics ban đầu
            g.Restore(state);
        }
        private HashSet<GameObject> expObj = new HashSet<GameObject>();
        
        private void CheckVaCham()
        {
            Rectangle ninjaRect = new Rectangle(ninja.X, ninja.Y, ninja.width, ninja.height);
            bool oTrenChuongNgai = false;
            int expNeeded = ninja.Level * 90;
            
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

                        laOTren = ninja.Y + ninja.height >= yVaCham - 10 && ninja.Y + ninja.height <= yVaCham + 20;
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

                            if (oTrenChuongNgai && (chuongngai.Type == "chuongngaiMove" || chuongngai.Type == "chuongngai") && !expObj.Contains(chuongngai))
                            {
                                expObj.Add(chuongngai);
                                ninja.GainExp(expPerElement* ninja.Level);
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
            if (ninja.Y > deathY && !oTrenChuongNgai)
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

            // --- Thêm phần vẽ Tiền ---
            int moneyDisplayY = 110;
            int moneyDisplayX = 5; 
            g.DrawString($"Money: ", new Font("Cambria", 12), Brushes.Gold, moneyDisplayX, moneyDisplayY);

            // Vẽ giá trị số tiền hiện có
            // Căn chỉnh vị trí X sang phải nhãn "Money:"
            int moneyValueX = moneyDisplayX + 67;
            g.DrawString($"{ninja.Money}", new Font("Cambria", 12), Brushes.Gold, moneyValueX, moneyDisplayY); // <-- Lấy giá trị từ player.Money
                                                                                                                 // -------------------------
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
            UpdateEnemies();
            UpdateEnergyColumn();
            UpdateBossBullet();
            XLVaChamPlayer();
            XLVaChamEnemies();
            XLTanCongPlayer();
            // Xóa đạn Boss không hoạt động (đã ra màn hình hoặc va chạm)
            bossBullets.RemoveAll(b => !b.IsActive);

            // Xóa các vật phẩm tiền đã thu thập khỏi danh sách
            moneyItems.RemoveAll(m => m.Collected);

            // Xóa các cột năng lượng đã kết thúc thời gian tồn tại
            energyColumns.RemoveAll(c => c.IsFinished);

            // --- Kiểm tra trạng thái của Boss để kết thúc game ---
            // Cần kiểm tra finalBoss có khác null trước khi truy cập IsAlive
            /*if (finalBoss != null && !finalBoss.IsAlive)
            {
                CollectRemainingMoney();
                timerGame.Stop(); // Dừng Timer Game
                // Boss đã chết!
               
                // Thêm logic kết thúc game khác (ví dụ: đóng Form, chuyển màn)
                finalBoss = null; // Đảm bảo không kiểm tra lại sau khi Boss đã bị xóa khỏi danh sách enemies
                timerGame.Stop(); // Dừng Timer Game

                //this.Close(); // Ví dụ: đóng Form
            }*/
            //this.Close(); // Ví dụ: đóng Form
            
            // Vẽ lại
            Invalidate();
        }

        public void UpdateEnemies()
        {
            // --- Cập nhật vị trí kẻ địch (bao gồm Boss) ---
            // Duyệt qua danh sách kẻ địch để cập nhật vị trí và animation
            foreach (var enemy in enemies)
            {
                if (enemy.IsAlive) // Chỉ cập nhật nếu còn sống
                {
                    enemy.Move(); // Kẻ địch di chuyển tuần tra hoặc theo logic riêng
                    enemy.UpdateAnimation(); // Cập nhật frame animation
                                             // Hướng nhìn (IsFacingRight) nên được cập nhật trong enemy.Move() hoặc sau khi Move() dựa vào sự thay đổi vị trí

                    // Có thể thêm logic xóa kẻ địch nếu chúng di chuyển ra quá xa màn hình trái
                    // if (enemy.X + enemy.Width < 0) { enemy.IsAlive = false; } // Đánh dấu để xóa sau
                }
                // Nếu là Boss, có thể cần logic chuyển trạng thái animation cụ thể cho Boss ở đây nếu nó không được xử lý trong Move() hoặc TakeDamage()
                if (enemy is Boss boss && boss.IsAlive)
                {
                    // Ví dụ: Nếu Boss dừng di chuyển (Speed = 0) và không ở trạng thái Hit hoặc Attack, chuyển sang Idle
                    // if (boss.Speed == 0 && boss.currentState != AnimationState.Idle && boss.currentState != AnimationState.Hit && boss.currentState != AnimationState.Attack)
                    // {
                    //     boss.SetAnimationState(AnimationState.Idle);
                    // }
                    // // Nếu Boss đang di chuyển và không ở trạng thái Hit hoặc Attack, chuyển sang Run
                    // else if (boss.Speed > 0 && boss.currentState != AnimationState.Run && boss.currentState != AnimationState.Hit && boss.currentState != AnimationState.Attack)
                    // {
                    //     boss.SetAnimationState(AnimationState.Run);
                    // }
                    // Cần đảm bảo trạng thái Attack được set đúng khi Boss dùng skill
                }
            }
            // ---------------------------------------------------------
        }

        public void UpdateEnergyColumn()
        {
            float deltaTimeSeconds = timerGame.Interval / 1000.0f;

            // --- Cập nhật trạng thái các cột năng lượng (EnergyColumn) ---
            // Duyệt qua danh sách cột năng lượng để cập nhật trạng thái
            for (int i = energyColumns.Count - 1; i >= 0; i--) // Duyệt ngược
            {
                EnergyColumn column = energyColumns[i];
                if (!column.IsFinished) // Chỉ cập nhật nếu cột chưa kết thúc
                {
                    column.Update(deltaTimeSeconds);
                }
            }
            // ----------------------------------------------------------
        }

        public void UpdateBossBullet()
        {
            // --- Cập nhật vị trí và xử lý Đạn Boss ---
            // Duyệt qua danh sách đạn Boss để cập nhật vị trí
            for (int i = bossBullets.Count - 1; i >= 0; i--) // Duyệt ngược
            {
                BossBullet bossBullet = bossBullets[i];
                if (bossBullet.IsActive) bossBullet.Move();
            }
        }

        public void XLVaChamPlayer()
        {
            // --- Xử lý Va chạm (Không dùng lưới, duyệt trực tiếp) ---
            Rectangle playerRect = ninja.GetBounds(); // Lấy PlayerRect MỘT LẦN
            playerRect.Width = playerRect.Width - 30;
            // Va chạm Player với Kẻ địch
            for (int i = enemies.Count - 1; i >= 0; i--) // Duyệt ngược (an toàn khi xóa)
            {
                Enemy enemy = enemies[i];
                if (enemy.IsAlive) // Chỉ kiểm tra nếu kẻ địch còn sống
                {
                    Rectangle enemyRect = new Rectangle((int)enemy.X, (int)enemy.Y, enemy.Width, enemy.Height);
                    if (playerRect.IntersectsWith(enemyRect)&& ninja.HP !=0)
                    {
                        ninja.TakeDamage(enemy is Boss ? 20 : 10); // Player nhận sát thương
                        XLPlayerHP();
                        // Logic bất tử tạm thời của Player được xử lý trong class Player
                    }
                }
            }

            if (bossBullets != null && bossBullets.Count > 0) 
            {
                for (int i = bossBullets.Count - 1; i >= 0; i--) 
                {
                    if (i < bossBullets.Count) 
                    {
                        BossBullet bossBullet = bossBullets[i];
                        if (bossBullet != null && bossBullet.IsActive) 
                        {
                            Rectangle bossBulletRect = new Rectangle((int)bossBullet.X, (int)bossBullet.Y, bossBullet.Width, bossBullet.Height);
                            if (playerRect.IntersectsWith(bossBulletRect) && ninja.HP != 0)
                            {
                                ninja.TakeDamage(bossBullet.Damage); 
                                XLPlayerHP();
                                bossBullet.IsActive = false; // Đánh dấu đạn Boss không hoạt động sau va chạm
                                System.Diagnostics.Debug.WriteLine($"Player hit by Boss bullet. HP: {ninja.HP}");
                            }
                        }
                    }
                }
            }

            // Va chạm Player với Vật phẩm tiền
            for (int i = moneyItems.Count - 1; i >= 0; i--) // Duyệt ngược
            {
                MoneyItem item = moneyItems[i];
                if (!item.Collected) // Chỉ kiểm tra nếu vật phẩm chưa thu thập
                {
                    Rectangle itemRect = new Rectangle((int)item.X, (int)item.Y, item.Width, item.Height);
                    if (playerRect.IntersectsWith(itemRect))
                    {
                        ninja.Money += item.Value; // Cộng tiền
                        item.Collected = true; // Đánh dấu đã thu thập
                        System.Diagnostics.Debug.WriteLine($"Collected {item.Value} money. Total money: {ninja.Money}");
                    }
                }
            }
           
            if (energyColumns.Count > 0) // Kiểm tra danh sách có phần tử không
            {
                for (int i = energyColumns.Count - 1; i >= 0; i--) // Duyệt ngược
                {
                    if (i < energyColumns.Count)
                    {
                        EnergyColumn column = energyColumns[i];
                        if (column.IsActive) // Chỉ kiểm tra nếu cột năng lượng đã Active
                        {
                            Rectangle columnRect = new Rectangle((int)column.X, (int)column.Y, column.Width, column.Height);
                            if (playerRect.IntersectsWith(columnRect))
                            {
                                ninja.TakeDamage(column.Damage);
                                XLPlayerHP();
                                System.Diagnostics.Debug.WriteLine($"Player inside energy column. HP: {ninja.HP}");
                            }
                        }
                    }
                }
            }
        }
        public void XLPlayerHP()
        {
            if(ninja.HP == 0)
            {
                //MessageBox.Show("Bạn đã chết");
                ResetGame();
            }
        }
        public void XLVaChamEnemies()
        {

            List<Bullet> remainingBullets = new List<Bullet>(); 
            foreach (Bullet bullet in bullets)
            {
                bullet.Move();
                bool shouldKeepBullet = true; 

                // Kiểm tra nếu phi tiêu ra ngoài màn hình
                int formWidth = this.ClientSize.Width;
                int formHeight = this.ClientSize.Height;
                if (bullet.X > formWidth || bullet.Y < 0 || bullet.Y > formHeight || bullet.X + SHURIKEN_DRAW_WIDTH < 0)
                {
                    shouldKeepBullet = false;
                }
                else // Nếu không ra ngoài màn hình, kiểm tra va chạm
                {
                    Rectangle bulletRect = new Rectangle((int)bullet.X, (int)bullet.Y, SHURIKEN_DRAW_WIDTH, SHURIKEN_DRAW_HEIGHT);
                    foreach (Enemy enemy in enemies) // Duyệt qua danh sách kẻ địch
                    {
                        if (enemy.IsAlive) // Chỉ kiểm tra va chạm với kẻ địch còn sống
                        {
                            Rectangle enemyRect = new Rectangle((int)enemy.X, (int)enemy.Y, enemy.Width, enemy.Height);

                            if (bulletRect.IntersectsWith(enemyRect))
                            {
                                // Va chạm 
                                enemy.TakeDamage(ninja.Level*70); // Kẻ địch nhận sát thương
                                // Nếu kẻ địch chết sau đòn đánh này
                                if (!enemy.IsAlive)
                                {
                                    ninja.GainExp(enemy.ExpValue* ninja.Level);
                                    CheckLevelUp();
                                    // --- Tạo vật phẩm tiền rơi ra ---
                                    if (coinImage != null) // Đảm bảo ảnh tiền không null
                                    {
                                        if (!(enemy is Boss)) // Nếu không phải Boss thì rơi tiền thường
                                        {
                                            if (!useObjectLimits || moneyItems.Count < MAX_MONEY_ITEMS) moneyItems.Add(new MoneyItem(enemy.X, enemy.Y + (enemy.Height / 2), MONEY_ITEM_DRAW_WIDTH, MONEY_ITEM_DRAW_HEIGHT, coinImage, enemy.MoneyValue));
                                            CollectRemainingMoney();
                                        }
                                        else // Nếu là Boss chết
                                        {
                                            ninja.GainExp(enemy.ExpValue*3*ninja.Level);
                                            CheckLevelUp();
                                        }
                                    }
                                }
                                shouldKeepBullet = false; // Viên đạn đã va chạm, không giữ lại
                                break;
                            }
                        }
                    }
                }
                if (shouldKeepBullet)
                {
                    remainingBullets.Add(bullet);
                }
            }

            // --- Thay thế danh sách phi tiêu cũ bằng danh sách mới chứa các viên đạn còn lại ---
            bullets = remainingBullets;
            // --- Xóa các đối tượng không còn hoạt động ---

            // Xóa các kẻ địch đã chết khỏi danh sách (xóa ngay lập tức)
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (!enemies[i].IsAlive) 
                {
                    enemies.RemoveAt(i); // Xóa ngay lập tức khỏi danh sách
                }
            }
        }

        public void XLTanCongPlayer()
        {
            // Xử lý animation tấn công của Player (Logic đã có)
            if (isAttacking)
            {
                attackFrameDelay++;
                if (attackFrameDelay >= 5)
                {
                    attackFrameDelay = 0;
                    attackFrameIndex++;

                    if (playerAttackFrames != null && attackFrameIndex < playerAttackFrames.Count)
                    {
                        if (attackFrameIndex == 3 && !bulletCreated) // Tại frame animation thứ 3, tạo phi tiêu
                        {
                            // Kiểm tra giới hạn số lượng phi tiêu Player nếu dùng
                            if (!useObjectLimits || bullets.Count < MAX_PLAYER_BULLETS)
                            {
                                int bulletStartX = ninja.X + 50; // Tùy chỉnh offset
                                int bulletStartY = ninja.Y + 30; // Tùy chỉnh offset
                                bullets.Add(new Bullet(bulletStartX, bulletStartY, shurikenImage)); // Tạo phi tiêu
                                bulletCreated = true; // Đánh dấu đã tạo đạn cho lần tấn công này
                            }
                        }
                    }

                    if (attackFrameIndex >= playerAttackFrames.Count) // Kết thúc animation tấn công
                    {
                        attackFrameIndex = 0;
                        isAttacking = false; // Đặt lại cờ trạng thái tấn công
                        bulletCreated = false; // Đặt lại cờ tạo đạn
                                               // Có thể thêm logic chuyển về animation Idle cho Player ở đây nếu có hệ thống animation tương tự Enemy
                                               // if (player has animation system) player.SetAnimationState(AnimationState.Idle);
                    }
                }
            }
            else
            {
                attackFrameIndex = 0; // Reset frame animation tấn công khi không tấn công
            }
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
                    timerGame.Stop();
                    CollectRemainingMoney();
                    this.Hide();
                    var dialogueForm = new DialogueForm(DialogueForm.DialogueState.AfterBoss);
                    dialogueForm.Show();

                }
                else
                {
                    MessageBox.Show($"Thăng cấp! Bạn đã qua cấp {ninja.Level}!");
                }

                try
                {   
                    enemies.Clear();
                    ChuongNgai.Clear();
                    VatPhamThuThap.Clear();
                    GayChet.Clear();
                    Obj.Clear();
                    nextLevel = Math.Min(ninja.Level, 3); 
                    levelLoader.LoadLevel(nextLevel);
                    background = levelLoader.GetBackgroundForLevel(nextLevel);
                    enemies = levelLoader.Enemies1;
                    //MessageBox.Show(nextLevel.ToString());

                    if(nextLevel == 3)
                    {
                       
                        LoadBoss();
                        ThemBoss();
  
                        finalBoss.StartSkillTimers();
                    }
                    
                    CollectRemainingMoney();

                    if(ChuongNgai.Count > 0)
            {
                        GameObject ground = ChuongNgai[0];
                        ninja.X = 50; 
                        ninja.Y = Math.Max(ninja.Y, ground.Y - ninja.height); 
                        MoveController.SetGroundLevel(ground.Y); 
                    }

                    // Đặt lại trạng thái ninja
                    ninja.Falling = false;
                    ninja.Jump = false;
                    ninja.SpeedY = 0;

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
            enemies.Clear();
            if(finalBoss!=null) {
                finalBoss.StopAllSkill();
                bossBullets.Clear();
                energyColumns.Clear();
                finalBoss = null;
            }
            
            bullets.Clear();
            moneyItems.Clear();
            levelLoader.LoadLevel(1);
            background = levelLoader.GetBackgroundForLevel(1);
            expObj.Clear();
            timerGame.Enabled = true;
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            playerAttackFrames = new List<Image>()
            {
                // Thay bằng đường dẫn ảnh animation tấn công của Player
                Image.FromFile("Assets/Attack/Attack0.png"),
                Image.FromFile("Assets/Attack/Attack2.png"),
                Image.FromFile("Assets/Attack/Attack3.png"),
                Image.FromFile("Assets/Attack/Attack4.png"),
                Image.FromFile("Assets/Attack/Attack9.png"),
            };
            //LoadBoss();
            //ThemBoss();
            shurikenImage = new Bitmap(Image.FromFile("Assets/VuKhi/phitieu22.png")); // Thay đường dẫn
            coinImage = Image.FromFile("Assets/Items/coin2.png"); // Thay đường dẫn
            enemies = levelLoader.Enemies1;
            // Bắt đầu Timer Game chính sau khi tải hết tài nguyên và khởi tạo đối tượng
            timerGame.Start();
        }

        public void LoadBoss()
        {
            columnIndicatorImage = Image.FromFile("Assets/VuKhi/CongNL.png"); // Thay đường dẫn
            columnActiveImage = Image.FromFile("Assets/VuKhi/CotNL2.png"); // Thay đường dẫn
            bossBulletImage = Image.FromFile("Assets/VuKhi/phitieu22.png"); // Thay đường dẫn (ví dụ dùng chung ảnh với phi tiêu)

            Dictionary<AnimationState, Dictionary<bool, List<Image>>> goblinAnimationFrames = new Dictionary<AnimationState, Dictionary<bool, List<Image>>>();           
            List<Image> goblinRunRight = new List<Image>()
                 {
                     Image.FromFile("Assets/Enemy/Bat/0.png"), // Thay đường dẫn
                     Image.FromFile("Assets/Enemy/Bat/1.png"),
                   
                 };
            // Goblin Run Left
            List<Image> goblinRunLeft = new List<Image>()
                 {
                     Image.FromFile("Assets/Enemy/Bat/3.png"), // Thay đường dẫn
                     Image.FromFile("Assets/Enemy/Bat/4.png"),
                     // ...
                 };
            // Goblin Idle Right/Left 
            List<Image> goblinIdleRight = new List<Image>() {  };
            List<Image> goblinIdleLeft = new List<Image>() {  };
            // Goblin Hit Right/Left 
            List<Image> goblinHitRight = new List<Image>() {
                    Image.FromFile("Assets/Enemy/Bat/2.png"),
                  
                };
            List<Image> goblinHitLeft = new List<Image>() {
                    Image.FromFile("Assets/Enemy/Bat/5.png"),
                };


            // Tổ chức vào Dictionary (Chỉ thêm nếu danh sách frame không trống)
            if (goblinRunRight.Count > 0) goblinAnimationFrames[AnimationState.Run] = new Dictionary<bool, List<Image>>() { { true, goblinRunRight }, { false, goblinRunLeft } };
            if (goblinIdleRight.Count > 0) goblinAnimationFrames[AnimationState.Idle] = new Dictionary<bool, List<Image>>() { { true, goblinIdleRight }, { false, goblinIdleLeft } };
            if (goblinHitRight.Count > 0) goblinAnimationFrames[AnimationState.Hit] = new Dictionary<bool, List<Image>>() { { true, goblinHitRight }, { false, goblinHitLeft } };

            // --- Tải tất cả các frame animation cho Enemy và Boss ---
            try
            {

                // --- Tải và tổ chức frame animation cho Boss ---
                bossAnimationFrames = new Dictionary<AnimationState, Dictionary<bool, List<Image>>>();

                // Boss Idle Right/Left
                List<Image> bossIdleRight = new List<Image>()
                 {
                     Image.FromFile("Assets/Enemy/Swordsman/m901.png"),
                 };
                List<Image> bossIdleLeft = new List<Image>()
                 {
                     Image.FromFile("Assets/Enemy/Swordsman/m911.png"),
                     
                 };
                // Boss Run Right/Left (Nếu có animation chạy)
                List<Image> bossRunRight = new List<Image>() {
                     Image.FromFile("Assets/Enemy/Swordsman/m900.png"),
                    /* ... */ 
       
                };
                List<Image> bossRunLeft = new List<Image>() {
                    Image.FromFile("Assets/Enemy/Swordsman/m910.png"),
                   
                };
                // Boss Attack Right/Left (Nếu có animation tấn công)
                List<Image> bossAttackRight = new List<Image>() {
                    Image.FromFile("Assets/Enemy/Swordsman/m903.png"),
                   
                };
                List<Image> bossAttackLeft = new List<Image>() {
                    Image.FromFile("Assets/Enemy/Swordsman/m913.png"),
                    
                };
                // Boss Hit Right/Left
                List<Image> bossHitRight = new List<Image>() {
                    Image.FromFile("Assets/Enemy/Swordsman/m902.png"),
                   
                };
                List<Image> bossHitLeft = new List<Image>() {
                    Image.FromFile("Assets/Enemy/Swordsman/m912.png"),
                   
                };

                // Tổ chức vào Dictionary (Chỉ thêm nếu danh sách frame không trống)
                if (bossIdleRight.Count > 0) bossAnimationFrames[AnimationState.Idle] = new Dictionary<bool, List<Image>>() { { true, bossIdleRight }, { false, bossIdleLeft } };
                if (bossRunRight.Count > 0) bossAnimationFrames[AnimationState.Run] = new Dictionary<bool, List<Image>>() { { true, bossRunRight }, { false, bossRunLeft } };
                if (bossAttackRight.Count > 0) bossAnimationFrames[AnimationState.Attack] = new Dictionary<bool, List<Image>>() { { true, bossAttackRight }, { false, bossAttackLeft } };
                if (bossHitRight.Count > 0) bossAnimationFrames[AnimationState.Hit] = new Dictionary<bool, List<Image>>() { { true, bossHitRight }, { false, bossHitLeft } };


            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải ảnh animation hoặc đối tượng: " + ex.Message);
            }


            this.DoubleBuffered = true;
        }

        public void ThemBoss()
        {

            try
            {
               
                // -----------------------------------------

                // --- Tạo Boss cuối ---
                // Chỉ tạo Boss nếu có animation Idle được tải (thường Boss bắt đầu ở Idle)
               if (bossAnimationFrames != null && bossAnimationFrames.ContainsKey(AnimationState.Idle))
                {
                    // Lấy kích thước từ frame Idle Right đầu tiên của Boss
                    List<Image> idleRightFrames = bossAnimationFrames[AnimationState.Idle][true];
                    int bossWidth = idleRightFrames[0].Width;
                    int bossHeight = idleRightFrames[0].Height;

                    // Các thông số cho Boss
                    float bossStartX = this.Width * 0.7f; // Bắt đầu ở 70% chiều rộng màn hình
                    float bossStartY = ChuongNgai[1].Y - bossHeight; // Vị trí Y cố định
                    int bossHP = 1000; // Máu Boss
                    int bossMoney = 500; // Tiền rơi khi chết
                    float bossSpeed = 1.0f; // Tốc độ di chuyển
                    float bossMinX = this.Width * 0.7f+100; // Giới hạn di chuyển
                    float bossMaxX = this.Width * 0.9f;
                    
                    // Constructor Boss: x, y, w, h, animationFrames, bossBulletImg, gameForm, minionImg, indicatorImg, activeImg, hp, money, speed, minX, maxX, playerRef
                    finalBoss = new Boss(bossStartX, bossStartY, bossWidth, bossHeight, bossAnimationFrames, bossBulletImage, this, minionGoblinImage, columnIndicatorImage, columnActiveImage, bossHP, bossMoney, bossSpeed, bossMinX, bossMaxX, ninja); // Đảm bảo tham số khớp
                    //MessageBox.Show(ninja.X.ToString());
                    //MessageBox.Show(ninja.Y.ToString());

                    // Thêm Boss vào danh sách enemies để nó được vẽ và cập nhật
                    enemies.Add(finalBoss);

                    // Bắt đầu các timer skill của Boss (nên gọi sau khi thêm Boss vào danh sách enemies)
                    finalBoss.StartSkillTimers();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Không tìm thấy Dictionary animation Boss hoặc animation Idle.");
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo đối tượng Enemy/Boss: " + ex.Message);
            }
        }
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            MoveController.KeyDown(ninja, e);
            // Xử lý tấn công Player
            if (e.KeyCode == Keys.Enter && !isAttacking)
            {
                isAttacking = true; 
                attackFrameIndex = 0; 
                attackFrameDelay = 0; 
                bulletCreated = false; 
            }
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            MoveController.KeyUp(ninja, e);
        }
        private void GameForm_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsConfirmedExit && !IsReturningToMenu)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn thoát game không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    IsConfirmedExit = true;
                    SoundManager.StopMusic();
                }
            }
        }



        private void BtnMenu_Click(object sender, EventArgs e)
        {
            timerGame.Stop();  
            PauseMenuForm pauseMenu = new PauseMenuForm(this); 
            pauseMenu.ShowDialog();
            timerGame.Start();
        }


    }
}
