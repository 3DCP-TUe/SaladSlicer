// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System.Linq;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Core.Geometry
{
    /// <summary>
    /// Represents a static class that contains a number of methods to manipulate and create meshes.
    /// </summary>
    public static class Meshes
    {
        #region methods
        /// <summary>
        /// Returns a list with contours by slicing a mesh based on a layer distance.
        /// </summary>
        /// <param name="mesh"> The mesh. </param>
        /// <param name="distance"> The layer distance. </param>
        /// <returns> The list with planar contours. </returns>
        public static List<List<Curve>> GetPlanarContoursByDistance(Mesh mesh, double distance) 
        {
            List<List<Curve>> result = new List<List<Curve>>();

            Plane plane = Plane.WorldXY; // TODO: parameter?

            // Get the bounding box and the start plane
            BoundingBox box = mesh.GetBoundingBox(true);
            Point3d[] corners = box.GetCorners();
            double min = corners[0].Z;

            for (int i = 1; i < corners.Length; i++)
            {
                if (corners[i].Z < min)
                {
                    min = corners[i].Z;
                }
            }

            plane.OriginZ = min;

            // Slice
            bool stop = false;
            int counter = 0;
                
            while (stop == false)
            {
                plane.OriginZ = min + counter * distance;
                Curve[] curves = Mesh.CreateContourCurves(mesh, plane);

                if (curves.Length != 0)
                {
                    result.Add(curves.ToList());
                    counter++;
                }
                else
                {
                    break;
                }
            }

            return result;
        }
        
        /// <summary>
        /// Returns a list with contours by slicing a mesh based on given layer heights.
        /// </summary>
        /// <param name="mesh"> The mesh. </param>
        /// <param name="heights"> The list with layer heights. </param>
        /// <returns> The list with planar contours. </returns>
        public static List<List<Curve>> GetPlanarContoursByHeights(Mesh mesh, List<double> heights)
        {
            List<List<Curve>> result = new List<List<Curve>>();

            Plane plane = Plane.WorldXY; // TODO: parameter?

            // Get the bounding box and the start plane
            BoundingBox box = mesh.GetBoundingBox(true);
            Point3d[] corners = box.GetCorners();
            double min = corners[0].Z;

            for (int i = 1; i < corners.Length; i++)
            {
                if (corners[i].Z < min)
                {
                    min = corners[i].Z;
                }
            }

            plane.OriginZ = min;

            // Slice
            for (int i = 0; i < heights.Count; i++)
            {
                plane.OriginZ = min + heights[i];
                Curve[] curves = Mesh.CreateContourCurves(mesh, plane);
                curves = Curve.JoinCurves(curves, 1.0);

                if (curves.Length != 0)
                {
                    result.Add(curves.ToList());
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a list with contours by slicing a mesh along a guiding curve. 
        /// </summary>
        /// <param name="mesh"> The mesh. </param>
        /// <param name="guide"> The guiding curve. </param>
        /// <param name="count"> The number of contours. </param>
        /// <returns> The list with contours. </returns>
        public static List<List<Curve>> GetNonPlanarContoursByGuide(Mesh mesh, Curve guide, int count)
        {
            List<List<Curve>> result = new List<List<Curve>>();

            double[] parameters = guide.DivideByCount(count, true);
            Plane[] planes = guide.GetPerpendicularFrames(parameters);

            for (int i = 0; i < planes.Length; i++)
            {
                Curve[] curves = Mesh.CreateContourCurves(mesh, planes[i]);
                
                if (curves.Length != 0)
                {
                    result.Add(curves.ToList());
                }
            }

            return result;
        }
        #endregion
    }
}
