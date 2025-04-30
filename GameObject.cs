using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameNinjaSchool_GK
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

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

            public Rectangle GetBounds()
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }

        public class CanhGame
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
                    var chuongngai = new GameObject
                    {
                        X = 0,
                        // Lay ground là đáy form
                        Y = 720 - 100 ,
                        Width = 800,
                        Height = 50,
                        Type = "chuongngai"
                    };
                    try
                    {
                        chuongngai.Image = Image.FromFile("Assets/Platform/ground.png");
                    }
                    catch (Exception ex)
                    {
                        chuongngai.Image = new Bitmap(chuongngai.Width, chuongngai.Height);
                        using (Graphics g = Graphics.FromImage(chuongngai.Image))
                        {
                            g.Clear(Color.Brown);
                        }
                    }
                  
                    ChuongNgai.Add(chuongngai);
                    Obj.Add(chuongngai);
                    /*MessageBox.Show(ChuongNgai[0].ToString());*/

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
                    Objects.Add(collectible);*/
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải level {level}: {ex.Message}");
                }
            }
        }
    }
}
