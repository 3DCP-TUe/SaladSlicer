// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU Lesser General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Interfaces
{
    /// <summary>
    /// Represents the interface for all classes that can have a geometry inside.
    /// </summary>
    public interface IGeometry
    {
        #region constructors
        /// <summary>
        /// Returns an exact duplicate of this IGeometry.
        /// </summary>
        /// <returns> The exact duplicate of this IGeometry. </returns>
        IGeometry DuplicateGeometryObject();
        #endregion

        #region methods
        /// <summary>
        /// Transforms the geometry.
        /// </summary>
        /// <param name="xform"> Transformation to apply to geometry. </param>
        /// <returns> True on success, false on failure. </returns>
        bool Transform(Transform xform);

        /// <summary>
        /// Returns the Bounding Box of the object.
        /// </summary>
        /// <returns> The Bounding Box. </returns>
        /// <param name="accurate"> If true, a physically accurate bounding box will be computed. </param>
        BoundingBox GetBoundingBox(bool accurate);
        #endregion
    }
}
