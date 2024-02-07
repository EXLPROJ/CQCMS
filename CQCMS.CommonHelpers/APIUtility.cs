using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CQCMS.CommonHelpers
{

    public class APIUtility<T>
    {
        private string _InvokerName;
        public APIUtility(string InvokerName)
        {

            _InvokerName = InvokerName;
        }
        public async Task<T> GetResponse(string url, HttpMethod httpMethod, string content = null)
        {


            HttpResponseMessage response;
            try
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.UseDefaultCredentials = true;
                using (var httpClient = new HttpClient(httpClientHandler))
                {

                    //bttpClient .DefaultRequestHeaders.Add("Authorization", $"Basic {GetToken()}");
                    httpClient.DefaultRequestHeaders.Add("InvokerName", _InvokerName);
                    if (httpMethod == HttpMethod.Get)
                    {

                        response = await httpClient.GetAsync(url);
                    }
                    else
                    {
                        response = await httpClient.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));
                    }
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {

                        throw new Exception("Unauianized. Please verify API details");
                    }
                    if (response.IsSuccessStatusCode)

                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<T>(apiResponse);
                        return data;
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<T> GetResponseFromSaveNewEmailAndCreateCase(string url, HttpMethod httpMethod, string content = null)
        {
            HttpResponseMessage response;
            try
            {
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.UseDefaultCredentials = true;
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    //bttpClient .DefaultRequestHeaders.Add("Authorization", $"Basic {GetToken()}");
                    if (httpMethod == HttpMethod.Get)
                    {
                        response = await httpClient.GetAsync(url);
                    }
                    else
                    {
                        response = await httpClient.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));
                    }

                    string apiResponse = await response.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<T>(apiResponse);

                    if (!response.IsSuccessStatusCode)
                        throw new ApplicationException("Message: " + response.ReasonPhrase + " Status: " + response.StatusCode);
                    else
                        return data;

                }
            }
            catch (Exception ex)
            {

                throw ex;

            }
        }
    }


}
