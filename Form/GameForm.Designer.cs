namespace GameNinjaSchool_GK
{
    partial class GameForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            timerGame = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // timerGame
            // 
            timerGame.Interval = 30;
            timerGame.Tick += timerGame_Tick;
            // 
            // GameForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1262, 673);
            DoubleBuffered = true;
            KeyPreview = true;
            MaximizeBox = false;
            Name = "GameForm";
            Text = "GameForm";
            Load += GameForm_Load;
            Paint += GameForm_Paint;
            KeyDown += GameForm_KeyDown;
            KeyPress += GameForm_KeyPress;
            KeyUp += GameForm_KeyUp;
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Timer timerGame;
    }
}
