namespace digimonMvc.Models
{
    public class responseModel
    {
        public int estado { get; set; }
        public string mensaje { get; set; } = string.Empty;

        public List<pokemonModel>? detalle { get; set; }
    }
}
