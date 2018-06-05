namespace Marvin.Workflows.Compiler
{
    /// <summary>
    /// Class representing a steps relations to other steps
    /// </summary>
    internal class OutputRelation
    {
        public OutputRelation(long connectorId)
        {
            ConnectorId = connectorId;
        }

        /// <summary>
        /// Id of the connector of the output - temporary till it was mapped
        /// </summary>
        public long ConnectorId { get; private set; }

        /// <summary>
        /// Id of the step it was mapped to
        /// </summary>
        public int StepId { get; private set; }

        /// <summary>
        /// Flag that this output was mapped
        /// </summary>
        public bool IsMapped { get; private set; }

        /// <summary>
        /// Map output to the next step
        /// </summary>
        public void MapTo(int stepId)
        {
            StepId = stepId;
            IsMapped = true;
        }
    }
}