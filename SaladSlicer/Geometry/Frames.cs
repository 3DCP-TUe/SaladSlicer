// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace SaladSlicer.Geometry
{
    /// <summary>
    /// Represents a static class that contains a number of methods to manipulate and create frames.
    /// </summary>
    public static class Frames
    {
        #region methods
        /// <summary>
        /// Returns a list with a reduced amount of frames. If there is no curvature the frame will be removed. 
        /// </summary>
        /// <returns> The list with reduced amount of frames. </returns>
        /// <param name="frames"> List with frames to check. </param>
        /// <param name="curve"> Curve to obtain the curvature from. </param>
        /// <param name="keep"> The least amount of frames to keep that are following within the given threshold. </param>
        /// <param name="threshold"> Threshold value for frame removal. </param>
        public static List<Plane> ReduceFramesByCurvature(IList<Plane> frames, Curve curve, int keep = 5, double threshold = Rhino.RhinoMath.SqrtEpsilon)
        {
            List<Plane> result = new List<Plane>();
            List<double> curvatures = new List<double>();
            List<bool> remove = new List<bool>();
            List<bool> correct = new List<bool>();
            int n = frames.Count;

            // Calculate the curvature (vector lengths)
            for (int i = 0; i < n; i++)
            {
                curve.ClosestPoint(frames[i].Origin, out double t);
                Vector3d curvature = curve.CurvatureAt(t);
                curvatures.Add(curvature.Length);
            }

            // Check treshold values
            for (int i = 0; i < n; i++)
            {
                // Always keep first
                if (i < keep)
                {
                    remove.Add(false);
                }
                // Always keep last
                else if (i > n - keep - 1)
                {
                    remove.Add(false);
                }
                // Remove if it is within the threshold
                else if (curvatures[i] < threshold)
                {
                    remove.Add(true);
                }
                // Else keep
                else
                {
                    remove.Add(false);
                }
            }

            // Correct for looking forward and backward
            List<bool> check = remove.ConvertAll(item => item);
            
            for (int i = keep; i < n-keep-1; i++)
            {
                if (check[i] == false)
                {
                    for (int j = i - keep; j < i + keep + 2; j++)
                    {
                        remove[j] = false;
                    }
                }
            }

            // Remove
            for (int i = 0; i < n; i++)
            {
                if (remove[i] == false)
                {
                    result.Add(new Plane(frames[i]));
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the frames for a curve.
        /// </summary>
        /// <param name="curve"> The curve to get the frames from. </param>
        /// <param name="distance"> The maximum distance between two frames. </param>
        /// <param name="includeStart"> Indicates if the start frame is included. </param>
        /// <param name="includeEnd"> Indicates if the end frame is included. </param>
        /// <returns> The list with frames. </returns>
        public static List<Plane> GetFramesByDistance(Curve curve, double distance, bool includeStart = true, bool includeEnd = true)
        {
            List<Plane> result = new List<Plane>();

            int n = Math.Max((int)(curve.GetLength() / distance), 1);
            double[] t = curve.DivideByCount(n, true);

            for (int i = 0; i != t.Length; i++)
            {
                Point3d point = curve.PointAt(t[i]);
                Vector3d x = curve.TangentAt(t[i]);
                Vector3d y = Vector3d.CrossProduct(x, new Vector3d(0, 0, 1));
                Plane plane = new Plane(point, x, y);
                result.Add(plane);
            }

            result = ReduceFramesByCurvature(result, curve); // Optional? Parameter?

            if (includeStart == true && includeEnd == true)
            {
                return result;
            }
            else if (includeStart == false && includeEnd == false && result.Count == 2)
            {
                return new List<Plane>();
            }
            else if (includeStart == false && includeEnd == false)
            {
                return result.GetRange(1, result.Count - 2);
            }
            else if (includeStart == true && includeEnd == false)
            {
                return result.GetRange(0, result.Count - 2);
            }
            else
            {
                return result.GetRange(1, result.Count - 1);
            }
        }

        /// <summary>
        /// Returns the frames of a curve by dividing it in mulitple segments. 
        /// </summary>
        /// <param name="curve"> The curve to get the frames from. </param>
        /// <param name="distance"> The maximum distance between two frames. </param>
        /// <param name="includeStart"> Indicates if the start frame is included. </param>
        /// <param name="includeEnd"> Indicates if the end frame is included. </param>
        /// <returns> The list with frames. </returns>
        public static List<Plane> GetFramesByDistanceAndSegment(Curve curve, double distance, bool includeStart = true, bool includeEnd = true)
        {
            List<Plane> result = new List<Plane>();
            Curve[] segments = curve.DuplicateSegments();

            if (segments.Length <= 1)
            {
                result = GetFramesByDistance(curve, distance, true, true);
            }
            else
            {
                for (int i = 0; i < segments.Length; i++)
                {
                    List<Plane> subset;

                    // First segment
                    if (i == 0)
                    {
                        subset = GetFramesByDistance(segments[i], distance, true, true);
                        result.AddRange(subset);
                    }
                    // In between and last segment
                    else
                    {
                        subset = GetFramesByDistance(segments[i], distance, true, true);
                        result[result.Count - 1] = InterpolateFrames(result[result.Count - 1], subset[0]);
                        result.AddRange(subset.GetRange(1, subset.Count - 1));
                    }
                }
            }

            if (includeStart == true && includeEnd == true)
            {
                return result;
            }
            else if (includeStart == false && includeEnd == false && result.Count == 2)
            {
                return new List<Plane>();
            }
            else if (includeStart == false && includeEnd == false)
            {
                return result.GetRange(1, result.Count - 2);
            }
            else if (includeStart == true && includeEnd == false)
            {
                return result.GetRange(0, result.Count - 2);
            }
            else
            {
                return result.GetRange(1, result.Count - 1);
            }
        }

        /// <summary>
        /// Returns a frame that is an interpolation between two other framess.
        /// </summary>
        /// <param name="frame1"> The first frame. </param>
        /// <param name="frame2"> The second frame. </param>
        /// <returns> The interpolated frame. </returns>
        public static Plane InterpolateFrames(Plane frame1, Plane frame2)
        {
            Point3d origin = 0.5 * (frame1.Origin + frame2.Origin);
            Vector3d xAxis = 0.5 * (frame1.XAxis + frame2.XAxis);
            Vector3d yAxis = 0.5 * (frame1.YAxis + frame2.YAxis);

            return new Plane(origin, xAxis, yAxis); 
        }

        /// <summary>
        /// Returns the frames of a curve by curvature.
        /// </summary>
        /// <param name="curve"> The curve to get the frames from. </param>
        /// <param name="tolerance"> The tolerance for fitting. </param>
        /// <param name="includeStart"> Indicates if the start frame is included. </param>
        /// <param name="includeEnd"> Indicates if the end frame is included. </param>
        /// <returns> The list with frames. </returns>
        public static List<Plane> GetFramesByCurvature(Curve curve, double tolerance = 0.1, bool includeStart = true, bool includeEnd = true)
        {
            List<Plane> result = new List<Plane>();

            // Fits a curve through the existing: This method does that based on curvature
            NurbsCurve fit = curve.Fit(curve.Degree, tolerance, 0.0).ToNurbsCurve();
            NurbsCurvePointList controlPoints = fit.Points;

            // Start frames
            Point3d point = curve.PointAtStart;
            Vector3d x = curve.TangentAtStart;
            Vector3d y = Vector3d.CrossProduct(x, new Vector3d(0, 0, 1));
            Plane plane = new Plane(point, x, y);
            result.Add(plane);

            // Intermediate frames
            for (int i = 1; i != controlPoints.Count - 1; i++)
            {
                curve.ClosestPoint(controlPoints[i].Location, out double t);
                point = curve.PointAt(t);
                x = curve.TangentAt(t);
                y = Vector3d.CrossProduct(x, new Vector3d(0, 0, 1));
                plane = new Plane(point, x, y);
                result.Add(plane);
            }

            // End frame
            point = curve.PointAtEnd;
            x = curve.TangentAtEnd;
            y = Vector3d.CrossProduct(x, new Vector3d(0, 0, 1));
            plane = new Plane(point, x, y);
            result.Add(plane);

            // Sort frames along curve
            result = SortFramesAlongCurve(result, curve);

            if (includeStart == true && includeEnd == true)
            {
                return result;
            }
            else if (includeStart == false && includeEnd == false && result.Count == 2)
            {
                return new List<Plane>();
            }
            else if (includeStart == false && includeEnd == false)
            {
                return result.GetRange(1, result.Count - 2);
            }
            else if (includeStart == true && includeEnd == false)
            {
                return result.GetRange(0, result.Count - 2);
            }
            else
            {
                return result.GetRange(1, result.Count - 1);
            }
        }

        /// <summary>
        /// Returns a list with frames that are sorted along a curve.
        /// </summary>
        /// <param name="frames"> Unsorted list with frames. </param>
        /// <param name="curve"> Curve. </param>
        /// <returns> Sorted list with frames. </returns>
        public static List<Plane> SortFramesAlongCurve(IList<Plane> frames, Curve curve)
        {
            Plane[] items = frames.ToArray();
            double[] keys = new double[frames.Count];

            for (int i = 0; i < frames.Count; i++)
            {
                curve.ClosestPoint(frames[i].Origin, out double t);
                keys[i] = t;
            }

            Array.Sort(keys, items);

            return items.ToList();
        }
        #endregion
    }
}
