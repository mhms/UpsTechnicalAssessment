using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace UPS.Homework.Service
{
    public class ServiceMessage
    {
        
        public MessageType Type { get; }
        public MessageId Message { get; }

        public ServiceMessage(MessageType type, MessageId message)
        {
            Type = type;
            Message = message;
        }
    }

    public enum MessageId
    {
        [Description("")] None = 0,

       
        [Description("Successfully Done!")]
        Succeeded = 1,

        
        [Description("An error has occured, please try again later")]
        Exception = 3,

        [Description("There was an internal error")]
        InternalError = 4,

        
        [Description("Data Inconsistency")]
        DataInconsistency = 5,


        [Description("The entry does not exist anymore")]
        EntityDoesNotExist = 6,

     
        [Description("Unsuccessful Operation")]
        UnsuccessfulOperation = 7,


        [Description("Invalid input")]
        InputDataValidationError = 8,

        [Description("Unauthorized access")] AccessDenied = 9,
        [Description("The email address is Already taken")] EmailAddressTaken = 10,
        [Description("The user successfully added")] UserAddition = 11,
    }
    public enum MessageType
    {
        /// <summary>
        /// Successful
        /// </summary>
        [Description("Succeed")]
        Succeed,

        /// <summary>
        /// Information
        /// </summary>
        [Description("Info")]
        Info,

        /// <summary>
        /// Warning
        /// </summary>
        [Description("Warning")]
        Warning,

        /// <summary>
        /// Error
        /// </summary>
        [Description("Error")]
        Error,
    }
}
