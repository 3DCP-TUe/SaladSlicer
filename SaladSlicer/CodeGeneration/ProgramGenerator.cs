﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
//Rhino Libs
using Rhino.Geometry;
// Salad Slicer Libs
using SaladSlicer.Enumerations;
using SaladSlicer.Interfaces;
using SaladSlicer.Utils;

namespace SaladSlicer.CodeGeneration
{
    /// <summary>
    /// Represents the Program Generator.
    /// </summary>
    [Serializable()]
    public class ProgramGenerator
    {
        #region fields
        private readonly List<string> _program = new List<string>();
        private PrinterSettings _printerSettings = new PrinterSettings();
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected ProgramGenerator(SerializationInfo info, StreamingContext context)
        {
            // string version = (int)info.GetValue("Version", typeof(string)); // <-- use this if the (de)serialization changes
            _program = (List<string>)info.GetValue("Program", typeof(List<string>));
            _printerSettings = (PrinterSettings)info.GetValue("Printer settings", typeof(PrinterSettings));
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
            info.AddValue("Program", _program, typeof(List<string>));
            info.AddValue("Printer settings", _printerSettings, typeof(PrinterSettings));
        }
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
        public List<string> CreateProgram(IList<IProgram> _objects)
        {
            _program.Clear();
            
            // Header
            _program.Add("; ----------------------------------------------------------------------");
            _program.Add("; This program was generated by Salad Slicer v" + HelperMethods.GetVersionNumber() + " (GPL v3).");
            _program.Add("; For more information visit https://github.com/3DCP-TUe/SaladSlicer.");
            _program.Add("; ----------------------------------------------------------------------");
            _program.Add(" ");

            // Set program settings
            if (_objects[0].GetType() != typeof(PrinterSettings))
            {
                _program.Add("; ----------------------------------------------------------------------");
                _program.Add("; SETTINGS");
                _program.Add("; ----------------------------------------------------------------------");
                _program.Add(" ");
                _program.Add("; NO PROGRAM SETTINGS DEFINED");
                _program.Add("; Defaults settings are used");
                _program.Add(" ");

                PrinterSettings printerSettings = new PrinterSettings();
                printerSettings.ToProgram(this);
            }

            // G-code of different objects
            for (int i = 0; i < _objects.Count; i++)
            {
                _objects[i].ToProgram(this);
            }
            
            // Footer (ending)
            if (_printerSettings.ProgramType == ProgramType.Sinumerik){
                _program.Add(" ");
                _program.Add("M30");
                _program.Add(" ");
                _program.Add(" ");
                _program.Add(" ");
            }
            else if (_printerSettings.ProgramType == ProgramType.Marlin)
            {
                _program.Add(" ");
                _program.Add("G91; Relative coordinates ");
                _program.Add("G1 Z10 E-2; Move off object and retract extrusion material ");
                _program.Add("G90; Absolute coordinates ");
                _program.Add("G1 X0 Y0; Move home ");

                if (_printerSettings.HotEndTemperature != 0 || _printerSettings.BedTemperature != 0)
                {
                    if (_printerSettings.HotEndTemperature >= 0)
                    {
                        _printerSettings.HotEndTemperature = 0;
                        _program.Add($"M104 S{_printerSettings.HotEndTemperature:0.#}; Set hotend temperature to 0");
                        _program.Add("M105; Report temperature");
                    }

                    if (_printerSettings.BedTemperature >= 0)
                    {
                        _printerSettings.BedTemperature = 0;
                        _program.Add($"M140 R{_printerSettings.BedTemperature:0.#}; Set bed temperature to 0");
                        _program.Add("M105; Report temperature");
                    }
                }

                _program.Add("M106 S0; Turn off fan");
                _program.Add(" ");
            }

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

        /// <summary>
        /// Returns the coordinate code lines including the added variables from an AddVariable instance.
        /// </summary>
        /// <param name="addVariable"> The added variable object to get the coorindate code lines from. </param>
        /// <returns> The nested list code lines. </returns>
        public static List<List<string>> GetCoordinateCodeLines(IAddVariable addVariable, PrinterSettings printerSettings)
        {
            List<List<string>> result = new List<List<string>>();
            List<List<Plane>> frames = addVariable.FramesByLayer;

            // Linear movements
            if (printerSettings.InterpolationType == InterpolationType.Linear)
            {
                for (int i = 0; i < frames.Count; i++)
                {
                    List<string> temp = new List<string>() { };
                    for (int j = 0; j < frames[i].Count; j++)
                    {
                        temp.Add("G1 ");
                    }
                    result.Add(temp);
                }
            }
            else
            {
                for (int i = 0; i < frames.Count; i++)
                {
                    List<string> temp = new List<string>() { };
                    for (int j = 0; j < frames[i].Count; j++)
                    {
                        temp.Add("");
                    }
                    result.Add(temp);
                }
            }

            // Coordinates
            for (int i = 0; i < result.Count; i++)
            {
                //List<string> temp = new List<string>() { };

                for (int j = 0; j < result[i].Count; j++)
                {
                    result[i][j] = result[i][j] + $"X{frames[i][j].OriginX:0.###} Y{frames[i][j].OriginY:0.###} Z{frames[i][j].OriginZ:0.###}";
                    //temp.Add($"X{frames[i][j].OriginX:0.###} Y{frames[i][j].OriginY:0.###} Z{frames[i][j].OriginZ:0.###}");
                }

                //result.Add(temp);
            }

            // Added variables
            foreach (KeyValuePair<string, List<List<string>>> entry in addVariable.AddedVariables)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    for (int j = 0; j < result[i].Count; j++)
                    {
                        result[i][j] = result[i][j] + $" {entry.Key}{entry.Value[i][j]:0.###}";
                    }
                }
            }

            return result;
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

        /// <summary>
        /// Gets or sets the printer settings.
        /// </summary>
        public PrinterSettings PrinterSettings
        {
            get { return _printerSettings; }
            set { _printerSettings = value; }
        }
        #endregion 
    }
}
