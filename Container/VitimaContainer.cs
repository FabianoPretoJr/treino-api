using projeto.HATEOAS;
using projeto.Models;

namespace projeto.Container
{
    public class VitimaContainer
    {
        public Vitima vitima { get; set; }
        public Link[] links { get; set; }
    }
}