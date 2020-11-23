using System;

namespace projeto.Models
{
    public class Autopsia
    {
        public int LegistaID { get; set; }
        public Legista Legista { get; set; }
        public int VitimaID { get; set; }
        public Vitima Vitima { get; set; }
        public DateTime Data { get; set; }
        public string Laudo { get; set; }
    }
}