using Newtonsoft.Json;
using System.Collections.Generic;

namespace SlackClient
{
    public partial class SlackClient
    {
        public class SlackPayload
        {
            [JsonProperty("text")]
            public string Text { get; set; }


            [JsonProperty("attachments")]
            public List<Attach> Attachments { get; set; }

            public class Attach
            {
                public string fallback { get; set; }
                public string color { get; set; }
                public string pretext { get; set; }
                public string author_name { get; set; }
                public string title { get; set; }
                public string text { get; set; }
            }
        }
    }

}
