﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
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
        #region methods

        #region join
        /// <summary>
        /// Joins start and end points of a list of curves linearly and returns a curve
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <param name="reverse">Reverse every other curve if true</param>
        /// /// <param name="param">Double between 0 and 1, redefining startpoint of curve if closed</param>
        /// <returns></returns>
        public static Curve JoinLinear(List<Curve> curves)
        {
            //Make a duplicate
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());
                        
            //Create linear transitions
            List<Curve> transitions = LinearTransitions(curvesCopy);
            
            //Join curves and transitions
            Curve joinedCurve = MergeCurves(curvesCopy,transitions);

            return joinedCurve;
        }

        /// <summary>
        /// Joins start and end points of a list of curves using arcs that connect outside of the original curves.
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <param name="reverse">Reverse every other curve if true</param>
        /// <param name="param">Double between 0 and 1, redefining startpoint of curve if closed</param>
        /// <returns></returns>
        public static Curve JoinOutsideArc(List<Curve> curves)
        {
            //Make a duplicate
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());
                        
            //Create arc transitions
            List<Curve> transitions = OutsideArcTransitions(curvesCopy);
            
            //Join curves and transitions
            Curve joinedCurve = MergeCurves(curvesCopy, transitions);
            
            return joinedCurve;
        }

        /// <summary>
        /// Joins start and end points of a list of curves using interpolated curves outside of the original curves
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <param name="reverse">Reverse every other curve if true</param>
        /// <returns></returns>
        public static Curve JoinBezier(List<Curve> curves)
        {
            //Make a duplicate
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());
                        
            //Create bezier transitions
            List<Curve> transitions = BezierTransitions(curvesCopy);
            
            //Join curves and transitions
            Curve joinedCurve = MergeCurves(curvesCopy, transitions);
            
            return joinedCurve;
        }
        #endregion

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
        /// Cuts of the end of every curve in a list of curves
        /// </summary>
        /// <param name="curves">List of curves</param>
        /// <param name="cutLength">Length to be cut off</param>
        /// <returns></returns>
        public static List<Curve> CutTransitionEnd(List<Curve> curves, double cutLength)//Doesn't allow for changelength 0
        {
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());
            
            for (int i = 0; i < curves.Count; i++)
            {
                curvesCopy[i].LengthParameter(curvesCopy[i].GetLength()-cutLength, out double param3);
                Curve[] tempCurves = curvesCopy[i].Split(param3);
                curvesCopy[i] = tempCurves[0];
            }
            
            return curvesCopy;
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

        #region transitions
        /// <summary>
        /// Creates linear transitions between start and endpoints of a list of curves
        /// </summary>
        /// <param name="curves">List of curves</param>
        /// <returns></returns>
        public static List<Curve> LinearTransitions(List<Curve> curves)
        {
            List<Curve> transitions = new List<Curve>();
            List<Plane> startFrames = GetStartFrames(curves);
            List<Plane> endFrames = GetEndFrames(curves);
            
            for (int i = 0; i < curves.Count-1; i++)
            {
                Line line = new Line(endFrames[i].Origin, startFrames[i + 1].Origin);
                transitions.Add(line.ToNurbsCurve());
            }
            
            return transitions;
        }

        /// <summary>
        /// Creates arc transitions between start and endpoints of a list of curves
        /// </summary>
        /// <param name="curves">List of curves</param>
        /// <returns></returns>
        public static List<Curve> OutsideArcTransitions(List<Curve> curves)
        {
            List<Curve> transitions = new List<Curve>();
            List<Plane> startFrames = GetStartFrames(curves);
            List<Plane> endFrames = GetEndFrames(curves);
            for (int i = 0; i < curves.Count - 1; i++)
            {
                Arc arc = new Arc(endFrames[i].Origin,
                    endFrames[i].XAxis,
                    startFrames[i + 1].Origin);
                transitions.Add(arc.ToNurbsCurve());
            }
            return transitions;
        }

        /// <summary>
        /// Creates Bezier ransitions between start and endpoints of a list of curves.
        /// </summary>
        /// <param name="curves"> List of curves. </param>
        /// <returns></returns>
        public static List<Curve> BezierTransitions(List<Curve> curves)
        {
            List<Curve> transitions = new List<Curve>();
            List<Plane> startFrames = GetStartFrames(curves);
            List<Plane> endFrames = GetEndFrames(curves);

            for (int i = 0; i < curves.Count - 1; i++)
            {
                Point3d[] points = { endFrames[i].Origin, startFrames[i + 1].Origin };
                Curve curve = Curve.CreateInterpolatedCurve(points, 3, CurveKnotStyle.Chord, endFrames[i].XAxis, startFrames[i + 1].XAxis);
                transitions.Add(curve.ToNurbsCurve());
            }

            return transitions;
        }
        #endregion

        /// <summary>
        /// Joins a list of curves and a list of transitions and return a single curve
        /// </summary>
        /// <param name="curves">List of curves</param>
        /// <param name="transitions">List of transitions</param>
        /// <returns></returns>
        public static Curve MergeCurves(List<Curve> curves,List<Curve> transitions)
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
        /// Returns a single curve from a list with unconnected curves. 
        /// Interpolates between the curves at a given location and transition length.
        /// Used to join the contours in a 2.5D object.
        /// </summary>
        /// <param name="curves">List with closed curves to interpolate between.</param>
        /// <param name="length">The length over the transition between two curves.</param>
        /// <param name="param">The parameter of the transition location.</param>
        /// <param name="precision">The precision of the transition.</param>
        /// <returns>The connected curve with interpolated transitions.</returns>
        public static Curve JoinInterpolatedTransitions(List<Curve> curves, double length = 100.0, double param = 0.0, double precision = 10.0)
        {
            Curve result = Curve.JoinCurves(InterpolatedTransitions(curves, length, param, precision))[0];
            result.Domain = new Interval(0, result.GetLength());

            return result;
        }

        /// <summary>
        /// Returns a list with connected curves from a list with unconnected curves. 
        /// Interpolates between the curves at a given location and transition length.
        /// Used to join the contours in a 2.5D object.
        /// </summary>
        /// <param name="curves">List with closed curves to interpolate between.</param>
        /// <param name="length">The length over the transition between two curves.</param>
        /// <param name="param">The parameter of the transition location (0-1).</param>
        /// <param name="precision">The precision of the transition.</param>
        /// <returns>The list with connected curves.</returns>
        public static List<Curve> InterpolatedTransitions(List<Curve> curves, double length = 100.0, double param = 0.0, double precision = 10.0)
        {
            List<Curve> result = new List<Curve>();
            List<Curve> part1 = new List<Curve>();
            List<Curve> part2 = new List<Curve>();

            for (int i = 0; i < curves.Count; i++)
            {
                curves[i].Domain = new Interval(0, 1);
                curves[i] = Locations.SeamAtLength(curves[i], param, true);

                Curve[] splitCurves = SplitAtLength(curves[i], curves[i].GetLength() - length);

                part1.Add(splitCurves[0].DuplicateCurve());
                part2.Add(splitCurves[1].DuplicateCurve());
            }

            for (int i = 0; i < curves.Count; i++)
            {
                result.Add(part1[i]);

                if (i < curves.Count - 1)
                {
                    int n = Math.Max((int)(part2[i].GetLength() / Convert.ToDouble(precision)), 8);
                    result.Add(InterpolateCurves(part2[i], part2[i+1], n));
                }
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
        #endregion

        #region properties
        #endregion
    }
}
