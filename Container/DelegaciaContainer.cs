using projeto.HATEOAS;
using projeto.Models;

namespace projeto.Container
{
    public class DelegaciaContainer
    {
        public Delegacia delegacia { get; set; }
        public Link[] links { get; set; }
    }
}