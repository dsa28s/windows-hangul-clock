using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Realms;

namespace HangulClockDataKit
{
    public class DataKit
    {
        public static Realm sharedRealms;

        public static DataKit getInstance()
        {
            var config = new RealmConfiguration("C:\\Hangul Clock Configuration Files\\") {
                SchemaVersion = 3,
            };

            sharedRealms = Realm.GetInstance(config);
            return new DataKit();
        }

        public Realm getSharedRealms()
        {
            if (sharedRealms == null)
            {
                sharedRealms = Realm.GetInstance();
            }

            return sharedRealms;
        }
    }
}
