using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    class Game
    {
        private Grid grid;
        public int WinnLineLength { get; private set; }
        public Player[] Players { get; private set; }
        public Player CurrentPlayer { get; private set; }
        private int currentPlayerIndex = 0;

        public delegate void DetectedWinnerHandler(Game sender, Player winner, int[] winningFields);

        public event DetectedWinnerHandler DetectedWinner;

        public Game(int width, int height, int winnLineLength,Player[] players, byte defaultFieldValue = default(byte))
        {
            grid = new Grid(width, height, defaultFieldValue);
            WinnLineLength = winnLineLength;
            Players = players;
            grid.FieldChangedEvent += GridChanged;
            //grid.FieldChangedEvent += GridChangedDebug;
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
            PrintGrid();
            List<int>[] lanes = senderGrid.GetAllLanes(fieldIndex);
            
            foreach(List<int> lane in lanes)
            {
                if (lane.Count < WinnLineLength)
                    continue;
                int currenntIndex = 0;
                int[] winningFields = new int[WinnLineLength];
                foreach(int gridIndex in lane)
                {
                    if(winningFields[currenntIndex] == grid[gridIndex] && grid[gridIndex] != grid.DefaultFieldValue)
                    {
                        currenntIndex++;
                        winningFields[currenntIndex] = grid[gridIndex];
                        if (currenntIndex + 1 >= WinnLineLength)
                        {
                            Player winner = Player.GetPlayerByGridId(Players, grid[gridIndex]);
                            DetectedWinner?.Invoke(this, winner, winningFields);
                            break;
                        }
                    }
                    else
                    {
                        currenntIndex = 0;
                        winningFields = new int[WinnLineLength];
                        winningFields[currenntIndex] = grid[gridIndex];
                    }
                }
            }
        }

        public void ResetGame()
        {
            grid.ClearGrid();
            currentPlayerIndex = 0;
        }
        
        public bool SetField(int fieldIndex)
        {
            CurrentPlayer = Players[currentPlayerIndex];

            if (fieldIndex >= grid.GridArray.Length || fieldIndex < 0 || grid[fieldIndex] != grid.DefaultFieldValue)
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
            Console.Clear();
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
