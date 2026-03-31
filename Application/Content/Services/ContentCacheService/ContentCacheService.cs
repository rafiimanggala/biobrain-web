using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Biobrain.Application.Content.Internal;
using Biobrain.Application.Interfaces.DataAccess;
using Biobrain.Application.Specifications;
using BiobrainWebAPI.Values;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Biobrain.Application.Content.Services.ContentCacheService
{
	internal class ContentCacheService : IContentCacheService
	{
		private const int FilesToSaveNum = 3;
		private const int DaysToSave = 7;

		private readonly IDb _db;
		private readonly IContentTreeDataBuilder _contentTreeDataBuilder;
		private readonly IContentPageDataBuilder _contentPageDataBuilder;
		private readonly IContentQuizDataBuilder _contentQuizDataBuilder;
		private readonly IContentGlossaryTermBuilder _contentGlossaryTermBuilder;
		private readonly IConfiguration _configuration;

		public ContentCacheService(IDb db,
			IContentTreeDataBuilder contentTreeDataBuilder,
			IContentPageDataBuilder contentPageDataBuilder,
			IContentQuizDataBuilder contentQuizDataBuilder,
			IContentGlossaryTermBuilder contentGlossaryTermBuilder,
			IConfiguration configuration)
		{
			_db = db;
			_contentTreeDataBuilder = contentTreeDataBuilder;
			_contentPageDataBuilder = contentPageDataBuilder;
			_contentQuizDataBuilder = contentQuizDataBuilder;
			_contentGlossaryTermBuilder = contentGlossaryTermBuilder;
			_configuration = configuration;
		}



		public async Task<string> CreateContentFile(Guid courseId, long dateTime, CancellationToken cancellationToken)
		{
			var fileName = GetFileName(courseId, dateTime);
			var content = await GetFileContent(courseId,cancellationToken);
			var filePath = Path.Combine(ContentFolderPath, fileName);
			await File.WriteAllTextAsync(filePath, content, cancellationToken);
			return fileName;
		}

		public async Task<ContentCacheModel> GetLatestContentFromFile(Guid courseId,CancellationToken cancellationToken)
		{
			// TODO: Load content for all courses available for student (user) based on user - class - course relation
			var version = await _db.ContentVersion
				.Where(x => x.CourseId == courseId)
				.OrderByDescending(x => x.Version)
				.FirstOrDefaultAsync(cancellationToken);
			
			if(string.IsNullOrEmpty(version.ContentFileName))
				version.ContentFileName = GetFileName(courseId, version.Version);
			
			var filePath = Path.Combine(ContentFolderPath, version.ContentFileName);
			if (!File.Exists(filePath))
			{
				version.ContentFileName = await CreateContentFile(courseId, version.Version, cancellationToken);
				await _db.SaveChangesAsync(cancellationToken);
				filePath = Path.Combine(ContentFolderPath, version.ContentFileName);
			}
			return JsonConvert.DeserializeObject<ContentCacheModel>(await File.ReadAllTextAsync(filePath, cancellationToken));
		}

		public async Task<int> DeleteOldFiles(CancellationToken cancellationToken = new())
		{
			var filesToSave = await _db.ContentVersion
				.OrderByDescending(x => x.Version)
				.ToListAsync(cancellationToken);
			
			var folderPath = Path.Combine(ContentFolderPath, AppSettings.ContentFolderLink);
            if (!Directory.Exists(folderPath)) return 0;

			var filesToDelete = Directory.GetFiles(folderPath, "*.json")
				.Where(x => filesToSave.All(fts => fts.ContentFileName != Path.GetFileName(x)) &&
				            (DateTime.UtcNow - File.GetCreationTimeUtc(x)).TotalDays > DaysToSave).ToList();
			foreach (var fileName in filesToDelete)
			{
				File.Delete(Path.Combine(folderPath, fileName));
			}

			return filesToDelete.Count;
		}

		private async Task<string> GetFileContent(Guid courseId, CancellationToken cancellationToken)
		{
			var subjectCode = await _db.Courses.Where(CourseSpec.ById(courseId)).Select(_ => _.SubjectCode)
				.FirstOrDefaultAsync(cancellationToken);
			return JsonConvert.SerializeObject(new ContentCacheModel()
			{
				Quizzes = await _contentQuizDataBuilder.Build(_db.Quizzes.Include(_ => _.ContentTreeNode)
						.Where(QuizSpec.ForCourse(courseId)),
					cancellationToken),
				Pages = await _contentPageDataBuilder.Build(_db.Pages.Include(_ => _.ContentTreeNode)
						.Where(PageSpec.ForCourse(courseId)),
					cancellationToken),
				ContentForest = await _contentTreeDataBuilder.Build(_db.ContentTree
						.Where(ContentTreeSpec.ForCourse(courseId)),
					cancellationToken),
				GlossaryTerms = await _contentGlossaryTermBuilder.Build(_db.Terms.Where(TermSpec.ForSubject(subjectCode)), cancellationToken)
			});
		}

		private string GetFileName(Guid courseId, long dateTime) => $"{courseId}-{dateTime}.json";

        private string ContentFolderPath => Path.Combine(Directory.GetCurrentDirectory(),
            _configuration.GetSection(ConfigurationSections.StaticFolder).Value, AppSettings.ContentFolderLink);
    }
}