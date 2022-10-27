namespace digimonMvc.Models
{
    public class respUpdateModel
    {
        public int estado { get; set; }
        public string mensaje { get; set; } = string.Empty;

        public List<updateModel>? detalle { get; set; }
    }
}
