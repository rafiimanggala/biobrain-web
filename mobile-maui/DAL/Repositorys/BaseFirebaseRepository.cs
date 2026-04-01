using System.Threading.Tasks;
using Firebase.Storage;
using Firebase.Xamarin.Auth;
using Firebase.Xamarin.Database;

namespace DAL.Repositorys
{
    public abstract class BaseFirebaseRepository
    {
#if Biology

#if DEBUG
        // Test Biology
#if US
        private const string WebApiKey = "AIzaSyCuZ7yaz4HoJFvrotX58Sek9uu16zZS0G8";
        private const string StorageUrl = "biobrainustesting.appspot.com";
        private const string DatabaseUrl = "https://biobrainustesting.firebaseio.com/";
#else
        //private const string WebApiKey = "AIzaSyDzCV-fzxXLOdErkkomQV2Antm2k7vdTsI";
        //private const string StorageUrl = "biobrain-6187e.appspot.com";
        //private const string DatabaseUrl = "https://biobrain-6187e.firebaseio.com/";

        private const string WebApiKey = "AIzaSyAW7-jYDK3u28NfKAQ6thM1aNJG2ad3syA";
        private const string StorageUrl = "cecbiobrain.appspot.com";
        private const string DatabaseUrl = "https://cecbiobrain.firebaseio.com/";
#endif
#else
        // Live Biology
#if US
        private const string WebApiKey = "AIzaSyBb4BjnHTiypJ5hmqVa3EsCFOiHDNF7mHo";
        private const string StorageUrl = "biobrain-biology-us.appspot.com";
        private const string DatabaseUrl = "https://biobrain-biology-us.firebaseio.com/";
#else
        private const string WebApiKey = "AIzaSyAW7-jYDK3u28NfKAQ6thM1aNJG2ad3syA";
        private const string StorageUrl = "cecbiobrain.appspot.com";
        private const string DatabaseUrl = "https://cecbiobrain.firebaseio.com/";
#endif
#endif
#elif Chemistry
#if DEBUG
#if US
        // Test
        //private const string WebApiKey = "AIzaSyAASdxxEEz5MLovc1WptObAVNW3UQoTzoQ";
        //private const string StorageUrl = "chemistrytest-49c6f.appspot.com";
        //private const string DatabaseUrl = "https://chemistrytest-49c6f.firebaseio.com/";
        // Live
        //private const string WebApiKey = "AIzaSyCzbcuquRG-aggqpybsDfLwA2cEnucGLI4";
        //private const string StorageUrl = "biobrain-chemistry-us.appspot.com";
        //private const string DatabaseUrl = "https://biobrain-chemistry-us.firebaseio.com/";
        //Live not US
        private const string WebApiKey = "AIzaSyAEpxKgrk9t9cocw245aQChIR0-zWJsQbg";
        private const string StorageUrl = "cec-chemistry.appspot.com";
        private const string DatabaseUrl = "https://cec-chemistry.firebaseio.com/";
#else
        // Test Chemistry
        private const string WebApiKey = "AIzaSyAASdxxEEz5MLovc1WptObAVNW3UQoTzoQ";
        private const string StorageUrl = "chemistrytest-49c6f.appspot.com";
        private const string DatabaseUrl = "https://chemistrytest-49c6f.firebaseio.com/";
#endif
#else
#if US
        //private const string WebApiKey = "AIzaSyCzbcuquRG-aggqpybsDfLwA2cEnucGLI4";
        //private const string StorageUrl = "biobrain-chemistry-us.appspot.com";
        //private const string DatabaseUrl = "https://biobrain-chemistry-us.firebaseio.com/";
		// Use from not us chemistry because now all users in default chemistry
        private const string WebApiKey = "AIzaSyAEpxKgrk9t9cocw245aQChIR0-zWJsQbg";
        private const string StorageUrl = "cec-chemistry.appspot.com";
        private const string DatabaseUrl = "https://cec-chemistry.firebaseio.com/";
#else
        // Live Chemistry
        private const string WebApiKey = "AIzaSyAEpxKgrk9t9cocw245aQChIR0-zWJsQbg";
        private const string StorageUrl = "cec-chemistry.appspot.com";
        private const string DatabaseUrl = "https://cec-chemistry.firebaseio.com/";
#endif
#endif
#elif Physics
#if DEBUG
#if US
	    private const string WebApiKey = "AIzaSyBJ8rXvj96jrXw21JAosjAU94irVoIG9JM";
	    private const string StorageUrl = "biobrain-physics-us.appspot.com";
	    private const string DatabaseUrl = "https://biobrain-physics-us.firebaseio.com/";
#else
	    // Live Physics
	    private const string WebApiKey = "AIzaSyBjsQhIGWkUFBkrBQ-De0rtN3uKiQBmjYc";
	    private const string StorageUrl = "biobrain-physics.appspot.com";
	    private const string DatabaseUrl = "https://biobrain-physics.firebaseio.com/";
#endif
#else
#if US
        private const string WebApiKey = "AIzaSyBJ8rXvj96jrXw21JAosjAU94irVoIG9JM";
        private const string StorageUrl = "biobrain-physics-us.appspot.com";
        private const string DatabaseUrl = "https://biobrain-physics-us.firebaseio.com/";
#else
        // Live Physics
        private const string WebApiKey = "AIzaSyBjsQhIGWkUFBkrBQ-De0rtN3uKiQBmjYc";
        private const string StorageUrl = "biobrain-physics.appspot.com";
        private const string DatabaseUrl = "https://biobrain-physics.firebaseio.com/";
#endif
#endif
#endif

        protected FirebaseAuthProvider AuthProvider = new FirebaseAuthProvider(new FirebaseConfig(WebApiKey));
        protected FirebaseClient Client = new FirebaseClient(DatabaseUrl);
        protected static readonly FirebaseStorage Storage = new FirebaseStorage(StorageUrl, new FirebaseStorageOptions
        {
            AuthTokenAsyncFactory = GetToken,
            ThrowOnCancel = true
        });

        protected const string Email = "B93BE8AD-5BAB-49FC-B89E-E118A9A6882A@m.com";
        protected static FirebaseAuthLink AuthData;
        protected static string AvatarFolder = "UserAvatars";

        public const string UsersField = "Users";
        public const string ReasonsField = "reasons";
        public const string PurchasesField = "Purchases";
        public const string PurchaseHistoryField = "History";
        public const string DataInfoField = "DataInfo";
        public const string IsGetMetrics = "IsGetMetrics";
        public const string MinimumAppVersion = "MinimumAppVersion";
        public const string MinimumIosAppVersion = "MinimumIosAppVersion";
        public const string MinimumAppVersionMessage = "MinimumAppVersionMessage";
        public const string DemoFilesListField = "DemoFilesList";
        public const string DataVersionField = "DataVersion";
        public const string StructureVersionField = "StructureVersion";
        public const string DemoDatabaseInfoField = "DemoDatabaseInfo";
        public const string FullFilesListField = "FullFilesList";
        public const string FullDatabaseInfoField = "FullDatabaseInfo";
        public const string Logs = "Logs";
        public const string Reviews = "reviews";
        public const string VersionsField = "Versions";
        public const string AppVersionField = "AppVersion";

        private static Task<string> GetToken()
        {
            return Task.FromResult(AuthData.FirebaseToken);
        }
    }
}