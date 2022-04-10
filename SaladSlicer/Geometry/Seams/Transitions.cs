// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Geometry.Seams
{
    /// <summary>
    /// Represents a static class that contains a number of methods to create transitions between curves. 
    /// </summary>
    public static class Transitions
    {
        #region methods
        /// <summary>
        /// Joins start and end points of a list of curves linearly and returns a curve
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <returns></returns>
        public static (Curve, List<Curve>) JoinLinear(IList<Curve> curves)
        {
            // Create linear transitions
            List<Curve> transitions = LinearTransitions(curves);

            // Join curves and transitions
            Curve joinedCurve = Curves.MergeCurves(curves, transitions);

            return (joinedCurve, transitions);
        }

        /// <summary>
        /// Joins start and end points of a list of curves using arcs that connect outside of the original curves.
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <returns></returns>
        public static (Curve, List<Curve>) JoinOutsideArc(IList<Curve> curves)
        {
            // Create arc transitions
            List<Curve> transitions = OutsideArcTransitions(curves);

            // Join curves and transitions
            Curve joinedCurve = Curves.MergeCurves(curves, transitions);

            return (joinedCurve, transitions);
        }

        /// <summary>
        /// Joins start and end points of a list of curves using interpolated curves outside of the original curves
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <returns></returns>
        public static (Curve, List<Curve>) JoinBezier(IList<Curve> curves)
        {
            // Create bezier transitions
            List<Curve> transitions = BezierTransitions(curves);

            // Join curves and transitions
            Curve joinedCurve = Curves.MergeCurves(curves, transitions);

            return (joinedCurve, transitions);
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
        public static (Curve, List<Curve>) JoinInterpolated(IList<Curve> curves, double length = 100.0, double precision = 1.0)
        {
            // Create trimmed contours and transitions
            List<Curve> trimmed = TrimCurveFromEnds(curves, length);
            List<Curve> transitions = InterpolatedTransitions(curves, length, precision);

            // Join curves and transitions
            Curve joinedCurve = Curves.MergeCurves(trimmed, transitions);

            return (joinedCurve, transitions);
        }

        /// <summary>
        /// Creates linear transitions between start and endpoints of a list of curves
        /// </summary>
        /// <param name="curves">List of curves</param>
        /// <returns></returns>
        public static List<Curve> LinearTransitions(IList<Curve> curves)
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
        public static List<Curve> OutsideArcTransitions(IList<Curve> curves)
        {
            List<Curve> transitions = new List<Curve>();
            List<Plane> startFrames = Curves.GetStartFrames(curves);
            List<Plane> endFrames = Curves.GetEndFrames(curves);

            for (int i = 0; i < curves.Count - 1; i++)
            {
                Arc arc = new Arc(endFrames[i].Origin, endFrames[i].XAxis, startFrames[i + 1].Origin);
                transitions.Add(arc.ToNurbsCurve());
            }

            return transitions;
        }

        /// <summary>
        /// Creates Bezier ransitions between start and endpoints of a list of curves.
        /// </summary>
        /// <param name="curves"> List of curves. </param>
        /// <returns></returns>
        public static List<Curve> BezierTransitions(IList<Curve> curves)
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
        public static List<Curve> InterpolatedTransitions(IList<Curve> curves, double length = 100.0, double precision = 1.0)
        {
            List<Curve> contours = new List<Curve>();
            List<Curve> cuts = new List<Curve>();
            List<Curve> transitions = new List<Curve>();

            // Trim ends
            for (int i = 0; i < curves.Count; i++)
            {
                contours.Add(TrimCurveFromEnds(curves[i], length, out Curve cut));
                cuts.Add(cut);
            }

            // Interpolate transitions
            for (int i = 0; i < curves.Count - 1; i++)
            {
                if (i < curves.Count - 1)
                {
                    int n = Math.Max(Convert.ToInt32((cuts[i].GetLength() / Convert.ToDouble(precision))), 8);
                    transitions.Add(Curves.InterpolateCurves(cuts[i], cuts[i + 1], n));
                }
            }

            return transitions;
        }

        /// <summary>
        /// Returns a curve that is trimmed at both ends by a given length. 
        /// </summary>
        /// <param name="curve"> Curve. </param>
        /// <param name="length"> Total length to be cut off. </param>
        /// <returns> Trimmed curve. </returns>
        public static Curve TrimCurveFromEnds(Curve curve, double length)
        {
            return TrimCurveFromEnds(curve, length, out _);
        }

        /// <summary>
        /// Returns a curve that is trimmed at both ends by a given length. 
        /// </summary>
        /// <param name="curve"> Curve. </param>
        /// <param name="length"> Total length to be cut off. </param>
        /// <returns> Trimmed curve. </returns>
        public static Curve TrimCurveFromEnds(Curve curve, double length, out Curve cut)
        {
            if (length <= 0)
            {
                throw new Exception("The method Trim Curve from Ends requires a length larger than zero.");
            }

            Curve[] temp1;
            Curve[] temp2;

            curve.LengthParameter(curve.GetLength() - 0.5 * length, out double param1);
            temp1 = curve.Split(param1);
            Curve result = temp1[0];

            result.LengthParameter(0.5 * length, out double param2);
            temp2 = result.Split(param2);
            result = temp2[1];
            
            if (curve.IsClosed == true)
            {
                cut = Curve.JoinCurves(new List<Curve>() { temp1[1], temp2[0] })[0];
            }
            else
            {
                cut = null;
            }

            return result;
        }

        /// <summary>
        /// Returns a list with curves that are trimmed at both ends by a given length. 
        /// </summary>
        /// <param name="curves"> List of curves. </param>
        /// <param name="length"> Total length to be cut off. </param>
        /// <returns></returns>
        public static List<Curve> TrimCurveFromEnds(IList<Curve> curves, double length)
        {
            return TrimCurveFromEnds(curves, length, out _);
        }

        /// <summary>
        /// Returns a list with curves that are trimmed at both ends by a given length. 
        /// </summary>
        /// <param name="curves"> List of curves. </param>
        /// <param name="length"> Total length to be cut off. </param>
        /// <returns></returns>
        public static List<Curve> TrimCurveFromEnds(IList<Curve> curves, double length, out List<Curve> cuts)
        {
            if (length <= 0)
            {
                throw new Exception("The method Trim Curve from Ends requires a length larger than zero.");
            }

            List<Curve> result = new List<Curve>();
            cuts = new List<Curve>();

            for (int i = 0; i < curves.Count; i++)
            {
                result.Add(TrimCurveFromEnds(curves[i], length, out Curve cut));
                cuts.Add(cut);
            }

            return result;
        }
        #endregion
    }
}
