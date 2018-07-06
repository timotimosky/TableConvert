using System.Collections.Generic;

namespace TableConvert.Utility.Tpl.CodeKey
{
    public class CodeKeyBase
    {

        public HashSet<string> Keys = new HashSet<string>(); 


        public virtual bool IsLegal(string key, out string legalKey)
        {
            legalKey = string.Empty;
            if (Keys.Contains(key))
            {
                legalKey = key + "_Amend";
                return false;
            }
            return true;
        }
    }
}