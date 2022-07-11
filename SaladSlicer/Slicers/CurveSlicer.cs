﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
// Rhino Libs
using Rhino.Geometry;
// Slicer Salad Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Interfaces;
using SaladSlicer.Utils;
using SaladSlicer.Enumerations;

namespace SaladSlicer.Slicers
{
    /// <summary>
    /// Represents the Planar 2D Slicer class.
    /// </summary>
    [Serializable()]
    public class CurveSlicer : IProgram, ISlicer, IGeometry, IAddVariable
    {
        #region fields
        private Curve _curve;
        private double _distance;
        private List<Plane> _frames = new List<Plane>();
        private readonly Dictionary<string, List<List<double>>> _addedVariables = new Dictionary<string, List<List<double>>>() { };
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
            _addedVariables = slicer.AddedVariables.ToDictionary(entry => entry.Key.Clone() as string, entry => entry.Value.ConvertAll(list => list.ConvertAll(item => item)));
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
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Curve Slicer instance as an ISlicer.
        /// </summary>
        /// <returns> The exact duplicate of this Curve Slicer instance as an ISlicer. </returns>
        public ISlicer DuplicateSlicerObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Curve Slicer instance as an IGeometry.
        /// </summary>
        /// <returns> The exact duplicate of this Curve Slicer instance as an IGeometry. </returns>
        public IGeometry DuplicateGeometryObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Curve Slicer instance as an IAddVariable
        /// </summary>
        /// <returns> The exact duplicate of this Curve Slicer instance as an IAddVariable. </returns>
        public IAddVariable DuplicateAddVariableObject()
        {
            return Duplicate();
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
            CreateFrames();
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
        public void ToProgram(ProgramGenerator programGenerator)
        {
            // Header
            programGenerator.AddSlicerHeader("CURVE SLICER OBJECT", GetLength());

            // Add coordinates
            List<List<string>> coordinates = ProgramGenerator.GetCoordinateCodeLines(this,programGenerator.PrinterSettings);

            for (int i = 0; i < coordinates.Count; i++)
            {
                programGenerator.Program.AddRange(coordinates[i]);
            }
               
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
            _addedVariables.Add(prefix, HelperMethods.MatchAddedVariable(this, values));
        }

        /// <summary>
        /// Adds an additional variable to the program, besides X, Y and Z.
        /// </summary>
        /// <param name="addedVariables"> The added variable(s) stored in a dictionary. </param>
        public void AddVariable(Dictionary<string, List<List<double>>> addedVariables)
        {
            foreach (KeyValuePair<string, List<List<double>>> entry in addedVariables)
            {
                _addedVariables.Add(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Removes the specific prefix from the added variables.
        /// </summary>
        /// <param name="prefix"> The prefix to remove. </param>
        /// <returns> Indicates whether the prefix wis succesfully found and removed. </returns>
        public bool RemoveAddedVariable(string prefix)
        {
            return _addedVariables.Remove(prefix);
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
        /// <param name="type"> The path type. </param>
        /// <returns> The path. </returns>
        public Curve GetPath(PathType type)
        {
            if (type == PathType.Original)
            {
                return GetPath();
            }
            else if (type == PathType.Spline)
            {
                return GetInterpolatedPath();
            }
            else if (type == PathType.Linear)
            {
                return GetLinearizedPath();
            }
            else
            {
                throw new Exception("Incorrect path type.");
            }
        }

        /// <summary>
        /// Returns the original path.
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
            return Curve.CreateInterpolatedCurve(GetPoints(), 3, CurveKnotStyle.Chord);
        }

        /// <summary>
        /// Returns the linearized path.
        /// </summary>
        /// <returns> The linearized path. </returns>
        public Curve GetLinearizedPath()
        {
            return new PolylineCurve(GetPoints());
        }

        /// <summary>
        /// Returns distance of every frame along the path.
        /// </summary>
        /// <param name="type"> The path type. </param>
        /// <returns> List with distances. </returns>
        public List<double> GetDistancesAlongPath(PathType type)
        {
            if (type == PathType.Linear)
            {
                List<double> distances = new List<double>() { };
                distances.Add(0.0);
                double distance = 0.0;

                List<Plane> frames = this.Frames;

                for (int i = 1; i < frames.Count; i++)
                {
                    distance += frames[i].Origin.DistanceTo(frames[i - 1].Origin);
                    distances.Add(distance);
                }

                return distances;
            }
            else
            {
                return Geometry.Frames.GetDistancesAlongCurve(GetPath(type), Frames);
            }
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
            distancesTemp.Add(0.0);
            
            for (int j = 1; j < _frames.Count; j++)
            {
                Point3d point = _frames[j].Origin;
                distance += point.DistanceTo(_frames[j - 1].Origin);
                distancesTemp.Add(distance);
            }
            
            distances.Add(distancesTemp);
            
            return distances;
        }

        /// <summary>
        /// Returns distance of every frame to the previous layer
        /// </summary>
        /// <param name="plane"> Plane to calculate the closest distance to for the first layer. </param>
        /// <param name="dx"> Distance in x-direction. </param>
        /// <param name="dy"> Distance in y-direction.</param>
        /// <param name="dz"> Distance in z-direction.</param>
        /// <returns> List with distaces. </returns>
        public List<List<double>> GetDistanceToPreviousLayer(Plane plane, out List<List<double>> dx, out List<List<double>> dy, out List<List<double>> dz)
        {
            List<List<double>> distances = new List<List<double>>();
            dx = new List<List<double>>();
            dy = new List<List<double>>();
            dz = new List<List<double>>();

            List<double> temp = new List<double>();
            List<double> tempx = new List<double>();
            List<double> tempy = new List<double>();
            List<double> tempz = new List<double>();

            for (int j = 0; j < _frames.Count; j++)
            {
                Point3d point = _frames[j].Origin;
                Point3d closestPoint = plane.ClosestPoint(point);

                temp.Add(point.DistanceTo(closestPoint));
                tempx.Add(Math.Abs(point.X - closestPoint.X));
                tempy.Add(Math.Abs(point.Y - closestPoint.Y));
                tempz.Add(Math.Abs(point.Z - closestPoint.Z));
            }
            
            distances.Add(temp);
            dx.Add(tempx);
            dy.Add(tempy);
            dz.Add(tempz);
            
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
            return GetPath().GetBoundingBox(accurate);
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
        /// Gets the dictionary with variables that have been added to the object.
        /// </summary>
        public Dictionary<string, List<List<double>>> AddedVariables 
        {
            get { return _addedVariables; }
        }
        #endregion
    }
}
