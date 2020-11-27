using projeto.HATEOAS;
using projeto.Models;

namespace projeto.Container
{
    public class CriminosoContainer
    {
        public Criminoso criminoso { get; set; }
        public Link[] links { get; set; }
    }
}