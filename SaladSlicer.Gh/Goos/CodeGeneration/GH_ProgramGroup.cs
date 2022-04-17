// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System.Collections.Generic;
// Grasshopper Libs
using Grasshopper.Kernel.Types;
// Salad Slicer Libs
using SaladSlicer.CodeGeneration;
using SaladSlicer.Interfaces;

namespace SaladSlicer.Gh.Goos.CodeGeneration
{
    /// <summary>
    /// Represents the GH_ProgramGroup class.
    /// </summary>
    public class GH_ProgramGroup : GH_Goo<ProgramGroup>
    {
        #region (de)serialisation
        //TODO
        #endregion

        #region constructors
        /// <summary>
        /// Initializes an empty instance of the GH_ProgramGroup class.
        /// </summary>
        public GH_ProgramGroup()
        {
            this.Value = null;
        }

        /// <summary>
        /// Initializes a new Program Group Goo instance from a Program Group instance.
        /// </summary>
        /// <param name="programGroup"> Program Group Value to store inside this Goo instance. </param>
        public GH_ProgramGroup(ProgramGroup programGroup)
        {
            this.Value = programGroup;
        }

        /// <summary>
        /// Returns a complete duplicate of this Goo instance.
        /// </summary>
        /// <returns> A duplicate of the Program Group Goo. </returns>
        public override IGH_Goo Duplicate()
        {
            if (this.Value == null)
            {
                return new GH_ProgramGroup();
            }
            else
            {
                return new GH_ProgramGroup(this.Value.Duplicate());
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
                return "Null Program Group";
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

            // Cast to Program Group
            if (typeof(Q).IsAssignableFrom(typeof(ProgramGroup)))
            {
                target = (Q)(object)this.Value;
                return true;
            }

            // Cast to Program Group Goo
            if (typeof(Q).IsAssignableFrom(typeof(GH_ProgramGroup)))
            {
                target = (Q)(object)new GH_ProgramGroup(this.Value);
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

            // Cast from Program Group
            if (typeof(ProgramGroup).IsAssignableFrom(source.GetType()))
            {
                this.Value = source as ProgramGroup;
                return true;
            }

            // Cast from Geometry Group Goo
            if (typeof(GH_GeometryGroup).IsAssignableFrom(source.GetType()))
            {
                GH_GeometryGroup groupGoo = source as GH_GeometryGroup;
                List<IGH_GeometricGoo> goos = groupGoo.Objects;
                List<IProgram> programObjects= new List<IProgram>() { };
                bool nonProgram = false;

                for (int i = 0; i < goos.Count; i++)
                {
                    if (goos[i] is GH_GeometricGoo<IProgram> geometricGoo)
                    {
                        programObjects.Add(geometricGoo.Value);
                    }
                    else if (goos[i] is GH_Goo<IProgram> goo)
                    {
                        programObjects.Add(goo.Value);
                    }
                    else
                    {
                        nonProgram= true;
                        break;
                    }
                }

                if (nonProgram == false)
                {
                    Value = new ProgramGroup(programObjects);
                    return true;
                }
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
                if (this.Value == null) { return "No internal Program Group instance"; }
                if (this.Value.IsValid) { return string.Empty; }
                return "Invalid Program Group instance: Did you define a Text?";
            }
        }

        /// <summary>
        /// Gets a description of the type of the implementation.
        /// </summary>
        public override string TypeDescription
        {
            get { return "Defines a Program Group."; }
        }

        /// <summary>
        /// Gets the name of the type of the implementation.
        /// </summary>
        public override string TypeName
        {
            get { return "Program Group"; }
        }
        #endregion
    }
}
