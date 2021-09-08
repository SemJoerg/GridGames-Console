using System;
using System.Collections.Generic;

namespace TicTacToe
{
    class Program
    {
        static char[] lineLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        static Game game;
        static void Main(string[] args)
        {
            //Game game = new Game(10, 10, 3, players, new Player(0, '#'), PrintGrid);
            MainMenu();
            game.DetectedWinner += OnWinnerDetected;

            game.DoGridOutput();
            while (true)
            {
                bool correctInput = game.SetNextField(GetIntInput());

                if(!correctInput)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Input");
                    Console.ResetColor();
                }    
            }
        }

        static void MainMenu()
        {
            int input = -1;
            List<Player> players = new List<Player>();
            int winnLineLength = 0;
            int gridWidth = 0;
            int gridHeight = 0;
            bool exitMainMenue = false;
            while (exitMainMenue == false)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("[1] Start game");
                    Console.WriteLine("[2] Set winn line length");
                    Console.WriteLine("[3] Set gridsize");
                    Console.WriteLine("[4] Add player");
                    Console.WriteLine("[5] Remove all Players");
                    input = Convert.ToInt32(Console.ReadLine());
                    switch (input)
                    {
                        case 1:
                            Console.ForegroundColor = ConsoleColor.Red;
                            if (gridHeight == 0 || gridWidth == 0)
                                Console.WriteLine("You need to set a gridsize");
                            else if (players.Count < 2)
                                Console.WriteLine("Not enough Players");
                            else if (winnLineLength == 0)
                                Console.WriteLine("You have to set a winn line length");
                            else
                            {
                                game = new Game(gridWidth, gridHeight, winnLineLength, players.ToArray(), new Player(0, '#'), PrintGrid);
                                exitMainMenue = true;
                            }
                            Console.ResetColor();
                            break;
                        case 2:
                            Console.Write("\nWinnLineLength: ");
                            winnLineLength = Convert.ToInt32(Console.ReadLine());
                            break;
                        case 3:
                            Console.Write("\nGridWidth: ");
                            gridWidth = Convert.ToInt32(Console.ReadLine());
                            Console.Write("\nGridHeight: ");
                            gridHeight = Convert.ToInt32(Console.ReadLine());
                            if (gridHeight > 30 || gridWidth > 30)
                            {
                                gridWidth = 0;
                                gridHeight = 0;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Gridsize can not be longer than 30");
                                Console.ResetColor();
                            }
                            break;
                        case 4:
                            if (players.Count > 5)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("You can only add 5 players");
                                Console.ResetColor();
                                break;
                            }
                            Console.Write("\nName: ");
                            string name = Console.ReadLine();
                            Console.Write("\nMarker: ");
                            char marker = Convert.ToChar(Console.ReadLine());
                            byte playerGridId = (byte)(players.Count + 1);
                            players.Add(new Player(name, playerGridId, marker));
                            break;
                        case 5:
                            players.Clear();
                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid Option");
                            Console.ResetColor();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
                Console.WriteLine("Press any Key to continue...");
                Console.ReadKey();
            }
        }

        static int GetIntInput()
        {
            while (true)
            {
                try
                {
                    Console.Write("\nFieldAddress: ");
                    int letterLength = 0;
                    int rowCharIndex = -1;
                    int colum = -1;
                    string rawInput = Console.ReadLine();
                    for(int i = 0; i < rawInput.Length; i++)
                    {
                        if(char.IsLetter(rawInput[i]))
                        {
                            if(letterLength == 0)
                            {
                                rowCharIndex = i;
                            }
                            else if(rawInput[i] != rawInput[rowCharIndex])
                            {
                                throw new Exception("Incorrect row letter");
                            }
                            letterLength++;
                        }
                        else if(rowCharIndex != -1)
                        {
                            colum = Convert.ToInt32(rawInput.Substring(rowCharIndex + 1));
                        }
                        else
                        {
                            throw new Exception("Invalid FieldAddress");
                        }
                    }
                    
                    int row = 0;
                    for(int i = 0; i < lineLetters.Length; i++)
                    {
                        if(rawInput[rowCharIndex] == lineLetters[i])
                        {
                            row = i;
                        }
                    }
                    
                    return (row * letterLength * (game.GridWidth)) + colum - 1;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
        }

        static void OnWinnerDetected(Game sender, Player winner, int[] winningFields)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"{winner.Name} has won!");
            Console.WriteLine("Press any Key to continue...");
            Console.ResetColor();
            Console.ReadKey();
            Console.Clear();
            sender.ResetGame();
        }

        static void PrintGrid(char[] markerGrid, int gridWidth, int gridHeight, int? changedFieldIndex = null, char? changedFieldMarker = null)
        {
            Console.Clear();
            string spaceSpacer = "";
            string underlineSpacer = "";
            for (int i = 0; i < gridWidth.ToString().Length; i++)
            {
                spaceSpacer += " ";
                underlineSpacer += "_";
            }
            
            int tempLineLengthIndex = 0;
            int lineIndex = 0;
            int charAmmount = 1;
            Console.Write("    ");
            for(int i = 1; i <= gridWidth; i++)
            {
                string numberSpacer = "";
                for (int ii = 0; ii < spaceSpacer.Length - i.ToString().Length + 1; ii++)
                    numberSpacer += " ";
                
                Console.Write(i + numberSpacer);
            }
            Console.WriteLine();
            Console.Write("____");
            for (int i = 1; i <= gridWidth; i++)
            {
                Console.Write("_" + underlineSpacer);
            }

            Console.WriteLine();
            Console.Write(lineLetters[0] + "  | ");
            foreach (char field in markerGrid)
            {
                Console.Write(field);
                tempLineLengthIndex++;
                if (tempLineLengthIndex >= gridWidth)
                {   
                    lineIndex++;
                    tempLineLengthIndex = 0;
                    Console.WriteLine();
                    if (lineIndex + (lineLetters.Length * (charAmmount-1)) < markerGrid.Length / gridWidth)
                    {
                        if (lineIndex >= lineLetters.Length)
                        {
                            charAmmount++;
                            lineIndex = 0;
                        }
                        for (int i = 0; i < charAmmount; i++)
                            Console.Write(lineLetters[lineIndex]);

                        for (int iii = 0; iii < 3 - charAmmount; iii++)
                            Console.Write(" ");
                        Console.Write("| ");
                    }
                }
                else
                {
                   Console.Write(spaceSpacer);
                }
            }
            Console.WriteLine($"\nnext player: {game.NextPlayer.Name} [{game.NextPlayer.Marker}]");
        }
    }
}
