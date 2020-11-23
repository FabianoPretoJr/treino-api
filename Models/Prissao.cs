using System;

namespace projeto.Models
{
    public class Prissao
    {
        public int PolicialID { get; set; }
        public Policial Policial { get; set; }
        public int CrimosoID { get; set; }
        public Criminoso Criminoso { get; set; }
        public int VitimaID { get; set; }
        public Vitima Vitima { get; set; }
        public DateTime Data { get; set; }
        public Delegacia Delegacia { get; set; }
        public Delegado Delegado { get; set; }
    }
}