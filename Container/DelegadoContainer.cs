using projeto.HATEOAS;
using projeto.Models;

namespace projeto.Container
{
    public class DelegadoContainer
    {
        public Delegado delegado { get; set; }
        public Link[] links { get; set; }
    }
}