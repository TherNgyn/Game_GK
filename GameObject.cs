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
        /*public int Direction = 1;
        public int MinX, MaxX;*/

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

        public Level(List<GameObject> chuongNgai, List<GameObject> vatPhamThuThap,
                    List<GameObject> gayChet, List<GameObject> obj)
        {
            ChuongNgai = chuongNgai;
            VatPhamThuThap = vatPhamThuThap;
            GayChet = gayChet;
            Obj = obj;
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
                     List<GameObject> gayChet, List<GameObject> obj)
            : base(chuongNgai, vatPhamThuThap, gayChet, obj)
        {
            imgbackground = LoadBGImg("Assets/Background/BGLevel1.png");
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
        public Level2(List<GameObject> chuongNgai, List<GameObject> vatPhamThuThap,
                     List<GameObject> gayChet, List<GameObject> obj)
            : base(chuongNgai, vatPhamThuThap, gayChet, obj)
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
                Type = "chuongngaiMove",
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
                     List<GameObject> gayChet, List<GameObject> obj)
            : base(chuongNgai, vatPhamThuThap, gayChet, obj) {
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

        // ... other methods
    }

    // LevelLoader class 
    public class LevelLoader
    {
        private List<GameObject> ChuongNgai;
        private List<GameObject> VatPhamThuThap;
        private List<GameObject> GayChet;
        private List<GameObject> Obj;

        public LevelLoader(List<GameObject> chuongNgai, List<GameObject> vatPhamThuThap,
                          List<GameObject> gayChet, List<GameObject> obj)
        {
            ChuongNgai = chuongNgai;
            VatPhamThuThap = vatPhamThuThap;
            GayChet = gayChet;
            Obj = obj;
        }

        public Image GetBackgroundForLevel(int levelNumber)
        {
            Level level;

            switch (levelNumber)
            {
                case 1:
                    level = new Level1(ChuongNgai, VatPhamThuThap, GayChet, Obj);
                    break;
                case 2:
                    level = new Level2(ChuongNgai, VatPhamThuThap, GayChet, Obj);
                    break;
                case 3:
                    level = new Level3(ChuongNgai, VatPhamThuThap, GayChet, Obj);
                    break;
                default:
                    level = new Level1(ChuongNgai, VatPhamThuThap, GayChet, Obj);
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
                    level = new Level1(ChuongNgai, VatPhamThuThap, GayChet, Obj);
                    break;
                case 2:
                    level = new Level2(ChuongNgai, VatPhamThuThap, GayChet, Obj);
                    break;
                case 3:
                    level = new Level3(ChuongNgai, VatPhamThuThap, GayChet, Obj);
                    break;
                default:
                    MessageBox.Show($"Level {levelNumber} không tồn tại. Đang tải Level 1.");
                    level = new Level1(ChuongNgai, VatPhamThuThap, GayChet, Obj);
                    break;
            }

            level.Load();
        }
    }
}
/*public class CanhGame
{
    public List<GameObject> Obj = new List<GameObject>();
    public List<GameObject> ChuongNgai = new List<GameObject>();
    public List<GameObject> VatPhamThuThap = new List<GameObject>();
    public List<GameObject> GayChet = new List<GameObject>();

    public void LoadLevel(int level)
    {
        ChuongNgai.Clear();
        VatPhamThuThap.Clear();
        GayChet.Clear();

        try
        {
            var ground = new GameObject
            {
                X = 0,
                // Lay ground là đáy form
                Y = 720 - 100 ,
                Width = 300,
                Height = 50,
                Type = "ground"
            };
            var chuongngai1 = new GameObject
            {
                X = 300,
                Y = 720 - 200,
                Width = 20,
                Height = 50,
                Type = "chuongngai"
            };
            var chuongngai2 = new GameObject
            {
                X = 400,
                Y = 720 - 350,
                Width = 50,
                Height = 50,
                Type = "chuongngai"
            };
            var chuongngai3 = new GameObject
            {
                X = 500,
                Y = 720 - 450,
                Width = 300,
                Height = 50,
                Type = "chuongngai"
            };
            var chuongngai4 = new GameObject
            {
                X = 600,
                Y = 720 - 550,
                Width = 50,
                Height = 200,
                Type = "grond"
            };
            var chuongngai5 = new GameObject
            {
                X = 800,
                Y = 720 - 650,
                Width = 50,
                Height = 200,
                Type = "chuongngai"
            };
            try
            {
                ground.Image = Image.FromFile("Assets/Platform/ground.png");
                chuongngai1.Image = Image.FromFile("Assets/Platform/groundHigh.png");
                chuongngai2.Image = Image.FromFile("Assets/Platform/box.png");
                chuongngai3.Image = Image.FromFile("Assets/Platform/ground.png");
                chuongngai4.Image = Image.FromFile("Assets/Platform/groundHigh.png");
                chuongngai5.Image = Image.FromFile("Assets/Platform/groundHigh.png");
            }
            catch (Exception ex)
            {
                ground.Image = new Bitmap(ground.Width, ground.Height);
                using (Graphics g = Graphics.FromImage(ground.Image))
                {
                    g.Clear(Color.Brown);
                }
            }
            ChuongNgai.Add(ground);
            ChuongNgai.Add(chuongngai1);
            ChuongNgai.Add(chuongngai2);
            ChuongNgai.Add(chuongngai3);
            ChuongNgai.Add(chuongngai4);
            ChuongNgai.Add(chuongngai5);
            Obj.Add(ground);
            Obj.Add(chuongngai1);
            Obj.Add(chuongngai2);
            Obj.Add(chuongngai3);
            Obj.Add(chuongngai4);
            Obj.Add(chuongngai5);
            *//*MessageBox.Show(ChuongNgai[0].ToString());*/

/*// Add a collectible item with an image
var collectible = new GameObject
{
    X = 300,
    Y = 550,
    Width = 30,
    Height = 30,
    Type = "exp"
};

try
{
    collectible.Image = Image.FromFile(Path.Combine(Application.StartupPath, "Assets", "Items", "exp_orb.png"));
}
catch (Exception ex)
{
    // Fallback image if the file can't be loaded
    collectible.Image = new Bitmap(collectible.Width, collectible.Height);
    using (Graphics g = Graphics.FromImage(collectible.Image))
    {
        g.Clear(Color.Yellow);
    }
}

VatPhamThuThap.Add(collectible);
Objects.Add(collectible);*//*
}
catch (Exception ex)
{
MessageBox.Show($"Lỗi tải level {level}: {ex.Message}");
}
}
}*/

