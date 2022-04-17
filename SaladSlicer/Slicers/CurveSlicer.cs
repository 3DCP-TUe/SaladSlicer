﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Slicer Salad Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Interfaces;

namespace SaladSlicer.Slicers
{
    /// <summary>
    /// Represents the Planar 2D Slicer class.
    /// </summary>
    public class CurveSlicer : IProgram, ISlicer, IGeometry, IAddVariable
    {
        #region fields
        private Curve _curve;
        private double _distance;
        private List<Plane> _frames = new List<Plane>();
        private readonly List<List<double>> _addedVariable=new List<List<double>>();
        private readonly List<string> _prefix=new List<string>();
        #endregion

        #region (de)serialisation
        //TODO
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
        public CurveSlicer(Curve curve, double distance)
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
            _prefix = slicer.Prefix;
            _addedVariable = slicer.AddedVariable[0];
        }

        /// <summary>
        /// Returns an exact duplicate of this Curve Slicer instance.
        /// </summary>
        /// <returns> The exact duplicate of this Curve Slicer instance. </returns>
        public CurveSlicer Duplicate()
        {
            return new CurveSlicer(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Curve Slicer instance as an IProgram.
        /// </summary>
        /// <returns> The exact duplicate of this Curve Slicer instance as an IProgram. </returns>
        public IProgram DuplicateProgramObject()
        {
            return this.Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Curve Slicer instance as an ISlicer.
        /// </summary>
        /// <returns> The exact duplicate of this Curve Slicer instance as an ISlicer. </returns>
        public ISlicer DuplicateSlicerObject()
        {
            return this.Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Curve Slicer instance as an IGeometry.
        /// </summary>
        /// <returns> The exact duplicate of this Curve Slicer instance as an IGeometry. </returns>
        public IGeometry DuplicateGeometryObject()
        {
            return this.Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Curve Slicer instance as an IAddVariable
        /// </summary>
        /// <returns> The exact duplicate of this Curve Slicer instance as an IAddVariable. </returns>
        public IAddVariable DuplicateAddVariableObject()
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
            _addedVariable.Add(new List<double>());
        }

        
        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToProgram(ProgramGenerator programGenerator,int programType)
        {
            // Header
            programGenerator.AddSlicerHeader("CURVE SLICER OBJECT", this.GetLength());

            // Coords
            programGenerator.AddCoordinates(_frames, _prefix, _addedVariable);
               
            // End
            programGenerator.AddFooter();
        }


        /// <summary>
        /// Adds an additional variable to the program, besides X, Y and Z.
        /// </summary>
        /// <param name="prefix">Prefix to use for the variable.</param>
        /// <param name="values">List of values to be added.</param>
        public void AddVariable(string prefix, List<List<double>> values)
        {
            _prefix.Add(prefix);
            
            if (_addedVariable[0].Count < 1)
            {
                _addedVariable[0] = values[0];
            }
            else
            {
                _addedVariable.Add(values[0]);
            }
        }

        /// <summary>
        /// Returns the AbsoluteCoordinate object as a string
        /// </summary>
        /// <returns>The string</returns>
        public string ToSingleString()
        {
            throw new Exception("A Slicer Object cannot be represented by a single string");
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
        /// Returns distance of every frame along the curve.
        /// </summary>
        /// <returns> List with distances. </returns>
        public List<List<double>> GetDistancesAlongContours()
        {
            List<List<double>> distances = new List<List<double>>();
            double distance = 0;
            
            List<double> distancesTemp = new List<double>();
            
            for (int j = 0; j < _frames.Count; j++)
            {
                if (j == 0)
                {
                    distancesTemp.Add(distance);
                }
                else
                {
                    Point3d point = _frames[j].Origin;
                    distance += point.DistanceTo(_frames[j - 1].Origin);
                    distancesTemp.Add(distance);
                }
            }
            
            distances.Add(distancesTemp);
            
            return distances;
        }

        /// <summary>
        /// Calculates the distance between every frame and the closest point on the previous layer.
        /// </summary>
        /// <param name="plane">Plane to calculate closest point to for the first layer</param>
        /// <returns></returns>
        public List<List<double>> GetDistanceToPreviousLayer(Plane plane)
        {
            List<List<double>> distances = new List<List<double>>();
            List<double> distancesTemp = new List<double>();
            
            for (int j = 0; j < _frames.Count; j++)
            {
                double distance;
                Point3d point = _frames[j].Origin;
                Point3d point2 = plane.ClosestPoint(point);
                distance = point.DistanceTo(point2);
                distancesTemp.Add(distance);
            }
            
            distances.Add(distancesTemp);
            
            return distances;
        }
        
        /// Returns a list with curvatures of the path at the frame location.
        /// </summary>
        /// <returns> The list with curvatures. </returns>
        public List<List<Vector3d>> GetCurvatures()
        {
            List<List<Vector3d>> result = new List<List<Vector3d>>();

            Curve path = GetPath();

            result.Add(new List<Vector3d>() { });

            for (int i = 0; i < _frames.Count; i++)
            {
                path.ClosestPoint(_frames[i].Origin, out double t);
                result[0].Add(path.CurvatureAt(t));
            }

            return result;
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
        /// Returns the Bounding Box of the object.
        /// </summary>
        /// <returns> The Bounding Box. </returns>
        /// <param name="accurate"> If true, a physically accurate bounding box will be computed. </param>

        public BoundingBox GetBoundingBox(bool accurate)
        {
            return this.GetPath().GetBoundingBox(accurate);
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

        /// <summary>
        /// Gets the contours of the sliced object.
        /// </summary>
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
                List<List<Plane>> result = new List<List<Plane>>
                {
                    new List<Plane>()
                };
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

        /// <summary>
        /// Gets a list of prefixes for variables that have been added to the object.
        /// </summary>
        public List<string> Prefix
        {
            get { return _prefix; }
        }

        /// <summary>
        /// Gets a list of variables that have been added to the object.
        /// </summary>
        public List<List<List<double>>> AddedVariable
        {
            get {
                List<List<List<double>>> result = new List<List<List<double>>>
                {
                    _addedVariable
                };
                return result; }
        } 
    #endregion
    }
}
