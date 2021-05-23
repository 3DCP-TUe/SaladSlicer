// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Rhino.Geometry;
using System.Collections.Generic;

namespace SaladSlicer.Core.Geometry
{
    /// <summary>
    /// A static class that contains a number of methods to manipulate a list of curves.
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
        /// <returns></returns>
        public static Curve JoinLinear(List<Curve> curves, bool reverse)
        {
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());
            if (reverse == true){curvesCopy = ReverseEveryOther(curvesCopy);}
            List<Curve> transitions = LinearTransitions(curvesCopy);
            Curve joinedCurve = MergeCurves(curvesCopy,transitions);
            return joinedCurve;
        }
        /// <summary>
        /// Joins start and end points of a list of curves using arcs that connect outside of the original curves.
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <param name="reverse">Reverse every other curve if true</param>
        /// <returns></returns>
        public static Curve JoinOutsideArc(List<Curve> curves, bool reverse)
        {
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());
            if (reverse == true) { curvesCopy = ReverseEveryOther(curvesCopy); }
            List<Curve> transitions = OutsideArcTransitions(curvesCopy);
            Curve joinedCurve = MergeCurves(curvesCopy, transitions);
            return joinedCurve;
        }

        /// <summary>
        /// Joins start and end points of a list of curves using interpolated curves outside of the original curves
        /// </summary>
        /// <param name="curves">The list of curves</param>
        /// <param name="reverse">Reverse every other curve if true</param>
        /// <returns></returns>
        public static Curve JoinOutsideInterpolated(List<Curve> curves, bool reverse)
        {
            List<Curve> curvesCopy = curves.ConvertAll(curve => curve.DuplicateCurve());
            if (reverse == true) { curvesCopy = ReverseEveryOther(curvesCopy); }
            List<Curve> transitions = OutsideInterpolatedTransitions(curvesCopy);
            Curve joinedCurve = MergeCurves(curvesCopy, transitions);
            return joinedCurve;
        }
        #endregion

        public static double NumberClosed(List<Curve> curves)
        {
            double numberClosed = 0 ;
            for (int i = 0; i < curves.Count; i++)
            {
                bool close = curves[i].IsClosed;
                if (close == true)
                {
                    numberClosed = numberClosed + 1;
                }
            }
            
            return numberClosed;
        }
        public static List<Curve> CutTransitionEnd(List<Curve> curves, double cutLength, double cutParameter)
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
        /// <param name="curves"></param>
        /// <returns></returns>
        public static List<Curve> ReverseEveryOther(List<Curve> curves)
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
                Line line = new Line(endFrames[i].Origin,startFrames[i + 1].Origin);
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

        public static List<Curve> OutsideInterpolatedTransitions(List<Curve> curves)
        {
            List<Curve> transitions = new List<Curve>();
            List<Plane> startFrames = GetStartFrames(curves);
            List<Plane> endFrames = GetEndFrames(curves);
            for (int i = 0; i < curves.Count - 1; i++)
            {
                Point3d[] points = { endFrames[i].Origin, startFrames[i + 1].Origin };
                Curve curve = Curve.CreateInterpolatedCurve(points,
                                                            3,
                                                            CurveKnotStyle.Chord,
                                                            endFrames[i].XAxis,
                                                            startFrames[i + 1].XAxis);
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
        #endregion

        #region properties
        #endregion
    }
}
