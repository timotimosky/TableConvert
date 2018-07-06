using System;
using TableConvert.Utility.Tpl;

namespace TableConvert.Utility.Lua
{
    public class LuaModelTpl : TemplateBase
    {

        public override string FileName
        {
            get { return "Mould.txt"; }
        }


        public override string Structure(params string[] args)
        {
            if (args.Length < 2)
                return ErrorInfo("args.Length < 2");

            mIsError = false;
            return Replace(TemplateInfo, args);
        }
        
    }
}