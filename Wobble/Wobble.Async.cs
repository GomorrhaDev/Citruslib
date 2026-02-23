using System;
using System.Threading.Tasks;
using Landfall.Network;
using UnityEngine;

namespace WobbleBridge
{
    public static partial class Wobble
    {
        /// <summary>
        /// Performs an action once the player is alive and sending position updates again.
        /// The action is performed immediately if the player is currently alive.
        /// Good uses: team changes, gear changes.
        /// </summary>
        public static void DoOnceAlive(PlayerRef p, Action action)
        {
            WaitUntil(() => (bool)p.data["aliveAware"], action, 0.1f);
        }

        /// <summary>
        /// TABGPlayerServer overload of DoOnceAlive.
        /// </summary>
        public static void DoOnceAlive(TABGPlayerServer p, Action action)
        {
            PlayerRef pr = players.Find(prf => prf.player == p);
            if (pr == null)
            {
                log.LogError("Player wasn't found! Action Cancelled!");
                return;
            }
            DoOnceAlive(pr, action);
        }

        /// <summary>
        /// Performs an action after a set amount of time.
        /// </summary>
        public static async void WaitThen(float time, Action a)
        {
            await Task.Delay((int)(time * 1000));
            a();
        }

        /// <summary>
        /// Performs an action once a condition becomes true.
        /// </summary>
        /// <param name="condition">The condition to wait for.</param>
        /// <param name="a">The action to perform once the condition is met.</param>
        /// <param name="tick">How often to check the condition in seconds. Don't set very low unless your condition is cheap!</param>
        internal static async void WaitUntil(Func<bool> condition, Action a, float tick = 0.2f)
        {
            while (!condition.Invoke())
            {
                if (tick <= 0)
                    await Task.Yield();
                else
                    await Task.Delay((int)(tick * 1000));
            }
            a();
        }

        /// <summary>
        /// Performs an action after a set delay, but only if the condition remains true throughout.
        /// Invokes the fail action if the condition becomes false at any point.
        /// </summary>
        /// <param name="time">Time to wait before invoking the succeed action.</param>
        /// <param name="condition">The condition that must remain true.</param>
        /// <param name="a">The action to perform on success.</param>
        /// <param name="fail">The action to invoke on failure (can be null).</param>
        /// <param name="tick">How often to check the condition in seconds.</param>
        public static async void WaitThenAsLongAs(float time, Func<bool> condition, Action a, Action fail, float tick = 0.2f)
        {
            float timeStart = Time.timeSinceLevelLoad;
            while (Time.timeSinceLevelLoad < timeStart + time)
            {
                if (!condition.Invoke())
                {
                    fail?.Invoke();
                    return;
                }

                if (tick <= 0)
                    await Task.Yield();
                else
                    await Task.Delay((int)(tick * 1000));
            }
            a?.Invoke();
        }
    }
}