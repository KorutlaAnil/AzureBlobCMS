using AzureBlobCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobCMS.Interfaces
{
    public interface IHome
    {
        Task<Object> GetData();
        Task<object> GetDataByLang(string lang);

        Task<bool> WriteData(Object home);

        Task<bool> WriteDataByLang(string lang, Object home);

    }
}
