using System;
using UnityEngine;

namespace WobbleBridge
{
    // Logging changes and stuff
    public static partial class Wobble
    {
        // Internal logger for WobbleBridge debugging — don't use this, make your own Logger!
        internal static Logger log = new Logger("WobbleBridge", ConsoleColor.Cyan);

        internal static bool landLogSupressed = false;

        // Takes over Landfall's logger to match our format
        internal static Logger landLog = new Logger("LandLog", ConsoleColor.Gray);
    }

    /// <summary>
    /// A class for prettier debug logging.
    /// </summary>
    public class Logger
    {
        string modName;
        ConsoleColor modColor;

        static string lastMessage = "";
        static int combo = 1;

        /// <summary>
        /// Creates a new Logger for your mod.
        /// </summary>
        /// <param name="n">The name of your mod, or a sub-name if your mod has multiple separate parts.</param>
        /// <param name="c">The color of the name prefix.</param>
        public Logger(string n, ConsoleColor c = ConsoleColor.White)
        {
            modName  = n;
            modColor = c;
        }

        /// <summary>
        /// Logs a standard message to the console.
        /// </summary>
        public void Log(string text) => Write(text, ConsoleColor.Gray);

        /// <summary>
        /// Logs a warning to the console (yellow).
        /// </summary>
        public void LogWarning(string text) => Write(text, ConsoleColor.Yellow, warning: true);

        /// <summary>
        /// Logs an error to the console (red).
        /// </summary>
        public void LogError(string text) => Write(text, ConsoleColor.Red, error: true);


        // ─── Internal Write ───────────────────────────────────────────────────────

        private void Write(string text, ConsoleColor textColor, bool error = false, bool warning = false)
        {
            if (this == Wobble.landLog && Wobble.landLogSupressed)
                return;

            if (Compare(text, lastMessage) / Mathf.Max(1f, Mathf.Max(text.Length, lastMessage.Length)) < 0.15f)
            {
                ClearLastLine();
                combo++;
            }
            else
            {
                combo = 1;
            }

            // Prefix
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");
            Console.ForegroundColor = modColor;
            Console.Write(modName);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");

            // Severity tag
            if (error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
            }
            else if (warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[WARN]  ");
            }

            Console.Write("- ");

            // Message
            Console.ForegroundColor = textColor;
            Console.Write(text);

            // Combo counter
            if (combo > 1)
            {
                Console.ForegroundColor = combo switch
                {
                    < 10  => ConsoleColor.White,
                    < 25  => ConsoleColor.Yellow,
                    < 50  => ConsoleColor.DarkYellow,
                    < 100 => ConsoleColor.Red,
                    < 500 => ConsoleColor.DarkRed,
                    _     => combo % 2 == 0 ? ConsoleColor.DarkRed : ConsoleColor.Yellow
                };
                Console.Write($" (COMBO: {combo})");
            }

            // Unity error hook (shows in Unity console / log file)
            if (error)
                Debug.LogError($"[{modName}] {text}");
            else if (warning)
                Debug.LogWarning($"[{modName}] {text}");
            else
                Console.WriteLine("");

            lastMessage = text;
        }


        // ─── Helpers ──────────────────────────────────────────────────────────────

        static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        // Levenshtein distance — used to detect repeated/similar messages for the combo system
        static int Compare(string s, string t)
        {
            if (string.IsNullOrEmpty(s)) return string.IsNullOrEmpty(t) ? 0 : t.Length;
            if (string.IsNullOrEmpty(t)) return s.Length;

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = t[j - 1] == s[i - 1] ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
    }
}