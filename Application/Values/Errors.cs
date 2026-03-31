namespace BiobrainWebAPI.Values
{
	public static class Errors
	{
		public static string UserNamePasswordInvalid => "The username/password couple is invalid.";
		public static string InvalidRefreshToken => "The refresh token is no longer valid.";
		public static string UserCantLogin => "The user is no longer allowed to sign in.";
		public static string InvalidSubject => "Invalid userId in subject.";
        public static string NoAccessCodeForClass => "You haven't got Access Code for this class";
        public static string StudentNotInSchool => "You are not added to the school students list. Please ask your teacher invite you by email from class admin page.";
    }
}