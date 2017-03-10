using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using yk.ConnectFour.Logics;

namespace yk.ConnectFour.Tests
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void BoardCreationTest()
        {
            var board = new GameBoard(new GameBoard.BoardBuilder
            {
                Rows = 8,
                Columns = 10
            });

            Assert.AreEqual(10, board.Columns);
            Assert.AreEqual(8, board.Rows);

            for (var a = 0; a < board.Rows; a++)
                for (var s = 0; s < board.Columns; s++)
                    Assert.AreEqual(ChipColor.None, board.GetChipColor(s, a));
        }

        [TestMethod]
        public void BoardFreeSlotsInColumnTest()
        {
            var board = new GameBoard(new GameBoard.BoardBuilder
            {
                Rows = 14,
                Columns = 24
            });

            const int col = 6;

            for (var a = 0; a < 14; a++)
            {
                Assert.AreEqual(true, board.ColumnHasFreeSlots(col));
                board.SetChip(col, a, ChipColor.Black);
            }

            Assert.AreEqual(!true, board.ColumnHasFreeSlots(col));
        }

        [TestMethod]
        public void BoardDetectHorzVictoryTest()
        {
            var board = new GameBoard(new GameBoard.BoardBuilder
            {
                Rows = 6,
                Columns = 6
            });

            for (var a = 0; a < 4; a++)
                board.SetChip(a, 0, ChipColor.Red);

            Assert.IsTrue(board.VictoryFound(3));
            Assert.IsTrue(board.VictoryFound(2));
            Assert.IsTrue(board.VictoryFound(1));
            Assert.IsTrue(board.VictoryFound(0));
            Assert.IsFalse(board.VictoryFound(4));
            Assert.IsFalse(board.VictoryFound(5));
        }

        [TestMethod]
        public void BoardDetectVertVictoryTest()
        {
            var board = new GameBoard(new GameBoard.BoardBuilder
            {
                Rows = 6,
                Columns = 6
            });

            for (var a = 0; a < 4; a++)
                board.SetChip(5, a, ChipColor.Red);

            for (var a = 0; a < 4; a++)
                board.SetChip(0, a, ChipColor.Red);

            for (var a = 0; a < 4; a++)
                board.SetChip(3, a, ChipColor.Red);

            Assert.IsFalse(board.VictoryFound(2));
            Assert.IsFalse(board.VictoryFound(1));
            Assert.IsFalse(board.VictoryFound(4));

            Assert.IsTrue(board.VictoryFound(3));
            Assert.IsTrue(board.VictoryFound(0));
            Assert.IsTrue(board.VictoryFound(5));
        }

        [TestMethod]
        public void BoardDetectUpDownDiagonalVictoryTest()
        {
            var board = new GameBoard(new GameBoard.BoardBuilder
            {
                Rows = 10,
                Columns = 10
            });

            var color = ChipColor.Red;
            for (var a = 0; a < 4; a++)
                for (var s = 0; s < 4 - a; s++)
                {
                    board.SetChip(s, a, color);
                    color = color == ChipColor.Black ? ChipColor.Red : ChipColor.Black;
                }

            Assert.IsFalse(board.VictoryFound(0));
            Assert.IsFalse(board.VictoryFound(1));
            Assert.IsFalse(board.VictoryFound(2));
            Assert.IsFalse(board.VictoryFound(3));

            for (var a = 0; a < 4; a++) // build diagonal
            {
                board.SetChip(a, 3 - a, ChipColor.Red);
            }

            Assert.IsTrue(board.VictoryFound(0));
            Assert.IsTrue(board.VictoryFound(1));
            Assert.IsTrue(board.VictoryFound(2));
            Assert.IsTrue(board.VictoryFound(3));
            Assert.IsFalse(board.VictoryFound(4));
        }

        [TestMethod]
        public void BoardDetectDownUpDiagonalVictoryTest()
        {
            var board = new GameBoard(new GameBoard.BoardBuilder
            {
                Rows = 8,
                Columns = 8
            });

            board.SetChip(0, 0, ChipColor.Red);
            board.SetChip(1, 0, ChipColor.Black);
            board.SetChip(2, 0, ChipColor.Black);
            board.SetChip(3, 0, ChipColor.Red);
            board.SetChip(4, 0, ChipColor.Red);

            board.SetChip(1, 1, ChipColor.Black);
            board.SetChip(2, 1, ChipColor.Black);
            board.SetChip(3, 1, ChipColor.Black);
            board.SetChip(4, 1, ChipColor.Red);

            board.SetChip(2, 2, ChipColor.Black);
            board.SetChip(3, 2, ChipColor.Red);
            board.SetChip(4, 2, ChipColor.Black);

            board.SetChip(3, 3, ChipColor.Black);
            board.SetChip(4, 3, ChipColor.Red);

            board.SetChip(4, 4, ChipColor.Black);

            Assert.IsFalse(board.VictoryFound(0));
            Assert.IsTrue(board.VictoryFound(1));
            Assert.IsTrue(board.VictoryFound(2));
            Assert.IsTrue(board.VictoryFound(3));
            Assert.IsTrue(board.VictoryFound(4));
            Assert.IsFalse(board.VictoryFound(5));
            Assert.IsFalse(board.VictoryFound(6));
        }

        [TestMethod]
        public void GameDropChipTest()
        {
            var board = new GameBoard(new GameBoard.BoardBuilder
            {
                Rows = 5,
                Columns = 5
            });

            var game = new Game(board, ChipColor.Red);
            var col = 0;
            Move move;
            ChipColor current;
            for (var a = 0; a < board.Rows; a++)
            {
                current = game.CurrentColor;
                move = game.DropChip(col);
                Assert.AreEqual(MoveState.Ok, move.State);
                Assert.AreNotEqual(current, game.CurrentColor);
            }
            current = game.CurrentColor;
            move = game.DropChip(col);
            Assert.AreEqual(MoveState.OutOfBounds, move.State);
            Assert.AreEqual(current, game.CurrentColor);
        }

        [TestMethod]
        public void GameOverTestA() // diagonal victory of Reds
        {
            var board = new GameBoard(new GameBoard.BoardBuilder
            {
                Rows = 7,
                Columns = 7
            });

            var moveCnt = 0;
            var g = new Game(board, ChipColor.Black);

            for (var a = 0; a < board.Rows; a++)
            {
                for (var s = 0; s < board.Columns; s++)
                {
                    var move = g.DropChip(s);
                    moveCnt += 1;
                    if (moveCnt != 3*board.Columns + 1)
                        continue;

                    Assert.AreEqual(MoveState.Victory, move.State);
                    Assert.AreEqual(ChipColor.Red, move.Color);
                    return;
                }
            }
        }

        [TestMethod]
        public void GameOverTestB() // vertical victory of Blacks
        {
            var board = new GameBoard(new GameBoard.BoardBuilder
            {
                Rows = 6,
                Columns = 7
            });

            var g = new Game(board, ChipColor.Black);
            Move move = null;
            for (var a = 0; a < board.Rows; a++)
            {
                move = g.DropChip(0);
                Assert.IsTrue(move.State == MoveState.Ok);
            }

            Assert.AreEqual(ChipColor.Black, g.CurrentColor);
            move = g.DropChip(0);
            Assert.AreEqual(ChipColor.Black, g.CurrentColor);
            Assert.IsTrue(move.State == MoveState.OutOfBounds);

            // simulation of players' madness
            move = g.DropChip(2);
            Assert.IsTrue(move.State == MoveState.Ok);
            move = g.DropChip(1);
            Assert.IsTrue(move.State == MoveState.Ok);
            move = g.DropChip(2);
            Assert.IsTrue(move.State == MoveState.Ok);
            move = g.DropChip(1);
            Assert.IsTrue(move.State == MoveState.Ok);
            move = g.DropChip(3);
            Assert.IsTrue(move.State == MoveState.Ok);
            move = g.DropChip(3);
            Assert.IsTrue(move.State == MoveState.Ok);
            move = g.DropChip(2);
            Assert.IsTrue(move.State == MoveState.Ok);
            move = g.DropChip(1);
            Assert.IsTrue(move.State == MoveState.Ok);
            move = g.DropChip(2);
            Assert.IsTrue(move.State == MoveState.Victory);
            Assert.IsTrue(move.Color == ChipColor.Black);
        }
    }
}