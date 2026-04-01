using System;
using System.Collections.Generic;
using Common;
using Common.Enums;
using DAL.Models.Interfaces;

namespace DAL.Repositorys.Implementations
{
    public class BackupRepository
    {
        private readonly AppContentType contentType;

        private List<IDoneAreaModel> areas;
        private List<IDoneTopicModel> topics;
        private List<IDoneMaterialModel> materials;
        private List<IDoneQuestionModel> questions;
        //private IAccountModel account;

        private DoneAreasRepository doneAreasRepository;
        private DoneTopicsRepository doneTopicsRepository;
        private DoneMaterialsRepository doneMaterialsRepository;
        private DoneQuestionRepository doneQuestionRepository;
        //private readonly AccountRepository accountRepository = new AccountRepository();
        
        public BackupRepository()
        {
            this.contentType = Settings.ContentType;
            InitData();
        }

        public BackupRepository(AppContentType contentType)
        {
            this.contentType = contentType;
            InitData();
        }

        private void InitData()
        {
            doneAreasRepository = new DoneAreasRepository(contentType);
            doneTopicsRepository = new DoneTopicsRepository(contentType);
            doneMaterialsRepository = new DoneMaterialsRepository(contentType);
            doneQuestionRepository = new DoneQuestionRepository(contentType);
        }

        public void BackUpData()
        {
            areas = doneAreasRepository.GetAll();
            topics = doneTopicsRepository.GetAll();
            try
            {
                materials = doneMaterialsRepository.GetAll();
            }
            catch (Exception)
            {
                materials = new List<IDoneMaterialModel>();
                doneMaterialsRepository.Clean();
            }
            questions = doneQuestionRepository.GetAll();
            //try
            //{
            //    account = accountRepository.GetAccountModel();
            //}
            //catch(Exception e)
            //{
            //    var a = e;
            //    // ignored
            //}
        }

        public void RestoreData()
        {
            if(areas != null)
	            foreach (var area in areas)
	            {
	                doneAreasRepository?.Insert(area);
	            }

            if(topics != null)
	            foreach (var topic in topics)
	            {
	                doneTopicsRepository?.Insert(topic);
	            }

            if(topics != null)
	            foreach (var material in materials)
	            {
	                doneMaterialsRepository?.Insert(material);
	            }

            if(questions != null)
	            foreach (var question in questions)
	            {
	                doneQuestionRepository?.Insert(question);
	            }
            //if(account != null)
            //    accountRepository.Insert(account);
        }

        /// <summary>
        /// Migrates data that has been Backuped
        /// </summary>
        public void MigrateBackupData()
        {
            //DeleteDnaAndRnaTopicResults();
        }

        //private void DeleteDnaAndRnaTopicResults()
        //{
        //    topics.RemoveAll(t => t.TopicID == 1);
        //    materials.RemoveAll(m => m.MaterialID >= 1 && m.MaterialID <= 3);
        //    questions.RemoveAll(q => q.QuestionID >= 1 && q.QuestionID <= 30);
        //}
    }
}