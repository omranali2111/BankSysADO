using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSysADO
{
    internal class Accounts
    {
        {
        public int AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public decimal Balance { get; set; }

        // Foreign key property
        public int UserId { get; set; }

        // Navigation property for the User
        public Users User { get; set; }
    }
}
}
