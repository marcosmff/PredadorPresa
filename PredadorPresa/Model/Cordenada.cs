namespace PredadorPresa.Model
{
    public class Cordenada : IComparable
    {
        public Cordenada(int posicaoX, int posicaoY)
        {
            PosicaoX = posicaoX;
            PosicaoY = posicaoY;
        }

        public int PosicaoX { get; set; }

        public int PosicaoY { get; set; }

        public int CompareTo(object obj)
        {
            var cordenada = (Cordenada)obj;

            return this.PosicaoX.CompareTo(cordenada.PosicaoX) == 1 && this.PosicaoY.CompareTo(cordenada.PosicaoY) == 1 ? 1 : 0;
        }
    }
}
