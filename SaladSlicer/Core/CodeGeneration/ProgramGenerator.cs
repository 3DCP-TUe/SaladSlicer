﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System.Collections.Generic;
// Salad Slicer Libs
using SaladSlicer.Core.Utils;
using SaladSlicer.Core.Interfaces;

namespace SaladSlicer.Core.CodeGeneration
{
    /// <summary>
    /// Represents the Program Generator.
    /// </summary>
    public class ProgramGenerator
    {
        #region fields
        private readonly List<string> _program = new List<string>();
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the Program Generator class.
        /// </summary>
        public ProgramGenerator()
        {

        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            return "Program Generator";
        }

        /// <summary>
        /// Returns the G-code program as list with code lines.
        /// </summary>
        /// <param name="_objects"> The objects to gerenator the program for. </param>
        /// <returns> The program as a list with code lines. </returns>
        public List<string> CreateProgram(List<IProgram> _objects)
        {
            _program.Clear();

            // Header
            _program.Add("; ----------------------------------------------------------------------");
            _program.Add("; This program was generated by Salad Slicer v" + HelperMethods.GetVersionNumber() + " (GPL v3).");
            _program.Add("; For more information visit https://github.com/3DCP-TUe/SaladSlicer.");
            _program.Add("; ----------------------------------------------------------------------");
            _program.Add(" ");

            // G-code of different objects
            for (int i = 0; i < _objects.Count; i++)
            {
                _objects[i].ToSinumerik(this);
            }

            // Program end
            _program.Add(" ");
            _program.Add("M30");
            _program.Add(" ");
            _program.Add(" ");
            _program.Add(" ");

            return _program;
        }

        /// <summary>
        /// Adds a default pre-defined slicer header.
        /// </summary>
        /// <param name="name"> The object name. </param>
        /// <param name="layers"> The number of layers of the pre-defined slicer. </param>
        /// <param name="length"> The path length in mm. </param>
        public void AddSlicerHeader(string name, int layers, double length)
        {
            _program.Add(" ");
            _program.Add("; ----------------------------------------------------------------------");
            _program.Add($"; {name} - {layers:0} LAYERS - {(length / 1000):0.###} METER");
            _program.Add("; ----------------------------------------------------------------------");
            _program.Add(" ");
        }

        /// <summary>
        /// Adds a default pre-defined slicer header.
        /// </summary>
        /// <param name="name"> The object name. </param>
        /// <param name="length"> The path length in mm. </param>
        public void AddSlicerHeader(string name, double length)
        {
            _program.Add(" ");
            _program.Add("; ----------------------------------------------------------------------");
            _program.Add($"; {name} - {(length / 1000):0.###} METER");
            _program.Add("; ----------------------------------------------------------------------");
            _program.Add(" ");
        }

        /// <summary>
        /// Adds a default footer.
        /// </summary>
        public void AddFooter()
        {
            _program.Add(" ");
            _program.Add("; ----------------------------------------------------------------------");
            _program.Add(" ");
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (_program == null) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets the list with program code lines.
        /// </summary>
        public List<string> Program
        {
            get { return _program; }
        }
        #endregion 
    }
}
