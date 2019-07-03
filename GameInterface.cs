namespace Monopoly
{
    interface GameInterface
    {
        void initializeGame();
        void makePlay(int player);
        bool endOfGame();
        void printWinner();
        void playOneGame(int playersCount);
     }
}
