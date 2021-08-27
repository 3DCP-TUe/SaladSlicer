﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Slicer Salad Libs
using SaladSlicer.Core.CodeGeneration;

namespace SaladSlicer.Core.Slicers
{
    /// <summary>
    /// Represents the Planar 2D Slicer class.
    /// </summary>
    public class CurveSlicer : IProgram, ISlicer
    {
        #region fields
        private Curve _curve;
        private double _distance;
        private List<Plane> _frames = new List<Plane>();
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Curve Slicer class.
        /// </summary>
        public CurveSlicer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Curve Slicer class from a Rhino Curve. 
        /// </summary>
        /// <param name="curve"> The curve. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        public CurveSlicer(Curve curve,double distance)
        {
            _curve = curve;
            _distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the Curve Slocer class by duplicating an existing Curve Slicer instance. 
        /// </summary>
        /// <param name="slicer"> The Curve Slicer instance to duplicate. </param>
        public CurveSlicer(CurveSlicer slicer)
        {
            _curve = slicer.Curve.DuplicateCurve();
            _distance = slicer.Distance;
            _frames = new List<Plane>(slicer.Frames);
        }

        /// <summary>
        /// Returns an exact duplicate of this Planar 2D Slicer instance.
        /// </summary>
        /// <returns> The exact duplicate of this Planar 2D Slicer instance. </returns>
        public CurveSlicer Duplicate()
        {
            return new CurveSlicer(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Planar 2D Slicer instance as an IProgram.
        /// </summary>
        /// <returns> The exact duplicate of this Planar 2D Slicer instance as an IProgram. </returns>
        public IProgram DuplicateProgramObject()
        {
            return this.Duplicate() as IProgram;
        }

        /// <summary>
        /// Returns an exact duplicate of this Planar 2D Slicer instance as an IObject.
        /// </summary>
        /// <returns> The exact duplicate of this Planar 2D Slicer instance as an IObject. </returns>
        public ISlicer DuplicateObject()
        {
            return this.Duplicate() as ISlicer;
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            return "Sliced curve";
        }

        /// <summary>
        /// Slices this object.
        /// </summary>
        public void Slice()
        {
            this.CreateFrames();
        }


        /// <summary>
        /// Creates the frames of the path.
        /// </summary>
        private void CreateFrames()
        {
            _frames.Clear();
            _frames = Geometry.Frames.GetFramesByDistanceAndSegment(_curve, _distance, true, true);
        }

        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToSinumerik(ProgramGenerator programGenerator)
        {
            // Header
            programGenerator.AddSlicerHeader("CURVE SLICER OBJECT", this.GetLength());

            // Settings
            programGenerator.Program.Add("BSPLINE");
            programGenerator.Program.Add("G642");
            programGenerator.Program.Add("G90");
            programGenerator.Program.Add(" ");

            // Coords
            for (int i = 0; i < _frames.Count; i++)
            {
                Point3d point = _frames[i].Origin;
                programGenerator.Program.Add($"X{point.X:0.###} Y{point.Y:0.###} Z{point.Z:0.###}");
            }

            // End
            programGenerator.AddFooter();
        }

        /// <summary>
        /// Returns the path.
        /// </summary>
        /// <returns> The path. </returns>
        public Curve GetPath()
        {
            return _curve;
        }

        /// <summary>
        /// Returns the interpolated path.
        /// </summary>
        /// <returns> The interpolated path. </returns>
        public Curve GetInterpolatedPath()
        {
            return Curve.CreateInterpolatedCurve(this.GetPoints(), 3, CurveKnotStyle.Chord);
        }

        /// <summary>
        /// Returns the linearized path.
        /// </summary>
        /// <returns> The linearized path. </returns>
        public Curve GetLinearizedPath()
        {
            return new PolylineCurve(this.GetPoints());
        }

        /// <summary>
        /// Returns the Bounding Box of the object.
        /// </summary>
        /// <returns> The Bounding Box. </returns>
        /// <param name="accurate"> If true, a physically accurate bounding box will be computed. </param>

        public BoundingBox GetBoundingBox(bool accurate)
        {
            return this.GetPath().GetBoundingBox(accurate);
        }

        /// <summary>
        /// Returns the length of the path.
        /// </summary>
        /// <returns> The length of the path. </returns>
        public double GetLength()
        {
            return _curve.GetLength();
        }

        /// <summary>
        /// Returns all the points of the path.
        /// </summary>
        /// <returns> The list with points. </returns>
        public List<Point3d> GetPoints() 
        {
            List<Point3d> points = new List<Point3d>();

            for (int i = 0; i < _frames.Count; i++)
            {
                points.Add(_frames[i].Origin);
            }

            return points;
        }

        /// <summary>
        /// Transforms the geometry.
        /// </summary>
        /// <param name="xform"> Transformation to apply to geometry. </param>
        /// <returns> True on success, false on failure. </returns>
        public bool Transform(Transform xform)
        {
            _curve.Transform(xform);
            
            for (int i = 0; i < _frames.Count; i++)
            {
                Plane frame = _frames[i];
                frame.Transform(xform);
                _frames[i] = frame;
            }
            
            return true;
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
                if (_curve == null) { return false; }
                if (_frames == null) { return false; }
                if (_distance <= 0.0) { return false; }
                if (_frames.Count == 0) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the base curve/contour. 
        /// </summary>
        public Curve Curve
        {
            get { return _curve; }
            set { _curve = value; }
        }

        public List<Curve> Contours
        {
            get { return new List<Curve>() { _curve }; }
        }

        /// <summary>
        /// Gets or sets the desired distance between two frames.
        /// </summary>
        public double Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        /// <summary>
        /// Gets the frames of the path.
        /// </summary>
        public List<Plane> Frames
        {
            get { return _frames; }
        }

        /// <summary>
        /// Gets the frames of the path by layer
        /// </summary>
        public List<List<Plane>> FramesByLayer
        {
            get 
            {
                List<List<Plane>> result = new List<List<Plane>>();
                result.Add(new List<Plane>());
                result[0].AddRange(_frames);
                return result; 
            }
        }

        /// <summary>
        /// Gets frame at the start of the path.
        /// </summary>
        public Plane FrameAtStart
        {
            get { return _frames[0]; }
        }

        /// <summary>
        /// Gets frame at the end of the path.
        /// </summary>
        public Plane FrameAtEnd
        {
            get { return _frames[_frames.Count - 1]; }
        }

        /// <summary>
        /// Gets point at the start of the path.
        /// </summary>
        public Point3d PointAtStart
        {
            get { return _frames[0].Origin; }
        }

        /// <summary>
        /// Gets point at the end of the path.
        /// </summary>
        public Point3d PointAtEnd
        {
            get { return _frames[_frames.Count - 1].Origin; }
        }
        #endregion
    }
}
