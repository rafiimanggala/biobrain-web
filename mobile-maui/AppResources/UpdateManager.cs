using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using BioBrain.Extensions;
using BioBrain.Helpers;
using Common;
using Common.Enums;
using Common.ErrorHandling;
using Common.Interfaces;
using DAL;
using DAL.Models.Implementations;
using DAL.Models.Interfaces;
using DAL.Repositorys.Implementations;
using DAL.Repositorys.Interfaces;
using Unity;
using Microsoft.Maui.Controls;
using Device = Microsoft.Maui.Controls.Device;
using AppActions = Common.Enums.AppActions;

namespace BioBrain.AppResources
{
    public static class UpdateManager
    {
	    public static int FilesFailed = 0;
        private const string VersionPath = "BioBrain.AppResources.Version.data";
        private const string StructVersionPath = "BioBrain.AppResources.StructureVersion.data";
        private static readonly IProjectDataWorker Updater = DependencyService.Get<IProjectDataWorker>();
        private static readonly IFilesPath FilePaths = DependencyService.Get<IFilesPath>();
        //private static readonly IWebWorker WebWorker = DependencyService.Get<IWebWorker>(); 
        public static int AppStructureVersion => int.Parse(ResourceHelper.GetStructureVersion());
        public static int AppDataVersion
        {
            get
            {
                var version = Updater.GetDataStoredVersion();
                return int.Parse(string.IsNullOrEmpty(version) ? ResourceHelper.GetVersion() : version);
            }
        }

        private static readonly IErrorLog Logger = DependencyService.Get<IErrorLog>();
        private const int MaxTotalTrys = 10;

        public static bool InitialUpdateFiles()
        {
            if (!Updater.DemoDataBaseExist())
            {
                // Try bundled raw SQLite database first (MAUI migration path)
                var rawDbStream = ResourceHelper.GetResourceStream("BioBrain.AppResources.ProjectData.db3");
                if (rawDbStream != null)
                {
                    try
                    {
                        Logger.Log($"Extracting bundled demo database ({rawDbStream.Length} bytes)");
                        using (rawDbStream)
                        using (var fileStream = System.IO.File.Create(FilePaths.DemoDatabasePath))
                        {
                            rawDbStream.CopyTo(fileStream);
                        }
                        Logger.Log($"Demo database extracted to {FilePaths.DemoDatabasePath}");
                        return true;
                    }
                    catch (Exception e)
                    {
                        Logger.Log($"Failed to extract bundled DB: {e.Message}");
                    }
                }

                // Try original encrypted ProjectData.data
                var demoStream = ResourceHelper.GetResourceStream("BioBrain.AppResources.ProjectData.data");
                if (demoStream == null)
                {
                    Logger.Log("No demo data resource found — online-only mode");
                    return true;
                }
                demoStream.Dispose();

                UnzipDemoData();
                var versionStream = ResourceHelper.GetResourceStream(VersionPath);
                if (versionStream != null) Updater.StoreVersionFile(versionStream);
                var structStream = ResourceHelper.GetResourceStream(StructVersionPath);
                if (structStream != null) Updater.StoreStructureVersionFile(structStream);

                return true;
            }
            
            //var isDemo = App.Container.Resolve<IDatabaseRepository>().CheckIsDemoDatabase();

            //if (!isDemo && Device.RuntimePlatform != Device.iOS) return true;
            
            //if (!Updater.IsUpdateNeeded(AppDataVersion) && !Updater.IsStructUpdateNeeded(AppStructureVersion)) return true;

            //var backupRepository = new BackupRepository();
            ////Migrate user data if need
            //if (Updater.IsStructUpdateNeeded(AppStructureVersion))
            //    backupRepository.MigrateBackupData();

            //backupRepository.BackUpData();

            //Updater.DeleteExistingDemo();
            //UnzipDemoData();

            //backupRepository.RestoreData();

            //Updater.StoreVersionFile(ResourceHelper.GetResourceStream(VersionPath));
            //Updater.StoreStructureVersionFile(ResourceHelper.GetResourceStream(StructVersionPath));
            return true;

        }

        public static AppContentType GetLocalContentType()
        {
            return Updater.FullDataBaseExist() ? AppContentType.Full : AppContentType.Demo;
        }

