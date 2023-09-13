using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace BankSysADO
{
    internal class Transactions
    {
        public int TransId { get; set; }
        public DateTime Timestamp { get; set; }
        public int Type { get; set; }
        public decimal Amount { get; set; }
        public int? SrcAccNO { get; set; }
        public int? TargetAccNO { get; set; }

        // Foreign key property
        public int Acc_Number { get; set; }

        // Navigation property for the Account
        public Accounts Account { get; set; }
    }
}
