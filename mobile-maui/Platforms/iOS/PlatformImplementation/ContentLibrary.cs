using System.Linq;
using ClassKit;
using DAL.Repositorys.Implementations;

namespace BioBrain.Platforms.iOS.PlatformImplementation
{
    public class ContentLibrary : CLSDataStoreDelegate
    {
        private readonly AreasRepository areasRepository;
        private readonly TopicsRepository topicsRepository;
        private readonly MaterialsRepository materialsRepository;
        private readonly LevelTypesRepository levelsRepository;

        public ContentLibrary()
        {
            areasRepository = new AreasRepository();
            topicsRepository = new TopicsRepository();
            materialsRepository = new MaterialsRepository();
            levelsRepository = new LevelTypesRepository();
        }

        public void SetupClassKit()
        {
            CLSDataStore.Shared.Delegate = this;
        }

        public override CLSContext CreateContext(string identifier, CLSContext parentContext, string[] parentIdentifierPath)
        {
            var identifierPath = parentIdentifierPath.Concat(new[] { identifier }).ToArray();

            switch (identifierPath.Length)
            {
                case 1:
                    int.TryParse(identifierPath[0], out var areaId);
                    var area = areasRepository.GetByID(areaId);
                    var areaContext = new CLSContext(CLSContextType.Chapter, area.AreaID.ToString(), area.AreaName);
                    return areaContext;
                case 2:
                    int.TryParse(identifierPath[1], out var topicId);
                    var topic = topicsRepository.GetByID(topicId);
                    var topicContext = new CLSContext(CLSContextType.Section, topic.TopicID.ToString(), topic.TopicName);
                    return topicContext;
                case 3:
                    int.TryParse(identifierPath[2], out var materialId);
                    var material = materialsRepository.GetByID(materialId);
                    var topicModel = topicsRepository.GetByID(material.TopicID);
                    var level = levelsRepository.GetByID(material.LevelTypeID);
                    var quizContext = new CLSContext(CLSContextType.Quiz, material.MaterialID.ToString(), $"{topicModel.TopicName} - {level.LevelName}");
                    return quizContext;
                default: return null;
            }
        }
    }
}
