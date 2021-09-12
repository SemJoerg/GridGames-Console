using System;
using System.Collections.Generic;

namespace GridGamesConsole
{
    class Program
    {
        static char[] lineLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        static Game<char> game;//game gets initialized in MainMenu()
        static void Main(string[] args)
        {
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
            List<Player<char>> players = new List<Player<char>>();
            uint winnLineLength = 0;
            uint gridWidth = 0;
            uint gridHeight = 0;
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
                    Console.WriteLine("[5] Remove all players");
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
                                game = new Game<char>(gridWidth, gridHeight, winnLineLength, players.ToArray(), new Player<char>(0, '#'), PrintGrid, OnGridIsFull);
                                exitMainMenue = true;
                            }
                            Console.ResetColor();
                            break;
                        case 2:
                            Console.Write("\nWinnLineLength: ");
                            winnLineLength = Convert.ToUInt32(Console.ReadLine());
                            break;
                        case 3:
                            Console.Write("\nGridWidth: ");
                            gridWidth = Convert.ToUInt32(Console.ReadLine());
                            Console.Write("\nGridHeight: ");
                            gridHeight = Convert.ToUInt32(Console.ReadLine());
                            if (gridHeight > 30 || gridHeight < -30 || gridWidth > 30 || gridWidth < -30)
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
                            players.Add(new Player<char>(name, playerGridId, marker));
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

        static uint GetIntInput()
        {
            while (true)
            {
                try
                {
                    Console.Write("\nFieldAddress: ");
                    int letterLength = 0; //Amount of same letters chained together (example: A = 1, AA = 2, AAA = 3)
                    int rowCharIndex = -1; //The index in the lineLetter array
                    int row = 0;
                    int colum = -1;
                    string rawInput = Console.ReadLine();

                    for(int i = 0; i < rawInput.Length; i++)
                    {
                        //Get rowCharIndex
                        if (char.IsLetter(rawInput[i]))
                        {
                            if(letterLength == 0)
                            {
                                rowCharIndex = i;
                            }
                            //throws an exception if the chained Letters are not equal
                            else if (rawInput[i] != rawInput[rowCharIndex])
                            {
                                throw new Exception("Incorrect row letter");
                            }
                            letterLength++;
                        }
                        //Get Colum after Row is set
                        else if (rowCharIndex != -1)
                        {
                            colum = Convert.ToInt32(rawInput.Substring(rowCharIndex + letterLength));
                        }
                        else
                        {
                            throw new Exception("Field address has to start with a letter");
                        }
                    }
                    
                    //Get Row
                    for(int i = 0; i < lineLetters.Length; i++)
                    {
                        if(rawInput.ToUpper()[rowCharIndex] == lineLetters[i])
                        {
                            row = i;
                        }
                    }

                    if (letterLength > 1)
                        row += lineLetters.Length * (letterLength - 1);

                    return (uint)((row * game.GridWidth) + colum - 1);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
        }

        static void OnWinnerDetected(Game<char> sender, Player<char> winner, uint[] winningFields)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"{winner.Name} has won!");
            Console.WriteLine("Press any Key to continue...");
            Console.ResetColor();
            Console.ReadKey();
            Console.Clear();
            sender.ResetGame();
        }

        static void OnGridIsFull(Grid sender)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"Nobody wonn it is a tie!!!");
            Console.WriteLine("Press any Key to continue...");
            Console.ResetColor();
            Console.ReadKey();
            Console.Clear();
            game.ResetGame();
        }

        static void PrintGrid(char[] markerGrid, uint gridWidth, uint gridHeight, char changedFieldMarker, uint? changedFieldIndex = null)
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
            Console.Write("     ");
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
