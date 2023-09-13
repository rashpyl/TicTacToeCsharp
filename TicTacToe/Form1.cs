using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class Form1 : Form
    {
        public enum Player
        {
            X, O
        }

        Player currentPlayer;
        Random random = new Random();
        int humanWinCount = 0;
        int CPUWinCount = 0;
        List<Button> buttons;
        int rowsAndColumns = 5; // Default grid size
        int buttonSize;

        public Form1(int gridRowsAndColumns)
        {
            // Set the grid size based on the input parameter
            rowsAndColumns = gridRowsAndColumns; 

            InitializeComponent();

            // Initialize the game board layout
            AdjustLayout();

            // Adjust the form size based on the grid size
            AdjustFormSize();

            // Start a new game
            RestartGame(); 
        }

        private void PlayerClickButton(object sender, EventArgs e)
        {
            var button = (Button)sender;

            currentPlayer = Player.X;
            button.Text = currentPlayer.ToString();

            // Disable the button after a move
            button.Enabled = false;

            // Change button color for visual feedback
            button.BackColor = Color.PowderBlue;

            // Remove the button from the available moves
            buttons.Remove(button);

            // Check if the game has ended
            CheckGame();

            // Start the CPU's turn timer
            CPUTimer.Start(); 
        }

        private void RestartGame(object sender, EventArgs e)
        {

            // Restart the game when the restart button is clicked
            RestartGame(); 
        }

        private int GetBestMove()
        {
            int bestMove = -1;
            int bestScore = int.MinValue;
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].Enabled)
                {
                    buttons[i].Text = currentPlayer.ToString();
                    int score = MiniMax(buttons, 0, alpha, beta, false);
                    buttons[i].Text = "?";

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = i;
                    }
                    alpha = Math.Max(alpha, bestScore);
                }
            }

            return bestMove;
        }

        private void CPUmove(object sender, EventArgs e)
        {
            if (buttons.Count > 0)
            {
                // Use the GetBestMove function to find the best move for the CPU
                int bestMoveIndex = GetBestMove();

                // Disable the selected button to prevent further moves on it
                buttons[bestMoveIndex].Enabled = false;

                // Set the current player to 'O' (CPU's symbol)
                currentPlayer = Player.O;

                // Set the text of the selected button to 'O' (CPU's symbol)
                buttons[bestMoveIndex].Text = currentPlayer.ToString();

                // Change the background color of the selected button for visual feedback
                buttons[bestMoveIndex].BackColor = Color.DarkOrchid;

                // Remove the selected button from the list of available moves
                buttons.RemoveAt(bestMoveIndex);

                // Check if the game has ended after the CPU's move
                CheckGame();
            }
        }



        private void CheckGame()
        {
            // Check if the human player (X) has won
            if (CheckWin(Player.X.ToString()))
            {
                CPUTimer.Stop(); // Stop the CPU's turn timer
                MessageBox.Show("Human Wins!"); // Display a message indicating human victory
                humanWinCount++; // Increment the human win count
                label1.Text = "Human Wins- " + humanWinCount; // Update the win count label

                // Restart the game after a win
                RestartGame();
            }

            // Check if the CPU player (O) has won
            else if (CheckWin(Player.O.ToString()))
            {
                CPUTimer.Stop(); // Stop the CPU's turn timer
                MessageBox.Show("CPU Wins!"); // Display a message indicating CPU victory
                CPUWinCount++; // Increment the CPU win count
                label2.Text = "CPU Wins- " + CPUWinCount; // Update the win count label

                // Restart the game after a win
                RestartGame();
            }

            // Check if the game has ended in a tie (no more available moves)
            else if (buttons.Count == 0)
            {
                // Display a message indicating a tie game
                MessageBox.Show("Tie game!"); 

                // Restart the game after a tie
                RestartGame();
            }
        }

        private void AdjustLayout()
        {
            // Calculate the button size and maximum overall size (width or height, whichever is smaller)
            var maxOverallSize = Math.Min(ClientSize.Width, ClientSize.Height);

            // Determine the size of each button
            buttonSize = maxOverallSize / rowsAndColumns; 

            // Create and position buttons based on the grid size
            buttons = new List<Button>();
            for (int i = 0; i < rowsAndColumns; i++)
            {
                for (int j = 0; j < rowsAndColumns; j++)
                {
                    var button = new Button();

                    // Set button position
                    button.Location = new Point(j * buttonSize, i * buttonSize);
                    // Assign a unique name to the button
                    button.Name = $"button{i * rowsAndColumns + j + 1}";
                    // Set button size
                    button.Size = new Size(buttonSize, buttonSize);
                    // Set button tab index
                    button.TabIndex = i * rowsAndColumns + j;
                    // Enable visual styling for the button
                    button.UseVisualStyleBackColor = true;
                    // Set button font size
                    button.Font = new Font("Microsoft Sans Serif", 18F);

                    // Attach the event handler for button clicks
                    button.Click += PlayerClickButton;
                    // Add the button to the list of buttons
                    buttons.Add(button);
                    // Add the button to the form's controls
                    Controls.Add(button);
                }
            }

            // Update the positions of labels and the Restart button
            label1.Location = new Point(10, maxOverallSize + 20); // Position of the first label
            label2.Location = new Point(maxOverallSize - label2.Width - 10, maxOverallSize + 20); // Position of the second label

            buttonRestart.Location = new Point((maxOverallSize - buttonRestart.Width) / 2, maxOverallSize + 60); // Position of the Restart button

            // Update the form size to fit the grid and additional UI elements
            Size = new Size(maxOverallSize, maxOverallSize + 130);
        }


        private int MiniMax(List<Button> currentButtons, int depth, int alpha, int beta, bool isMaximizing)
        {
            // Check if O has won
            bool isOWinner = CheckWin(Player.O.ToString());

            // Check if X has won
            bool isXWinner = CheckWin(Player.X.ToString());

            // Terminal states: win, lose, or draw
            if (isOWinner)
                return 10;
            else if (isXWinner)
                return -10;
            else if (currentButtons.Count == 0)
                return 0;

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                foreach (Button button in currentButtons)
                {
                    if (button.Enabled)
                    {
                        button.Text = Player.O.ToString();
                        int score = MiniMax(currentButtons, depth + 1, alpha, beta, false);
                        button.Text = "?";
                        bestScore = Math.Max(score, bestScore);
                        alpha = Math.Max(alpha, score);
                        if (beta <= alpha)
                            break; // Beta cutoff
                    }
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                foreach (Button button in currentButtons)
                {
                    if (button.Enabled)
                    {
                        button.Text = Player.X.ToString();
                        int score = MiniMax(currentButtons, depth + 1, alpha, beta, true);
                        button.Text = "?";
                        bestScore = Math.Min(score, bestScore);
                        beta = Math.Min(beta, score);
                        if (beta <= alpha)
                            break; // Alpha cutoff
                    }
                }
                return bestScore;
            }
        }


        // Check for a win for the specified player symbol
        private bool CheckWin(string player)
        {
            // Check rows
            for (int row = 0; row < rowsAndColumns; row++)
            {
                int count = 0;
                for (int col = 0; col < rowsAndColumns; col++)
                {
                    var button = GetButton(row, col);
                    if (button != null && button.Text == player)
                    {
                        count++;
                        if (count == rowsAndColumns)
                            return true;
                    }
                }
            }

            // Check columns
            for (int col = 0; col < rowsAndColumns; col++)
            {
                int count = 0;
                for (int row = 0; row < rowsAndColumns; row++)
                {
                    var button = GetButton(row, col);
                    if (button != null && button.Text == player)
                    {
                        count++;
                        if (count == rowsAndColumns)
                            return true;
                    }
                }
            }

            // Check diagonals
            int diagonal1Count = 0;
            int diagonal2Count = 0;
            for (int i = 0; i < rowsAndColumns; i++)
            {
                var button1 = GetButton(i, i);
                var button2 = GetButton(i, rowsAndColumns - i - 1);

                if (button1 != null && button1.Text == player)
                {
                    diagonal1Count++;
                    if (diagonal1Count == rowsAndColumns)
                        return true;
                }

                if (button2 != null && button2.Text == player)
                {
                    diagonal2Count++;
                    if (diagonal2Count == rowsAndColumns)
                        return true;
                }
            }

            return false;
        }

        private Button GetButton(int row, int col)
        {
            string buttonName = "button" + (row * rowsAndColumns + col + 1);
            return Controls.OfType<Button>().FirstOrDefault(btn => btn.Name == buttonName);
        }

        private void AdjustFormSize()
        {
            int formWidth = buttonSize * rowsAndColumns + 50; // Add some padding
            int formHeight = buttonSize * rowsAndColumns + 100 + buttonRestart.Height; // Add some padding
            Size = new Size(formWidth, formHeight);
        }

        private void RestartGame()
        {
            buttons = Controls.OfType<Button>().ToList();

            buttons.Remove(buttonRestart);

            foreach (Button button in buttons)
            {
                if (button.Name != "buttonRestart")
                {
                    button.Enabled = true;
                    button.Text = "?";
                    button.BackColor = DefaultBackColor;
                }
            }
        }
    }
}
