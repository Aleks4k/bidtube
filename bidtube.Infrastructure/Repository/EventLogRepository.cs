using bidtube.Application.Common.Contracts;
using bidtube.Domain.Data;
using bidtube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bidtube.Infrastructure.Repository
{
    public class EventLogRepository : IEventLog
    {
        private readonly AppDbContext _context;
        public EventLogRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task WriteLog(EventLog eventLog)
        {
            await _context.EventLogs.AddAsync(eventLog);
            await _context.SaveChangesAsync();
        }
    }
}