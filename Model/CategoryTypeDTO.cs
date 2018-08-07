using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CategoryTypeDTO
    {

        public CategoryTypeDTO()
        {
            MaincategoryList = new List<KategoriDTO>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public bool UserLoggedIn { get; set; }

        public List<KategoriDTO> MaincategoryList { get; set; }

    }
}
