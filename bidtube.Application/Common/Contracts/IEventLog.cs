using bidtube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Common.Contracts
{
    public interface IEventLog
    {
        Task WriteLog(EventLog eventLog);
    }
}
