using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameNinjaSchool_GK.Character
{
    internal class Bullet
    {
        public float X, Y;
        public int Speed = 70;
        public Bitmap Img = new Bitmap(Image.FromFile("Assets/VuKhi/phitieu3.png"));
        public float Angle; // Thêm góc xoay
        private const float ROTATION_SPEED = 15; // Tốc độ tự xoay của phi tiêu (Bạn có thể thay đổi giá trị này)

        public Bullet(float x, float y, Bitmap img)
        {
            X = x;
            Y = y;
            Img = img;
        }

        public void Move()
        {
            X += Speed;
            Angle += ROTATION_SPEED;
            // Giữ góc trong khoảng 0-360 để tránh số quá lớn
            if (Angle >= 360) Angle -= 360;
            if (Angle < 0) Angle += 360; // Xử lý trường hợp xoay ngược nếu tốc độ âm
        }
    }
}
