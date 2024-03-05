using UnityEngine;

namespace Helper.Libs
{
    public static class MathExtra
    {
        /// <summary>
        /// Degrees of a full circle.
        /// </summary>
        public const int FullCircleDegrees = 360;
        
        /// <summary>
        /// Degrees of a half circle.
        /// </summary>
        public const int HalfCircleDegrees = 180;
        
        /// <summary>
        /// take any angle, and see if it's outside the bounds provided. If it's outside the bounds provided,
        /// return the bound passed. otherwise, return the original angle
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="lowerAngleBound"></param>
        /// <param name="upperAngleBound"></param>
        /// <returns></returns>
        public static float ClampAngle(float angle, float lowerAngleBound, float upperAngleBound)
        {
            // accepts e.g. -80, 80
            if (angle < 0f) angle = FullCircleDegrees + angle;

            return angle > HalfCircleDegrees ? Mathf.Max(angle, FullCircleDegrees + lowerAngleBound) : Mathf.Min(angle, upperAngleBound);
        }
    }
}