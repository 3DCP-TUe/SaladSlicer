// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published 
// by the Free Software Foundation. For more information and the 
// LICENSE file, see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Linq;
using System.Collections.Generic;
// Rhino Libs
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
// SaladSlicer
using SaladSlicer.Geometry;
using SaladSlicer.Geometry.Seams;

namespace SaladSlicer.Nozzles
{
    /// <summary>
    /// Represent methods to create nozzles. 
    /// </summary>
    public static class Nozzles
    {
        #region fields
        private static double _tolerance = 0.001;
        #endregion

        #region methods
        /// <summary>
        /// Return a round nozzle. 
        /// </summary>
        /// <param name="length1"> The length of the connector. </param>
        /// <param name="length2"> The length between the connector and the unreduced circle. </param>
        /// <param name="length3"> The length between the unreduced and reduced circle. </param>
        /// <param name="outerDiameter"> The outer diameter of the connected pipe. </param>
        /// <param name="innerDiameter"> The innner diameter of the connected pipe. </param>
        /// <param name="nozzleDiameter"> The diameter of the nozzle. </param>
        /// <param name="wallThickness"> The minimum wall thickness. </param>
        /// <param name="gap"> The size of the connector gap. </param>
        /// <returns> The round nozzle. </returns>
        public static Brep Round(double length1, double length2, double length3, double outerDiameter, double innerDiameter, double nozzleDiameter, double wallThickness, double gap)
        {
            if (length1 <= 0 | length2 <= 0 | length3 <= 0)
            {
                throw new Exception("The defined lengths must be larger than 0.");
            }

            length1 = Math.Max(0, length1);
            length2 = Math.Max(0, length2);
            length3 = Math.Max(0, length3);

            Plane plane1 = new Plane(new Point3d(0, 0, 0), Vector3d.XAxis, Vector3d.YAxis);
            Plane plane2 = new Plane(new Point3d(0, 0, length1), Vector3d.XAxis, Vector3d.YAxis);
            Plane plane3 = new Plane(new Point3d(0, 0, length1 + length2), Vector3d.XAxis, Vector3d.YAxis);
            Plane plane4 = new Plane(new Point3d(0, 0, length1 + length2 + length3), Vector3d.XAxis, Vector3d.YAxis);

            List<Brep> breps = new List<Brep>() { };

            #region outer shape
            List<Curve> outerCurves = new List<Curve>() { };

            outerCurves.Add(new Circle(plane1, outerDiameter / 2 + wallThickness).ToNurbsCurve());
            outerCurves.Add(new Circle(plane2, outerDiameter / 2 + wallThickness).ToNurbsCurve());
            outerCurves.Add(new Circle(plane3, nozzleDiameter / 2 + wallThickness).ToNurbsCurve());
            outerCurves.Add(new Circle(plane4, nozzleDiameter / 2 + wallThickness).ToNurbsCurve());

            for (int i = 1; i < outerCurves.Count; i++)
            {
                breps.Add(Loft(outerCurves[i - 1], outerCurves[i]));
            }
            #endregion

            #region inner shape
            List<Curve> innerCurves = new List<Curve>() { };
            innerCurves.Add(new Circle(plane1, outerDiameter / 2).ToNurbsCurve());
            innerCurves.Add(new Circle(plane2, outerDiameter / 2).ToNurbsCurve());
            innerCurves.Add(new Circle(plane2, innerDiameter / 2).ToNurbsCurve());
            innerCurves.Add(new Circle(plane3, nozzleDiameter / 2).ToNurbsCurve());
            innerCurves.Add(new Circle(plane4, nozzleDiameter / 2).ToNurbsCurve());

            for (int i = 1; i < innerCurves.Count; i++)
            {
                breps.Add(Loft(innerCurves[i - 1], innerCurves[i]));
            }
            #endregion

            #region caps
            breps.Add(Brep.CreatePlanarBreps(new List<Curve>() { outerCurves[0], innerCurves[0] }, _tolerance)[0]);
            breps.Add(Brep.CreatePlanarBreps(new List<Curve>() { outerCurves.Last(), innerCurves.Last() }, _tolerance)[0]);
            #endregion

            Brep result = Brep.JoinBreps(breps, _tolerance)[0];

            #region gap
            if (gap > 0)
            {
                Brep box = new BoundingBox(-gap / 2, -outerDiameter / 2 - wallThickness - 5, -5, gap / 2, outerDiameter / 2 + wallThickness + 5, length1).ToBrep();
                result = Brep.CreateBooleanDifference(result, box, _tolerance)[0];
            }
            #endregion

            return result;
        }

