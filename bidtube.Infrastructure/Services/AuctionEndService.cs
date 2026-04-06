using bidtube.Application.Auctions.Contracts;
using bidtube.Application.Notifications.Contracts;
using bidtube.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Infrastructure.Services
{
    public class AuctionEndService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public AuctionEndService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _auctionRepository = scope.ServiceProvider.GetRequiredService<IAuction>();
                    var _notificationRepository = scope.ServiceProvider.GetRequiredService<INotification>();
                    var toNotify = await _auctionRepository.EndAuctions();
                    await _notificationRepository.AddNotifications(toNotify);
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}
