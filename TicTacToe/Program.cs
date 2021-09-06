using System;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            Player[] players = new Player[2];
            players[0] = new Player("Player 1", 1, 'X');
            players[1] = new Player("Player 2", 2, 'O');
            Game game = new Game(10, 10, 4, players);
            game.DetectedWinner += OnWinnerDetected;

            game.PrintGrid();
            while (true)
            {
                bool correctInput = game.SetField(GetIntInput());

                if(!correctInput)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Input");
                    Console.ResetColor();
                }    
            }
        }

        private static void OnWinnerDetected(Game sender, Player winner, int[] winningFields)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"{winner.Name} has won!");
            Console.WriteLine("Press any Key to continue...");
            Console.ResetColor();
            Console.ReadKey();
            Console.Clear();
            sender.ResetGame();
            sender.PrintGrid();
        }

        static int GetIntInput()
        {
            while(true)
            {
                try
                {
                    return Convert.ToInt32(Console.ReadLine());
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
        }

        
    }
}
