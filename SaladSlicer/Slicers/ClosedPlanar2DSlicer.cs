// This file is part of SaladSlicer. SaladSlicer is licensed 
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
using SaladSlicer.Geometry;
using SaladSlicer.Geometry.Seams;
using SaladSlicer.Interfaces;
using SaladSlicer.Utils;
using SaladSlicer.Enumerations;

namespace SaladSlicer.Slicers
{
    /// <summary>
    /// Represents the Closed Planar 2D Slicer class.
    /// </summary>
    [Serializable()]
    public class ClosedPlanar2DSlicer : IProgram, ISlicer, IGeometry, IAddVariable
    {
        #region fields
        private Curve _baseContour;
        private double _distance;
        private List<Curve> _path = new List<Curve>();
        private readonly List<Curve> _contours = new List<Curve>();
        private List<double> _heights = new List<double>();
        private readonly List<List<Plane>> _framesByLayer = new List<List<Plane>>() { };
        private readonly List<List<Plane>> _framesInContours = new List<List<Plane>>() { };
        private readonly List<List<Plane>> _framesInTransitions = new List<List<Plane>>() { };
        private double _seamLocation;
        private double _seamLength;
        private readonly Dictionary<string, List<List<double>>> _addedVariables = new Dictionary<string, List<List<double>>>() { };
        #endregion

        #region (de)serialisation
        //TODO
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Planar 2D Slicer class.
        /// </summary>
        public ClosedPlanar2DSlicer()
        {

        }

