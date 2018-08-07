using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{    
    public class SubcategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int InstruksVeiledningId { get; set; }
        public string Instructions { get; set; }
        public int LongInstructionsID { get; set; }
        public int Category_Id { get; set; }
    }
}