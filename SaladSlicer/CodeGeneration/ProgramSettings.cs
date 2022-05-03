﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Salad Libs
using SaladSlicer.Interfaces;

namespace SaladSlicer.CodeGeneration
{
    /// <summary>
    /// Represents the Program Settings
    /// </summary>
    [Serializable()]
    public class ProgramSettings: IProgram
    {
        #region fields
        private int _programType = 0;
        private int _interpolation = 0;
        private double _hotEndTemperature = -1;
        private double _bedTemperature = -1;
        #endregion

        #region constructors
        /// <summary>
        /// Creates a default temperature setting with zeros
        /// </summary>
        public ProgramSettings(){}

        public ProgramSettings(ProgramSettings programSettings)
        {
            _programType = programSettings._programType;
            _interpolation = programSettings._interpolation;
            _hotEndTemperature = programSettings.HotEndTemperature;
            _bedTemperature = programSettings.BedTemperature;
        }

        /// <summary>
        /// Creates temperature settings
        /// </summary>
        /// <param name="hotEndTemperature">The hot end temperature to be set</param>
        /// <param name="bedTemperature">The bed temperature to be set</param>
        public ProgramSettings(int programType, int interpolation, double hotEndTemperature, double bedTemperature)
        {
            _programType = programType;
            _interpolation = interpolation;
            _hotEndTemperature = hotEndTemperature;
            _bedTemperature = bedTemperature;    
        }

        /// <summary>
        /// Returns an exact duplicate of this Program Settings object.
        /// </summary>
        /// <returns> The exact duplicate of this Program Settings instance </returns>
        public ProgramSettings Duplicate()
        {
            return new ProgramSettings(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Program Settings object as an IProgram.
        /// </summary>
        /// <returns> The exact duplicate of this Program Settings instance as an IProgram. </returns>
        public IProgram DuplicateProgramObject()
        {
            return new ProgramSettings(this);
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            return ($"Program settings");
        }

        /// <summary>
        /// Returns the Temperature Settings as a string
        /// </summary>
        /// <returns>The string</returns>
        public string ToSingleString()
        {
            throw new Exception("Program Settings cannot be represented by a single string");
        }

        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToProgram(ProgramGenerator programGenerator)
        {
            // Overwrite program settings
            programGenerator.ProgramSettings = this.Duplicate();

            // Code header
            programGenerator.Program.Add("; ----------------------------------------------------------------------");
            programGenerator.Program.Add("; PROGRAM SETTINGS");
            programGenerator.Program.Add("; ----------------------------------------------------------------------");
            
            // Program Type
            if (_programType == 0)
            {
                programGenerator.Program.Add("; G-Code flavor: Sinumerik");
                programGenerator.Program.Add("G500; Zero frame");
                programGenerator.Program.Add("SPCON; Position-controlled spindle ON");
                programGenerator.Program.Add("G90; Absolute coordinates ");
                programGenerator.Program.Add("G1 C0 F10000; Moves the C-axis to the zero position");
            }
            else if(_programType == 1)
            {
                programGenerator.Program.Add("; G-Code flavor: Marlin");
                programGenerator.Program.Add("M106; Turn on fans");
                //ogramGenerator.Program.Add("M201 X500.00 Y500.00 Z100.00 E5000.00; Setup machine max acceleration");
                //ogramGenerator.Program.Add("M203 X500.00 Y500.00 Z10.00 E50.00; Setup machine max feedrate");
                //ogramGenerator.Program.Add("M204 P500.00 R1000.00 T500.00; Setup Print/ Retract / Travel acceleration");
                //ogramGenerator.Program.Add("M205 X8.00 Y8.00 Z0.40 E5.00; Setup Jerk");
                programGenerator.Program.Add("M82; Absolute extrusion mode");
                programGenerator.Program.Add("G90; Absolute coordinates ");
                programGenerator.Program.Add("G28; Move home");
            }
            else
            {
                throw new Exception("Program type not implemented");
            }

            // Interpolations
            if (_interpolation == 0)
            {
                programGenerator.Program.Add("BSPLINE; Bspline interpolation");
                programGenerator.Program.Add("G642; Continuous-path mode with smoothing within the defined tolerances");
                programGenerator.Program.Add("TANG(C, X, Y, 1)");
                programGenerator.Program.Add("TANGON(C, 0)");
            }
            else if (_interpolation == 1)
            {
                programGenerator.Program.Add("G1; Linear movements ");
            }
            else
            {
                throw new Exception("Interpolation type not implemented");
            }
            
            // HotEndTemperature && BedTemperature
            if (_hotEndTemperature >= 0 || _bedTemperature >= 0)
            {
                programGenerator.Program.Add("Z10; Move off printbed");
                
                if (_hotEndTemperature >= 0)
                {
                    programGenerator.Program.Add($"M104 S{_hotEndTemperature:0.#}; Set hotend temperature");
                    programGenerator.ProgramSettings.HotEndTemperature = _hotEndTemperature;
                }
                
                if (_bedTemperature >= 0) 
                { 
                    programGenerator.Program.Add($"M140 S{_bedTemperature:0.#}; Set bed temperature");
                    programGenerator.ProgramSettings.BedTemperature = _bedTemperature;
                }
                
                programGenerator.Program.Add("M105; Report temperature");
                
                if (_hotEndTemperature >= 0)
                { 
                    programGenerator.Program.Add($"M109 S{_hotEndTemperature:0.#}; Wait for hotend temperature ");
                }
                if (_bedTemperature >= 0) 
                { 
                    programGenerator.Program.Add($"M190 S{_bedTemperature:0.#}; Wait for bed temperature "); 
                }
                
                programGenerator.Program.Add("Z-10; Move back to original position");
            }

            programGenerator.Program.Add(" ");

            //Checks 
            if (_programType == 1 &&_interpolation == 0)
            {
                throw new Exception("The Marlin G-Code flavor does not implement G642 interpolation");
            }
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
                if (_hotEndTemperature == double.NaN) { return false; }
                if (_bedTemperature == double.NaN) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the program type.
        /// </summary>
        public int ProgramType
        {
            get { return _programType; }
            set { _programType = value; }
        }

        /// <summary>
        /// Gets or sets the interpolation type
        /// </summary>
        public int Interpolation
        {
            get { return _interpolation; }
            set { _interpolation = value; }
        }

        /// <summary>
        /// Gets or sets the hot end temperature.
        /// </summary>
        public double HotEndTemperature
        {
            get { return _hotEndTemperature; }
            set { _hotEndTemperature = value; }
        }

        /// <summary>
        /// Gets or sets the bed temperature.
        /// </summary>
        public double BedTemperature
        {
            get { return _bedTemperature; }
            set { _bedTemperature = value; }
        }
        #endregion
    }
}
