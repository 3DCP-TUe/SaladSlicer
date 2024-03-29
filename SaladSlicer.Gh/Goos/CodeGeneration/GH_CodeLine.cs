﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
using GH_IO.Serialization;
// Rhino Libs
using Rhino.Geometry;
// Salad Slicer Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Interfaces;
using SaladSlicer.Utils;

namespace SaladSlicer.Gh.Goos.CodeGeneration
{
    /// <summary>
    /// Represents the GH_CodeLine class.
    /// </summary>
    public class GH_CodeLine : GH_GeometricGoo<CodeLine>
    {
        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Code Line";

        /// <summary>
        /// This method is called whenever the instance is required to serialize itself.
        /// </summary>
        /// <param name="writer"> Writer object to serialize with. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Write(GH_IWriter writer)
        {
            if (Value != null)
            {
                byte[] array = HelperMethods.ObjectToByteArray(Value);
                writer.SetByteArray(IoKey, array);
            }

            return true;
        }

        /// <summary>
        /// This method is called whenever the instance is required to deserialize itself.
        /// </summary>
        /// <param name="reader"> Reader object to deserialize from. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool Read(GH_IReader reader)
        {
            if (!reader.ItemExists(IoKey))
            {
                Value = null;
                return true;
            }

            byte[] array = reader.GetByteArray(IoKey);
            Value = (CodeLine)HelperMethods.ByteArrayToObject(array);

            return true;
        }
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

        /// <summary>
        /// Returns a complete duplicate of this Goo insance.
        /// </summary>
        /// <returns> A duplicate of the Code Line Goo instance. </returns>
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return this.Duplicate() as IGH_GeometricGoo;
        }
        #endregion

        #region methods
        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Value == null)
            {
                return "Null Code Line";
            }
            else
            {
                return this.Value.ToString();
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
            if (this.Value == null)
            {
                target = default;
                return false;
            }

            // Cast to Code Line
            if (typeof(Q).IsAssignableFrom(typeof(CodeLine)))
            {
                target = (Q)(object)this.Value;
                return true;
            }

            // Cast to Code Line Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_CodeLine)))
            {
                target = (Q)(object)new GH_CodeLine(this.Value);
                return true;
            }

            // Cast to IProgram
            if (typeof(Q).IsAssignableFrom(typeof(IProgram)))
            {
                target = (Q)(object)this.Value;
                return true;
            }

            // Cast to IProgram Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ProgramObject)))
            {
                target = (Q)(object)new GH_ProgramObject(this.Value);
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
                this.Value = source as CodeLine;
                return true;
            }

            // Cast from Text
            if (typeof(GH_String).IsAssignableFrom(source.GetType()))
            {
                GH_String goo = source as GH_String;
                this.Value = new CodeLine(goo.Value);
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
                if (this.Value == null) { return false; }
                return this.Value.IsValid;
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
                if (this.Value == null) { return "No internal Code Line instance"; }
                if (this.Value.IsValid) { return string.Empty; }
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

        #region transformation methods
        /// <summary>
        /// Transforms the object or a deformable representation of the object.
        /// </summary>
        /// <param name="xform"> Transformation matrix. </param>
        /// <returns> Transformed geometry. </returns>
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            if (Value == null)
            {
                return null;
            }

            else if (Value.IsValid == false)
            {
                return null;
            }

            else
            {
                return new GH_CodeLine(Value);
            }
        }

        /// <summary>
        /// Morph the object or a deformable representation of the object.
        /// </summary>
        /// <param name="xmorph"> Spatial deform. </param>
        /// <returns> Deformed geometry. </returns>
        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            return null;
        }
        #endregion

        #region preview data
        /// <summary>
        /// Compute an aligned boundingbox.
        /// </summary>
        /// <param name="xform"> Transformation to apply to geometry for BoundingBox computation. </param>
        /// <returns> The world aligned boundingbox of the transformed geometry. </returns>
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            return BoundingBox.Empty;
        }

        /// <summary>
        /// Gets the boundingbox for this geometry.
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get { return this.GetBoundingBox(new Transform()); }
        }
        #endregion
    }
}
