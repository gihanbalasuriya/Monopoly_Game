using System;
using System.Collections;

namespace Monopoly
{

    [Serializable]
    public class Player : Trader
    {
        private int location;
        private int lastMove;

        public bool HasChanceFreeCard { get; set; }         
        public bool HasChestFreeCard { get; set; }          
        public bool IsInJail { get; set; }           
        public int DoubleCount { get; set; }          
        public int FailureRollDouble { get; set; }

        Dice dice1 = new Dice();
        Dice dice2 = new Dice();
        bool isInactive = false;

        public event EventHandler playerBankrupt;
        public event EventHandler playerPassGo;
        public event EventHandler playerLandOnChance;
        public event EventHandler playerGoJail;

        public Player()
        {
            this.sName = "Player";
            
            this.dBalance = Board.access().PlayerInitialBalance;
            this.location = 0;
            HasChanceFreeCard = false;
            HasChestFreeCard = false;
            IsInJail = false;
            DoubleCount = 0;
            FailureRollDouble = 0;
        }

        public Player(string sName)
        {
            this.sName = sName;
            this.dBalance = Board.access().PlayerInitialBalance;
            
            HasChestFreeCard = false;
            IsInJail = false;
            DoubleCount = 0;
            FailureRollDouble = 0;

        }


        public Player(string sName, decimal dBalance) : base(sName, dBalance)
        {
            this.location = 0;
            HasChanceFreeCard = false;
            HasChestFreeCard = false;
            IsInJail = false;
            DoubleCount = 0;
            FailureRollDouble = 0;

        }

        public void move()
        {

            int iMoveDistance = dice1.roll() + dice2.roll();
            Console.WriteLine($"{this.getName()}:    Rolling Dice:  Dice1:[{dice1}] Dice2:[{dice2}]");

            if (dice1.numberLastRolled()==dice2.numberLastRolled())
            {
                if (++this.DoubleCount>=3 && playerGoJail != null)
                {
                    Console.WriteLine($"{this.getName()}:    You have rolled double thrice and will be sent to Jail!");
                    this.playerGoJail(this,new EventArgs());
                    return;
                }
            }
            else
            {
                this.DoubleCount = 0;
            }

            this.setLocation(this.getLocation() + iMoveDistance);
            this.lastMove = iMoveDistance;
        }

        public int getLastMove()
        {
            return this.lastMove;
        }

        public string BriefDetailsToString()
        {
            return String.Format("You are on {0}.\tYou have ${1:N2}.", Board.access().getProperty(this.getLocation()).getName(), this.getBalance());
        }

        public override string ToString()
        {
            return this.getName();
        }

        public string FullDetailsToString()
        {
            return String.Format("Player:{0,-12}    Balance: ${1:N2}\nLocation:\n{2} (Square {3}) \n--------------------------------------------------------------------------------------------\nProperties Owned:\n{4}", this.getName(), this.getBalance(), Board.access().getProperty(this.getLocation()), this.getLocation(), this.PropertiesOwnedToString());
        }

        public string PropertiesOwnedToString()
        {
            string owned = "";

            if (getPropertiesOwnedFromBoard().Count == 0)
                return "None";

            for (int i = 0; i < getPropertiesOwnedFromBoard().Count; i++)
            {
                owned += getPropertiesOwnedFromBoard()[i].ToString() + "\n";
            }
            return owned;
        }

        public void setLocation(int location)
        {

            if (location >= Board.access().getSquares())
            {
                location = (location - Board.access().getSquares());
                if (playerPassGo != null)
                    this.playerPassGo(this, new EventArgs());
              
            }

            this.location = location;
            Property p = Board.access().getProperty(location);
            if (p.getName() == "Chance" || p.getName() == "Community Chest")
            {
                if (playerLandOnChance != null)
                {
                    this.playerLandOnChance(this, new EventArgs());

                }
            }
            if ("Go To Jail" == p.getName() && playerGoJail != null)
            {
                Console.WriteLine($@"{this.getName()}:  You landed on 'Go To Jail' and will be sent to Jail!!! ");
                this.playerGoJail(this,new EventArgs());
            }
        }

        public int getLocation()
        {
            return this.location;
        }

        public string diceRollingToString()
        {
            return String.Format("Rolling Dice:\tDice 1: {0}\tDice 2: {1}", dice1, dice2);
        }

        public ArrayList getPropertiesOwnedFromBoard()
        {
            ArrayList propertiesOwned = new ArrayList();
            for (int i = 0; i < Board.access().getProperties().Count; i++)
            {
                if (Board.access().getProperty(i).getOwner() == this)
                {
                    propertiesOwned.Add(Board.access().getProperty(i));
                }
            }
            return propertiesOwned;
        }

        public override void checkBankrupt()
        {
            if (this.getBalance() <= 0)
            {
                if (playerBankrupt != null)
                    this.playerBankrupt(this, new EventArgs());

                Banker b = Banker.access();
                foreach (Property p in this.getPropertiesOwnedFromBoard())
                {
                    p.setOwner(b);
                }
                this.isInactive = true;

                int activePlayerCount = 0;
                Player winner = new Player();
                foreach (Player p in Board.access().getPlayers())
                {
                    if (!p.isNotActive())
                    {
                        activePlayerCount++;
                        winner = p;
                    }
                }

                if (activePlayerCount < 2)
                {
                    Console.WriteLine("\n\n>>>>>>>>>{0} has won the game!<<<<<<<<<<\n\n", winner.getName());
                    Console.WriteLine(">>>>>>>>>>The game is now over. Please press enter to exit.<<<<<<<<<");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }

        public bool isNotActive()
        {
            return this.isInactive;
        }

    }
}
