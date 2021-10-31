// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
using System.Linq;
// Rhino Libs
using Rhino.Geometry;
// Slicer Salad Libs
using SaladSlicer.Core.CodeGeneration;
using SaladSlicer.Core.Geometry;
using SaladSlicer.Core.Geometry.Seams;
using SaladSlicer.Core.Interfaces;

namespace SaladSlicer.Core.Slicers
{
    /// <summary>
    /// Represents the Planar 3D Slicer class.
    /// </summary>
    public class ClosedPlanar3DSlicer : IProgram, ISlicer, IGeometry, IAddVariable
    {
        #region fields
        private Mesh _mesh ;
        private double _distance;
        private List<Curve> _path = new List<Curve>();
        private List<Curve> _contours = new List<Curve>();
        private List<double> _heights = new List<double>();
        private readonly List<List<Plane>> _framesByLayer = new List<List<Plane>>() { };
        private double _seamLocation;
        private double _seamLength;
        private bool _reverse;
        private List<List<List<double>>> _addedVariable = new List<List<List<double>>>(0);
        private List<string> _prefix = new List<string>();
        #endregion

        #region (de)serialisation
        //TODO
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Planar 3D Slicer class.
        /// </summary>
        public ClosedPlanar3DSlicer()
        {

        }

        /// <summary>
        /// Initializes a new instance of the Planar 3D Slicer class from a list with absolute layer heights. 
        /// </summary>
        /// <param name="mesh"> The base mesh. </param>
        /// <param name="parameter"> The parameter of the starting point. </param>
        /// <param name="length"> The length of the seam between two layers. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        /// <param name="reverse"> Indicates if the path direction will be reversed. </param>
        /// <param name="heights"> A list with absolute layer heights. </param>
        public ClosedPlanar3DSlicer(Mesh mesh, double parameter, double length, double distance, bool reverse, List<double> heights)
        {
            _mesh = mesh;
            _heights = heights;
            _seamLocation = parameter;
            _seamLength = length;
            _distance = distance;
            _reverse = reverse;
        }

        /// <summary>
        /// Initializes a new instance of the Planar 3D Slicer class from a given number of layers and layer thickness.
        /// </summary>
        /// <param name="mesh"> The base mesh. </param>
        /// <param name="parameter"> The parameter of the starting point. </param>
        /// <param name="length"> The length of the seam between two layers. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        /// <param name="height"> The layer height. </param>
        /// <param name="reverse"> Indicates if the path direction will be reversed. </param>
        /// <param name="layers"> The number of layers. </param>
        public ClosedPlanar3DSlicer(Mesh mesh, double parameter, double length, double distance, double height, bool reverse, int layers)
        {
            _mesh = mesh;
            _heights.AddRange(Enumerable.Repeat(height, layers).ToList());
            _seamLocation = parameter;
            _seamLength = length;
            _distance = distance;
            _reverse = reverse;
        }

        /// <summary>
        /// Initializes a new instance of the Planar 3D Slicer class by duplicating an existing Planar 3D Slicer instance. 
        /// </summary>
        /// <param name="slicer"> The Planar 2D Slicer instance to duplicate. </param>
        public ClosedPlanar3DSlicer(ClosedPlanar3DSlicer slicer)
        {
            _mesh = slicer.Mesh.DuplicateMesh();
            _heights = new List<double>(slicer.Heights);
            _seamLocation = slicer.SeamLocation;
            _seamLength = slicer.SeamLength;
            _distance = slicer.Distance;
            _path = slicer.Path.ConvertAll(curve => curve.DuplicateCurve());
            _contours = slicer.Contours.ConvertAll(curve => curve.DuplicateCurve());
            _reverse = slicer.Reverse;

            _framesByLayer = new List<List<Plane>>();

            for (int i = 0; i < slicer.FramesByLayer.Count; i++)
            {
                _framesByLayer.Add(new List<Plane>(slicer.FramesByLayer[i]));
            }
            _prefix = slicer.Prefix;
            _addedVariable = slicer.AddedVariable;
        }

