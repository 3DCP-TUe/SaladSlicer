// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Salad Libs
using SaladSlicer.Core.Geometry;

namespace SaladSlicer.Core.Geometry.Seams
{
    /// <summary>
    /// A static class that contains a number of methods to create transitions between curves. 
    /// </summary>
    public static class Transitions
    {
        #region methods
        /// <summary>
        /// Joins start and end points of a list of curves linearly and returns a curve
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <returns></returns>
        public static Curve JoinLinear(List<Curve> curves)
        {
            //Make a duplicate
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());

            //Create linear transitions
            List<Curve> transitions = LinearTransitions(curvesCopy);

            //Join curves and transitions
            Curve joinedCurve = Curves.MergeCurves(curvesCopy, transitions);

            return joinedCurve;
        }

        /// <summary>
        /// Joins start and end points of a list of curves using arcs that connect outside of the original curves.
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <returns></returns>
        public static Curve JoinOutsideArc(List<Curve> curves)
        {
            //Make a duplicate
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());

            //Create arc transitions
            List<Curve> transitions = OutsideArcTransitions(curvesCopy);

            //Join curves and transitions
            Curve joinedCurve = Curves.MergeCurves(curvesCopy, transitions);

            return joinedCurve;
        }

        /// <summary>
        /// Joins start and end points of a list of curves using interpolated curves outside of the original curves
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <returns></returns>
        public static Curve JoinBezier(List<Curve> curves)
        {
            //Make a duplicate
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());

            //Create bezier transitions
            List<Curve> transitions = BezierTransitions(curvesCopy);

            //Join curves and transitions
            Curve joinedCurve = Curves.MergeCurves(curvesCopy, transitions);

            return joinedCurve;
        }

        /// <summary>
        /// Returns a single curve from a list with unconnected curves. 
        /// Interpolates between the curves at a given location and transition length.
        /// Used to join the contours in a 2.5D object.
        /// </summary>
        /// <param name="curves">List with closed curves to interpolate between.</param>
        /// <param name="length">The length over the transition between two curves.</param>
        /// <param name="precision">The precision of the transition.</param>
        /// <returns>The connected curve with interpolated transitions.</returns>
        public static Curve JoinInterpolated(List<Curve> curves, double length = 100.0, double precision = 10.0)
        {
            Curve result = Curve.JoinCurves(InterpolatedTransitions(curves, length, precision))[0];
            result.Domain = new Interval(0, result.GetLength());

            return result;
        }

        /// <summary>
        /// Creates linear transitions between start and endpoints of a list of curves
        /// </summary>
        /// <param name="curves">List of curves</param>
        /// <returns></returns>
        public static List<Curve> LinearTransitions(List<Curve> curves)
        {
            List<Curve> transitions = new List<Curve>();
            List<Plane> startFrames = Curves.GetStartFrames(curves);
            List<Plane> endFrames = Curves.GetEndFrames(curves);

            for (int i = 0; i < curves.Count - 1; i++)
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
            List<Plane> startFrames = Curves.GetStartFrames(curves);
            List<Plane> endFrames = Curves.GetEndFrames(curves);

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
            List<Plane> startFrames = Curves.GetStartFrames(curves);
            List<Plane> endFrames = Curves.GetEndFrames(curves);

            for (int i = 0; i < curves.Count - 1; i++)
            {
                Point3d[] points = { endFrames[i].Origin, startFrames[i + 1].Origin };
                Curve curve = Curve.CreateInterpolatedCurve(points, 3, CurveKnotStyle.Chord, endFrames[i].XAxis, startFrames[i + 1].XAxis);
                transitions.Add(curve.ToNurbsCurve());
            }

            return transitions;
        }

        /// <summary>
        /// Returns a list with connected curves from a list with unconnected curves. 
        /// Interpolates between the curves at a given location and transition length.
        /// Used to join the contours in a 2.5D object.
        /// </summary>
        /// <param name="curves">List with closed curves to interpolate between.</param>
        /// <param name="length">The length over the transition between two curves.</param>
        /// <param name="precision">The precision of the transition.</param>
        /// <returns>The list with connected curves.</returns>
        public static List<Curve> InterpolatedTransitions(List<Curve> curves, double length = 100.0, double precision = 10.0)
        {
            List<Curve> result = new List<Curve>();
            List<Curve> part1 = new List<Curve>();
            List<Curve> part2 = new List<Curve>();

            for (int i = 0; i < curves.Count; i++)
            {
                Curve[] splitCurves = Curves.SplitAtLength(curves[i], curves[i].GetLength() - length);
                part1.Add(splitCurves[0].DuplicateCurve());
                part2.Add(splitCurves[1].DuplicateCurve());
            }

            for (int i = 0; i < curves.Count; i++)
            {
                result.Add(part1[i]);

                if (i < curves.Count - 1)
                {
                    int n = Math.Max((int)(part2[i].GetLength() / Convert.ToDouble(precision)), 8);
                    result.Add(Curves.InterpolateCurves(part2[i], part2[i + 1], n));
                }
            }

            return result;
        }

        /// <summary>
        /// Cuts of the end of every curve in a list of curves
        /// </summary>
        /// <param name="curves">List of curves</param>
        /// <param name="length">Length to be cut off</param>
        /// <returns></returns>
        public static List<Curve> CutTransitionEnd(List<Curve> curves, double length)//Doesn't allow for changelength 0
        {
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());

            for (int i = 0; i < curves.Count; i++)
            {
                curvesCopy[i].LengthParameter(curvesCopy[i].GetLength() - length, out double param);
                Curve[] tempCurves = curvesCopy[i].Split(param);
                curvesCopy[i] = tempCurves[0];
            }

            return curvesCopy;
        }
        #endregion
    }
}
