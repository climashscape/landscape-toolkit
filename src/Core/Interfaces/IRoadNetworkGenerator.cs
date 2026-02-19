using Rhino.Geometry;
using System.Collections.Generic;

namespace LandscapeToolkit.Core.Interfaces
{
    /// <summary>
    /// Interface for any road generation strategy (e.g., Quad Mesh, SubD, or NURBS).
    /// </summary>
    public interface IRoadNetworkGenerator
    {
        /// <summary>
        /// Generates the road network geometry.
        /// </summary>
        /// <returns>A Mesh or list of Meshes representing the roads.</returns>
        Mesh Generate();
        
        /// <summary>
        /// Gets the underlying graph structure.
        /// </summary>
        object GetGraph();
    }
}
