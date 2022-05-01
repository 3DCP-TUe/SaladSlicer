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
    /// Represents a custom (user definied) Code Line.
    /// </summary>
    [Serializable()]
    public class CodeLine : IProgram, ISerializable
    {
        #region fields
        private string _code;
        #endregion

        #region (de)serialization
        /// <summary>
        /// Protected constructor needed for deserialization of the object.  
        /// </summary>
        /// <param name="info"> The SerializationInfo to extract the data from. </param>
        /// <param name="context"> The context of this deserialization. </param>
        protected CodeLine(SerializationInfo info, StreamingContext context)
        {
            _code = (string)info.GetValue("Code", typeof(string));
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the object.
        /// </summary>
        /// <param name="info"> The SerializationInfo to populate with data. </param>
        /// <param name="context"> The destination for this serialization. </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Code", _code, typeof(string));
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Code Line class.
        /// </summary>
        public CodeLine()
        {
            _code = "";
        }

        /// <summary>
        /// Initializes a new instance of the Code Line class.
        /// </summary>
        /// <param name="code"> The custom code line. </param>
        public CodeLine (string code)
        {
            _code = code;
        }

        /// <summary>
        /// Initializes a new instance of the Code Line class by duplicating an existing Code Line instance. 
        /// </summary>
        /// <param name="codeLine"> The Code Line instance to duplicate. </param>
        public CodeLine(CodeLine codeLine)
        {
            _code = codeLine.Code;
        }

        /// <summary>
        /// Returns an exact duplicate of this Code Line instance.
        /// </summary>
        /// <returns> The duplicate of the Code Line instance. </returns>
        public CodeLine Duplicate()
        {
            return new CodeLine(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Code Line instance as an IProgram
        /// </summary>
        /// <returns> The exact duplicate of this Code Line instance as an IProgram. </returns>
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
            if (this.IsValid == false)
            {
                return "InValid Code Line";
            }
            else if (_code == "")
            {
                return "Empty Code Line";
            }
            else
            {
                return "Custom Code Line";
            }
        }

        public string ToSingleString()
        {
            return this.Code;
        }

        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToProgram(ProgramGenerator programGenerator)
        {
            programGenerator.Program.Add(_code);
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
                if (_code == null) { return false; } 
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the custom Code Line text.
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        #endregion
    }
}