        /// <summary>
        /// Returns an exact duplicate of this Planar 3D Slicer instance.
        /// </summary>
        /// <returns> The exact duplicate of this Planar 3D Slicer instance. </returns>
        public ClosedPlanar3DSlicer Duplicate()
        {
            return new ClosedPlanar3DSlicer(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Planar 3D Slicer instance as an IProgram.
        /// </summary>
        /// <returns> The exact duplicate of this Planar 3D Slicer instance as an IProgram. </returns>
        public IProgram DuplicateProgramObject()
        {
            return this.Duplicate() as IProgram;
        }

        /// <summary>
        /// Returns an exact duplicate of this Planar 3D Slicer instance as an ISlicer.
        /// </summary>
        /// <returns> The exact duplicate of this Planar 3D Slicer instance as an ISlicer. </returns>
        public ISlicer DuplicateSlicerObject()
        {
            return this.Duplicate() as ISlicer;
        }

        /// <summary>
        /// Returns an exact duplicate of this Planar 3D Slicer instance as an IGeometry.
        /// </summary>
        /// <returns> The exact duplicate of this Planar 3D Slicer instance as an IGeometry. </returns>
        public IGeometry DuplicateGeometryObject()
        {
            return this.Duplicate() as IGeometry;
        }

        /// <summary>
        /// Returns an exact duplicate of this Planar 3D Slicer instance as an IAddVariable
        /// </summary>
        /// <returns> The exact duplicate of this Planar 3D Slicer instance as an IAddVariable. </returns>
        public IAddVariable DuplicateAddVariableObject()
        {
            return this.Duplicate() as IAddVariable;
        }
        #endregion

        #region methods
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns> A string that represents the current object. </returns>
        public override string ToString()
        {
            return "Closed 3D Planar Object";
        }

        /// <summary>
        /// Slices this object.
        /// </summary>
        public void Slice()
        {
            this.CreateContours();
            this.CreatePath();
            this.CreateFrames();
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

            // Set seam location
            _contours = Curves.AlignCurves(_contours);
            _contours[0] = Locations.SeamAtLength(_contours[0], _seamLocation, true);
            //_contours[0] = Geometry.Seams.Locations.SeamAtLength(_contours[0], _contours[0].GetLength() - 0.5 * _seamLength, false); //TODO: to discuss... 
            _contours = Locations.AlignSeamsByClosestPoint(_contours);

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
            _addedVariable.Add(new List<List<double>>());
            for (int i = 0; i < _contours.Count; i++)
            {
                _framesByLayer.Add(new List<Plane>() { });
                _addedVariable[0].Add(new List<double>());
            }

            for (int i = 0; i < _contours.Count; i++)
            {
                // Contours
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

                // Transitions
                if (i < _contours.Count - 1)
                {
                    n = (int)(_path[i * 2 + 1].GetLength() / _distance);
                    n = Math.Max(2, n);
                    t = _path[i * 2 + 1].DivideByCount(n, false);

                    for (int j = 0; j != t.Length; j++)
                    {
                        Point3d point = _path[i * 2 + 1].PointAt(t[j]);

                        MeshPoint meshPoint = _mesh.ClosestMeshPoint(point, 100.0);
                        Vector3d meshNormal = _mesh.NormalAt(meshPoint);

                        Vector3d x = _path[i * 2 + 1].TangentAt(t[j]);
                        Vector3d y = Vector3d.CrossProduct(x, new Vector3d(0, 0, 1));

                        double angle1 = Vector3d.VectorAngle(y, meshNormal);
                        double angle2 = Vector3d.VectorAngle(y, -meshNormal);

                        if (angle1 < angle2) { y = meshNormal; }
                        else { y = -meshNormal; }

                        Plane plane = new Plane(point, x, y);
                        _framesByLayer[i].Add(plane);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        public void ToProgram(ProgramGenerator programGenerator,int programType)
        {
            // Header
            programGenerator.AddSlicerHeader("3D CLOSED PLANAR OBJECT", _contours.Count, this.GetLength());

            // Coords
            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                programGenerator.Program.Add(" ");
                programGenerator.Program.Add($"; LAYER {i + 1:0}");
                List<List<double>> addedVariable2 = new List<List<double>>();
                for (int k = 0; k < _addedVariable.Count; k++)
                {
                    addedVariable2.Add(_addedVariable[k][i]);
                }
                programGenerator.AddCoordinates(_framesByLayer[i], _prefix, addedVariable2);
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
            _prefix.Add(prefix);
            if (_addedVariable[0][0].Count < 1)
            {
                _addedVariable[0] = values;
            }
            else
            {
                _addedVariable.Add(values);
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
            return Curve.JoinCurves(_path)[0];
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
            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                double distance = 0;
                List<double> distancesTemp = new List<double>();
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
        /// Calculates the distance between every frame and the closest point on the previous layer.
        /// </summary>
        /// <param name="plane">Plane to calculate closest point to for the first layer</param>
        /// <returns></returns>
        public List<List<double>> GetDistanceToPreviousLayer(Plane plane)
        {
            List<List<double>> distances = new List<List<double>>();
            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                List<double> distancesTemp = new List<double>();
                for (int j = 0; j < _framesByLayer[i].Count; j++)
                {
                    Point3d point = _framesByLayer[i][j].Origin;
                    if (i == 0)
                    {
                        Point3d point2 = plane.ClosestPoint(point);
                        distancesTemp.Add(point.DistanceTo(point2));
                    }
                    else
                    {
                        _contours[i - 1].ClosestPoint(point, out double parameter);
                        distancesTemp.Add(point.DistanceTo(_contours[i - 1].PointAt(parameter)));
                    }
                }
                distances.Add(distancesTemp);
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
            List<Plane> frames = this.Frames;

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
            return this.GetPath().GetBoundingBox(accurate);
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
                if (_seamLength <= 0.0) { return false; }
                if (_seamLocation < 0.0) { return false; }
                if (_seamLocation > 1.0) { return false; }
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
        /// Gets or sets the location of the seam based on the normalized length of the first contour. 
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
            get { return _framesByLayer[_framesByLayer.Count - 1][_framesByLayer[_framesByLayer.Count - 1].Count - 1]; }
        }

        /// <summary>
        /// Gets point at the start of the path.
        /// </summary>
        public Point3d PointAtStart
        {
            get { return this.FrameAtStart.Origin; }
        }

        /// <summary>
        /// Gets point at the end of the path.
        /// </summary>
        public Point3d PointAtEnd
        {
            get { return this.FrameAtEnd.Origin; }
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
            get { return _addedVariable; }
        }
        #endregion
    }
}
