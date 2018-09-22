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
            sharedRealms = Realm.GetInstance(new RealmConfiguration("C:\\Hangul Clock Configuration Files\\"));
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
