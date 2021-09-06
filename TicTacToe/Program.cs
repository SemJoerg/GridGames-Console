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
            Game game = new Game(10, 5, players);
            

            while(true)
            {
                game.PrintGrid();
                game.SetField(GetIntInput());
            }
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
