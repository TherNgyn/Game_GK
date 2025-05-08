using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameNinjaSchool_GK.Character
{
    // --- Enum cho các trạng thái Animation (Loại bỏ Die) ---
    public enum AnimationState
    {
        Idle,
        Run,
        Attack, // Nếu Enemy/Boss có animation tấn công
        Hit    // Khi nhận sát thương
    }

    public class Enemy
    {
        public float X { get; set; } // Vị trí X
        public float Y { get; set; } // Vị trí Y
        public int Width { get; set; } // Kích thước (thường lấy từ ảnh)
        public int Height { get; set; } // Kích thước (thường lấy từ ảnh)
        public int ExpValue { get; set; } = 10;
        // public Image Image { get; set; } // Có thể không cần thuộc tính Image chung này nữa nếu dùng CurrentAnimationFrame

        public int HP { get; set; }
        public int MaxHP { get; private set; }
        public int MoneyValue { get; set; }
        public float Speed { get; set; }
        public bool IsAlive { get; set; }

        // --- Thuộc tính giới hạn di chuyển và hướng ---
        private float MinX;
        private float MaxX;
        private bool MovingRight;
        

        // --- Thuộc tính Animation ---
        public bool IsFacingRight { get; private set; } // Hướng nhìn

        // --- Thuộc tính Animation ---
        private List<Image> currentAnimationFrames; // Danh sách frame animation HIỆN TẠI
        private int currentFrameIndex = 0; // Chỉ số frame hiện tại
        private int animationTimer = 0; // Bộ đếm thời gian chuyển frame
        private const int ANIMATION_DELAY = 8; // Số tick để chuyển frame
        protected AnimationState currentState = AnimationState.Idle; // Trạng thái animation hiện tại
        protected AnimationState previousState = AnimationState.Idle; // Lưu trạng thái trước đó

        // --- Danh sách các frame cho từng trạng thái và hướng (Loại bỏ Die) ---
        protected Dictionary<AnimationState, Dictionary<bool, List<Image>>> allAnimationFrames;
        // ------------------------------------------------------------------

        // Thuộc tính trả về frame ảnh hiện tại
        public Image CurrentAnimationFrame
        {
            get
            {
                if (allAnimationFrames != null && allAnimationFrames.TryGetValue(currentState, out var directionalFrames))
                {
                    if (directionalFrames.TryGetValue(IsFacingRight, out var frames))
                    {
                        if (frames != null && currentFrameIndex >= 0 && currentFrameIndex < frames.Count)
                        {
                            return frames[currentFrameIndex];
                        }
                    }
                }
                return null; // Trả về null nếu không tìm thấy animation hoặc frame
            }
        }

        // Constructor - Cập nhật để không nhận frame animation chết
        public Enemy(float x, float y, int width, int height, Dictionary<AnimationState, Dictionary<bool, List<Image>>> animationFrames, int hp, int moneyValue, float speed, float minX, float maxX) // Không nhận Die frames
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            // --- Lưu tất cả các frame animation (Không bao gồm Die) ---
            this.allAnimationFrames = animationFrames;
            HP = hp;
            MaxHP = hp;
            MoneyValue = moneyValue;
            Speed = speed;
            IsAlive = true;

            MinX = minX;
            MaxX = maxX;
            MovingRight = X < (MinX + MaxX) / 2;
            IsFacingRight = MovingRight;

            
            // Nếu có animation Run, bắt đầu bằng Run, ngược lại dùng Idle
            if (allAnimationFrames != null && allAnimationFrames.ContainsKey(AnimationState.Run))
            {
                SetAnimationState(AnimationState.Run);
            }
            else if (allAnimationFrames != null && allAnimationFrames.ContainsKey(AnimationState.Idle))
            {
                SetAnimationState(AnimationState.Idle);
            }
            else
            {
                this.currentAnimationFrames = null; // Không có animation
            }
           
        }

        //  Phương thức cập nhật Hướng nhìn dựa trên vận tốc 
        public void UpdateFacingDirection(float velocityX)
        {
            if (velocityX > 0) IsFacingRight = true;
            else if (velocityX < 0) IsFacingRight = false;
            // Nếu velocityX == 0, giữ nguyên hướng nhìn cuối cùng
        }
        //


        // --- Phương thức cập nhật Animation ---
        public void UpdateAnimation()
        {
            // Không cập nhật animation nếu đối tượng đã chết
            if (!IsAlive) return;

            if (currentAnimationFrames == null || currentAnimationFrames.Count == 0) return;

            animationTimer++;

            if (animationTimer >= ANIMATION_DELAY)
            {
                animationTimer = 0;
                currentFrameIndex++;

                // Xử lý lặp hoặc kết thúc animation dựa trên trạng thái
                if (currentFrameIndex >= currentAnimationFrames.Count)
                {
                    // --- Xử lý kết thúc animation Hit (Không có animation chết ở đây nữa) ---
                    if (currentState == AnimationState.Hit)
                    {
                        // Nếu là animation nhận sát thương, quay lại trạng thái trước đó (Run/Idle)
                        // Dựa vào Speed để quyết định quay lại Run hay Idle
                        if (Speed > 0) // Hoặc kiểm tra cờ đang di chuyển
                        {
                            SetAnimationState(AnimationState.Run);
                        }
                        else
                        {
                            SetAnimationState(AnimationState.Idle);
                        }
                    }
                    else // Các animation lặp lại (Idle, Run, Attack nếu lặp)
                    {
                        currentFrameIndex = 0; // Quay về frame đầu tiên
                    }
                }
            }
        }
        // ------------------------------------

        // --- Phương thức để đặt Trạng thái Animation hiện tại ---
        public void SetAnimationState(AnimationState newState)
        {
            // Không đổi trạng thái animation nếu đối tượng đã chết
            if (!IsAlive) return; // Giữ nguyên trạng thái nếu chết (mặc dù Die đã bị loại bỏ)

            // Chỉ đổi trạng thái nếu là trạng thái mới (tối ưu)
            if (this.currentState == newState)
            {
                return;
            }

            previousState = this.currentState; // Lưu trạng thái hiện tại trước khi đổi
            this.currentState = newState; // Đổi trạng thái mới

            // Lấy danh sách frame cho trạng thái MỚI và hướng hiện tại
            if (allAnimationFrames != null && allAnimationFrames.TryGetValue(this.currentState, out var directionalFrames))
            {
                if (directionalFrames.TryGetValue(IsFacingRight, out var frames))
                {
                    this.currentAnimationFrames = frames;
                    this.currentFrameIndex = 0; // Bắt đầu animation mới từ frame đầu
                    this.animationTimer = 0; // Reset bộ đếm thời gian
                }
                else // Không tìm thấy frame cho hướng hiện tại, thử hướng ngược lại (tùy chọn)
                {
                    if (directionalFrames.TryGetValue(!IsFacingRight, out var fallbackFrames))
                    {
                        this.currentAnimationFrames = fallbackFrames;
                        this.currentFrameIndex = 0;
                        this.animationTimer = 0;
                        // Bạn có thể cần lật ảnh khi vẽ trong GameForm_Paint nếu dùng fallback frame này
                    }
                    else // Không tìm thấy frame cho trạng thái này ở cả hai hướng
                    {
                        this.currentAnimationFrames = null; // Không có animation
                        System.Diagnostics.Debug.WriteLine($"Warning: No animation frames found for state {this.currentState} in either direction.");
                    }
                }
            }
            else // Không tìm thấy danh sách frame cho trạng thái mới này
            {
                this.currentAnimationFrames = null; // Không có animation
                System.Diagnostics.Debug.WriteLine($"Warning: No animation frames dictionary found for state {this.currentState}.");
            }
        }
        // -------------------------------------------------------------

        // Phương thức di chuyển (Cập nhật hướng và gọi SetAnimationState)
        public void Move()
        {
            // Không di chuyển nếu không sống
            if (!IsAlive) return;

            // Logic di chuyển tuần tra
            float previousX = X; // Lưu vị trí X trước khi di chuyển

            if (MovingRight)
            {
                X += Speed;
                if (X >= MaxX)
                {
                    X = MaxX;
                    MovingRight = false;
                }
            }
            else // Đang di chuyển sang trái
            {
                X -= Speed;
                if (X <= MinX)
                {
                    X = MinX;
                    MovingRight = true;
                }
            }

            // --- Cập nhật hướng nhìn dựa trên vận tốc (thay đổi vị trí X) ---
            if (X > previousX) IsFacingRight = true; // Di chuyển sang phải
            else if (X < previousX) IsFacingRight = false; // Di chuyển sang trái
            // Nếu X == previousX (đứng yên), giữ nguyên hướng nhìn
            // ------------------------------------------------------------

            // --- Cập nhật trạng thái animation dựa trên tốc độ và hướng ---
            // Chỉ cập nhật trạng thái Run/Idle nếu không ở trạng thái Hit
            if (currentState != AnimationState.Hit)
            {
                if (Speed > 0) // Đang di chuyển
                {
                    if (currentState != AnimationState.Run || currentAnimationFrames != GetFrames(AnimationState.Run, IsFacingRight))
                    {
                        SetAnimationState(AnimationState.Run); // Chuyển sang animation chạy hoặc cập nhật frame set theo hướng
                    }
                }
                else // Đứng yên (trong trường hợp Enemy có thể dừng di chuyển, ví dụ: khi tấn công)
                {
                    if (currentState != AnimationState.Idle || currentAnimationFrames != GetFrames(AnimationState.Idle, IsFacingRight))
                    {
                        SetAnimationState(AnimationState.Idle); // Chuyển sang animation đứng yên hoặc cập nhật frame set theo hướng
                    }
                }
            }
            // Nếu đang ở trạng thái Hit, logic chuyển về Run/Idle sẽ được xử lý trong UpdateAnimation khi animation Hit hoàn thành.
            // ---------------------------------------------------------------
        }

        // --- Phương thức giúp lấy danh sách frame từ Dictionary ---
        private List<Image> GetFrames(AnimationState state, bool facingRight)
        {
            if (allAnimationFrames != null && allAnimationFrames.TryGetValue(state, out var directionalFrames))
            {
                if (directionalFrames.TryGetValue(facingRight, out var frames))
                {
                    return frames;
                }
            }
            return null; // Trả về null nếu không tìm thấy
        }
        // ---------------------------------------------------------


        // Phương thức nhận sát thương (Ghi đè để thêm animation nhận sát thương)
        public virtual void TakeDamage(int damage) // <-- Thay 'override' bằng 'virtual'
        {
            int hpBeforeDamage = HP; // Cần giữ lại dòng này nếu logic Hit dựa vào nó

            HP -= damage;
            if (HP < 0) HP = 0;

            // Logic xử lý khi chết (không có animation chết)
            if (HP <= 0) // Kiểm tra cả IsAlive để tránh gọi lại khi đã chết
            {
                IsAlive = false; // Đảm bảo IsAlive được set thành false
                System.Diagnostics.Debug.WriteLine($"Enemy/Boss died at {X}, {Y}"); // Log debug
            }
            else // Vẫn còn sống sau khi nhận sát thương
            {
                // Chỉ chuyển sang animation Hit nếu bị giảm máu và không ở trạng thái Hit
                if (HP < hpBeforeDamage && currentState != AnimationState.Hit)
                {
                    SetAnimationState(AnimationState.Hit); // Chuyển sang animation nhận sát thương
                }
            }
        }
        // 
    }
}


