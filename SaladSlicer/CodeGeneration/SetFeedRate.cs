﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Salad Libs
using SaladSlicer.Interfaces;
using SaladSlicer.Utils;

namespace SaladSlicer.CodeGeneration
{
    /// <summary>
    /// Represents the Feed Rate.
    /// </summary>
    [Serializable()]
    public class SetFeedRate : IProgram
    {
        #region fields
        private double _feedRate;
        #endregion

        #region (de)serialisation
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected SetFeedRate(SerializationInfo info, StreamingContext context)
        {
            // string version = (int)info.GetValue("Version", typeof(string)); // <-- use this if the (de)serialization changes
            _feedRate = (double)info.GetValue("Feed rate", typeof(double));
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
            info.AddValue("Feed rate", _feedRate, typeof(double));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the FeedRate class.
        /// </summary>         
        public SetFeedRate()
        {
            _feedRate = double.NaN;
        }

        /// <summary>
        /// Initializes a new instance of the FeedRate class.
        /// </summary>
        /// <param name="feedRate">Double representing the velocity of movement.</param>
        public SetFeedRate(double feedRate)
        {
            _feedRate = feedRate;
        }

        /// <summary>
        /// Initializes a new instance of the Feed Rate class by duplicating an existing Feed Rate instance. 
        /// </summary>
        /// <param name="feedRate"> The Feed Rate instance to duplicate. </param>
        public SetFeedRate(SetFeedRate feedRate)
        {
            _feedRate = feedRate.Feedrate;
        }

        /// <summary>
        /// Returns an exact duplicate of this Feed Rate instance.
        /// </summary>
        /// <returns> The exact duplicate of this Feed Rate instance. </returns>
        public SetFeedRate Duplicate()
        {
            return new SetFeedRate(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Feed Rate instance as an IProgram
        /// </summary>
        /// <returns> The exact duplicate of this Feed Rate instance as an IProgram. </returns>
        public IProgram DuplicateProgramObject()
        {
            return this.Duplicate();
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            return $"Set Feedrate (F{_feedRate:0.###} mm/min)";
        }

        /// <summary>
        /// Returns the FeedRate object as a string
        /// </summary>
        /// <returns>The string</returns>
        public string ToSingleString()
        {
            return ($"F{_feedRate:0.###}");
        }

        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToProgram(ProgramGenerator programGenerator)
        {
            programGenerator.Program.Add($"F{_feedRate:0.###} ; Feedrate in mm/min");
        }

        /// <summary>
        /// Collects the data of this object to the program generator to generate the path.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToPath(ProgramGenerator programGenerator)
        {

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
                if (_feedRate == double.NaN) { return false; } 
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the feedrate in mm/min.
        /// </summary>
        public double Feedrate
        {
            get { return _feedRate; }
            set { _feedRate = value; }
        }
        #endregion
    }
}
