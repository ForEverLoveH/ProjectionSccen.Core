using System.Collections.Generic;

namespace ProjectionSccen.Core.Models
{
    /// <summary>
    ///
    /// </summary>
    public class IMessageData
    {
        /// <summary>
        ///
        /// </summary>
        public RspGetMachineNum RspGetMachineNum { get; set; }

        /// <summary>
        ///
        /// </summary>
        public RspGetStudentDataList RspGetStudentDataList { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class RspGetStudentDataList
    {
        /// <summary>
        ///
        /// </summary>
        public string examTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<StudentData> studentDataList { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsExamEnd { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class RspGetMachineNum
    {
        /// <summary>
        ///
        /// </summary>
        public int num { get; set; }
    }
}