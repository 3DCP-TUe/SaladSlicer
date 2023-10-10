// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Salad Libs
using SaladSlicer.Interfaces;
using SaladSlicer.Enumerations;
using SaladSlicer.Utils;

namespace SaladSlicer.CodeGeneration
{
    /// <summary>
    /// Represents the Printer Settings
    /// </summary>
    [Serializable()]
    public class PrinterSettings : IProgram
    {
        #region fields
        private ProgramType _programType = ProgramType.Sinumerik;
        private InterpolationType _interpolationType = InterpolationType.Spline;
        private double _hotEndTemperature = -1;
        private double _bedTemperature = -1;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected PrinterSettings(SerializationInfo info, StreamingContext context)
        {
            // string version = (int)info.GetValue("Version", typeof(string)); // <-- use this if the (de)serialization changes
            _programType = (ProgramType)info.GetValue("Program type", typeof(ProgramType));
            _interpolationType = (InterpolationType)info.GetValue("Interpolation type", typeof(InterpolationType));
            _hotEndTemperature = (double)info.GetValue("Hot end temperature", typeof(double));
            _bedTemperature = (double)info.GetValue("Bed temperature", typeof(double));
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Version", HelperMethods.GetVersionNumber(), typeof(string));
            info.AddValue("Program type", _programType, typeof(ProgramType));
            info.AddValue("Interpolation type", _interpolationType, typeof(InterpolationType));
            info.AddValue("Hot end temperature", _hotEndTemperature, typeof(double));
            info.AddValue("Bed temperature", _bedTemperature, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a default temperature setting with zeros
        /// </summary>
        public PrinterSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Printer Settings class.
        /// </summary>
        /// <param name="programType"> The program type. </param>
        /// <param name="interpolationType"> The interpolation type. </param>
        /// <param name="hotEndTemperature"> The hot end temperature to be set. </param>
        /// <param name="bedTemperature"> The bed temperature to be set. </param>
        public PrinterSettings(ProgramType programType, InterpolationType interpolationType, double hotEndTemperature, double bedTemperature)
        {
            _programType = programType;
            _interpolationType = interpolationType;
            _hotEndTemperature = hotEndTemperature;
            _bedTemperature = bedTemperature;    
        }

        /// <summary>
        /// Initializes a new instance of the Printer Settings class by duplicating an existing Printer Settings instance. 
        /// </summary>
        /// <param name="printerSettings"> The Printer Settings instance to duplicate. </param>
        public PrinterSettings(PrinterSettings printerSettings)
        {
            _programType = printerSettings.ProgramType;
            _interpolationType = printerSettings.InterpolationType;
            _hotEndTemperature = printerSettings.HotEndTemperature;
            _bedTemperature = printerSettings.BedTemperature;
        }

        /// <summary>
        /// Returns an exact duplicate of this Printer Settings object.
        /// </summary>
        /// <returns> The exact duplicate of this Printer Settings instance </returns>
        public PrinterSettings Duplicate()
        {
            return new PrinterSettings(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Printer Settings object as an IProgram.
        /// </summary>
        /// <returns> The exact duplicate of this Printer Settings instance as an IProgram. </returns>
        public IProgram DuplicateProgramObject()
        {
            return new PrinterSettings(this);
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            return ("Printer settings");
        }

        /// <summary>
        /// Returns the Temperature Settings as a string
        /// </summary>
        /// <returns>The string</returns>
        public string ToSingleString()
        {
            throw new Exception("The printer settings cannot be represented by a single string");
        }

        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToProgram(ProgramGenerator programGenerator)
        {
            // Overwrite printer settings
            programGenerator.PrinterSettings = this.Duplicate();

            // Code header
            programGenerator.Program.Add("; ----------------------------------------------------------------------");
            programGenerator.Program.Add("; PRINTER SETTINGS");
            programGenerator.Program.Add("; ----------------------------------------------------------------------");
            
            // Program Type
            if (_programType == ProgramType.Sinumerik)
            {
                programGenerator.Program.Add("; G-Code flavor: Sinumerik");
                programGenerator.Program.Add("G500; Zero frame");
                programGenerator.Program.Add("SPCON; Position-controlled spindle ON");
                programGenerator.Program.Add("G90; Absolute coordinates ");
                programGenerator.Program.Add("G1 C90 F10000; Moves the C-axis tangent to y direction");
            }
            else if(_programType == ProgramType.Marlin)
            {
                programGenerator.Program.Add("; G-Code flavor: Marlin");
                programGenerator.Program.Add("M106; Turn on fans");
                //ogramGenerator.Program.Add("M201 X500.00 Y500.00 Z100.00 E5000.00; Setup machine max acceleration");
                //ogramGenerator.Program.Add("M203 X500.00 Y500.00 Z10.00 E50.00; Setup machine max feedrate");
                //ogramGenerator.Program.Add("M204 P500.00 R1000.00 T500.00; Setup Print/ Retract / Travel acceleration");
                //ogramGenerator.Program.Add("M205 X8.00 Y8.00 Z0.40 E5.00; Setup Jerk");
                programGenerator.Program.Add("M82; Absolute extrusion mode");
                programGenerator.Program.Add("G28; Move home");
                programGenerator.Program.Add("G92 E2; Set current extruder position as 0");
            }
            else
            {
                throw new Exception("Program type not implemented");
            }

            // Interpolations
            if (_interpolationType == InterpolationType.Spline)
            {
                programGenerator.Program.Add("BSPLINE; Bspline interpolation");
                programGenerator.Program.Add("G642; Continuous-path mode with smoothing within the defined tolerances");
                programGenerator.Program.Add("TANG(C, X, Y, 1)");
                programGenerator.Program.Add("TANGON(C, 0)");
            }
            else if (_interpolationType == InterpolationType.Linear)
            {
            }
            else
            {
                throw new Exception("Interpolation type not implemented");
            }
            
            // HotEndTemperature && BedTemperature
            if (_hotEndTemperature >= 0 || _bedTemperature >= 0)
            {
                programGenerator.Program.Add("G91; Relative coordinates");
                programGenerator.Program.Add("G1 Z10; Move off printbed");
                
                if (_hotEndTemperature >= 0)
                {
                    programGenerator.Program.Add($"M104 S{_hotEndTemperature:0.#}; Set hotend temperature");
                    programGenerator.PrinterSettings.HotEndTemperature = _hotEndTemperature;
                }
                
                if (_bedTemperature >= 0) 
                { 
                    programGenerator.Program.Add($"M140 S{_bedTemperature:0.#}; Set bed temperature");
                    programGenerator.PrinterSettings.BedTemperature = _bedTemperature;
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
                
                programGenerator.Program.Add("G1 Z-10; Move back to original position");
                programGenerator.Program.Add("G90; Absolute coordinates");
            }

            programGenerator.Program.Add(" ");

            //Checks 
            if (_programType == ProgramType.Marlin && _interpolationType == InterpolationType.Spline)
            {
                throw new Exception("The Marlin G-Code flavor does not implement G642 interpolation");
            }
        }

        /// <summary>
        /// Collects the data of this object to the program generator to generate the path.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToPath(ProgramGenerator programGenerator)
        {
            programGenerator.InterpolationType = _interpolationType;
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
        public ProgramType ProgramType
        {
            get { return _programType; }
            set { _programType = value; }
        }

        /// <summary>
        /// Gets or sets the interpolation type
        /// </summary>
        public InterpolationType InterpolationType
        {
            get { return _interpolationType; }
            set { _interpolationType = value; }
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
