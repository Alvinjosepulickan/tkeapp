using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.Common.Caching.Helper
{
    public class Constant
    {
        /// <summary>
        /// Constants Related to Special Charcters
        /// </summary>
        #region Special characters related constants
        public static readonly string UNDERSCORE = "_";
        public static readonly string COMMA = ",";
        public static readonly char COMMACHAR = ',';
        #endregion

        /// <summary>
        /// Constants Related to APP Settings
        /// </summary>
        #region Settings related constants
        public static readonly string APPSETTINGS = "appsettings.json";
        public static readonly string PARAMSETTINGS = "ParamSettings";
        public static readonly string ENVIRONMENT = "Environment";
        public static readonly string CACHESLIDINGTIMEOUT = "CacheSlidingTimeOut";
        public static readonly string CPQ = "CPQ";
        public static readonly string KEYBUNCH = "KEYBUNCH";
        #endregion

        /// <summary>
        /// Constants Related to Logging
        /// </summary>
        #region Logging related constants
        public static readonly string DISTRIBUTEDCACHENULLREDISNOTINITIALIZED = "distributedCache is null, Redis is not initialized";
        public static readonly int NOTFOUND = 404;
        #endregion
    }
}
