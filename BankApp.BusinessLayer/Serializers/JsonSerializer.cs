using BankApp.DataLayer.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using BankApp.BusinessLayer.Models;

namespace BankApp.BusinessLayer.Serializers
{
    class JsonSerializer : ISerializer
    {
        public void Serialize(string filePath, AccountStatement dataSet)
        {
            var jsonData = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }

        public string Serialize(Transfer dataSet)
        {
            return JsonConvert.SerializeObject(dataSet, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
    }
}
