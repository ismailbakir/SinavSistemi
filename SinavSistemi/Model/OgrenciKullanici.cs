using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SinavSistemi.Model
{
    public class OgrenciKullanici
    {
        public string ad { get; set; }
        public string soyad { get; set; }
        public string email { get; set; }
        public string tel { get; set; }
        public int sinif { get; set; }
        public int seviye { get; set; }
    }
}