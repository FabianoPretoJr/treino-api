using projeto.HATEOAS;
using projeto.Models;

namespace projeto.Container
{
    public class PolicialContainer
    {
        public Policial policial { get; set; }
        public Link[] links { get; set; }
    }
}