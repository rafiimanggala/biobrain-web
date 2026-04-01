using System;
using System.Collections.Generic;
using Common.Enums;

namespace Common
{
    public static class Settings
    {
	    public static event EventHandler<bool> DataUpdateChanged; 

        public static AppContentType ContentType = AppContentType.Demo;
        public static bool IsDemo => ContentType == AppContentType.Demo;

        public static int UpdateCheckPeriodMinutes = 60*24;
        public static string MonthSubscriptionPrice;
        public static string YearSubscriptionPrice;
        public static long MicrosMonthSubscriptionPrice;
        public static long MicrosYearSubscriptionPrice;

        private static bool isUpdateAvailable;
        public static bool IsUpdateAvailable
        {
	        get => isUpdateAvailable;
            set
            {
	            isUpdateAvailable = value;
	            OnDataUpdateChanged(isUpdateAvailable);
            }
        }

        public static void Init(float density)
        {
            Density = density;
        }

        public static readonly string DatabaseTypeProproperty = "DatabaseType";
        public static readonly string DemoValue = "demo";
        public static readonly string PromoGooglePlayUrl = "https://play.google.com/redeem?code=";
        public static readonly string RateAppTitle = "We're happy you love using Biobrain!";
        public const string ReviewApp = "Rate Biobrain";
        public const string RemindNextTime = "Remind me later";
        public const string DontRemind = "Don't remind me later";
        public const string Cancel = "Cancel";
        public const string Responce1 = "responce1";
        public const string Responce2 = "responce2";
        public const string Responce = "responce";
        public const string AttachmentFileName = "attachment.png";
        public static float Density { get; private set; }

#if Biology
        public const bool IsPeriodicTableVisible = false;
        public const string TiledImage = "background.png";
#elif Chemistry
        public const bool IsPeriodicTableVisible = true;
        public const string TiledImage = "";
#elif Physics
        public const bool IsPeriodicTableVisible = false;
        public const string TiledImage = "";
#endif

        public static readonly List<string> TopicNamesExtensions = new List<string>()
        {
            "pH",
        };

        private static void OnDataUpdateChanged(bool e)
        {
	        DataUpdateChanged?.Invoke(null, e);
        }
    }
}