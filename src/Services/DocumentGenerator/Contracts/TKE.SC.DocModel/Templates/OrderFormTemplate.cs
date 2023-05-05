using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKE.CPQ.DocModel.Templates
{
    public class OrderFormTemplate
    {
        public Section[] Sections { get; set; }

        public class Section
        {
            public string Template { get; set; }
            public string Name { get; set; }
            public string Culture { get; set; } = "en-US";
            public Variable[] Variables { get; set; }
            public Table[] Tables { get; set; }
            public Subsection[] Subsections { get; set; }
        }

        public class Variable
        {
            public string Key { get; set; }
            public string DisplayName { get; set; }
            public object Value { get; set; }
            public string DataType { get; set; }
            public string Classes { get; set; }
            public string ComponentType { get; set; } = "label";
        }

        public class Table
        {
            public string Name { get; set; }
            public string Template { get; set; }
            public Variable[] Headers { get; set; }
            public Variable[][] Rows { get; set; }
            public Variable[] Footer { get; set; }
        }

        public class Subsection : Section
        {
        }
    }
}
