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

namespace SaladSlicer.Core.Slicers
{
    /// <summary>
    /// Represents the Planar 2D Slicer class.
    /// </summary>
    public class ClosedPlanar2DSlicer : IProgram, IObject
    {
        #region fields
        private Curve _baseContour;
        private double _distance;
        private List<Curve> _path = new List<Curve>();
        private Curve _interpolatedPath;
        private readonly List<Curve> _contours = new List<Curve>();
        private List<double> _heights = new List<double>();
        private readonly List<List<Plane>> _framesByLayer = new List<List<Plane>>() { };
        private double _seamLocation;
        private double _seamLength;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Planar 2D Slicer class.
        /// </summary>
        public ClosedPlanar2DSlicer()
        {

        }

        /// <summary>
        /// Initializes a new instance of the Planar 2D Slicer class from a list with absolute layer heights. 
        /// </summary>
        /// <param name="curve"> The base contour. </param>
        /// <param name="parameter"> The parameter of the starting point. </param>
        /// <param name="length"> The length of the seam between two layers. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        /// <param name="heights"> A list with absolute layer heights. </param>
        public ClosedPlanar2DSlicer(Curve curve, double parameter, double length, double distance, List<double> heights)
        {
            _baseContour = curve;
            _heights = heights;
            _seamLocation = parameter;
            _seamLength = length;
            _distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the Planar 2D Slicer class from a given number of layers and layer thickness.
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
        /// Initializes a new instance of the Planar 2D Slicer class by duplicating an existing Planar 2D Slicer instance. 
        /// </summary>
        /// <param name="slicer"> The Planar 2D Slicer instance to duplicate. </param>
        public ClosedPlanar2DSlicer(ClosedPlanar2DSlicer slicer)
        {
            _baseContour = slicer.BaseContour.DuplicateCurve();
            _heights = new List<double>(slicer.Heights);
            _seamLocation = slicer.SeamLocation;
            _seamLength = slicer.SeamLength;
            _distance = slicer.Distance;
            _path = slicer.Path.ConvertAll(curve => curve.DuplicateCurve());
            _contours = slicer.Contours.ConvertAll(curve => curve.DuplicateCurve());
            _interpolatedPath = slicer.InterpolatedPath.DuplicateCurve();
            
            _framesByLayer = new List<List<Plane>>();
              
            for (int i = 0; i<slicer.FramesByLayer.Count; i++)
            {
                _framesByLayer.Add(new List<Plane>(slicer.FramesByLayer[i]));
            }
        }

        /// <summary>
        /// Returns an exact duplicate of this Planar 2D Slicer instance.
        /// </summary>
        /// <returns> The exact duplicate of this Planar 2D Slicer instance. </returns>
        public ClosedPlanar2DSlicer Duplicate()
        {
            return new ClosedPlanar2DSlicer(this);
        }

        /// <summary>
        /// Returns an exact duplicate of this Planar 2D Slicer instance as an IObject.
        /// </summary>
        /// <returns> The exact duplicate of this Planar 2D Slicer instance as an IObject. </returns>
        public IObject DuplicateObject()
        {
            return this.Duplicate() as IObject;
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
            this.CreateContours();
            this.CreatePath();
            this.CreateFrames();
            this.GetInterpolatedPath();
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
        }

        /// <summary>
        /// Creates the path of this object.
        /// </summary>
        private void CreatePath()
        {
            _path.Clear();
            _path = Curves.InterpolatedTransitions(_contours, _seamLength, _seamLocation, 0.25 * _distance);
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
            }

            for (int i = 0; i < _contours.Count; i++)
            {
                // Contours
                _framesByLayer[i].AddRange(Geometry.Frames.GetFramesBySegment(_path[i * 2], _distance, true, true));

                // Transitions
                if (i < _contours.Count - 1)
                {
                    _framesByLayer[i].AddRange(Geometry.Frames.GetFramesBySegment(_path[i * 2 + 1], _distance, false, false));
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
            programGenerator.AddSlicerHeader("2.5D CLOSED PLANAR OBJECT", _contours.Count, this.GetLength());

            // Settings
            programGenerator.Program.Add("; Settings");
            programGenerator.Program.Add("BSPLINE");
            programGenerator.Program.Add("G642                ; Continuous-path mode with smoothing within the defined tolerances");
            programGenerator.Program.Add("G90                 ; Set to absolute programming");
            programGenerator.Program.Add(" ");

            // TODO
            // TANG?
            // FEEDRATE?
            // WORK OFFSET?

            // Create a loop for objects with an constant height increase per layer
            if (this.ConstantHeightIncrease())
            {
                double layerHeight = _heights[1] - _heights[0];

                // Initialize loop
                programGenerator.Program.Add($"R10 = {layerHeight:0.###}         ; Height of a single layer");
                programGenerator.Program.Add(" ");
                programGenerator.Program.Add("R20 = 0             ; Number of the current layer");
                programGenerator.Program.Add($"R21 = {_heights.Count,2:G}            ; Total number of layers");
                programGenerator.Program.Add(" ");
                programGenerator.Program.Add("; Start loop");
                programGenerator.Program.Add("LINE1:");
                
                for (int i = 0; i < _framesByLayer[0].Count; i++)
                {
                    Point3d point = _framesByLayer[0][i].Origin;
                    programGenerator.Program.Add($"X{point.X:0.###} Y{point.Y:0.###} Z={point.Z:0.###}+R10*R20");
                }

                programGenerator.Program.Add(" ");
                programGenerator.Program.Add("R20 = R20 + 1            ; Increase layer number");
                programGenerator.Program.Add("IF R20 < R21 GOTOB LINE1");
            }

            // Create standard code for objects with irregular height increse
            else
            {
                for (int i = 0; i < _framesByLayer.Count; i++)
                {
                    programGenerator.Program.Add(" ");
                    programGenerator.Program.Add($"; LAYER {i + 1:0}");

                    for (int j = 0; j < _framesByLayer[i].Count; j++)
                    {
                        Point3d point = _framesByLayer[i][j].Origin;
                        programGenerator.Program.Add($"X{point.X:0.###} Y{point.Y:0.###} Z{point.Z:0.###}");
                    }
                }
            }

            // End
            programGenerator.AddFooter();
        }

        /// <summary>
        /// Returns the interpolated path.
        /// </summary>
        /// <returns> The interpolated path. </returns>
        public Curve GetInterpolatedPath()
        {
            _interpolatedPath = Curve.CreateInterpolatedCurve(this.GetPoints(), 3, CurveKnotStyle.Chord);

            return _interpolatedPath;
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
        /// Transforms the geometry.
        /// </summary>
        /// <param name="xform"> Transformation to apply to geometry. </param>
        /// <returns> True on success, false on failure. </returns>
        public bool Transform(Transform xform)
        {
            _baseContour.Transform(xform);
            _interpolatedPath.Transform(xform);

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
        /// Gets the interpolated path as a single curve
        /// </summary>
        public Curve InterpolatedPath
        {
            get { return _interpolatedPath; }
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
