using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HafızaOyunu
{
    public class GameForm : Form
    {
        List<Image> images = new List<Image>();
        Image backImage;
        PictureBox firstClicked = null;
        PictureBox secondClicked = null;
        int playerTurn = 1;
        int player1Score = 0;
        int player2Score = 0;
        int totalPairsFound = 0;
        Timer initialShowTimer = new Timer();
        Timer selectionTimer = new Timer();
        Timer flipBackTimer = new Timer();
        FlowLayoutPanel flowLayoutPanel1;
        Label lblScore;
        Label lblPlayerTurn;

        string projectPath = AppDomain.CurrentDomain.BaseDirectory + @"images\";

        public GameForm()
        {
            InitializeGameForm();
            LoadImages();
            StartGame();
        }

        private void InitializeGameForm()
        {
            this.Text = "Hafıza Oyunu";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblScore = new Label();
            lblScore.Text = "Oyuncu 1: 0 | Oyuncu 2: 0";
            lblScore.Location = new Point(50, 10);
            lblScore.Font = new Font("Arial", 14, FontStyle.Bold);
            lblScore.AutoSize = true;
            this.Controls.Add(lblScore);

            lblPlayerTurn = new Label();
            lblPlayerTurn.Text = "Sıra: Oyuncu 1";
            lblPlayerTurn.Location = new Point(600, 10);
            lblPlayerTurn.Font = new Font("Arial", 14, FontStyle.Bold);
            lblPlayerTurn.AutoSize = true;
            lblPlayerTurn.ForeColor = Color.Blue;
            this.Controls.Add(lblPlayerTurn);

            flowLayoutPanel1 = new FlowLayoutPanel();
            flowLayoutPanel1.Size = new Size(800, 600);
            flowLayoutPanel1.Location = new Point(50, 50);
            flowLayoutPanel1.WrapContents = true;
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.AutoScroll = true;
            this.Controls.Add(flowLayoutPanel1);

            initialShowTimer.Interval = 5000;
            initialShowTimer.Tick += InitialShowTimer_Tick;

            selectionTimer.Interval = 5000;
            selectionTimer.Tick += SelectionTimer_Tick;

            flipBackTimer.Interval = 1000;
            flipBackTimer.Tick += FlipBackTimer_Tick;
        }

        private void LoadImages()
        {
            backImage = Image.FromFile(projectPath + "back.png");

            for (int i = 1; i <= 20; i++)
            {
                Image img = Image.FromFile(projectPath + i + ".png");
                images.Add(img);
                images.Add(img);
            }

            Random rnd = new Random();
            images = images.OrderBy(x => rnd.Next()).ToList();

            foreach (var img in images)
            {
                PictureBox pb = new PictureBox();
                pb.Width = 80;
                pb.Height = 80;
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Image = backImage;
                pb.Tag = img;
                pb.Margin = new Padding(5);
                pb.Click += PictureBox_Click;
                flowLayoutPanel1.Controls.Add(pb);
            }
        }

        private void StartGame()
        {
            foreach (PictureBox pb in flowLayoutPanel1.Controls)
                pb.Image = pb.Tag as Image;
            initialShowTimer.Start();
        }

        private void InitialShowTimer_Tick(object sender, EventArgs e)
        {
            initialShowTimer.Stop();
            foreach (PictureBox pb in flowLayoutPanel1.Controls)
                pb.Image = backImage;
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox clicked = sender as PictureBox;
            if (clicked.Image != backImage) return;
            clicked.Image = clicked.Tag as Image;
            if (firstClicked == null)
            {
                firstClicked = clicked;
                selectionTimer.Start();
            }
            else if (secondClicked == null)
            {
                secondClicked = clicked;
                selectionTimer.Stop();
                CheckMatch();
            }
        }

        private void SelectionTimer_Tick(object sender, EventArgs e)
        {
            selectionTimer.Stop();
            if (firstClicked != null && secondClicked == null)
            {
                firstClicked.Image = backImage;
                firstClicked = null;
                playerTurn = (playerTurn == 1) ? 2 : 1;
                lblPlayerTurn.Text = $"Sıra: Oyuncu {playerTurn}";
                lblPlayerTurn.ForeColor = (playerTurn == 1) ? Color.Blue : Color.Red;
            }
        }

        private void FlipBackTimer_Tick(object sender, EventArgs e)
        {
            flipBackTimer.Stop();
            if (firstClicked != null) firstClicked.Image = backImage;
            if (secondClicked != null) secondClicked.Image = backImage;
            firstClicked = null;
            secondClicked = null;
        }

        private void CheckMatch()
        {
            if (firstClicked.Tag == secondClicked.Tag)
            {
                if (playerTurn == 1) player1Score++;
                else player2Score++;
                lblScore.Text = $"Oyuncu 1: {player1Score} | Oyuncu 2: {player2Score}";
                totalPairsFound++;
                firstClicked = null;
                secondClicked = null;
                if (player1Score == 11 || player2Score == 11 || totalPairsFound == 20) EndGame();
            }
            else
            {
                flipBackTimer.Start();
                playerTurn = (playerTurn == 1) ? 2 : 1;
                lblPlayerTurn.Text = $"Sıra: Oyuncu {playerTurn}";
                lblPlayerTurn.ForeColor = (playerTurn == 1) ? Color.Blue : Color.Red;
            }
        }

        private void EndGame()
        {
            string winner;
            if (player1Score > player2Score) winner = "Oyuncu 1 Kazandı!";
            else if (player2Score > player1Score) winner = "Oyuncu 2 Kazandı!";
            else winner = "Berabere!";
            MessageBox.Show(winner, "Oyun Bitti");
            Application.Restart();
        }
    }
}
