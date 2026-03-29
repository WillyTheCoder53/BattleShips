class BattleShipGame
{
    static Random random = new Random(); // Global random int generator

    static int lost = 0; // Global Variable used to measure amount of ships the computer has sunk
    static int sunk = 0; // Global Variable used to measure amount of ships the player has sunk

    static int length = 0; // Global Variable used to measure the length of the ship
    static int shipColour = 0; // Global variable used to make each ship a different colour

    static string[,] YourGrid = new string[8, 8]; // Global battleship grid of the player
    static string[,] EnemyGrid = new string[8, 8]; // Global battleship grid of the computer

    static string ComputerAttackMessage = @"";
    static string PlayerAttackMessage = @"";

    static bool DebuggingMode; // Global variable used to determine if the player can see the computer's ships


    static void Main()
    {
        Console.WriteLine("Welcome to battle ships.\n");

        InitializeGrid(YourGrid); // Fills the grid with empty spaces
        InitializeGrid(EnemyGrid); // Fills the grid with empty spaces

        PrintMenu();
        MenuInput();

        static void PrintMenu()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"Here is the menu:
-----------------------------
| 1. Play a new game.       |
| 2. Resume a game.         |
| 3. Read the instructions. |
| 4. Quit the program.      |
-----------------------------
"); // Verbatim identifier
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void MenuInput()
        {
            int number = Convert.ToInt32(Console.ReadLine()); // Conversion

            switch (number) // Selection
            {
                case 1: PlayGame(); break;
                case 2: ResumeGame(); break;
                case 3: Instructions(); break;
                case 4: Environment.Exit(0); break;
                default: Console.WriteLine("You did not enter a valid option, try again"); MenuInput(); break; // Recursion if it fails
            }
        }

        static void InitializeGrid(string[,] grid)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    grid[i, j] = " "; // Fills the grid with empty spaces
                }
            }
        }

        static void PlayGame()
        {
            IsAGameAlreadySaved(); // Checks if there is already a game saved

            Console.WriteLine("Do you want to enter debugging mode? (This allows you to see Computer's ships) (y/n)");

            DebuggingMode = Console.ReadLine().ToLower() == "y"; // If the player enters y, debugging mode is activated
            if (DebuggingMode)
            {
                Console.WriteLine("Debugging mode activated");
                Thread.Sleep(2000);
            }

            Console.Clear(); // Clears the text in console window

            PrintScreen(YourGrid); // Displays the two battleship screens

            PlacePlayerShips(); // Lets the player place their ships

            PlaceEnemyShips(); // Randomally places the computer's ships

            Console.Clear();

            PrintScreen(YourGrid); // Displays updated player screens

            Console.WriteLine("The computer has placed its ships");

            AttackShips();
        }

        static void PrintSquare(string squareValue, bool DebugMode) // Square value is from fillscreen, it is the value of the square at that coordinate. Either unchecked, hit or miss.
        {
            Console.BackgroundColor = ConsoleColor.White;

            if (squareValue == "M") // If coordinate = Miss
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else if (squareValue.StartsWith("X")) // If coordinate = Hit
            {
                int shipId = int.Parse(squareValue.Substring(1)); // Gets the ship id from the coordinate

                Console.ForegroundColor = ConsoleColor.Red;

                // Sets the colour of the hit ship to the same colour it was before

                switch (shipId % 5)
                {
                    case 0: Console.BackgroundColor = ConsoleColor.DarkGreen; break; // Final ship colour
                    case 1: Console.BackgroundColor = ConsoleColor.Blue; break; // First ship colour
                    case 2: Console.BackgroundColor = ConsoleColor.DarkBlue; break; // Second ship colour
                    case 3: Console.BackgroundColor = ConsoleColor.Magenta; break; // Third ship colour
                    case 4: Console.BackgroundColor = ConsoleColor.DarkCyan; break; // Fourth ship colour
                }
            }
            else if (squareValue.StartsWith("S")) // If coordinate = Ship
            {
                if (DebugMode) // Allows the player to see the ships
                {

                    int shipId = int.Parse(squareValue.Substring(1)); // Gets the ship id from the coordinate

                    switch (shipId % 5)
                    {
                        case 0: Console.BackgroundColor = ConsoleColor.DarkGreen; break; // Final ship colour
                        case 1: Console.BackgroundColor = ConsoleColor.Blue; break; // First ship colour
                        case 2: Console.BackgroundColor = ConsoleColor.DarkBlue; break; // Second ship colour
                        case 3: Console.BackgroundColor = ConsoleColor.Magenta; break; // Third ship colour
                        case 4: Console.BackgroundColor = ConsoleColor.DarkCyan; break; // Fourth ship colour
                    }
                }
            }
            Console.Write(" ■ ");
            Console.ResetColor();
            Console.Write(" ");

        }

        static void PrintScreen(string[,] grid)
        {
            Console.WriteLine("            YOUR SHIPS                                     ENEMY SHIPS");
            Console.WriteLine("------------------------------------          ------------------------------------");
            Console.WriteLine("  | 1   2   3   4   5   6   7   8  |            | 1   2   3   4   5   6   7   8  |");
            Console.WriteLine("------------------------------------          ------------------------------------");
            int count = 0;
            int column = 0;

            for (int row = 0; row < 8; row++)
            {
                // Print the grid entered as a paramter, usually your grid.
                Console.Write($"|{row + 1}|");
                for (int col = 0; col < 8; col++)
                {
                    PrintSquare(grid[row, col], true); // True allows the player to see their own ship
                }
                Console.Write("|");

                // Print ENEMY grid
                Console.Write($"          |{row + 1}|");
                for (int col = 0; col < 8; col++)
                {
                    PrintSquare(EnemyGrid[row, col], DebuggingMode); // Allows the user to choose to see the enemy's ship or not (for debugging)
                }
                Console.WriteLine("|\n");

            }
            Console.WriteLine("----------------------------------------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"{lost} of your ships have been decimated.");
            Console.WriteLine($"You have sunk {sunk} ships.");

            Console.ResetColor();
            Console.WriteLine("----------------------------------------------------------------------------------");

        }

        static int RandomCoordinate(int limit1, int limit2)
        {
            Random random = new Random();
            return random.Next(limit1, limit2 -1); // Gives a random number from limit1 to limit2

        }

        static bool IsValidPlacement(string[,] Grid, int x1, int y1, int x2, int y2, int length)
        {
            if (Math.Abs(y1 - y2) + 1 != length && Math.Abs(x1 - x2) + 1 != length) // Checks if the ship has been placed with the right length
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"The ship is {length} long!");
                Console.ResetColor();
                return false;
            }

            else if (x1 != x2 && y1 != y2) // Checks if the ship has been placed diagonally
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Ships cannot be placed diagonally!");
                Console.ResetColor();
                return false;
            }

            int startingX = Math.Min(x1, x2); // = the smaller of the two x values
            int endX = Math.Max(x1, x2); // = the bigger of the two x values
            int startingY = Math.Min(y1, y2); // = the smaller of the two y values
            int endY = Math.Max(y1, y2); // = the bigger of the two y values

            for (int i = startingY; i <= endY; i++)
            {
                for (int j = startingX; j <= endX; j++)
                {
                    if (Grid[i, j].StartsWith("S"))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Ships cannot overlap!");
                        Console.ResetColor();
                        return false;
                    }
                }
            }

            return true; // Only executes if everything else was succesful

        }

        static void PlaceYourShips(string name, int length, int shipId)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine($"Place your {name} ({length} long). Enter start coordinate e.g. 55");
                    string start = Console.ReadLine();
                    int x1 = (int)char.GetNumericValue(start[0]) - 1; // -1 To account for zero based indexing 
                    int y1 = (int)char.GetNumericValue(start[1]) - 1; // -1 To account for zero based indexing

                    string[,] tempGrid = (string[,])YourGrid.Clone(); // Makes a temporary copy

                    if (YourGrid[y1, x1].StartsWith("S")) // If there is already a ship there
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("There is already a ship there");
                        Console.ResetColor();
                        continue;
                    }
                    else
                    {
                        tempGrid[y1, x1] = $"S{shipId}"; // Temporarily marks the starting position and acts as input validation

                        Console.Clear();

                        PrintScreen(tempGrid); // Marks the starting coordinate for the ship, it will get removed if the next coorindate is invalid. 
                    }


                    Console.WriteLine($"Enter the end coordinate for your {name} ({length} long).");
                    string end = Console.ReadLine();
                    int x2 = (int)char.GetNumericValue(end[0]) - 1;
                    int y2 = (int)char.GetNumericValue(end[1]) - 1;

                    string test2 = YourGrid[y2, x2]; // Validates input

                    Console.ForegroundColor = ConsoleColor.Yellow;

                    if (IsValidPlacement(YourGrid, x1, y1, x2, y2, length))
                    {
                        Console.ResetColor();

                        int startX = Math.Min(x1, x2); int endX = Math.Max(x1, x2);
                        int startY = Math.Min(y1, y2); int endY = Math.Max(y1, y2);

                        // Fills the squares and the gaps between them with part of a ship. 
                        for (int i = startY; i <= endY; i++)
                        {
                            for (int j = startX; j <= endX; j++)
                            {
                                YourGrid[i, j] = $"S{shipId}"; // Assigns the same id to each cell of the same ship
                            }
                        }

                        Console.Clear();

                        PrintScreen(YourGrid); // Prints screen with a new ship
                        break;
                    }
                    else
                    {
                        Console.ResetColor();
                        tempGrid[y1, x1] = $"S{shipId}"; // Resets the temporary grid 
                        Thread.Sleep(2000);

                        Console.Clear();
                        PrintScreen(YourGrid); // Prints screen with same amount of ships
                    }

                }
                catch (IndexOutOfRangeException) // Out of grid
                {
                    Console.WriteLine("You did not enter a valid coordinate");
                }
            }
        }

        static void PlacePlayerShips()
        {
            PlaceYourShips("Aircraft Carrier", 5, 1); // Ship name, length, ship id. The id is used for the colour of th
            PlaceYourShips("Battleship", 4, 2);
            PlaceYourShips("Cruiser", 3, 3);
            PlaceYourShips("Submarine", 3, 4);
            PlaceYourShips("Destroyer", 2, 5);
        }

        static void PlaceEnemyShips()
        {
            int id = 1; // Ship id



            int[] sizes = { 5, 4, 3, 3, 2 }; // Length of the battleships

            foreach (int size in sizes) // Places every sized ship, one at a time.
            {
                // Keep randomizing coordinates for each ship, until they meet the requirements.
                while (true)
                {

                    int x1 = RandomCoordinate(1,8) - 1;

                    int y1 = RandomCoordinate(1,8) - 1;

                    bool isHorizontal = (random.Next(2) == 1); // Coinflips to see if the ship will be horizontal or vertical

                    int x2, y2;

                    if (isHorizontal)
                    {
                        y2 = y1; // The same row (horizontal placement) as the starting coordinate. Only x2 coordinate will need to be randomised.


                        if (x1 + size <= 8) // Checks if the ship can fit to the right
                        {
                            x2 = x1 + (size -1);
                        }
                        else // Put the ship on the left
                        {
                            x2 = x1 - (size - 1);
                        }
                    }
                    else
                    {
                        x2 = x1; // The same colum (vertical placement) as the starting coordinate. Only x2 coordinate will need to be randomised.

                        if (y1 + size <= 8) // Checks if the ship can be placed downwards
                        {
                            y2 = y1 + (size - 1);
                        }
                        else // Place it upwards
                        {
                            y2 = y1 - (size - 1);
                        }
                    }

                    // Attempt to place the ship using the randomised coordinates

                    if (IsValidPlacement(EnemyGrid, x1, y1, x2, y2, size)) // Checks if the coordinates meet the requirements
                    {

                        int startX = Math.Min(x1, x2); int startY = Math.Min(y1, y2);
                        int endX = Math.Max(x1, x2); int endY = Math.Max(y1, y2);

                        // Fills the squares and the gaps between them with part of a ship. 
                        for (int i = startY; i <= endY && i < 8; i++)
                        {
                            for (int j = startX; j <= endX && j < 8; j++)
                            {
                                EnemyGrid[i, j] = $"S{id}"; // S = ship
                            }
                        }
                        break; // Ship placed succesfully, moves on to next ship
                    }
                }
                id++; // Increments the ship id
            }
        }

        static bool IsShipSunk(string Id, string[,] grid, ref string attackMessage) // Passing by reference so the string is changed instead of a new string being made
        {
            int HealthPoints = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (grid[i,j] == $"S{Id}" )
                    {
                        HealthPoints++;
                    }
                }
            }

            if (HealthPoints == 0)
            {
                attackMessage += "That ship has been sunk!!!";
                return true;
            }
            else
            {
                attackMessage += $"That ship has {HealthPoints}HP remaining!";
                return false;
            }
        }

        static void AttackShips()
        {
            int turn = 0; // Used to calculate whose turn it is

            while (lost < 5 && sunk < 5)
            {
                try
                {
                    if (turn % 2 == 0) // If it is the players turn
                    {
                        PlayerAttackMessage = @""; // Only clears the player's attack 

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\nEnter the coordinates of where you want to attack. E.g. 33");
                        Console.ResetColor();

                        string coordinates = Console.ReadLine();
                        int XPlayerAttack = (int)char.GetNumericValue(coordinates[0]) - 1; // -1 To account for zero based indexing 
                        int YPlayerAttack = (int)char.GetNumericValue(coordinates[1]) - 1; // -1 To account for zero based indexing

                        if (EnemyGrid[YPlayerAttack, XPlayerAttack].StartsWith("S"))
                        {
                            string id = EnemyGrid[YPlayerAttack, XPlayerAttack].Substring(1); // Gets the ship Id from the coordinate
                            EnemyGrid[YPlayerAttack, XPlayerAttack] = "X" + id;

                            PlayerAttackMessage += $"You attacked {XPlayerAttack +1}, {YPlayerAttack +1} and hit a ship!\n";

                            if (IsShipSunk(id, EnemyGrid, ref PlayerAttackMessage)) // If the ship has no health remaining also passed by reference
                            {
                                sunk++; // Increments amount of ships sunk
                            }
                        }
                        // If the player has already attacked that coordinate
                        else if (EnemyGrid[YPlayerAttack, XPlayerAttack].StartsWith("X") || EnemyGrid[YPlayerAttack, XPlayerAttack] == "M")
                        {
                            Console.WriteLine("You have already attacked that coordinate... You can retry in 3 seconds.");
                            Thread.Sleep(3000);
                            turn--; // Makes the player attack again
                        }
                        else 
                        { 
                            EnemyGrid[YPlayerAttack, XPlayerAttack] = "M"; 

                            PlayerAttackMessage += $"You attacked {XPlayerAttack + 1}, {YPlayerAttack + 1} and missed...";
                        }
                    }
                    else // If it is the computer's turn
                    {
                        ComputerAttackMessage = @""; // Only clears the Computer's message

                        int XComputerAttack = RandomCoordinate(1,8) - 1; 
                        int YComputerAttack = RandomCoordinate(1,8) - 1;

                        if (YourGrid[YComputerAttack, XComputerAttack].StartsWith("S")) // If that coordinate has a ship
                        {

                            string id = YourGrid[YComputerAttack, XComputerAttack].Substring(1); // Gets the ship Id from the coordinate
                            YourGrid[YComputerAttack, XComputerAttack] = "X" + id; // Sets the coordinate computer attacked to hit

                            ComputerAttackMessage += $"The computer attacked {XComputerAttack +1}, {YComputerAttack +1} and hit your ship!\n";

                            if (IsShipSunk(id, YourGrid, ref ComputerAttackMessage)) // If the ship has no health remaining also passed by reference
                            {
                                lost++; // Increments amount of the player's ships sunk
                            }
                        }
                        // If the computer has already attacked that coordinate
                        else if (YourGrid[YComputerAttack, XComputerAttack].StartsWith("X") || YourGrid[YComputerAttack, XComputerAttack] == "M")
                        {
                            turn--; // Makes the computer attack again
                        }

                        else 
                        {
                            YourGrid[YComputerAttack, XComputerAttack] = "M";

                            ComputerAttackMessage += $"The computer attacked {XComputerAttack + 1},{YComputerAttack + 1} and missed...";
                        }
                    }

                    Console.Clear();
                    PrintScreen(YourGrid); // Prints updated screen

                    Console.WriteLine(PlayerAttackMessage);
                    Console.WriteLine(); // To make a space between the two messages
                    Console.WriteLine(ComputerAttackMessage);
                    turn++;
                    SaveGame(); // Autosaves the game
                }
                catch (FormatException)
                {
                    Console.WriteLine("You did not enter a number");
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("You did not enter a valid coordinate");
                }
            }
            // If someone has won the game

            if (lost > sunk && lost == 5)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nAll of your ships have been sunk... you lose.");
                Console.ResetColor();
            }
            else if (sunk > lost && sunk == 5)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nYou have decimated all of the computer's ships. Well done!");
                Console.ResetColor();
            }
        }

        static void SaveGame()
        {

            // Writes YourGrid to file
            using(StreamWriter writer = new StreamWriter("YourGrid.txt"))
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        writer.WriteLine(YourGrid[i, j]);
                    }
                }

            } // Closes writer

            // Writes EnemyGrid to file
            using (StreamWriter writer = new StreamWriter("EnemyGrid.txt"))
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        writer.WriteLine(EnemyGrid[i, j]);
                    }
                }

            } // Closes writer

            using (StreamWriter writer = new StreamWriter("GameStats.txt"))
            {
                writer.WriteLine(lost); // Amount of ships the player has lost
                writer.WriteLine(sunk); // How many of the player's have been sunk

            } // Closes writer

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nGame autosaved to files...");
            Console.ResetColor();
        }

        static void LoadGame()
        {

            // Loads the player's saved grid into the YourGrid Array
            using (StreamReader reader = new StreamReader("YourGrid.txt"))
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        YourGrid[i, j] = reader.ReadLine();
                    }
                }
            } // Closes writer

            // Loads the Computer's saved grid into the EnemyGrid Array
            using (StreamReader reader = new StreamReader("EnemyGrid.txt"))
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        EnemyGrid[i, j] = reader.ReadLine();
                    }
                }
            } // Closes Writer

            using (StreamReader reader = new StreamReader("GameStats.txt"))
            {
                lost = Convert.ToInt32(reader.ReadLine());
                sunk = Convert.ToInt32(reader.ReadLine());
            }


        }

        static void ResumeGame()
        {
            LoadGame(); // Loads the game from the files

            Console.Clear();
            PrintScreen(YourGrid); // Prints the screen with the loaded grids

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The game has been loaded!");
            Console.ResetColor();

            AttackShips(); // Resumes the game
        }

        static void IsAGameAlreadySaved()
        {
            if (new FileInfo("YourGrid.txt").Length > 0)
            {
                Console.WriteLine("There is already a game saved, are you sure you want to override it? y/n");
                string shouldOverride = Console.ReadLine().ToLower();

                if (shouldOverride == "n")
                {
                    Console.WriteLine("Returning to menu...");
                    PrintMenu();
                    MenuInput();
                }
                else if (shouldOverride == "y")
                {
                    Console.WriteLine("Overriding game...");
                }
                else
                {
                    Console.WriteLine("You did not enter a valid option, try again");
                    IsAGameAlreadySaved(); // Uses recursion to get valid input
                }
            }
        }

        static void Instructions()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;

            Console.WriteLine();
            Console.WriteLine(@"Objective: sink all 5 of the computer's randomally generated ships, before it sinks yours!

Sinking a ship:
When you hit a ship, that coordinate will be marked red in your menu.
Once all parts of a ship has been hit, it will be sunk.

Misses:
When you miss a ship, that coordinate will be marked blue in your menu.

Rules for placing ships:
1. You cannot place ships diagonally, they must be horizontal or vertical.
2. Ships cannot overlap.
3. The orientation or position of ships cannot be changed during the game.");

            Console.ResetColor();
        }
    }
}