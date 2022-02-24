using System;
using System.Collections.Generic;

namespace Lesson1 {
    public class TemperatureTracker {
        class TemperatureRecord {
            public DateTime Date;
            public int Temperature;
        }

        List<TemperatureRecord> Data;
        public int Count { get => Data.Count; }

        public bool Add(DateTime Date, int Temperature) {
            foreach (TemperatureRecord T in Data) if (T.Date == Date) return false;
            TemperatureRecord X = new TemperatureRecord();
            X.Date = Date;
            X.Temperature = Temperature;
            Data.Add(X);
            return true;
        }

        public bool UpDate(DateTime Date, int Temperature) {
            foreach (TemperatureRecord T in Data) if (T.Date == Date) {
                    T.Temperature = Temperature;
                    return true;
                }
            return false;
        }

        public bool Delete(DateTime Date) {
            foreach (TemperatureRecord T in Data) if (T.Date == Date) {
                    Data.Remove(T);
                    return true;
                }
            return false;
        }

        public string[] List(DateTime Date1, DateTime Date2) {
            int n = 0;
            foreach (TemperatureRecord T in Data) if ((T.Date >= Date1) && (T.Date <= Date2)) n++;
            var str = new string[n];
            n = 0;
            foreach (TemperatureRecord T in Data) if ((T.Date >= Date1) && (T.Date <= Date2)) str[n++] = $"On {T.Date} was {T.Temperature} degrees C.";
            return str;
        }

        public string[] ListALL() {
            var str = new string[Count];
            int n = 0;
            foreach (TemperatureRecord T in Data) str[n++] = $"On {T.Date} was {T.Temperature} degrees C.";
            return str;
        }

        public static TemperatureTracker Tracker;

        public TemperatureTracker() {
            Data = new List<TemperatureRecord>();
        }
    }
}
