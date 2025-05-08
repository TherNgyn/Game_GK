using GameNinjaSchool_GK.Character;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameNinjaSchool_GK.forms
{
    public partial class BuyForm : Form
    {
        private GameForm parentGameForm;
        private int healthPotCount = 0;
        private int manaPotCount = 0;
        private const int potPrice = 25;

        public BuyForm(GameForm parent)
        {
            InitializeComponent();
            this.parentGameForm = parent;

            this.BackgroundImage = Image.FromFile("Resources/pausemenu_bg.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
             
            this.Size = new Size(300, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            this.Load += BuyForm_Load;
        }

        private void BuyForm_Load(object sender, EventArgs e)
        {
            PictureBox picHP = new PictureBox()
            {
                Image = Image.FromFile("Resources/hp.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(20, 20),
                Size = new Size(40, 40),
                BackColor = Color.Transparent
            };

            Button btnBuyHP = new Button()
            {
                Text = "Mua Máu (25 xu)",
                Location = new Point(70, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Cambria", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 },
                AutoSize = true
            };
            btnBuyHP.Click += BtnBuyHP_Click;

            PictureBox picMP = new PictureBox()
            {
                Image = Image.FromFile("Resources/mana.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(20, 80),
                Size = new Size(40, 40),
                BackColor = Color.Transparent
            };

            Button btnBuyMP = new Button()
            {
                Text = "Mua Mana (25 xu)",
                Location = new Point(70, 85),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Cambria", 10, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 },
                AutoSize = true
            };
            btnBuyMP.Click += BtnBuyMP_Click;

            this.Controls.Add(picHP);
            this.Controls.Add(btnBuyHP);
            this.Controls.Add(picMP);
            this.Controls.Add(btnBuyMP);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                parentGameForm.Location.X + (parentGameForm.Width - this.Width) / 2,
                parentGameForm.Location.Y + (parentGameForm.Height - this.Height) / 2
            );

        }



        private void BtnBuyHP_Click(object sender, EventArgs e)
        {
            if (healthPotCount >= 2)
            {
                MessageBox.Show("Bạn chỉ có thể mua tối đa 2 bình máu.");
                return;
            }

            if (parentGameForm.ninja.HP >= 100)
            {
                MessageBox.Show("Máu đã đầy, không thể mua thêm!");
                return;
            }

            if (parentGameForm.ninja.Money >= potPrice)
            {
                parentGameForm.ninja.Money -= potPrice;
                parentGameForm.ninja.HP = Math.Min(100, parentGameForm.ninja.HP + 50);
                healthPotCount++;

                parentGameForm.Invalidate(); //  vẽ lại thanh máu
            }
            else
            {
                MessageBox.Show("Không đủ xu!");
            }
        }

        private void BtnBuyMP_Click(object sender, EventArgs e)
        {
            if (manaPotCount >= 2)
            {
                MessageBox.Show("Bạn chỉ có thể mua tối đa 2 bình mana.");
                return;
            }

            if (parentGameForm.ninja.MP >= 100)
            {
                MessageBox.Show("Mana đã đầy, không thể mua thêm!");
                return;
            }

            if (parentGameForm.ninja.Money >= potPrice)
            {
                parentGameForm.ninja.Money -= potPrice;
                parentGameForm.ninja.MP = Math.Min(100, parentGameForm.ninja.MP + 50);
                manaPotCount++;

                parentGameForm.Invalidate(); // vẽ lại thanh mana
            }
            else
            {
                MessageBox.Show("Không đủ xu!");
            }
        }

         


    }
}

