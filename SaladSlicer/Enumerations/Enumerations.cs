// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

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
    public enum ProgramTypes : int
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
    public enum InterpolationTypes : int
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
}
