// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
// Salad Slicer Libs
using SaladSlicer.Core.CodeGeneration;

namespace SaladSlicer.Gh.Goos.CodeGeneration
{
    /// <summary>
    /// Represents the GH_CodeLine class.
    /// </summary>
    public class GH_CodeLine : GH_Goo<CodeLine>
    {
        #region (de)serialisation
        //TODO
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the GH_CodeLine class.
        /// </summary>
        public GH_CodeLine()
        {
            this.Value = null;
        }

        /// <summary>
        /// Initializes a new Code Line Goo instance from a Code Line instance.
        /// </summary>
        /// <param name="codeLine"> Code Line Value to store inside this Goo instance. </param>
        public GH_CodeLine(CodeLine codeLine)
        {
            this.Value = codeLine;
        }

        /// <summary>
        /// Returns a complete duplicate of this Goo instance.
        /// </summary>
        /// <returns> A duplicate of the Code Line Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            if (this.Value == null)
            {
                return new GH_CodeLine();
            }
            else
            {
                return new GH_CodeLine(this.Value.Duplicate());
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
            {
                return "Null Code Line";
            }
            else
            {
                return Value.ToString();
            }
        }

        /// <summary>
        /// Attempt a cast to type Q.
        /// </summary>
        /// <typeparam name="Q"> Type to cast to. </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(ref Q target)
        {
            // Check null type
            if (Value == null) 
            { 
                target = default;
                return false;
            }

            //Cast to Code Line
            if (typeof(Q).IsAssignableFrom(typeof(CodeLine)))
            {
                target = (Q)(object)Value;
                return true;
            }

            //Cast to Code Line Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_CodeLine)))
            {
                target = (Q)(object)new GH_CodeLine(Value);
                return true;
            }

            // Invalid cast
            target = default;
            return false;
        }

        /// <summary>
        /// Attempt a cast from generic object.
        /// </summary>
        /// <param name="source"> Reference to data that should be cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastFrom(object source)
        {
            // Check null type
            if (source == null) 
            {
                return false; 
            }

            // Cast from Code Line
            if (typeof(CodeLine).IsAssignableFrom(source.GetType()))
            {
                Value = source as CodeLine;
                return true;
            }

            // Cast from Text
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                GH_String goo = (GH_String)source;
                Value = new CodeLine(goo.Value);
                return true;
            }

            // Invalid cast
            return false;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the current value is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return Value.IsValid;
            }
        }

        /// <summary>
        /// Gets a string describing the state of "invalidness". 
        /// If the instance is valid, then this property should return String.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (Value == null) { return "No internal Code Line instance"; }
                if (Value.IsValid) { return string.Empty; }
                return "Invalid Code Line instance: Did you define a Text?";
            }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Code Line."; }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Code Line"; }
        }
        #endregion
    }
}
