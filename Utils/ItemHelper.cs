using System;

namespace CitrusLib
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
            if (!string.IsNullOrEmpty(input) && input[0] != '_' && Enum.TryParse<Item>("_" + input, true, out var underscored))
            {
                id = (int)underscored;
                return true;
            }
            return false;
        }
    }
}