using System;

namespace WobbleBridge.Utils
{
    public static class ItemHelper
    {
        public static bool TryParseItemId(string input, out int id)
        {
            id = 0;
            if (int.TryParse(input, out id)) return true;
            if (Enum.TryParse<Item>(input, true, out var item))
            {
                id = (int)item;
                return true;
            }
            return false;
        }
    }
}