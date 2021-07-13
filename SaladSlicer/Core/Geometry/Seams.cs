// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Core.Geometry
{
    /// <summary>
    /// A static class that contains a number of methods to manipulate the start point of closed curves.
    /// </summary>
    public static class Seams
    {
        #region methods for a single curve
        /// <summary>
        /// Returns the curve with a starting point at the given parameters. Requires a closed curve. 
        /// </summary>
        /// <param name="curve">The closed curve to change the point at start from.</param>
        /// <param name="param">The parameter of the new point at start.</param>
        /// <returns> The closed curve with a new point at start. </returns>
        public static Curve SeamAtParam(Curve curve, double param)
        {
            if (curve.IsClosed == false)
            {
                throw new Exception("Requires a closed curve.");
            }

            Curve result = curve.DuplicateCurve();

            if (param > result.Domain.T0 & param < result.Domain.T1)
            {
                result.ChangeClosedCurveSeam(param);
            }

            return result;
        }

        /// <summary>
        /// Returns the curve with the starting point a the given length. Requires a closed curve.
        /// </summary>
        /// <param name="curve"> The closed curve to change the point at start from. </param>
        /// <param name="length"> The position of the new start point defined by the length. </param>
        /// <returns> The closed curve with a new point at start. </returns>
        public static Curve SeamAtLength(Curve curve, double length)
        {
            if (curve.IsClosed == false)
            {
                throw new Exception("Requires a closed curve.");
            }

            Curve result = curve.DuplicateCurve();
            Point3d point = result.PointAtLength(length);

            result.ClosestPoint(point, out double param);
            result.ChangeClosedCurveSeam(param);

            return curve;
        }

        /// <summary>
        /// Returns the curve with the starting point closest to the given test point. Requires a closed curve.
        /// </summary>
        /// <param name="curve"> The closed curve to change the point at start from. </param>
        /// <param name="point"> The test point. </param>
        /// <returns> The closed curve with a new point at start. </returns>
        public static Curve SeamAtClosestPoint(Curve curve, Point3d point)
        {
            if (curve.IsClosed == false)
            {
                throw new Exception("Requires a closed curve.");
            }

            Curve result = curve.DuplicateCurve();

            result.ClosestPoint(point, out double param);
            result.ChangeClosedCurveSeam(param);

            return result;
        }
        #endregion

        #region methods for lists with curves
        /// <summary>
        /// Returns a list with curves with as starting point the closest point to the starting point of the curve before. 
        /// </summary>
        /// <param name="contours"> The contours as a list with Curves. </param>
        /// <returns></returns>
        public static List<Curve> SeamsAtClosestPoint(List<Curve> contours)
        {
            List<Curve> result = new List<Curve>() { };
            result.Add(contours[0].DuplicateCurve());

            Point3d testPoint = contours[0].PointAtStart;
            contours[0].ClosestPoint(testPoint, out double t);

            for (int i = 1; i < contours.Count; i++)
            {
                testPoint = contours[i - 1].PointAt(t);
                contours[i].ClosestPoint(testPoint, out t);
                result.Add(SeamAtParam(contours[i], t));
            }

            return result;
        }
        #endregion
    }
}
