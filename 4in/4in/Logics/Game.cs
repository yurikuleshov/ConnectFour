using System;

namespace yk.ConnectFour.Logics
{
    public class Move
    {
        public MoveState State { get; set; }
        public ChipColor Color { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
    }

    public class Game
    {
        private readonly GameBoard m_Board;

        public Game(GameBoard Board, ChipColor FirstColor)
        {
            m_Board = Board;
            CurrentColor = FirstColor;
        }

        public ChipColor CurrentColor { get; private set; }

        public Move DropChip(int Column)
        {
            if (!m_Board.ColumnHasFreeSlots(Column))
                return new Move
                {
                    State = MoveState.OutOfBounds
                };

            var row = m_Board.TakeChip(Column, CurrentColor);
            var state = MoveState.Victory;
            var color = CurrentColor;
            if (!m_Board.VictoryFound(Column))
            {
                state = MoveState.Ok;
                ChangeChipColor();
            }

            return new Move
            {
                Color = color,
                Column = Column,
                State = state,
                Row = row
            };
        }

        private void ChangeChipColor()
        {
            if (CurrentColor == ChipColor.None)
                throw new ArgumentOutOfRangeException("ChipColor");

            CurrentColor = CurrentColor == ChipColor.Black ? ChipColor.Red : ChipColor.Black;
        }
    }
}