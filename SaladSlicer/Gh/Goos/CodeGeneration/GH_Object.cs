// This file is part of SaladSlicer. SaladSlicer is licensed 
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

namespace SaladSlicer.Gh.Goos.CodeGeneration
{
    /// <summary>
    /// Represents the GH_ProgramObject class.
    /// </summary>
    public class GH_Object : GH_GeometricGoo<IObject>, IGH_PreviewData
    {
        #region (de)serialisation
        //TODO
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the GH_ProgramObject class.
        /// </summary>
        public GH_Object()
        {
            this.Value = null;
        }

        /// <summary>
        /// Initializes a new Oject Goo instance from an IObject instance.
        /// </summary>
        /// <param name="programObject"> IObject Value to store inside this Goo instance. </param>
        public GH_Object(IObject programObject)
        {
            this.Value = programObject;
        }

        /// <summary>
        /// Returns a complete duplicate of this Goo instance.
        /// </summary>
        /// <returns> A duplicate of the Object Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            if (this.Value == null)
            {
                return new GH_Object();
            }
            else
            {
                return new GH_Object(this.Value.DuplicateObject());
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
                return "Null Program Object";
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

            // Cast from IObject
            if (typeof(IObject).IsAssignableFrom(source.GetType()))
            {
                this.Value = source as IObject;
                return true;
            }

            // Cast from IObject Goo 
            if (typeof(GH_Object).IsAssignableFrom(source.GetType()))
            {
                GH_Object goo = source as GH_Object;
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
                if (this.Value == null) { return "No internal Program Object instance"; }
                if (this.Value.IsValid) { return string.Empty; }
                return "Invalid Program Object instance.";
            }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Program Object."; }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Program Object"; }
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
                IObject programObject = Value.DuplicateObject();
                programObject.Transform(xform);
                return new GH_Object(programObject);
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
            else if (this.Value.InterpolatedPath == null)
            {
                return BoundingBox.Empty;
            }
            else
            {
                return this.Value.InterpolatedPath.GetBoundingBox(true);
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
            get { return this.GetBoundingBox(new Transform()); ; }
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
            if (this.Value.InterpolatedPath != null)
            {
                args.Pipeline.DrawCurve(this.Value.InterpolatedPath, args.Color, args.Thickness);
            }
            if (this.Value.PointAtStart != null)
            {
                args.Pipeline.DrawPoint(this.Value.PointAtStart, args.Color);
            }
        }
        #endregion
    }
}
