using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
    public interface ISubscriptionJobService
    {
        Task CheckPendingSubscriptionsAsync();
    }
}
