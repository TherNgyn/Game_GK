using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GameNinjaSchool_GK.Character
{
    public class Player
    {
        
        
        public int SpeedX = 0, SpeedY = 0;
        public bool IsJumping = false;
        public Image Image = Image.FromFile("Assets/Run/Ninja0.png");
        public int HP = 100;
    }
}
