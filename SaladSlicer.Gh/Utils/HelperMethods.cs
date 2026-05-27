// SPDX-License-Identifier: GPL-3.0-or-later
// Salad Slicer
// Project: https://github.com/3DCP-TUe/SaladSlicer
//
// Copyright (c) 2021-2026 Eindhoven University of Technology
//
// Authors:
//  - Derk Bos (2021)
//  - Arjen Deetman (2021-2026)
// 
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
using System.Drawing;
using System.Text.RegularExpressions;
// Grasshopper Libs
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;

namespace SaladSlicer.Gh.Utils
{
    /// <summary>
    /// Represents general helper methods
    /// </summary>
    internal static class HelperMethods
    {
        /// <summary>
        /// Creates a Grasshopper value list and returns true if it's created
        /// </summary>
        /// <param name="component">Component to connect to</param>
        /// <param name="inputIndex">Index of the input to connect the list to</param>
        /// <param name="enumType">Enumeration to take values from</param>
        /// <param name="addSpaces">If true, inserts spaces between words in enum names (e.g., "ByLayer" becomes "By Layer")</param>
        /// <returns>Returns true if created.</returns>
        public static bool CreateValueList(GH_Component component, int inputIndex, Type enumType, bool addSpaces = false)
        {
            if (component.Params.Input[inputIndex].SourceCount == 0)
            {
                var parameter = component.Params.Input[inputIndex];

                // Creates the empty value list
                GH_ValueList obj = new GH_ValueList();
                obj.CreateAttributes();
                obj.ListMode = GH_ValueListMode.DropDown;
                obj.ListItems.Clear();

                // Add the items to the value list
                string[] names = Enum.GetNames(enumType);
                int[] values = (int[])Enum.GetValues(enumType);


                for (int i = 0; i < names.Length; i++)
                {
                    if (addSpaces)
                    {
                        obj.ListItems.Add(new GH_ValueListItem(Regex.Replace(names[i], "(?<=[a-z])([A-Z])", " $1"), values[i].ToString()));
                    }
                    else
                    {
                        obj.ListItems.Add(new GH_ValueListItem(names[i], values[i].ToString()));
                    }
                }

                // Make point where the valuelist should be created on the canvas
                if (parameter.Attributes.Pivot.X < 1 && parameter.Attributes.Pivot.Y < 1)
                {
                    obj.Attributes.Pivot = new PointF(component.Attributes.Pivot.X + parameter.Attributes.InputGrip.X - 120, component.Attributes.Pivot.Y + parameter.Attributes.InputGrip.Y - 11);
                }
                else
                {
                    obj.Attributes.Pivot = new PointF(parameter.Attributes.InputGrip.X - 120, parameter.Attributes.InputGrip.Y - 11);
                }

                // Add the value list to the active canvas
                Instances.ActiveCanvas.Document.AddObject(obj, false);

                // Connect the value list to the input parameter
                parameter.AddSource(obj);

                // Collect data
                parameter.CollectData();

                // Expire value list
                obj.ExpireSolution(true);

                // Return that it's created
                return true;
            }
            else
            {
                // Return that it isn't created
                return false;
            }
        }
    }
}

