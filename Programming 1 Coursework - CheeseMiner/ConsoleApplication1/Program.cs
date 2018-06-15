using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    // NOTE during demo cheese duel will mess up positions. Use dice values 2 for loser 
    // and 1 for winner to avoid moving!!!

    /* to do:  User guide: pretty muchwrite up the instructions section to word and throw 
     * in some extra screenshots for examples. Test report: record each move and its outcome * expected outcome and any observations for a few games.
     */
    class cell
    {
        public string display;
        public ConsoleColor color;
        public int xPos;
        public int yPos;
        public int cellID;
        public bool hasCheese = false;
        public bool[] occupants = { false, false, false, false };        // as per, 0 = player, 1 = Barry, 2 = Carol, 3 = Sterling.
        public int cellType;            // used for switch on cell enter/exit events
        // 0 for blank
        // 1 for cheese
        // 2 for player
        // 3 for Barry (red), 4 for Carol (magenta), 5 for Sterling (green).
    }



    class player
    {
        public bool isHuman;
        public string name;
        public int score;
        public int xPos;
        public int yPos;
        public cell currentCell;
        public ConsoleColor color;
    }

    struct demoCoords
    {
        private readonly int xCoord;
        private readonly int yCoord;

        public demoCoords(int xCoord, int yCoord)
        {
            this.yCoord = yCoord;
            this.xCoord = xCoord;
        }

        public int YCoord { get { return yCoord; } }
        public int XCoord { get { return xCoord; } }

    }


    class Program
    {

        private static Random rnd = new Random();
        private static int gridCount = 0;
        private static player[] players;
        private static cell[] cells;
        private static int roundNumber = 1; // player : 1, Barry : 2, Carol : 3, Sterling : 4.
        private static List<cell> cheesePositions = new List<cell>();
        private static List<cell> gridList = new List<cell>();
        private static int rollNumber;
        private static int cheeseRemaining;
        private static bool barryLeft = true;
        private static bool carolright = true;
        private static bool isDemoMode = false;
        private static bool isFirstRound = true; // if this is the first round of the game show help (controls etc).

        static void Main(string[] args)
        {
            Program instance = new Program();
            Console.Title = "Cheese Miner";
            instance.initialiseGame();
        }

        void initialiseGame()
        {
            Program instance = new Program();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            isFirstRound = true;
            players = new player[4];
            cells = new cell[64];
            gridList.Clear();
            cheesePositions.Clear();
            gridCount = 0;
            roundNumber = 1;
            bool valid = false;
            for (int i = 0; i < 64; i++)
            {
                cells[i] = new cell { cellType = 0 };
            }

            do
            {
                Console.Clear();
                Console.WriteLine("How many humans are playing? (No synths!) 1 - 4");
                ConsoleKeyInfo keyPress = Console.ReadKey();
                if (keyPress.Key == ConsoleKey.D1 || keyPress.Key == ConsoleKey.NumPad1)
                {
                    setupPlayers(1);
                    valid = true;
                }
                else if (keyPress.Key == ConsoleKey.D2 || keyPress.Key == ConsoleKey.NumPad2)
                {
                    setupPlayers(2);
                    valid = true;
                }
                else if (keyPress.Key == ConsoleKey.D3 || keyPress.Key == ConsoleKey.NumPad3)
                {
                    setupPlayers(3);
                    valid = true;
                }
                else if (keyPress.Key == ConsoleKey.D4 || keyPress.Key == ConsoleKey.NumPad4)
                {
                    setupPlayers(4);
                    valid = true;
                }

            } while (!valid);


            instance.generateGrid();
        }

        void changeTextColorAddText(ConsoleColor col, string text, bool line)
        {
            Console.ForegroundColor = col;
            if (line)
                Console.WriteLine(text);
            else Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        void setupPlayers(int numberOfPlayers)
        {

            Console.Clear();
            bool valid = false;
            for (int i = 0; i < 4; i++)
            {
                players[i] = new player();  // instance new player. 

                if (i < numberOfPlayers)
                {
                    players[i].isHuman = true;  // sets number of human players.
                }

                if (players[i].isHuman)         // set players names. If human let them choose their own.
                {
                    do
                    {
                        Console.WriteLine("Player " + (i + 1) + ", please enter your name:");
                        string input = Console.ReadLine();
                        if (input != "")
                        {
                            players[i].name = input;
                            valid = true;
                        }
                    } while (!valid);
                }
                else
                {
                    if (i == 1)
                        players[i].name = "Barry";
                    else if (i == 2)
                        players[i].name = "Carol";
                    else if (i == 3)
                        players[i].name = "Sterling";
                }
            }
            players[0].color = ConsoleColor.Cyan;       // cell case 2   
            players[1].color = ConsoleColor.Red;        // cell case 3
            players[2].color = ConsoleColor.Magenta;    // cell case 4
            players[3].color = ConsoleColor.Green;      // cell case 5

        }

        void generateGrid()
        {

            for (int i = 0; i < 8; i++) // x axis.
            {
                for (int j = 0; j < 8; j++) // y axis.
                {
                    cells[gridCount].display = "B ";
                    cells[gridCount].cellType = 0;
                    cells[gridCount].xPos = i;
                    cells[gridCount].yPos = j;
                    cells[gridCount].color = ConsoleColor.White;
                    cells[gridCount].cellID = gridCount;


                    if (gridCount == 0)
                    {
                        cells[gridCount].occupants[3] = true;
                        cells[gridCount].cellType = 5;
                        players[3].currentCell = cells[gridCount];
                        players[3].xPos = players[3].currentCell.xPos;
                        players[3].yPos = players[3].currentCell.yPos;
                    }
                    if (gridCount == 7)
                    {


                        cells[gridCount].cellType = 4;
                        cells[gridCount].occupants[2] = true;
                        players[2].currentCell = cells[gridCount];
                        players[2].xPos = players[2].currentCell.xPos;
                        players[2].yPos = players[2].currentCell.yPos;
                    }
                    if (gridCount == 64 - 8)
                    {


                        cells[gridCount].cellType = 2;
                        cells[gridCount].occupants[0] = true;
                        players[0].currentCell = cells[gridCount];
                        players[0].xPos = players[0].currentCell.xPos;
                        players[0].yPos = players[0].currentCell.yPos;
                    }
                    if (gridCount == 63)
                    {


                        cells[gridCount].cellType = 3;
                        cells[gridCount].occupants[1] = true;
                        players[1].currentCell = cells[gridCount];
                        players[1].xPos = players[1].currentCell.xPos;
                        players[1].yPos = players[1].currentCell.yPos;
                    }


                    gridList.Add(cells[gridCount]);
                    gridCount++;
                }
            }

            checkCheeseMode();


        }

        void checkCheeseMode()
        {
            Program instance = new Program();
            bool valid = false;
            do
            {
                Console.Clear();
                Console.WriteLine("Would you like cheese positioning to be entered:");
                Console.WriteLine("1 : Randomly");
                Console.WriteLine("2 : By Players (manually)");
                Console.WriteLine("3 : Demo mode");
                ConsoleKeyInfo key = new ConsoleKeyInfo();
                key = Console.ReadKey();
                Console.Clear();
                if (key.Key == ConsoleKey.D1 || key.Key == ConsoleKey.NumPad1) // check for either '1' entry
                {
                    instance.addCheese();
                    valid = true;
                }
                else if (key.Key == ConsoleKey.D2 || key.Key == ConsoleKey.NumPad2)
                {
                    instance.addCheeseManual();
                    valid = true;
                }
                else if (key.Key == ConsoleKey.D3 || key.Key == ConsoleKey.NumPad3)
                {
                    isDemoMode = true;
                    instance.addCheese();
                    valid = true;
                }
                else
                    valid = false;
            } while (!valid);



        }

        void addCheeseManual()
        {
            bool valid = false;
            string[] splitCoords; // used to seperate x and y coordinates after validity check.
            cheeseRemaining = 0;
            int entryX;
            int entryY;
            for (int i = 0; i < 16; i++)
            {
                do
                {
                    Console.Clear();
                    drawGrid();
                    Console.WriteLine("Please enter cheese coordinates in the form 'x,y'     " + i + " of " + "16");
                    Console.WriteLine("");
                    string rawInput = Console.ReadLine();
                    string[] seperators = new string[] { ".", ",", " " };
                    splitCoords = rawInput.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                    cell tempCell;
                    if (splitCoords.Length == 2 && int.TryParse(splitCoords[0], out entryY) && int.TryParse(splitCoords[1], out entryX))
                    {
                        tempCell = findCell(entryX, entryY);        // check cheese entry to ensure a) it is not placed on top of a player. b) it is not placed on another cheese. c) it exists within the grid.

                        if (entryX == 0 && entryY == 0)
                            valid = false;
                        else if (entryX == 0 && entryY == 7)
                            valid = false;
                        else if (entryX == 7 && entryY == 7)            // could have reduced this to one !if statement but this is 'allegedly' easier to understand for people that didnt make the code. 
                            valid = false;
                        else if (entryX == 7 && entryY == 0)
                            valid = false;
                        else if (cheesePositions.Contains(tempCell))
                            valid = false;
                        else if (entryX > 7 || entryX < 0 || entryY > 7 || entryY < 0)
                            valid = false;
                        else
                        {
                            cheesePositions.Add(tempCell);
                            tempCell.cellType = 1;
                            tempCell.display = "C ";
                            tempCell.color = ConsoleColor.Yellow;
                            tempCell.hasCheese = true;
                            cheeseRemaining++;
                            valid = true;
                        }
                    }
                    else
                    {
                        valid = false;
                        Console.Clear();
                        Console.WriteLine("");//padding
                        Console.WriteLine("Please enter valid coordinates; note you cannot place cheese on player positions."); // message to display when invalid coords are entered. 
                        Console.WriteLine("");

                    }

                } while (!valid);


            }
            Console.Clear();
            drawGrid();
            checkRound();
        }



        void addCheese()
        {
            if (!isDemoMode)
            {
                cheeseRemaining = 16;
                int tempRandomNumberX;    // use this temp int for checking that we do not repeat positions
                int tempRandomNumberY;
                bool isNotViablePosition;

                for (int i = 0; i < 16; i++)
                {
                    isNotViablePosition = true;
                    cell tempCell;

                    do
                    {
                        isNotViablePosition = false;
                        tempRandomNumberX = rnd.Next(7);
                        tempRandomNumberY = rnd.Next(7);
                        System.Threading.Thread.Sleep(1); // sometimes doesnt update before the check.

                        tempCell = findCell(tempRandomNumberX, tempRandomNumberY);

                        if (tempRandomNumberX == 0 && tempRandomNumberY == 0)
                            isNotViablePosition = true;
                        else if (tempRandomNumberX == 0 && tempRandomNumberY == 7)
                            isNotViablePosition = true;
                        else if (tempRandomNumberX == 7 && tempRandomNumberY == 7)
                            isNotViablePosition = true;
                        else if (tempRandomNumberX == 7 && tempRandomNumberY == 0)
                            isNotViablePosition = true;
                        else if (cheesePositions.Contains(tempCell))
                            isNotViablePosition = true;




                    } while (isNotViablePosition);



                    cheesePositions.Add(tempCell);
                    tempCell.cellType = 1;
                    tempCell.display = "C ";
                    tempCell.color = ConsoleColor.Yellow;
                    tempCell.hasCheese = true;

                }
            }
            else
            {
                // demo mode cheese positions entered if in demo mode. 
                demoCoords[] demoArray = new demoCoords[]
                {
                    new demoCoords(1, 0),
                    new demoCoords(3, 6),
                    new demoCoords(5, 3),
                    new demoCoords(2, 7),
                    new demoCoords(0, 5),
                    new demoCoords(0, 1),
                    new demoCoords(5, 5),
                    new demoCoords(2, 4),
                    new demoCoords(1, 3),
                    new demoCoords(6, 1),
                    new demoCoords(6, 2),
                    new demoCoords(6, 7),
                    new demoCoords(4, 5),
                    new demoCoords(5, 0),
                    new demoCoords(7, 6),
                    new demoCoords(6, 0)
                };

                for (int i = 0; i < 16; i++)
                {
                    cell tempCell = findCell(demoArray[i].YCoord, demoArray[i].XCoord);
                    cheesePositions.Add(tempCell);
                    tempCell.cellType = 1;
                    tempCell.display = "C ";
                    tempCell.color = ConsoleColor.Yellow;
                    tempCell.hasCheese = true;
                }
                cheeseRemaining = 16;
                players[0].score = 3;

            }
            Console.Clear();
            drawGrid();
            checkRound();
        }

        public cell findCell(int x, int y)
        {

            if (x > 7)
                x -= 8;
            else if (y > 7)
                y -= 8;
            else if (x < 0)
                x += 8;
            else if (y < 0)
                y += 8;

            cell searchCell = new cell();

            for (int i = 0; i < 63; i++)
            {
                if (cells[i].xPos == x && cells[i].yPos == y)   // slightly less efficient search method but saves confusion.
                {
                    searchCell = cells[i];
                }
            }
            return searchCell;
        }

        public ConsoleColor assignTextColor(int num)        // used to set color of background on grid to match each cells current circumstances. 
        {
            switch (num)
            {
                case 0: return ConsoleColor.White;
                case 1: return ConsoleColor.Yellow;
                case 2: return ConsoleColor.Cyan;
                case 3: return ConsoleColor.Red;
                case 4: return ConsoleColor.Magenta;
                case 5: return ConsoleColor.Green;
                default: return ConsoleColor.White;

            }
        }



        void roll(int turn)
        {
            turn -= 1;
            Program instance = new Program();
            if (!isDemoMode)
                rollNumber = rnd.Next(1, 7);
            else
            {
                bool valid = false;
                do
                {
                    Console.WriteLine("");
                    Console.WriteLine("Please enter dice value");
                    string input = Console.ReadLine();
                    int.TryParse(input, out rollNumber);
                    if (rollNumber > 0 && rollNumber < 7)
                        valid = true;
                } while (!valid);
            }
            changeTextColorAddText(players[turn].color, players[turn].name, false);
            Console.WriteLine(" rolled a " + rollNumber);

            if (turn == 0)      // player 0 must be human player 
            {
                handleHumanPlayersTurn(turn);
            }

            if (turn == 1) // handle Barry's turn
            {
                if (!players[turn].isHuman)
                {
                    if (barryLeft)
                    {
                        players[turn].yPos -= rollNumber;
                        if (players[turn].yPos < 0)
                            players[turn].yPos += 8;
                        changeTextColorAddText(players[turn].color, players[turn].name, false);
                        Console.WriteLine(" moved " + rollNumber + " to the left.");
                    }
                    else
                    {
                        players[turn].xPos -= rollNumber;
                        changeTextColorAddText(players[turn].color, players[turn].name, false);
                        Console.WriteLine(" moved " + rollNumber + " upwards.");
                    }

                    if (players[turn].xPos < 0)
                        players[turn].xPos += 8;



                    barryLeft = !barryLeft;
                }
                else handleHumanPlayersTurn(turn);
            }

            if (turn == 2) // handle Carols turn.
            {
                if (!players[turn].isHuman)
                {
                    if (carolright)
                    {
                        players[turn].yPos += rollNumber;
                        if (players[turn].yPos > 7)
                            players[turn].yPos -= 8;
                        changeTextColorAddText(players[turn].color, players[turn].name, false);
                        Console.WriteLine(" moved " + rollNumber + " to the right.");
                    }
                    else
                    {
                        players[turn].xPos += rollNumber;
                        changeTextColorAddText(players[turn].color, players[turn].name, false);
                        Console.WriteLine(" moved " + rollNumber + " downwards.");
                    }
                    if (players[turn].xPos > 7)
                        players[turn].xPos -= 8;


                    carolright = !carolright;
                }
                else handleHumanPlayersTurn(turn);
            }

            if (turn == 3) // handle Sterling's turn
            {
                if (!players[turn].isHuman)
                {
                    List<cell> possibleDirections = new List<cell>();
                    possibleDirections.Add(findCell(players[turn].xPos, players[turn].yPos + rollNumber));
                    possibleDirections.Add(findCell(players[turn].xPos, players[turn].yPos - rollNumber));
                    possibleDirections.Add(findCell(players[turn].xPos + rollNumber, players[turn].yPos));
                    possibleDirections.Add(findCell(players[turn].xPos - rollNumber, players[turn].yPos));


                    cell newDestination = new cell();

                    foreach (cell point in possibleDirections)
                    {
                        if (point.cellType > 1)
                        {
                            newDestination = point;
                        }
                    }
                    if (newDestination.display == "") ;
                    newDestination = possibleDirections[rnd.Next(3)];
                    changeTextColorAddText(players[turn].color, players[turn].name, false);
                    if (players[turn].xPos == newDestination.xPos)
                    {
                        if (newDestination.yPos > players[turn].yPos)
                            Console.WriteLine(" moved " + (newDestination.yPos - players[turn].yPos) + " to the right.");
                        else
                            Console.WriteLine(" moved " + (players[turn].yPos - newDestination.yPos) + " to the left.");        // x axis has become y axis on these comparissons... why? no idea, works in search
                    }                                                                                                       // not a problem, just flip them... still odd maybe command prompt syntax?
                    else
                    {
                        if (newDestination.xPos > players[turn].xPos)
                            Console.WriteLine(" moved " + (newDestination.xPos - players[turn].xPos) + " downwards.");
                        else Console.WriteLine(" moved " + (players[turn].xPos - newDestination.xPos) + " upwards.");
                    }
                    players[turn].yPos = newDestination.yPos;
                    players[turn].xPos = newDestination.xPos;
                }
                else handleHumanPlayersTurn(turn);
            }

            players[turn].currentCell.occupants[turn] = false;
            players[turn].currentCell.display = "B ";
            players[turn].currentCell.cellType = 0;
            cell newPosition = findCell(players[turn].xPos, players[turn].yPos);
            newPosition.color = players[turn].color;
            if (newPosition.cellType == 1)
            {                            // add score if landed on cheese.
                players[turn].score++;
                newPosition.hasCheese = false;
                cheesePositions.Remove(newPosition);
                newPosition.display = "B ";
                cheeseRemaining--;
                Console.WriteLine("");
                changeTextColorAddText(players[turn].color, players[turn].name, false);
                Console.WriteLine(" Gained Cheese!");
                Console.WriteLine("");
            }


            newPosition.cellType = turn + 2;     // id for Player occupied cell.
            players[turn].currentCell = newPosition;
            players[turn].currentCell.occupants[turn] = true;
            players[turn].xPos = players[turn].currentCell.xPos;
            players[turn].yPos = players[turn].currentCell.yPos;
            drawGrid();



            if (rollNumber == 6)    // player gets another turn if he/she rolls a 6.
            {
                changeTextColorAddText(players[turn].color, players[turn].name, false);
                Console.WriteLine(" Rolled a 6 and may choose to roll again or enter a cheese ");
                Console.WriteLine("wormhole and jump to a player of their choice.");
                Console.WriteLine("");
                if (players[turn].isHuman)
                {
                    bool check = false;

                    do
                    {
                        Console.WriteLine("1: Roll again");
                        Console.WriteLine("2: Wormhole");

                        ConsoleKeyInfo key = Console.ReadKey();

                        if (key.Key == ConsoleKey.D1 || key.Key == ConsoleKey.NumPad1)
                        {
                            roundNumber--;
                            check = true;
                        }

                        if (key.Key == ConsoleKey.D2 || key.Key == ConsoleKey.NumPad2)
                        {
                            cheeseWormhole(turn);
                            check = true;
                        }

                    } while (check == false);
                }
                else
                {
                    int rng = rnd.Next(2);
                    if (rng == 1)
                    {
                        roundNumber--;
                        changeTextColorAddText(players[turn].color, players[turn].name, false);
                        Console.WriteLine(" chose to take another turn.");
                    }
                    else
                    {
                        changeTextColorAddText(players[turn].color, players[turn].name, false);
                        Console.WriteLine(" chose to take the cheese wormhole!");
                        instance.cheeseWormhole(turn);
                    }
                }
            }

            for (int i = 0; i < 4; i++)             // check through respective player ID's (2-6) to determine if a player (not themselves) are occupying a cell
            {
                if (players[i].currentCell == players[turn].currentCell && i != turn)
                {
                    if (players[i].score > 0 || players[turn].score > 0)
                        cheeseGrappleDuel(turn, i);
                    else
                    {
                        changeTextColorAddText(players[i].color, players[i].name, false);
                        Console.WriteLine(" does not have enough cheese, there will be no duel.");
                    }
                }
            }

            roundNumber++;
            checkRound();
        }


        void cheeseWormhole(int currentPlayer)
        {
            // add array of other players' current cell, give player option to choose between them. For computer 
            // jump to the player with the most cheese. 
            Console.Clear();
            drawGrid();
            players[currentPlayer].currentCell.occupants[currentPlayer] = false;
            players[currentPlayer].currentCell.cellType = 0;
            player[] jumpPlayers = new player[3];
            int cellIndex = 0;
            for (int i = 0; i < 4; i++)
            {
                if (i != currentPlayer)
                {
                    jumpPlayers[cellIndex] = players[i];
                    cellIndex++;
                }
            }
            bool valid = false;
            if (players[currentPlayer].isHuman)
            {
                do
                {

                    Console.WriteLine("");
                    Console.WriteLine("Please select a player to warp to");     // display each player that is not the player taking a turn alongside their x,y coords. 
                    for (int i = 0; i < 3; i++)
                    {
                        Console.Write((i + 1) + ": ");
                        changeTextColorAddText(jumpPlayers[i].color, jumpPlayers[i].name, false);
                        Console.WriteLine(" , (" + jumpPlayers[i].yPos + "," + jumpPlayers[i].xPos + ")   " + jumpPlayers[i].score + " score");
                    }

                    Console.WriteLine("");
                    ConsoleKeyInfo key = Console.ReadKey(); // take key info for choice
                    if (key.Key == ConsoleKey.D1 || key.Key == ConsoleKey.NumPad1)
                    {
                        players[currentPlayer].currentCell = jumpPlayers[0].currentCell;
                        changeTextColorAddText(players[currentPlayer].color, players[currentPlayer].name, false);
                        Console.Write(" warped to ");
                        changeTextColorAddText(jumpPlayers[0].color, jumpPlayers[0].name, false);
                        Console.WriteLine("!");

                        valid = true;
                    }

                    else if (key.Key == ConsoleKey.D2 || key.Key == ConsoleKey.NumPad2)
                    {
                        players[currentPlayer].currentCell = jumpPlayers[1].currentCell;
                        changeTextColorAddText(players[currentPlayer].color, players[currentPlayer].name, false);
                        Console.Write(" warped to ");
                        changeTextColorAddText(jumpPlayers[1].color, jumpPlayers[1].name, false);
                        Console.WriteLine("!");
                        valid = true;
                    }

                    else if (key.Key == ConsoleKey.D3 || key.Key == ConsoleKey.NumPad3)
                    {
                        players[currentPlayer].currentCell = jumpPlayers[2].currentCell;
                        changeTextColorAddText(players[currentPlayer].color, players[currentPlayer].name, false);
                        Console.Write(" warped to ");
                        changeTextColorAddText(jumpPlayers[2].color, jumpPlayers[2].name, false);
                        Console.WriteLine("!");
                        valid = true;
                    }
                    else valid = false;


                } while (!valid);
            }
            else
            {
                player highScorer = jumpPlayers[0];
                for (int i = 0; i < 3 /*i heart you too code*/; i++)
                {
                    if (jumpPlayers[i].score > highScorer.score)
                    {
                        highScorer = jumpPlayers[i];
                    }
                }
                players[currentPlayer].currentCell = highScorer.currentCell;
                players[currentPlayer].xPos = players[currentPlayer].xPos;
                players[currentPlayer].yPos = players[currentPlayer].yPos;
                changeTextColorAddText(players[currentPlayer].color, players[currentPlayer].name, false);
                Console.Write(" warped to ");
                changeTextColorAddText(highScorer.color, highScorer.name, false);
                Console.WriteLine("!");
            }
            players[currentPlayer].xPos = players[currentPlayer].currentCell.xPos;
            players[currentPlayer].yPos = players[currentPlayer].currentCell.yPos;
            players[currentPlayer].currentCell.cellID = currentPlayer + 2;
            players[currentPlayer].currentCell.occupants[currentPlayer] = true;

            for (int i = 0; i < 4; i++)             // check through respective player ID's (2-6) to determine if a player (not themselves) are occupying a cell
            {
                if (players[i].currentCell == players[currentPlayer].currentCell && i != currentPlayer)
                {
                    if (players[i].score > 0 || players[currentPlayer].score > 0)
                        cheeseGrappleDuel(currentPlayer, i);
                    else
                    {
                        changeTextColorAddText(players[i].color, players[i].name, false);
                        Console.WriteLine(" does not have enough cheese, there will be no duel.");
                    }
                }
            }

            drawGrid();

        }



        public void cheeseGrappleDuel(int player1, int player2)
        {
            bool rollNotEqual = false;
            int player1Roll;
            int player2Roll;
            do
            {
                Console.Clear();
                Console.WriteLine("A cheese duel begins!");
                Console.WriteLine("");
                changeTextColorAddText(players[player1].color, players[player1].name, false);
                Console.WriteLine(" will roll first!");
                Console.WriteLine("");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                Console.WriteLine("");
                if (!isDemoMode)
                    player1Roll = rnd.Next(1, 7);
                else
                {
                    bool inRange = false;
                    do
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Please enter dice value");
                        string input = Console.ReadLine();
                        int.TryParse(input, out player1Roll);
                        if (player1Roll > 0 && player1Roll < 7)
                            inRange = true;
                    } while (!inRange);
                }
                changeTextColorAddText(players[player1].color, players[player1].name, false);
                Console.WriteLine(" rolled a " + player1Roll + "!");
                Console.WriteLine("");
                changeTextColorAddText(players[player2].color, players[player2].name, false);
                Console.WriteLine(" will roll next!");
                Console.WriteLine("");
                Console.WriteLine("Press any key to continue.");
                Console.WriteLine("");
                Console.ReadKey();
                if (!isDemoMode)
                    player2Roll = rnd.Next(1, 7);
                else
                {
                    bool inRange = false;
                    do
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Please enter dice value");
                        string input = Console.ReadLine();
                        int.TryParse(input, out player2Roll);
                        if (player2Roll > 0 && player2Roll < 7)
                            inRange = true;
                    } while (!inRange);
                }
                changeTextColorAddText(players[player2].color, players[player2].name, false);
                Console.WriteLine(" rolled a " + player2Roll + "!");
                Console.WriteLine("");
                Console.ReadKey();
                if (player1Roll == player2Roll)
                {
                    rollNotEqual = false;
                }
                else rollNotEqual = true;
            } while (!rollNotEqual);

            bool player1Moved = false;
            bool player2Moved = false;
            int movedPlayer;

            if (player1Roll > player2Roll)
            {
                if (player1Roll == 2)
                {
                    if (players[player1].score > 0)
                    {
                        players[player1].score--;
                        players[player2].score++;
                        changeTextColorAddText(players[player1].color, players[player1].name, false);
                        Console.Write(" won a poor victory and loses 1 ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.Write("to ");
                        changeTextColorAddText(players[player2].color, players[player2].name, true);
                    }
                    else
                    {
                        Console.Write("No ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.WriteLine(" to steal.");
                    }
                    player1Moved = false;
                    player2Moved = false;
                }

                if (player1Roll == 3)
                {
                    if (players[player2].score > 0)
                    {
                        players[player1].score++;
                        players[player2].score--;
                        changeTextColorAddText(players[player1].color, players[player1].name, false);
                        Console.Write(" won a honourable victory and stole 1 ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.Write("from ");
                        changeTextColorAddText(players[player2].color, players[player2].name, true);
                    }
                    else
                    {
                        Console.Write("No ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.WriteLine(" to steal.");
                    }
                    player2Moved = true;


                }

                if (player1Roll == 4 || player1Roll == 5)
                {
                    if (players[player2].score > 0)
                    {
                        players[player1].score++;
                        players[player2].score--;
                        changeTextColorAddText(players[player1].color, players[player1].name, false);
                        Console.Write(" won a great victory and stole 1 ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.Write("from ");
                        changeTextColorAddText(players[player2].color, players[player2].name, true);
                    }
                    else
                    {
                        Console.Write("No ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.WriteLine(" to steal.");
                    }
                    player1Moved = true;

                }

                if (player1Roll == 6)
                {
                    if (players[player2].score > 1)
                    {
                        players[player1].score += 2;
                        players[player2].score -= 2;
                        changeTextColorAddText(players[player1].color, players[player1].name, false);
                        Console.Write(" won a glorious victory and stole 2 ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.Write("from ");
                        changeTextColorAddText(players[player2].color, players[player2].name, true);
                    }
                    else if (players[player2].score > 0)
                    {
                        players[player1].score += 1;
                        players[player2].score -= 1;
                        changeTextColorAddText(players[player1].color, players[player1].name, false);
                        Console.Write(" won a glorious victory and stole 1 ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.Write("from ");
                        changeTextColorAddText(players[player2].color, players[player2].name, true);
                    }
                    else
                    {
                        Console.Write("No ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.WriteLine(" to steal.");
                    }
                    player1Moved = true;
                }
                Console.ReadKey();
                Console.WriteLine("");
            }

            else if (player2Roll > player1Roll)
            {
                if (player2Roll == 2)
                {
                    if (players[player2].score > 0)
                    {
                        players[player2].score--;
                        players[player1].score++;
                        changeTextColorAddText(players[player2].color, players[player2].name, false);
                        Console.Write(" won a poor victory and loses 1 ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.Write("to ");
                        changeTextColorAddText(players[player1].color, players[player1].name, true);
                    }
                    else
                    {
                        Console.Write("No ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.WriteLine(" to steal.");
                    }
                    player1Moved = false;
                    player2Moved = false;
                }

                if (player2Roll == 3)
                {
                    if (players[player1].score > 0)
                    {
                        players[player2].score++;
                        players[player1].score--;
                        changeTextColorAddText(players[player2].color, players[player2].name, false);
                        Console.Write(" won a honourable victory and stole 1 ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.Write("from ");
                        changeTextColorAddText(players[player1].color, players[player1].name, true);
                    }
                    else
                    {
                        Console.Write("No ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.WriteLine(" to steal.");
                    }
                    player1Moved = true;

                }

                if (player2Roll == 4 || player2Roll == 5)
                {
                    if (players[player1].score > 0)
                    {
                        players[player2].score++;
                        players[player1].score--;
                        changeTextColorAddText(players[player2].color, players[player2].name, false);
                        Console.Write(" won a glorious victory and stole 1 ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.Write("from ");
                        changeTextColorAddText(players[player1].color, players[player1].name, true);
                    }
                    else
                    {
                        Console.Write("No ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.WriteLine(" to steal.");
                    }

                    player2Moved = true;
                }

                if (player2Roll == 6)
                {
                    if (players[player1].score > 1)
                    {
                        players[player2].score += 2;
                        players[player1].score -= 2;
                        changeTextColorAddText(players[player2].color, players[player2].name, false);
                        Console.Write(" won a glorious victory and stole 2 ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.Write("from ");
                        changeTextColorAddText(players[player1].color, players[player1].name, true);
                    }
                    else if (players[player1].score > 0)
                    {
                        players[player2].score += 1;
                        players[player2].score -= 1;
                        changeTextColorAddText(players[player2].color, players[player2].name, false);
                        Console.Write(" won a glorious victory and stole 1 ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.Write("from ");
                        changeTextColorAddText(players[player1].color, players[player1].name, true);
                    }
                    else
                    {
                        Console.Write("No ");
                        changeTextColorAddText(ConsoleColor.Yellow, "cheese ", false);
                        Console.WriteLine(" to steal.");
                    }
                    Console.WriteLine("");
                    Console.WriteLine("Press any key to continue");

                    player2Moved = true;
                }
                Console.ReadKey();
            }
            bool hasMoved;   // check that somebody has to move. 
            if (player1Moved)
            {
                movedPlayer = player1;
                hasMoved = true;
            }

            else if (player2Moved)
            {
                movedPlayer = player2;
                hasMoved = true;
            }
            else
            {
                movedPlayer = 0;
                hasMoved = false;
            }

            Console.WriteLine("");
            drawGrid();


            bool valid = false;
            int MoveCase = 0; // 1 for up, 2 for down, 3 left and 4 right. 
            if (players[movedPlayer].isHuman && hasMoved)
            {
                players[movedPlayer].currentCell.occupants[movedPlayer] = false;
                players[movedPlayer].currentCell.display = "B ";
                do
                {
                    changeTextColorAddText(players[movedPlayer].color, players[movedPlayer].name, false);
                    Console.WriteLine(", please enter a direction to move that has no ship in it.");

                    ConsoleKeyInfo key = Console.ReadKey();
                    if ((key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W) && findCell(players[movedPlayer].xPos - 1, players[movedPlayer].yPos).cellType < 2)
                    {
                        MoveCase = 1;
                        valid = true;
                    }
                    else if ((key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S) && findCell(players[movedPlayer].xPos + 1, players[movedPlayer].yPos).cellType < 2)
                    {
                        MoveCase = 2;
                        valid = true;
                    }
                    else if ((key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.A) && findCell(players[movedPlayer].xPos, players[movedPlayer].yPos - 1).cellType < 2)
                    {
                        MoveCase = 3;
                        valid = true;
                    }
                    else if ((key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.D) && findCell(players[movedPlayer].xPos, players[movedPlayer].yPos + 1).cellType < 2)
                    {
                        MoveCase = 4;
                        valid = true;
                    }
                } while (!valid);
            }
            else if (hasMoved)
            {
                players[movedPlayer].currentCell.occupants[movedPlayer] = false;
                do
                {

                    int flip = rnd.Next(0, 2);      // flip for x or y
                    if (flip == 0) // do x
                    {
                        flip = rnd.Next(0, 2); // flip again for pos or neg
                        if (flip == 0 && findCell(players[movedPlayer].xPos - 1, players[movedPlayer].yPos).cellType < 2)     // check cell type to ensure no other players occupy. 
                        {
                            valid = true;
                            MoveCase = 1;
                        }
                        else if (findCell(players[movedPlayer].xPos + 1, players[movedPlayer].yPos).cellType < 2)
                        {
                            valid = true;
                            MoveCase = 2;
                        }
                    }
                    else // do y
                    {
                        flip = rnd.Next(0, 2);
                        if (flip == 0 && findCell(players[movedPlayer].xPos, players[movedPlayer].yPos - 1).cellType < 2)
                        {
                            valid = true;
                            MoveCase = 3;
                        }
                        else if (findCell(players[movedPlayer].xPos, players[movedPlayer].yPos + 1).cellType < 2)
                        {
                            valid = true;
                            MoveCase = 4;
                        }
                    }
                } while (!valid);
            }
            else hasMoved = false;

            if (hasMoved)
            {
                switch (MoveCase)
                {
                    case 1:
                        Console.Clear();
                        players[movedPlayer].currentCell = findCell(players[movedPlayer].xPos - 1, players[movedPlayer].yPos);
                        players[movedPlayer].xPos -= 1;
                        changeTextColorAddText(players[movedPlayer].color, players[movedPlayer].name, false);
                        Console.WriteLine(" has moved 1 tile upwards");
                        break;
                    case 2:
                        Console.Clear();
                        players[movedPlayer].currentCell = findCell(players[movedPlayer].xPos + 1, players[movedPlayer].yPos);
                        players[movedPlayer].xPos += 1;
                        changeTextColorAddText(players[movedPlayer].color, players[movedPlayer].name, false);
                        Console.WriteLine(" has moved 1 tile downwards");
                        break;
                    case 3:
                        Console.Clear();
                        players[movedPlayer].currentCell = findCell(players[movedPlayer].xPos, players[movedPlayer].yPos - 1);
                        players[movedPlayer].yPos -= 1;
                        changeTextColorAddText(players[movedPlayer].color, players[movedPlayer].name, false);
                        Console.WriteLine(" has moved 1 tile to the left");
                        break;
                    case 4:
                        Console.Clear();
                        players[movedPlayer].currentCell = findCell(players[movedPlayer].xPos, players[movedPlayer].yPos + 1);
                        players[movedPlayer].yPos += 1;
                        changeTextColorAddText(players[movedPlayer].color, players[movedPlayer].name, false);
                        Console.WriteLine(" has moved 1 tile to the right");
                        break;
                }
                if (players[movedPlayer].currentCell.hasCheese)   // check new position for cheese.
                {
                    players[movedPlayer].score++;
                    players[movedPlayer].currentCell.hasCheese = false;
                    changeTextColorAddText(players[movedPlayer].color, players[movedPlayer].name, false);
                    Console.Write(" Gained ");
                    changeTextColorAddText(ConsoleColor.Yellow, "cheese!", true);
                }
                players[movedPlayer].currentCell.cellID = player1 + 2;
                players[movedPlayer].currentCell.occupants[movedPlayer] = true;
            }
        }




        public void handleHumanPlayersTurn(int player)
        {
            Console.WriteLine("");
            Console.WriteLine("Enter a direction (wasd or arrow keys).");
            Console.WriteLine("");
            ConsoleKeyInfo KeyPressInfo;
            bool badInput = true;

            do
            {
                KeyPressInfo = Console.ReadKey();
                Console.Clear();
                if (KeyPressInfo.Key == ConsoleKey.S || KeyPressInfo.Key == ConsoleKey.DownArrow)
                {
                    players[player].xPos += rollNumber;
                    if (players[player].xPos > 7)
                        players[player].xPos -= 8;
                    changeTextColorAddText(players[player].color, players[player].name, false);
                    Console.WriteLine(" moved " + rollNumber + " downwards.");
                    badInput = false;
                }

                else if (KeyPressInfo.Key == ConsoleKey.W || KeyPressInfo.Key == ConsoleKey.UpArrow)
                {
                    players[player].xPos -= rollNumber;
                    if (players[player].xPos < 0)
                        players[player].xPos += 8;
                    changeTextColorAddText(players[player].color, players[player].name, false);
                    Console.WriteLine(" moved " + rollNumber + " upwards.");
                    badInput = false;
                }

                else if (KeyPressInfo.Key == ConsoleKey.A || KeyPressInfo.Key == ConsoleKey.LeftArrow)
                {
                    players[player].yPos -= rollNumber;
                    if (players[player].yPos < 0)
                        players[player].yPos += 8;
                    changeTextColorAddText(players[player].color, players[player].name, false);
                    Console.WriteLine(" moved " + rollNumber + " to the left.");
                    badInput = false;
                }

                else if (KeyPressInfo.Key == ConsoleKey.D || KeyPressInfo.Key == ConsoleKey.RightArrow)
                {
                    players[player].yPos += rollNumber;
                    if (players[player].yPos > 7)
                        players[player].yPos -= 8;
                    changeTextColorAddText(players[player].color, players[player].name, false);
                    Console.WriteLine(" moved " + rollNumber + " to the right.");
                    badInput = false;
                }
            } while (badInput);
        }



        void drawGrid()
        {
            Program instance = new Program();
            int count = 0;
            for (int i = 0; i < 8; i++) // x axis.
            {
                for (int j = 0; j < 8; j++) // y axis.
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    int playersInCellCount = 0;
                    cells[count].display = "B ";


                    if (cells[count].hasCheese)
                    {
                        cells[count].cellType = 1;
                        cells[count].display = "C ";
                    }


                    for (int x = 0; x < 4; x++)
                    {
                        if (cells[count].occupants[x] == true)
                        {
                            cells[count].cellType = x + 2;  // check if any players are in a cell and if so applying the adjusted int value for each case.
                            cells[count].display = players[x].name[0].ToString() + " ";
                            playersInCellCount++;
                        }
                      
                    }


                    if (playersInCellCount > 1)
                    {
                        cells[count].display = "M ";
                        cells[count].cellType = 0;
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Console.BackgroundColor = instance.assignTextColor(cells[count].cellType);

                    //Console.ForegroundColor = cells[count].color; // Not needed.
                    Console.Write(cells[count].display);
                    if (j == 7)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine("");
                    }

                    count++;


                }
            }

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("");

            /*if (!isPreGame)
                checkRound();*/

        }



        void checkRound()
        {
            Program instance = new Program();
            if (cheeseRemaining == 0)
                gameOver();
            else
            {

                if (roundNumber > 4 || roundNumber == 1)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (players[i].score >= 6)
                            gameOver();

                    }
                    roundNumber = 1;
                    changeTextColorAddText(players[roundNumber - 1].color, players[roundNumber - 1].name + "'s ", false);
                    Console.WriteLine("turn. Press any key to continue");
                    Console.WriteLine("");
                    Console.ReadKey();
                    Console.Clear();
                    drawGrid();
                    changeTextColorAddText(players[roundNumber - 1].color, players[roundNumber - 1].name + "'s ", false);
                    Console.WriteLine("turn.");
                    Console.WriteLine("");
                    awaitInput();
                }
                else
                {
                    changeTextColorAddText(players[roundNumber - 1].color, players[roundNumber - 1].name + "'s ", false);
                    Console.WriteLine("turn. Press any key to continue");
                    Console.WriteLine("");
                    Console.ReadKey();
                    Console.Clear();
                    drawGrid();
                    changeTextColorAddText(players[roundNumber - 1].color, players[roundNumber - 1].name + "'s ", false);
                    Console.WriteLine("turn.");
                    Console.WriteLine("");

                    if (players[roundNumber - 1].isHuman)
                        awaitInput();
                    else instance.roll(roundNumber);

                }
            }

        }

        void awaitInput()
        {
            string input;
            Program instance = new Program();
            if (isFirstRound)
            {
                Console.WriteLine("Type:");
                Console.WriteLine("'Draw' to refresh the board.");
                Console.WriteLine("'Roll' to take your turn and roll the dice");        // on the first round of a game show controls.
                Console.WriteLine("'Score' to view the current scores");
                Console.WriteLine("'Instructions' to view the objective of the game and its rules.");
                Console.WriteLine("'Clear' to clear the screen");
                isFirstRound = false;
            }
            drawGrid();
            Console.WriteLine("");
            Console.WriteLine("Its your turn!");
            Console.WriteLine("Waiting for input from ");
            changeTextColorAddText(players[roundNumber - 1].color, players[roundNumber - 1].name + ".", true);
            Console.WriteLine("");
            input = Console.ReadLine();

            if (input.ToLower() == "draw" || input.ToLower() == "d")
            {
                instance.drawGrid();
                awaitInput();
            }

            else if (input.ToLower() == "score" || input.ToLower() == "s")
            {
                Console.WriteLine("");
                for (int i = 0; i < 4; i++)
                {
                    changeTextColorAddText(players[i].color, players[i].name, false);
                    Console.WriteLine(" : has a score of " + players[i].score + ".");
                }
                awaitInput();
            }

            else if (input.ToLower() == "roll" || input.ToLower() == "r")
            {
                Console.Clear();
                instance.drawGrid();
                instance.roll(roundNumber);
            }
            else if (input.ToLower() == "instructions" || input.ToLower() == "i" || input.ToLower().Contains("ins"))
            {
                Console.Write("The objective of this game is to collect ");
                changeTextColorAddText(ConsoleColor.Yellow, "Cheese!", true);
                Console.WriteLine("Four players take turns to roll the dice,");
                Console.WriteLine("you then choose a direction (up, down, left or right) to move the ");
                Console.WriteLine("distance equal to the number you rolled in said direction.");
                Console.WriteLine("If you should reach the end of the board, you will come out on the ");
                Console.WriteLine("other opposing side and continue travelling in the same direction for the");
                Console.WriteLine("remainder of your roll value.");
                Console.WriteLine("");
                Console.Write("Landing on ");
                changeTextColorAddText(ConsoleColor.Yellow, "CHEESE", true);
                Console.WriteLine("will add gain you score and remove the it from the board.");
                Console.WriteLine("Landing on another player will steal cheese from said player.");
                awaitInput();
            }
            else if (input.ToLower() == "help" || input.ToLower() == "h")
            {
                Console.WriteLine("Type:");
                Console.WriteLine("'Draw' to refresh the board.");
                Console.WriteLine("'Roll' to take your turn and roll the dice");
                Console.WriteLine("'Score' to view the current scores");
                Console.WriteLine("'Instructions' to view the objective of the game and its rules.");
                Console.WriteLine("Clear to clear the screen");
                awaitInput();

            }
            else if (input.ToLower() == "clear" || input.ToLower() == "c")
            {
                Console.Clear();
                awaitInput();
            }

            else
            {
                Console.WriteLine("Invalid entry. Type help to see commands.");
                awaitInput();
            }
        }

        void gameOver()
        {
            Program instance = new Program();
            Console.WriteLine("");
            Console.WriteLine("Game Over.");
            Console.WriteLine("");

            //players.OrderByDescending(x => x.score);
            Array.Sort(players, (x, y) => y.score.CompareTo(x.score));

            changeTextColorAddText(players[0].color, players[0].name, false);
            Console.WriteLine(" Won the game with a score of " + players[0].score);

            for (int i = 1; i < players.Length; i++)
            {
                changeTextColorAddText(players[i].color, players[i].name, false);
                Console.WriteLine(" scored " + players[i].score);
            }

            Console.WriteLine("");
            Console.Write("Press enter to restart or any other key to exit.");

            ConsoleKeyInfo key = new ConsoleKeyInfo();
            key = Console.ReadKey();

            if (key.Key == ConsoleKey.Enter)
                instance.initialiseGame();
            else Environment.Exit(0);

        }
    }
}
