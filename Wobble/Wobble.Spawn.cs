using System;
using UnityEngine;

namespace WobbleBridge
{
    public static partial class Wobble
    {
        /// <summary>
        /// Gets a random point within the current ring's white circle, snapped to the ground.
        /// </summary>
        /// <param name="tries">Number of attempts to find a valid point.</param>
        /// <param name="func">Optional predicate for additional position validation.</param>
        public static Vector3 RandomInRing(int tries = 10, Predicate<Vector3> func = null)
        {
            return RandomInCircle(
                World.SpawnedRing.currentWhiteRingPosition,
                World.SpawnedRing.currentWhiteSize,
                tries,
                func
            );
        }

        /// <summary>
        /// Gets a random point on land within a specified radius, snapped to the ground.
        /// </summary>
        /// <param name="pos">Center of the search circle.</param>
        /// <param name="radius">Radius of the search circle.</param>
        /// <param name="tries">Number of attempts to find a valid point.</param>
        /// <param name="func">Optional predicate for additional position validation.</param>
        /// <param name="initTries">Internal use — tracks original try count for error logging.</param>
        public static Vector3 RandomInCircle(Vector3 pos, float radius, int tries = 10, Predicate<Vector3> func = null, int initTries = -1)
        {
            if (initTries == -1) initTries = tries;

            pos += UnityEngine.Random.insideUnitSphere * radius * 0.45f;
            pos.y = 1500;

            Vector3 result = Vector3.zero;
            Physics.Raycast(
                new Ray(pos, Vector3.down),
                out RaycastHit raycastHit,
                3000f,
                LayerMask.NameToLayer("Map") | LayerMask.NameToLayer("Terrain")
            );

            if (raycastHit.transform && raycastHit.point.y > 140f)
                result = raycastHit.point;

            if (result != Vector3.zero && (func == null || func(result)))
                return result;

            if (--tries > 0)
                return RandomInCircle(pos, radius, tries, func, initTries);

            pos.y = 200;
            log.LogError($"RandomInCircle failed after {initTries} tries!");
            return pos;
        }
    }
}