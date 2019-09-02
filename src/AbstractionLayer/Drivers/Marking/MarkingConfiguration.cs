using System.Collections.Generic;

namespace Marvin.AbstractionLayer.Drivers.Marking
{
    /// <summary>
    /// Holds a lot of possilbe configurations for the marking process like variables or roations.
    /// </summary>
    public class MarkingConfiguration
    {
        /// <summary>
        /// The Segments which should be marked.
        /// </summary>
        public int? Segment { get; set; }

        /// <summary>
        /// Dictionary with all the variables which are necessary for the marking.
        /// </summary>
        public IDictionary<string, string> Variables { get; set; }

        /// <summary>
        /// The factor for resizing the marking file. 1 is original size.
        /// </summary>
        public double? ResizingFactor { get; protected set; }

        /// <summary>
        /// The angle of rotation in ° clockwise around the mid point of the image field. 0 is no rotation.
        /// </summary>
        public double? Angle { get; protected set; }

        /// <summary>
        /// Displacement along the X-axis in mm.
        /// </summary>
        public double? XAxis { get; protected set; }

        /// <summary>
        /// Displacement along the Y-axis in mm.
        /// </summary>
        public double? YAxis { get; protected set; }

        /// <summary>
        /// Constuctor to set the necessary information when creating.
        /// </summary>
        public MarkingConfiguration()
        {
            ResizingFactor = 1;
            Variables = new Dictionary<string, string>();
        }

        /// <summary>
        /// Sets the transformation parameters on block.
        /// </summary>
        /// <param name="resizingFactor">The factor for resizing the marking file. 1 is original size.</param>
        /// <param name="angle">The angle of rotation in ° clockwise around the mid point of the image field. 0 is no rotation.</param>
        /// <param name="xaxis">Displacement along the X-axis in mm.</param>
        /// <param name="yaxis">Displacement along the Y-axis in mm.</param>
        /// <returns>This object.</returns>
        public MarkingConfiguration SetTransformation(double resizingFactor, double angle, double xaxis, double yaxis)
        {
            ResizingFactor = resizingFactor;
            Angle = angle;
            XAxis = xaxis;
            YAxis = yaxis;
            return this;
        }
    }
}