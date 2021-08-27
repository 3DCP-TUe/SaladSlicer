// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// Grasshopper Libs
using Grasshopper.Kernel.Types;
// Salad Slicer Libs
using SaladSlicer.Core.CodeGeneration;
using SaladSlicer.Core.Interfaces;

namespace SaladSlicer.Gh.Goos.CodeGeneration
{
    /// <summary>
    /// Represents the GH_FeedRate class.
    /// </summary>
    public class GH_FeedRate : GH_Goo<FeedRate>
    {
        #region (de)serialisation
        //TODO
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the GH_FeedRate class.
        /// </summary>
        public GH_FeedRate()
        {
            this.Value = null;
        }

        /// <summary>
        /// Initializes a new Feed Rate Goo instance from a Feed Rate instance.
        /// </summary>
        /// <param name="feedRate"> Feed Rate Value to store inside this Goo instance. </param>
        public GH_FeedRate(FeedRate feedRate)
        {
            this.Value = feedRate;
        }

        /// <summary>
        /// Returns a complete duplicate of this Goo instance.
        /// </summary>
        /// <returns> A duplicate of the Feed Rate Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            if (this.Value == null)
            {
                return new GH_FeedRate();
            }
            else
            {
                return new GH_FeedRate(this.Value.Duplicate());
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Creates a string description of the current instance value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Value == null)
            {
                return "Null Feed Rate";
            }
            else
            {
                return this.Value.ToString();
            }
        }

        /// <summary>
        /// Attempt a cast to type Q.
        /// </summary>
        /// <typeparam name="Q"> Type to cast to. </typeparam>
        /// <param name="target"> Pointer to target of cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastTo<Q>(ref Q target)
        {
            // Check null type
            if (this.Value == null) 
            { 
                target = default;
                return false;
            }

            // Cast to Feed Rate
            if (typeof(Q).IsAssignableFrom(typeof(FeedRate)))
            {
                target = (Q)(object)this.Value;
                return true;
            }

            // Cast to Feed Rate Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_FeedRate)))
            {
                target = (Q)(object)new GH_FeedRate(this.Value);
                return true;
            }

            // Cast to IProgram
            if (typeof(Q).IsAssignableFrom(typeof(IProgram)))
            {
                target = (Q)(object)this.Value;
                return true;
            }

            // Cast to IProgram Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ProgramObject)))
            {
                target = (Q)(object)new GH_ProgramObject(this.Value);
                return true;
            }

            // Cast to double
            if (typeof(Q).IsAssignableFrom(typeof(double)))
            {
                target = (Q)(object)this.Value.Feedrate;
                return true;
            }

            // Cast to Number Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_Number)))
            {
                target = (Q)(object)new GH_Number(this.Value.Feedrate);
                return true;
            }

            // Invalid cast
            target = default;
            return false;
        }

        /// <summary>
        /// Attempt a cast from generic object.
        /// </summary>
        /// <param name="source"> Reference to data that should be cast. </param>
        /// <returns> True on success, false on failure. </returns>
        public override bool CastFrom(object source)
        {
            // Check null type
            if (source == null) 
            {
                return false; 
            }

            // Cast from Feed Rate
            if (typeof(FeedRate).IsAssignableFrom(source.GetType()))
            {
                this.Value = source as FeedRate;
                return true;
            }

            // Cast from double
            if (typeof(double).IsAssignableFrom(source.GetType()))
            {
                this.Value = new FeedRate((double)source);
                return true;
            }

            // Cast from Number Goo
            if (typeof(GH_Number).IsAssignableFrom(source.GetType()))
            {
                GH_Number goo = source as GH_Number;
                this.Value = new FeedRate(goo.Value);
                return true;
            }

            // Invalid cast
            return false;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not the current value is valid.
        /// </summary>
        public override bool IsValid
        {
            get
            {
                if (this.Value == null) { return false; }
                return this.Value.IsValid;
            }
        }

        /// <summary>
        /// Gets a string describing the state of "invalidness". 
        /// If the instance is valid, then this property should return String.Empty.
        /// </summary>
        public override string IsValidWhyNot
        {
            get
            {
                if (this.Value == null) { return "No internal Feed Rate instance"; }
                if (this.Value.IsValid) { return string.Empty; }
                return "Invalid Feed Rate instance: Did you define a Text?";
            }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Feed Rate."; }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Feed Rate"; }
        }
        #endregion
    }
}
