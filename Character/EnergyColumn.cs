using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameNinjaSchool_GK.Character
{
    // Class biểu diễn cột năng lượng/vùng ảnh hưởng của Boss
    public class EnergyColumn
    {
        public float X { get; set; } // Vị trí X của cột
        public float Y { get; set; } // Vị trí Y của cột
        public int Width { get; set; } // Chiều rộng cột
        public int Height { get; set; } // Chiều cao cột (thường bằng chiều cao màn hình)

        // Ảnh cho các trạng thái khác nhau
        public Image IndicatorImage { get; set; } // Ảnh vệt sáng báo hiệu
        public Image ActiveImage { get; set; } // Ảnh cột năng lượng đầy đủ

        public int Damage { get; set; } // Sát thương cột gây ra mỗi tick (nếu Player đứng trong)
        public float Duration { get; set; } // Thời gian cột năng lượng tồn tại (giây)
        private float remainingDuration; // Thời gian tồn tại còn lại

        // Trạng thái skill
        public bool IsActive { get; private set; } = false; // True khi cột năng lượng đã xuất hiện đầy đủ
        private float chargeDuration; // Thời gian vệt sáng báo hiệu tồn tại trước khi cột xuất hiện (giây)
        private float timeCharged = 0; // Thời gian đã sạc (đếm từ 0 đến chargeDuration)

        public bool IsFinished { get; private set; } = false; // True khi cột đã hết thời gian tồn tại

        // Constructor
        public EnergyColumn(float x, float y, int width, int height, Image indicatorImg, Image activeImg, int damage, float duration, float chargeDurationSec)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            IndicatorImage = indicatorImg;
            ActiveImage = activeImg;
            Damage = damage;
            Duration = duration;
            remainingDuration = duration;
            chargeDuration = chargeDurationSec;
            IsActive = false; // Ban đầu chưa Active (chỉ có indicator)
            IsFinished = false; // Ban đầu chưa kết thúc
        }

        // Phương thức cập nhật trạng thái và thời gian tồn tại của cột năng lượng
        public void Update(float deltaTimeSeconds) // deltaTimeSeconds là thời gian trôi qua kể từ tick trước (tính bằng giây)
        {
            if (IsFinished) return;

            if (!IsActive)
            {
                // Đang trong giai đoạn báo hiệu/sạc
                timeCharged += deltaTimeSeconds;
                if (timeCharged >= chargeDuration)
                {
                    IsActive = true; // Chuyển sang trạng thái Active
                    timeCharged = chargeDuration; // Đảm bảo không vượt quá
                    System.Diagnostics.Debug.WriteLine("Energy column activated.");
                }
            }
            else
            {
                // Đang trong giai đoạn Active
                remainingDuration -= deltaTimeSeconds;
                if (remainingDuration <= 0)
                {
                    IsFinished = true; // Hết thời gian, đánh dấu kết thúc
                    remainingDuration = 0; // Đảm bảo không âm
                    System.Diagnostics.Debug.WriteLine("Energy column finished.");
                }
            }
        }

        // Phương thức kiểm tra va chạm (dùng trong GameForm)
        public Rectangle GetBounds()
        {
            return new Rectangle((int)X, (int)Y, Width, Height);
        }

        // Bạn có thể thêm các phương thức khác như PlaySound(), Animation() khi chuyển trạng thái
    }
}
