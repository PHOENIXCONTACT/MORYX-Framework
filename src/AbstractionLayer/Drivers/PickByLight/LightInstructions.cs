namespace Marvin.AbstractionLayer.Drivers.PickByLight
{
    /// <summary>
    /// Output structure for a pick by light driver
    /// </summary>
    public struct LightInstructions
    {
        /// <summary>
        /// Activate light instructions
        /// </summary>
        public LightInstructions(long colorCode, string instruction)
        {
            Active = true;
            ColorCode = colorCode;
            Instruction = instruction;
        }

        /// <summary>
        /// Instruction active
        /// </summary>
        public bool Active { get; }

        /// <summary>
        /// Encoded color 
        /// </summary>
        public long ColorCode { get; }

        /// <summary>
        /// Instruction text
        /// </summary>
        public string Instruction { get; set; }
    }
}