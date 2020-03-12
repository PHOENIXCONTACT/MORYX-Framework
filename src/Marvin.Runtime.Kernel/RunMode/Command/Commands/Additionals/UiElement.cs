// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Runtime.Kernel.Additionals
{
    /// <summary>
    /// Representation of a character on the screen
    /// </summary>
    internal class UiElement
    {
        /// <summary>
        /// Lock used to avoid cursor jumps while drawing
        /// </summary>
        private static readonly object DrawingLock = new object();

        /// <summary>
        /// Create new uilement with current ui color
        /// </summary>
        internal UiElement()
        {
            Color = Console.ForegroundColor;
        }

        /// <summary>
        /// Horizontal position of this element
        /// </summary>
        public int PositionX { get; set; }

        /// <summary>
        /// Vertical position of this element
        /// </summary>
        public int PositionY { get; set; }

        /// <summary>
        /// Icon representing this object
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Color it should be drawn as
        /// </summary>
        public ConsoleColor Color { get; set; }

        /// <summary>
        /// Draw the current state of the object
        /// </summary>
        public void Draw()
        {
            lock (DrawingLock)
            {
                Console.SetCursorPosition(PositionX, PositionY);
                Console.ForegroundColor = Color;
                Console.Write(Icon);
            }
        }

        /// <summary>
        /// Clear the current position of this element
        /// </summary>
        public void Clear()
        {
            lock (DrawingLock)
            {
                Console.SetCursorPosition(PositionX, PositionY);
                Console.Write(" ");
            }
        }

        /// <summary>
        /// Update the objects position
        /// </summary>
        public void UpdatePosition(int posX, int posY)
        {
            // Clear old position
            Clear();

            // Set new position
            PositionX = posX;
            PositionY = posY;

            // Draw new position
            Draw();
        }
    }
}
