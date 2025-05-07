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
        private static string currentTrack = "";
        public static bool IsMuted { get; private set; } = false;

        public static void PlayMusic(string path)
        {
            // Luôn cập nhật track
            currentTrack = path;

            // Nếu đang mute thì không tạo player, không phát
            if (IsMuted)
                return;

            // Nếu đang phát bài này rồi thì không cần phát lại
            if (player != null && currentTrack == path)
                return;

            StopMusic();

            try
            {
                player = new SoundPlayer(path);
                player.PlayLooping();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi phát nhạc: {ex.Message}");
            }
        }

        public static void StopMusic()
        {
            player?.Stop();
            player = null;
        }

        public static void ToggleMute()
        {
            IsMuted = !IsMuted;

            if (IsMuted)
            {
                StopMusic();
            }
            else
            {
                // 🔧 Khi bật lại âm, phải tạo player mới nếu nó bị null
                if (!string.IsNullOrEmpty(currentTrack))
                {
                    try
                    {
                        player = new SoundPlayer(currentTrack);
                        player.PlayLooping();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi phát nhạc khi bật âm: {ex.Message}");
                    }
                }
            }
        }
    }
}
