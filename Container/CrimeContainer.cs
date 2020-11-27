using projeto.HATEOAS;
using projeto.Models;

namespace projeto.Container
{
    public class CrimeContainer
    {
        public Crime crime { get; set; }
        public Link[] linksCriminoso { get; set; }
        public Link[] linksVitima { get; set; }
        public Link[] linksPolicial { get; set; }
    }
}