        private static void UnzipDemoData()
        {
	        try
	        {
		        using (var zipStream = ResourceHelper.GetResourceStream("BioBrain.AppResources.ProjectData.data"))
		        {
			        using (
				        var decryptedStream = DependencyService.Get<IDecryption>()
					        .Def(zipStream, "A03E7952A16E46189E352984059F3DCD"))
			        {
				        Logger.Log($"DbStreamSize - {decryptedStream.Length}");

						var zip = DependencyService.Get<IZipResource>();
						zip.Unzip(decryptedStream, null);
						//UnzipFile(decryptedStream, null);
					}
		        }
	        }
	        catch (Exception e)
	        {
                Logger.Log(e.ToString());
                throw e;
	        }

	        bool isDemoDB = true;
            try
            {
	            using (var database = DependencyService.Get<ISQLite>().GetConnection())
	            {
		            var model = database
			            .Query<DatabaseDataModel>(
				            $"Select * From DatabaseData Where Property = '{Settings.DatabaseTypeProproperty}'")
			            .FirstOrDefault();
		            isDemoDB = (model?.Value ?? Settings.DemoValue) == Settings.DemoValue;
                    Logger.Log($"Database detected as {model?.Value}");
	            }
            }
            finally
            {
            }

            File.Move(FilePaths.DatabasePath, isDemoDB ? FilePaths.DemoDatabasePath : FilePaths.DatabasePath);
        }

		//private static void UnzipFile(Stream zipStream, string dstPath)
		//{
		//	if (File.Exists(FilePaths.DatabasePath) && new FileInfo(FilePaths.DatabasePath).Length > 1
		//		|| File.Exists(FilePaths.DemoDatabasePath) && new FileInfo(FilePaths.DemoDatabasePath).Length > 1)
		//		return;
		//	Logger.Log("DB Not Exist");

		//	if (string.IsNullOrEmpty(dstPath))
		//		dstPath = FilePaths.AppPath;
		//	Logger.Log($"Dst Path: {dstPath}");

		//	var zipPath = Path.Combine(dstPath, "tmp.zip");
		//	if (File.Exists(zipPath)) File.Delete(zipPath);
		//	using (var stream = File.OpenWrite(zipPath))
		//	{
		//		zipStream.CopyTo(stream);
		//		zipStream.Close();
		//	}
		//	GC.Collect();

		//	//ZipFile.ExtractToDirectory(zipPath, dstPath);

		//	var middle = 0;
		//	using (var zip = ZipFile.OpenRead(zipPath))
		//	{
		//		middle = zip.Entries.Count / 2;
		//		for (var i = 0; i < middle; i++)
		//		{
		//			var entry = zip.Entries[i];
		//			entry.ExportToFolder(dstPath);

		//		}
		//	}
		//	GC.Collect();

		//	using (var zip = ZipFile.OpenRead(zipPath))
		//	{
		//		for (var i = middle; i < zip.Entries.Count; i++)
		//		{
		//			var entry = zip.Entries[i];
		//			entry.ExportToFolder(dstPath);

		//		}
		//	}

		//	Logger.Log($"Zip finish. DbSize: {new FileInfo(FilePaths.DatabasePath).Length}");

		//	if (File.Exists(zipPath)) File.Delete(zipPath);
		//}

		public static bool IsSuitableStructure()
        {
            return !Updater.IsStructUpdateNeeded(AppStructureVersion);
        }

        public static bool IsUpdateNeed(int newVersion)
        {
            var databaseRepository = App.Container.Resolve<IDatabaseRepository>();
            return Updater.IsUpdateNeeded(newVersion) || databaseRepository.CheckIsFilesError();
        }

