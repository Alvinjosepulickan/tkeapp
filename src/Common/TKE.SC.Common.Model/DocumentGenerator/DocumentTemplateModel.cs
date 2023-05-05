namespace TKE.SC.Common.Model.DocumentGenerator
{
    public class DocumentTemplateModel
    {
        public Section[] Sections { get; set; }
    }
    public class Section
    {
        public string Template { get; set; }
        public string Name { get; set; }
        public string Culture { get; set; }
        public Variable[] Variables { get; set; }
        public Table[] Tables { get; set; }
        public Subsection[] Subsections { get; set; }
    }

    public class Variable
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string DataType { get; set; }
    }

    public class Table
    {
        public string Name { get; set; }
        public string Template { get; set; }
        public string[] Headers { get; set; }
        public Variable[][] Rows { get; set; }
        public Variable[] Footer { get; set; }
    }

    public class Subsection : Section
    {
    }
}