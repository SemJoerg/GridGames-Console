using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe
{
    struct Player
    {
        public readonly bool emptyPlayer;
        public int Winns { get; set; }
        public string Name { get; set; }
        public byte GridId { get; set; }
        public char Marker { get; set; }

        public Player(string name, byte gridId, char marker)
        {
            emptyPlayer = false;
            Winns = 0;
            Name = name;
            GridId = gridId;
            Marker = marker;
        }

        public Player(int gridId, char marker)
        {
            emptyPlayer = true;
            Winns = 0;
            Name = null;
            GridId = 0;
            Marker = marker;
        }

        static public Player GetPlayerByGridId(Player[] players, int gridId)
        {
            foreach(Player player in players)
            {
                if(player.GridId == gridId)
                {
                    return player;
                }
            }

            return new Player(0, ' ');
        }
    }
}
