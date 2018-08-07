using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    //Tetiana model for statistikk på antall klikk fra brukere
    public class ClickCountDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime ClickDate { get; set; }
        public int CategoryId { get; set; }
        public int IsMainCat { get; set; }
    }
}
