﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaladSlicer.Core.CodeGeneration
{
    /// <summary>
    /// Represents the interface for all classes that can generate G-code.
    /// </summary>
    public interface IGCode
    {
        #region constructors

        #endregion

        #region methods
        /// <summary>
        /// Adds the G-code generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        void ToProgram(ProgamGenerator programGenerator);
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        bool IsValid { get; }
        #endregion
    }
}