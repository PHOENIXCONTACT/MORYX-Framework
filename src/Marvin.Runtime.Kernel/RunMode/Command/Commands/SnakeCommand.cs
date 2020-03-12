// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Marvin.Container;
using Marvin.Runtime.Kernel.Additionals;

namespace Marvin.Runtime.Kernel
{
    internal enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    [Registration(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class SnakeCommand : ICommandHandler
    {
        private const string SnakeIcon = "*";
        private const string FoodIcon = "#";

        // Snake fields
        private long _color = 0;
        private UiElement _nextFood;
        private readonly List<UiElement> _snake = new List<UiElement>();
        private Direction _currentDirection;

        // Infrastructure
        private readonly Random _rnd = new Random();
        private Timer _cycleTimer;
        private ManualResetEvent _gameOver;

        // Flag if currently reading key
        private bool _reading = false;

        // 3 stage input barrier
        private readonly StageCommand _stageCommand = new StageCommand("easter", "egg", "snake");
        /// <summary>
        /// Check for correct input sequence
        /// </summary>
        public bool CanHandle(string command)
        {
            // 3 stage input for better hiding
            return _stageCommand.HandleChain(command);
        }

        /// <summary>
        /// Handle the entered command
        /// </summary>
        public void Handle(string[] fullCommand)
        {
            Console.Clear();
            _currentDirection = Direction.Right;
            _snake.Clear();

            // Create snake
            var screenLeft = Console.WindowWidth / 2 - 2;
            var screenTop = Console.WindowHeight / 2;
            for (var i = 0; i < 5; i++)
            {
                var element = CreateSnakeElement(screenLeft + i, screenTop);
                element.Draw();
                _snake.Add(element);
            }

            // Create food
            _nextFood = GenerateFood();

            // Start snake loop
            _gameOver = new ManualResetEvent(false);
            var speed = fullCommand.Length >= 2 ? int.Parse(fullCommand[1]) : 100;
            _cycleTimer = new Timer(RunLoop, null, 0, speed);
            _gameOver.WaitOne();

            // Game over
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Game over!");
            Console.WriteLine("Your score: " + CalculateScore(200 / speed));
            Console.WriteLine();
            Console.WriteLine("Press any key twice to return!");
            Console.ReadLine();
            Console.Clear();
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
        }

        /// <summary>
        /// Loop executed in timer
        /// </summary>
        /// <param name="unused"></param>
        private void RunLoop(object unused)
        {
            try
            {
                MoveSnake();
                if (!_reading)
                    ThreadPool.QueueUserWorkItem(ReadInput);
            }
            catch
            {
                EndGame();
            }
        }

        /// <summary>
        /// Read input in new thread
        /// </summary>
        private void ReadInput(object unused)
        {
            _reading = true;

            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Escape)
                EndGame();
            else if (key.Key == ConsoleKey.UpArrow && _currentDirection != Direction.Down)
                _currentDirection = Direction.Up;
            else if (key.Key == ConsoleKey.RightArrow && _currentDirection != Direction.Left)
                _currentDirection = Direction.Right;
            else if (key.Key == ConsoleKey.DownArrow && _currentDirection != Direction.Up)
                _currentDirection = Direction.Down;
            else if (key.Key == ConsoleKey.LeftArrow && _currentDirection != Direction.Right)
                _currentDirection = Direction.Left;

            _reading = false;
        }

        private ConsoleColor NextSnakeColor()
        {
            return _color++ % 2 == 0 ? ConsoleColor.Blue : ConsoleColor.Green;
        }

        /// <summary>
        /// Create new snake element
        /// </summary>
        private UiElement CreateSnakeElement(int posX, int posY)
        {
            var snake = new UiElement
            {
                PositionX = posX,
                PositionY = posY,
                Icon = SnakeIcon,
                Color = NextSnakeColor(),
            };
            return snake;
        }

        /// <summary>
        /// Move the snake forward
        /// </summary>
        private void MoveSnake()
        {
            var head = _snake.Last();
            UiElement next;
            switch (_currentDirection)
            {
                case Direction.Up:
                    next = CreateSnakeElement(head.PositionX, head.PositionY - 1);
                    break;
                case Direction.Down: 
                    next = CreateSnakeElement(head.PositionX, head.PositionY + 1);
                    break;
                case Direction.Left: 
                    next = CreateSnakeElement(head.PositionX - 1, head.PositionY);
                    break;
                case Direction.Right: 
                    next = CreateSnakeElement(head.PositionX + 1, head.PositionY);
                    break;
                default: return;
            }

            // Check if game lost
            if (next.PositionX == Console.WindowWidth || next.PositionX == -1 || next.PositionY == Console.WindowHeight || next.PositionX == -1
                || _snake.Any(e => e.PositionX == next.PositionX && e.PositionY == next.PositionY))
            {
                EndGame();
                return;
            }

            // Compare next to food
            if (next.PositionX == _nextFood.PositionX && next.PositionY == _nextFood.PositionY)
            {
                next.Icon = FoodIcon;
                _nextFood = GenerateFood();
            }
            else
                next.Draw();

            // Move forward
            _snake.Add(next);
            var tail = _snake.ElementAt(0);
            if (tail.Icon == FoodIcon)
            {
                tail.Icon = SnakeIcon;
                tail.Color = NextSnakeColor();
                tail.Draw();
            }
            else
            {
                tail.Clear();
                _snake.Remove(tail);
            }
        }

        /// <summary>
        /// Generate and draw a new food icon
        /// </summary>
        /// <returns></returns>
        private UiElement GenerateFood()
        {
            // Find coordinates not occupied by the snake
            int x = 0, y = 0;
            while (x == 0 && y == 0)
            {
                x = _rnd.Next(Console.WindowWidth);
                y = _rnd.Next(Console.WindowHeight);
                if (_snake.Any(e => e.PositionX == x && e.PositionY == y))
                    x = y = 0;
            }
            var food = new UiElement
            {
                PositionX = x,
                PositionY = y,
                Icon = FoodIcon,
                Color = _rnd.Next(3) % 2 == 0 ? ConsoleColor.Red : ConsoleColor.Yellow
            };
            food.Draw();
            return food;
        }

        /// <summary>
        /// End this game by disposing timer and signaling game over
        /// </summary>
        private void EndGame()
        {
            if (_cycleTimer == null)
                return;

            _cycleTimer.Dispose();
            _cycleTimer = null;
            _gameOver.Set();
        }

        private int CalculateScore(int accelerator)
        {
            var score = 0;
            for (var index = 0; index < _snake.Count - 5; index++)
            {
                score += 10 + index * accelerator;
            }
            return score;
        }

        /// <summary>
        /// Print all valid commands
        /// </summary>
        public void ExportValidCommands(int pad)
        {
        }
    }
}
