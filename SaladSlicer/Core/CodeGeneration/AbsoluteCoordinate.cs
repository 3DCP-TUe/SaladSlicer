﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Salad Libs
using SaladSlicer.Core.Interfaces;

namespace SaladSlicer.Core.CodeGeneration
{
    /// <summary>
    /// Represents an Absolute Coordinate.
    /// </summary>
    public class AbsoluteCoordinate : IProgram, IGeometry
    {
        #region fields
        private Plane _plane;
        #endregion

        #region (de)serialisation
        //TODO
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the AbsoluteCoordinate class.
        /// </summary>         
        public AbsoluteCoordinate()
        {
            _plane = Plane.Unset;
        }

        /// <summary>
        /// Initializes a new instance of the AbsoluteCoordinate class.
        /// </summary>
        /// <param name="plane">Plane representing the origin of the point and optionally the direction (x-axis) of the movement.</param>
        public AbsoluteCoordinate(Plane plane)
        {
            _plane = plane;
        }

        /// <summary>
        /// Initializes a new instance of the Absolute Coordinate class by duplicating an existing Absolute Coordinate instance. 
        /// </summary>
        /// <param name="absoluteCoordinate"> The Absolute Coordinate instance to duplicate. </param>
        public AbsoluteCoordinate(AbsoluteCoordinate absoluteCoordinate)
        {
            _plane = absoluteCoordinate.Plane;
        }

        /// <summary>
        /// Returns an exact duplicate of this Absolute Coordinate instance.
        /// </summary>
        /// <returns> The exact duplicate of this Absolute Coordinate instance. </returns>
        public AbsoluteCoordinate Duplicate()
        {
            return new AbsoluteCoordinate(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Absolute Coordinate instance as an IProgram
        /// </summary>
        /// <returns> The exact duplicate of this Absolute Coordinate instance as an IProgram. </returns>
        public IProgram DuplicateProgramObject()
        {
            return this.Duplicate() as IProgram;
        }

        /// <summary>
        /// Returns an exact duplicate of this Absolute Coordinate instance as an IAddVariable
        /// </summary>
        /// <returns> The exact duplicate of this Absolute Coordinate instance as an IAddVariable. </returns>
        public IAddVariable DuplicateAddVariableObject()
        {
            return this.Duplicate() as IAddVariable;
        }

        /// <summary>
        /// Returns an exact duplicate of this Absolute Coordinater instance as an IGeometry.
        /// </summary>
        /// <returns> The exact duplicate of this Absolute Coordinate instance as an IGeometry. </returns>
        public IGeometry DuplicateGeometryObject()
        {
            return this.Duplicate() as IGeometry;
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            return ($"X{_plane.OriginX:0.###} Y{_plane.OriginY:0.###} Z{_plane.OriginZ:0.###}");
        }

        /// <summary>
        /// Returns the AbsoluteCoordinate object as a string
        /// </summary>
        /// <returns>The string</returns>
        public string ToSingleString()
        {
            return ($"X{_plane.OriginX:0.###} Y{_plane.OriginY:0.###} Z{_plane.OriginZ:0.###}");
        }

        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToProgram(ProgramGenerator programGenerator, int programType)
        {
            List<Plane> plane= new List<Plane>{_plane};
            programGenerator.AddCoordinates(plane, new List<string>(), new List<List<double>>());
        }

        /// <summary>
        /// Returns the Bounding Box of the object.
        /// </summary>
        /// <returns> The Bounding Box. </returns>
        /// <param name="accurate"> If true, a physically accurate bounding box will be computed. If not, a bounding box estimate will be computed. </param>

        public BoundingBox GetBoundingBox(bool accurate)
        {
            return BoundingBox.Empty;
        }

        /// <summary>
        /// Transforms the geometry.
        /// </summary>
        /// <param name="xform"> Transformation to apply to geometry. </param>
        /// <returns> True on success, false on failure. </returns>
        public bool Transform(Transform xform)
        {
            return _plane.Transform(xform);
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
                if (_plane == null) { return false; } 
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the plane.
        /// </summary>
        public Plane Plane
        {
            get { return _plane; }
            set { _plane = value; }
        }

        /// <summary>
        /// Empty contour property.
        /// </summary>
        public List<Curve> Contours
        {
            get { return new List<Curve>(); }
        }

        /// <summary>
        /// Gets a list of prefixes for variables that have been added to the object.
        /// </summary>
        public List<string> Prefix
        {
            get { return new List<string>(); }
        }

        /// <summary>
        /// Gets a list of variables that have been added to the object.
        /// </summary>
        public List<List<List<double>>> AddedVariable
        {
            get
            {
                return new List<List<List<double>>>();
            }
        }
        #endregion
    }
}
