using System;

namespace projeto.Models
{
    public class Crime
    {
        public int CriminosoID { get; set; }
        public Criminoso Criminoso { get; set; }
        public int VitimaID { get; set; }
        public Vitima Vitima { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
    }
}