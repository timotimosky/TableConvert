using TableConvert.Utility.Tpl;

namespace TableConvert.Utility.Lua
{
    public class LuaFieldTpl : TemplateBase
    {
        public override string FileName
        {
            get { return "Field.txt"; }
        }


        public override string Structure(params string[] args)
        {
            if (args.Length < 3)
                return ErrorInfo("args.Length < 3");

            mIsError = false;
            return string.Format(TemplateInfo, args[0], args[1], args[2]);
        }
    }
}