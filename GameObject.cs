using GameNinjaSchool_GK.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace GameNinjaSchool_GK
{
    public class GameObject
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public Image Image;
        public string Type;
        // Thuộc tính hiển thị nhãn
        public bool ShowLabel => Type == "chuongngai";
        public string Label => ShowLabel ? "Chướng Ngại" : "";

        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, Width, Height);
        }
    }
    public class MovingGameObject : GameObject
    {
        public int SpeedX;
        public int SpeedY;
        public int DirectionX=1; 
        //1 phải, xuống
        public int DirectionY=1;
        public int MinX;
        public int MaxX;
        public int MinY;
        public int MaxY;
        public bool ngang=false;
        public bool doc=false;
        public void Move()
        {
            if (ngang)
            {
                X = X + SpeedX * DirectionX;
                if (X <= MinX || X + Width >= MaxX)
                {
                    DirectionX *= -1;
                }
            }
            if (doc)
            {
                Y =Y+ SpeedY*DirectionY;
                if (Y <= MinY || Y + Height >= MaxY)
                {
                    DirectionY *= -1;
                }
            }
        }
    }
    //
    public abstract class Level
    {
        public Image imgbackground;
        public List<GameObject> ChuongNgai;
        public List<GameObject> VatPhamThuThap;
        public List<GameObject> GayChet;
        public List<GameObject> Obj;
        public List<Enemy> Enemies;

        public Level(List<GameObject> chuongNgai, List<GameObject> vatPhamThuThap,
                    List<GameObject> gayChet, List<GameObject> obj, List<Enemy> enemies)
        {
            ChuongNgai = chuongNgai;
            VatPhamThuThap = vatPhamThuThap;
            GayChet = gayChet;
            Obj = obj;
            Enemies = enemies;
            imgbackground = LoadBGImg("Assets/Background/BGLevel1.png");
        }

        protected abstract void LoadChuongNgai();
        protected virtual void LoadVatPham() { }
        protected virtual void LoadKeThu() { }

        protected Image LoadBGImg(string path)
        {
            try
            {
                return Image.FromFile(path);
            }
            catch (Exception ex)
            {
                Bitmap fallbackBg = new Bitmap(800, 600);
                using (Graphics g = Graphics.FromImage(fallbackBg))
                {
                    g.Clear(Color.LightBlue);
                }
                return fallbackBg;
            }
        }

        protected Image LoadImage(string path, Color errorbackColor)
        {
            try
            {
                return Image.FromFile(path);
            }
            catch (Exception)
            {
                Bitmap errorbackImage = new Bitmap(1, 1);
                using (Graphics g = Graphics.FromImage(errorbackImage))
                {
                    g.Clear(errorbackColor);
                }
                return errorbackImage;
            }
        }

        public void Load()
        {
            try
            {
                ChuongNgai.Clear();
                VatPhamThuThap.Clear();
                GayChet.Clear();

                LoadChuongNgai();
                LoadVatPham();
                LoadKeThu();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải level: {ex.Message}");
            }
        }
    }
    public class Level1 : Level
    {
        public Level1(List<GameObject> chuongNgai, List<GameObject> vatPhamThuThap,
                     List<GameObject> gayChet, List<GameObject> obj, List<Enemy> enemies)
            : base(chuongNgai, vatPhamThuThap, gayChet, obj,enemies)
        {
            imgbackground = LoadBGImg("Assets/Background/BGLevel1.png");
        }
        protected override void LoadKeThu()
        {
            //Dictionary<AnimationState, Dictionary<bool, List<Image>>> goblinAnimationFrames; // Frame animation cho kẻ địch loại Goblin
            // --- Tải và tổ chức frame animation cho Kẻ địch thường (ví dụ: Goblin) ---
            Dictionary<AnimationState, Dictionary<bool, List<Image>>> goblinAnimationFrames = new Dictionary<AnimationState, Dictionary<bool, List<Image>>>();

            // Goblin Run Right
            List<Image> goblinRunRight = new List<Image>()
                 {
                     Image.FromFile("Assets/Enemy/Scorpion/m600.png"), // Thay đường dẫn
                     Image.FromFile("Assets/Enemy/Scorpion/m601.png"),
                     // ... thêm các frame khác ...
                 };
            // Goblin Run Left
            List<Image> goblinRunLeft = new List<Image>()
                 {
                     Image.FromFile("Assets/Enemy/Scorpion/m610.png"), // Thay đường dẫn
                     Image.FromFile("Assets/Enemy/Scorpion/m611.png"),
                     // ...
                 };
            // Goblin Idle Right/Left (Nếu có)
            List<Image> goblinIdleRight = new List<Image>() { /* ... */ };
            List<Image> goblinIdleLeft = new List<Image>() { /* ... */ };
            // Goblin Hit Right/Left (Nếu có)
            List<Image> goblinHitRight = new List<Image>() {
                    Image.FromFile("Assets/Enemy/Scorpion/m602.png"),
                    /* ... */ 
                };
            List<Image> goblinHitLeft = new List<Image>() {
                    Image.FromFile("Assets/Enemy/Scorpion/m612.png"),
                    /* ... */ 
                };


            // Tổ chức vào Dictionary (Chỉ thêm nếu danh sách frame không trống)
            if (goblinRunRight.Count > 0) goblinAnimationFrames[AnimationState.Run] = new Dictionary<bool, List<Image>>() { { true, goblinRunRight }, { false, goblinRunLeft } };
            if (goblinIdleRight.Count > 0) goblinAnimationFrames[AnimationState.Idle] = new Dictionary<bool, List<Image>>() { { true, goblinIdleRight }, { false, goblinIdleLeft } };
            if (goblinHitRight.Count > 0) goblinAnimationFrames[AnimationState.Hit] = new Dictionary<bool, List<Image>>() { { true, goblinHitRight }, { false, goblinHitLeft } };

            // --- Tạo kẻ địch thường (ví dụ: Goblin) ---
            if (goblinAnimationFrames != null && goblinAnimationFrames.Count > 0) // Chỉ tạo nếu có bất kỳ animation nào được tải
            {
                // Lấy kích thước từ một bộ frame bất kỳ (ví dụ: Run Right)
                List<Image> sampleFrames = null;
                if (goblinAnimationFrames.ContainsKey(AnimationState.Run) && goblinAnimationFrames[AnimationState.Run].ContainsKey(true)) sampleFrames = goblinAnimationFrames[AnimationState.Run][true];
                else if (goblinAnimationFrames.ContainsKey(AnimationState.Idle) && goblinAnimationFrames[AnimationState.Idle].ContainsKey(true)) sampleFrames = goblinAnimationFrames[AnimationState.Idle][true];
                // ... (kiểm tra các trạng thái khác nếu cần) ...

                if (sampleFrames != null && sampleFrames.Count > 0)
                {
                    int goblinWidth = sampleFrames[0].Width / 2;
                    int goblinHeight = sampleFrames[0].Height / 2;
                    GameObject chuongngai = ChuongNgai[5];
                    int Y = chuongngai.Y - goblinHeight;
                    int minX = chuongngai.X + 30;
                    int maxX = chuongngai.X + 150;
                    //MessageBox.Show(Y.ToString());
                    // Constructor Enemy: x, y, w, h, animationFrames, hp, money, speed, minX, maxX
                    Enemies.Add(new Enemy(minX, Y, goblinWidth, goblinHeight, goblinAnimationFrames, 50, 10, 2, minX, maxX));
                    chuongngai = ChuongNgai[1];
                    Y = chuongngai.Y - goblinHeight;
                    minX = chuongngai.X + 30;
                    maxX = chuongngai.X + 150;
                    Enemies.Add(new Enemy(minX, Y, goblinWidth, goblinHeight, goblinAnimationFrames, 50, 10, 3, minX, maxX));
                    Enemies.Add(new Enemy(minX + 20, Y, goblinWidth, goblinHeight, goblinAnimationFrames, 60, 15, 3, minX, maxX));
                        
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Không tìm thấy đủ frame mẫu để lấy kích thước Goblin.");
                }
            }
        }
        protected override void LoadChuongNgai()
        {
            var ground = new GameObject
            {
                X = 0,
                Y = 720 - 100,
                Width = 300,
                Height = 50,
                Type = "ground",
                Image = LoadImage("Assets/Platform/ground.png", Color.Brown)
            };
            var ground2 = new GameObject
            {
                X = 600,
                Y = 720 - 100,
                Width = 300,
                Height = 50,
                Type = "ground",
                Image = LoadImage("Assets/Platform/ground.png", Color.Brown)
            };

            var chuongngai1 = new GameObject
            {
                X = 400,
                Y = 720 - 200,
                Width = 50,
                Height = 100,
                Type = "chuongngai",
                Image = LoadImage("Assets/Platform/groundHigh.png", Color.Brown)
            };

            var chuongngai2 = new GameObject
            {
                X = 400,
                Y = 720 - 350,
                Width = 50,
                Height = 50,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/box.png", Color.Brown)
            };
            var chuongngaibox2 = new GameObject
            {
                X = 300,
                Y = 720 - 450,
                Width = 50,
                Height = 50,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/box.png", Color.Brown)
            };

            var chuongngai3 = new GameObject
            {
                X = 500,
                Y = 720 - 450,
                Width = 200,
                Height = 50,
                Type = "ground",
                Image = LoadImage("Assets/Platform/ground.png", Color.Brown)
            };

            var chuongngai4 = new GameObject
            {
                X = 150,
                Y = 720 - 600,
                Width = 50,
                Height = 200,
                Type = "chuongngai",
                Image = LoadImage("Assets/Platform/groundHigh.png", Color.Brown)
            };

            var chuongngai5 = new GameObject
            {
                X = 800,
                Y = 720 - 650,
                Width = 50,
                Height = 200,
                Type = "chuongngai",
                Image = LoadImage("Assets/Platform/groundHigh.png", Color.Brown)
            };
            var chuongngai6 = new GameObject
            {
                X = 20,
                Y = 720 - 200,
                Width = 150,
                Height = 100,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/h2.png", Color.Brown)
            };
            var chuongngai7 = new GameObject
            {
                X = 650,
                Y = 720 - 200,
                Width = 150,
                Height = 100,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/h4.png", Color.Brown)
            };
            var chuongngai8 = new GameObject
            {
                X = 900,
                Y = 720 - 500,
                Width = 150,
                Height = 300,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/groudnHighMulti.png", Color.Brown)
            };

            ChuongNgai.AddRange(new[] { ground, ground2,  chuongngai1, chuongngai2, chuongngaibox2, chuongngai3, chuongngai4, chuongngai5, chuongngai6, chuongngai7, chuongngai8 });
            Obj.AddRange(new[] { ground, ground2,chuongngai1, chuongngai2, chuongngaibox2, chuongngai3, chuongngai4, chuongngai5, chuongngai6, chuongngai7, chuongngai8 });
        }

    }
    public class Level2 : Level
    {
        protected override void LoadKeThu()
        {
            //Dictionary<AnimationState, Dictionary<bool, List<Image>>> goblinAnimationFrames; // Frame animation cho kẻ địch loại Goblin
            // --- Tải và tổ chức frame animation cho Kẻ địch thường (ví dụ: Goblin) ---
            Dictionary<AnimationState, Dictionary<bool, List<Image>>> goblinAnimationFrames = new Dictionary<AnimationState, Dictionary<bool, List<Image>>>();

            // Goblin Run Right
            List<Image> goblinRunRight = new List<Image>()
                 {
                     Image.FromFile("Assets/Enemy/Bat/0.png"), // Thay đường dẫn
                     Image.FromFile("Assets/Enemy/Bat/1.png"),
                     // ... thêm các frame khác ...
                 };
            // Goblin Run Left
            List<Image> goblinRunLeft = new List<Image>()
                 {
                     Image.FromFile("Assets/Enemy/Bat/3.png"), // Thay đường dẫn
                     Image.FromFile("Assets/Enemy/Bat/4.png"),
                     // ...
                 };
            // Goblin Idle Right/Left (Nếu có)
            List<Image> goblinIdleRight = new List<Image>() { /* ... */ };
            List<Image> goblinIdleLeft = new List<Image>() { /* ... */ };
            // Goblin Hit Right/Left (Nếu có)
            List<Image> goblinHitRight = new List<Image>() {
                    Image.FromFile("Assets/Enemy/Bat/2.png"),
                    /* ... */ 
                };
            List<Image> goblinHitLeft = new List<Image>() {
                    Image.FromFile("Assets/Enemy/Bat/5.png"),
                    /* ... */ 
                };


            // Tổ chức vào Dictionary (Chỉ thêm nếu danh sách frame không trống)
            if (goblinRunRight.Count > 0) goblinAnimationFrames[AnimationState.Run] = new Dictionary<bool, List<Image>>() { { true, goblinRunRight }, { false, goblinRunLeft } };
            if (goblinIdleRight.Count > 0) goblinAnimationFrames[AnimationState.Idle] = new Dictionary<bool, List<Image>>() { { true, goblinIdleRight }, { false, goblinIdleLeft } };
            if (goblinHitRight.Count > 0) goblinAnimationFrames[AnimationState.Hit] = new Dictionary<bool, List<Image>>() { { true, goblinHitRight }, { false, goblinHitLeft } };

            // --- Tạo kẻ địch thường (ví dụ: Goblin) ---
            if (goblinAnimationFrames != null && goblinAnimationFrames.Count > 0) // Chỉ tạo nếu có bất kỳ animation nào được tải
            {
                // Lấy kích thước từ một bộ frame bất kỳ (ví dụ: Run Right)
                List<Image> sampleFrames = null;
                if (goblinAnimationFrames.ContainsKey(AnimationState.Run) && goblinAnimationFrames[AnimationState.Run].ContainsKey(true)) sampleFrames = goblinAnimationFrames[AnimationState.Run][true];
                else if (goblinAnimationFrames.ContainsKey(AnimationState.Idle) && goblinAnimationFrames[AnimationState.Idle].ContainsKey(true)) sampleFrames = goblinAnimationFrames[AnimationState.Idle][true];
                // ... (kiểm tra các trạng thái khác nếu cần) ...

                if (sampleFrames != null && sampleFrames.Count > 0)
                {
                    int goblinWidth = sampleFrames[0].Width / 2;
                    int goblinHeight = sampleFrames[0].Height / 2;
                    GameObject chuongngai = ChuongNgai[7];
                    int Y = chuongngai.Y - goblinHeight;
                    int minX = chuongngai.X + 50;
                    int maxX = chuongngai.X + 150;
                    //MessageBox.Show(Y.ToString());
                    // Constructor Enemy: x, y, w, h, animationFrames, hp, money, speed, minX, maxX
                    Enemies.Add(new Enemy(minX, Y, goblinWidth, goblinHeight, goblinAnimationFrames, 50, 10, 2, minX, maxX));
                    chuongngai = ChuongNgai[8];
                    Y = chuongngai.Y - goblinHeight;
                    minX = chuongngai.X + 150;
                    maxX = chuongngai.X + 250;
                    Enemies.Add(new Enemy(minX, Y, goblinWidth, goblinHeight, goblinAnimationFrames, 50, 10, 3, minX, maxX));
                    Enemies.Add(new Enemy(minX + 20, Y, goblinWidth, goblinHeight, goblinAnimationFrames, 60, 15, 3, minX, maxX));

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Không tìm thấy đủ frame mẫu để lấy kích thước Goblin.");
                }
            }
        }
        public Level2(List<GameObject> chuongNgai, List<GameObject> vatPhamThuThap,
                     List<GameObject> gayChet, List<GameObject> obj, List<Enemy> enemies)
            : base(chuongNgai, vatPhamThuThap, gayChet, obj, enemies)
        {
            imgbackground = LoadBGImg("Assets/Background/BG3-01-01-01.png");
        }

        protected override void LoadChuongNgai()
        {
            var ground = new GameObject
            {
                X = 0,
                Y = 720 - 100,
                Width = 400,
                Height = 450,
                Type = "ground",
                Image = LoadImage("Assets/Platform/water-04.png", Color.Brown)
            };
            var platform1 = new GameObject
            {
                X = 300,
                Y = 720 - 450,
                Width = 60,
                Height = 50,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/go.png", Color.Brown)
            };
            var platformwater2 = new GameObject
            {
                X = 390,
                Y = 720 - 200,
                Width = 60,
                Height = 50,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/go.png", Color.Brown)
            };
            var platformwater3 = new GameObject
            {
                X = 120,
                Y = 720 - 500,
                Width = 60,
                Height = 50,
                Type = "chuongngai",
                Image = LoadImage("Assets/Platform/ground2.png", Color.Brown)
            };

            var platform2 = new GameObject
            {
                X = 490,
                Y = 720 - 120,
                Width = 70,
                Height = 100,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/11.png", Color.Brown)
            };

            var platform3 = new GameObject
            {
                X = 120,
                Y = 720 - 550,
                Width = 50,
                Height = 60,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/12.png", Color.Brown)
            };
            var platform4 = new GameObject
            {
                X = 550,
                Y = 720 - 600,
                Width = 70,
                Height = 200,
                Type = "chuongngai",
                Image = LoadImage("Assets/Platform/rock.png", Color.Brown)
            };


            var movingPlatform = new MovingGameObject
            {
                X = 600,
                Y = 720 - 200,
                Width = 200,
                Height = 50,
                Type = "chuongngai",
                Image = LoadImage("Assets/Platform/ground.png", Color.Brown),
                SpeedX = 4,
                DirectionX = 1,
                MinX = 600,
                MaxX = 1000,
                doc = false,
                ngang=true
            };
            var movingPlatform2 = new MovingGameObject
            {
                X = 430,
                Y = 200, 
                Width = 60,
                Height = 50,
                Type = "chuongngai",
                Image = LoadImage("Assets/Platform/go.png", Color.Brown),
                SpeedY = 2,
                DirectionY = 1,
                MinY = 100,
                MaxY = 500,
                ngang = false,
                doc = true
            };
            var movingPlatform3 = new MovingGameObject
            {
                X = 800,
                Y = 300,
                Width = 50,
                Height = 50,
                Type = "chuongngai",
                Image = LoadImage("Assets/Platform/box.png", Color.Brown),
                SpeedY = 5,
                DirectionY = 1,
                MinY = 200,
                MaxY = 500,
                ngang = false,
                doc = true
            };
            ChuongNgai.AddRange(new[] { ground, platform1, platform2, platformwater2, platformwater3, platform3, platform4, movingPlatform, movingPlatform2, movingPlatform3 });
            Obj.AddRange(new[] { ground, platform1, platform2, platformwater2, platformwater3, platform3, platform4, movingPlatform, movingPlatform2, movingPlatform3 });
        }
    }

    public class Level3 : Level
    {
        public Level3(List<GameObject> chuongNgai, List<GameObject> vatPhamThuThap,
                     List<GameObject> gayChet, List<GameObject> obj, List<Enemy> enemies)
            : base(chuongNgai, vatPhamThuThap, gayChet, obj, enemies) {
            ChuongNgai = chuongNgai;
            VatPhamThuThap = vatPhamThuThap;
            GayChet = gayChet;
            Obj = obj;
            imgbackground = LoadBGImg("Assets/Background/bg17.png");
        }

        protected override void LoadChuongNgai()
        {
            var ground = new GameObject
            {
                X = 0,
                Y = 720 - 100,
                Width = 350,
                Height = 100,
                Type = "ground",
                Image = LoadImage("Assets/Platform/redrock-02-03.png", Color.Brown)
            };
            var ground2 = new GameObject
            {
                X = 1200- 350,
                Y = 720 - 100,
                Width = 350,
                Height = 100,
                Type = "ground",
                Image = LoadImage("Assets/Platform/redrock-02-03.png", Color.Brown)
            };
            var ground3 = new GameObject
            {
                X =  450,
                Y = 720 - 100,
                Width = 250,
                Height = 100,
                Type = "ground",
                Image = LoadImage("Assets/Platform/redrock-02-03.png", Color.Brown)
            };
            var tt = new GameObject
            {
                X = 50,
                Y = 720 - 300,
                Width = 150,
                Height = 200,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/16.png", Color.Brown)
            };
            var tt2 = new GameObject
            {
                X = 550,
                Y = 720 - 300,
                Width = 150,
                Height = 200,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/16.png", Color.Brown)
            };
            var platform1 = new GameObject
            {
                X = 150,
                Y = 720 - 550,
                Width = 70,
                Height = 100,
                Type = "chuongngai",
                Image = LoadImage("Assets/Platform/rock3.png", Color.Brown)
            };
            var platform2 = new GameObject
            {
                X = 200,
                Y = 720 - 390,
                Width = 70,
                Height = 100,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/rock3.png", Color.Brown)
            };

            var platform3 = new GameObject
            {
                X = 400,
                Y = 720 - 500,
                Width = 70,
                Height = 300,
                Type = "chuongngai",
                Image = LoadImage("Assets/Platform/rock2.png", Color.Brown)
            };

            var platform4 = new GameObject
            {
                X = 830,
                Y = 720 - 500,
                Width = 70,
                Height = 300,
                Type = "trangtri",
                Image = LoadImage("Assets/Platform/rock2.png", Color.Brown)
            };
            var platform5 = new GameObject
            {
                X = 550,
                Y = 720 - 600,
                Width = 70,
                Height = 200,
                Type = "chuongngai",
                Image = LoadImage("Assets/Platform/rock.png", Color.Brown)
            };
            
            var movingPlatform = new MovingGameObject
            {
                X = 700,
                Y = 720 - 500,
                Width = 70,
                Height = 70,
                Type = "chuongngai",
                Image = LoadImage("Assets/Platform/box.png", Color.Brown),
                SpeedY = 4,
                DirectionY = 1,
                MinY = 720 - 500,
                MaxY = 720 - 100,
                ngang = false,
                doc = true
            };
            ChuongNgai.AddRange(new[] { ground, ground2, ground3 , tt, tt2, platform1, platform2, platform3, platform4, platform5, movingPlatform });
            Obj.AddRange(new[] { ground, ground2, ground3 , tt, tt2, platform1, platform2, platform3, platform4, platform5, movingPlatform });
        }
    }

    public class LevelLoader
    {
        private List<GameObject> ChuongNgai;
        private List<GameObject> VatPhamThuThap;
        private List<GameObject> GayChet;
        private List<GameObject> Obj;
        private List<Enemy> Enemies;

        public List<Enemy> Enemies1
        {
            get
            {
                // Getter: 
                return Enemies;
            }
            set
            {
                // Setter: Gán giá trị mới cho biến private
                Enemies = value;
            }
        }
        public LevelLoader(List<GameObject> chuongNgai, List<GameObject> vatPhamThuThap,
                          List<GameObject> gayChet, List<GameObject> obj, List<Enemy> enemies)
        {
            ChuongNgai = chuongNgai;
            VatPhamThuThap = vatPhamThuThap;
            GayChet = gayChet;
            Obj = obj;
            Enemies = enemies;
        }

        public Image GetBackgroundForLevel(int levelNumber)
        {
            Level level;

            switch (levelNumber)
            {
                case 1:
                    level = new Level1(ChuongNgai, VatPhamThuThap, GayChet, Obj, Enemies);
                    break;
                case 2:
                    level = new Level2(ChuongNgai, VatPhamThuThap, GayChet, Obj, Enemies);
                    break;
                case 3:
                    level = new Level3(ChuongNgai, VatPhamThuThap, GayChet, Obj, Enemies);
                    break;
                default:
                    level = new Level1(ChuongNgai, VatPhamThuThap, GayChet, Obj, Enemies);
                    break;
            }

            return level.imgbackground;
        }
        public void LoadLevel(int levelNumber)
        {
            Level level;

            switch (levelNumber)
            {
                case 1:
                    level = new Level1(ChuongNgai, VatPhamThuThap, GayChet, Obj, Enemies);
                    break;
                case 2:
                    level = new Level2(ChuongNgai, VatPhamThuThap, GayChet, Obj, Enemies);
                    break;
                case 3:
                    level = new Level3(ChuongNgai, VatPhamThuThap, GayChet, Obj, Enemies);
                    break;
                default:
                    MessageBox.Show($"Level {levelNumber} không tồn tại. Đang tải Level 1.");
                    level = new Level1(ChuongNgai, VatPhamThuThap, GayChet, Obj, Enemies);
                    break;
            }

            level.Load();
        }
    }
}