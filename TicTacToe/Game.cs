using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    class Game
    {
        private Grid grid;
        public Player[] Players { get; private set; }
        public Player CurrentPlayer { get; private set; }
        private int currentPlayerIndex = 0;

        public Game(int width, int height, Player[] players, byte defaultFieldValue = default(byte))
        {
            grid = new Grid(width, height, defaultFieldValue);
            Players = players;
            grid.FieldChangedEvent += GridChanged;
            grid.FieldChangedEvent += GridChangedDebug;
        }

        private void GridChangedDebug(Grid senderGrid, int fieldIndex)
        {
            List<int>[] lanes = senderGrid.GetAllLanes(fieldIndex);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Lanes: ");
            foreach(List<int> lane in lanes)
            {
                foreach(int item in lane)
                {
                    Console.Write(item);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }
        
        private void GridChanged(Grid senderGrid, int fieldIndex)
        {
            List<int>[] lanes = senderGrid.GetAllLanes(fieldIndex);
            Console.ForegroundColor = ConsoleColor.Green;
            foreach(List<int> lane in lanes)
            {
                foreach(int item in lane)
                {
                    //
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        public bool SetField(int fieldIndex)
        {
            CurrentPlayer = Players[currentPlayerIndex];

            if (grid[fieldIndex] != grid.DefaultFieldValue)
                return false;

            grid[fieldIndex] = CurrentPlayer.GridId;

            if(currentPlayerIndex >= Players.Length - 1)
            {
                currentPlayerIndex = 0;
            }
            else
            {
                currentPlayerIndex++;
            }
            return true;
        }

        public void PrintGrid()
        {
            int tempLineIndex = 0;
            foreach(byte field in grid.GridArray)
            {
                Console.Write(field);
                tempLineIndex++;
                if(tempLineIndex >= grid.Width)
                {
                    Console.WriteLine();
                    tempLineIndex = 0;
                }
                else
                {
                    Console.Write("  ");
                }
            }
        }
    }
}
