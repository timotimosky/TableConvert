using System.Collections.Generic;

namespace TableConvert.Utility.Tpl.CodeKey
{

    public class CodeKeyDataManager
    {

        public Dictionary<string, CodeKeyBase> CodeKey = new Dictionary<string, CodeKeyBase>();

        public CodeKeyBase Get(string language)
        {
            CodeKeyBase result = null;
            CodeKey.TryGetValue(language, out result);
            return result;
        }

    }
}