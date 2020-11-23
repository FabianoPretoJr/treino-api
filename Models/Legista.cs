using System.Collections.Generic;

namespace projeto.Models
{
    public class Legista
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CRM { get; set; }
        public bool Status { get; set; }
        public ICollection<Autopsia> Autopsias { get; set; }
    }
}