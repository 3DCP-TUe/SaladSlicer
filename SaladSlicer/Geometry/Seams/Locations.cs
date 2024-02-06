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
using Rhino.Geometry.Intersect;

namespace SaladSlicer.Geometry.Seams
{
    /// <summary>
    /// Represents a static class that contains a number of methods to manipulate the start point of closed curves.
    /// </summary>
    public static class Locations
    {
        #region methods for a single curve
        /// <summary>
        /// Returns the curve with a starting point at the given parameters. Requires a closed curve. 
        /// </summary>
        /// <param name="curve">The closed curve to change the point at start from.</param>
        /// <param name="param">The parameter of the new point at start.</param>
        /// <param name="reparametrized"> Indicates if the curve domain is normalized [0 - 1]. </param>
        /// <returns> The closed curve with a new point at start location. </returns>
        public static void SeamAtParam(this Curve curve, double param, bool reparametrized = false)
        {
            if (curve.IsClosed == false)
            {
                throw new Exception("The method Seam at Param requires a closed curve.");
            }

            if (reparametrized == true)
            {
                curve.Domain = new Interval(0, 1);
            }

            if (param >= curve.Domain.T0 & param <= curve.Domain.T1)
            {
                curve.ChangeClosedCurveSeam(param);
                curve.ResetDomain();
            }
            else
            {
                throw new Exception("Parameter is not inside curve domain.");
            }
        }

        /// <summary>
        /// Returns the curve with the starting point a the given length. Requires a closed curve.
        /// </summary>
        /// <param name="curve"> The closed curve to change the point at start from. </param>
        /// <param name="length"> The position of the new start point defined by the length. </param>
        /// <param name="normalized"> Indicates if the length factor is normalized [0 - 1] </param>
        /// <returns> The closed curve with a new point at start location. </returns>
        public static void SeamAtLength(this Curve curve, double length, bool normalized = false)
        {
            if (curve.IsClosed == false)
            {
                throw new Exception("The method Seam at Length requires a closed curve.");
            }
            if (normalized == false & length < 0.0)
            {
                throw new Exception("Length factor cannot be smaller than 0.");
            }
            if (normalized == true & length < 0.0)
            {
                throw new Exception("Normalized length factor cannot be smaller than 0.");
            }
            if (normalized == false & length > curve.GetLength())
            {
                throw new Exception("Length factor cannot be larger than curve length.");
            }
            if (normalized == true & length > 1.0)
            {
                throw new Exception("Normalized length factor cannot be larger than 1.");
            }

            double param;

            if (normalized == true)
            {
                curve.NormalizedLengthParameter(length, out param);
            }
            else
            {
                curve.LengthParameter(length, out param);
            }

            curve.ChangeClosedCurveSeam(param);
            curve.ResetDomain();
        }

        /// <summary>
        /// Returns the curve with the starting point closest to the given test point. Requires a closed curve.
        /// </summary>
        /// <param name="curve"> The closed curve to change the point at start from. </param>
        /// <param name="point"> The test point. </param>
        public static void SeamAtClosestPoint(this Curve curve, Point3d point)
        {
            if (curve.IsClosed == false)
            {
                throw new Exception("The method Seam at Closest Point requires a closed curve.");
            }

            curve.ClosestPoint(point, out double param);
            curve.ChangeClosedCurveSeam(param);
            curve.ResetDomain();
        }

        /// <summary>
        /// Returns a curve with as starting point the closest point on the curve to the given guiding curve. Requires a closed curve. 
        /// </summary>
        /// <param name="curve"> The closed curve to change the point at start from. </param>
        /// <param name="guide"> Guiding curve. </param>
        /// <returns> The closed curve with a new point at start location. </returns>
        public static void SeamClosestToCurve(this Curve curve, Curve guide)
        {
            if (curve.IsClosed == false)
            {
                throw new Exception("The method Seam Closest to Curve requires a closed curve.");
            }

            curve.ClosestPoints(guide, out Point3d point, out _);
            curve.SeamAtClosestPoint(point);
        }

        /// <summary>
        /// Returns a curve with as starting point the plane intersection point that is the closest to the plane origin. 
        /// Requires a closed curve. 
        /// </summary>
        /// <param name="curve"> The closed curve to change the point at start from. </param>
        /// <param name="plane"> The intersection plane. </param>
        /// <returns> The closed curve with a new point at start location. </returns>
        public static void SeamAtClosestPlaneIntersection(this Curve curve, Plane plane)
        {
            CurveIntersections intersections = Intersection.CurvePlane(curve, plane, 0.0);

            // No intersections
            if (intersections == null)
            {
                throw new Exception("No intersections found between the curve and the plane.");
            }

            else if (intersections.Count == 0)
            {
                throw new Exception("No intersections found between the curve and the plane.");
            }

            // Intersection closest to origin
            else
            {
                double shortestDistance = double.MaxValue;
                Point3d closestPoint = plane.Origin;

                for (int i = 0; i < intersections.Count; i++)
                {
                    double dist = intersections[i].PointA.DistanceTo(plane.Origin);

                    if (dist < shortestDistance)
                    {
                        shortestDistance = dist;
                        closestPoint = intersections[i].PointA;
                    }
                }

                curve.SeamAtClosestPoint(closestPoint);
            }
        }
        #endregion

        #region methods for lists with curves
        /// <summary>
        /// Returns a list with curves with as starting point the closest point to the starting 
        /// point of the curve before. Requires closed curves.
        /// </summary>
        /// <param name="curves"> The closed curves to change the point at start from. </param>
        /// <returns> List with closed curves with a new point at start. </returns>
        public static List<Curve> AlignSeamsByClosestPoint(IList<Curve> curves)
        {
            List<Curve> result = curves.ToList().ConvertAll(item => item.DuplicateCurve());

            Point3d testPoint = curves[0].PointAtStart;
            curves[0].ClosestPoint(testPoint, out double t);

            for (int i = 1; i < curves.Count; i++)
            {
                testPoint = curves[i - 1].PointAt(t);
                curves[i].ClosestPoint(testPoint, out t);
                result[i].SeamAtParam(t);
            }

            return result;
        }

        /// <summary>
        /// Returns a list with curves with as starting point the closest point on the curve 
        /// to the given guiding curve. Requires closed curves.
        /// </summary>
        /// <param name="curves"> The closed curves to change the point at start from. </param>
        /// <param name="guide"> The guiding curve. </param>
        /// <returns> List with closed curves with a new point at start. </returns>
        public static List<Curve> AlignSeamsAlongCurve(IList<Curve> curves, Curve guide)
        {
            List<Curve> result = curves.ToList().ConvertAll(item => item.DuplicateCurve());

            for (int i = 0; i < curves.Count; i++)
            {
                curves[i].SeamClosestToCurve(guide);
            }

            return result;
        }

        /// <summary>
        /// Returns a list with curves with as curve starting point the plane intersection point 
        /// that is the closest to the plane origin. Requires closed curves.
        /// </summary>
        /// <param name="curves"> The closed curves to change the point at start from. </param>
        /// <param name="plane"> The intersection plane. </param>
        /// <returns> List with closed curves with a new point at start.</returns>

        public static List<Curve> AlignSeamsAlongClosestPlaneIntersection(IList<Curve> curves, Plane plane)
        {
            List<Curve> result = curves.ToList().ConvertAll(item => item.DuplicateCurve());

            for (int i = 0; i < curves.Count; i++)
            {
                curves[i].SeamAtClosestPlaneIntersection(plane);
            }

            return result;
        }
        #endregion
    }
}
