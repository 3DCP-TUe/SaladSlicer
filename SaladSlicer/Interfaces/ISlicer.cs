﻿// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
// Salad Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Enumerations;

namespace SaladSlicer.Interfaces
{
    /// <summary>
    /// Represents the interface for all classes that can generate code lines for a program.
    /// </summary>
    public interface ISlicer
    {
        #region constructors
        /// <summary>
        /// Returns an exact duplicate of this ISlicer
        /// </summary>
        /// <returns> The exact duplicate of this ISlicer. </returns>
        ISlicer DuplicateSlicerObject();
        #endregion

        #region methods
        /// <summary>
        /// Adds the NC program lines generated by this object to the program.
        /// </summary>
        /// <param name="programGenerator"> The program generator. </param>
        void ToProgram(ProgramGenerator programGenerator);

        /// <summary>
        /// Returns the original path.
        /// </summary>
        /// <returns> The path. </returns>
        Curve GetPath();

        /// <summary>
        /// Returns the path.
        /// </summary>
        /// <param name="type"> The path type. </param>
        /// <returns> The path. </returns>
        Curve GetPath(PathType type);

        /// <summary>
        /// Returns the interpolated path.
        /// </summary>
        /// <returns> The interpolated path. </returns>
        Curve GetInterpolatedPath();

        /// <summary>
        /// Returns the linearized path.
        /// </summary>
        /// <returns> The linearized path. </returns>
        Curve GetLinearizedPath();

        /// <summary>
        /// Returns distance of every frame along the path.
        /// </summary>
        /// <param name="type"> The path type. </param>
        /// <returns> List with distances. </returns>
        List<double> GetDistancesAlongPath(PathType type);

        /// <summary>
        /// Returns distance of every frame along the curve.
        /// </summary>
        /// <returns> List with distances. </returns>
        List<List<double>> GetDistancesAlongContours();

        /// <summary>
        /// Returns distance of every frame to the previous layer
        /// </summary>
        /// <param name="plane"> Plane to calculate the closest distance to for the first layer. </param>
        /// <param name="dx"> Distance in x-direction. </param>
        /// <param name="dy"> Distance in y-direction.</param>
        /// <param name="dz"> Distance in z-direction.</param>
        /// <returns> List with distaces. </returns>
        List<List<double>> GetDistanceToPreviousLayer(Plane plane, out List<List<double>> dx, out List<List<double>> dy, out List<List<double>> dz);

        /// Returns a list with curvatures of the path at the frame location.
        /// </summary>
        /// <returns> The list with curvatures. </returns>
        List<List<Vector3d>> GetCurvatures();
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the object is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets the contours.
        /// </summary>
        List<Curve> Contours { get; }

        /// <summary>
        /// Gets the frames of the object.
        /// </summary>
        List<Plane> Frames { get; }

        /// <summary>
        /// Gets the frames of the object by layer.
        /// </summary>
        List<List<Plane>> FramesByLayer { get; }

        /// <summary>
        /// Gets frame at the start of the path.
        /// </summary>
        Plane FrameAtStart { get; }

        /// <summary>
        /// Gets frame at the end of the path.
        /// </summary>
        Plane FrameAtEnd { get; }

        /// <summary>
        /// Gets point at the start of the path.
        /// </summary>
        Point3d PointAtStart { get; }

        /// <summary>
        /// Gets point at the end of the path.
        /// </summary>
        Point3d PointAtEnd { get; }
        #endregion
    }
}
