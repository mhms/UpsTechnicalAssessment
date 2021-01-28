using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace UPS.Homework.Service
{
    public class ServiceResult
    {
        
        protected ILogger _logger;
        public bool Succeeded { get; protected set; }
        public IList<ServiceMessage> Messages { get; protected set; }
        public _Exception Exception { get; protected set; }

        public ServiceResult(bool succeeded, IList<ServiceMessage> messages = null, _Exception exception = null)
        {
            Succeeded = succeeded;
            Messages = messages;
            Exception = exception;
            if (exception == null) return;
            _logger = LogManager.GetCurrentClassLogger();
            var logger = LogManager.GetLogger("databaseLogger");
            logger.Error(exception.GetBaseException(), "ServiceResult");
        }
    }
    public class ServiceResult<TResult> : ServiceResult
    {
        public TResult Result { get; }
       

        public ServiceResult(bool succeeded, TResult result, IList<ServiceMessage> messages = null, _Exception exception = null): base(succeeded, messages, exception)
        {
            Result = result;

           
        }
    }
}
