using System;
using System.Linq;

namespace yk.ConnectFour.Logics
{
    public class Row
    {
    }

    public enum ChipColor
    {
        None,
        Red,
        Black
    }

    public enum MoveState
    {
        Ok, // move is accepted, game is continuing
        Occupied, // 
        OutOfBounds, // colums/board is overflown    
        Victory // last move led to victory
    }

    public class GameBoard
    {
        private readonly ChipColor[] m_Cells;

        public GameBoard(BoardBuilder Builder)
        {
            Rows = Builder.Rows;
            Columns = Builder.Columns;
            m_Cells = new ChipColor[Rows*Columns];
            for (var a = 0; a < Rows; a++)
                for (var s = 0; s < Columns; s++)
                {
                    m_Cells[a*Columns + s] = ChipColor.None;
                }
        }

        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public bool NoMoreCells
        {
            get { return m_Cells.All(c => c != ChipColor.None); }
        }

        public ChipColor GetChipColor(int X, int Y)
        {
            if (Y >= Rows)
                throw new ArgumentOutOfRangeException("Y");
            if (X >= Columns)
                throw new ArgumentOutOfRangeException("X");
            return m_Cells[Y*Columns + X];
        }


        public MoveState SetChip(int X, int Y, ChipColor Chip)
        {
            if (Y >= Rows)
                return MoveState.OutOfBounds;
            if (X >= Columns)
                return MoveState.OutOfBounds;

            var pos = Y*Columns + X;

            m_Cells[pos] = Chip;
            return MoveState.Ok;
        }

        public bool ColumnHasFreeSlots(int Column)
        {
            for (var a = Rows - 1; a >= 0; a--)
            {
                if (CellEmpty(Column, a))
                    return true;
            }
            return false;
        }

        private bool ColumnIsEmpty(int Column)
        {
            return CellEmpty(Column, 0);
        }

        private bool ColumnIsFull(int Column)
        {
            return !CellEmpty(Column, Rows - 1);
        }

        private int GetFirstFreeCellInsideColumn(int Column)
        {
            for (var a = 0; a < Rows; a++)
            {
                if (CellEmpty(Column, a))
                    return a;
            }

            return -1;
        }

        private bool CellEmpty(int X, int Y)
        {
            return GetChipColor(X, Y) == ChipColor.None;
        }

        public int TakeChip(int Column, ChipColor Color)
        {
            int row;
            for (row = 0; row < Rows; row++)
            {
                if (!CellEmpty(Column, row))
                    continue;

                SetChip(Column, row, Color);
                break;
            }
            return row;
        }

        public bool VictoryFound(int Column)
        {
            if (ColumnIsEmpty(Column))
                return false;

            var y = ColumnIsFull(Column) ? Rows - 1 : GetFirstFreeCellInsideColumn(Column) - 1;

            var color = GetChipColor(Column, y);
            var len = 0; // sequence length

            for (var a = Column - 4; a < Column + 4; a++) // horz
            {
                if (a >= Columns)
                    break;
                if (a < 0)
                    continue;
                len = GetChipColor(a, y) == color ? len + 1 : 0;
                if (len == 4)
                    return true;
            }

            len = 0;
            for (var a = y - 4; a < y + 4; a++) // vert
            {
                if (a == Rows)
                    break;
                if (a < 0)
                    continue;
                len = GetChipColor(Column, a) == color ? len + 1 : 0;
                if (len == 4)
                    return true;
            }
            len = 0;
            var leftMargin = Column - 4;
            var rightMargin = Column + 4;

            for (var a = rightMargin; a > leftMargin; a--) // asc, ltr
            {
                var ypos = y + (a - Column);
                if (a > Columns - 1 || ypos > Rows - 1)
                    continue;
                if (a < 0 || ypos < 0)
                    break;
                len = GetChipColor(a, ypos) == color ? len + 1 : 0;
                if (len == 4)
                    return true;
            }
            len = 0;
            for (var a = leftMargin; a < rightMargin; a++) // desc, ltr
            {
                var ypos = y - a + Column;
                if (a < 0 || ypos < 0)
                    continue;
                if (a > Columns - 1 || ypos > Rows - 1)
                    break;
                len = GetChipColor(a, ypos) == color ? len + 1 : 0;
                if (len == 4)
                    return true;
            }
            return false;
        }

        public class BoardBuilder
        {
            public int Rows { get; set; }
            public int Columns { get; set; }
        }
    }
}