// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Geometry
{
    /// <summary>
    /// Represents a static class that contains a number of methods to manipulate curves.
    /// </summary>
    public static class Curves
    {
        #region method
        /// <summary>
        /// Returns a curve with a domain that starts at zero by shifting the current domain.
        /// </summary>
        /// <param name="curve"> The curve. </param>
        /// <returns> The curve with a new domain. </returns>
        public static void ResetDomain(this Curve curve)
        {
            curve.Domain = new Interval(curve.Domain.Min - curve.Domain.Min, curve.Domain.Max - curve.Domain.Min);
        }

        /// <summary>
        /// Returns the number of closed curves in a list of curves
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <returns> The number of closed curves. </returns>
        public static double NumberClosed(IList<Curve> curves)
        {
            double numberClosed = 0;

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
        /// <param name="curves"> List of curves. </param>
        /// <returns> The alternated curves. </returns>
        public static List<Curve> AlternateCurves(IList<Curve> curves)
        {
            List<Curve> result = curves.ToList().ConvertAll(item => item.DuplicateCurve());

            for (int i = 1; i < result.Count; i += 2)
            {
                result[i].Reverse();
            }
            
            return result;
        }

        /// <summary>
        /// Returns a list of starting frames from a list of curves.
        /// </summary>
        /// <param name="curves"> List of curves. </param>
        /// <returns> The list with start frames. </returns>
        public static List<Plane> GetStartFrames(IList<Curve> curves)
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
        /// Returns a list of end frames from a list of curves.
        /// </summary>
        /// <param name="curves"> List of curves. </param>
        /// <returns> The list with end frames. </returns>
        public static List<Plane> GetEndFrames(IList<Curve> curves)
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
        /// <param name="curves"> List of curves. </param>
        /// <param name="transitions"> List of transitions. </param>
        /// <returns> The joined curve. </returns>
        public static Curve MergeCurves(IList<Curve> curves, IList<Curve> transitions)
        {
            List<Curve> weave = WeaveCurves(curves, transitions);
            Curve result = Curve.JoinCurves(weave)[0];

            return result;
        }

        /// <summary>
        /// Returns a woven list with curves from two lists of curves. 
        /// </summary>
        /// <param name="curves"> First list with curves. </param>
        /// <param name="transitions"> Second list with curves. </param>
        /// <returns> Woven list with curves. </returns>
        public static List<Curve> WeaveCurves(IList<Curve> curves, IList<Curve> transitions)
        {
            List<Curve> result = new List<Curve>() { };

            int n1 = curves.Count;
            int n2 = transitions.Count;
            int n = Math.Max(n1, n2);

            for (int i = 0; i < n + 1; i++)
            {
                if (i < n1)
                {
                    result.Add(curves[i].DuplicateCurve());
                }

                if (i < n2)
                {
                    result.Add(transitions[i].DuplicateCurve());
                }
            }

            return result;
        }

        /// <summary>
        /// Splits (divides) the curve at a specified length.
        /// </summary>
        /// <remarks>
        /// The length must be in the interior of the curve.
        /// </remarks>
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
        /// Returns a list with aligned curves.  
        /// <remarks>
        /// Checks the direction of all curves and aligns (reverses) them if necessary. 
        /// </remarks>
        /// </summary>
        /// <param name="curves"> List with curves. </param>
        /// <returns> List with aligned curves. </returns>
        public static List<Curve> AlignCurves(IList<Curve> curves)
        {
            List<Curve> result = curves.ToList().ConvertAll(item => item.DuplicateCurve());

            for (int i = 1; i < result.Count; i++)
            {
                bool match = Curve.DoDirectionsMatch(result[i - 1], result[i]);

                if (match == false)
                {
                    result[i].Reverse();
                    result[i].Domain = curves[i].Domain;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a curve that is alinged with another curve.
        /// </summary>
        /// <remarks>
        /// Checks the direction of the curve and reverses them if necessary. 
        /// </remarks>
        /// <param name="curve1"> The curve to align. </param>
        /// <param name="curve2"> The curve to check againts. </param>
        public static void AlignCurve(this Curve curve1, Curve curve2)
        {
            if (!Curve.DoDirectionsMatch(curve1, curve2))
            {
                Interval domain = new Interval(curve1.Domain.Min, curve1.Domain.Max);
                curve1.Reverse();
                curve1.Domain = domain;
            }
        }

        /// <summary>
        /// Returns a curve that is an interpolation between two curves.
        /// </summary>
        /// <remarks>
        /// The start point of the interpolated curve is equal to the start point of the first curve.
        /// The end point of the interpolated curve is equal to the end point of the second curve.
        /// </remarks>
        /// <param name="curve1"> The first curve. </param>
        /// <param name="curve2"> The second curve. </param>
        /// <param name="tolerance"> The tolarance defined as the distance between interpolation points. </param>
        /// <returns> The interpolated curve. </returns>
        public static Curve InterpolateCurves(Curve curve1, Curve curve2, double tolerance = 1.0)
        {
            List<Point3d> points = new List<Point3d>() { };
            double length = Math.Max(curve1.GetLength(), curve2.GetLength());
            int n = Math.Max((int)(length / tolerance), 8);

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
