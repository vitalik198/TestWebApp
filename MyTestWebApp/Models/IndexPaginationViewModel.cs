using System.Collections.Generic;

namespace MyTestWebApp.Models
{

    public class IndexPaginationViewModel
    {
        public Ad ad {get;set;}
        public IEnumerable<Ad> ads { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
