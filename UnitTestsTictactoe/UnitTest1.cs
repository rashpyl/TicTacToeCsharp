using TicTacToe;
using static TicTacToe.Form1;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TicTacToeTests
{
    [TestClass]
    public class TicTacToeTests
    {

        [TestMethod]
        public void TestCheckWin()
        {
            // Create an instance of your Form1 class
            Form1 gameForm = new Form1(3); // Assuming a 3x3 grid for this test

            // Simulate a win for Player X
            Player[,] winningBoard = new Player[3, 3]
            {
                { Player.X, Player.X, Player.X },
                { Player.O, Player.O, Player.None },
                { Player.O, Player.None, Player.None }
            };

            // Check if Player X is the winner
            bool isXWinner = gameForm.CheckWin(winningBoard, Player.X);
            Assert.IsTrue(isXWinner);

            // Clean up the gameForm
        }

        [TestMethod]
        public void TestGetBestMove()
        {
            // Create an instance of your Form1 class
            Form1 gameForm = new Form1(3); // Assuming a 3x3 grid for this test

            // Simulate a game state for Player O to make the best move
            Player[,] testBoard = new Player[3, 3]
            {
                { Player.X, Player.None, Player.None },
                { Player.None, Player.O, Player.X },
                { Player.O, Player.X, Player.None }
            };

            gameForm.gameBoard = testBoard;
            gameForm.currentPlayer = Form1.Player.O;
            gameForm.buttons = DisableUsedButtons(testBoard, gameForm.buttons, gameForm.rowsAndColumns);

            // Get the best move for Player O
            int bestMove = gameForm.GetBestMove();

            // In this case, the best move should be at (0, 2) (row and column indices are zero-based)
            Assert.AreEqual(2, bestMove);

            // Clean up the gameForm
        }

        private List<Button> DisableUsedButtons(Player[,] testBoard, List<Button> buttons, int rowsAndColumns)
        {
            // Iterate through the buttons
            for (int i = 0; i < buttons.Count; i++)
            {
                int rowIndex = buttons[i].TabIndex / rowsAndColumns;
                int colIndex = buttons[i].TabIndex % rowsAndColumns;

                // Check if the corresponding cell in the testBoard is not Player.None
                if (testBoard[rowIndex, colIndex] != Player.None)
                {
                    buttons[i].Enabled = false;
                }
            }

            return buttons;
        }
    }
}
