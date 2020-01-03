using System.Collections.Generic;

namespace SinavSistemi
{
    public class CevapModel
    {
        public IEnumerable<Cevap> cevaplar { get; set; }

    }


    public class Cevap
    {
        public int soruId { get; set; }
        public string verilenCevap { get; set; }
    }
}