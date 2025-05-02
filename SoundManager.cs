using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace GameNinjaSchool_GK
{
    public static class SoundManager
    {
        private static SoundPlayer player;
        public static bool IsMuted { get; private set; } = false;

        public static void PlayMusic(string path)
        {
            if (IsMuted) return;
            StopMusic();
            player = new SoundPlayer(path);
            player.PlayLooping();
        }

        public static void StopMusic()
        {
            player?.Stop();
        }

        public static void ToggleMute()
        {
            IsMuted = !IsMuted;
            if (IsMuted) StopMusic();
            else player?.PlayLooping();
        }
    }
}
