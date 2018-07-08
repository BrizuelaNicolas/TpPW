using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using TpPW.Models;

namespace TpPW.Models.Entities
{
    public class Captcha
    {
        public bool Success { get; set; }
        public List<string> ErrorCodes { get; set; }

        public static bool Validate(string encodedResponse)
        {
            var result = false;
            var captchaResponse = encodedResponse;

            if (string.IsNullOrEmpty(encodedResponse)) return false;
            var client = new System.Net.WebClient();

            var secret = "6Leyol8UAAAAAAPsqeYil0NqhtGCQubxaG3kXmhK";
            if (string.IsNullOrEmpty(secret)) return false;
                        
            var googleReply = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
            var requestUri = string.Format(googleReply, secret, captchaResponse);
            var request = (HttpWebRequest)WebRequest.Create(requestUri);

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    JObject jResponse = JObject.Parse(stream.ReadToEnd());
                    var isSuccess = jResponse.Value<bool>("success");
                    result = (isSuccess) ? true : false;
                }
            }

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();      


            return result;
        }
    }
}
