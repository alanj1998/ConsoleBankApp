using SSD.Lib;

namespace SSD.Models
{
    public class BankAdmin : Person
    {

        public string ManagerId { get; set; }
        public BankAdmin Manager { get; set; }
        public string BranchLocation { get; set; }
        public BankAdmin() { }
    }
}