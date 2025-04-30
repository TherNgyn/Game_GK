using GameNinjaSchool_GK.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameNinjaSchool_GK.Controller
{
    public class MoveController
    {
        private static int luc_hut = 1;
        private static int ground;

        public static void SetGroundLevel(int level)
        {
            ground = level;
        }

        public static void Update(Ninja player)
        {
            player.X += player.SpeedX;

            if (player.Jump || player.Falling)
            {
                player.SpeedY += luc_hut;
            }

            player.Y += player.SpeedY;

            if (player.Jump && player.Y <= player.banDauJumpY - player.maxJumpHeight)
            {
                player.Jump = false;
                player.Falling = true;
            }

            player.UpdateAnimation();
        }

        public static void KeyDown(Ninja player, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    player.X -= 5;
                    player.SpeedX = -5;
                    player.TurnAround = true;
                    break;
                case Keys.Right:
                    player.X += 5;
                    player.SpeedX = 5;
                    player.TurnAround = false;
                    break;
                case Keys.Up:
                    if (!player.Jump && !player.Falling)
                    {
                        player.Jump = true;
                        player.banDauJumpY = player.Y;
                        player.SpeedY = player.jumpHeight;
                    }
                    break;
                case Keys.Down:
                    if (player.Jump || player.Falling)
                    {
                        player.SpeedY += 2; 
                    }
                    break;
                default:
                    break;
            }
        }

        public static void KeyUp(Ninja player, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Left && player.SpeedX < 0) ||
                (e.KeyCode == Keys.Right && player.SpeedX > 0))
            {
                player.SpeedX = 0;
            }
        }
    }
}