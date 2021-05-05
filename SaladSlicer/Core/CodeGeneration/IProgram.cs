﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

namespace SaladSlicer.Core.CodeGeneration
{
    /// <summary>
    /// Represents the interface for all classes that can generate code lines for a program.
    /// </summary>
    public interface IProgram
    {
        #region constructors

        #endregion

        #region methods
        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        void ToProgram(ProgramGenerator programGenerator);
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        bool IsValid { get; }
        #endregion
    }
}