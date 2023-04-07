// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU Lesser General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.ComponentModel;
// Grasshopper Libs
using Grasshopper.Kernel;
// Rhino Libs
using Rhino.Geometry;

namespace SaladSlicer.Gh.Components.Nozzles
{
    /// <summary>
    /// Represents the round nozzle component.
    /// </summary>
    public class RoundNozzleComponent : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public RoundNozzleComponent()
          : base("Round Nozzle", // Component name
              "RouNoz", // Component nickname
              "Creates a round nozzle.", // Description
              "Salad Slicer", // Category
              "Nozzles") // Subcategory
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Length 1", "L1", "The length of the connector as a Number.", GH_ParamAccess.item, 24.5);
            pManager.AddNumberParameter("Length 2", "L2", "The length between the connector and the unreduced circle as a Number.", GH_ParamAccess.item, 100.0);
            pManager.AddNumberParameter("Length 3", "L3", "The length between the unreduced and reduced circle as a Number.", GH_ParamAccess.item, 25.0);
            pManager.AddNumberParameter("Outer Diameter", "OD", "The outer diameter of the connected pipe as a Number.", GH_ParamAccess.item, 41.0);
            pManager.AddNumberParameter("Inner Diameter", "ID", "The innner diameter of the connected pipe as a Number.", GH_ParamAccess.item, 35.0);
            pManager.AddNumberParameter("Nozzle Diameter", "ND", "The diameter of the nozzle as a Number.", GH_ParamAccess.item, 30.0);
            pManager.AddNumberParameter("Wall Thickness", "WT", "The minimum wall thickness as a Number.", GH_ParamAccess.item, 4.0);
            pManager.AddNumberParameter("Gap", "G", "The size of the connector gap as a Number.", GH_ParamAccess.item, 1.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "The round nozzle as a Brep.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            double length1 = 25.0;
            double length2 = 100.0;
            double length3 = 25.0;
            double outerDiameter = 41.0;
            double innerDiameter = 35.0;
            double nozzleDiameter = 30.0;
            double wallThickness = 4.0;
            double gap = 1.0;

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref length1)) return;
            if (!DA.GetData(1, ref length2)) return;
            if (!DA.GetData(2, ref length3)) return;
            if (!DA.GetData(3, ref outerDiameter)) return;
            if (!DA.GetData(4, ref innerDiameter)) return;
            if (!DA.GetData(5, ref nozzleDiameter)) return;
            if (!DA.GetData(6, ref wallThickness)) return;
            if (!DA.GetData(7, ref gap)) return;

            // Declare the output variables
            Brep result = new Brep();

            // Create the new curves
            try
            {
                // Set tolerance from document
                SaladSlicer.Nozzles.Nozzles.Tolerance = DocumentTolerance();

                // Result
                result = SaladSlicer.Nozzles.Nozzles.Round(length1, length2, length3, outerDiameter, innerDiameter, nozzleDiameter, wallThickness, gap);
            }
            catch (WarningException w)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, w.Message);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
            }

            // Assign the output parameters
            DA.SetData(0, result);
        }

        /// <summary>
        /// Gets the exposure of this object in the Graphical User Interface.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the 24x24 pixels icon.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.RoundNozzle_Icon; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7DB81A7B-00D6-4406-B804-CEA53125397F"); }
        }
    }
}