using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AerobicWithMe.Models;
using System.Text.Json;
using Realms;
using Realms.Sync;
using AerobicWithMe.Services;

namespace AerobicWithMe.Interfaces
{
    public interface IRealmService
    {
        User CurrentUser { get; }
        string DataExplorerLink { get; }
        Task Init();
        Realm GetMainThreadRealm();
        Realm GetRealm();
        Task RegisterAsync(string email, string password);
        Task LoginAsync(string email, string password);
        Task LogoutAsync();
        Task SetSubscription(Realm realm, SubscriptionType subType);
        SubscriptionType GetCurrentSubscriptionType(Realm realm);
    }
}
