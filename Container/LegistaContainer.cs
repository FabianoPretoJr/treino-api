using projeto.HATEOAS;
using projeto.Models;

namespace projeto.Container
{
    public class LegistaContainer
    {
        public Legista legista { get; set; }
        public Link[] links { get; set; }
    }
}