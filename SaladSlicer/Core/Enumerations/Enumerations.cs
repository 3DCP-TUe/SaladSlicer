// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

namespace SaladSlicer.Core.Enumerations
{
    /// <summary>
    /// Defines the transition type between two layers.
    /// </summary>
    public enum Transition : int
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
}
