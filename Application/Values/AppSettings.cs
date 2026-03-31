namespace BiobrainWebAPI.Values
{
	public class AppSettings
	{
		public static string StaticFolderLink => "static";
		public static string FontsFolderLink => "fonts";
		public static string RootFolderLink => "wwwroot";
		public static string ImagesFolderLink => "images";
        public static string UserGuideImagesFolderLink => "user-guide-images";
        public static string ContentFolderLink => "content";
		public static string ReportFolderLink => "report";
        public static string IconsFolderLink => "Icons";
        public static string AppIconLink => "app_icon.png";
		public static int QuizQuestionNumber => 10;
        public static int FreeTrialDays => 7;
		// Should be less then 24 (subscription mechanism can't work with more then two years)
        public static int AccessCodeMonth => 15;
        public static int MinutesForNewSession => 10;
        public static int UsageReportPagesCount => 5;
        public static int ShortTextSymbolsNumber => 100;

		public static class Students
		{
			public static string DefaultCountry => "Australia";
			public static string DefaultState => "Victoria";
			public static int DefaultCurriculumCode => 1;
		}
	}
}
