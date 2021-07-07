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


            // if (curves[0].IsClosed == false)
            //{
            //    throw new Exception("You did stupid stuff");
            //}

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

        public static List<Curve> BezierTransitions(List<Curve> curves)
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

        /// <summary>
        /// Returns the curve with a starting point at the given parameters. Requires a closed curve. 
        /// </summary>
        /// <param name="curve">The closed curve to change the point at start from.</param>
        /// <param name="param">The parameter of the new point at start.</param>
        /// <returns>The closed curve with a new point at start.</returns>
        public static Curve SetStartPointAtParam(Curve curve, double param)
        {
            Curve result = curve.DuplicateCurve();

            if (param > result.Domain.T0 & param < result.Domain.T1 & result.IsClosed == true)
            {
                result.ChangeClosedCurveSeam(param);
            }

            return result;
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
                curves[i] = SetStartPointAtParam(curves[i], param);

                curves[i].Domain = new Interval(0, curves[i].GetLength());
                double splitParam = curves[i].GetLength() - length;
                Curve[] splitCurves = curves[i].Split(splitParam);

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
        /// Returns a list with curves with as starting point the closest point to the starting point of the curve before. 
        /// The parameter defines the starting point of the first curve. 
        /// </summary>
        /// <param name="contours"> The contours as a list with Curves. </param>
        /// <param name="parameter"> The parameter tha defines the starting point of the first curve. </param>
        /// <returns></returns>
        public static List<Curve> SetSeamClosestPoint(List<Curve> contours, double parameter)
        {
            List<Curve> result = new List<Curve>() { };
            result.Add(Curves.SetStartPointAtParam(contours[0], parameter));
            
            double t = parameter;

            for (int i = 1; i < contours.Count; i++)
            {
                Point3d testPoint = contours[i - 1].PointAt(t);
                contours[i].ClosestPoint(testPoint, out t);
                result.Add(Curves.SetStartPointAtParam(contours[i], t));
            }

            return result;
        }
        #endregion

        #region properties
        #endregion
    }
}
