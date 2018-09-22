using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Realms;

namespace HangulClockDataKit.Model
{
    public class PreferencesModel: RealmObject
    {  
        public string pref_name { get; set; }
        public string pref_string_value { get; set; }
        public bool pref_bool_value { get; set; }
        public int pref_int_value { get; set; }
    }
}
