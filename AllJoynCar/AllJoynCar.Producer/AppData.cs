using com.microsoft.Sample.AllJoynCar;
using System;
using Windows.Storage;

namespace AllJoynCar.Producer
{
    class AppData
    {
        private const string speedKey = "Speed";
        private const string isMovingKey = "isMoving";
        private const string directionKey = "direction";
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static uint Speed
        {
            get
            {
                if (!localSettings.Values.ContainsKey(speedKey))
                    localSettings.Values[speedKey] = 0;

                return (uint)localSettings.Values[speedKey];
            }
            set
            {
                if (Speed != value)
                {
                    localSettings.Values[speedKey] = value;
                    SpeedPropertyDataChanged?.Invoke(new object(), new EventArgs());
                }
            }
        }

        public static EventHandler SpeedPropertyDataChanged { get; private set; }

        public static AllJoynCarState State
        {
            get
            {
                if (!localSettings.Values.ContainsKey(isMovingKey))
                    localSettings.Values[isMovingKey] = false;
                if (!localSettings.Values.ContainsKey(directionKey))
                    localSettings.Values[directionKey] = 0;

                AllJoynCarState mv = new AllJoynCarState();
                mv.Value1 = (bool)localSettings.Values[isMovingKey];
                mv.Value2 = (uint)localSettings.Values[directionKey];

                return mv;
            }
            set
            {
                if (State != value)
                {
                    localSettings.Values[isMovingKey] = value.Value1;
                    localSettings.Values[directionKey] = value.Value2;
                    EventHandler handler = statePropertyDataChanged;
                    handler?.Invoke(new object(), new EventArgs());
                }
            }
        }

        public static EventHandler statePropertyDataChanged { get; private set; }
    }
}