        /// <summary>
        /// Returns a rectanglur nozzle surrounded by a bouding box with fillet edges at the bottom. 
        /// </summary>
        /// <param name="boxWidth"> The width of the bouding box. </param>
        /// <param name="boxDepth"> The depth of the bounding box. </param>
        /// <param name="filletRadius"> The fillet radius of the bouding box. </param>
        /// <param name="length1"> The length of the connector. </param>
        /// <param name="length2"> The length between the connector and the unreduced rectangle. </param>
        /// <param name="length3"> The length between the unreduced and reduced rectangle. </param>
        /// <param name="length4"> The length of the final reduced cross section. </param>
        /// <param name="outerDiameter"> The outer diameter of the connected pipe. </param>
        /// <param name="innerDiameter"> The innner diameter of the connected pipe. </param>
        /// <param name="nozzleWidth"> The width of the nozzle. </param>
        /// <param name="nozzleHeight"> The height of the nozzle. </param>
        /// <param name="divide"> Indicates whether or not the nozzle needs to be divide. </param>
        /// <returns> The rectangular nozzle. </returns>
        public static Brep RectangularType1(double boxWidth, double boxDepth, double filletRadius, double length1, double length2, double length3, double length4, double outerDiameter, double innerDiameter, double nozzleWidth, double nozzleHeight, bool divide)
        {
            if (length1 <= 0 | length2 <= 0 | length4 <= 0)
            {
                throw new Exception("The defined lengths must be larger than 0.");
            }

            length1 = Math.Max(0, length1);
            length2 = Math.Max(0, length2);
            length3 = Math.Max(0, length3);
            length4 = Math.Max(0, length4);

            Plane plane1 = new Plane(new Point3d(0, 0, 0), Vector3d.XAxis, Vector3d.YAxis);
            Plane plane2 = new Plane(new Point3d(0, 0, length1), Vector3d.XAxis, Vector3d.YAxis);
            Plane plane3 = new Plane(new Point3d(0, 0, length1 + length2), Vector3d.XAxis, Vector3d.YAxis);
            Plane plane4 = new Plane(new Point3d(0, 0, length1 + length2 + length3), Vector3d.XAxis, Vector3d.YAxis);
            Plane plane5 = new Plane(new Point3d(0, 0, length1 + length2 + length3 + length4), Vector3d.XAxis, Vector3d.YAxis);

            double innerArea = 0.25 * innerDiameter*innerDiameter * Math.PI;

            #region outer shape
            // Contour curves
            Curve outerCurve1 = Rectangle3dCenter(plane1, boxWidth, boxDepth).ToNurbsCurve();
            Curve outerCurve2 = Rectangle3dCenter(plane5, boxWidth, boxDepth).ToNurbsCurve();

            // Generate brep
            List<Brep> brepOuter = new List<Brep>() { };
            brepOuter.Add(Brep.CreatePlanarBreps(new List<Curve>() { outerCurve1 }, _tolerance)[0]);
            brepOuter.Add(Loft(outerCurve1, outerCurve2));
            brepOuter.Add(Brep.CreatePlanarBreps(new List<Curve>() { outerCurve2 }, _tolerance)[0]);
            Brep outerShape = Brep.JoinBreps(brepOuter, _tolerance)[0];
            
            // Fillet edges
            if (filletRadius > 0.0)
            {
                // Find edge 1
                Point3d point1 = new Point3d(0, boxDepth / 2, plane5.Origin.Z);
                int index1 = outerShape.Edges.ToList().FindIndex(item => item.PointAt(item.Domain.Mid) == point1);

                // Find edge 2
                Point3d point2 = new Point3d(0, -boxDepth / 2, plane5.Origin.Z);
                int index2 = outerShape.Edges.ToList().FindIndex(item => item.PointAt(item.Domain.Mid) == point2);

                List<double> radii = new List<double>() { filletRadius, filletRadius };
                List<int> edges = new List<int>() { index1, index2 };

                outerShape = Brep.CreateFilletEdges(outerShape, edges, radii, radii, BlendType.Fillet, RailType.RollingBall, _tolerance)[0];
            }
            #endregion

            #region inner shape
            // Contour curves
            List<Curve> innerCurves = new List<Curve>() { };
            innerCurves.Add(new Circle(plane1, outerDiameter / 2).ToNurbsCurve()); // Connector
            innerCurves[0].Rotate(Rhino.RhinoMath.ToRadians(225), Vector3d.ZAxis, new Point3d(0, 0, 0));
            innerCurves.Add(new Circle(plane2, outerDiameter / 2).ToNurbsCurve()); // Connector
            innerCurves[1].Rotate(Rhino.RhinoMath.ToRadians(225), Vector3d.ZAxis, new Point3d(0, 0, 0));
            
            Curve curve1 = new Circle(plane2, innerDiameter / 2).ToNurbsCurve(); // Hose/pipe diameter
            curve1.Rotate(Rhino.RhinoMath.ToRadians(225), Vector3d.ZAxis, new Point3d(0, 0, 0));

            if (length3 > 0)
            {
                Curve curve2 = Rectangle3dCenter(plane3, nozzleWidth, innerArea / nozzleWidth).ToNurbsCurve(); // Rectangle: equal area
                LoftWithLinearCrossSection(curve1, curve2, 50, out List<Curve> scaledCurves);
                innerCurves.AddRange(scaledCurves);

                innerCurves.Add(Rectangle3dCenter(plane4, nozzleWidth, nozzleHeight).ToNurbsCurve()); // Rectangle: reduced equal area
            }
            else
            {
                Curve curve2 = Rectangle3dCenter(plane3, nozzleWidth, nozzleHeight).ToNurbsCurve(); // Rectangle: reduced area
                LoftWithLinearCrossSection(curve1, curve2, 50, out List<Curve> scaledCurves);
                innerCurves.AddRange(scaledCurves);
            }

            innerCurves.Add(Rectangle3dCenter(plane5, nozzleWidth, nozzleHeight).ToNurbsCurve()); // Rectangle: reduced equal are
            // Generate brep
            List<Brep> brepInner = new List<Brep>() { };
            brepInner.AddRange(Brep.CreatePlanarBreps(new List<Curve>() { innerCurves[0] }, _tolerance));

            for (int i = 1; i < innerCurves.Count; i++)
            {
                brepInner.Add(Loft(innerCurves[i - 1], innerCurves[i]));
            }

            brepInner.AddRange(Brep.CreatePlanarBreps(new List<Curve>() { innerCurves[innerCurves.Count - 1] }, _tolerance));
            Brep innerShape = Brep.JoinBreps(brepInner, _tolerance)[0];
            #endregion

            #region differences
            Brep result = Brep.CreateBooleanDifference(outerShape, innerShape, _tolerance)[0];
            #endregion

            #region divide
            if (divide == true)
            {
                Brep box = new BoundingBox(0, -boxDepth / 2 - 5, -5, boxWidth / 2 + 5, boxDepth / 2 + 5, plane5.OriginZ + 5).ToBrep();
                result = Brep.CreateBooleanDifference(result, box, _tolerance)[0];
            }
            #endregion

            return result;
        }

