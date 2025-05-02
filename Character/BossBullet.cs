using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameNinjaSchool_GK.Character
{
    public class BossBullet
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Image Image { get; set; }
        public float Speed { get; set; } // Tốc độ đạn
        public int Damage { get; set; } // Sát thương đạn gây ra
        public bool IsActive { get; set; } = true; // Trạng thái hoạt động của đạn

        private float targetX; // Vị trí X của Player khi đạn được bắn
        private float targetY; // Vị trí Y của Player khi đạn được bắn
        private float velocityX; // Vận tốc theo trục X
        private float velocityY; // Vận tốc theo trục Y

        // Constructor
        public BossBullet(float startX, float startY, int width, int height, Image image, float speed, int damage, float playerX, float playerY)
        {
            X = startX;
            Y = startY;
            Width = width;
            Height = height;
            Image = image;
            Speed = speed;
            Damage = damage;
            IsActive = true;

            // Lưu vị trí mục tiêu
            targetX = playerX;
            targetY = playerY;

            // Tính toán hướng và vận tốc để đạn bay về phía Player
            float deltaX = targetX - X;
            float deltaY = targetY - Y;
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            if (distance > 0)
            {
                velocityX = (deltaX / distance) * Speed;
                velocityY = (deltaY / distance) * Speed;
            }
            else // Tránh chia cho 0 nếu Player ở cùng vị trí Boss
            {
                velocityX = Speed; // Hoặc một hướng mặc định nào đó
                velocityY = 0;
            }
            // Bạn có thể thêm tính toán góc quay nếu muốn đạn xoay
            // Angle = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI);
        }

        // Phương thức di chuyển
        public void Move()
        {
            if (!IsActive) return;
            X += velocityX;
            Y += velocityY;
        }
    }
}
