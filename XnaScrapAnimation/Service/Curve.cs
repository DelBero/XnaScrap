using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Interpolation;

namespace XnaScrapAnimation.Service
{
    public struct CurvePoint
    {
        public TimeSpan time;
        public float value;
    }

    public struct CurveVector
    {
        public TimeSpan time;
        public Vector3 vector;
    }

    public struct CurveQuaternion
    {
        public TimeSpan time;
        public Quaternion quaternion;
    }

    public class Curve : ICurve
    {
        #region member
        private List<CurvePoint> m_points = new List<CurvePoint>();
        #endregion

        #region CDtors
        public Curve() {}

        public Curve(List<CurvePoint> points)
        {
            m_points.AddRange(points);
        }
        #endregion

        public void addPoints(List<CurvePoint> points)
        {
            m_points.AddRange(points);
        }

        public float getValue(TimeSpan time)
        {
            if (m_points.Count == 0)
            {
                throw new Exception("Error: no  points in curve");
            }
            int last = 0, next = 0;

            while (m_points[next].time < time)
            {
                last = next;
                ++next;
            }
            //return Interpolation.lerp(m_points[last].value, m_points[next].value, (time - m_points[last].time) / (m_points[next].time - m_points[last].time));
            return m_points[next].value;
        }
    }

    public class VectorCurve : ICurve
    {
        #region member
        private List<CurveVector> m_points = new List<CurveVector>();
        #endregion
    }

    public class QuaternionCurve : ICurve
    {
        #region member
        private List<CurveQuaternion> m_points = new List<CurveQuaternion>();
        #endregion
    }
}
