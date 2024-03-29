﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GH_IO.Serialization;
// Salad Slicer Libs
using SaladSlicer.Slicers;
using SaladSlicer.Interfaces;
using SaladSlicer.Gh.Goos.CodeGeneration;
using SaladSlicer.Utils;

namespace SaladSlicer.Gh.Goos.Slicers
{
    /// <summary>
    /// Represents the GH_SlicerObject class.
    /// </summary>
    public class GH_SlicerObject : GH_GeometricGoo<ISlicer>, IGH_PreviewData
    {
        #region (de)serialisation
        /// <summary>
        /// IO key for (de)serialisation of the value inside this Goo.
        /// </summary>
        private const string IoKey = "Slicer Object";

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
            Value = (ISlicer)HelperMethods.ByteArrayToObject(array);

            return true;
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the GH_SlicerObject class.
        /// </summary>
        public GH_SlicerObject()
        {
            this.Value = null;
        }

        /// <summary>
        /// Initializes a new Slicer Oject Goo instance from an ISlicer instance.
        /// </summary>
        /// <param name="slicerObject"> ISlicer Value to store inside this Goo instance. </param>
        public GH_SlicerObject(ISlicer slicerObject)
        {
            this.Value = slicerObject;
        }

        /// <summary>
        /// Returns a complete duplicate of this Goo instance.
        /// </summary>
        /// <returns> A duplicate of the Object Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            if (this.Value == null)
            {
                return new GH_SlicerObject();
            }
            else
            {
                return new GH_SlicerObject(this.Value.DuplicateSlicerObject());
            }
        }

        /// <summary>
        /// Returns a complete duplicate of this Goo insance.
        /// </summary>
        /// <returns> A duplicate of the Object Goo instance. </returns>
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
                return "Null Slicer Object";
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
            if (this.Value == null)
            {
                target = default;
                return false;
            }

            // Cast to ISlicer
            if (typeof(Q).IsAssignableFrom(typeof(ISlicer)))
            {
                target = (Q)(object)this.Value;
                return true;
            }

            // Cast to ISlicer Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_SlicerObject)))
            {
                target = (Q)(object)new GH_SlicerObject(this.Value);
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
                target = (Q)(object)new GH_ProgramObject(this.Value as IProgram);
                return true;
            }

            // Cast to OpenPlanar2DSlicer
            if (typeof(Q).IsAssignableFrom(typeof(OpenPlanar2DSlicer)))
            {
                target = (Q)(object)this.Value;
                return true;
            }

            // Cast to GH_OpenPlanar2DSlicer
            if (typeof(Q).IsAssignableFrom(typeof(GH_OpenPlanar2DSlicer)))
            {
                target = (Q)(object)new GH_OpenPlanar2DSlicer(this.Value as OpenPlanar2DSlicer);
                return true;
            }

            // Cast to ClosedPlanar2DSlicer
            if (typeof(Q).IsAssignableFrom(typeof(ClosedPlanar2DSlicer)))
            {
                target = (Q)(object)this.Value;
                return true;
            }

            // Cast to GH_ClosedPlanar2DSlicer
            if (typeof(Q).IsAssignableFrom(typeof(GH_ClosedPlanar2DSlicer)))
            {
                target = (Q)(object)new GH_ClosedPlanar2DSlicer(this.Value as ClosedPlanar2DSlicer);
                return true;
            }

            // Cast to CurveSlicer
            if (typeof(Q).IsAssignableFrom(typeof(CurveSlicer)))
            {
                target = (Q)(object)this.Value;
                return true;
            }

            // Cast to CurveSlicer Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_CurveSlicer)))
            {
                target = (Q)(object)new GH_CurveSlicer(this.Value as CurveSlicer);
                return true;
            }

            // Cast to Curve
            if (typeof(Q).IsAssignableFrom(typeof(Curve)))
            {
                target = (Q)(object)this.Value.GetPath();
                return true;
            }

            // Cast to Curve Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
            {
                target = (Q)(object)new GH_Curve(this.Value.GetPath());
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

            // Cast from ISlicer
            if (typeof(ISlicer).IsAssignableFrom(source.GetType()))
            {
                this.Value = source as ISlicer;
                return true;
            }

            // Cast from ISlicer Goo 
            if (typeof(GH_SlicerObject).IsAssignableFrom(source.GetType()))
            {
                GH_SlicerObject goo = source as GH_SlicerObject;
                this.Value = goo.Value;
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
                if (this.Value == null) { return "No internal Slicer Object instance"; }
                if (this.Value.IsValid) { return string.Empty; }
                return "Invalid Slicer Object instance.";
            }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Slicer Object."; }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Slicer Object"; }
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

            else if (Value is IGeometry geo)
            {
                IGeometry geoObject = geo.DuplicateGeometryObject();
                geoObject.Transform(xform);
                return new GH_SlicerObject(geoObject as ISlicer);
            }

            else
            {
                return null;
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
            if (this.Value == null)
            {
                return BoundingBox.Empty;
            }
            else if (this.Value.GetPath() == null)
            {
                return BoundingBox.Empty;
            }
            else
            {
                return this.Value.GetPath().GetBoundingBox(true);
            }
        }

        /// <summary>
        /// Gets the boundingbox for this geometry.
        /// </summary>
        public override BoundingBox Boundingbox
        {
            get { return this.GetBoundingBox(new Transform()); }
        }

        /// <summary>
        /// Gets the clipping box for this data.
        /// </summary>
        public BoundingBox ClippingBox
        {
            get { return this.GetBoundingBox(new Transform()); }
        }

        /// <summary>
        /// Implement this function to draw all shaded meshes. 
        /// If the viewport does not support shading, this function will not be called.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {

        }

        /// <summary>
        /// Implement this function to draw all wire and point previews.
        /// </summary>
        /// <param name="args"> Drawing arguments. </param>
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (this.Value.GetPath() != null)
            {
                args.Pipeline.DrawCurve(this.Value.GetPath(), args.Color, args.Thickness);
            }

            if (this.Value.PointAtStart != null)
            {
                args.Pipeline.DrawPoint(this.Value.PointAtStart, args.Color);
            }
        }
        #endregion
    }
}
