using ff14bot.Managers;
using ff14bot.RemoteWindows;

namespace ExBuddy.Helpers
{
    public static class GetRetainerNum
    {
        public static int GetNumberOfRetainers()
        {
            string bell = GetBellLuaString();
            var numOfRetainers = 0;

            if (bell.Length > 0) numOfRetainers = Lua.GetReturnVal<int>(string.Format("return _G['{0}']:GetRetainerEmployedCount();", bell));

            return numOfRetainers;
        }

        public static string GetRetainerName()
        {
            if (GetBellLuaString().Length > 0 && CanGetName()) return Lua.GetReturnVal<string>($"return _G['{GetBellLuaString()}']:GetRetainerName();");

            return "";
        }

        private static string GetBellLuaString()
        {
            string func = "local values = '' for key,value in pairs(_G) do if string.match(key, '{0}:') then return key;   end end return values;";
            string searchString = "CmnDefRetainerBell";
            string bell = Lua.GetReturnVal<string>(string.Format(func, searchString)).Trim();

            return bell;
        }

        private static bool CanGetName()
        {
            return (RaptureAtkUnitManager.GetWindowByName("InventoryRetainer") != null ||
                    RaptureAtkUnitManager.GetWindowByName("InventoryRetainerLarge") != null ||
                    SelectString.IsOpen);
        }
    }
}