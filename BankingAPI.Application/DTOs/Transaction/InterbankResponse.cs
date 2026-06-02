using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAPI.Application.DTOs.Transaction
{

    public class InterbankResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string account_number { get; set; }
        public string account_name { get; set; }
        public int bank_id { get; set; }
    }

}
