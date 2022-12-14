// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU Lesser General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Interfaces
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
        /// Adds an additional variable to the program, besides X, Y and Z coordinates.
        /// </summary>
        void AddVariable(string prefix, List<List<string>> values);

        /// <summary>
        /// Adds an additional variable to the program, besides X, Y and Z.
        /// </summary>
        /// <param name="addedVariables"> The added variable(s) stored in a dictionary. </param>
        void AddVariable(Dictionary<string, List<List<string>>> addedVariables);

        /// <summary>
        /// Removes the specific prefix from the added variables.
        /// </summary>
        /// <param name="prefix"> The prefix to remove. </param>
        /// <returns> Indicates whether the prefix wis succesfully found and removed. </returns>
        bool RemoveAddedVariable(string prefix);
        #endregion

        #region properties
        /// <summary>
        /// Gets the dictionary with variables that have been added to the object.
        /// </summary>
        Dictionary<string, List<List<string>>> AddedVariables { get; }

        /// <summary>
        /// Gets the frames of the object by layer.
        /// </summary>
        List<List<Plane>> FramesByLayer { get; }
        #endregion
    }
}
