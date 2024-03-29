﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs]
using System;
using System.Linq;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Slicer Salad Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Interfaces;
using SaladSlicer.Geometry;
using SaladSlicer.Utils;
using SaladSlicer.Enumerations;

namespace SaladSlicer.Slicers
{
    /// <summary>
    /// Represents the Planar 2D Slicer class.
    /// </summary>
    [Serializable()]
    public class ContoursTransitionsSlicer : IProgram, ISlicer, IGeometry, IAddVariable
    {
        #region fields
        private readonly List<Curve> _contours = new List<Curve>();
        private readonly List<Curve> _transitions = new List<Curve>();
        private List<Curve> _path = new List<Curve>();
        private double _distance;
        private readonly List<List<Plane>> _framesByLayer = new List<List<Plane>>() { };
        private readonly Dictionary<string, List<List<string>>> _addedVariables = new Dictionary<string, List<List<string>>>() { };
        private readonly List<List<Plane>> _framesInContours = new List<List<Plane>>() { };
        private readonly List<List<Plane>> _framesInTransitions = new List<List<Plane>>() { };
        #endregion

        #region (de)serialisation
        //TODO
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Curve Slicer class.
        /// </summary>
        public ContoursTransitionsSlicer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Curve Slicer class from a Rhino Curve. 
        /// </summary>
        /// <param name="curve"> The curve. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        public ContoursTransitionsSlicer(IList<Curve> curve, IList<Curve> transitions, double distance)
        {
            _contours = curve as List<Curve>;
            _transitions = transitions as List<Curve>;
            _distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the Curve Slocer class by duplicating an existing Curve Slicer instance. 
        /// </summary>
        /// <param name="slicer"> The Curve Slicer instance to duplicate. </param>
        public ContoursTransitionsSlicer(ContoursTransitionsSlicer slicer)
        {
            _contours = slicer.Contours.ConvertAll(curve => curve.DuplicateCurve());
            _transitions = slicer.Transitions.ConvertAll(curve => curve.DuplicateCurve());
            _path = slicer.Path.ConvertAll(curve => curve.DuplicateCurve());
            _distance = slicer.Distance;
            _addedVariables = slicer.AddedVariables.ToDictionary(entry => entry.Key.Clone() as string, entry => entry.Value.ConvertAll(list => list.ConvertAll(item => item)));
            _framesByLayer = slicer.FramesByLayer.ConvertAll(list => new List<Plane>(list));
            _framesInTransitions = slicer._framesInTransitions.ConvertAll(list => new List<Plane>(list));
            _framesInContours = slicer._framesInContours.ConvertAll(list => new List<Plane>(list));
        }

        /// <summary>
        /// Returns an exact duplicate of this  Curves Transitions Slicer instance.
        /// </summary>
        /// <returns> The exact duplicate of this Curves Transitions Slicer instance. </returns>
        public ContoursTransitionsSlicer Duplicate()
        {
            return new ContoursTransitionsSlicer(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Curves Transitions Slicer as an IProgram.
        /// </summary>
        /// <returns> The exact duplicate of this Curves Transitions Slicer as an IProgram. </returns>
        public IProgram DuplicateProgramObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Curves Transitions Slicer instance as an ISlicer.
        /// </summary>
        /// <returns> The exact duplicate of this Curves Transitions Slicer instance as an ISlicer. </returns>
        public ISlicer DuplicateSlicerObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Curves Transitions Slicer instance as an IGeometry.
        /// </summary>
        /// <returns> The exact duplicate of this Curves Transitions Slicer instance as an IGeometry. </returns>
        public IGeometry DuplicateGeometryObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Curves Transitions Slicer instance as an IAddVariable
        /// </summary>
        /// <returns> The exact duplicate of this Curves Transitions Slicer instance as an IAddVariable. </returns>
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
            return "Curves and Transitions as Slicer Object";
        }

        /// <summary>
        /// Slices this object.
        /// </summary>
        public void Slice()
        {
            CreatePath();
            CreateFrames();
        }

        /// <summary>
        /// Creates the path of the object
        /// </summary>
        private void CreatePath()
        {
            _path.Clear();
            _path = Curves.WeaveCurves(_contours, _transitions);
        }

        /// <summary>
        /// Creates the frames of the path.
        /// </summary>
        private void CreateFrames()
        {
            _framesByLayer.Clear();

            for (int i = 0; i < _contours.Count; i++)
            {
                _framesByLayer.Add(new List<Plane>() { });
                _framesInContours.Add(new List<Plane>() { });
                _framesInTransitions.Add(new List<Plane>() { });
            }

            for (int i = 0; i < _contours.Count; i++)
            {
                // Contours
                _framesByLayer[i].AddRange(Geometry.Frames.GetFramesByDistanceAndSegment(_path[i * 2], _distance, true, true));
                _framesInContours[i].AddRange(Geometry.Frames.GetFramesByDistanceAndSegment(_path[i * 2], _distance, true, true));

                // Transitions
                if (i < _contours.Count - 1)
                {
                    _framesByLayer[i].AddRange(Geometry.Frames.GetFramesByDistanceAndSegment(_path[(i * 2) + 1], _distance, false, false));
                    _framesInTransitions[i].AddRange(Geometry.Frames.GetFramesByDistanceAndSegment(_path[(i * 2) + 1], _distance, false, false));
                }
            }
        }

        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToProgram(ProgramGenerator programGenerator)
        {
            // Header
            programGenerator.AddSlicerHeader("CUSTOM SLICER OBJECT", GetLength());

            // Add coordinates
            List<List<string>> coordinates = ProgramGenerator.GetCoordinateCodeLines(this, programGenerator.PrinterSettings);

            for (int i = 0; i < coordinates.Count; i++)
            {
                programGenerator.Program.Add(" ");
                programGenerator.Program.Add($"; LAYER {i + 1:0}");
                programGenerator.Program.AddRange(coordinates[i]);
            }

            // End
            programGenerator.AddFooter();
        }

        /// <summary>
        /// Collects the data of this object to the program generator to generate the path.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToPath(ProgramGenerator programGenerator)
        {
            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                for (int j = 0; j < _framesByLayer[i].Count; j++)
                {
                    programGenerator.Points.Add(_framesByLayer[i][j].Origin);
                    programGenerator.InterpolationTypes.Add(programGenerator.InterpolationType);
                }
            }
        }

        /// <summary>
        /// Adds an additional variable to the program, besides X, Y and Z.
        /// </summary>
        /// <param name="prefix">Prefix to use for the variable.</param>
        /// <param name="values">List of values to be added.</param>
        public void AddVariable(string prefix, List<List<string>> values)
        {
            _addedVariables.Add(prefix, HelperMethods.MatchAddedVariable(this, values));
        }

        /// <summary>
        /// Adds an additional variable to the program, besides X, Y and Z.
        /// </summary>
        /// <param name="addedVariables"> The added variable(s) stored in a dictionary. </param>
        public void AddVariable(Dictionary<string, List<List<string>>> addedVariables)
        {
            foreach (KeyValuePair<string, List<List<string>>> entry in addedVariables)
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
            return Curve.JoinCurves(_path)[0];
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
        /// <returns> Array with distances. </returns>
        public double[] GetDistancesAlongPath(PathType type)
        {
            if (type == PathType.Linear)
            {
                double[] distances = new double[Frames.Count];
                double distance = 0.0;

                distances[0] = distance;

                for (int i = 1; i < Frames.Count; i++)
                {
                    distance += Frames[i].Origin.DistanceTo(Frames[i - 1].Origin);
                    distances[i] = distance;
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

            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                List<double> distancesTemp = new List<double>();
                distancesTemp.Add(0.0);
                double distance = 0;

                for (int j = 1; j < _framesByLayer[i].Count; j++)
                {
                    Point3d point = _framesByLayer[i][j].Origin;
                    distance += point.DistanceTo(_framesByLayer[i][j - 1].Origin);
                    distancesTemp.Add(distance);
                }

                distances.Add(distancesTemp);
            }

            return distances;
        }

        /// Returns a list with curvatures of the path at the frame location.
        /// </summary>
        /// <returns> The list with curvatures. </returns>
        public List<List<Vector3d>> GetCurvatures()
        {
            List<List<Vector3d>> result = new List<List<Vector3d>>();

            Curve path = GetInterpolatedPath();

            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                result.Add(new List<Vector3d>() { });

                for (int j = 0; j < _framesByLayer[i].Count; j++)
                {
                    path.ClosestPoint(_framesByLayer[i][j].Origin, out double t);
                    result[i].Add(path.CurvatureAt(t));
                }
            }

            return result;
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

            for (int i = 0; i < _framesInContours.Count; i++)
            {
                List<double> temp = new List<double>();
                List<double> tempx = new List<double>();
                List<double> tempy = new List<double>();
                List<double> tempz = new List<double>();

                //Contours
                if (i == 0)
                {
                    for (int j = 0; j < _framesInContours[i].Count; j++)
                    {
                        Point3d point = _framesInContours[i][j].Origin;
                        Point3d closestPoint = plane.ClosestPoint(point);

                        temp.Add(point.DistanceTo(closestPoint));
                        tempx.Add(Math.Abs(point.X - closestPoint.X));
                        tempy.Add(Math.Abs(point.Y - closestPoint.Y));
                        tempz.Add(Math.Abs(point.Z - closestPoint.Z));
                    }
                }
                else
                {
                    for (int j = 0; j < _framesInContours[i].Count; j++)
                    {
                        Point3d point = _framesInContours[i][j].Origin;
                        _contours[i - 1].ClosestPoint(point, out double parameter);

                        Point3d closestPoint = _contours[i - 1].PointAt(parameter);

                        temp.Add(point.DistanceTo(closestPoint));
                        tempx.Add(Math.Abs(point.X - closestPoint.X));
                        tempy.Add(Math.Abs(point.Y - closestPoint.Y));
                        tempz.Add(Math.Abs(point.Z - closestPoint.Z));
                    }
                }

                //Transitions: linearly interpolate between last distance on this contour and first on the next contour
                if (i < _framesInContours.Count - 1)
                {
                    //Last distance on current contour
                    double firstDistance = temp[temp.Count - 1];
                    double firstDistanceX = tempx[tempx.Count - 1];
                    double firstDistanceY = tempy[tempy.Count - 1];
                    double firstDistanceZ = tempz[tempz.Count - 1];

                    //Calculate closest point for first frame in next contour
                    Point3d tempPoint = _framesInContours[i + 1][0].Origin;
                    _contours[i].ClosestPoint(tempPoint, out double tempParameter);
                    Point3d tempClosestPoint = _contours[i].PointAt(tempParameter);

                    double secondDistance = tempPoint.DistanceTo(tempClosestPoint);
                    double secondDistanceX = Math.Abs(tempPoint.X - tempClosestPoint.X);
                    double secondDistanceY = Math.Abs(tempPoint.Y - tempClosestPoint.Y);
                    double secondDistanceZ = Math.Abs(tempPoint.Z - tempClosestPoint.Z);

                    //Linearly interpolate
                    for (int j = 0; j < _framesInTransitions[i].Count; j++)
                    {
                        temp.Add(firstDistance + ((j + 1) * (secondDistance - firstDistance) / (_framesInTransitions[i].Count + 1)));
                        tempx.Add(firstDistanceX + ((j + 1) * (secondDistanceX - firstDistanceX) / (_framesInTransitions[i].Count + 1)));
                        tempy.Add(firstDistanceY + ((j + 1) * (secondDistanceY - firstDistanceY) / (_framesInTransitions[i].Count + 1)));
                        tempz.Add(firstDistanceZ + ((j + 1) * (secondDistanceZ - firstDistanceZ) / (_framesInTransitions[i].Count + 1)));
                    }
                }

                distances.Add(temp);
                dx.Add(tempx);
                dy.Add(tempy);
                dz.Add(tempz);

            }

            return distances;
        }

        /// <summary>
        /// Returns the length of the path.
        /// </summary>
        /// <returns> The length of the path. </returns>
        public double GetLength()
        {
            double length = 0.0;

            for (int i = 0; i < _path.Count; i++)
            {
                length += _path[i].GetLength();
            }
            return length;
        }


        /// <summary>
        /// Returns all the points of the path.
        /// </summary>
        /// <returns> The list with points. </returns>
        public List<Point3d> GetPoints()
        {
            List<Point3d> points = new List<Point3d>();
            List<Plane> frames = Frames;

            for (int i = 0; i < frames.Count; i++)
            {
                points.Add(frames[i].Origin);
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
            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                for (int j = 0; j < _framesByLayer[i].Count; j++)
                {
                    Plane frame = _framesByLayer[i][j];
                    frame.Transform(xform);
                    _framesByLayer[i][j] = frame;
                }
            }

            for (int i = 0; i < _path.Count; i++)
            {
                _path[i].Transform(xform);
            }

            for (int i = 0; i < _contours.Count; i++)
            {
                _contours[i].Transform(xform);
            }

            for (int i = 0; i < _transitions.Count; i++)
            {
                _transitions[i].Transform(xform);
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
                if (_contours == null) { return false; }
                if (_transitions == null) { return false; }
                if (_framesByLayer == null) { return false; }
                if (_distance <= 0.0) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets the contours of the sliced object.
        /// </summary>
        public List<Curve> Contours
        {
            get { return _contours; }
        }

        public List<Curve> Transitions
        {
            get { return _transitions; }
        }

        /// <summary>
        /// Gets the path as a list with curves. 
        /// </summary>
        public List<Curve> Path
        {
            get { return _path; }
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
        /// Gets the frames of the path sorted by layer. 
        /// </summary>
        public List<List<Plane>> FramesByLayer
        {
            get { return _framesByLayer; }
        }

        /// <summary>
        /// Gets the frames of the path.
        /// </summary>
        public List<Plane> Frames
        {
            get
            {
                List<Plane> frames = new List<Plane>() { };

                for (int i = 0; i < _framesByLayer.Count; i++)
                {
                    frames.AddRange(_framesByLayer[i]);
                }

                return frames;
            }
        }

        /// <summary>
        /// Gets frame at the start of the path.
        /// </summary>
        public Plane FrameAtStart
        {
            get { return _framesByLayer[0][0]; }
        }

        /// <summary>
        /// Gets frame at the end of the path.
        /// </summary>
        public Plane FrameAtEnd
        {
            get { return _framesByLayer[_framesByLayer.Count - 1][_framesByLayer[_framesByLayer.Count - 1].Count - 1]; }
        }

        /// <summary>
        /// Gets point at the start of the path.
        /// </summary>
        public Point3d PointAtStart
        {
            get { return FrameAtStart.Origin; }
        }

        /// <summary>
        /// Gets point at the end of the path.
        /// </summary>
        public Point3d PointAtEnd
        {
            get { return FrameAtEnd.Origin; }
        }

        /// <summary>
        /// Gets the dictionary with variables that have been added to the object.
        /// </summary>
        public Dictionary<string, List<List<string>>> AddedVariables
        {
            get { return _addedVariables; }
        }
        #endregion
    }
}
