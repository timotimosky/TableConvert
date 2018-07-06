namespace TableConvert.Utility.Tpl
{
    public interface ITemplate
    {

        string FileName { get; }

        string TemplateInfo { get; }

        string Structure(params string[] args);

    }
}