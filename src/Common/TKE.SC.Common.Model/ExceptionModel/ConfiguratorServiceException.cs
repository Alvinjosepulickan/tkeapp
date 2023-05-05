using TKE.SC.Common.Model;

namespace TKE.SC.Common.Model.ExceptionModel
{
    /// <summary>
    /// ConfiguratorServiceException
    /// </summary>
    public class ConfiguratorServiceException : ExternalCallException
    {
        /// <summary>
        /// Configuration Service Exception
        /// </summary>
        /// <param Name="responseMessage"></param>
        public ConfiguratorServiceException(ResponseMessage responseMessage) : base(responseMessage)
        {
            
        }
    }
}
