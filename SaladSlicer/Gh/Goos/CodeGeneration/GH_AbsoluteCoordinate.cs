// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// Grasshopper Libs
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
// Rhino Libs
using Rhino.Geometry;
// Salad Slicer Libs
using SaladSlicer.Core.CodeGeneration;
using SaladSlicer.Core.Interfaces;

namespace SaladSlicer.Gh.Goos.CodeGeneration
{
    /// <summary>
    /// Represents the GH_AbsoluteCoordinate class.
    /// </summary>
    public class GH_AbsoluteCoordinate : GH_GeometricGoo<AbsoluteCoordinate>, IGH_PreviewData, IGH_Goo
    {
        #region (de)serialisation
        //TODO
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the GH_AbsoluteCoordinate class.
        /// </summary>
        public GH_AbsoluteCoordinate()
        {
            this.Value = null;
        }

        /// <summary>
        /// Initializes a new Absolute Coordinate Goo instance from a Absolute Coordinate instance.
        /// </summary>
        /// <param name="absoluteCoordinate"> Absolute Coordinate Value to store inside this Goo instance. </param>
        public GH_AbsoluteCoordinate(AbsoluteCoordinate absoluteCoordinate)
        {
            this.Value = absoluteCoordinate;
        }

        /// <summary>
        /// Returns a complete duplicate of this Goo instance.
        /// </summary>
        /// <returns> A duplicate of the Absolute Coordinate Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            if (this.Value == null)
            {
                return new GH_AbsoluteCoordinate();
            }
            else
            {
                return new GH_AbsoluteCoordinate(this.Value.Duplicate());
            }
        }

        /// <summary>
        /// Returns a complete duplicate of this Goo insance.
        /// </summary>
        /// <returns> A duplicate of the Closed Planar 2D Slicer Goo instance. </returns>
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
                return "Null Absolute Coordinate";
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

            // Cast to Absolute Coordinate
            if (typeof(Q).IsAssignableFrom(typeof(AbsoluteCoordinate)))
            {
                target = (Q)(object)this.Value;
                return true;
            }

            // Cast to Absolute Coordinate Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_AbsoluteCoordinate)))
            {
                target = (Q)(object)new GH_AbsoluteCoordinate(this.Value);
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

            // Cast to Plane
            if (typeof(Q).IsAssignableFrom(typeof(Plane)))
            {
                target = (Q)(object)this.Value.Plane;
                return true;
            }

            // Cast to Plane Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Plane)))
            {
                target = (Q)(object)new GH_Plane(this.Value.Plane);
                return true;
            }

            // Cast to Plane
            if (typeof(Q).IsAssignableFrom(typeof(Point3d)))
            {
                target = (Q)(object)this.Value.Plane.Origin;
                return true;
            }

            // Cast to Plane Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
            {
                target = (Q)(object)new GH_Point(this.Value.Plane.Origin);
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

            // Cast from Absolute Coordinate
            if (typeof(AbsoluteCoordinate).IsAssignableFrom(source.GetType()))
            {
                this.Value = source as AbsoluteCoordinate;
                return true;
            }

            // Cast from Plane
            if (typeof(Plane).IsAssignableFrom(source.GetType()))
            {
                this.Value = new AbsoluteCoordinate((Plane)source);
                return true;
            }

            // Cast from Plane Goo
            if (typeof(GH_Plane).IsAssignableFrom(source.GetType()))
            {
                GH_Plane goo = source as GH_Plane;
                this.Value = new AbsoluteCoordinate(goo.Value);
                return true;
            }

            // Cast from Point
            if (typeof(Point3d).IsAssignableFrom(source.GetType()))
            {
                Plane plane = new Plane((Point3d)source, new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
                this.Value = new AbsoluteCoordinate(plane);
                return true;
            }

            // Cast from Point Goo
            if (typeof(GH_Point).IsAssignableFrom(source.GetType()))
            {
                GH_Point goo = source as GH_Point;
                Plane plane = new Plane(goo.Value, new Vector3d(1, 0, 0), new Vector3d(0, 1, 0));
                this.Value = new AbsoluteCoordinate(plane);
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
                if (this.Value == null) { return "No internal Absolute Coordinate instance"; }
                if (this.Value.IsValid) { return string.Empty; }
                return "Invalid Absolute Coordinate instance: Did you define a Text?";
            }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Absolute Coordinate."; }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Absolute Coordinate"; }
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
                AbsoluteCoordinate absoluteCoordinate = Value.Duplicate();
                absoluteCoordinate.Transform(xform);
                return new GH_AbsoluteCoordinate(absoluteCoordinate);
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
            if (this.Value.Plane.Origin != null)
            {
                args.Pipeline.DrawPoint(this.Value.Plane.Origin, args.Color);
            }
        }
        #endregion
    }
}
