using AzureBlobCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobCMS.Interfaces
{
    public interface IHome
    {
        Task<Home> GetData();
        Task<Home> GetDataByLang(string lang);

        Task<bool> WriteData(Home home);

        Task<bool> WriteDataByLang(string lang, Home home);

    }
}
