using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;

namespace TMS.Common.Helpers
{
    public class SmsHelper
    {
        private readonly IConfiguration _configuration;

        public SmsHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SmsResponseDto SendSms(string toNumber, string MessageText)
        {
            string apiToken = _configuration.GetSection("LifeTimsSms:Token").Value;
            string apiSecret = _configuration.GetSection("LifeTimsSms:Secret").Value;
            toNumber = toNumber.Remove(0, 1);
            toNumber = $"92{toNumber}";
            string Masking = "UmerKhan96";
            string jsonResponse = SendSMSPOST(apiToken, apiSecret, toNumber, Masking, MessageText);
            var obj = JsonConvert.DeserializeObject<SmsResponseDto>(jsonResponse);
            return obj;
        }

        public string SendSMSPOST(string apiToken, string apiSecret, string toNumber, string Masking, string MessageText)
        {
            string api = "https://lifetimesms.com/json";
            string parameters = "api_token=" + apiToken + "&api_secret=" + apiSecret + "&to=" + toNumber + "&from=" + Masking + "&message=" + MessageText;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(api);
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = " application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new System.IO.StreamWriter(httpWebRequest.GetRequestStream()))
            {
                parameters = "api_token=" + apiToken + "&api_secret=" + apiSecret + "&to=" + toNumber + "&from=" + Masking + "&message=" + MessageText;
                streamWriter.Write(parameters);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result.ToString();
            }
        }
    }

    public class Message
    {
        public string status { get; set; }
        public string messageid { get; set; }
        public string gsm { get; set; }
    }

    public class SmsResponseDto
    {
        public int status { get; set; }
        public string type { get; set; }
        public double totalprice { get; set; }
        public int totalgsm { get; set; }
        public double remaincredit { get; set; }
        public List<Message> messages { get; set; }
    }
}
