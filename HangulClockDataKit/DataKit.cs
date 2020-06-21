
using Realms;

namespace HangulClockDataKit
{
    public class DataKit
    {
        public Realm Realm;

        public DataKit()
        {
            var config = new RealmConfiguration("C:\\Hangul Clock Configuration Files\\")
            {
                SchemaVersion = 5,
            };

            Realm = Realm.GetInstance(config);
        }
    }
}
