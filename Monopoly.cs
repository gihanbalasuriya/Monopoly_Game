using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Monopoly
{
    public class Monopoly : Game
    {
        ConsoleColor[] colors = new ConsoleColor[8] { ConsoleColor.Cyan, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Red, ConsoleColor.Magenta, ConsoleColor.Gray, ConsoleColor.Blue, ConsoleColor.DarkYellow};

        public override void initializeGame()
        {
            displayMainChoiceMenu();

        }

        public override void makePlay(int iPlayerIndex)
        {
            
            Board.access().CurrentPlayer = iPlayerIndex;
            
            Player player = Board.access().getPlayer(iPlayerIndex);
            
            Console.ForegroundColor = this.colors[iPlayerIndex];

            if (player.isNotActive())
            {
                Console.WriteLine("\n>>>>>>>>>>{0} is inactive.<<<<<<<<<\n", player.getName());

                int activePlayerCount = 0;
                foreach (Player p in Board.access().getPlayers())
                {
                    if (!p.isNotActive())
                        activePlayerCount++;
                }

                if (activePlayerCount < 2)
                {
                    this.printWinner();
                }
               
                return;
            }


            if (player.IsInJail)
            {
                this.displayMenuInJail(iPlayerIndex);
            }
            else
            {
                Console.WriteLine("------------------------------------------");
                Console.WriteLine("{0}Your turn. Press Enter to make move==>", playerPrompt(iPlayerIndex));
                Console.ReadLine();

                player.move();
                if (player.IsInJail)
                {
                    Console.WriteLine($">>>>>>>>>>{player.getName()}: You are in jail now, your turn ends!!!<<<<<<<<<<");
                    Console.WriteLine("Press any key to continue!!!==>");
                    Console.ReadLine();
                    return;
                }
                Property propertyLandedOn = Board.access().getProperty(player.getLocation());
                Console.WriteLine(">>>>>>>>>>"+propertyLandedOn.landOn(ref player)+"<<<<<<<<<<");
                Console.WriteLine("\n{0}{1}", playerPrompt(iPlayerIndex), player.BriefDetailsToString());
                displayPlayerChoiceMenu(player);
                if (player.DoubleCount>0)
                {
                    Console.WriteLine($">>>>>>>>>>{player.getName()}: You rolled double last time so you can roll a second time!<<<<<<<<<<");
                    this.makePlay(iPlayerIndex);
                }
            }
            
        }

        public override bool endOfGame()
        {
            Console.WriteLine(">>>>>>>>>>The game is now over. Please press enter to exit.<<<<<<<<<");
            Console.ReadLine();

            Environment.Exit(0);
            return true;
        }

        public override void printWinner()
        {
            Player winner = null;
            foreach (Player p in Board.access().getPlayers())
            {
                if (!p.isNotActive())
                    winner = p;
            }
            Console.WriteLine("\n\n>>>>>>>>>{0} has won the game!<<<<<<<<<<\n\n" , winner.getName());
            this.endOfGame();
        }

        public void displayMainChoiceMenu()
        {
            int resp = 0;
            Console.WriteLine("                 ========================================");
            Console.WriteLine("                 Please make a selection:\n");
            Console.WriteLine("                 ---------------------------------------");
            Console.WriteLine("                 1. Start New Game");
            Console.WriteLine("                 2. Load Game");
            Console.WriteLine("                 3. Quit Game");
            Console.WriteLine("                 ========================================");
            Console.Write("                 (1-3)>");

            resp = inputInteger();

            if (resp == 0)
                this.displayMainChoiceMenu();

            try
            {
                switch (resp)
                {
                    case 1:
                        this.setUpGame();
                        this.playGame();
                        break;
                    case 2:
                        this.loadGame();
                        break;
                    case 3:
                        this.quitGame();
                        break;
                    default:
                        throw new ApplicationException(">>>>>>>>>>That option is not avaliable. Please try again.<<<<<<<<<<");
                }
            }
            catch(ApplicationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public void setUpGame()
        {
          
            this.loadProperties();

            this.loadCards();
            
            this.setUpPlayers();
            
        }

        public void playGame()
        {
            while (Board.access().getPlayerCount() >= 2)
            {
                for (int i = 0; i < Board.access().getPlayerCount(); i++)
                {
                    Console.Clear();
                    this.makePlay(i);
                }
            } 
        }

        public int inputInteger()
        {
            try
            {
                return int.Parse(Console.ReadLine());
            }
            catch (FormatException ex)
            {
                Console.WriteLine(">>>>>>>>>>Please enter a number such as 1 or 2. Please try again.<<<<<<<<<<");
                return 0;
            }
        }

        public decimal inputDecimal() 
        {
            try
            {
                return decimal.Parse(Console.ReadLine());
            }
            catch (FormatException ex)
            {
                Console.WriteLine(">>>>>>>>>>Please enter a decimal number such as 25.54 or 300. Please try again.<<<<<<<<<<");
                return 0;
            }
        }

        public decimal inputDecimal(string msg)
        {
            Console.WriteLine(msg);
             Console.Write("==>");
            decimal amount = this.inputDecimal();

            if (amount == 0)
            {
                Console.WriteLine(">>>>>>>>>>That was not a valid amount. Please try again<<<<<<<<<<");
                this.inputDecimal(msg);
            }
            return amount;
        }

        public void loadProperties()
        {
            LuckFactory luckFactory = new LuckFactory();
            ResidentialFactory residentialFactory = new ResidentialFactory();
            TransportFactory transportFactory = new TransportFactory();
            UtilityFactory utilityFactory = new UtilityFactory();
            PropertyFactory propertyFactory = new PropertyFactory();
            int squarCount = 0;
            string lineBuffer = null;

            using (StreamReader initReader = new StreamReader(@"..\..\Config\Properties.ini"))
            {
                while ((lineBuffer = initReader.ReadLine()) != null)
                {
                    if (lineBuffer[0] == '#')
                    {
                        continue;
                    }
                    string[] factors = lineBuffer.Split('|');

                    switch (factors[0])
                    {
                        case "BKBAL":
                            Board.access().BankInitialBalance = decimal.Parse(factors[1]);
                            break;
                        case "PLAYERBAL":
                            Board.access().PlayerInitialBalance = decimal.Parse(factors[1]);
                            break;
                        case "PROPERTY":
                            Board.access().addProperty(propertyFactory.create(factors[1]));
                            squarCount++;
                            break;
                        case "LUCK":
                            Board.access().addProperty(luckFactory.create(factors[1], bool.Parse(factors[2]), decimal.Parse(factors[3])));
                            squarCount++;
                            break;
                        case "RESIDENTIAL":
                            Board.access().addProperty(residentialFactory.create(factors[1], decimal.Parse(factors[2]), decimal.Parse(factors[3]), decimal.Parse(factors[4]), decimal.Parse(factors[5])));
                            squarCount++;
                            break;
                        case "TRANSPORT":
                            Board.access().addProperty(transportFactory.create(factors[1]));
                            squarCount++;
                            break;
                        case "UTILITY":
                            Board.access().addProperty(utilityFactory.create(factors[1]));
                            squarCount++;
                            break;
                        case "Chance":
                        case "Community Chest":
                            Board.access().addProperty(propertyFactory.create(factors[0]));
                            squarCount++;
                            break;
                        default:
                            throw new ApplicationException($">>>>>>>>>>Read configure file Properties.ini error!!![{ lineBuffer}]<<<<<<<<<<");
                           

                    }
                }
            }
            Board.access().setSquares(squarCount);         
        }
        public void loadCards()
        {
            string lineBuffer = null;

            using (StreamReader initReader = new StreamReader(@"..\..\Config\Cards.ini"))
            {
                while ((lineBuffer = initReader.ReadLine()) != null)
                {
                    if (lineBuffer[0] == '#')
                    {
                        continue;
                    }
                    string[] factors = lineBuffer.Split('|');
                    if (factors[0] == "Chance")
                    {
                        switch (factors[1])
                        {
                            case "MOVE":
                                Board.access().insertChance(new MovingCard(factors[0], factors[1], factors[4], factors[2], int.Parse(factors[3])));
                                break;
                            case "PAY":
                            case "RCV":
                            case "COLLECT":
                            case "PAYALL":
                                Board.access().insertChance(new PaymentCard(factors[0], factors[1], factors[3], decimal.Parse(factors[2])));
                                break;
                            case "REPAIR":
                                Board.access().insertChance(new RepairCard(factors[0], factors[1], factors[4], decimal.Parse(factors[2]), decimal.Parse(factors[3])));
                                break;
                            case "FREE":
                                Board.access().insertChance(new FreeCard(factors[0], factors[1], factors[2]));
                                break;
                            default:
                                throw new ApplicationException($">>>>>>>>>>Read configure file Cards.ini error!!![{ lineBuffer}]<<<<<<<<<<<");

                        }

                    }
                    else if (factors[0] == "Community Chest")
                    {
                        switch (factors[1])
                        {
                            case "MOVE":
                                Board.access().insertChest(new MovingCard(factors[0], factors[1], factors[4], factors[2], int.Parse(factors[3])));
                                break;
                            case "PAY":
                            case "RCV":
                            case "COLLECT":
                            case "PAYALL":
                                Board.access().insertChest(new PaymentCard(factors[0], factors[1], factors[3], decimal.Parse(factors[2])));
                                break;
                            case "REPAIR":
                                Board.access().insertChest(new RepairCard(factors[0], factors[1], factors[4], decimal.Parse(factors[2]), decimal.Parse(factors[3])));
                                break;
                            case "FREE":
                                Board.access().insertChest(new FreeCard(factors[0], factors[1], factors[2]));
                                break;
                            default:
                                throw new ApplicationException($">>>>>>>>>>>Read configure file Cards.ini error!!![{ lineBuffer}]<<<<<<<<<<");
                        }
                    }
                    else
                    {
                            throw new ApplicationException($">>>>>>>>>>Read configure file Cards.ini error!!![{ lineBuffer}]<<<<<<<<<<");
                    }

                }
            }
        }

        public void setUpPlayers()
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine("How many players are playing?");
            Console.Write("(2-8)>");
            int playerCount = this.inputInteger();

            if ((playerCount < 2) || (playerCount > 8))
            {
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>That is an invalid amount. Please try again.<<<<<<<<<<");
                this.setUpPlayers();
            }

            for (int i = 0; i < playerCount; i++)
            {
                Console.WriteLine("------------------------------------");
                Console.WriteLine("Please enter the name for Player {0}:", i + 1);
                Console.Write("==>");
                string sPlayerName = Console.ReadLine();
                Player player = new Player(sPlayerName);

                player.playerBankrupt += playerBankruptHandler;
                player.playerPassGo += playerPassGoHandler;
                player.playerLandOnChance += playerLandOnChanceHandler;
                player.playerGoJail += playerGoJailHandler;

                Board.access().addPlayer(player);
                Console.WriteLine("{0} has been added to the game.", Board.access().getPlayer(i).getName());
            }

            Console.WriteLine("Players have been setup");
        }

        public string playerPrompt(int playerIndex)
        {
            return string.Format("{0}:\t", Board.access().getPlayer(playerIndex).getName());
        }

        public string playerPrompt(Player player)
        {
            return string.Format("{0}:\t", player.getName());
        }

        public bool getInputYN(Player player, string sQuestion)
        {
            Console.WriteLine(playerPrompt(player) + sQuestion);
            Console.Write("y/n=>");
            string resp = Console.ReadLine().ToUpper();

            switch (resp)
            {
                case "Y":
                    return true;
                case "N":
                    return false;
                default:
                    Console.WriteLine("--------------------------------------------------------------------------------");
                    Console.WriteLine(">>>>>>>>>>That answer cannot be understood. Please answer 'y' or 'n'.<<<<<<<<<<");
                    return this.getInputYN(player, sQuestion);
            }
        }

        public void displayPlayerChoiceMenu(Player player)
        {
            int resp = 0;
            Console.WriteLine("\n");
            Console.WriteLine("                 =======================================");
            Console.WriteLine("                  {0}Please make a selection:", playerPrompt(player));
            Console.WriteLine("                 ---------------------------------------");
            Console.WriteLine("                  1. Finish turn");
            Console.WriteLine("                  2. View your details");
            Console.WriteLine("                  3. Purchase This Property");
            Console.WriteLine("                  4. Buy A House for Property");
            Console.WriteLine("                  5. Sell A House to Bank");
            Console.WriteLine("                  6. Buy A Hotel for Property");
            Console.WriteLine("                  7. Sell A Hotel to Bank");
            Console.WriteLine("                  8. Mortgage Property");
            Console.WriteLine("                  9. Unmortgage Property");
            Console.WriteLine("                 10. Trade Property with Player");
            Console.WriteLine("                 11. Save Game");
            Console.WriteLine("                 12. Quit Game");
            Console.WriteLine("                 =======================================");
            Console.Write("                 (1-12)=>");

            resp = inputInteger();

            if (resp == 0)
                this.displayPlayerChoiceMenu(player);

            switch (resp)
            {
                case 1:
                    break;
                case 2:
                    Console.WriteLine(">>>=======================================================================================");
                    Console.WriteLine(player.FullDetailsToString());
                    Console.WriteLine("=======================================================================================<<<");
                    this.displayPlayerChoiceMenu(player);
                    break;
                case 3:
                    this.purchaseProperty(player);
                    this.displayPlayerChoiceMenu(player);
                    break;
                case 4:
                    this.buyHouse(player);
                    this.displayPlayerChoiceMenu(player);
                    break;
                case 5:
                    this.sellHouse(player);
                    this.displayPlayerChoiceMenu(player);
                    break;
                case 6:
                    this.buyHotel(player);
                    this.displayPlayerChoiceMenu(player);
                    break;
                case 7:
                    this.sellHotel(player);
                    this.displayPlayerChoiceMenu(player);
                    break;
                case 8:
                    this.mortgageProperty(player);
                    this.displayPlayerChoiceMenu(player);
                    break;
                case 9:
                    this.unmortgageProperty(player);
                    this.displayPlayerChoiceMenu(player);
                    break;
                case 10:
                    this.tradeProperty(player);
                    this.displayPlayerChoiceMenu(player);
                    break;
                case 11:
                    this.saveGame();
                    this.displayPlayerChoiceMenu(player);
                    break;
                case 12:
                    this.quitGame();
                    break;
                default:
                    Console.WriteLine(">>>>>>>>>>That option is not avaliable. Please try again.<<<<<<<<<<");
                    this.displayPlayerChoiceMenu(player);
                    break;
            }
        }

        public void purchaseProperty(Player player)
        {
            if (Board.access().getProperty(player.getLocation()).availableForPurchase())
            {
                TradableProperty propertyLocatedOn = (TradableProperty)Board.access().getProperty(player.getLocation());
                Console.WriteLine("------------------------------------------------------------");
                bool respYN = getInputYN(player, string.Format("'{0}' is available to purchase for ${1}. Are you sure you want to purchase it?", propertyLocatedOn.getName(), propertyLocatedOn.getPrice()));
                if (respYN)
                {
                    propertyLocatedOn.purchase(ref player);
                    Console.WriteLine("------------------------------------------------------------");
                    Console.WriteLine(">>>>>>>>>>{0}You have successfully purchased {1}.<<<<<<<<<<", playerPrompt(player), propertyLocatedOn.getName());
                }
            }
            else
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}{1} is not available for purchase.<<<<<<<<<<", playerPrompt(player), Board.access().getProperty(player.getLocation()).getName());
            }
        }

        public void buyHouse(Player player)
        {
            
            Console.WriteLine("------------------------------------------------------------");
            string sPrompt = String.Format("{0}Please select a property to buy a house for:", this.playerPrompt(player));

            Residential propertyToBuyFor = null;
            if (player.getPropertiesOwnedFromBoard().Count == 0)
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0} do not own any properties.<<<<<<<<<<", playerPrompt(player));

                return;
            }
            Property property = this.displayPropertyChooser(player.getPropertiesOwnedFromBoard(), sPrompt);
            
            if (property.GetType() == (new Residential().GetType()))
            {
               propertyToBuyFor = (Residential) property;
            }
            else 
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}A house cannot be bought for {1} because it is not a Residential Property.<<<<<<<<<<", this.playerPrompt(player), property.getName());
                return;
            }
            
            if (propertyToBuyFor.getHouseCount() >= Residential.getMaxHouses())
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}The maximum house limit for {1} of {2} houses has been reached.<<<<<<<<<<", playerPrompt(player), propertyToBuyFor.getName(), Residential.getMaxHouses());
            }
            else
            {
                Console.WriteLine("------------------------------------------------------------");
                bool doBuyHouse = this.getInputYN(player, String.Format("You chose to buy a house for {0}. Are you sure you want to purchase a house for ${1}?", propertyToBuyFor.getName(), propertyToBuyFor.getHouseCost()));

                if (doBuyHouse)
                {
                    propertyToBuyFor.addHouse();
                    Console.WriteLine("-------------------------------------------------------------");
                    Console.WriteLine(">>>>>>>>>>{0}A new house for {1} has been bought successfully<<<<<<<<<<", playerPrompt(player), propertyToBuyFor.getName());
                }
            }
        }

        public void buyHotel(Player player)
        {
            Console.WriteLine("------------------------------------------------------------");
            string sPrompt = String.Format("{0}Please select a property to buy a hotel for:", this.playerPrompt(player));
            Residential propertyToBuyFor = null;
            if (player.getPropertiesOwnedFromBoard().Count == 0)
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0} do not own any properties.<<<<<<<<<<", playerPrompt(player));
                return;
            }
            Property property = this.displayPropertyChooser(player.getPropertiesOwnedFromBoard(), sPrompt);

            if (property.GetType() == (new Residential().GetType()))
            {
                propertyToBuyFor = (Residential)property;
            }
            else 
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}A hotel cannot be bought for {1} because it is not a Residential Property.<<<<<<<<<<", this.playerPrompt(player), property.getName());
                return;
            }

            if (propertyToBuyFor.getHotelCount() >= Residential.getMaxHotels())
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}The maximum hotel limit for {1} of {2} hotels has been reached.<<<<<<<<<<", playerPrompt(player), propertyToBuyFor.getName(), Residential.getMaxHotels());
            }
            else
            {
                Console.WriteLine("------------------------------------------------------------");
                bool doBuyHotel = this.getInputYN(player, String.Format("You chose to buy a hotel for {0}. Are you sure you want to purchase a hotel for ${1}?", propertyToBuyFor.getName(), propertyToBuyFor.getHotelCost()));
                if (doBuyHotel)
                {
                    propertyToBuyFor.addHotel();
                    Console.WriteLine("-------------------------------------------------------------");
                    Console.WriteLine(">>>>>>>>>>{0}A new hotel for {1} has been bought successfully<<<<<<<<<<", playerPrompt(player), propertyToBuyFor.getName());
                }
            }
        }


        public void tradeProperty(Player player)
        {
            decimal mortgageValue = 0.00m;
            decimal moneyPayBank = 0.00m;
            bool toUnmortgage = false;

            string sPropPrompt = String.Format("{0}Please select a property to trade:", this.playerPrompt(player));
            string sPlayerPrompt = String.Format("{0}Please select a player to trade with:", this.playerPrompt(player));

            Console.WriteLine("--------------------------------------------------------------------------");
            TradableProperty propertyToTrade = (TradableProperty)this.displayPropertyChooser(player.getPropertiesOwnedFromBoard(), sPropPrompt);

            if (propertyToTrade == null)
            {
                Console.WriteLine("---------------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}You do not own any properties.<<<<<<<<<<", playerPrompt(player));
                return;
            }

            Player playerToTradeWith = this.displayPlayerChooser(Board.access().getPlayers(), player, sPlayerPrompt);

            Console.WriteLine("--------------------------------------------------");
            string inputAmtMsg = string.Format("{0}How much do you want for this property?", playerPrompt(player));

            decimal amountWanted = inputDecimal(inputAmtMsg);

            ConsoleColor origColor = Console.ForegroundColor;
            int i = Board.access().getPlayers().IndexOf(playerToTradeWith);
            Console.ForegroundColor = this.colors[i];
            mortgageValue = propertyToTrade.getMortgageValue();
            Console.WriteLine("---------------------------------------------------------------------------");
            bool agreesToTrade = getInputYN(playerToTradeWith, string.Format("{0} wants to trade '{1}' with you for ${2}. Do you agree to pay {2} for '{1}'.", player.getName(), propertyToTrade.getName(), amountWanted));
            if (agreesToTrade)
            {
                if (propertyToTrade.IsMortgaged)
                {
                    Console.WriteLine("---------------------------------------------------------------------------");
                    toUnmortgage = getInputYN(playerToTradeWith, string.Format($"The property is mortgaged, Do you want to unmortgage it now? If \"Y\", you will pay ${mortgageValue * 1.1m:N2} now, or you will pay ${mortgageValue * 0.1m:N2}") );
                    if (toUnmortgage)
                    {
                        moneyPayBank = mortgageValue * 1.1m;
                    }
                    else
                    {
                        moneyPayBank = mortgageValue * 0.1m;

                    }
                }


                if (amountWanted+moneyPayBank> playerToTradeWith.getBalance())
                {
                    Console.WriteLine("---------------------------------------------------------------------------");
                    Console.WriteLine($"{playerToTradeWith.getName()}, your balance ${playerToTradeWith.getBalance()} is not enough for the trade ${amountWanted + moneyPayBank}");
                    return;
                }
                Console.ForegroundColor = origColor;
                Player playerFromBoard = Board.access().getPlayer(playerToTradeWith.getName());

                player.tradeProperty(ref propertyToTrade, ref playerFromBoard, amountWanted);
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0} has been traded successfully. {0} is now owned by {1}<<<<<<<<<<", propertyToTrade.getName(), playerFromBoard.getName());
                if (propertyToTrade.IsMortgaged)
                {
                    playerToTradeWith.pay(moneyPayBank);
                    if (toUnmortgage)
                    {
                        propertyToTrade.IsMortgaged=false;
                        Console.WriteLine("-----------------------------------------------------------");
                        Console.WriteLine(">>>>>>>>>>{0} has been unmortgaged<<<<<<<<<<", propertyToTrade.getName());
                    }
                }          
            }
            else
            {
                Console.ForegroundColor = origColor;
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}{1} does not agree to trade {2} for ${3}<<<<<<<<<<<", playerPrompt(player), playerToTradeWith.getName(), propertyToTrade.getName(), amountWanted);
            }     
        }

        public Property displayPropertyChooser(ArrayList properties, String sPrompt)
        {
            if (properties.Count == 0)
                return null;
            Console.WriteLine(sPrompt);
            for (int i = 0; i < properties.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, properties[i].ToString());
            }
            Console.Write("({0}-{1})=>", 1, properties.Count);
            int resp = this.inputInteger();

            if ((resp < 1) || (resp > properties.Count))
            {
                Console.WriteLine(">>>>>>>>>>That option is not avaliable. Please try again.<<<<<<<<<<");
                return this.displayPropertyChooser(properties, sPrompt);
                
            }
            else
            {
                return (Property) properties[resp - 1];
            }
        }

        public Player displayPlayerChooser(ArrayList players, Player playerToExclude, String sPrompt)
        {
            if (players.Count == 0)
                return null;
            Console.WriteLine(sPrompt);
            ArrayList displayList = new ArrayList(players);

            displayList.Remove(playerToExclude);

            for (int i = 0; i < displayList.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, displayList[i].ToString());
            }
            Console.Write("({0}-{1})=>", 1, displayList.Count);
            int resp = this.inputInteger();

            if ((resp < 1) || (resp > displayList.Count))
            {
                Console.WriteLine(">>>>>>>>>>That option is not avaliable. Please try again.<<<<<<<<<<");
                this.displayPlayerChooser(players, playerToExclude, sPrompt);
                return null;
            }
            else
            {
                Player chosenPlayer = (Player) displayList[resp - 1];
                foreach (Player p in players)
                {
                    if(p.getName() == chosenPlayer.getName())
                        return p;
                }
                return null;
            }
        }

        public static void playerBankruptHandler(object obj, EventArgs args)
            {
                Player p = (Player) obj;
            Console.WriteLine("--------------------------------------");
            Console.WriteLine(">>>>>>>>>>{0} IS BANKRUPT!<<<<<<<<<<", p.getName().ToUpper());

            }

        public static void playerPassGoHandler(object obj, EventArgs args)
        {
            Player p = (Player)obj;
            p.receive(200);         
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine(">>>>>>>>>>{0} has passed go.{0} has received $200<<<<<<<<<<", p.getName());
        }

        public static void playerLandOnChanceHandler(object obj, EventArgs args)
        {
            Player p = (Player)obj;
            if (Board.access().getProperty(p.getLocation()).getName()=="Chance")
            {
                Card card = Board.access().drawChance();
                if (card.Instruction!="FREE")
                {
                    Board.access().insertChance(card);
                }
                card.ExecuteInstruction(p);
            }
            else
            {
                Card card = Board.access().drawChest();
                if (card.Instruction != "FREE")
                {
                    Board.access().insertChest(card);
                }
                card.ExecuteInstruction(p);
            }
        }

        public static void playerGoJailHandler(object obj, EventArgs args)
        {
            int indexOfJail = -1;
            Player p = (Player)obj;
            for (int i=0;i<Board.access().getProperties().Count;i++)
            {
                if ("Jail"== ((Property)(Board.access().getProperties()[i])).getName())
                {
                    indexOfJail = i;
                    break;
                }
            }
            if (indexOfJail==-1)
            {
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine(">>>>>>>>>>Error! Can not find Jail!!!<<<<<<<<<<");
                return;
            }

            p.setLocation(indexOfJail);
            p.IsInJail = true;
            p.DoubleCount = 0;
        }

        
        public bool saveGame()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream file = new FileStream("monopoly.sav", FileMode.Create))
            {
                formatter.Serialize(file, Board.access());
            }
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(">>>>>>>>>>Save successfully!!!<<<<<<<<<<");
            return true;
        }

        
        public void loadGame()
        {
            int initIndexOfPlayer = 0;
            int totalAmtOfPlayers = 0;

            if (!loadData())
            {
                Console.WriteLine(">>>>>>>>>>Load save file failur!<<<<<<<<<<");
                return;
            }
            
            initIndexOfPlayer = Board.access().CurrentPlayer;
            totalAmtOfPlayers = Board.access().getPlayerCount();

            for (int i = initIndexOfPlayer; i < totalAmtOfPlayers; i++)
            {
                Console.Clear();
                Board.access().CurrentPlayer = i;
                Player player = Board.access().getPlayer(i);
                Console.ForegroundColor = this.colors[i];

                if (player.isNotActive())
                {
                    Console.WriteLine("\n>>>>>>>>>>{0} is inactive.<<<<<<<<<<\n", player.getName());
                    int activePlayerCount = 0;
                    foreach (Player p in Board.access().getPlayers())
                    {
                        if (!p.isNotActive())
                            activePlayerCount++;
                    }

                    if (activePlayerCount < 2)
                    {
                        this.printWinner();
                    }

                    return;
                }

                if (player.IsInJail)
                {
                    this.displayMenuInJail(i);
                }
                else
                {

                    if (i != initIndexOfPlayer)
                    {
                        Console.WriteLine("------------------------------------------------");
                        Console.WriteLine("{0}Your turn. Press Enter to make move==>", playerPrompt(i));
                        Console.ReadLine();
                        player.move();

                        Property propertyLandedOn = Board.access().getProperty(player.getLocation());
                        Console.WriteLine(propertyLandedOn.landOn(ref player));
                        Console.WriteLine("\n{0}{1}", playerPrompt(i), player.BriefDetailsToString());
                        if (player.IsInJail)
                        {
                            Console.WriteLine("---------------------------------------------------------------------");
                            Console.WriteLine($">>>>>>>>>>{player.getName()}: You are in jail now, your turn ends!!!<<<<<<<<<<");
                            Console.WriteLine("Press any key to continue!!!==>");
                            Console.ReadLine();
                        }
                    }                  
                    else
                    {
                        displayPlayerChoiceMenu(player);
                    }
                }
            }
            playGame();

        }

        public bool loadData()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                using (FileStream file = new FileStream("monopoly.sav", FileMode.Open))
                {
                    Board.access().load((formatter.Deserialize(file) as Board));

                }
                foreach (Property p in Board.access().getProperties())
                {
                    if (p.getOwner().getName() == "Banker")
                    {
                        Banker.access().setBalance(p.getOwner().getBalance());
                        p.setOwner(Banker.access());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }

        public void displayMenuInJail(int indexOfPlayer)
        {
            int resp = 0;

            Player player = Board.access().getPlayer(indexOfPlayer);
            Console.WriteLine();
            Console.WriteLine("                 =======================================");
            Console.WriteLine("                 {0}Please make a selection:", playerPrompt(player));
            Console.WriteLine("                 ---------------------------------------");
            Console.WriteLine("                 1. Finish turn");
            Console.WriteLine("                 2. View your details");
            Console.WriteLine("                 3. Try to roll double");
            Console.WriteLine("                 4. Pay $50 to leave from Jail");
            Console.WriteLine("                 5. Use Free Card");
            Console.WriteLine("                 6. Trade Property with Player");
            Console.WriteLine("                 7. Mortgage Property");
            Console.WriteLine("                 8. Save Game");
            Console.WriteLine("                 9. Quit Game");
            Console.WriteLine("                 =======================================");

            Console.Write("                 (1-8)=>");
            resp = inputInteger();
            if (resp == 0)
                this.displayMenuInJail(indexOfPlayer);

            switch (resp)
            {
                case 1:
                    break;
                case 2:
                    Console.WriteLine("==================================");
                    Console.WriteLine(player.FullDetailsToString());
                    Console.WriteLine("==================================");
                    this.displayMenuInJail(indexOfPlayer);
                    break;
                case 3:
                    if (player.FailureRollDouble >= 3)
                    {
                        Console.WriteLine("---------------------------------------------------------------------------");
                        Console.WriteLine($">>>>>>>>>>{player.getName()}: You have failed to roll double thrice, you can not roll any more!!!<<<<<<<<<<");
                        this.displayMenuInJail(indexOfPlayer);
                    }
                    else
                    {
                        this.rollDouble(player);
                    }
                    break;
                case 4:
                    if (this.payForLeave(player)) {
                        this.makePlay(indexOfPlayer);
                    }
                    else
                    {
                        this.displayMenuInJail(indexOfPlayer);
                    }
                    break;
                case 5:
                    if (this.useFreeCard(player))
                    {
                        this.makePlay(indexOfPlayer);
                    }
                    else
                    {
                        this.displayMenuInJail(indexOfPlayer);
                    }
                    break;
                case 6:
                    this.tradeProperty(player);
                    this.displayMenuInJail(indexOfPlayer);
                    break;
                case 7:
                    this.mortgageProperty(player);
                    this.displayMenuInJail(indexOfPlayer);
                    break;
                case 8:
                    this.saveGame();
                    this.displayMenuInJail(indexOfPlayer);
                    break;
                case 9:
                    this.quitGame();
                    break;
                default:
                    Console.WriteLine(">>>>>>>>>>That option is not avaliable. Please try again.<<<<<<<<<<");
                    this.displayMenuInJail(indexOfPlayer);
                    break;
            }
        }
        public bool rollDouble(Player player)
        {
            int i , j ;
            Dice dice = new Dice();
            i = j = 0;
            Console.WriteLine("----------------------------------------===--------");
            Console.WriteLine($"{player.getName()}:Please press Enter to roll dice!!!==>");
            Console.ReadLine();
            i = dice.roll();
            j = dice.roll();
            Console.WriteLine($"{player.getName()}:Number rolled is [{i}][{j}]");
            if (i == j)
            {
                Console.WriteLine("------------------------------------------------------------------");
                Console.WriteLine($">>>>>>>>>>{player.getName()}:Congratulation! You leave the jail!!!<<<<<<<<<<");
                player.IsInJail = false;
                player.DoubleCount = 0;         
                player.FailureRollDouble = 0;  

                player.setLocation(player.getLocation()+i+j);
                Property propertyLandedOn = Board.access().getProperty(player.getLocation());
                Console.WriteLine(">>>>>>>>>>" + propertyLandedOn.landOn(ref player) + "<<<<<<<<<<");
                Console.WriteLine("\n{0}:\t{1}", player.getName(), player.BriefDetailsToString());

                Console.WriteLine("Press any key to continue!!!==>");
                Console.ReadLine();
                return true;
            }
            else
            {
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine($">>>>>>>>>>{player.getName()}:It's a pity you fail to roll double and your turn is over!!!<<<<<<<<<<");
                player.FailureRollDouble++;
                Console.WriteLine("Press any key to continue!!!==>");
                Console.ReadLine();
                return false;
            }

        }
        public bool payForLeave(Player player)
        {
            if (player.getBalance() >= 50)
            {
                player.pay(50);
                player.IsInJail = false;
                player.DoubleCount = 0;         
                player.FailureRollDouble = 0;  
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($">>>>>>>>>>{player.getName()}:Congratulation! You leave the jail!!!<<<<<<<<<<");
                return true;
            }
            else
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($">>>>>>>>>>{player.getName()}, you have no enough money to pay the $50!<<<<<<<<<<");
                return false;
            }
        }

        public bool useFreeCard(Player player)
        {
            if (!player.HasChanceFreeCard && !player.HasChestFreeCard)
            {
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine($">>>>>>>>>>{player.getName()}, you have no free card!<<<<<<<<<<");
                return false;
            }
            if (player.HasChanceFreeCard)
            {
                player.IsInJail = false;
                player.HasChanceFreeCard = false;
                Board.access().insertChance(new FreeCard("Chance","FREE", "Get Out of Jail Free"));
                player.DoubleCount = 0;         
                player.FailureRollDouble = 0;   
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine($">>>>>>>>>>{player.getName()}:Congratulation! You leave the jail!!!<<<<<<<<<<");
                return true;
            }
            else
            {
                player.IsInJail = false;
                player.HasChestFreeCard = false;
                Board.access().insertChest(new FreeCard("Community Chest", "FREE", "Get Out of Jail Free"));
                player.DoubleCount = 0;         
                player.FailureRollDouble = 0;   
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine($">>>>>>>>>>{player.getName()}:Congratulation! You leave the jail!!!<<<<<<<<<<");
                return true;
            }

        }

        public void quitGame()
        {
            Console.Clear();
            Console.WriteLine("                 =========================================");
            Console.WriteLine("                                 GAME OVER");
            Console.WriteLine("                 =========================================");

            Environment.Exit(0);
        }

        public void sellHouse(Player player)
        {
            string sPrompt = String.Format("{0}Please select a property to sell a house:", this.playerPrompt(player));
            Residential residential = null;

            if (player.getPropertiesOwnedFromBoard().Count == 0)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}You do not own any properties.<<<<<<<<<<", playerPrompt(player));
                return;
            }

            Console.WriteLine("-------------------------------------------------------");
            Property property = this.displayPropertyChooser(player.getPropertiesOwnedFromBoard(), sPrompt);

            if (property.GetType() == (new Residential().GetType()))
            {
                residential = (Residential)property;
                if (residential.getHouseCount()==0)
                {
                    Console.WriteLine("-------------------------------------------------------");
                    Console.WriteLine(">>>>>>>>>>{0}: {1} has no house.<<<<<<<<<<", this.playerPrompt(player), residential.getName());
                    return;
                }
            }
            else
            {
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}A house can not be sold for {1} because it is not a Residential Property.<<<<<<<<<<", this.playerPrompt(player), property.getName());
                return;
            }

            Console.WriteLine("-----------------------------------------------------------");
            bool doSellHouse = this.getInputYN(player, String.Format("You chose to sell a house on {0}. Are you sure?", residential.getName()));
            if (doSellHouse)
            {
                residential.reduceHouse();
                Console.WriteLine(">>>>>>>>>>{0}A  house on {1} has been sold back to the bank successfully!<<<<<<<<<<", playerPrompt(player), residential.getName());
            }

        }

        public void sellHotel(Player player)
        {
            string sPrompt = String.Format("{0}Please select a property to sell a hotel:", this.playerPrompt(player));
            Residential residential = null;

            if (player.getPropertiesOwnedFromBoard().Count == 0)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}You do not own any properties.<<<<<<<<<<", playerPrompt(player));
                return;
            }

            Console.WriteLine("-------------------------------------------------------");
            Property property = this.displayPropertyChooser(player.getPropertiesOwnedFromBoard(), sPrompt);

            if (property.GetType() == (new Residential().GetType()))
            {
                residential = (Residential)property;
                if (residential.getHotelCount() == 0)
                {
                    Console.WriteLine("-------------------------------------------------------");
                    Console.WriteLine(">>>>>>>>>>{0}: {1} has no hotel.<<<<<<<<<<", this.playerPrompt(player), residential.getName());
                    return;
                }
            }
            else
            {
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}A hotel can not be sold for {1} because it is not a Residential Property.<<<<<<<<<<", this.playerPrompt(player), property.getName());
                return;
            }

            Console.WriteLine("-----------------------------------------------------------");
            bool doSellHotel = this.getInputYN(player, String.Format("You chose to sell a hotel on {0}. Are you sure?", residential.getName()));
            if (doSellHotel)
            {
                residential.reduceHotel();
                Console.WriteLine(">>>>>>>>>>{0}A  hotel on {1} has been sold back to the bank successfully!<<<<<<<<<<", playerPrompt(player), residential.getName());
            }

        }

        public void mortgageProperty(Player player)
        {
            string sPrompt = String.Format("{0}Please select a property to mortgage:", this.playerPrompt(player));
            TradableProperty tp = null;

            if (player.getPropertiesOwnedFromBoard().Count == 0)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}You do not own any properties!<<<<<<<<<<", playerPrompt(player));
                return;
            }
            Console.WriteLine("-------------------------------------------------------");
            Property property = this.displayPropertyChooser(player.getPropertiesOwnedFromBoard(), sPrompt);

            if (new TradableProperty().GetType().IsAssignableFrom(property.GetType()))
            {
                tp = (TradableProperty)property;
                if (tp.IsMortgaged)
                {
                    Console.WriteLine(">>>>>>>>>>{0}: {1} has been mortgaged already!<<<<<<<<<<", this.playerPrompt(player), tp.getName());
                    return;
                }
            }
            else
            {
                Console.WriteLine(">>>>>>>>>>{0} {1} can not be mortgaged because it is not a tradable Property!<<<<<<<<<<", this.playerPrompt(player), property.getName());
                return;
            }


            bool doMortgage = this.getInputYN(player, String.Format("You chose to mortgage {0}. Are you sure?", tp.getName()));
            if (doMortgage)
            {
                if (tp.mortgage(player))
                {
                    Console.WriteLine(">>>>>>>>>>{0}you have mortgaged {1} to the bank successfully!<<<<<<<<<<", playerPrompt(player), tp.getName());
                }
            }

        }

        public void unmortgageProperty(Player player)
        {
            string sPrompt = String.Format("{0}Please select a property to unmortgage:", this.playerPrompt(player));
            TradableProperty tp = null;

            if (player.getPropertiesOwnedFromBoard().Count == 0)
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine(">>>>>>>>>>{0}You do not own any properties!<<<<<<<<<<", playerPrompt(player));
                return;
            }

            Console.WriteLine("-------------------------------------------------------");
            Property property = this.displayPropertyChooser(player.getPropertiesOwnedFromBoard(), sPrompt);

            if (new TradableProperty().GetType().IsAssignableFrom(property.GetType()))
            {
                tp = (TradableProperty)property;
                if (!tp.IsMortgaged)
                {
                    Console.WriteLine(">>>>>>>>>>{0}: {1} has not been mortgaged yet!<<<<<<<<<<", this.playerPrompt(player), tp.getName());
                    return;
                }
            }
            else
            {
                Console.WriteLine(">>>>>>>>>>{0} {1} can not be unmortgaged because it is not a tradable Property!<<<<<<<<<<", this.playerPrompt(player), property.getName());
                return;
            }


            bool doUnmortgage = this.getInputYN(player, String.Format("You chose to unmortgage {0}. Are you sure?", tp.getName()));

            if (doUnmortgage)
            {
                if (tp.unmortgage(player))
                {
                    Console.WriteLine(">>>>>>>>>>{0}you have unmortgaged {1} from the bank successfully<<<<<<<<<<", playerPrompt(player), tp.getName());
                }
            }
        }
    }
}

