using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Timer = System.Windows.Forms.Timer;


namespace GameNinjaSchool_GK.Character
{
    //  Lớp abstract cho các nhân vật: NPC, ninja, enemy
    public abstract class Character
    {
        public int X;
        public int Y;
        public int width;
        public int height;
        public int SpeedX = 0;
        public int SpeedY = 0;
        public bool Jump  = false;
        public bool Falling = false;
        public bool TurnAround = false;
        public Image Image;
        public int HP;
        public int MaxHP;
         
       // Amimation 
        public int AnimationFrame = 0;
        public List<Image> RunFrames  = new List<Image>();
        public string currentAni = "dungyen";

       // Method cập nhật animation 
        public abstract void UpdateAnimation();

       // Method là nhân vật có sống hong 
        public bool isLive()
        {
            return HP > 0;
        }

        // Method mất máu 
        public virtual void isDamaged(int mucmau)
        {
            HP = HP - mucmau;
            if (HP < 0)
            {
                HP = 0;
            }
        }

        // Method lấy bound bên ngoài của Character
        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, width, height);
        }
    }

    // Ninja class ke thua Character
    public class Ninja : Character
    {
        // Dat mau ban dau
        /*public int HP = 100;*/
        public int MP = 100;
        public int EXP  = 0;
        public int Level = 1;
        public int jumpHeight = -20;
        public int maxJumpHeight = 200;
        public int banDauJumpY = 0;
        public bool IsClimbing   = false;
        public List<Image> JumpFrames = new List<Image>();
        public List<Image> ClimbFrames = new List<Image>();


        public int Money { get; set; } = 0;

        // --- Thuộc tính và Timer cho trạng thái bất tử tạm thời ---
        private bool isInvincible = false; // Cờ bất tử
        // Sửa đổi dòng khai báo Timer:
        private Timer invincibilityTimer; // <-- Sử dụng tên đầy đủ ở đây
        private const int InvincibilityDuration = 1000; // Thời gian bất tử (ms)
        public Ninja()
        {
            width = 30;
            height = 65;
            HP = 100;
            MaxHP = 100;

            // Khởi tạo Timer bất tử
            invincibilityTimer = new Timer(); // <-- Sử dụng tên đầy đủ ở đây
            invincibilityTimer.Interval = InvincibilityDuration;
            invincibilityTimer.Tick += InvincibilityTimer_Tick;
            invincibilityTimer.Stop();

            try
            {
                for (int i = 0; i <= 5; i++)
                {
                    RunFrames.Add(Image.FromFile($"Assets/Run/Ninja{i}.png"));
                }
                for (int i = 1; i <= 2; i++)
                {
                    JumpFrames.Add(Image.FromFile($"Assets/Jump/Jump{i}.png"));
                }
                Image = RunFrames[0];
            }
            catch (Exception ex)
            {
                Image = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(Image))
                {
                    g.Clear(Color.Red);
                }
            }
        }

        // --- Phương thức xử lý khi Timer bất tử Tick ---
        private void InvincibilityTimer_Tick(object sender, EventArgs e)
        {
            isInvincible = false;
            invincibilityTimer.Stop();
            System.Diagnostics.Debug.WriteLine("Invincibility ended.");
        }

        public override void UpdateAnimation()
        {
            int frameIndex = 0;
            if (Jump)
            {
                currentAni = "jump";
                if(SpeedY>0)
                {
                    frameIndex = 1;
                }
                if (JumpFrames.Count > frameIndex)
                    Image = JumpFrames[frameIndex];
            }
            else if(Falling)
            {
                currentAni = "fall";
                if (JumpFrames.Count > 2)
                    Image = JumpFrames[2];
            }
            else if (SpeedX != 0)
            {
                currentAni = "run";
                if (RunFrames.Count > 0)
                    Image = RunFrames[AnimationFrame % RunFrames.Count];
            }
            else
            {
                currentAni = "dungyen";
                if (RunFrames.Count > 0)
                    Image = RunFrames[0];
            }
            AnimationFrame++;
            if (AnimationFrame >= 30)
            {
                AnimationFrame = 0;
            }
        }

        public void GainExp(int amount)
        {
            EXP += amount;
            int expNeeded = Level * 50; 

            if (EXP >= expNeeded)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
            EXP -= (Level - 1) * 100; 
            MaxHP += 10; 
            HP = MaxHP; 
            MP = 100; 
        }
        public void TakeDamage(int damage)
        {
            if (!isInvincible)
            {
                HP -= damage;
                if (HP < 0) HP = 0;

                isInvincible = true;
                invincibilityTimer.Start();

                System.Diagnostics.Debug.WriteLine($"Player took {damage} damage. Current HP: {HP}");

                if (HP <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("Player Died!");
                    // Logic xử lý khi chết
                }
            }
        }
    }


    /*public class Enemy : Character
    {
        public int DamageAmount { get; set; } = 10;
        public int ExpValue { get; set; } = 20;
        public bool IsActive { get; set; } = true;
        public int DetectionRange { get; set; } = 300;
        public int AttackRange { get; set; } = 50;
        public string EnemyType { get; set; } = "basic";

        public Enemy(string type = "basic", int x = 0, int y = 0)
        {
            EnemyType = type;
            X = x;
            Y = y;
            width = 70;
            height = 100;
            HP = 50;
            MaxHP = 50;

            try
            {
                // Load enemy animations based on type
                for (int i = 0; i <= 3; i++)
                {
                    RunFrames.Add(Image.FromFile($"Assets/Enemies/{type}/Run{i}.png"));
                }

                // Default image
                Image = RunFrames[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading enemy animations: {ex.Message}");
                // Create fallback image
                Image = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(Image))
                {
                    g.Clear(Color.Red);
                }
            }
        }

        public override void UpdateAnimation()
        {
            if (SpeedX != 0)
            {
                currentAni = "run";
                if (RunFrames.Count > 0)
                    Image = RunFrames[AnimationFrame % RunFrames.Count];
            }
            else
            {
                currentAni = "idle";
                if (RunFrames.Count > 0)
                    Image = RunFrames[0];
            }

            // Increment animation frame
            AnimationFrame++;
            if (AnimationFrame >= 20) AnimationFrame = 0;
        }

        // Enemy-specific AI method
        public void UpdateAI(Ninja player)
        {
            if (!IsActive || !isLive()) return;

            // Calculate distance to ninja
            int dx = player.X - X;
            int distance = Math.Abs(dx);

            // If ninja is within detection range
            if (distance < DetectionRange)
            {
                // Move towards ninja
                SpeedX = (dx > 0) ? 2 : -2;

                // Attack if in range
                if (distance < AttackRange)
                {
                    Attack(player);
                }
            }
            else
            {
                // Stand idle if ninja is out of range
                SpeedX = 0;
            }

            // Apply movement
            X += SpeedX;
        }

        public void Attack(Ninja player)
        {
            player.isDamaged(DamageAmount);
        }

        // Override the base isDamaged to handle enemy-specific effects
        public override void isDamaged(int amount)
        {
            base.isDamaged(amount);

            // If enemy dies
            if (HP <= 0)
            {
                IsActive = false;
                // Death animation could be triggered here
            }
        }
    }*/
}