using Microsoft.AspNetCore.Http.HttpResults;

namespace Instadvert.CZ.Data.Static
{
    public class Status
    {

        public const string Created = "Created"; // transaction is created
        public const string Failed = "Failed"; // transaction failed or cancelled
        public const string Success= "Success"; // company transfered money
        public const string Paid = "Paid"; // blogger received money

    }
}
