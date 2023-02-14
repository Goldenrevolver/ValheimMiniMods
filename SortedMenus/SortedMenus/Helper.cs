using UnityEngine;

namespace SortedMenus
{
    public static class Helper
    {
        internal static void Log(object s)
        {
            if (!SortConfig.EnableDebugLogs.Value)
            {
                return;
            }

            var toPrint = $"{SortedMenusPlugin.NAME} {SortedMenusPlugin.VERSION}: {(s != null ? s.ToString() : "null")}";

            Debug.Log(toPrint);
        }

        internal static void LogWarning(object s)
        {
            var toPrint = $"{SortedMenusPlugin.NAME} {SortedMenusPlugin.VERSION}: {(s != null ? s.ToString() : "null")}";

            Debug.LogWarning(toPrint);
        }
    }
}