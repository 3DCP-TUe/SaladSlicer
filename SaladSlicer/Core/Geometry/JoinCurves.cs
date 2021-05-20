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
    /// Represents an Absolute Coordinate.
    /// </summary>
    public static class JoinCurves
    {
        #region methods
        /// <summary>
        /// Joins start and end points of a list of curves linearly and returns a curve
        /// </summary>
        /// <param name="curves">List of curves</param>
        /// <returns></returns>
        public static Curve JoinLinear(List<Curve> curves,bool reverse)
        {
            List<Curve> curvesCopy = curves;
            if (reverse == true)
            {
                for (int i=1; i < curves.Count; i=i+2)
                {
                    curvesCopy[i].Reverse();
                }
            }
            List<Curve> transitions = LinearTransitions(curvesCopy);
            Curve joinedCurve = MergeCurves(curvesCopy,transitions);
            return joinedCurve;
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
