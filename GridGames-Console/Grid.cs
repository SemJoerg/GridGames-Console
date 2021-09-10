using System;
using System.Collections.Generic;
using System.Text;

namespace GridGamesConsole
{
    public class Grid
    {
        private int width;
        public int Width { get { return width; } private set { width = GetPositiveNumber(value); } }
        private int height;
        public int Height { get { return height; } private set { height = GetPositiveNumber(value); } }
        public byte DefaultFieldValue { get; private set; }
        private byte[] gridArray;
        public byte[] GridArray
        {
            get { return gridArray; }
            private set { gridArray = value; }
        }

        public byte this[int index]
        {
            get
            {
                return gridArray[index];
            }

            set
            {
                gridArray[index] = value;
                FieldChangedEvent?.Invoke(this, index);
                foreach (byte item in gridArray)
                {
                    if (item == DefaultFieldValue)
                        return;
                }

                GridIsFullEvent?.Invoke(this);
            }
        }

        public delegate void FieldChangedHandler(Grid senderGrid, int fieldIndex);
        public delegate void GridIsFullHandler(Grid senderGrid);
        public delegate int? GetField(int fieldIndex);

        public event FieldChangedHandler FieldChangedEvent; //Event wont be triggered through ClearGrid()
        public event GridIsFullHandler GridIsFullEvent;

        public Grid(int width, int height, byte defaultFieldValue = default(byte))
        {
            Width = width;
            Height = height;
            DefaultFieldValue = defaultFieldValue;
            GridArray = new byte[width * height];
            ClearGrid();
        }

        private int GetPositiveNumber(int value)
        {
            if(value < 0)
            {
                return -value;
            }
            if(value == 0)
            {
                return 1;
            }
            return value;
        }

        public void ClearGrid()
        {
            for(int i = 0; i < GridArray.Length; i++)
            {
                gridArray[i] = DefaultFieldValue;
            }
        }

        public int? GetUpperField(int fieldIndex)
        {
            int newIndex = fieldIndex - Width;
            if(newIndex < GridArray.Length && newIndex >= 0)
            {
                return newIndex;
            }
            return null;
        }
        
        public int? GetLowerField(int fieldIndex)
        {
            int newIndex = fieldIndex + Width;
            if(newIndex < GridArray.Length && newIndex >= 0)
            {
                return newIndex;
            }
            return null;
        }

        public int? GetLeftField(int fieldIndex)
        {
            int newIndex = fieldIndex - 1;
            if (newIndex < GridArray.Length && newIndex >= 0)
            {
                int tempIndex = Width - 1;
                while (true)
                {
                    if (fieldIndex <= tempIndex)
                    {
                        break;
                    }
                    tempIndex += Width;
                }
                if(newIndex > tempIndex - Width && newIndex <= tempIndex)
                {
                    return newIndex;
                }
            }
            return null;
        }

        public int? GetRightField(int fieldIndex)
        {
            int newIndex = fieldIndex + 1;
            if (newIndex < GridArray.Length && newIndex >= 0)
            {
                int tempIndex = Width - 1;
                while (true)
                {
                    if (fieldIndex <= tempIndex)
                    {
                        break;
                    }
                    tempIndex += Width;
                }
                if (newIndex >= tempIndex - Width && newIndex <= tempIndex)
                {
                    return newIndex;
                }
            }
            return null;
        }

        private int? GetCombindedField(int fieldIndex, GetField firstField, GetField secondField)
        {
            int? firstFieldIndex = firstField(fieldIndex);
            int? secondFieldIndex = null;
            if (firstFieldIndex != null)
                secondFieldIndex = secondField(firstFieldIndex ?? default(int));

            if (secondFieldIndex != null)
                return secondFieldIndex;

            return null;
        }

        public int? GetUpperLeftField(int fieldIndex)
        {
            return GetCombindedField(fieldIndex, GetUpperField, GetLeftField);
        }

        public int? GetUpperRightField(int fieldIndex)
        {
            return GetCombindedField(fieldIndex, GetUpperField, GetRightField);
        }

        public int? GetLowerLeftField(int fieldIndex)
        {
            return GetCombindedField(fieldIndex, GetLowerField, GetLeftField);
        }

        public int? GetLowerRightField(int fieldIndex)
        {
            return GetCombindedField(fieldIndex, GetLowerField, GetRightField);
        }
        

        public List<int> GetLine(int fieldIndex, GetField firstField, GetField secondField)
        {
            int? tempFieldIndex = fieldIndex;
            int movingIndex = fieldIndex;
            List<int> lane = new List<int>();
            while(true)
            {
                tempFieldIndex = firstField(movingIndex);
                if (tempFieldIndex == null)
                    break;
                movingIndex = tempFieldIndex ?? default(int);
                lane.Insert(0, movingIndex);
            }
            lane.Add(fieldIndex);
            movingIndex = fieldIndex;
            while(true)
            {
                tempFieldIndex = secondField(movingIndex);
                if (tempFieldIndex == null)
                    break;
                movingIndex = tempFieldIndex ?? default(int);
                lane.Add(movingIndex);
            }

            return lane;
        }

        public List<int>[] GetAllLanes(int fieldIndex)
        {
            List<int>[] lanes = new List<int>[4];

            lanes[0] = GetLine(fieldIndex, GetLeftField, GetRightField);
            lanes[1] = GetLine(fieldIndex, GetUpperField, GetLowerField);
            lanes[2] = GetLine(fieldIndex, GetLowerLeftField, GetUpperRightField);
            lanes[3] = GetLine(fieldIndex, GetUpperLeftField, GetLowerRightField);

            return lanes;
        }
    }
}
