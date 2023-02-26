namespace PredadorPresa.Entities
{
    public class Agente
    {
        public Agente(TipoAgente tipo)
        {
            Tipo = tipo;
            Cor = CorAgente.AZUL;
            Modo = ModoAgente.VIVER;
            Interacoes = 0;
        }

        public TipoAgente Tipo { get; set; }
        
        public ModoAgente Modo { get; set; }

        public CorAgente Cor { get; set; }

        public int QualidadeEmocao { get; set; }

        public int IntensidadeEmocao { get; set; }

        public int Interacoes { get; set; }
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
    public enum ModoAgente
    {
        VIVER,
        CACAR
    }

}
