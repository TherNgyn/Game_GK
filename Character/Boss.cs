using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameNinjaSchool_GK.Character
{
    internal class Boss : Enemy
    {
        // --- Thuộc tính riêng cho Boss ---
        // Cooldown cho việc bắn đạn thông thường
        private System.Windows.Forms.Timer shootTimer;
        private const int ShootInterval = 5000; // Bắn mỗi 10 giây (có thể điều chỉnh)

        // Biến để lưu trữ tham chiếu đến Player (Boss cần biết vị trí Player để tấn công)
        private Ninja targetPlayer;

        // Cooldown và Timer cho Skill 1
        private System.Windows.Forms.Timer skill1Timer;
        private const int Skill1Cooldown = 12000; // Skill 1 hồi chiêu 20 giây
        //private bool isSkill1Ready = true;

        // Cooldown và Timer cho Skill 2
        private System.Windows.Forms.Timer skill2Timer;
        private const int Skill2Cooldown = 7000; // Skill 2 hồi chiêu 24 giây
        //private bool isSkill2Ready = true;

        // Cooldown và Timer cho Skill 3
        private System.Windows.Forms.Timer skill3Timer;
        private const int Skill3Cooldown = 15000; // Skill 3 hồi chiêu 25 giây
        //private bool isSkill3Ready = true;

        private Random random = new Random(); // Để chọn skill ngẫu nhiên

        // Cần biến để tham chiếu đến danh sách đạn của Boss trong GameForm
        // Hoặc phương thức để thêm đạn vào danh sách đó từ class Boss
        // Ví dụ: public Action<BossBullet> OnShootBossBullet; // Delegate/Event để GameForm lắng nghe

        // Cần biến để lưu ảnh đạn của Boss
        private Image bossBulletImage;

        private GameForm gameFormRef;
        // --------------------------------------------------

        // --- Thông tin về Minions sẽ triệu hồi ---
        private Image minionImage; // Ảnh của lính tay sai
        private int minionHP = 30; // Máu của lính tay sai (có thể điều chỉnh)
        private int minionMoneyValue = 5; // Tiền rơi ra từ lính tay sai
        private float minionSpeed = 2.5f; // Tốc độ di chuyển của lính tay sai
        private int numberOfMinionsToSummon = 2; // Số lượng lính tay sai triệu hồi mỗi lần

        private Image columnIndicatorImage; // Ảnh vệt sáng báo hiệu
        private Image columnActiveImage;    // Ảnh cột năng lượng đầy đủ
        private const int HealAmount = 50; // <-- Lượng máu hồi mỗi lần dùng Skill 3 (Ví dụ: 150 HP)

        // Cooldown và Timer cho Skill 4 (Rapid Fire Barrage)
        private System.Windows.Forms.Timer skill4Timer;
        private const int Skill4Cooldown = 18000; // Skill 4 hồi chiêu 18 giây (có thể điều chỉnh)

        // Timer và biến đếm cho chuỗi bắn trong Skill 4
        private System.Windows.Forms.Timer barrageTimer; // Timer phụ cho khoảng cách giữa các viên đạn
        private const int BurstShootDelay = 200; // Khoảng cách giữa mỗi viên đạn trong chuỗi bắn (ms)
        private const int TotalBarrageShots = 8; // Tổng số viên đạn trong một chuỗi bắn
        private bool isBarrageActive = false; // Cờ để biết Boss có đang trong chuỗi bắn không
        private int barrageShotCount = 0; // Biến đếm số viên đạn đã bắn trong chuỗi hiện tại
        // Constructor của Boss
        public Boss(float x, float y, int width, int height, Dictionary<AnimationState, Dictionary<bool, List<Image>>> bossAnimationFrames, Image bossBulletImg, GameForm gameForm, Image minionImg, Image indicatorImg, Image activeImg, int hp, int moneyValue, float speed, float minX, float maxX, Ninja playerRef)
            // Truyền các tham số cho Enemy base constructor
            : base(x, y, width, height, bossAnimationFrames, hp, moneyValue, speed, minX, maxX)
        {
            // --- allAnimationFrames đã được gán trong Enemy base constructor ---

            // --- Lưu các tham chiếu và ảnh cho skill ---
            this.targetPlayer = playerRef;
            this.bossBulletImage = bossBulletImg;
            this.gameFormRef = gameForm;
            this.minionImage = minionImg;
            this.columnIndicatorImage = indicatorImg;
            this.columnActiveImage = activeImg;
            // -----------------------------------------

            // --- Thiết lập animation ban đầu cho Boss (sử dụng SetAnimationState kế thừa từ Enemy) ---
            // Base constructor Enemy đã set animation ban đầu dựa trên Dictionary.
            // Nếu bạn muốn Boss luôn bắt đầu ở Idle, bạn có thể gọi lại SetAnimationState ở đây:
            if (allAnimationFrames != null && allAnimationFrames.ContainsKey(AnimationState.Idle))
            {
                SetAnimationState(AnimationState.Idle); // Boss bắt đầu với animation đứng yên
            }
            // -------------------------------------------------------------------------------------

            // --- Khởi tạo các Timer cho skill và bắn đạn ---
            shootTimer = new System.Windows.Forms.Timer(); // Đã khai báo biến
            shootTimer.Interval = ShootInterval;
            shootTimer.Tick += ShootTimer_Tick;
            //shootTimer.Start(); // Bắt đầu Timer bắn đạn thông thường (Có thể Start sau khi Boss xuất hiện đầy đủ)

            skill1Timer = new System.Windows.Forms.Timer(); // Đã khai báo biến
            skill1Timer.Interval = Skill1Cooldown;
            skill1Timer.Tick += Skill1Timer_Tick;
            //skill1Timer.Stop(); // Dừng timer ban đầu, sẽ Start trong StartSkillTimers()

            skill2Timer = new System.Windows.Forms.Timer(); // Đã khai báo biến
            skill2Timer.Interval = Skill2Cooldown;
            skill2Timer.Tick += Skill2Timer_Tick;
            //skill2Timer.Stop(); // Dừng timer ban đầu

            skill3Timer = new System.Windows.Forms.Timer(); // Đã khai báo biến
            skill3Timer.Interval = Skill3Cooldown;
            skill3Timer.Tick += Skill3Timer_Tick;
            //skill3Timer.Stop(); // Dừng timer ban đầu

            // --- Khởi tạo các Timer cho Skill 4 ---
            skill4Timer = new System.Windows.Forms.Timer();
            skill4Timer.Interval = Skill4Cooldown; // Interval là thời gian hồi chiêu của skill
            skill4Timer.Tick += Skill4Timer_Tick; // Gắn phương thức xử lý Tick cho skill4Timer

            barrageTimer = new System.Windows.Forms.Timer();
            barrageTimer.Interval = BurstShootDelay; // Interval là khoảng cách giữa các viên đạn
            barrageTimer.Tick += BarrageTimer_Tick; // Gắn phương thức xử lý Tick cho barrageTimer
            barrageTimer.Stop(); // Đảm bảo barrageTimer dừng ban đầu
                                 // Timers chính (skill4Timer) sẽ được Start trong StartSkillTimers()
                                 // ------------------------------------
                                 // ----------------------------------------------
        }

        // Ghi đè phương thức TakeDamage (Nếu cần hiệu ứng đặc biệt cho Boss khi bị đánh, ngoài animation Hit đã có từ Enemy)
        // Logic xử lý giảm máu, IsAlive, và animation Hit đã có trong Enemy.TakeDamage()
        // Animation chết đã bị loại bỏ.
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage); // Gọi phương thức của lớp cha

            if (!IsAlive) // Boss chết
            {
                // Boss sẽ bị xóa ngay lập tức trong GameForm.
                // Thêm logic/hiệu ứng riêng của Boss khi chết tại đây (ví dụ: âm thanh đặc biệt, vụ nổ lớn)
                System.Diagnostics.Debug.WriteLine("Boss died - Triggering death effects."); // Log debug
                                                                                             // TriggerBossDeathEffects(); // Cần phương thức này
                                                                                             // Dừng tất cả các timer liên quan đến tấn công/skill
                if (shootTimer != null) shootTimer.Stop();
                if (skill1Timer != null) skill1Timer.Stop();
                if (skill2Timer != null) skill2Timer.Stop();
                if (skill3Timer != null) skill3Timer.Stop();
                // --- Dừng các Timer liên quan đến Skill 4 ---
                if (skill4Timer != null) skill4Timer.Stop(); // Dừng Timer hồi chiêu
                if (barrageTimer != null) barrageTimer.Stop(); // Dừng Timer bắn chuỗi (nếu đang chạy)
                                                               // -------------------------
            }
            else // Vẫn sống sau khi nhận sát thương
            {
                // Logic animation Hit đã được xử lý trong base.TakeDamage()
                // Thêm logic/hiệu ứng riêng của Boss khi nhận sát thương tại đây
                // PlayBossHitSound();
            }
        }

        // Ghi đè phương thức Move() nếu Boss có cách di chuyển khác
        // Logic cập nhật hướng và chuyển đổi Run/Idle animation đã có trong Enemy.Move() và UpdateFacingDirection()
        // Bạn chỉ cần đảm bảo Speed của Boss được cập nhật đúng khi nó di chuyển/dừng lại.
        // Nếu muốn Boss di chuyển theo logic riêng, ghi đè Move() và sau khi tính X mới,
        // gọi UpdateFacingDirection() và SetAnimationState() thủ công hoặc gọi base.Move() để dùng logic của Enemy.
        public virtual void Move()
        {
            if (!IsAlive) return; // Không di chuyển nếu chết

            float previousX = X;
            // --- Logic di chuyển riêng của Boss ---
            // Ví dụ: di chuyển về phía Player (cần đảm bảo targetPlayer không null)
            if (targetPlayer != null)
            {
                if (this.X < targetPlayer.X +600) X += Speed; // Giữ khoảng cách 100px
                else if (this.X > targetPlayer.X + 100) X -= Speed;
                else // Nếu ở gần Player, có thể dừng di chuyển và tấn công (nếu có animation Attack)
                {
                    // Tùy chọn: Nếu Boss đứng yên khi tấn công, đặt Speed = 0 tạm thời
                    // float originalSpeed = Speed;
                    // Speed = 0;
                    // SetAnimationState(AnimationState.Attack); // Cần logic animation Attack và quay lại
                    // Speed = originalSpeed;
                }
            }
            // --- Kết thúc Logic di chuyển riêng của Boss ---


            // Cập nhật hướng nhìn dựa trên sự thay đổi vị trí X
            UpdateFacingDirection(X - previousX);

            // Cập nhật trạng thái animation (Run/Idle) dựa trên Speed và hướng
            // Chỉ cập nhật trạng thái Run/Idle nếu không ở trạng thái Hit hoặc Attack
            // Cần đảm bảo Speed được set > 0 khi Boss di chuyển, = 0 khi Boss đứng yên/tấn công
            if (currentState != AnimationState.Hit && currentState != AnimationState.Attack) // Cần thêm trạng thái Attack nếu Boss có animation tấn công riêng
            {
                // Kiểm tra xem Boss có thực sự di chuyển không (vị trí X có đổi không)
                if (Math.Abs(X - previousX) > 0.1f) // Sử dụng ngưỡng nhỏ để tránh lỗi float
                {
                    SetAnimationState(AnimationState.Run); // Chuyển sang animation chạy
                }
                else // Đứng yên
                {
                    SetAnimationState(AnimationState.Idle); // Chuyển sang animation đứng yên
                }
            }
            // Nếu đang ở trạng thái Hit hoặc Attack, logic chuyển về Run/Idle sẽ được xử lý trong UpdateAnimation (cho Hit) hoặc trong logic Skill/Timer phụ (cho Attack).
        }


        public void StopAllSkill()
        {
            //MessageBox.Show("stop");
            // Dừng tất cả các timer liên quan đến tấn công/skill
            if (shootTimer != null) shootTimer.Stop();
            if (skill1Timer != null) skill1Timer.Stop();
            if (skill2Timer != null) skill2Timer.Stop();
            if (skill3Timer != null) skill3Timer.Stop();
            // --- Dừng các Timer liên quan đến Skill 4 ---
            if (skill4Timer != null) skill4Timer.Stop(); // Dừng Timer hồi chiêu
            if (barrageTimer != null) barrageTimer.Stop(); // Dừng Timer bắn chuỗi (nếu đang chạy)
        }
        // --- Logic Bắn đạn thông thường (Skill cơ bản) ---
        private void ShootTimer_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"ShootTimer_Tick fired. Time: {DateTime.Now.ToString("HH:mm:ss.fff")}"); // <-- Thêm dòng này TẠI ĐẦU HÀM
            System.Diagnostics.Debug.WriteLine($"  Shoot Conditions: IsAlive={IsAlive}, currentState={currentState}, targetPlayerIsNull={targetPlayer == null}, gameFormRefIsNull={gameFormRef == null}, bossBulletImageIsNull={bossBulletImage == null}");

            // Chỉ bắn nếu Boss còn sống, có tham chiếu đến Player và GameForm, và có ảnh đạn, và không ở trạng thái Hit/Die
            if (IsAlive && targetPlayer != null && gameFormRef != null && bossBulletImage != null ) // Thêm điều kiện trạng thái
            {
                System.Diagnostics.Debug.WriteLine("Boss shot a bullet.");

                // Tạo một BossBullet mới hướng về phía Player
                // Vị trí bắn có thể là từ tâm Boss hoặc một điểm cụ thể trên ảnh Boss
                float bulletStartX = this.X + this.Width / 2;
                float bulletStartY = this.Y + this.Height / 2;

                // Tạo BossBullet. Damage có thể là giá trị cố định hoặc từ thuộc tính của Boss
                // Constructor: startX, startY, width, height, image, speed, damage, playerX, playerY
                // Tốc độ đạn Boss (ví dụ 20)
                BossBullet bullet = new BossBullet(bulletStartX, bulletStartY, 60, 60, bossBulletImage, 20, 15, targetPlayer.X, targetPlayer.Y); // w, h, img, speed, damage

                // Thêm đạn này vào danh sách đạn của Boss trong GameForm
                // Kiểm tra giới hạn nếu dùng
                // if (gameFormRef.bossBullets.Count < gameFormRef.MAX_BOSS_BULLETS)
                gameFormRef.AddBossBullet(bullet);

                // Có thể thêm logic animation Attack ở đây nếu animation Attack được dùng cho bắn thường
                // SetAnimationState(AnimationState.Attack); // Cần logic quay về Idle/Run sau animation Attack
            }
        }


        // --- Logic Skill 1: Cột năng lượng ---
        private void Skill1Timer_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Skill1Timer_Tick fired. Time: {DateTime.Now.ToString("HH:mm:ss.fff")}"); // <-- Thêm dòng này TẠI ĐẦU HÀM
            System.Diagnostics.Debug.WriteLine($"  Skill1 Conditions: IsAlive={IsAlive}, currentState={currentState}, gameFormRefIsNull={gameFormRef == null}, indicatorIsNull={columnIndicatorImage == null}, activeIsNull={columnActiveImage == null}");

            // Logic để quyết định khi nào dùng Skill 1
            // Chỉ dùng skill nếu Boss còn sống, Skill sẵn sàng, có tham chiếu và ảnh cần thiết, và không ở trạng thái Hit/Die
            if (IsAlive  && gameFormRef != null && columnIndicatorImage != null && columnActiveImage != null ) // Thêm điều kiện trạng thái
            {
                System.Diagnostics.Debug.WriteLine("Boss performed Skill 1: Energy Columns!");
                PerformSkill1(); // Thực hiện logic skill

                // Reset cooldown Skill 1
                //isSkill1Ready = false;
                skill1Timer.Interval = Skill1Cooldown;
                skill1Timer.Start();

                // Có thể thêm animation Skill 1 ở đây nếu có
                // SetAnimationState(AnimationState.Skill1); // Cần thêm Skill1 vào enum và logic xử lý
            }
            else if (!IsAlive && skill1Timer != null) // Dừng timer nếu Boss chết
            {
                MessageBox.Show("check stop");
                skill1Timer.Stop();
            }
        }

        private void PerformSkill1()
        {
            // Logic thực hiện Skill 1: Tạo 5 cột năng lượng
            if (gameFormRef == null || columnIndicatorImage == null || columnActiveImage == null) return;

            int numberOfColumns = 5; // Số lượng cột
            int columnWidth = 160; // Chiều rộng mỗi cột (có thể điều chỉnh)
            // Lấy kích thước màn hình từ gameFormRef (đảm bảo gameFormRef không null)
            int screenWidth = gameFormRef.ClientSize.Width;
            int screenHeight = gameFormRef.ClientSize.Height;
            float columnDuration = 2; // Cột tồn tại 2 giây sau khi Active
            float columnChargeDuration = 1.5f; // Vệt sáng báo hiệu 1.5 giây trước khi Active
            int columnDamage = 5; // Sát thương mỗi tick nếu Player đứng trong cột Active

            // Tính toán vị trí X cho 5 cột năng lượng trải đều màn hình
            // Khoảng cách giữa các cột
            float spacing = (float)(screenWidth - numberOfColumns * columnWidth) / (numberOfColumns + 1);

            for (int i = 0; i < numberOfColumns; i++)
            {
                // Vị trí X của cột
                float columnX = spacing * (i + 1) + columnWidth * i;
                // Vị trí Y của cột (thường là đỉnh màn hình)
                float columnY = 0; // Bắt đầu từ đỉnh màn hình

                // Tạo đối tượng EnergyColumn mới
                // Constructor: x, y, w, h, indicatorImg, activeImg, damage, duration, chargeDurationSec
                EnergyColumn column = new EnergyColumn(columnX, columnY, columnWidth, screenHeight, columnIndicatorImage, columnActiveImage, columnDamage, columnDuration, columnChargeDuration);

                // Thêm cột năng lượng này vào danh sách trong GameForm
                gameFormRef.AddEnergyColumn(column);
            }
            System.Diagnostics.Debug.WriteLine($"Boss created {numberOfColumns} energy columns.");
        }


        // --- Logic Skill 2: Bắn đạn Targeted ---
        private void Skill2Timer_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Skill2Timer_Tick fired. Time: {DateTime.Now.ToString("HH:mm:ss.fff")}"); // <-- Thêm dòng này TẠI ĐẦU HÀM
            System.Diagnostics.Debug.WriteLine($"  Skill2 Conditions: IsAlive={IsAlive}, currentState={currentState}, targetPlayerIsNull={targetPlayer == null}, bossBulletImageIsNull={bossBulletImage == null}, gameFormRefIsNull={gameFormRef == null}");

            // Chỉ dùng skill nếu Boss còn sống, Skill sẵn sàng, có tham chiếu và ảnh cần thiết, và không ở trạng thái Hit/Die
            if (IsAlive && targetPlayer != null && bossBulletImage != null && gameFormRef != null ) // Thêm điều kiện trạng thái
            {
                System.Diagnostics.Debug.WriteLine("Boss performed Skill 2: Targeted Attack!");
                PerformSkill2(); // Thực hiện logic skill

                // Reset cooldown Skill 2
                //isSkill2Ready = false;
                skill2Timer.Interval = Skill2Cooldown;
                skill2Timer.Start();

                // Có thể thêm animation Skill 2 ở đây nếu có
                // SetAnimationState(AnimationState.Skill2); // Cần thêm Skill2 vào enum và logic xử lý
            }
            else if (!IsAlive && skill2Timer != null) // Dừng timer nếu Boss chết
            {
                skill2Timer.Stop();
            }
        }

        private void PerformSkill2()
        {
            // Logic thực hiện Skill 2: Bắn 3 viên đạn đồng thời hướng về vị trí Player HIỆN TẠI
            if (targetPlayer == null || bossBulletImage == null || gameFormRef == null) return;
            //MessageBox.Show(targetPlayer.X.ToString());
            //MessageBox.Show(targetPlayer.Y.ToString());

            float startX = this.X + this.Width / 2;
            float startY = this.Y + this.Height / 2;
            float bulletSpeed = 10; // Tốc độ đạn Skill 2
            int bulletDamage = 20; // Sát thương đạn Skill 2

            // Tạo 3 viên đạn hướng về vị trí Player lúc này
            BossBullet bullet1 = new BossBullet(startX, startY, 60, 60, bossBulletImage, bulletSpeed, bulletDamage, targetPlayer.X, targetPlayer.Y);
            BossBullet bullet2 = new BossBullet(startX, startY, 60, 60, bossBulletImage, bulletSpeed, bulletDamage, targetPlayer.X, targetPlayer.Y);
            BossBullet bullet3 = new BossBullet(startX, startY, 60, 60, bossBulletImage, bulletSpeed, bulletDamage, targetPlayer.X, targetPlayer.Y);

            // --- Thêm các viên đạn này vào danh sách đạn của Boss trong GameForm ---
            // Kiểm tra giới hạn nếu dùng
            // if (gameFormRef.bossBullets.Count + 3 <= gameFormRef.MAX_BOSS_BULLETS) {
            gameFormRef.AddBossBullet(bullet1);
            gameFormRef.AddBossBullet(bullet2);
            gameFormRef.AddBossBullet(bullet3);
            // }
            // -------------------------------------------------------------------

            // Để bắn liên tiếp có delay, bạn cần dùng một Timer phụ trong Boss hoặc GameForm để tạo từng viên đạn sau khoảng thời gian
            // Có thể thêm animation Skill 2 ở đây nếu có
            // SetAnimationState(AnimationState.Skill2); // Cần logic quay về Idle/Run sau animation Skill 2
        }


        // --- Logic Skill 3: Hồi máu ---
        private void Skill3Timer_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Skill3Timer_Tick fired (Heal). Time: {DateTime.Now.ToString("HH:mm:ss.fff")}"); // Cập nhật log cho rõ

            // Logic để quyết định khi nào dùng Skill 3 (Hồi máu)
            // Chỉ dùng skill nếu Boss còn sống VÀ không ở trạng thái Hit VÀ máu chưa đầy
            // Hồi máu không cần các tham chiếu ảnh/GameForm nếu logic hồi máu chỉ sửa HP nội bộ
            if (IsAlive && currentState != AnimationState.Hit && HP < MaxHP) // Kiểm tra máu chưa đầy
            {
                System.Diagnostics.Debug.WriteLine("  Skill3Timer_Tick: Conditions met for healing.");
                HealBoss(); // <-- GỌI PHƯƠNG THỨC HỒI MÁU MỚI

                // Optional: Thêm animation hồi máu nếu có
                // SetAnimationState(AnimationState.Heal); // Cần thêm AnimationState.Heal và animation frames tương ứng

            }
            else
            {
                // Log lý do skill KHÔNG được thực hiện
                System.Diagnostics.Debug.WriteLine($"  Skill3Timer_Tick: Healing NOT performed. Alive={IsAlive}, State={currentState}, HP={HP}, MaxHP={MaxHP}"); // Cập nhật log lý do
            }

            // --- DỪNG TIMER CHỈ KHI BOSS KHÔNG CÒN SỐNG ---
            if (!IsAlive && skill3Timer != null)
            {
                System.Diagnostics.Debug.WriteLine("  Skill3Timer_Tick: Boss not alive. Stopping timer.");
                skill3Timer.Stop(); // Dừng timer chỉ khi Boss chết
            }
            // -------------------------------------------
        }

        // --- Tạo phương thức hồi máu cho Boss ---
        private void HealBoss()
        {
            System.Diagnostics.Debug.WriteLine($"HealBoss called. HP before: {HP}/{MaxHP}"); // Log trước khi hồi máu

            // Tăng máu lên một lượng xác định (sử dụng hằng số HealAmount)
            HP += HealAmount;

            // Giới hạn máu không vượt quá MaxHP
            if (HP > MaxHP)
            {
                HP = MaxHP;
            }

            System.Diagnostics.Debug.WriteLine($"Boss healed {HealAmount} HP. Current HP: {HP}/{MaxHP}"); // Log sau khi hồi máu
                                                                                                          // Optional: Gửi tín hiệu đến GameForm để cập nhật thanh máu Boss nếu cần
                                                                                                          // OnBossHealed?.Invoke(HP, MaxHP); // Nếu bạn có event này
        }
        // ---------------------------------------
        // --- Phương thức xử lý sự kiện Tick cho Timer chính của Skill 4 ---
        // Phương thức này được gọi khi skill4Timer tick (khi hết thời gian hồi chiêu)
        private void Skill4Timer_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Skill4Timer_Tick fired (Initiate Barrage). Time: {DateTime.Now.ToString("HH:mm:ss.fff")}");

            // Kiểm tra các điều kiện để BẮT ĐẦU một chuỗi bắn
            // Chỉ bắt đầu chuỗi bắn nếu Boss còn sống, không ở trạng thái Hit, không đang trong một chuỗi bắn khác, và có đủ tài nguyên
            if (IsAlive && currentState != AnimationState.Hit && !isBarrageActive && targetPlayer != null && bossBulletImage != null && gameFormRef != null)
            {
                System.Diagnostics.Debug.WriteLine("  Skill4Timer_Tick: Conditions met. Initiating Barrage.");

                // --- Khởi tạo chuỗi bắn ---
                isBarrageActive = true; // Đặt cờ đang bắn chuỗi
                barrageShotCount = 0; // Reset bộ đếm viên đạn
                if (barrageTimer != null) barrageTimer.Start(); // <-- Bắt đầu Timer phụ để bắn từng viên
                                                                // ------------------------

                // ... Tùy chọn: Thêm animation hoặc hiệu ứng khi bắt đầu chuỗi bắn ...
                // SetAnimationState(AnimationState.Skill4); // Nếu bạn có animation Skill 4
            }
            else
            {
                // Ghi log lý do chuỗi bắn KHÔNG được bắt đầu
                System.Diagnostics.Debug.WriteLine($"  Skill4Timer_Tick: Barrage NOT initiated. Alive={IsAlive}, State={currentState}, BarrageActive={isBarrageActive}, Target: {targetPlayer != null}, Img: {bossBulletImage != null}, GameForm: {gameFormRef != null}");
            }

            // Dừng skill4Timer chỉ khi Boss KHÔNG còn sống
            if (!IsAlive && skill4Timer != null)
            {
                System.Diagnostics.Debug.WriteLine("  Skill4Timer_Tick: Boss not alive. Stopping skill4Timer.");
                skill4Timer.Stop();
            }
        }


        // --- Phương thức xử lý sự kiện Tick cho Timer phụ của Skill 4 ---
        // Phương thức này được gọi bởi barrageTimer, mỗi khi Boss cần bắn một viên đạn trong chuỗi
        private void BarrageTimer_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"BarrageTimer_Tick fired. Time: {DateTime.Now.ToString("HH:mm:ss.fff")}. Shot Count: {barrageShotCount}");

            // Kiểm tra các điều kiện để BẮN VIÊN ĐẠN tiếp theo trong chuỗi
            // Phải đang trong chuỗi bắn, Boss còn sống, không bị Hit, và chưa bắn đủ số viên
            if (isBarrageActive && IsAlive && currentState != AnimationState.Hit && barrageShotCount < TotalBarrageShots)
            {
                // Đảm bảo tài nguyên cần thiết để bắn
                if (targetPlayer != null && bossBulletImage != null && gameFormRef != null)
                {
                    System.Diagnostics.Debug.WriteLine($"  BarrageTimer_Tick: Firing bullet {barrageShotCount + 1} of {TotalBarrageShots}.");

                    // --- BẮN MỘT VIÊN ĐẠN (Logic tương tự như bắn đạn thông thường) ---
                    float bulletStartX = this.X + this.Width / 2;
                    float bulletStartY = this.Y + this.Height / 2;
                    float bulletSpeed = 25; // Tốc độ đạn của chuỗi bắn (có thể khác đạn thường)
                    int bulletDamage = 10; // Sát thương đạn của chuỗi bắn (có thể khác đạn thường)
                                           // Constructor BossBullet: startX, startY, width, height, image, speed, damage, playerX, playerY
                    BossBullet bullet = new BossBullet(bulletStartX, bulletStartY, 60, 60, bossBulletImage, bulletSpeed, bulletDamage, targetPlayer.X, targetPlayer.Y);
                    gameFormRef.AddBossBullet(bullet); // Thêm đạn vào danh sách của GameForm
                                                       // ------------------------------------------------------

                    // ... Tùy chọn: Thêm animation hoặc hiệu ứng khi bắn một viên đạn ...

                    barrageShotCount++; // Tăng bộ đếm viên đạn đã bắn

                    // --- Kiểm tra xem chuỗi bắn đã hoàn thành chưa ---
                    if (barrageShotCount >= TotalBarrageShots)
                    {
                        System.Diagnostics.Debug.WriteLine("  BarrageTimer_Tick: Barrage finished.");
                        isBarrageActive = false; // Đặt cờ kết thúc chuỗi bắn
                        if (barrageTimer != null) barrageTimer.Stop(); // <-- Dừng Timer phụ bắn chuỗi
                                                                       // Timer chính skill4Timer vẫn chạy để đếm ngược cho lần bắn chuỗi tiếp theo
                    }
                }
                else
                {
                    // Log nếu điều kiện bắn viên đạn riêng lẻ không được đáp ứng (tham chiếu null)
                    System.Diagnostics.Debug.WriteLine($"  BarrageTimer_Tick: Conditions NOT met for firing bullet {barrageShotCount + 1}. Target: {targetPlayer != null}, Img: {bossBulletImage != null}, GameForm: {gameFormRef != null}");
                    // Tùy chọn: Nếu tài nguyên biến mất giữa chừng, dừng chuỗi bắn
                    isBarrageActive = false;
                    if (barrageTimer != null) barrageTimer.Stop();
                }
            }
            else
            {
                // Log nếu Timer phụ tick nhưng không bắn (không đang trong chuỗi, Boss chết/bị hit, hoặc đã đủ số viên)
                System.Diagnostics.Debug.WriteLine($"  BarrageTimer_Tick: NOT firing bullet. BarrageActive={isBarrageActive}, Alive={IsAlive}, State={currentState}, ShotCount={barrageShotCount}, TotalShots={TotalBarrageShots}");

                // Đảm bảo chuỗi bắn dừng nếu Boss chết hoặc bị Hit trong lúc đang bắn
                if (isBarrageActive && (!IsAlive || currentState == AnimationState.Hit))
                {
                    System.Diagnostics.Debug.WriteLine("  BarrageTimer_Tick: Barrage interrupted because Boss is not alive or is hit.");
                    isBarrageActive = false;
                    if (barrageTimer != null) barrageTimer.Stop();
                }
                // Đảm bảo chuỗi bắn dừng nếu somehow bộ đếm vượt quá tổng số viên (phòng lỗi)
                if (barrageShotCount >= TotalBarrageShots)
                {
                    System.Diagnostics.Debug.WriteLine("  BarrageTimer_Tick: Barrage finished based on shot count (redundant check).");
                    isBarrageActive = false;
                    if (barrageTimer != null) barrageTimer.Stop();
                }
            }
        }
            // Phương thức để bắt đầu các Timer Skill sau khi Boss xuất hiện hoặc sẵn sàng
            // Nên gọi phương thức này sau khi tạo Boss trong GameForm_Load
        public void StartSkillTimers()
        {
            // Bắt đầu cả timer bắn đạn thường và timer skill
            if (IsAlive)
            {
                if (shootTimer != null && !shootTimer.Enabled) shootTimer.Start();

                // Đặt interval ban đầu ngẫu nhiên một chút để các skill không bắn cùng lúc ngay từ đầu
                // Chỉ bắt đầu các timer skill nếu chúng chưa chạy
                if (skill1Timer != null && !skill1Timer.Enabled) skill1Timer.Interval = random.Next(1000, Skill1Cooldown);
                if (skill2Timer != null && !skill2Timer.Enabled) skill2Timer.Interval = random.Next(1000, Skill2Cooldown);
                if (skill3Timer != null && !skill3Timer.Enabled) skill3Timer.Interval = random.Next(1000, Skill3Cooldown);

                if (skill1Timer != null && !skill1Timer.Enabled) skill1Timer.Start();
                if (skill2Timer != null && !skill2Timer.Enabled) skill2Timer.Start();
                if (skill3Timer != null && !skill3Timer.Enabled) skill3Timer.Start();

                // --- Start Timer chính cho Skill 4 ---
                if (skill4Timer != null && !skill4Timer.Enabled)
                {
                    // Có thể đặt Interval ban đầu ngẫu nhiên cho lần tick đầu tiên
                    skill4Timer.Interval = random.Next(1000, Skill4Cooldown);
                    skill4Timer.Start();
                    System.Diagnostics.Debug.WriteLine($"skill4Timer started with initial interval: {skill4Timer.Interval}ms");
                }
                else if (skill4Timer != null && skill4Timer.Enabled) System.Diagnostics.Debug.WriteLine("skill4Timer already enabled.");
                // -------------------------
            }
        }
    }
}
    
