// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
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
    /// Represents the Planar 2D Slicer class.
    /// </summary>
    public class OpenPlanar2DSlicer : IProgram, ISlicer, IGeometry, IAddVariable
    {
        #region fields
        private Curve _baseContour;
        private double _distance;
        private List<Curve> _path = new List<Curve>();
        private List<Curve> _contours = new List<Curve>();
        private List<double> _heights = new List<double>();
        private readonly List<List<Plane>> _framesByLayer = new List<List<Plane>>() { };
        private List<List<double>> _addedVariable=new List<List<double>>(0);
        private string _prefix="";
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
        public OpenPlanar2DSlicer(Curve curve, double distance, List<double> heights)
        {
            _baseContour = curve;
            _heights = heights;
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
        /// Initializes a new instance of the Open Planar 2D Slicer class by duplicating an existing Planar 2D Slicer instance. 
        /// </summary>
        /// <param name="slicer"> The Open Planar 2D Slicer instance to duplicate. </param>
        public OpenPlanar2DSlicer(OpenPlanar2DSlicer slicer)
        {
            _baseContour = slicer.BaseContour.DuplicateCurve();
            _heights = new List<double>(slicer.Heights);
            _distance = slicer.Distance;
            _path = slicer.Path.ConvertAll(curve => curve.DuplicateCurve());
            _contours = slicer.Contours.ConvertAll(curve => curve.DuplicateCurve());

            _framesByLayer = new List<List<Plane>>();

            for (int i = 0; i < slicer.FramesByLayer.Count; i++)
            {
                _framesByLayer.Add(new List<Plane>(slicer.FramesByLayer[i]));
            }
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
            return this.Duplicate() as IProgram;
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar 2D Slicer instance as an ISlicer.
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar 2D Slicer instance as an ISlicer. </returns>
        public ISlicer DuplicateSlicerObject()
        {
            return this.Duplicate() as ISlicer;
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar 2D Slicer instance as an IGeometry.
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar 2D Slicer instance as an IGeometry. </returns>
        public IGeometry DuplicateGeometryObject()
        {
            return this.Duplicate() as IGeometry;
        }

        /// <summary>
        /// Returns an exact duplicate of this Open Planar 2D Slicer instance as an IAddVariable
        /// </summary>
        /// <returns> The exact duplicate of this Open Planar 2D Slicer instance as an IAddVariable. </returns>
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
            return "Open 2.5 Planar Object";
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
                _addedVariable.Add(new List<double>());
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
        public void ToProgram(ProgramGenerator programGenerator,int programType)
        {
            // Header
            programGenerator.AddSlicerHeader("2.5D OPEN PLANAR OBJECT", _contours.Count, this.GetLength());

            // Settings
            if (programType == 0)
            {
                programGenerator.Program.Add("TANG(C, X, Y, 1)");
                programGenerator.Program.Add("TANGON(C, 0)");
                programGenerator.Program.Add("BSPLINE");
                programGenerator.Program.Add("G642");
                programGenerator.Program.Add("G90");
                programGenerator.Program.Add(" ");
            }
            else
            {
                programGenerator.Program.Add("; No settings defined for this type of program");
            }
            // TODO
            // FEEDRATE?
            // WORK OFFSET?

            // Coords
            if (programType == 0)
            {
                for (int i = 0; i < _framesByLayer.Count; i++)
                {
                    programGenerator.Program.Add(" ");
                    programGenerator.Program.Add($"; LAYER {i + 1:0}");

                    if (i != 0)
                    {
                        programGenerator.Program.Add("TANGOF(C)");
                    }

                    for (int j = 0; j < _framesByLayer[i].Count; j++)
                    {
                        if (j == 0)
                        {
                            Point3d point = _framesByLayer[i][j].Origin;
                            programGenerator.Program.Add($"G1 X{point.X:0.###} Y{point.Y:0.###} Z{point.Z:0.###}");

                            if (i % 2 == 0 & j == 0)
                            {
                                programGenerator.Program.Add("TANGON(C, 0)");
                            }
                            else if (j == 0)
                            {
                                programGenerator.Program.Add("TANGON(C, 180)");
                            }

                            programGenerator.Program.Add("BSPLINE");
                            programGenerator.Program.Add("G642");
                        }
                        else
                        {
                            Point3d point = _framesByLayer[i][j].Origin;
                            programGenerator.Program.Add($"X{point.X:0.###} Y{point.Y:0.###} Z{point.Z:0.###}");
                        }
                    }
                }
            }
            else if(programType==1)
            {
                for (int i = 0; i < _framesByLayer.Count; i++)
                {
                    programGenerator.Program.Add(" ");
                    programGenerator.Program.Add($"; LAYER {i + 1:0}");
                    programGenerator.AddCoordinates(_framesByLayer[i], programType,_prefix,_addedVariable[i]);
                }
            }

            // End
            programGenerator.AddFooter();
        }

        /// <summary>
        /// Adds an additional variable to the program, besides X, Y and Z.
        /// </summary>
        /// <param name="prefix">Prefix to use for the variable.</param>
        /// <param name="factor">Factor difference between method variable and added variable.</param>
        public void AddVariableByDisplacement(string prefix, double factor)
        {
            _prefix = prefix;
            double distance = 0;
                
            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                List<double> AddedVariable = new List<double>();
                for (int j = 0; j < _framesByLayer[i].Count; j++)
                {
                    if (j == 0)
                    {
                        AddedVariable.Add(distance * factor);
                    }
                    else
                    {
                        Point3d point = _framesByLayer[i][j].Origin;
                        distance += point.DistanceTo(_framesByLayer[i][j - 1].Origin);
                        AddedVariable.Add(distance * factor);
                    }
                }
                _addedVariable.Add(AddedVariable);
                AddedVariable.Clear();
            }
        }

        /// <summary>
        /// Adds a variable by multiplying a factor with the interlayer distance. Only one distance is used for the outerlayers, while the average of two is used for the intermediates. 
        /// </summary>
        /// <param name="prefix">Prefix to use for the variable.</param>
        /// <param name="factor">Factor between the variable and the interlayer distance.</param>
        public void AddVariableByLayerDistance(string prefix, double factor)
        {
            _prefix = prefix;
            for (int i = 0; i < _framesByLayer.Count; i++)
            {
                List<double> AddedVariable = new List<double>();
                for (int j = 0; j < _framesByLayer[i].Count; j++)
                {
                    if (i == 0)
                    {
                        Point3d point = _framesByLayer[i][j].Origin;
                        _contours[i + 1].ClosestPoint(point, out double parameter);
                        double distance = point.DistanceTo(_contours[i + 1].PointAt(parameter));
                        AddedVariable.Add(distance * factor);
                    }
                    else if (i == _framesByLayer.Count - 1)
                    {
                        Point3d point = _framesByLayer[i][j].Origin;
                        _contours[i - 1].ClosestPoint(point, out double parameter);
                        double distance = point.DistanceTo(_contours[i - 1].PointAt(parameter));
                        AddedVariable.Add(distance * factor);
                    }
                    else
                    {
                        Point3d point = _framesByLayer[i][j].Origin;
                        _contours[i - 1].ClosestPoint(point, out double parameter1);
                        _contours[i + 1].ClosestPoint(point, out double parameter2);
                        double distance1 = point.DistanceTo(_contours[i - 1].PointAt(parameter1));
                        double distance2 = point.DistanceTo(_contours[i + 1].PointAt(parameter2));
                        AddedVariable.Add((distance1 + distance2) / 2 * factor);
                    }
                }
                _addedVariable.Add(AddedVariable);
            }
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
            List<List<Point3d>> points = this.GetPointsByLayer();

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
            return new PolylineCurve(this.GetPoints());
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
            get { return this.FrameAtStart.Origin; }
        }

        /// <summary>
        /// Gets point at the end of the path.
        /// </summary>
        public Point3d PointAtEnd
        {
            get { return this.FrameAtEnd.Origin; }
        }
        #endregion
    }
}
