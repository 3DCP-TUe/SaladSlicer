// SPDX-License-Identifier: GPL-3.0-or-later
// Salad Slicer
// Project: https://github.com/3DCP-TUe/SaladSlicer
//
// Copyright (c) 2021-2026 Eindhoven University of Technology
//
// Authors:
//  - Arjen Deetman (2021-2026)
//  - Derk Bos (2021)
// 
// For license details, see the LICENSE file in the project root.

namespace SaladSlicer.Enumerations
{
    /// <summary>
    /// Defines the transition type between two open layers.
    /// </summary>
    public enum OpenTransition : int
    {
        /// <summary>
        /// Linear transitions
        /// </summary>
        Linear = 0,

        /// <summary>
        /// Bezier transition
        /// </summary>
        Bezier = 1,
    }

    /// <summary>
    /// Defines the transition type between two closed layers.
    /// </summary>
    public enum ClosedTransition : int
    {
        /// <summary>
        /// Linear transitions
        /// </summary>
        Linear = 0,

        /// <summary>
        /// Bezier transition
        /// </summary>
        Bezier = 1,

        /// <summary>
        /// Bezier transition
        /// </summary>
        Interpolated = 2,
    }

    /// <summary>
    /// Defines the program types.
    /// </summary>
    public enum ProgramType : int
    {
        /// <summary>
        /// Generates a Sinumerik NC program (G-Code)
        /// </summary>
        Sinumerik = 0,

        /// <summary>
        /// Generates a Marlin G-Code
        /// </summary>
        Marlin = 1,
    }

    /// <summary>
    /// Defines the interpolation types.
    /// </summary>
    public enum InterpolationType : int
    {
        /// <summary>
        /// Spline interpolation (BSPLINE G642)
        /// </summary>
        Spline = 0,

        /// <summary>
        /// Linear interpolation (G1)
        /// </summary>
        Linear = 1,
    }

    /// <summary>
    /// Defines the path type
    /// </summary>
    public enum PathType : int
    {
        /// <summary>
        /// Original path
        /// </summary>
        Original = 0,

        /// <summary>
        /// Spline interpolated path
        /// </summary>
        Spline = 1,

        /// <summary>
        /// Linear interpolated path
        /// </summary>
        Linear = 2,
    }

    /// <summary>
    /// Defines the output structure
    /// </summary>
    /// <remarks>
    /// Typically used to set the structure of the datatrees in Grasshopper components.
    /// </remarks>
    public enum OutputStructure : int
    {
        /// <summary>
        /// By layer
        /// </summary>
        ByLayer = 0,

        /// <summary>
        /// Spline interpolated path
        /// </summary>
        ByOject = 1
    }
}
