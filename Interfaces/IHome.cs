using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobCMS.Interfaces
{
    public interface IHome
    {
        Task<Object> GetData(string blobFile);
        Task<object> GetDataByLang(string bolbFile,string lang);

        Task<bool> WriteData(string blobFile,Object home);

        Task<bool> WriteDataByLang(string bolbFile,string lang, Object home);
        Task<object> CreateDynamicFile(string fileName, string moduleName);
        Task<object> GetModules();

    }
}
