// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System.Linq;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Geometry
{
    /// <summary>
    /// Represents a static class that contains a number of methods to manipulate, create and obtain data from surfaces.
    /// </summary>
    public static class Surfaces
    {
        /// <summary>
        /// Returns a list with curves mapped on a surface. 
        /// </summary>
        /// <param name="surface"> The surface to map the contour on. </param>
        /// <param name="count"> The amount of curves. </param>
        /// <param name="samples"> The amount of samples along the curve. </param>
        /// <param name="transpose"> Indicates whether or not the surface should be tranposed. </param>
        /// <returns> List with mapped curves. </returns>
        public static List<Curve> ContourMapping(Surface surface, int count, int samples, bool transpose = false)
        {
            List<Curve> result = new List<Curve>() { };
            Surface surfaceCopy = (Surface)surface.Duplicate();

            if (transpose == true)
            {
                surfaceCopy = surfaceCopy.Transpose();
            }

            if (count < 2)
            {
                count = 2;
            }

            if (samples < 2)
            {
                samples = 2;
            }

            double min1 = surfaceCopy.Domain(0).Min;
            double max1 = surfaceCopy.Domain(0).Max;
            double min2 = surfaceCopy.Domain(1).Min;
            double max2 = surfaceCopy.Domain(1).Max;

            double[] range1 = Enumerable.Range(0, count).Select(i => min1 + (max1 - min1) * ((double)i / (count - 1))).ToArray();
            double[] range2 = Enumerable.Range(0, samples).Select(i => min2 + (max2 - min2) * ((double)i / (samples - 1))).ToArray();

            for (int i = 0; i < range1.Length; i++)
            {
                Point3d[] points = new Point3d[range2.Length];

                for (int j = 0; j < range2.Length; j++)
                {
                    surfaceCopy.PointAt(range1[i], range2[j]);
                    points[j] = surfaceCopy.PointAt(range1[i], range2[j]);
                }

                result.Add(Curve.CreateInterpolatedCurve(points, 3, CurveKnotStyle.Chord));
            }

            return result;
        }
    }
}
