namespace projeto.DTO
{
    public class CrimeDTO
    {
        public int CriminosoID { get; set; }
        public int VitimaID { get; set; }
        public string Descricao { get; set; }
        public string Data { get; set; }
        public int PolicialID { get; set; }
        public int DelegaciaID { get; set; }
        public int DelegadoID { get; set; }
    }
}