        public static async Task<bool> UpdateDatabase(IFirebaseFileModel databaseInfo, AppContentType contentType)
        {
            var result = true;
            try
            {
                var path = contentType == AppContentType.Demo ? FilePaths.DemoDatabasePath : FilePaths.DatabasePath;
                var downloadPath = Path.Combine(FilePaths.AppPath, "download.db3");
                var backupRepository = new BackupRepository(contentType);

                // Backup user data if DB exists
                if (File.Exists(path))
                {
	                Logger.Log("Update - Backup Data");
                    backupRepository.BackUpData();
                }

                Updater.DeleteFile(downloadPath);

                for (var i = 0; i < 3; i++)
                {
                    try
                    {
                        await App.Sleep(10);
                        Logger.Log($"Update - Download Data {DateTime.UtcNow}");
                        //Debug.WriteLine($"File: DB start: {DateTime.UtcNow}");
                        using (var worker = DependencyService.Resolve<IWebWorker>())
                        {
							worker.DownloadFile(databaseInfo.Url, downloadPath);
                        }
                        //Debug.WriteLine($"File: DB end: {DateTime.UtcNow}");
                        if (Updater.CheckFileSize(downloadPath, databaseInfo.Size))
                            throw new FileNotFoundException("File not downloaded");
                        break;
                    }
                    catch (Exception e)
                    {
                        //Debug.WriteLine($"File: Database i: {i}");
                        Logger.Log($"Update error: Download File:DataBase; Message:{e.ToString()};");
                        if(i>=2)
                            throw;
                    }
                }

                Logger.Log($"Update - Replace existing  {DateTime.UtcNow}");
                Updater.DeleteFile(path);
                Updater.MoveFile(downloadPath, path);

                // Restore user data if DB exists
                if (File.Exists(path))
                {
	                Logger.Log("Update - Restore Data");
	                backupRepository.RestoreData();
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Update error: File:DataBase; Message:{e.ToString()};");
                result = false;
            }
            return result;
        }

        public static void RestoreDbToDemo()
        {
            Settings.ContentType = AppContentType.Demo;
        }

        public static UpdateData CheckFiles(List<IFirebaseFileModel> filesList)
        {
	        var filesNotLoaded = 0;

            foreach (var file in filesList)
	        {
		        //Create file path
		        var shortFilePath = string.IsNullOrEmpty(file.Path)
			        ? file.Name
			        : Path.Combine(file.Path, file.Name);
		        var filePath = Path.Combine(FilePaths.HtmlFolderPath, shortFilePath);

		        //Check for file changed
		        if (Updater.CheckFileSize(filePath, file.Size))
			        filesNotLoaded++;
	        }

            return new UpdateData{FilesFailed = filesNotLoaded, IsSuccess = filesNotLoaded == 0};
        }

        public static async Task<UpdateData> UpdateFiles(List<IFirebaseFileModel> filesList, IProgress<int> progressCallback)
        {
            var fileCount = filesList.Count;
            var fileNum = 1d;
            var result = true;
            var startTime = DateTime.UtcNow;
            FilesFailed = 0;

            var workers = new List<IWebWorker>();
            for (int i = 0; i < 10; i++)
            {
	            var worker = DependencyService.Resolve<IWebWorker>();
	            worker.Id = i;
                workers.Add(worker);
            }

            try
            {
	            foreach (var file in filesList)
	            {

		            try
		            {
			            //Init progress callback
			            progressCallback.Report((int) (fileNum / fileCount * 100));
			            await App.Sleep(100);

                        //Create file path
                        var shortFilePath = string.IsNullOrEmpty(file.Path)
				            ? file.Name
				            : Path.Combine(file.Path, file.Name);
			            var filePath = Path.Combine(FilePaths.HtmlFolderPath, shortFilePath);

			            //Check for file changed
			            if (!Updater.CheckFileSize(filePath, file.Size)) continue;

			            //Delete file if exist
			            Updater.DeleteFile(filePath);

			            //Create directory if not exist
			            var directory = Path.GetDirectoryName(filePath);
			            Updater.CreateDiectory(directory);

			            IWebWorker worker;
			            while ((worker = workers.FirstOrDefault(x => !x.IsBusy)) == null)
			            {
				            await App.Sleep(100);
			            }

			            //Debug.WriteLine($"File: {file.Name} start: {DateTime.UtcNow}");
			            worker.DownloadFileInTask(file.Url, filePath);
			            //Debug.WriteLine($"File: {file.Name} end: {DateTime.UtcNow}");
			            //if (Updater.IsNeedUpdateFile(filePath, file.Size))
			            // throw new FileNotFoundException("File not downloaded");
		            }
		            catch (Exception e)
		            {
			            Logger.Log($"Update error: File:{file.Name}; Message:{e.Message};");
			            result = false;
		            }
		            finally
		            {
			            fileNum++;
		            }
	            }

	            while (workers.Any(x => x.IsBusy))
	            {
		            await App.Sleep(100);
	            }
	            var totalUpdateTime = DateTime.UtcNow - startTime;

	            return new UpdateData { UpdateProcessMetric = totalUpdateTime, IsSuccess = result, FilesFailed = FilesFailed };
            }
            finally
            {
                Logger.Log($"Update end - {DateTime.UtcNow - startTime} - fails - {FilesFailed}.");
                workers.ForEach(x => x.Dispose());
            }

        }

        public static void StoreVersion(string version, string structVersion)
        {
            if(!Updater.ReplaceFileText(FilePaths.DataVersionPath, version)) throw new UpdateFilesException(FilePaths.DataVersionPath);
            if(!Updater.ReplaceFileText(FilePaths.StructureVersionPath, version)) throw new UpdateFilesException(FilePaths.StructureVersionPath);
        }
    }
}