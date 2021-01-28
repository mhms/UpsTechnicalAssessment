using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPS.Homework.Service
{
    public class BaseService
    {
        protected Exception _exception;
        protected List<ServiceMessage> _messages;

        public BaseService()
        {
            _messages = new List<ServiceMessage>();
            _exception = null;
        }
    }
}
