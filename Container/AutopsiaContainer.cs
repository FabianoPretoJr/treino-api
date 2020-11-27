using projeto.HATEOAS;
using projeto.Models;

namespace projeto.Container
{
    public class AutopsiaContainer
    {
        public Autopsia autopsia { get; set; }
        public Link[] linksVitima { get; set; }
        public Link[] linksLegista { get; set; }
    }
}