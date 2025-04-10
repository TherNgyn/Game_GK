using GameNinjaSchool_GK.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameNinjaSchool_GK.Controller
{
    public class MoveController
    {
        public static bool movingLeft = false, movingRight = false;
        public static void Update(Player player)
        {
            if (movingLeft) player.X -= 5;
            if (movingRight) player.X += 5;
        }
        public static void KeyDown(Player player, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) movingLeft = true;
            if (e.KeyCode == Keys.Right) movingRight = true;
        }
        public static void KeyUp(Player player, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) movingLeft = false;
            if (e.KeyCode == Keys.Right) movingRight = false;
        }
    }
}
