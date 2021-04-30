// This file is part of SaladSlicer. SaladSlicer is licensed 
// under the terms of GNU General Public License as published by the 
// Free Software Foundation. For more information and the LICENSE file, 
// see <https://github.com/3DCP-TUe/SaladSlicer>.

// System Libs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaladSlicer.Core.Examples
{
    /// <summary>
    /// Represents a simple example class of a point
    /// </summary>
    public class ExamplePoint
    {
        #region fields
        private double _x;
        private double _y;
        private double _z;
        #endregion

        #region contructors
        public ExamplePoint(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }
        #endregion

        #region methods
        public double DistanceTo(ExamplePoint point)
        {
            double dx = _x - point.X;
            double dy = _y - point.Y;
            double dz = _z - point.Z;

            double distance = Math.Sqrt(dx*dx + dy*dy + dz*dz);

            return distance;
        }
        #endregion

        #region properties
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }
        #endregion

        #region operators
        public static ExamplePoint operator +(ExamplePoint point1, ExamplePoint point2)
        {
            return new ExamplePoint(point1.X + point2.X, point1.Y + point2.Y, point1.Z + point2.Z);
        }
        #endregion
    }
}
