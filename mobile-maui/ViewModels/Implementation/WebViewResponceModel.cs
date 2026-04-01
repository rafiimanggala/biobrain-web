using System.Collections.Generic;

namespace BioBrain.ViewModels.Implementation
{
    public enum ResponseTypes
    {
        Glossary,
        SingleAnswer,
        MultiAnswer, 
        Error,
        Material,
        Area
    }

    public class WebViewResponseModel
    {
        public WebViewResponseModel()
        {
            ParsedResponse =new Dictionary<string, string>();
        }

        public ResponseTypes Type { get; set; }

        public Dictionary<string, string> ParsedResponse { get; set; }
    }
}