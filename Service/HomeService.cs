using AzureBlobCMS.Data;
using AzureBlobCMS.Interfaces;
using HtmlAgilityPack;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private readonly WTTDbContext _wTTDbContext;
        #endregion

        #region Constructor 
        /// <summary>
        /// HomeService Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public HomeService(IConfiguration configuration, WTTDbContext wTTDbContext)
        {
            _configuration = configuration;
            _wTTDbContext = wTTDbContext;
        }
        #endregion

        #region Public Method
        public async Task<object> GetData(string bolbFile)
        {
            try
            {
                string response = await GetBlob("jsonconfig", bolbFile);
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

        public async Task<object> GetDataByLang(string bolbFile,string lang)
        {
            try
            {
                string response = await GetBlob("jsonconfig", bolbFile);
                var data = JObject.Parse(response);
                
               // var data = JsonConvert.DeserializeObject<dynamic>(response);
                if (data != null)
                {
                    if (string.IsNullOrEmpty(lang))
                    {
                        return data;
                    }
                    else
                    {
                        foreach (var obj in data)
                        {
                            if (obj.Key == lang)
                            {
                                return obj.Value;
                            }
                        }
                    }
                    return null;

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

        public async Task<bool> WriteData(string blobFile, Object home)
        {
            try
            {
                string data= home.ToString();
                string trimmeddata = ExtractHtmlInnerText(data);
                await UpdateBlob("jsonconfig", blobFile, trimmeddata);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> WriteDataByLang(string blobFile,string lang, Object home)
        {
            try
            {

                var inputdata = JObject.FromObject(home);         
                string response = await GetBlob("jsonconfig", blobFile);

                JObject blobData = JObject.Parse(response);

                var contents1 = home.ToString();
                JObject blobData1 = JObject.Parse(contents1);

                if (string.IsNullOrEmpty(lang))
                {
                    return false;
                }
                else
                {
                    foreach (var data in blobData)
                    {
                        if (data.Key == lang)
                        {
                            data.Value.Replace(blobData1);
                        }
                    }
                }


                //var data = JsonConvert.DeserializeObject<dynamic>(response);
                //if (lang == "eng")
                //{
                //    inputdata["Html"] = ExtractHtmlInnerText(inputdata["Html"].ToString());
                //   // home.eng.Html = ExtractHtmlInnerText(home.eng.Html);
                //    data.eng = inputdata;
                //}
                //else if (lang == "chn")
                //{
                //    inputdata["Html"] = ExtractHtmlInnerText(inputdata["Html"].ToString());
                //    // home.chn.Html = ExtractHtmlInnerText(home.chn.Html);
                //    data.chn = inputdata;
                //}
                //else
                //{
                //    return false;
                //}

                string updatedData = blobData.ToString();
                string trimmedData = ExtractHtmlInnerText(updatedData);
                await UpdateBlob("jsonconfig", blobFile, trimmedData);
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
        private async Task<bool> CheckBlobExist(string containerName, string fileName)
        {
            string connectionString = _configuration.GetValue<string>("AzureConn");

            // Setup the connection to the storage account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Connect to the blob storage
            CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();
            // Connect to the blob container
            CloudBlobContainer container = serviceClient.GetContainerReference($"{containerName}");
            // Check to the blob file
            return container.GetBlockBlobReference($"{fileName}").Exists();
            

        }

        private async Task UpdateBlob(string containerName, string fileName, string content)
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

        public async Task<object> CreateDynamicFile(string fileName, string moduleName)
        {
            var count = _wTTDbContext.Modules.Where(x => x.FileName == fileName).Count();
            if (count < 1)
            {
                if (!await CheckBlobExist("jsonconfig", fileName))
                {
                    using (StreamReader r = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "Utility", "Dynamic.json")))
                    {
                        string jsondata = await r.ReadToEndAsync();
                        await UpdateBlob("jsonconfig", fileName, jsondata);
                        var res = _wTTDbContext.Modules.Add(new Models.Modules()
                        {
                            FileName = fileName,
                            ModuleName = moduleName

                        });
                        _wTTDbContext.SaveChanges();
                        return jsondata;
                    }
                }
            }
            return null;
        }

        public async Task<object> GetModules()
        {
            try
            {
               return  _wTTDbContext.Modules.ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
