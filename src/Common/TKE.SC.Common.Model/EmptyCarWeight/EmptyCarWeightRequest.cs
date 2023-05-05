using TKE.SC.Common.Model.ViewModel;

namespace TKE.SC.Common.Model.EmptyCarWeight
{
    public class EmptyCarWeightRequest : ConfigurationRequest
    {
        public Line Line { get; set; }
    }

    public class Line : Configit.Configurator.Server.Common.Line
    {
        public ValueWithUnit Value { get; set; }
    }

    public class ValueWithUnit
    {

        public int Value { get; set; }
        public string Unit { get; set; }

    }
}
