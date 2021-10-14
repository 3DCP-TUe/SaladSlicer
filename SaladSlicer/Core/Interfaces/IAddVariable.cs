// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Core.Interfaces
{
    /// <summary>
    /// Represents the interface for all classes that can add an axis inside.
    /// </summary>
    public interface IAddVariable
    {
        #region constructors
        /// <summary>
        /// Returns an exact duplicate of this IAddVariable.
        /// </summary>
        /// <returns> The exact duplicate of this IAddVariable.</returns>
        IAddVariable DuplicateAddVariableObject();
        #endregion

        #region methods
        /// <summary>
        /// Adds a variable.
        /// </summary>
        void AddVariable(string prefix, List<List<double>> values);
        #endregion

        #region properties
        /// <summary>
        /// Gets the contours.
        /// </summary>
        List<Curve> Contours { get; }

        /// <summary>
        /// Gets a list of prefixes for variables that have been added to the object.
        /// </summary>
        List<string> Prefix { get; }


        /// <summary>
        /// Gets a list of variables that have been added to the object.
        /// </summary>
        List<List<List<double>>> AddedVariable { get; }
        #endregion
    }
}
