using TableConvert.Utility.Tpl;

namespace TableConvert.Utility.Lua
{
    public class LuaLineTpl : TemplateBase
    {
        public override string FileName
        {
            get { return "Line.txt"; }
        }

        public override string Structure(params string[] args)
        {
            if (args.Length < 2)
            {
                return ErrorInfo("args.Length < 2");
            }
            mIsError = false;
            return Replace(TemplateInfo, args);
        }
    }
}