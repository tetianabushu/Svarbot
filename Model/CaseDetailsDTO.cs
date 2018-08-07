using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CaseDetailsDTO
    {
        public string UserName { get; set; }
        public int CaseId { get; set; }
        public string CaseDetails { get; set; }
        public DateTime DateCreated { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

    }
}
