using Realms;
using HangulClockDataKit.Model;
using System.Linq;

namespace HangulClockDataKit
{
    public class DataKit
    {
        public Realm Realm;

        public DataKit()
        {
            var config = new RealmConfiguration("C:\\Hangul Clock Configuration Files\\")
            {
                SchemaVersion = 6,
                MigrationCallback = (migration, oldSchemaVersion) =>
                {
                    if (oldSchemaVersion < 6)
                    {
                        var newBackgroundSetting = migration.NewRealm.All<ClockSettingsByMonitor>();
                        var oldBackgroundSetting = migration.OldRealm.All("ClockSettingsByMonitor").ToList();
                    }
                }
            };

            Realm = Realm.GetInstance(config);
        }
    }
}
