using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Marvin.Container;
using Marvin.Runtime.Kernel.Additionals;

namespace Marvin.Runtime.Kernel
{
    [Registration(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class SpaceCommand : ICommandHandler
    {
        #region Enums
        /// <summary>
        /// Direction for the NPC's
        /// </summary>
        private enum NpcDirection
        {
            Down,
            Left,
            Right
        }
        /// <summary>
        /// Possible directions for the Human controlled Space ship
        /// </summary>
        private enum HumanDirection
        {
            Left,
            Right
        }
        #endregion

        /// <summary>
        /// Number of enemy lines
        /// </summary>
        private const int EnemyLines = 3;
        /// <summary>
        /// Icons of enemies
        /// </summary>
        private static readonly string[] EnemyIcons = { "V", "X", "W" };

        #region All Variables and Fields
        /// <summary>
        /// The Spaceship itself.
        /// </summary>
        private readonly UiElement _spaceShip = new UiElement { Icon = "A" };

        /// <summary>
        /// All rows of enemies
        /// </summary>
        private List<UiElement>[] _npcRows;

        /// <summary>
        /// Class maintaining rocket objects
        /// </summary>
        private Shots _shots;

        /// <summary>
        /// Current direction where the enemie space ships are flying.
        /// </summary>
        private NpcDirection _currentNpcDirection = NpcDirection.Left;

        /// <summary>
        /// Pray that this luck Creator is working for you :) determines how often the enemy can shoot.
        /// </summary>
        private Random _luckCreator = new Random();
        /// <summary>
        /// The counter after which the enemies will move.
        /// </summary>
        private int _moveEnemiesAfter = 0;
        /// <summary>
        /// Used for reading the keyboard and interpred the keys.
        /// </summary>
        private Timer _cycleTimer;
        /// <summary>
        /// timer when all the other things on the screen should move
        /// </summary>
        private Timer _moveElementsTimer;
        /// <summary>
        /// When the game is over then this event will be reseted.
        /// </summary>
        private ManualResetEvent _gameOver;

        /// <summary>
        /// Ui representation of the score
        /// </summary>
        private UiElement _scoreUi = new UiElement();
        /// <summary>
        /// The score which was reached.
        /// </summary>
        private int _score = 0;

        /// <summary>
        /// Stats for selected difficulty
        /// </summary>
        private DifficultyStats _difficulty;
        #endregion

        private readonly StageCommand _stage = new StageCommand("save", "the", "world");
        public bool CanHandle(string command)
        {
            // 3 stage input for better hiding
            return _stage.HandleChain(command);
        }

        public void Handle(string[] fullCommand)
        {
            Console.Clear();
            Prepare();
            _difficulty = DifficultyStats.Create((Difficulty)SelectDifficulty());
            Console.CursorVisible = false;
            Console.Clear();

            // Create some enemies.
            CreateEnemies();

            // Draw the Spaceship
            _spaceShip.Draw();

            // Prepare reset event
            _gameOver = new ManualResetEvent(false);

            // Start reading the keybord for movements
            _cycleTimer = new Timer(ReadInput, null, 0, 30);

            // Start the movement timer.
            _moveElementsTimer = new Timer(MoveAllElements, null, 100, 100);

            // Wait for game to finish
            _gameOver.WaitOne();

            Console.Clear();
            Console.WriteLine("Your Score: " + _score);
            Console.WriteLine("Game Over");
            Console.CursorVisible = true;
            Console.ReadLine();
            Console.Clear();
        }


        private void Prepare()
        {
            _score = 0;
            _currentNpcDirection = NpcDirection.Left;
            _moveEnemiesAfter = 0;

            // Get start position
            _spaceShip.PositionX = (Console.WindowWidth / 2) - 2;
            _spaceShip.PositionY = Console.WindowHeight - 1;

            _shots = new Shots();
        }

        private int SelectDifficulty()
        {
            Console.WriteLine("Select your difficulty: (0 easy, 1 medium, 2 hard, 3 pain in the A)");
            int input = 0;
            Console.Write("Your choice: ");
            int.TryParse(Console.ReadLine(), out input);
            return input <= 3 ? input : 0;
        }

        /// <summary>
        /// Create some enemies.
        /// </summary>
        private void CreateEnemies()
        {
            var npcRows = new List<List<UiElement>>();
            for (var enemyRow = 0; enemyRow < EnemyLines; enemyRow++)
            {
                var row = new List<UiElement>();
                var ypos = (EnemyLines + 1) - enemyRow;
                var icon = EnemyIcons[enemyRow];

                for (var xpos = 20 + (enemyRow % 2); xpos <= 60 - (enemyRow % 2); xpos+=2)
                {
                    var enemy = new UiElement {PositionX = xpos, PositionY = ypos, Icon = icon};
                    enemy.Draw();
                    row.Add(enemy);
                }
                npcRows.Add(row);
            }
            _npcRows = npcRows.ToArray();
        }

        /// <summary>
        /// Read input in new thread
        /// </summary>
        private void ReadInput(object unused)
        {
            if (!Console.KeyAvailable)
                return;

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    EndGame();
                    break;
                case ConsoleKey.RightArrow:
                    MoveShip(HumanDirection.Right);
                    break;
                case ConsoleKey.LeftArrow:
                    MoveShip(HumanDirection.Left);
                    break;
                case ConsoleKey.Spacebar:
                    Shoot();
                    break;
            }
        }

        /// <summary>
        /// Move the spaceship around.
        /// </summary>
        /// <param name="nextStep"></param>
        private void MoveShip(HumanDirection nextStep)
         {
            var movement = 0;
            // Get direction.
            switch (nextStep)
            {
                case HumanDirection.Left:
                    movement = _spaceShip.PositionX == 0 ? 0 : -1;
                    break;
                case HumanDirection.Right:
                    movement = _spaceShip.PositionX == Console.WindowWidth ? 0 : 1;
                    break;
            }

            // Move the spaceship with the buffer for smooooooth graphics.
            _spaceShip.UpdatePosition(_spaceShip.PositionX + movement, _spaceShip.PositionY);
        }

        /// <summary>
        /// Creates a Bullet for the spaceship and enqueue it to the movement list.
        /// </summary>
        private void Shoot()
        {
            if (_shots.PlayerShots.Count >= _difficulty.BulletsForSpaceShip)
                return;

            var rocket = new UiElement() { PositionX = _spaceShip.PositionX, PositionY = _spaceShip.PositionY - 2, Icon = "|" };
            rocket.Draw();
            _shots.PendingShots.Enqueue(rocket);
        }

        /// <summary>
        /// Timer event which will coordinate all the movements on the screen.
        /// </summary>
        private bool _moving = false;
        private void MoveAllElements(object unused)
        {
            if(_moving)
                return;

            _moving = true;
            MoveHumanRockets();
            MoveEnemies();
            MoveNpcRockets();
            _moving = false;
        }

        /// <summary>
        /// Moves all enemies on the screen.
        /// </summary>
        private void MoveEnemies()
        {
            // Don't move all the time.
            if (_moveEnemiesAfter < _difficulty.MoveEnemySpeed)
            {
                _moveEnemiesAfter++;
                return;
            }

            _moveEnemiesAfter = 0;
            int modX = 0, modY = 0;
            switch (_currentNpcDirection)
            {
                case NpcDirection.Left:
                    modX = 1;
                    _currentNpcDirection = NpcDirection.Right;
                    break;
                case NpcDirection.Right:
                    modX = -1;
                    _currentNpcDirection = NpcDirection.Left;
                    break;
                case NpcDirection.Down:
                    modY = 1;
                    _currentNpcDirection = NpcDirection.Left;
                    break;
            }

            for (var enemyRow = 0; enemyRow < EnemyLines; enemyRow++)
            {
                MoveTheEnemy(enemyRow, modX, modY);
            }
        }

        /// <summary>
        /// Move it!
        /// </summary>
        private void MoveTheEnemy(int row, int modX, int modY)
        {
            // Check all lists of enemies for movement and if they are hit by a bullet.
            var enemies = _npcRows[row];
            var hitEnemies = new List<UiElement>();
            foreach (var enemy in enemies)
            {
                int nextX = enemy.PositionX + modX, nextY = enemy.PositionY + modY;
                var hit = _shots.PlayerShots.FirstOrDefault(s => s.PositionX == nextX && s.PositionY == nextY);
                if (hit != null)
                {
                    // Clear from screen
                    enemy.Clear();

                    // Clear from lists
                    _shots.RemovePlayerShot(hit);
                    hitEnemies.Add(enemy);

                    // Update score
                    DrawScore(_difficulty.ScoreValue[row]);
                }
                enemy.UpdatePosition(nextX, nextY);
            }

            foreach (var enemy in hitEnemies)
            {
                enemies.Remove(enemy);
            }
        }

        /// <summary>
        /// Moves the spaceship bullet.
        /// </summary>
        private void MoveHumanRockets()
        {
            var removedShots = new List<UiElement>();

            // when the spaceship has fired a new bullet.
            _shots.IncludePendingShots();

            // Check all shots if they hit something and then move them.
            foreach (var shot in _shots.PlayerShots)
            {
                // Check if screen end reached
                var nextY = shot.PositionY - 1;
                if (nextY == 0)
                {
                    shot.Clear();
                    removedShots.Add(shot);
                    continue;
                }

                // Check if we hit a ship
                var hitTarget = false;
                for (var enemyLine = 0; enemyLine < EnemyLines; enemyLine++)
                {
                    var enemies = _npcRows[enemyLine];
                    var hit = enemies.FirstOrDefault(enemy => enemy.PositionX == shot.PositionX && enemy.PositionY == nextY);
                    if (hit == null)
                        continue;

                    hit.Clear();
                    enemies.Remove(hit);
                    removedShots.Add(shot);

                    DrawScore(_difficulty.ScoreValue[enemyLine]);

                    hitTarget = true;
                    break;
                }

                // Move if no hit
                if (!hitTarget)
                    shot.UpdatePosition(shot.PositionX, nextY);

                if (_npcRows.All(row => row.Count == 0))
                {
                    EndGame();
                    return;
                }
            }

            foreach (var shot in removedShots)
            {
                _shots.RemovePlayerShot(shot);
            }
        }

        /// <summary>
        /// Moves the bullets of the npcs
        /// </summary>
        private void MoveNpcRockets()
        {
            var removedRockets = new List<UiElement>();

            // If it is possible to fire again, then try it.
            if (_shots.NpcShots.Count < _difficulty.NumberOfBulletsAtTheSameTimeFromEnemy)
            {
                // Get a random number if the enemy will fire or not.
                if (_luckCreator.Next(1000) > _difficulty.FireFireFrequenceOfEnemy)
                {
                    // Check the lists, only the lower line of enemies can fire
                    UiElement newShot = null;
                    while (newShot == null && _npcRows.Any(row => row.Count > 0))
                    {
                        var enemyLine = _luckCreator.Next(EnemyLines);
                        var enemies = _npcRows[enemyLine];
                        if(!enemies.Any())
                            continue;
                        var shooter = enemies[_luckCreator.Next(enemies.Count)];
                        newShot = new UiElement
                        {
                            PositionX = shooter.PositionX,
                            PositionY = shooter.PositionY + 2 + enemyLine,
                            Icon = "|"
                        };
                    }

                    if (newShot != null)
                    {
                        newShot.Draw();
                        _shots.NpcShots.Add(newShot);
                    }
                }
            }
            // move all visible bullets.
            foreach (var shot in _shots.NpcShots)
            {
                var nextY = shot.PositionY + 1;
                if (nextY == Console.WindowHeight)
                {
                    shot.Clear();
                    removedRockets.Add(shot);
                    continue;
                }
                if (_spaceShip.PositionX == shot.PositionX && _spaceShip.PositionY == nextY)
                {
                    EndGame();
                    return;
                }

                // Move shot
                shot.UpdatePosition(shot.PositionX, nextY);
            }

            foreach (var shot in removedRockets)
            {
                _shots.RemovePlayerShot(shot);
            }
        }

        /// <summary>
        /// End this game by disposing timer and signaling game over
        /// </summary>
        private void EndGame()
        {
            if (_cycleTimer != null)
            {
                _cycleTimer.Dispose();
                _cycleTimer = null;
            }
            if (_moveElementsTimer != null)
            {
                _moveElementsTimer.Dispose();
                _moveElementsTimer = null;
            }
            _gameOver.Set();
        }

        /// <summary>
        /// Draws the socre on the screen.
        /// </summary>
        /// <param name="addToScore"></param>
        private void DrawScore(int addToScore)
        {
            _score = _score + addToScore;
            _scoreUi.Icon = _score.ToString("D8");
            _scoreUi.Draw();
        }

        public void ExportValidCommands(int pad)
        {

        }
    }
}
