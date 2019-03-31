using UnityEngine;

namespace Intersection.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 ClipToPlane(this Vector3 v)
        {
            return new Vector3(v.x, 0, v.z);
        }
    }
}