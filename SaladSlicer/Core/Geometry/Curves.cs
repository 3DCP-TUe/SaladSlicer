// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Salad Slicer Libs
using SaladSlicer.Core.Geometry.Seams;

namespace SaladSlicer.Core.Geometry
{
    /// <summary>
    /// A static class that contains a number of methods to manipulate curves.
    /// </summary>
    public static class Curves
    {
        #region method
        /// <summary>
        /// Returns the number of closed curves in a list of curves
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <returns></returns>
        public static double NumberClosed(List<Curve> curves)
        {
            double numberClosed = 0 ;
            for (int i = 0; i < curves.Count; i++)
            {
                bool close = curves[i].IsClosed;
                if (close == true)
                {
                    numberClosed++;
                }
            }
            
            return numberClosed;
        }

        /// <summary>
        /// Reverses every other curve in a list of curves, starting with the second.
        /// </summary>
        /// <param name="curves">List of curves</param>
        /// <returns></returns>
        public static List<Curve> AlternateCurves(List<Curve> curves)
        {
            for (int i = 1; i < curves.Count; i += 2)
            {
                curves[i].Reverse();
            }
            
            return curves;
        }

        /// <summary>
        /// Returns a list of starting frames from a list of curves
        /// </summary>
        /// <param name="curves">List of curves</param>
        /// <returns></returns>
        public static List<Plane> GetStartFrames(List<Curve> curves)
        {
            List<Plane> startFrames = new List<Plane>();
            
            for (int i = 0; i < curves.Count; i++)
            {
                Point3d point = curves[i].PointAtStart;
                Vector3d x = curves[i].TangentAtStart;
                Vector3d y = Vector3d.CrossProduct(x, new Vector3d(0, 0, 1));
                startFrames.Add(new Plane(point, x, y));
            }

            return startFrames;
        }

        /// <summary>
        /// Returns a list of end frames from a list of curves
        /// </summary>
        /// <param name="curves">List of curves</param>
        /// <returns></returns>
        public static List<Plane> GetEndFrames(List<Curve> curves)
        {
            List<Plane> endFrames = new List<Plane>();
            
            for (int i = 0; i < curves.Count; i++)
            {
                Point3d point = curves[i].PointAtEnd;
                Vector3d x = curves[i].TangentAtEnd;
                Vector3d y = Vector3d.CrossProduct(x, new Vector3d(0, 0, 1));
                endFrames.Add(new Plane(point, x, y));
            }
            
            return endFrames;
        }

        /// <summary>
        /// Joins a list of curves and a list of transitions and return a single curve
        /// </summary>
        /// <param name="curves">List of curves</param>
        /// <param name="transitions">List of transitions</param>
        /// <returns></returns>
        public static Curve MergeCurves(List<Curve> curves, List<Curve> transitions)
        {
            Curve curve1=curves[0];
            
            for (int i = 0; i < transitions.Count; i++)
            {
                curve1 = Curve.JoinCurves(new List<Curve>() {curve1, transitions[i] })[0];
                curve1 = Curve.JoinCurves(new List<Curve>() {curve1, curves[i+1] })[0];
            }
            return curve1;
        }

        /// <summary>
        /// Returns a woven list with curves from two lists of curves. 
        /// </summary>
        /// <param name="curves1"> First list with curves. </param>
        /// <param name="curves2"> Second list with curves. </param>
        /// <returns> Woven list with curves. </returns>
        public static List<Curve> WeaveCurves(List<Curve> curves1, List<Curve> curves2)
        {
            List<Curve> result = new List<Curve>() { };

            int n1 = curves1.Count;
            int n2 = curves2.Count;
            int n = Math.Max(n1, n2);

            for (int i = 0; i < n + 1; i++)
            {
                if (i < n1)
                {
                    result.Add(curves1[i].DuplicateCurve());
                }

                if (i < n2)
                {
                    result.Add(curves2[i].DuplicateCurve());
                }
            }

            return result;
        }

        /// <summary>
        /// Splits (divides) the curve at a specified length. The length must be in the interior of the curve.
        /// </summary>
        /// <param name="length"> The specified length. </param>
        /// <returns> The collection with split curves. </returns>
        public static Curve[] SplitAtLength(Curve curve, double length)
        {
            if (length < 0.0 & length > curve.GetLength())
            {
                throw new Exception("Defined length is outside the interior of the curve.");
            }

            Point3d point = curve.PointAtLength(length);
            curve.ClosestPoint(point, out double param);
            Curve[] result = curve.Split(param);

            return result;
        }

        /// <summary>
        /// Returns a list with aligned contours. 
        /// Checks directions of all curves and aligns (reverses) them if necessary. 
        /// </summary>
        /// <param name="contours"> List with contours. </param>
        /// <returns> List with aligned curves. </returns>
        public static List<Curve> AlignContours(List<Curve> contours)
        {
            List<Curve> result = Locations.SeamsAtClosestPoint(contours);

            for (int i = 1; i < result.Count; i++)
            {
                Vector3d tan1 = result[i - 1].TangentAtStart;
                Vector3d tan2 = result[i].TangentAtStart;

                double angle1 = Vector3d.VectorAngle(tan1, tan2);
                double angle2 = Vector3d.VectorAngle(tan1, -tan2);

                if (angle1 > angle2) { result[i].Reverse(); }
            }

            return result;
        }

        /// <summary>
        /// Returns a curve that is an interpolation between two curves.
        /// The start point of the interpolated curve is equal to the start point of the first curve.
        /// The end point of the interpolated curve is equal to the end point of the second curve.
        /// </summary>
        /// <param name="curve1"> The first curve. </param>
        /// <param name="curve2"> The second curve. </param>
        /// <param name="precision"> The precision. </param>
        /// <returns></returns>
        public static Curve InterpolateCurves(Curve curve1, Curve curve2, double precision = 10.0)
        {
            List<Point3d> points = new List<Point3d>() { };
            double length = Math.Max(curve1.GetLength(), curve2.GetLength());
            int n = Math.Max((int)(length / precision), 8);

            double[] param1 = curve1.DivideByCount(n, true);
            double[] param2 = curve2.DivideByCount(n, true);

            for (int i = 0; i < n + 1; i++)
            {
                Point3d point1 = curve1.PointAt(param1[i]);
                Point3d point2 = curve2.PointAt(param2[i]);

                Vector3d delta = point2 - point1;
                double factor = Convert.ToDouble(i) / Convert.ToDouble(n);

                points.Add(point1 + factor * delta);
            }

            Curve result = Curve.CreateInterpolatedCurve(points, 3, CurveKnotStyle.Chord);

            return result;
        }
        #endregion

        #region properties
        #endregion
    }
}
