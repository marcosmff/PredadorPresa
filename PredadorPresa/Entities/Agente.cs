using PredadorPresa.Model;

namespace PredadorPresa.Entities
{
    public class Agente
    {
        public Agente(TipoAgente tipo)
        {
            Tipo = tipo;
            Cor = CorAgente.AZUL;
        }

        public TipoAgente Tipo { get; set; }

        public int QuantidadeEmocao { get; set; }

        public int IntensidadeEmocao { get; set; }

        public CorAgente Cor { get; set; }
    }

    public enum TipoAgente
    {
        PRESA,
        PREDADOR
    }

    public enum CorAgente
    {
        AZUL,
        VERMELHO
    }
}
