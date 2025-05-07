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
        public int Value { get; set; } 
        public bool Collected { get; set; } = false;

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

    }
}
