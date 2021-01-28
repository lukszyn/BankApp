using BankApp.BusinessLayer.Models;
using BankApp.DataLayer.Models;
using System.Collections.Generic;

namespace BankApp.BusinessLayer.Serializers
{
    interface ISerializer
    {
        void Serialize(string filePath, AccountStatement dataSet);
        string Serialize(Transfer dataSet);
    }
}