        /// <summary>
        /// Initializes a new instance of the Closed Planar 2D Slicer class from a list with absolute layer heights. 
        /// </summary>
        /// <param name="curve"> The base contour. </param>
        /// <param name="parameter"> The parameter of the starting point. </param>
        /// <param name="length"> The length of the seam between two layers. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        /// <param name="heights"> A list with absolute layer heights. </param>
        public ClosedPlanar2DSlicer(Curve curve, double parameter, double length, double distance, IList<double> heights)
        {
            _baseContour = curve;
            _heights = heights as List<double>;
            _seamLocation = parameter;
            _seamLength = length;
            _distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the Closed Planar 2D Slicer class from a given number of layers and layer thickness.
        /// </summary>
        /// <param name="curve"> The base contour. </param>
        /// <param name="parameter"> The parameter of the starting point. </param>
        /// <param name="length"> The length of the seam between two layers. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        /// <param name="height"> The layer height. </param>
        /// <param name="layers"> The number of layers. </param>
        public ClosedPlanar2DSlicer(Curve curve, double parameter, double length, double distance, double height, int layers)
        {
            _baseContour = curve;
            _heights.AddRange(Enumerable.Repeat(height, layers).ToList());
            _seamLocation = parameter;
            _seamLength = length;
            _distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the Closed Planar 2D Slicer class by duplicating an existing Closed Planar 2D Slicer instance. 
        /// </summary>
        /// <param name="slicer"> The Closed Planar 2D Slicer instance to duplicate. </param>
        public ClosedPlanar2DSlicer(ClosedPlanar2DSlicer slicer)
        {
            _baseContour = slicer.BaseContour.DuplicateCurve();
            _heights = new List<double>(slicer.Heights);
            _seamLocation = slicer.SeamLocation;
            _seamLength = slicer.SeamLength;
            _distance = slicer.Distance;
            _path = slicer.Path.ConvertAll(curve => curve.DuplicateCurve());
            _contours = slicer.Contours.ConvertAll(curve => curve.DuplicateCurve());
            _addedVariables = slicer.AddedVariables.ToDictionary(entry => entry.Key.Clone() as string, entry => entry.Value.ConvertAll(list => list.ConvertAll(item => item)));
            _framesByLayer = slicer.FramesByLayer.ConvertAll(list => new List<Plane>(list));
            _framesInTransitions = slicer._framesInTransitions.ConvertAll(list => new List<Plane>(list));
            _framesInContours = slicer._framesInContours.ConvertAll(list => new List<Plane>(list));
        }

        /// <summary>
        /// Returns an exact duplicate of this Closed Planar 2D Slicer instance.
        /// </summary>
        /// <returns> The exact duplicate of this Closed Planar 2D Slicer instance. </returns>
        public ClosedPlanar2DSlicer Duplicate()
        {
            return new ClosedPlanar2DSlicer(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Closed Planar 2D Slicer instance as an IProgram.
        /// </summary>
        /// <returns> The exact duplicate of this Closed Planar 2D Slicer instance as an IProgram. </returns>
        public IProgram DuplicateProgramObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Closed Planar 2D Slicer instance as an ISlicer.
        /// </summary>
        /// <returns> The exact duplicate of this Closed Planar 2D Slicer instance as an ISlicer. </returns>
        public ISlicer DuplicateSlicerObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Planar 2D Slicer instance as an IGeometry.
        /// </summary>
        /// <returns> The exact duplicate of this Planar 2D Slicer instance as an IGeometry. </returns>
        public IGeometry DuplicateGeometryObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Closed Planar 2D Slicer instance as an IAddVariable
        /// </summary>
        /// <returns> The exact duplicate of this Closed Planar 2D Slicer instance as an IAddVariable. </returns>
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
            return "Closed 2.5D Planar Object";
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
            Curve contour = Geometry.Seams.Locations.SeamAtLength(_baseContour, _seamLocation, true);
            //contour = Geometry.Seams.Locations.SeamAtLength(contour, contour.GetLength() - 0.5 * _seamLength, false); //TODO: to discuss... 
            contour.Domain = new Interval(0, contour.GetLength());

            for (int i = 0; i < _heights.Count; i++)
            {
                _contours.Add(contour.DuplicateCurve());
                _contours[i].Translate(0, 0, _heights[i]);
            }
        }

        /// <summary>
        /// Creates the path of this object.
        /// </summary>
        private void CreatePath()
        {
            List<Curve> trimmed = Transitions.TrimCurveFromEnds(_contours, _seamLength);
            List<Curve> transitions = Transitions.InterpolatedTransitions(_contours, _seamLength, 0.25 * _distance);
            
            _path.Clear();
            _path = Curves.WeaveCurves(trimmed, transitions);
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
                    _framesByLayer[i].AddRange(Geometry.Frames.GetFramesByDistanceAndSegment(_path[i * 2 + 1], _distance, false, false));
                    _framesInTransitions[i].AddRange(Geometry.Frames.GetFramesByDistanceAndSegment(_path[i * 2 + 1], _distance, false, false));
                }
            }
        }
        
        /// <summary>
        /// Check if the layer height is constant throughout the object.
        /// </summary>
        private bool ConstantHeightIncrease()
        {
            bool constantHeight = true;
            double heightIncrease0 = _heights[1] - _heights[0];
            double heightIncrease1;

            for (int i = 2; i < _heights.Count; i++)
            {
                heightIncrease1 = _heights[i] - _heights[i - 1];

                if (heightIncrease0 != heightIncrease1)
                {
                    constantHeight = false;
                    break;
                }
            }

            return constantHeight;
        }

        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToProgram(ProgramGenerator programGenerator)
        {
            // Header
            programGenerator.AddSlicerHeader("2.5D CLOSED PLANAR OBJECT", _contours.Count, GetLength());

            // Create a loop for objects with an constant height increase per layer
            
            if (ConstantHeightIncrease() && programGenerator.PrinterSettings.ProgramType == ProgramType.Sinumerik && _addedVariables.Count == 0)
            {
                double layerHeight = _heights[1] - _heights[0];

                // Initialize loop
                programGenerator.Program.Add($"R10 = {layerHeight:0.###} ; Height of a single layer");
                programGenerator.Program.Add(" ");
                programGenerator.Program.Add("R20 = 0 ; Number of the current layer");
                programGenerator.Program.Add($"R21 = {_heights.Count,2:G} ; Total number of layers");
                programGenerator.Program.Add(" ");
                programGenerator.Program.Add("; Start loop");
                programGenerator.Program.Add("LINE1:");
                
                for (int i = 0; i < _framesByLayer[0].Count; i++)
                {
                    Point3d point = _framesByLayer[0][i].Origin;
                    programGenerator.Program.Add($"X{point.X:0.###} Y{point.Y:0.###} Z={point.Z:0.###}+R10*R20");
                }

                programGenerator.Program.Add(" ");
                programGenerator.Program.Add("R20 = R20 + 1 ; Increase layer number");
                programGenerator.Program.Add("IF R20 < R21 GOTOB LINE1");
            }

            // Create standard code for objects with irregular height increase and/or added variables
            else 
            {
                // Add coordinates
                List<List<string>> coordinates = ProgramGenerator.GetCoordinateCodeLines(this, programGenerator.PrinterSettings);

                for (int i = 0; i < coordinates.Count; i++)
                {
                    programGenerator.Program.Add(" ");
                    programGenerator.Program.Add($"; LAYER {i + 1:0}");
                    programGenerator.Program.AddRange(coordinates[i]);
                }
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

            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                List<double> distancesTemp = new List<double>() { 0.0 };
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
                        temp.Add(firstDistance + (j + 1) * (secondDistance - firstDistance) / (_framesInTransitions[i].Count + 1));
                        tempx.Add(firstDistanceX + (j + 1) * (secondDistanceX - firstDistanceX) / (_framesInTransitions[i].Count + 1));
                        tempy.Add(firstDistanceY + (j + 1) * (secondDistanceY - firstDistanceY) / (_framesInTransitions[i].Count + 1));
                        tempz.Add(firstDistanceZ + (j + 1) * (secondDistanceZ - firstDistanceZ) / (_framesInTransitions[i].Count + 1));
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
                if (_seamLength <= 0.0) { return false; }
                if (_seamLocation < 0.0) { return false; }
                if (_seamLocation > 1.0) { return false; }
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
        /// Gets or sets the location of the seam based on the length of the contour. 
        /// </summary>
        public double SeamLocation
        {
            get { return _seamLocation; }
            set { _seamLocation = value; }
        }

        /// <summary>
        /// Gets or sets the length of the seam between two layers.
        /// </summary>
        public double SeamLength
        {
            get { return _seamLength; }
            set { _seamLength = value; }
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
            get { return _framesByLayer[_framesByLayer.Count-1][_framesByLayer[_framesByLayer.Count - 1].Count - 1]; }
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
        public Dictionary<string, List<List<double>>> AddedVariables 
        {
            get { return _addedVariables; }
        }
        #endregion
    }
}
