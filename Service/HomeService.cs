using AzureBlobCMS.Interfaces;
using HtmlAgilityPack;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzureBlobCMS.Service
{
    public class HomeService : IHome
    {
        #region Private Variables
        /// <summary>
        /// IConfiguration to get configuration values from appsettings.json
        /// </summary>
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor 
        /// <summary>
        /// HomeService Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public HomeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Public Method
        public async Task<object> GetData()
        {
            try
            {
                string response = await GetBlob("jsonconfig", "Anil.json");
                var home = JsonConvert.DeserializeObject<dynamic>(response);
                if (home != null)
                {
                    return home;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<object> GetDataByLang(string lang)
        {
            try
            {
                string response = await GetBlob("jsonconfig", "Anil.json");
                var res = JObject.Parse(response);
                
                var data = JsonConvert.DeserializeObject<dynamic>(response);
                if (data != null)
                {
                    if (lang == "eng")
                    {
                        return data.eng; 
                       
                    }
                    else if (lang == "chn")
                    {
                        return data.chn;
                    }
                    else
                    {
                        return null;
                    }
                   
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> WriteData(Object home)
        {
            try
            {
                string data= JsonConvert.SerializeObject(home);
                await UpdateBlob("jsonconfig", "Anil.json", data);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> WriteDataByLang(string lang, Object home)
        {
            try
            {
                var inputdata = JObject.FromObject(home);         
                string response = await GetBlob("jsonconfig", "Anil.json");
                var data = JsonConvert.DeserializeObject<dynamic>(response);
                if (lang == "eng")
                {
                    inputdata["Html"] = ExtractHtmlInnerText(inputdata["Html"].ToString());
                   // home.eng.Html = ExtractHtmlInnerText(home.eng.Html);
                    data.eng = inputdata;
                }
                else if (lang == "chn")
                {
                    inputdata["Html"] = ExtractHtmlInnerText(inputdata["Html"].ToString());
                    // home.chn.Html = ExtractHtmlInnerText(home.chn.Html);
                    data.chn = inputdata;
                }
                else
                {
                    return false;
                }
                string updatedData = JsonConvert.SerializeObject(data);
                await UpdateBlob("jsonconfig", "Anil.json", updatedData);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Private Methods
        private async Task<string> GetBlob(string containerName, string fileName)
        {
            string connectionString = _configuration.GetValue<string>("AzureConn");

            // Setup the connection to the storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Connect to the blob storage
            CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();
            // Connect to the blob container
            CloudBlobContainer container = serviceClient.GetContainerReference($"{containerName}");
            // Connect to the blob file
            CloudBlockBlob blob = container.GetBlockBlobReference($"{fileName}");
            // Get the blob file as text
            string contents =await blob.DownloadTextAsync();


            return contents;
        }
        private async Task UpdateBlob(string containerName, string fileName,string content)
        {
            string connectionString = _configuration.GetValue<string>("AzureConn");

            // Setup the connection to the storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Connect to the blob storage
            CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();
            // Connect to the blob container
            CloudBlobContainer container = serviceClient.GetContainerReference($"{containerName}");
            // Connect to the blob file
            CloudBlockBlob blob = container.GetBlockBlobReference($"{fileName}");
            // Upload the blob file as text
            await blob.UploadTextAsync(content);

        }

        private  string ExtractHtmlInnerText(string htmlText)
        {
          
            Regex regex = new Regex("(<.*?>\\s*)+", RegexOptions.Singleline);

            string resultText = regex.Replace(htmlText, " ").Trim();

            return resultText;
        }
        #endregion
    }
}
