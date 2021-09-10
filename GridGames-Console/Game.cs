using System;
using System.Collections.Generic;
using System.Text;

namespace GridGamesConsole
{
    class Game
    {
        private Grid grid;
        public int GridWidth { get { return grid.Width; } }
        public int GridHeight { get { return grid.Height; } }
        private int winnLineLength;
        public int WinnLineLength 
        {
            get { return winnLineLength; }
            set
            {
                if(value < 1)
                {
                    winnLineLength = 1;
                }
                else
                {
                    winnLineLength = value;
                }
            }
        }
        public Player[] Players { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public Player NextPlayer { get; private set; }
        private int currentPlayerIndex = 1;

        public delegate void OutputGrid(char[] markerGrid, int gridWidth, int gridHeight, int? changedFieldIndex = null, char? changedFieldMarker = null);
        private OutputGrid outputGrid;
        public delegate void DetectedWinnerHandler(Game sender, Player winner, int[] winningFields);
        public event DetectedWinnerHandler DetectedWinner;

        public Game(int width, int height, int _winnLineLength, Player[] players, Player emptyPlayer, OutputGrid _outputGrid, Grid.GridIsFullHandler gridIsFull)
        {
            grid = new Grid(width, height, emptyPlayer.GridId);
            WinnLineLength = _winnLineLength;
            
            Players = new Player[players.Length + 1];
            Players[0] = emptyPlayer;
            for(int i = 1; i < Players.Length; i++)
            {
                Players[i] = players[i - 1];
            }
            NextPlayer = Players[currentPlayerIndex];

            outputGrid = _outputGrid;
            grid.FieldChangedEvent += GridChangedDebug;
            grid.FieldChangedEvent += GridFieldChanged;
            grid.GridIsFullEvent += gridIsFull;
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
            Console.ReadKey();
        }
        
        private void GridFieldChanged(Grid senderGrid, int fieldIndex)
        {
            DoGridOutput(fieldIndex);
            List<int>[] lanes = senderGrid.GetAllLanes(fieldIndex);
            
            foreach(List<int> lane in lanes)
            {
                if (lane.Count < WinnLineLength)
                    continue;
                int currenntIndex = 0;
                int[] winningFields = new int[WinnLineLength];
                Player winner;
                foreach(int gridIndex in lane)
                {
                    winner = Player.GetPlayerByGridId(Players, grid[gridIndex]);
                    if (winningFields[currenntIndex] == grid[gridIndex] && grid[gridIndex] != grid.DefaultFieldValue)
                    {
                        currenntIndex++;
                        winningFields[currenntIndex] = grid[gridIndex];
                        if (currenntIndex + 1 >= WinnLineLength)
                        {
                            DetectedWinner?.Invoke(this, winner, winningFields);
                            break;
                        }
                    }
                    else
                    {
                        currenntIndex = 0;
                        winningFields = new int[WinnLineLength];
                        winningFields[currenntIndex] = grid[gridIndex];

                        if (winningFields[currenntIndex] != grid.DefaultFieldValue && currenntIndex + 1 >= WinnLineLength)
                        {
                            DetectedWinner?.Invoke(this, winner, winningFields);
                            break;
                        }
                    }
                }
            }
        }

        private char[] GetConvertedGridArray()
        {
            char[] convertedGridArray = new char[grid.GridArray.Length];
            for(int i = 0; i < grid.GridArray.Length; i++)
            {
                convertedGridArray[i] = Player.GetPlayerByGridId(Players, grid.GridArray[i]).Marker;
            }
            return convertedGridArray;
        }

        public void DoGridOutput(int? changedFieldIndex = null)
        {
            char? changedFieldMarker = null;
            if(changedFieldIndex != null)
            {
                changedFieldMarker = Player.GetPlayerByGridId(Players, changedFieldIndex ?? Players[0].GridId).Marker;
            }

            outputGrid(GetConvertedGridArray(), grid.Width, grid.Height, changedFieldIndex, changedFieldMarker);
        }

        public void ResetGame()
        {
            grid.ClearGrid();
            currentPlayerIndex = 1;
            NextPlayer = Players[currentPlayerIndex];
            DoGridOutput();
        }
        
        public bool SetNextField(int fieldIndex)
        {
            CurrentPlayer = Players[currentPlayerIndex];

            if (fieldIndex >= grid.GridArray.Length || fieldIndex < 0 || grid[fieldIndex] != grid.DefaultFieldValue)
                return false;

            //Next Player has to be dertimined that it is updated when the gridChanged Event gets called
            if (currentPlayerIndex >= Players.Length - 1)
            {
                currentPlayerIndex = 1;
            }
            else
            {
                currentPlayerIndex++;
            }
            NextPlayer = Players[currentPlayerIndex];

            grid[fieldIndex] = CurrentPlayer.GridId;
            return true;
        }
        
    }
}