        /// <summary>
        /// Returns a minimal rectangular nozzle. 
        /// </summary>
        /// <param name="length1"> The length of the connector. </param>
        /// <param name="length2"> The length between the connector and the unreduced rectangle. </param>
        /// <param name="length3"> The length between the unreduced and reduced rectangle. </param>
        /// <param name="length4"> The length of the final reduced cross section. </param>
        /// <param name="outerDiameter"> The outer diameter of the connected pipe. </param>
        /// <param name="innerDiameter"> The innner diameter of the connected pipe. </param>
        /// <param name="nozzleWidth"> The width of the nozzle. </param>
        /// <param name="nozzleHeight"> The height of the nozzle. </param>
        /// <param name="wallThickness"> The minimum wall thickness. </param>
        /// <param name="gap"> The size of the connector gap. </param>
        /// <returns> The rectanglur nozzle. </returns>
        public static Brep RectangularType2(double length1, double length2, double length3, double length4, double outerDiameter, double innerDiameter, double nozzleWidth, double nozzleHeight, double wallThickness, double gap)
        {
            if (length1 <= 0 | length2 <= 0 | length4 <= 0)
            {
                throw new Exception("The defined lengths must be larger than 0.");
            }

            length1 = Math.Max(0, length1);
            length2 = Math.Max(0, length2);
            length3 = Math.Max(0, length3);
            length4 = Math.Max(0, length4);

            Plane plane1 = new Plane(new Point3d(0, 0, 0), Vector3d.XAxis, Vector3d.YAxis);
            Plane plane2 = new Plane(new Point3d(0, 0, length1), Vector3d.XAxis, Vector3d.YAxis);
            Plane plane3 = new Plane(new Point3d(0, 0, length1 + length2), Vector3d.XAxis, Vector3d.YAxis);
            Plane plane4 = new Plane(new Point3d(0, 0, length1 + length2 + length3), Vector3d.XAxis, Vector3d.YAxis);
            Plane plane5 = new Plane(new Point3d(0, 0, length1 + length2 + length3 + length4), Vector3d.XAxis, Vector3d.YAxis);

            List<Brep> breps = new List<Brep>() { };

            #region outer shape
            List<Curve> outerCurves = new List<Curve>() { };
            double outerArea = 0.25 * (outerDiameter + 2 * wallThickness) * (outerDiameter + 2 * wallThickness) * Math.PI;
            outerCurves.Add(new Circle(plane1, outerDiameter / 2 + wallThickness).ToNurbsCurve()); // Connector
            outerCurves[0].Rotate(Rhino.RhinoMath.ToRadians(225), Vector3d.ZAxis, new Point3d(0, 0, 0));

            Curve curve1a = new Circle(plane2, outerDiameter / 2 + wallThickness).ToNurbsCurve(); // Hose/pipe diameter
            curve1a.Rotate(Rhino.RhinoMath.ToRadians(225), Vector3d.ZAxis, new Point3d(0, 0, 0));

            if (length3 > 0)
            {
                Curve curve2a = Rectangle3dCenter(plane3, nozzleWidth + 2 * wallThickness, outerArea / (nozzleWidth + 2 * wallThickness)).ToNurbsCurve(); // Rectangle: equal area
                LoftWithLinearCrossSection(curve1a, curve2a, 50, out List<Curve> scaledCurves1);
                outerCurves.AddRange(scaledCurves1);

                outerCurves.Add(Rectangle3dCenter(plane4, nozzleWidth + 2 * wallThickness, nozzleHeight + 2 * wallThickness).ToNurbsCurve()); // Rectangle: reduced area
            }
            else
            {
                Curve curve2a = Rectangle3dCenter(plane3, nozzleWidth + 2 * wallThickness, nozzleHeight + 2 * wallThickness).ToNurbsCurve(); // Rectangle: reduced area
                LoftWithLinearCrossSection(curve1a, curve2a, 50, out List<Curve> scaledCurves1);
                outerCurves.AddRange(scaledCurves1);
            }

            outerCurves.Add(Rectangle3dCenter(plane5, nozzleWidth + 2 * wallThickness, nozzleHeight + 2 * wallThickness).ToNurbsCurve()); // Rectangle: reduced area

            for (int i = 1; i < outerCurves.Count; i++)
            {
                breps.Add(Loft(outerCurves[i - 1], outerCurves[i]));
            }
            #endregion

            #region inner shape
            List<Curve> innerCurves = new List<Curve>() { };
            double innerArea = 0.25 * innerDiameter * innerDiameter * Math.PI;
            innerCurves.Add(new Circle(plane1, outerDiameter / 2).ToNurbsCurve()); // Connector
            innerCurves[0].Rotate(Rhino.RhinoMath.ToRadians(225), Vector3d.ZAxis, new Point3d(0, 0, 0));
            innerCurves.Add(new Circle(plane2, outerDiameter / 2).ToNurbsCurve()); // Connector
            innerCurves[1].Rotate(Rhino.RhinoMath.ToRadians(225), Vector3d.ZAxis, new Point3d(0, 0, 0));

            Curve curve1b = new Circle(plane2, innerDiameter / 2).ToNurbsCurve(); // Hose/pipe diameter
            curve1b.Rotate(Rhino.RhinoMath.ToRadians(225), Vector3d.ZAxis, new Point3d(0, 0, 0));

            if (length3 > 0)
            {
                Curve curve2b = Rectangle3dCenter(plane3, nozzleWidth, innerArea / nozzleWidth).ToNurbsCurve(); // Rectangle: equal area
                LoftWithLinearCrossSection(curve1b, curve2b, 50, out List<Curve> scaledCurves2);
                innerCurves.AddRange(scaledCurves2);

                innerCurves.Add(Rectangle3dCenter(plane4, nozzleWidth, nozzleHeight).ToNurbsCurve()); // Rectangle: equal area
            }
            else
            {
                Curve curve2b = Rectangle3dCenter(plane3, nozzleWidth, nozzleHeight).ToNurbsCurve(); // Rectangle: reduced area
                LoftWithLinearCrossSection(curve1b, curve2b, 50, out List<Curve> scaledCurves2);
                innerCurves.AddRange(scaledCurves2);
            }

            innerCurves.Add(Rectangle3dCenter(plane5, nozzleWidth, nozzleHeight).ToNurbsCurve()); // Rectangle: equal area

            for (int i = 1; i < innerCurves.Count; i++)
            {
                breps.Add(Loft(innerCurves[i - 1], innerCurves[i]));
            }
            #endregion

            #region caps
            breps.Add(Brep.CreatePlanarBreps(new List<Curve>() { outerCurves[0], innerCurves[0] }, _tolerance)[0]);
            breps.Add(Brep.CreatePlanarBreps(new List<Curve>() { outerCurves.Last(), innerCurves.Last() }, _tolerance)[0]);
            #endregion

            Brep result = Brep.JoinBreps(breps, _tolerance)[0];

            #region gap
            if (gap > 0)
            {
                Brep box = new BoundingBox(-gap / 2, -outerDiameter / 2 - wallThickness - 5, -5, gap / 2, outerDiameter / 2 + wallThickness + 5, length1).ToBrep();
                result = Brep.CreateBooleanDifference(result, box, _tolerance)[0];
            }
            #endregion

            return result;
        }

