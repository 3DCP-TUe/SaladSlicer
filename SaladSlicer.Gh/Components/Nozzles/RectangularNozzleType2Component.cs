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
    /// Represents the reactangular nozzle component.
    /// </summary>
    public class RectangularNozzleType2Component : GH_Component
    {
        #region fields
        #endregion

        /// <summary>
        /// Public constructor without any arguments.
        /// </summary>
        public RectangularNozzleType2Component()
          : base("Rectangular Nozzle Type 2", // Component name
              "RecNozTy2", // Component nickname
              "Creates a rectanglur nozzle.", // Description
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
            pManager.AddNumberParameter("Length 2", "L2", "The length between the connector and the unreduced rectangle as a Number.", GH_ParamAccess.item, 100.0);
            pManager.AddNumberParameter("Length 3", "L3", "The length between the unreduced and reduced rectangle as a Number.", GH_ParamAccess.item, 25.0);
            pManager.AddNumberParameter("Length 4", "L4", "The length of the final reduced cross section.", GH_ParamAccess.item, 25.0);
            pManager.AddNumberParameter("Outer Diameter", "OD", "The outer diameter of the connected pipe as a Number.", GH_ParamAccess.item, 30.0);
            pManager.AddNumberParameter("Inner Diameter", "ID", "The innner diameter of the connected pipe as a Number.", GH_ParamAccess.item, 25.0);
            pManager.AddNumberParameter("Nozzle Width", "NW", "The width of the nozzle as a Number.", GH_ParamAccess.item, 50.0);
            pManager.AddNumberParameter("Nozzle Height", "NH", "The height of the nozzle as a Number.", GH_ParamAccess.item, 10.0);
            pManager.AddNumberParameter("Wall Thickness", "WT", "The minimum wall thickness as a Number.", GH_ParamAccess.item, 4.0);
            pManager.AddNumberParameter("Gap", "G", "The size of the connector gap as a Number.", GH_ParamAccess.item, 1.0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Brep", "B", "The rectangular nozzle as a Brep.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare variable of input parameters
            double length1 = 24.5;
            double length2 = 100.0;
            double length3 = 25.0;
            double length4 = 25.0;
            double outerDiameter = 30.0;
            double innerDiameter = 25.0;
            double nozzleWidth = 50.0;
            double nozzleHeight = 10.0;
            double wallThickness = 4.0;
            double gap = 1.0;

            // Access the input parameters individually. 
            if (!DA.GetData(0, ref length1)) return;
            if (!DA.GetData(1, ref length2)) return;
            if (!DA.GetData(2, ref length3)) return;
            if (!DA.GetData(3, ref length4)) return;
            if (!DA.GetData(4, ref outerDiameter)) return;
            if (!DA.GetData(5, ref innerDiameter)) return;
            if (!DA.GetData(6, ref nozzleWidth)) return;
            if (!DA.GetData(7, ref nozzleHeight)) return;
            if (!DA.GetData(8, ref wallThickness)) return;
            if (!DA.GetData(9, ref gap)) return;

            // Declare the output variables
            Brep result = new Brep();

            // Create the new curves
            try
            {
                // Set tolerance from document
                SaladSlicer.Nozzles.Nozzles.Tolerance = DocumentTolerance();

                // Result
                result = SaladSlicer.Nozzles.Nozzles.RectangularType2(length1, length2, length3, length4, outerDiameter, innerDiameter, nozzleWidth, nozzleHeight, wallThickness, gap);
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
            get { return null; }
        }

        /// <summary>
        /// Gets the ID for this object type. 
        /// Every object must supply a unique and unchanging ID that is used to identify objects of the same type.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6C9FCC1A-1201-4471-AA12-F3C5BD6F094D"); }
        }
    }
}