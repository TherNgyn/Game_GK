using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameNinjaSchool_GK.Character
{
    internal class MoneyItem
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Image Image { get; set; }
        public int Value { get; set; } // Giá trị tiền của vật phẩm
        public bool Collected { get; set; } = false; // Trạng thái đã được thu thập

        // Constructor
        public MoneyItem(float x, float y, int width, int height, Image image, int value)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Image = image;
            Value = value;
        }

        // Bạn có thể thêm phương thức Move() ở đây nếu muốn tiền rơi ra bay lơ lửng hoặc rơi xuống
        // Ví dụ:
        /*
        public void Move()
        {
             Y += 1; // Rơi xuống chậm
        }
        */

    }
}
