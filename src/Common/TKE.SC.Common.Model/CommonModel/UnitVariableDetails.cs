using System;
using System.Collections.Generic;
using System.Text;
using TKE.SC.Common.Model.UIModel;

namespace TKE.SC.Common.Model.CommonModel
{
    public class UnitVariableDetails : ConfigVariable
    {
        public List<ConfigVariable>  listOfConfigVariables { get; set; }

        public List<string> conflictListOfStrings { get; set; }
    }
}
