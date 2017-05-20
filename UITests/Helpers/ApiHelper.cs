using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace UITests.Helpers
{
    public class ApiHelper
    {
        private void GetData(string campaignGroupId, string accessToken)
        {
            string campaignUri =
                string.Format(
                    "https://graph.facebook.com/v2.3/{0}/?fields=name,id,account_id,objective,campaign_group_status,buying_type,spend_cap&access_token={1}",
                    campaignGroupId, accessToken);

            HttpWebRequest webRequest = CreateRequest(campaignUri, null, "",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            var webResponse = (HttpWebResponse) webRequest.GetResponse();
            string strResponse = GetResponseString(webResponse);
            DeserializeData(strResponse);
            webResponse.Close();
        }

        private void DeserializeData(string strResponse)
        {
            dynamic jsonResponse = JsonConvert.DeserializeObject(strResponse);
            /*  var campaignGroup = new CampaignGroup()
            {
                Objective = jsonResponse.objective,
                Campaign_Group_Status = jsonResponse.campaign_group_status,
                Buying_Type = jsonResponse.buying_type
            };
            CurrentScenario<CampaignGroup>.Value = campaignGroup; */
        }

        private string GetResponseString(HttpWebResponse webResponse)
        {
            // Get full response
            string strResponse = "";
            Stream receiveStream = webResponse.GetResponseStream();
            Encoding encode = Encoding.GetEncoding("utf-8");
            var readStream = new StreamReader(receiveStream, encode);
            var read = new Char[256];
            int count = readStream.Read(read, 0, 256);
            while (count > 0)
            {
                // Dump the 256 characters on a string
                var str = new String(read, 0, count);
                strResponse += str;
                count = readStream.Read(read, 0, 256);
            }
            readStream.Close();

            return strResponse;
        }

        private HttpWebRequest CreateRequest(string uri, string postData, string contentType = "",
            string acceptContent = "", bool xrequest = false, string referer = "")
        {
            // Create web request and set params
            var webRequest = (HttpWebRequest) WebRequest.Create(uri);
            var cookies = new CookieCollection();

            // Set common params
            webRequest.KeepAlive = true;
            webRequest.AllowAutoRedirect = false;
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";

            // Add accept content if specified
            if (!string.IsNullOrEmpty(acceptContent))
                webRequest.Accept = acceptContent;

            // Add content type if specified
            if (!string.IsNullOrEmpty(contentType))
                webRequest.ContentType = contentType;

            // Add specific headers
            if (xrequest)
                webRequest.Headers["x-requested-with"] = "XMLHttpRequest";

            // Add referer if specified
            if (!string.IsNullOrEmpty(referer))
                webRequest.Referer = referer;

            // Set post or get depending on data passed
            if (string.IsNullOrEmpty(postData))
            {
                // Set request method
                webRequest.Method = "GET";
            }
            else
            {
                // Set request method
                webRequest.Method = "POST";

                // Create byte array for post data
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                webRequest.ContentLength = byteArray.Length;

                // Write post data to request
                Stream dataStream = webRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            // Add any cookies returned from previous requests
            webRequest.CookieContainer = new CookieContainer();
            webRequest.CookieContainer.Add(cookies);

            // Return request
            return webRequest;
        }
    }
}