// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Runtime.Kernel.Additionals
{
    internal class Shots
    {
        /// <summary>
        /// Constructor for the shots of the both, player and outer wold space ships.
        /// </summary>
        public Shots()
        {
            PendingShots = new Queue<UiElement>();
            PlayerShots = new List<UiElement>();
            NpcShots = new List<UiElement>();
        }

        public Queue<UiElement> PendingShots { get; private set; } 

        public List<UiElement> PlayerShots { get; private set; }

        public List<UiElement> NpcShots { get; private set; }

        /// <summary>
        /// Copies pending shots to player shots
        /// </summary>
        public void IncludePendingShots()
        {
            while (PendingShots.Count > 0)
            {
                PlayerShots.Add(PendingShots.Dequeue());
            }
        }

        /// <summary>
        /// Remove a player shot
        /// </summary>
        public void RemovePlayerShot(UiElement playerShot)
        {
            playerShot.Clear();
            PlayerShots.Remove(playerShot);
        }

        /// <summary>
        /// Remove a npc shot
        /// </summary>
        public void RemoveNpcShot(UiElement npcShot)
        {
            npcShot.Clear();
            NpcShots.Remove(npcShot);
        }
    }
}
