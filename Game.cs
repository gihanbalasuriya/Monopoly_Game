using System;
using System.Collections.Generic;
using System.Text;

namespace Monopoly
{


    public abstract class Game: GameInterface
    {
        private int playersCount;
    
        public abstract void initializeGame();
        public abstract void makePlay(int player);
        public abstract bool endOfGame();
        public abstract void printWinner();

        public void playOneGame(int playersCount)
        {
            
        }
    }
}
