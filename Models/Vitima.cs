using System.Collections.Generic;

namespace projeto.Models
{
    public class Vitima
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public int Idade { get; set; }
        public bool Status { get; set; }
        public ICollection<Crime> Crimes { get; set; }
        public ICollection<Autopsia> Autopsias { get; set; }
    }
}