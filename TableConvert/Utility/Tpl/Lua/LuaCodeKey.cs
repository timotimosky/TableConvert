using System.Collections.Generic;

namespace TableConvert.Utility.Lua
{
    public static class LuaCodeKey
    {

        static HashSet<string> _hashSet = new HashSet<string>()
        {
            "and","break","do","else","elseif","end",
            "false","for","function",
            "if","in","local","nil","not","or",
            "repeat", "return", "then", "true", "until", "while",
        };


        public static bool IsLegal(string key, out string legalKey)
        {
            legalKey = string.Empty;
            if (_hashSet.Contains(key))
            {
                legalKey = key + "_Amend";
                return false;
            }
            return true;
        }

    }
}