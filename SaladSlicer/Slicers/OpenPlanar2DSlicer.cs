﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Slicer Salad Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Geometry;
using SaladSlicer.Geometry.Seams;
using SaladSlicer.Enumerations;
using SaladSlicer.Interfaces;
using SaladSlicer.Utils;

namespace SaladSlicer.Slicers
{
    /// <summary>
    /// Represents the Open Planar 2D Slicer class.
    /// </summary>
    [Serializable()]
    public class OpenPlanar2DSlicer : IProgram, ISlicer, IGeometry, IAddVariable
    {
        #region fields
        private Curve _baseContour;
        private double _distance;
        private List<Curve> _path = new List<Curve>();
        private List<Curve> _contours = new List<Curve>();
        private List<double> _heights = new List<double>();
        private readonly List<List<Plane>> _framesByLayer = new List<List<Plane>>() { };
        private readonly Dictionary<string, List<List<string>>> _addedVariables = new Dictionary<string, List<List<string>>>() { };
        #endregion

        #region (de)serialisation
        //TODO
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Open Planar 2D Slicer class.
        /// </summary>
        public OpenPlanar2DSlicer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Open Planar 2D Slicer class from a list with absolute layer heights. 
        /// </summary>
        /// <param name="curve"> The base contour. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        /// <param name="heights"> A list with absolute layer heights. </param>
        public OpenPlanar2DSlicer(Curve curve, double distance, IList<double> heights)
        {
            _baseContour = curve;
            _heights = heights as List<double>;
            _distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the Open Planar 2D Slicer class from a given number of layers and layer thickness.
        /// </summary>
        /// <param name="curve"> The base contour. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        /// <param name="height"> The layer height. </param>
        /// <param name="layers"> The number of layers. </param>
        public OpenPlanar2DSlicer(Curve curve, double distance, double height, int layers)
        {
            _baseContour = curve;
            _heights.AddRange(Enumerable.Repeat(height, layers).ToList());
            _distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the Open Planar 2D Slicer class by duplicating an existing Open Planar 2D Slicer instance. 
        /// </summary>
        /// <param name="slicer"> The Open Planar 2D Slicer instance to duplicate. </param>
        public OpenPlanar2DSlicer(OpenPlanar2DSlicer slicer)
        {
            _baseContour = slicer.BaseContour.DuplicateCurve();
            _heights = new List<double>(slicer.Heights);
            _distance = slicer.Distance;
            _path = slicer.Path.ConvertAll(curve => curve.DuplicateCurve());
            _contours = slicer.Contours.ConvertAll(curve => curve.DuplicateCurve());
            _addedVariables = slicer.AddedVariables.ToDictionary(entry => entry.Key.Clone() as string, entry => entry.Value.ConvertAll(list => list.ConvertAll(item => item)));
            _framesByLayer = slicer.FramesByLayer.ConvertAll(list => new List<Plane>(list));
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar 2D Slicer instance.
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar 2D Slicer instance. </returns>
        public OpenPlanar2DSlicer Duplicate()
        {
            return new OpenPlanar2DSlicer(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar 2D Slicer instance as an IProgram.
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar 2D Slicer instance as an IProgram. </returns>
        public IProgram DuplicateProgramObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar 2D Slicer instance as an ISlicer.
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar 2D Slicer instance as an ISlicer. </returns>
        public ISlicer DuplicateSlicerObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar 2D Slicer instance as an IGeometry.
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar 2D Slicer instance as an IGeometry. </returns>
        public IGeometry DuplicateGeometryObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar 2D Slicer instance as an IAddVariable
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar 2D Slicer instance as an IAddVariable. </returns>
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
            return "Open 2.5 Planar Object";
        }

        /// <summary>
        /// Slices this object.
        /// </summary>
        public void Slice()
        {
            CreateContours();
            CreatePath();
            CreateFrames();
        }

        /// <summary>
        /// Creates the contours of this object.
        /// </summary>
        private void CreateContours()
        {
            _contours.Clear();

            for (int i = 0; i < _heights.Count; i++)
            {
                Curve contour = _baseContour.DuplicateCurve();
                contour.Domain = new Interval(0, contour.GetLength());
                contour.Translate(0, 0, _heights[i]);
                _contours.Add(contour.DuplicateCurve());
            }

            _contours = Curves.AlternateCurves(_contours);
        }

        /// <summary>
        /// Creates the path of this object.
        /// </summary>
        private void CreatePath()
        {
            _path.Clear();
            _path = Curves.WeaveCurves(_contours, Transitions.LinearTransitions(_contours));
        }

        /// <summary>
        /// Creates the frames of the path.
        /// </summary>
        private void CreateFrames()
        {
            _framesByLayer.Clear();
            _contours = Curves.AlternateCurves(_contours);

            for (int i = 0; i < _contours.Count; i++)
            {
                _framesByLayer.Add(Geometry.Frames.GetFramesByDistanceAndSegment(_contours[i], _distance, true, true));
            }

            for (int i = 1; i < _framesByLayer.Count; i += 2)
            {
                _framesByLayer[i].Reverse();
            }

            _contours = Curves.AlternateCurves(_contours);
        }

        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToProgram(ProgramGenerator programGenerator)
        {
            // Header
            programGenerator.AddSlicerHeader("2.5D OPEN PLANAR OBJECT", _contours.Count, GetLength());

            // Get coordinates
            List<List<string>> coordinates = ProgramGenerator.GetCoordinateCodeLines(this, programGenerator.PrinterSettings);

            // Sinumerik
            if (programGenerator.PrinterSettings.ProgramType == ProgramType.Sinumerik)
            {
                for (int i = 0; i < coordinates.Count; i++)
                {
                    programGenerator.Program.Add(" ");
                    programGenerator.Program.Add($"; LAYER {i + 1:0}");

                    // Add TANGOF for all layers but the first
                    if (i != 0)
                    {
                        if (programGenerator.PrinterSettings.IsTangentialControlEnabled)
                        {
                            if (programGenerator.PrinterSettings.InterpolationType == InterpolationType.Spline)
                            {
                                programGenerator.Program.Add("G1");
                            }

                            programGenerator.Program.Add("TANGOF(C)");
                        }
                    }

                    // Move 1 point with the TANGOF
                    programGenerator.Program.Add(coordinates[i][0]);

                    // Turn TANGON again
                    if (programGenerator.PrinterSettings.IsTangentialControlEnabled)
                    {
                        if (i % 2 == 0)
                        {
                            programGenerator.Program.Add("TANGON(C, 0)");
                        }
                        else
                        {
                            programGenerator.Program.Add("TANGON(C, 180)");
                        }

                        if (programGenerator.PrinterSettings.InterpolationType == InterpolationType.Spline)
                        {
                            programGenerator.Program.Add("BSPLINE");
                            programGenerator.Program.Add("G642");
                        }
                    }

                    // Add the rest of the coordinates
                    programGenerator.Program.AddRange(coordinates[i].GetRange(1, coordinates[i].Count - 1));
                }
            }

            // Marlin
            else if (programGenerator.PrinterSettings.ProgramType == ProgramType.Marlin)
            {
                for (int i = 0; i < coordinates.Count; i++)
                {
                    programGenerator.Program.Add(" ");
                    programGenerator.Program.Add($"; LAYER {i + 1:0}, {_framesByLayer[i].Count} Points");
                    programGenerator.Program.AddRange(coordinates[i]);
                }
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
                    if (j == 0)
                    {
                        if (i == 0)
                        {
                            programGenerator.Points.Add(_framesByLayer[i][j].Origin);
                            programGenerator.InterpolationTypes.Add(programGenerator.InterpolationType);
                        }
                        else
                        {
                            programGenerator.Points.Add(_framesByLayer[i][j].Origin);
                            programGenerator.InterpolationTypes.Add(InterpolationType.Linear);
                        }
                    }

                    else
                    {
                        programGenerator.Points.Add(_framesByLayer[i][j].Origin);
                        programGenerator.InterpolationTypes.Add(InterpolationType.Spline);
                    }
                }
            }

            programGenerator.InterpolationType = InterpolationType.Spline;
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
        /// Returns the OpenPlanar2DSlicer object as a string
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
            List<Curve> curves = new List<Curve>() { };
            List<List<Point3d>> points = GetPointsByLayer();

            for (int i = 0; i < points.Count; i++)
            {
                curves.Add(Curve.CreateInterpolatedCurve(points[i], 3, CurveKnotStyle.Chord));
            }

            curves = Curves.WeaveCurves(curves, Transitions.LinearTransitions(curves));

            Curve result = Curve.JoinCurves(curves)[0];

            return result;
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

            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                List<double> temp = new List<double>();
                List<double> tempx = new List<double>();
                List<double> tempy = new List<double>();
                List<double> tempz = new List<double>();

                if (i == 0)
                {
                    for (int j = 0; j < _framesByLayer[i].Count; j++)
                    {
                        Point3d point = _framesByLayer[i][j].Origin;
                        Point3d closestPoint = plane.ClosestPoint(point);

                        temp.Add(point.DistanceTo(closestPoint));
                        tempx.Add(Math.Abs(point.X - closestPoint.X));
                        tempy.Add(Math.Abs(point.Y - closestPoint.Y));
                        tempz.Add(Math.Abs(point.Z - closestPoint.Z));
                    }
                }
                else
                {
                    for (int j = 0; j < _framesByLayer[i].Count; j++)
                    {
                        Point3d point = _framesByLayer[i][j].Origin;
                        _contours[i - 1].ClosestPoint(point, out double parameter);
                        Point3d closestPoint = _contours[i - 1].PointAt(parameter); //TODO: This goes wrong at transitions!

                        temp.Add(point.DistanceTo(closestPoint));
                        tempx.Add(Math.Abs(point.X - closestPoint.X));
                        tempy.Add(Math.Abs(point.Y - closestPoint.Y));
                        tempz.Add(Math.Abs(point.Z - closestPoint.Z));
                    }
                }

                distances.Add(temp);
                dx.Add(tempx);
                dy.Add(tempy);
                dz.Add(tempz);
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
        /// Returns all the points of the path sorted by layer.
        /// </summary>
        /// <returns> The lists with points. </returns>
        public List<List<Point3d>> GetPointsByLayer()
        {
            List<List<Point3d>> points = new List<List<Point3d>>();

            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                points.Add(new List<Point3d>());

                for (int j = 0; j < _framesByLayer[i].Count; j++)
                {
                    points[i].Add(_framesByLayer[i][j].Origin);
                }
            }

            return points;
        }

        /// <summary>
        /// Returns the Bounding Box of the object.
        /// </summary>
        /// <returns> The Bounding Box. </returns>
        /// <param name="accurate"> If true, a physically accurate bounding box will be computed. If not, a bounding box estimate will be computed. </param>

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
            _baseContour.Transform(xform);

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
                if (_baseContour == null) { return false; }
                if (_path == null) { return false; }
                if (_contours == null) { return false; }
                if (_heights == null) { return false; }
                if (_framesByLayer == null) { return false; }
                if (_distance <= 0.0) { return false; }
                if (_contours.Count == 0) { return false; }
                if (_heights.Count == 0) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the base contour of the first layer. 
        /// </summary>
        public Curve BaseContour
        {
            get { return _baseContour; }
            set { _baseContour = value; }
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
        /// Gets or sets the abosolute heights (location) of the layers. 
        /// </summary>
        public List<double> Heights
        {
            get { return _heights; }
            set { _heights = value; }
        }

        /// <summary>
        /// Gets the contours of each layers as a list with curves.
        /// </summary>
        public List<Curve> Contours
        {
            get { return _contours; }
        }

        /// <summary>
        /// Gets the path as a list with curves. 
        /// </summary>
        public List<Curve> Path
        {
            get { return _path; }
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
        /// Gets the frames of the path sorted by layer. 
        /// </summary>
        public List<List<Plane>> FramesByLayer
        {
            get { return _framesByLayer; }
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
