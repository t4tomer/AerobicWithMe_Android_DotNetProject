using System.Text.Json;
using Realms;
using Realms.Sync;
using AerobicWithMe.Models;

namespace AerobicWithMe.Services
{
    public static class RealmService
    {
        private static bool serviceInitialised;

        private static Realms.Sync.App app;

        private static Realm mainThreadRealm;

        public static User CurrentUser => app.CurrentUser;

        public static string DataExplorerLink;

        private static FlexibleSyncConfiguration config3;

        public static async Task Init()
        {
            if (serviceInitialised)
            {
                return;
            }

            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("atlasConfig.json");
            using StreamReader reader = new(fileStream);
            var fileContent = await reader.ReadToEndAsync();

            var config = JsonSerializer.Deserialize<RealmAppConfig>(fileContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var appConfiguration = new AppConfiguration(config.AppId)
            {
                BaseUri = new Uri(config.BaseUrl)
            };

            app = Realms.Sync.App.Create(appConfiguration);

            serviceInitialised = true;

            // If you're getting this app code by cloning the repository at
            // https://github.com/mongodb/template-app-maui-todo, 
            // it does not contain the data explorer link. Download the
            // app template from the Atlas UI to view a link to your data.
            DataExplorerLink = config.DataExplorerLink;
            Console.WriteLine($"To view your data in Atlas, use this link: {DataExplorerLink}");
        }

        public static Realm GetMainThreadRealm()
        {

            return mainThreadRealm ??= GetRealm();//original code line


        }


        private static (IQueryable<MapPin> Query, string Name) GetQueryForSubscriptionMapPinType(Realm realm, SubscriptionType subType)
        {

            Console.WriteLine($"(GetQueryForSubscriptionDogType)inputObject is MapPin ");


            IQueryable<MapPin> query = null;
            string queryName = null;

            if (subType == SubscriptionType.Mine)
            {
                query = realm.All<MapPin>().Where(i => i.OwnerId == CurrentUser.Id);
                queryName = "mine";
            }
            else if (subType == SubscriptionType.All)
            {
                query = realm.All<MapPin>();
                queryName = "all";
            }
            else
            {
                throw new ArgumentException("Unknown subscription type");
            }

            return (query, queryName);
        }

        private static (IQueryable<UserRecord> Query, string Name) GetQueryForSubscriptionUserRecordType(Realm realm, SubscriptionType subType)
        {

            Console.WriteLine($"(GetQueryForSubscriptionDogType)inputObject is UserRecord ");


            IQueryable<UserRecord> query = null;
            string queryName = null;

            if (subType == SubscriptionType.Mine)
            {
                query = realm.All<UserRecord>().Where(i => i.OwnerId == CurrentUser.Id);
                queryName = "mine";
            }
            else if (subType == SubscriptionType.All)
            {
                query = realm.All<UserRecord>();
                queryName = "all";
            }
            else
            {
                throw new ArgumentException("Unknown subscription type");
            }

            return (query, queryName);
        }





        public static Realm GetRealm()
        {

            var singleton = ObjectSingleton.Instance;
            
            // Default type
            Console.WriteLine($"Default type: {singleton.GetCurrentType().Name}");

            if (singleton.GetCurrentType() == typeof(MapPin))
            {
                Console.WriteLine($"GetRealm type is MapPin");

                var configPinMap = new FlexibleSyncConfiguration(app.CurrentUser)
                {
                    PopulateInitialSubscriptions = (realm) =>
                    {
                        var (query, queryName) = GetQueryForSubscriptionMapPinType(realm, SubscriptionType.Mine);
                        realm.Subscriptions.Add(query, new SubscriptionOptions { Name = queryName });
                    }
                };

                return Realm.GetInstance(configPinMap);

            }


            Console.WriteLine($" GetRealm the type is UserRecord");

            var configUserRecord = new FlexibleSyncConfiguration(app.CurrentUser)
            {
                PopulateInitialSubscriptions = (realm) =>
                {
                    var (query, queryName) = GetQueryForSubscriptionUserRecordType(realm, SubscriptionType.Mine);
                    realm.Subscriptions.Add(query, new SubscriptionOptions { Name = queryName });
                }
            };
            return Realm.GetInstance(configUserRecord);


        }
        /* //the orignal method for GetRealm
        public static Realm GetRealm()
        {

            var config = new FlexibleSyncConfiguration(app.CurrentUser)
            {
                PopulateInitialSubscriptions = (realm) =>
                {
                    var (query, queryName) = GetQueryForSubscriptionType(realm, SubscriptionType.Mine);
                    realm.Subscriptions.Add(query, new SubscriptionOptions { Name = queryName });
                }
            };

            return Realm.GetInstance(config);
        }

        */

        public static async Task RegisterAsync(string email, string password)
        {
            await app.EmailPasswordAuth.RegisterUserAsync(email, password);
        }

        public static async Task LoginAsync(string email, string password)
        {
            await app.LogInAsync(Credentials.EmailPassword(email, password));

            //This will populate the initial set of subscriptions the first time the realm is opened
            using var realm = GetRealm();//orignal code line
            //using var realm = GetRealmForMultipleTypes();
            await realm.Subscriptions.WaitForSynchronizationAsync();
        }

        public static async Task LogoutAsync()
        {
            await app.CurrentUser.LogOutAsync();
            mainThreadRealm?.Dispose();
            mainThreadRealm = null;
        }

        public static async Task SetSubscription(Realm realm, SubscriptionType subType)
        {
            if (GetCurrentSubscriptionType(realm) == subType)
            {
                return;
            }

            realm.Subscriptions.Update(() =>
            {
                realm.Subscriptions.RemoveAll(true);

                var (query, queryName) = GetQueryForSubscriptionType(realm, subType);

                realm.Subscriptions.Add(query, new SubscriptionOptions { Name = queryName });
            });

            //There is no need to wait for synchronization if we are disconnected
            if (realm.SyncSession.ConnectionState != ConnectionState.Disconnected)
            {
                await realm.Subscriptions.WaitForSynchronizationAsync();
            }
        }

        public static SubscriptionType GetCurrentSubscriptionType(Realm realm)
        {
            var activeSubscription = realm.Subscriptions.FirstOrDefault();

            return activeSubscription.Name switch
            {
                "all" => SubscriptionType.All,
                "mine" => SubscriptionType.Mine,
                _ => throw new InvalidOperationException("Unknown subscription type")
            };
        }

        private static (IQueryable<MapPin> Query, string Name) GetQueryForSubscriptionType(Realm realm, SubscriptionType subType)
        {
            IQueryable<MapPin> query = null;
            string queryName = null;

            if (subType == SubscriptionType.Mine)
            {
                query = realm.All<MapPin>().Where(i => i.OwnerId == CurrentUser.Id);
                queryName = "mine";
            }
            else if (subType == SubscriptionType.All)
            {
                query = realm.All<MapPin>();
                queryName = "all";
            }
            else
            {
                throw new ArgumentException("Unknown subscription type");
            }

            return (query, queryName);
        }
    }

    public enum SubscriptionType
    {
        Mine,
        All,
    }

    public class RealmAppConfig
    {
        public string AppId { get; set; }

        public string BaseUrl { get; set; }


        // If you're getting this app code by cloning the repository at
        // https://github.com/mongodb/template-app-maui-todo, 
        // it does not contain the data explorer link. Download the
        // app template from the Atlas UI to view a link to your data.
        public string DataExplorerLink { get; set; }
    }
}

