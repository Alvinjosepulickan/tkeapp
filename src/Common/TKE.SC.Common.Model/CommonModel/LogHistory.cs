using System.Collections.Generic;

namespace TKE.SC.Common.Model
{
    /// <summary>
    /// LogHistoryRequestBody
    /// </summary>
    public class LogHistoryRequestBody
    {
        /// <summary>
        /// Section
        /// </summary>
        public string Section { get; set; }
        /// <summary>
        /// BuildindId
        /// </summary>
        public int BuildingId { get; set; }
        /// <summary>
        /// GroupId
        /// </summary>
        public int GroupId { get; set; }
        /// <summary>
        /// SetID
        /// </summary>
        public int SetID { get; set; }
        /// <summary>
        /// UnitID
        /// </summary>
        public int UnitID { get; set; }
        /// <summary>
        /// LastDate
        /// </summary>
        public string LastDate { get; set; }
    }

    /// <summary>
    /// LogHistoryResponse
    /// </summary>
    public class LogHistoryResponse
    {
        /// <summary>
        /// Section
        /// </summary>
        public string Section { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        public List<Data> Data { get; set; }
        /// <summary>
        /// ShowLoadMore
        /// </summary>
        public bool ShowLoadMore { get; set; }
    }

    /// <summary>
    /// Data
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Date
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        ///LogParameters 
        /// </summary>
        public List<LogParameters> LogParameters { get; set; }

    }

    /// <summary>
    /// VariableId
    /// </summary>
    public class LogParameters
    {
        /// <summary>
        /// VariableId
        /// </summary>
        public string VariableId { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// UpdatedValue
        /// </summary>
        public string UpdatedValue { get; set; }
        /// <summary>
        /// PreviousValue
        /// </summary>
        public string PreviousValue { get; set; }
        /// <summary>
        /// User
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// Role
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// Time
        /// </summary>
        public string Time { get; set; }
    }

    /// <summary>
    /// ConsoleHistory
    /// </summary>
    public class ConsoleHistory
    {
        /// <summary>
        /// Console
        /// </summary>
        public string Console { get; set; }
        /// <summary>
        /// Parameter
        /// </summary>
        public string Parameter { get; set; }
        /// <summary>
        /// FloorNumber
        /// </summary>
        public int FloorNumber { get; set; }
        /// <summary>
        /// Opening
        /// </summary>
        public string Opening { get; set; }
        /// <summary>
        /// PresentValue
        /// </summary>
        public string PresentValue { get; set; }
        /// <summary>
        /// PreviousValue
        /// </summary>
        public string PreviousValue { get; set; }
        /// <summary>
        /// UnitId
        /// </summary>
        public string UnitId { get; set; }
        /// <summary>
        /// GroupName
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// HallStationName
        /// </summary>
        public string HallStationName { get; set; }
    }

    /// <summary>
    /// LogHistoryTable
    /// </summary>
    public class LogHistoryTable
    {
        /// <summary>
        /// VariableId
        /// </summary>
        public string VariableId { get; set; }
        /// <summary>
        /// UpdatedValue
        /// </summary>
        public string UpdatedValue { get; set; }
        /// <summary>
        /// PreviuosValue
        /// </summary>
        public string PreviuosValue { get; set; }
    }
}
