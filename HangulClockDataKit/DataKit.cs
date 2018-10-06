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
