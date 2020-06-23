// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Kernel.Additionals
{
    /// <summary>
    /// Enum for the difficulty of the outer world ships.
    /// </summary>
    internal enum Difficulty
    {
        Easy = 0,
        Medium = 1,
        Hard = 2,
        PainInTheA = 3
    }

    internal class DifficultyStats
    {
        private DifficultyStats()
        {
        }

        internal static DifficultyStats Create(Difficulty difficulty)
        {
            DifficultyStats instance;
            switch (difficulty)
            {
                case Difficulty.Medium:
                    instance = new DifficultyStats
                    {
                        FireFireFrequenceOfEnemy = 925,
                        MoveEnemySpeed = 95,
                        NumberOfBulletsAtTheSameTimeFromEnemy = 7,
                        ScoreValue = new[] { 250, 350, 700 },
                        BulletsForSpaceShip = 4,
                    };
                    break;
                case Difficulty.Hard:
                    instance = new DifficultyStats
                    {
                        FireFireFrequenceOfEnemy = 900,
                        MoveEnemySpeed = 90,
                        NumberOfBulletsAtTheSameTimeFromEnemy = 9,
                        ScoreValue = new[] { 500, 800, 1250 },
                        BulletsForSpaceShip = 3,
                    };
                    break;
                case Difficulty.PainInTheA:
                    instance = new DifficultyStats
                    {
                        FireFireFrequenceOfEnemy = 200,
                        MoveEnemySpeed = 20,
                        NumberOfBulletsAtTheSameTimeFromEnemy = 50,
                        ScoreValue = new[] { 1250, 1900, 2590 },
                        BulletsForSpaceShip = 4,
                    };
                    break;
                case Difficulty.Easy: // Left for readability
                default:
                    instance = new DifficultyStats
                    {
                        FireFireFrequenceOfEnemy = 950,
                        MoveEnemySpeed = 100,
                        NumberOfBulletsAtTheSameTimeFromEnemy = 5,
                        ScoreValue = new[] { 150, 250, 500 },
                        BulletsForSpaceShip = 5,
                    };
                    break;
            }
            return instance;
        }

        /// <summary>
        /// Sets the frequency of the enemy when he fires a bullet at the spaceship.
        /// </summary>
        public int FireFireFrequenceOfEnemy { get; private set; }
        /// <summary>
        /// The speed of the Enemy when it will move left or right or down.
        /// </summary>
        public int MoveEnemySpeed { get; private set; } // easy
        /// <summary>
        /// the Number of bullets on the screen which the enemy can fire simultaniosly.
        /// </summary>
        public int NumberOfBulletsAtTheSameTimeFromEnemy { get; private set; } // easy
        /// <summary>
        /// The Bounty for getting the enemies.
        /// </summary>
        public int[] ScoreValue { get; private set; }// easy
        /// <summary>
        /// The amount of bullets the spaceship can fire at once.
        /// </summary>
        public int BulletsForSpaceShip { get; private set; } // easy   
    }
}
