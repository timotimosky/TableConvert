using TableConvert.Utility.Tpl;

namespace TableConvert.Utility.Lua
{
    public class LuaTableMouldTpl : TemplateBase
    {
        public override string FileName
        {
            get { return "TableMould.txt"; }
        }


        public override string Structure(params string[] args)
        {
            if (args.Length < 4)
                return ErrorInfo("args.Length < 4");

            mIsError = false;
            return Replace(TemplateInfo, args);
        }

    }
}