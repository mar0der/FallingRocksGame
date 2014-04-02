/*
Implement the "Falling Rocks" game in the text console. A small dwarf stays at the bottom of the screen and can move left and right (by the arrows keys). A number of rocks of different sizes and forms constantly fall down and you need to avoid a crash.
Rocks are the symbols ^, @, *, &, +, %, $, #, !, ., ;, - distributed with appropriate density. The dwarf is (O). Ensure a constant game speed by Thread.Sleep(150).
Implement collision detection and scoring system
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FallingRocksGame
{
    class FallingRocksGame
    {
        struct Position
        {
            public int X, Y;
            public Position(int x = 0, int y = 0)
            {
                this.X = x;
                this.Y = y;
            }
        }
        struct Stones
        {
            public int X, Y, Speed, Type, Colour, Delay;
            public Stones(int x = 0, int y = 0, int speed = 3, int type = 0, int colour = 0, int delay = 0)
            {
                this.X = x;
                this.Y = y;
                this.Speed = speed;
                this.Type = type;
                this.Colour = colour;
                this.Delay = delay;
            }

        }
        private static int lives = 5;
        private static int score = 0;
        private static int speed = 30; // lower is faster
        private static int level = 0;
        private static int playFieldWidth = 78;
        private static int playFieldHeight = 30;
        private static int statusBarWidth = 10;
        private static Position dworfCurrentPosition = new Position();
        private static int currentDirection = 0; // 0 = stopped, 1 = left , 2 = right
        private static Random randomGenerator = new Random();
        private static Position[] moveDirections = new Position[]
            {
                new Position(0, Console.WindowHeight-1), // stopped
                new Position(-1, Console.WindowHeight-1), // left
                new Position(1, Console.WindowHeight-1), // right
            };
        //stones
        private static int minStones = 15;
        private static int currentStones = 15;
        private static int maxStones = 100;
        private static int maxStonesColors = 5;
        private static int maxStonesShapes = 5;
        private static int maxStonesSpeed = 5;
        private static Stones[] stonesArray = new Stones[maxStones];

        static void Main()
        {
            Console.Title = "Falling Rocks by Petar Petkov";
            //setting the area to play
            SetPlaygoundSize(playFieldWidth, playFieldHeight);
            //infinite loop
            bool gameOn = true;
            //Setting the default position
            dworfCurrentPosition.X = Console.WindowWidth / 2 -1;
            dworfCurrentPosition.Y = Console.WindowHeight - 1;
            PrintInfoScreen();
            PrintDworf(dworfCurrentPosition);
            while (gameOn)
            {

                // Read user key
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo pressedKey = Console.ReadKey();
                    if (pressedKey.Key == ConsoleKey.DownArrow)
                        currentDirection = 0;
                    if (pressedKey.Key == ConsoleKey.LeftArrow)
                        currentDirection = 1;
                    if (pressedKey.Key == ConsoleKey.RightArrow)
                        currentDirection = 2;
                    if (pressedKey.Key == ConsoleKey.Q)
                        gameOn = false;
                }
                MoveDworf();
                RandomizeStones();
                PrintStones();
                PrintBorders();
                UpdateLevel();
                PrintLabels();
                if (lives == 0)
                {
                    gameOn = false;
                }
                Thread.Sleep(speed);
            }
            Console.Clear();
            if (lives == 0)
            {
                Console.Clear();
                Console.SetCursorPosition( (Console.WindowWidth / 2) - 15, Console.WindowHeight / 2 - 5);
                Console.WriteLine("YOUR SCORE IS:{0}", score);
                Console.SetCursorPosition((Console.WindowWidth / 2) - 15, Console.WindowHeight / 2 - 4);
                Console.WriteLine("GAME OVER");
                Console.SetCursorPosition((Console.WindowWidth / 2) - 15, Console.WindowHeight / 2 - 3);
                Console.WriteLine("Pres any key to quit the game!");
                Console.ReadLine();
                Environment.Exit(0);
            }
            else
            {
                Console.Clear();
                Console.SetCursorPosition((Console.WindowWidth / 2) - 15, Console.WindowHeight / 2 - 5);
                Console.WriteLine("Pres any key to quit the game!");
                Console.ReadLine();
                Environment.Exit(0);
            }
            
        }

// Printing and game logic
        static void PrintInfoScreen()
        {
            string info = 
@"RULES OF THE GAME 
You can move the dworf left and right with the keyboard
arrows. If you want to stop it use down arrow. To quit 
the game please pres Q.  
Press any key to start the game!";
            Console.Clear();
//some magic numbers here but i dont have time i have to submit the homework in 1 hours
            Console.SetCursorPosition(15, 10);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(info);
            Console.CursorVisible = false;
            Console.ReadLine();
            Console.Clear();
        }
        static void SetPlaygoundSize(int width, int height)
        {
            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, height);
            Console.CursorVisible = false;

        }
        static void PrintBorders()
        {
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.SetCursorPosition(Console.WindowWidth - statusBarWidth, i);
                Console.Write("|");
            }
        }
        static void PrintLabels()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(Console.WindowWidth - statusBarWidth + 1, 1);
            Console.WriteLine("Lives:{0}", lives);
            Console.SetCursorPosition(Console.WindowWidth - statusBarWidth + 1, 2);
            Console.WriteLine("Score:{0}", score);
            Console.SetCursorPosition(Console.WindowWidth - statusBarWidth + 1, 3);
            Console.WriteLine("Level:{0}", level);
            Console.SetCursorPosition(Console.WindowWidth - statusBarWidth + 1, 4);
            Console.WriteLine("(Q)uit");
        }
        static void UpdateLevel()
        {
            if (currentStones <= maxStones)
            {
                level = score / 10;
                currentStones = minStones + level;
            }
        }
        static void ColisionDetection(Stones stonePosition)
        {
            if (dworfCurrentPosition.X <= stonePosition.X && stonePosition.X <= dworfCurrentPosition.X + 2 && stonePosition.Y == dworfCurrentPosition.Y)
            {
                lives--;
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Red;
                Thread.Sleep(100);
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Thread.Sleep(100);
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Red;
                Thread.Sleep(100);
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Thread.Sleep(100);
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Red;
                Thread.Sleep(100);
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Black;
                Thread.Sleep(100);
                Console.Clear();
            }

        }
//Dworf stuff
        static void PrintDworf(Position p)
        {
            //clean the old dworf
            if (currentDirection == 1)
            {
                Console.SetCursorPosition(p.X+3, p.Y);
                Console.ForegroundColor = Console.BackgroundColor;
                Console.Write(" ");
            }
            else if (currentDirection == 2 && p.X >0)
            {
                Console.SetCursorPosition(p.X - 1, p.Y);
                Console.ForegroundColor = Console.BackgroundColor;
                Console.Write(" ");
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.SetCursorPosition(p.X, p.Y);
            Console.Write("<@>");
            
        }

        static void MoveDworf()
        {
            if ((dworfCurrentPosition.X == Console.WindowWidth - statusBarWidth - 3 && currentDirection == 2) || (dworfCurrentPosition.X == 0 && currentDirection == 1))
            {
                currentDirection = 0;
            }
            dworfCurrentPosition.X += (moveDirections[currentDirection]).X;
            PrintDworf(dworfCurrentPosition);
        }
//Stones stuff
        static void PrintStones()
        {
            for (int i = 0; i < currentStones; i++)
            {
                Console.SetCursorPosition(stonesArray[i].X, stonesArray[i].Y);
                Console.ForegroundColor = GetStoneColour(stonesArray[i].Colour);
                Console.Write(GetStoneType(stonesArray[i].Type));
                //deleating the old position to create motion effect
                if (stonesArray[i].Y > 0)
                {
                    Console.SetCursorPosition(stonesArray[i].X, stonesArray[i].Y - 1);
                    Console.ForegroundColor = Console.BackgroundColor;
                    Console.Write(" ");
                }

                if (stonesArray[i].Y < Console.WindowHeight-1)
                {
                    if (stonesArray[i].Delay < GetStoneSpeed(stonesArray[i].Speed))
                    {
                        stonesArray[i].Delay++;
                    }
                    else
                    {
                        stonesArray[i].Delay = 0;
                        stonesArray[i].Y += 1;
                        ColisionDetection(stonesArray[i]);
                    }
                }
                else
                {
                    Console.SetCursorPosition(stonesArray[i].X, stonesArray[i].Y);
                    Console.ForegroundColor = Console.BackgroundColor;
                    Console.Write(" ");
                    stonesArray[i].X = 0;
                    stonesArray[i].Y = 0;
                    score++;
                    break;
                }
            }
        }

        static void RandomizeStones()
        {
            for (int i = 0; i < currentStones; i++)
            {
                if(stonesArray[i].X == 0 && stonesArray[i].Y == 0)
                {
                    stonesArray[i].X = randomGenerator.Next(Console.WindowWidth - statusBarWidth);
                    stonesArray[i].Speed = randomGenerator.Next(maxStonesSpeed-1)+1; // it could be 1 to maxSpeed
                    stonesArray[i].Type = randomGenerator.Next(maxStonesShapes); //0 to max shapes
                    stonesArray[i].Colour = randomGenerator.Next(maxStonesColors); //0 to max colors
                }
            }
        }

        private static char GetStoneType(int type = 0)
        {
            char returnedType;
            switch (type)
            {
                case 0:
                    returnedType = '!';
                    break;
                case 1:
                    returnedType = '#';
                    break;
                case 2:
                    returnedType = '$';
                    break;
                case 3:
                    returnedType = '&';
                    break;
                case 4:
                    returnedType = '*';
                    break;
                case 5:
                    returnedType = '+';
                    break;
                default:
                    returnedType = 'O';
                    break;
            }
            return returnedType;
        }

        private static ConsoleColor GetStoneColour(int type = 0)
        {
            ConsoleColor returnedColour;
            switch (type)
            {
                case 0:
                    returnedColour = ConsoleColor.Blue;
                    break;
                case 1:
                    returnedColour = ConsoleColor.Cyan;
                    break;
                case 2:
                    returnedColour = ConsoleColor.DarkMagenta;
                    break;
                case 3:
                    returnedColour = ConsoleColor.DarkYellow;
                    break;
                case 4:
                    returnedColour = ConsoleColor.Red;
                    break;
                case 5:
                    returnedColour = ConsoleColor.Yellow;
                    break;
                default:
                    returnedColour = ConsoleColor.White;
                    break;
            }
            return returnedColour;
        }

        private static int GetStoneSpeed(int speed = 0)
        {
            int returnedSpeed;
            switch (speed)
            {
                case 0:
                    returnedSpeed = 2;
                    break;
                case 1:
                    returnedSpeed = 5;
                    break;
                case 2:
                    returnedSpeed = 7;
                    break;
                case 3:
                    returnedSpeed = 9;
                    break;
                case 4:
                    returnedSpeed = 15;
                    break;
                case 5:
                    returnedSpeed = 17;
                    break;
                default:
                    returnedSpeed = 40;
                    break;
            }
            return returnedSpeed;
        }
        
    }
}
