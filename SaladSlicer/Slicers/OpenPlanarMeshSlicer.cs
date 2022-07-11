// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/MeshCP-TUe/SaladSlicer>.

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
    /// Represents the Open Planar Mesh Slicer class.
    /// </summary>
    [Serializable()]
    public class OpenPlanarMeshSlicer : IProgram, ISlicer, IGeometry, IAddVariable
    {
        #region fields
        private Mesh _mesh ;
        private double _distance;
        private List<Curve> _path = new List<Curve>();
        private List<Curve> _contours = new List<Curve>();
        private List<double> _heights = new List<double>();
        private readonly List<List<Plane>> _framesByLayer = new List<List<Plane>>() { };
        private bool _reverse;
        private readonly Dictionary<string, List<List<double>>> _addedVariables = new Dictionary<string, List<List<double>>>() { };
        #endregion

        #region (de)serialisation
        //TODO
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Open Planar Mesh Slicer class.
        /// </summary>
        public OpenPlanarMeshSlicer()
        {

        }

        /// <summary>
        /// Initializes a new instance of the Open Planar Mesh Slicer class from a list with absolute layer heights. 
        /// </summary>
        /// <param name="mesh"> The base mesh. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        /// <param name="reverse"> Indicates if the path direction will be reversed. </param>
        /// <param name="heights"> A list with absolute layer heights. </param>
        public OpenPlanarMeshSlicer(Mesh mesh, double distance, bool reverse, IList<double> heights)
        {
            _mesh = mesh;
            _heights = heights as List<double>;
            _distance = distance;
            _reverse = reverse;
        }

        /// <summary>
        /// Initializes a new instance of the Open Planar Mesh Slicer class from a given number of layers and layer thickness.
        /// </summary>
        /// <param name="mesh"> The base mesh. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        /// <param name="height"> The layer height. </param>
        /// <param name="reverse"> Indicates if the path direction will be reversed. </param>
        /// <param name="layers"> The number of layers. </param>
        public OpenPlanarMeshSlicer(Mesh mesh, double distance, double height, bool reverse, int layers)
        {
            _mesh = mesh;
            _heights.AddRange(Enumerable.Repeat(height, layers).ToList());
            _distance = distance;
            _reverse = reverse;
        }

        /// <summary>
        /// Initializes a new instance of the Open Planar Mesh Slicer class by duplicating an existing Open Planar Mesh Slicer instance. 
        /// </summary>
        /// <param name="slicer"> The Open Planar Mesh Slicer instance to duplicate. </param>
        public OpenPlanarMeshSlicer(OpenPlanarMeshSlicer slicer)
        {
            _mesh = slicer.Mesh.DuplicateMesh();
            _heights = new List<double>(slicer.Heights);
            _distance = slicer.Distance;
            _path = slicer.Path.ConvertAll(curve => curve.DuplicateCurve());
            _contours = slicer.Contours.ConvertAll(curve => curve.DuplicateCurve());
            _reverse = slicer.Reverse;
            _addedVariables = slicer.AddedVariables.ToDictionary(entry => entry.Key.Clone() as string, entry => entry.Value.ConvertAll(list => list.ConvertAll(item => item)));
            _framesByLayer = slicer.FramesByLayer.ConvertAll(list => new List<Plane>(list));
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar Mesh Slicer instance.
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar Mesh Slicer instance. </returns>
        public OpenPlanarMeshSlicer Duplicate()
        {
            return new OpenPlanarMeshSlicer(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar Mesh Slicer instance as an IProgram.
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar Mesh Slicer instance as an IProgram. </returns>
        public IProgram DuplicateProgramObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar Mesh Slicer instance as an ISlicer.
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar Mesh Slicer instance as an ISlicer. </returns>
        public ISlicer DuplicateSlicerObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar Mesh Slicer instance as an IGeometry.
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar Mesh Slicer instance as an IGeometry. </returns>
        public IGeometry DuplicateGeometryObject()
        {
            return Duplicate();
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar Mesh Slicer instance as an IAddVariable
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar Mesh Slicer instance as an IAddVariable. </returns>
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
            return "Open Planar Mesh Sicer Object";
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

            Plane plane = Plane.WorldXY;

            // Get the bounding box and start position
            BoundingBox box = _mesh.GetBoundingBox(true);
            Point3d[] corners = box.GetCorners();
            double min = corners[0].Z;

            for (int i = 1; i < corners.Length; i++)
            {
                if (corners[i].Z < min)
                {
                    min = corners[i].Z;
                }
            }

            plane.OriginZ = min;

            // One height value defined
            if (_heights.Count == 1)
            {
                bool stop = false;
                int i = 0;
                
                while (stop == false)
                {
                    plane.OriginZ = min + i * _heights[0];
                    Curve[] curves = Mesh.CreateContourCurves(_mesh, plane);
                    curves = Curve.JoinCurves(curves, 1.0);

                    if (curves.Length != 0)
                    {
                        _contours.Add(curves[0].ToNurbsCurve());
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            // Multiple values defined
            else
            {
                for (int i = 0; i < _heights.Count; i++)
                {
                    plane.OriginZ = min + _heights[i];
                    Curve[] curves = Mesh.CreateContourCurves(_mesh, plane);
                    curves = Curve.JoinCurves(curves, 1.0);

                    if (curves.Length != 0)
                    {
                        _contours.Add(curves[0].ToNurbsCurve());
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Align curves
            _contours = Curves.AlignCurves(_contours);
            _contours = Curves.AlternateCurves(_contours);

            // Reverse the contours
            if (_reverse == true)
            {
                for (int i = 0; i < _contours.Count; i++)
                {
                    _contours[i].Reverse();
                }
            }
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
                _framesByLayer.Add(new List<Plane>() { });
            }

            for (int i = 0; i < _contours.Count; i++)
            {
                int n = (int)(_path[i * 2].GetLength() / _distance);
                n = Math.Max(2, n);
                double[] t = _path[i * 2].DivideByCount(n, true);

                for (int j = 0; j != t.Length; j++)
                {
                    Point3d point = _path[i * 2].PointAt(t[j]);

                    MeshPoint meshPoint = _mesh.ClosestMeshPoint(point, 100.0);
                    Vector3d meshNormal = _mesh.NormalAt(meshPoint);

                    Vector3d x = _path[i * 2].TangentAt(t[j]);
                    Vector3d y = Vector3d.CrossProduct(x, new Vector3d(0, 0, 1));

                    double angle1 = Vector3d.VectorAngle(y, meshNormal);
                    double angle2 = Vector3d.VectorAngle(y, -meshNormal);

                    if (angle1 < angle2) { y = meshNormal; }
                    else { y = -meshNormal; }

                    Plane plane = new Plane(point, x, y);
                    _framesByLayer[i].Add(plane);
                }
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
            programGenerator.AddSlicerHeader("Mesh OPEN PLANAR OBJECT", _contours.Count, GetLength());

            // Get coordinates
            List<List<string>> coordinates = ProgramGenerator.GetCoordinateCodeLines(this,programGenerator.PrinterSettings);

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
                        programGenerator.Program.Add("TANGOF(C)");
                    }

                    // Move 1 point with the TANGOF
                    programGenerator.Program.Add(coordinates[i][0]);

                    // Turn TANGON again
                    if (i % 2 == 0)
                    {
                        programGenerator.Program.Add("TANGON(C, 0)");
                    }
                    else
                    {
                        programGenerator.Program.Add("TANGON(C, 180)");
                    }

                    programGenerator.Program.Add("BSPLINE");
                    programGenerator.Program.Add("G642");

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
        /// Returns the Open Planar Mesh Slicer object as a string
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
        /// Returns distance of every frame along the curve.
        /// </summary>
        /// <returns> List with distances. </returns>
        public List<List<double>> GetDistancesAlongContours()
        {
            List<List<double>> distances = new List<List<double>>();

            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                List<double> distancesTemp = new List<double>();
                double distance = 0;

                for (int j = 0; j < _framesByLayer[i].Count; j++)
                {
                    if (j == 0)
                    {
                        distancesTemp.Add(distance);
                    }
                    else
                    {
                        Point3d point = _framesByLayer[i][j].Origin;
                        distance += point.DistanceTo(_framesByLayer[i][j - 1].Origin);
                        distancesTemp.Add(distance);
                    }
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
            _mesh.Transform(xform);

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
                if (_mesh == null) { return false; }
                if (_path == null) { return false; }
                if (_contours == null) { return false; }
                if (_heights == null) { return false; }
                if (_framesByLayer == null) { return false; }
                if (_distance <= 0.0) { return false; }
                if (_contours.Count == 0) { return false; }
                if (_heights.Count == 0) { return false; }
                if (_framesByLayer.Count == 0) { return false; }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the mesh.
        /// </summary>
        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
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
        /// Gets or set a value indicating whether or not the path direction is reversed.
        /// </summary>
        public bool Reverse
        {
            get { return _reverse; }
            set { _reverse = value; }
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
