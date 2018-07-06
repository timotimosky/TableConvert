using TableConvert.Utility.Lua;

// ReSharper disable InconsistentNaming
namespace TableConvert.Utility.Tpl
{
    public class TemplateBase : ITemplate
    {
        protected bool mIsError = false;
        protected string mTemplateInfo = string.Empty;


        public virtual string FileName
        {
            get { return string.Empty; }
        }


        public virtual string TemplateInfo
        {
            get
            {
                if (string.IsNullOrEmpty(mTemplateInfo))
                {
                    mTemplateInfo = LuaTemplate.GetTemplateValue(FileName);
                }
                return mTemplateInfo;
            }
        }


        public virtual string Structure(params string[] args)
        {
            return string.Empty;
        }


        public string ErrorInfo(string info)
        {
            mIsError = true;
            return string.Format("{0} - {1} !", this.GetType().Name, info);
        }


        public string Replace(int index, string str, string value)
        {
            return str.Replace("{" + index.ToString() + "}", value);
        }


        public string Replace(string str, string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                str = str.Replace("{" + i.ToString() + "}", args[i]);
            }
            return str;
        }

    }
}