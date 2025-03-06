using UnityEngine;

namespace V1king
{
    public class MathConversions
    {
        public static float ConvertNumberRange(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return ((value - oldMin) * (newMax - newMin) / (oldMax - oldMin)) + newMin;
        }
    }

    public class VectorConversions
    {
        public static Vector3Int GetVector3Int(Vector3 vector3)
        {
            return new Vector3Int((int)vector3.x, (int)vector3.y, (int)vector3.z);
        }

        public static Vector3 GetVector3(Vector3 vector3)
        {
            return new Vector3(vector3.x, vector3.y, vector3.z);
        }

        public static Vector3 GetVectorFromAngle(float angle)
        {
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static float GetAngleFromVector(Vector3 dir)
        {
            //dir = dir.normalized;
            //float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //if(n < 0) n += 360;
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
    }

    public class Measurements
    {
        public static Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
        {
            Vector3 P = x * Vector3.Normalize(B - A) + A;
            return P;
        }

        public static Vector3 GetDeflection(Vector3 targetPos, Vector3 targetVelocity, float distance, float projectileSpeed)
        {
            float travelTime = distance / projectileSpeed;
            return targetPos + targetVelocity * travelTime;
        }
    }
}