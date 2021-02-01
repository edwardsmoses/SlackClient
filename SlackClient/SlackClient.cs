using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace SlackClient
{
    public partial class SlackClient
    {
        private readonly string AuthorName;
        private readonly Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();



        public SlackClient(string webHookURL, string authorName)
        {
            this.AuthorName = authorName;

            _uri = new Uri(webHookURL);
        }

        //Post a message
        public void PostMessage(string additionalInformation, Exception ex)
        {
            SlackPayload payload = new SlackPayload()
            {
                Text = $"🔥 {additionalInformation} 🔥",
                Attachments = new List<SlackPayload.Attach>()
            };

            payload.Attachments.AddRange(GetSlackAttachmentList(ex));

            PostMessage(payload);
        }


        private List<SlackPayload.Attach> GetSlackAttachmentList(Exception ex, List<SlackPayload.Attach> attachments = null)
        {
            if (attachments == null)
                attachments = new List<SlackPayload.Attach>();

            attachments.Add(GetSlackPayloadFromException(ex));

            //if exception has Inner Exception, add the Inner Exception to Slack Attachments
            if (ex.InnerException != null)
                attachments.AddRange(GetSlackAttachmentList(ex.InnerException, attachments));

            return attachments;
        }

        /// <summary>
        /// Get Slack Attach Payload from Exception Object
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private SlackPayload.Attach GetSlackPayloadFromException(Exception ex, bool isInnerException = false)
        {
            return new SlackPayload.Attach()
            {
                author_name = this.AuthorName,
                color = "#D00000",
                //* => means bold
                //[spacing]  \n => means new line
                text = $"* {(isInnerException ? "Inner" : string.Empty)} Exception:*   \n{ex.Message}    \n \n*StackTrace:*    \n{ex.StackTrace}  \n  \n*Data:*  \n{ex.Data}",
                fallback = ex.Message,
                pretext = ex.Source,
                title = ex.HelpLink,
            };
        }


        //Post a message using a Payload object
        private void PostMessage(SlackPayload payload)
        {
            try
            {
                string payloadJson = JsonConvert.SerializeObject(payload);
                using (WebClient client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    NameValueCollection data = new NameValueCollection();
                    data["payload"] = payloadJson;
                    var response = client.UploadValues(_uri, "POST", data);
                    string responseText = _encoding.GetString(response);
                }
            }
            catch
            {
                //don't do anything with the exception here in this Slack Client... 
            }

        }
    }
}
