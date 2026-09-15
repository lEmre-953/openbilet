using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openbilet.Models
{
    public class SeferBilgisi
    {
        public string Id { get; set; }
        public string Firma { get; set; }
        public string Saat { get; set; }
        public string KoltukTipi { get; set; }
        public string Fiyat { get; set; }
        public string Aciklama { get; set; }
    }
}
