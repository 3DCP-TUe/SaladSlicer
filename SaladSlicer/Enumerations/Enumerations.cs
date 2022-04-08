// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

namespace SaladSlicer.Enumerations
{
    /// <summary>
    /// Defines the transition type between two layers.
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

    public enum InterpolationTypes : int
    {
        /// <summary>
        /// BSPLINE interpolation (G-Code)
        /// </summary>
        G642 = 0,

        /// <summary>
        /// Linear interpolation
        /// </summary>
        G1 = 1,
    }

    public enum AddVariableMethod : int
    {
        /// <summary>
        /// Generates the variable value as a factor of the global displacement
        /// </summary>
        ByDisplacement = 0,

        /// <summary>
        /// Generates the variable value based as a factor of the distance the previous layer
        /// </summary>
        ByLayerdistance = 1,
    }
}