        /// <summary>
        /// Returns a rectangle with as centroid the origin of the given plane. 
        /// </summary>
        /// <param name="plane"> The plane. </param>
        /// <param name="width"> The width of the rectangle. </param>
        /// <param name="height"> The height of the reactangle. </param>
        /// <returns> The rectangle. </returns>
        private static Rectangle3d Rectangle3dCenter(Plane plane, double width, double height)
        {
            return new Rectangle3d(plane, new Interval(-width / 2, width / 2), new Interval(-height / 2, height / 2));
        }

        /// <summary>
        /// Returns a brep between two closed curves linear changing cross section areas between the to curves. 
        /// </summary>
        /// <param name="curve1"> The first closed curve. </param>
        /// <param name="curve2"> The second closed curve. </param>
        /// <param name="count"> The number of intermediate cross sections between the two curves. </param>
        /// <param name="curves"> The intermediate curves. </param>
        /// <returns> The brep. </returns>
        private static Brep LoftWithLinearCrossSection(Curve curve1, Curve curve2, int count, out List<Curve> curves)
        {
            if (curve1.IsClosed == false | curve2.IsClosed == false)
            {
                throw new Exception("Loft with linear cross section operation failed. The curves are not closed.");
            }

            if (curve1.IsPlanar() == false | curve2.IsPlanar() == false)
            {
                throw new Exception("Loft with linear cross section operation failed. The curves are not planar.");
            }

            // Get necessary properties
            bool check1 = curve1.TryGetPlane(out Plane plane1);
            bool check2 = curve2.TryGetPlane(out Plane plane2);

            if (check1 == false | check2 == false)
            {
                throw new Exception("Loft with linear cross section operation failed. Coult not get the start and end plane.");
            }

            AreaMassProperties prop1 = AreaMassProperties.Compute(curve1);
            AreaMassProperties prop2 = AreaMassProperties.Compute(curve2);

            double area1 = prop1.Area;
            double area2 = prop2.Area;

            plane1.Origin = prop1.Centroid;
            plane2.Origin = prop2.Centroid;

            // First loft operation
            Brep loft = Loft(curve1, curve2);

            // Divide into curves
            List<Plane> planes = Frames.InterpolateFrames(plane1, plane2, count, false, false);
            curves = new List<Curve>() { };

            for (int i = 0; i < planes.Count; i++)
            {
                Intersection.BrepPlane(loft, planes[i], _tolerance, out Curve[] intersections, out _);

                if (intersections.Length == 0)
                {
                    throw new Exception("No intersections found.");
                }

                curves.Add(intersections[0]);
            }

            // Scale curves
            List<int> range = Enumerable.Range(0, count + 2).ToList();
            List<double> factors = range.ConvertAll(item => item == 0 ? 0.0 : Convert.ToDouble(item) / range.Last());
            factors.Remove(0.0);
            factors.Remove(1.0);

            List<double> desiredAreas = new List<double>() { };
            
            for (int i = 0; i < factors.Count; i++)
            {
                desiredAreas.Add(area1 + factors[i] * (area2 - area1));
            }

            for (int i = 0; i < curves.Count; i++)
            {
                double area = AreaMassProperties.Compute(curves[i]).Area;
                double scaleFactor = Math.Sqrt(desiredAreas[i] / area);
                Transform xform = Transform.Scale(planes[i], scaleFactor, scaleFactor, 1.0);
                curves[i].Transform(xform);
            }

            // Add first and last curves
            curves.Insert(0, curve1);
            curves.Add(curve2);

            // Loft and join
            List<Brep> breps = new List<Brep>();

            for (int i = 1; i < curves.Count; i++)
            {
                breps.Add(Loft(curves[i - 1], curves[i]));
            }

            Brep result = Brep.JoinBreps(breps, _tolerance)[0];

            return result;
        }

        /// <summary>
        /// Returns a loft between two curves.
        /// </summary>
        /// <param name="curve1"> The first curve. </param>
        /// <param name="curve2"> The second curve. </param>
        /// <returns> The loft betweeon two curves. </returns>
        private static Brep Loft(Curve curve1, Curve curve2)
        {
            curve2.AlignCurve(curve1);

            Brep[] breps = (Brep.CreateFromLoft(new List<Curve>() { curve1, curve2 }, Point3d.Unset, Point3d.Unset, LoftType.Normal, false));

            if (breps.Length == 0)
            {
                throw new Exception("Loft operation failed.");
            }
            else if (breps.Length == 1)
            {
                breps[0].Faces.SplitKinkyFaces();
                return breps[0];
            }
            else
            {
                breps = Brep.JoinBreps(breps, _tolerance);
                
                if (breps.Length != 1)
                {
                    throw new Exception("Brep joint operation failed.");
                }

                breps[0].Faces.SplitKinkyFaces();

                return breps[0];
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the tolerance for the methods used in this static class.
        /// </summary>
        public static double Tolerance
        {
            get { return _tolerance; }
            set { _tolerance = value; }
        }
        #endregion
    }
}
