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

namespace SaladSlicer.Core.Slicers
{
    /// <summary>
    /// Represents the Planar 2D Slicer class.
    /// </summary>
    public class Planar2DSlicer : IProgram, IObject
    {
        #region fields
        private Curve _baseContour;
        private double _distance;
        private readonly List<Curve> _path = new List<Curve>();
        private Curve _interpolatedPath;
        private readonly List<Curve> _contours = new List<Curve>();
        private List<double> _heights = new List<double>();
        private readonly List<Plane> _frames = new List<Plane>();
        private double _changeParameter;
        private double _changeLength;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the Planar 2D Slicer class.
        /// </summary>
        public Planar2DSlicer()
        {

        }

        /// <summary>
        /// Initializes a new instance of the Planar 2D Slicer class from a list with absolute layer heights. 
        /// </summary>
        /// <param name="curve"> The base contour. </param>
        /// <param name="parameter"> The parameter of the starting point. </param>
        /// <param name="length"> The length of the change between two layers. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        /// <param name="heights"> A list with absolute layer heights. </param>
        public Planar2DSlicer(Curve curve, double parameter, double length, double distance, List<double> heights)
        {
            _baseContour = curve;
            _heights = heights;
            _changeParameter = parameter;
            _changeLength = length;
            _distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the Planar 2D Slicer class from a given number of layers and layer thickness.
        /// </summary>
        /// <param name="curve"> The base contour. </param>
        /// <param name="parameter"> The parameter of the starting point. </param>
        /// <param name="length"> The length of the change between two layers. </param>
        /// <param name="distance"> The desired distance between two frames. </param>
        /// <param name="height"> The layer height. </param>
        /// <param name="layers"> The number of layers. </param>
        public Planar2DSlicer(Curve curve, double parameter, double length, double distance, double height, int layers)
        {
            _baseContour = curve;
            _heights.AddRange(Enumerable.Repeat(height, layers).ToList());
            _changeParameter = parameter;
            _changeLength = length;
            _distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the Planar 2D Slicer class by duplicating an existing Planar 2D Slicer instance. 
        /// </summary>
        /// <param name="slicer"> The Planar 2D Slicer instance to duplicate. </param>
        public Planar2DSlicer(Planar2DSlicer slicer)
        {
            _baseContour = slicer.BaseContour.DuplicateCurve();
            _heights = new List<double>(slicer.Heights);
            _changeParameter = slicer.ChangeParameter;
            _changeLength = slicer.ChangeLength;
            _distance = slicer.Distance;
            _path = slicer.Path.ConvertAll(curve => curve.DuplicateCurve());
            _contours = slicer.Contours.ConvertAll(curve => curve.DuplicateCurve());
            _frames = new List<Plane>(slicer.Frames);
        }

        /// <summary>
        /// Returns an exact duplicate of this Planar 2D Slicer instance.
        /// </summary>
        /// <returns> The exact duplicate of this Planar 2D Slicer instance. </returns>
        public Planar2DSlicer Duplicate()
        {
            return new Planar2DSlicer(this);
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
            return "2.5D Planar Object";
        }

        /// <summary>
        /// Slices this object.
        /// </summary>
        public void Slice()
        {
            this.CreateContours();
            this.CreatePath();
            this.CreateFrames();
            this.CreateInterpolatedPath();
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

            double contourLength = _baseContour.GetLength();
            double splitLocation = _changeParameter * contourLength;

            List<double> parameters;
            Point3d point1;
            Point3d point2;
            double param1;
            double param2;
            Curve curve1;
            Curve curve2;
            Curve dum1;
            Curve dum2;

            if (splitLocation < _changeLength)
            {
                param1 = contourLength - splitLocation - 0.5 * _changeLength;
                param2 = splitLocation + 0.5 * _changeLength;
                parameters = new List<double>() { param1, param2, contourLength };
            }
            else if (contourLength - splitLocation < _changeLength)
            {
                param1 = splitLocation - 0.5 * _changeLength;
                param2 = contourLength - splitLocation + 0.5 * _changeLength;
                parameters = new List<double>() { param2, param1, contourLength };
            }
            else
            {
                param1 = splitLocation - 0.5 * _changeLength;
                param2 = splitLocation + 0.5 * _changeLength;
                parameters = new List<double>() { param1, param2, contourLength };
            }
            
            for (int i = 0; i < _contours.Count; i++)
            {
                Curve[] splitted = _contours[i].Split(parameters);

                if (i < _contours.Count - 1)
                {
                    point1 = _contours[i].PointAt(param1);
                    point2 = _contours[i + 1].PointAt(param2);
                }
                else
                {
                    point1 = _contours[i].PointAt(param1);
                    point2 = _contours[i].PointAt(param2);
                }

                if (splitLocation < _changeLength )
                {
                    curve1 = splitted[1];
                    dum1 = Curve.JoinCurves(new List<Curve>() { splitted[2], splitted[0] })[0];
                }
                else if (contourLength - splitLocation < _changeLength)
                {
                    curve1 = splitted[1];
                    dum1 = Curve.JoinCurves(new List<Curve>() { splitted[2], splitted[0] })[0];
                }
                else
                {
                    curve1 = Curve.JoinCurves(new List<Curve>() { splitted[0], splitted[2] })[0];
                    dum1 = splitted[1];
                }

                dum2 = new Line(point1, point2).ToNurbsCurve();

                int n = (int)(dum1.GetLength() / _distance) * 2;
                n = Math.Max(2, n);

                double[] t1 = dum1.DivideByCount(n, true);
                double[] t2 = dum2.DivideByCount(n, true);

                List<Point3d> points = new List<Point3d>();

                for (int j = 0; j != n + 1; j++)
                {
                    Point3d p1 = dum1.PointAt(t1[j]);
                    Point3d p2 = dum2.PointAt(t2[j]);

                    points.Add(new Point3d(p1.X, p1.Y, p2.Z));
                }

                curve2 = Curve.CreateInterpolatedCurve(points, 3);

                _path.Add(curve1.DuplicateCurve());

                if (i < _contours.Count - 1)
                {
                    _path.Add(curve2.DuplicateCurve());
                }
            }
        }

        /// <summary>
        /// Creates the frames of the path.
        /// </summary>
        private void CreateFrames()
        {
            _frames.Clear();

            for (int i = 0; i < _path.Count; i++)
            {
                bool includeEnds = false;

                if (i % 2 == 0)
                {
                    includeEnds = true;
                }

                int n = (int)(_path[i].GetLength() / _distance);
                n = Math.Max(2, n);
                double[] t = _path[i].DivideByCount(n, includeEnds);

                for (int j = 0; j != t.Length; j++)
                {
                    Point3d point = _path[i].PointAt(t[j]);
                    Vector3d x = _path[i].TangentAt(t[j]);
                    Vector3d y = Vector3d.CrossProduct(x, new Vector3d(0, 0, 1));
                    Plane plane = new Plane(point, x, y);

                    _frames.Add(plane);
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
            programGenerator.Program.Add(" ");
            programGenerator.Program.Add("; --------------------------------------------------------------------------");
            programGenerator.Program.Add($"; 2.5D PLANAR OBJECT - {_contours.Count:0} LAYERS - {(this.GetLength() / 1000):0.###} METER");
            programGenerator.Program.Add("; --------------------------------------------------------------------------");
            programGenerator.Program.Add(" ");

            // Settings
            programGenerator.Program.Add("BSPLINE");
            programGenerator.Program.Add("G642");
            programGenerator.Program.Add("G90");
            programGenerator.Program.Add(" ");

            // TODO
            // TANG?
            // FEEDRATE?
            // WORK OFFSET?

            // Coords
            for (int i = 0; i < _frames.Count; i++)
            {
                Point3d point = _frames[i].Origin;
                programGenerator.Program.Add($"X{point.X:0.###} Y{point.Y:0.###} Z{point.Z:0.###}");
            }

            // End
            programGenerator.Program.Add(" ");
            programGenerator.Program.Add("; --------------------------------------------------------------------------");
            programGenerator.Program.Add(" ");
        }

        /// <summary>
        /// Creates the interpolated path.
        /// </summary>
        public void CreateInterpolatedPath()
        {
            _interpolatedPath = Curve.CreateInterpolatedCurve(this.GetPoints(), 3);
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
            _baseContour.Transform(xform);
            _interpolatedPath.Transform(xform);

            for (int i = 0; i < _frames.Count; i++)
            {
                _frames[i].Transform(xform);
            }

            for (int i = 0; i < _path.Count; i++)
            {
                _path[i].Transform(xform);
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
                if (_frames == null) { return false; }
                if (_distance <= 0.0) { return false; }
                if (_changeLength <= 0.0) { return false; }
                if (_changeParameter < 0.0) { return false; }
                if (_changeParameter > 1.0) { return false; }
                if (_contours.Count == 0) { return false; }
                if (_heights.Count == 0) { return false; }
                if (_frames.Count == 0) { return false; }
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
        /// Gets or sets the parameter of the starting point.
        /// </summary>
        public double ChangeParameter
        {
            get { return _changeParameter; }
            set { _changeParameter = value; }
        }

        /// <summary>
        /// Gets or sets the transition length beteen two layers.
        /// </summary>
        public double ChangeLength
        {
            get { return _changeLength; }
            set { _changeLength = value; }
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
            get { return _frames; }
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
