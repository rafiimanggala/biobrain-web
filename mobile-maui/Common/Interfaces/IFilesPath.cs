using System;

namespace BioBrain.AppResources
{
    public interface IFilesPath
    {
        string DatabasePath { get; }
        string DemoDatabasePath { get; }

        string AppPath { get; }

        string IconsPath { get; }

        string ImagesPath { get; }

        string PagesPath { get; }

        string MarketUrl { get; }
        string DemoMarketUrl { get; }

        string AvatarPath { get; }
        string HtmlFolderPath { get; }

        string DataVersionPath { get; }
        string StructureVersionPath { get; }
        string InternetCache { get; }
    }
}