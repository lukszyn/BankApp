using BankApp.BusinessLayer.Models;
using BankApp.BusinessLayer.Serializers;
using System.IO;

namespace BankApp.BusinessLayer
{
    public class FilesService
    {
        public bool ExportToFile(string path, AccountStatement data)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            var filePath = Path.Combine(path, $"account_statement.json");

            new JsonSerializer().Serialize(filePath, data);

            return true;

        }
    }
}
