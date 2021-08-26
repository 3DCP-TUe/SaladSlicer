﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// Rhino Libs
using Rhino.Geometry;
// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
// Salad Slicer Libs
using SaladSlicer.Core.CodeGeneration;
using SaladSlicer.Core.Slicers;
using SaladSlicer.Gh.Goos.CodeGeneration;

namespace SaladSlicer.Gh.Goos.Slicers
{
    /// <summary>
    /// Represents the GH_CurveSlicer class.
    /// </summary>
    public class GH_CurveSlicer : GH_GeometricGoo<CurveSlicer>, IGH_PreviewData
    {
        #region (de)serialisation
        //TODO
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the GH_CurveSlicer class.
        /// </summary>
        public GH_CurveSlicer()
        {
            this.Value = null;
        }

        /// <summary>
        /// Initializes a new Oject Goo instance from a CurveSlicer instance.
        /// </summary>
        /// <param name="curveSlicer"> CurveSlicer Value to store inside this Goo instance. </param>
        public GH_CurveSlicer(CurveSlicer curveSlicer)
        {
            this.Value = curveSlicer;
        }

        /// <summary>
        /// Returns a complete duplicate of this Goo instance.
        /// </summary>
        /// <returns> A duplicate of the Curve Slicer Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            if (this.Value == null)
            {
                return new GH_CurveSlicer();
            }
            else
            {
                return new GH_CurveSlicer(this.Value.Duplicate());
            }
        }

        /// <summary>
        /// Returns a complete duplicate of this Goo insance.
        /// </summary>
        /// <returns> A duplicate of the Curve Slicer Goo instance. </returns>
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
                return "Null Curve Slicer";
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

            // Cast to IObject
            if (typeof(Q).IsAssignableFrom(typeof(IObject)))
            {
                target = (Q)(object)this.Value;
                return true;
            }

            // Cast to IObject Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Object)))
            {
                target = (Q)(object)new GH_Object(this.Value);
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
                target = (Q)(object)new GH_CurveSlicer(this.Value);
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

            // Cast to Bounding Box
            if (typeof(Q).IsAssignableFrom(typeof(BoundingBox)))
            {
                target = (Q)(object)this.Value.GetBoundingBox(true);
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

            // Cast from CurveSlicer
            if (typeof(CurveSlicer).IsAssignableFrom(source.GetType()))
            {
                this.Value = source as CurveSlicer;
                return true;
            }

            // Cast from CurveSlicer Goo
            if (typeof(GH_CurveSlicer).IsAssignableFrom(source.GetType()))
            {
                GH_CurveSlicer goo = source as GH_CurveSlicer;
                this.Value = goo.Value;
                return true;
            }

            // Cast from IObject
            if (typeof(IObject).IsAssignableFrom(source.GetType()))
            {
                if (source is CurveSlicer slicer)
                {
                    this.Value = slicer;
                    return true;
                }
            }

            // Cast from IObject Goo 
            if (typeof(GH_Object).IsAssignableFrom(source.GetType()))
            {
                GH_Object goo = source as GH_Object;

                if (goo.Value is CurveSlicer slicer)
                {
                    this.Value = slicer;
                    return true;
                }
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
                if (this.Value == null) { return "No internal Curve Slicer instance"; }
                if (this.Value.IsValid) { return string.Empty; }
                return "Invalid Curve Slicer instance.";
            }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Curve Slicer."; }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Curve Slicer"; }
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
                CurveSlicer slicer = Value.Duplicate();
                slicer.Transform(xform);
                return new GH_CurveSlicer(slicer);
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
