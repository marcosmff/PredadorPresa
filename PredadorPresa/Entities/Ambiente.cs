using PredadorPresa.Model;

namespace PredadorPresa.Entities
{
    public static class Ambiente
    {
        public static Posicao[,] Posicoes { get; set; }
        private static Random GeradorRandomico = new Random();
        private static Dictionary<Cordenada, Cordenada> PosicoesAtualizadas;

        public static void Iniciar(int tamanho, int numeroPresas, int numeroPredadores)
        {
            InicializaPosicoes(tamanho);

            InicializaPresa(tamanho, numeroPresas);

            InicializaPredadores(tamanho, numeroPredadores);

            EscreveAmbiente(tamanho);

            /*for (int i = 0; i < 10; i++)
            {
                Console.WriteLine();
                Movimenta(tamanho);
                EscreveAmbiente(tamanho);

                Thread.Sleep(1000);
            }*/

            var a = "";

        }

        public static bool Movimenta(int tamanho)
        {

            PosicoesAtualizadas = new Dictionary<Cordenada, Cordenada>();

            for (int i = 0; i < tamanho; i++)
            {
                for (int j = 0; j < tamanho; j++)
                {
                    if (Posicoes[i, j].Agente != null)
                    {
                        var novaPosicao = GeraNovaCoredenada(tamanho, i, j);

                        if (novaPosicao == null)
                            return true;

                        PosicoesAtualizadas.Add(new Cordenada(i, j), novaPosicao);
                    }
                }
            }

            foreach (var posicao in PosicoesAtualizadas)
            {
                if (PosicoesAtualizadas.Any(a => a.Value == posicao.Value && a.Key != posicao.Key) || Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente != null)
                    continue;

                Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente = Posicoes[posicao.Key.PosicaoX, posicao.Key.PosicaoY].Agente;
                Posicoes[posicao.Key.PosicaoX, posicao.Key.PosicaoY].Agente = null;
            }

            EscreveAmbiente(tamanho);
            Console.WriteLine();

            return false;
        }

        private static void InicializaPosicoes(int tamanho)
        {
            Posicoes = new Posicao[tamanho, tamanho];

            for (int i = 0; i < tamanho; i++)
            {
                for (int j = 0; j < tamanho; j++)
                {
                    Posicoes[i, j] = new Posicao();
                }
            }
        }

        private static void InicializaPresa(int tamanho, int numeroPresas)
        {
            for (int i = 0; i < numeroPresas; i++)
            {
                var cordenada = GeraCoredenada(tamanho);

                Posicoes[cordenada.PosicaoX, cordenada.PosicaoY].Agente = new Agente(TipoAgente.PRESA);
            }
        }

        private static void InicializaPredadores(int tamanho, int numeroPredadores)
        {
            for (int i = 0; i < numeroPredadores; i++)
            {
                var cordenada = GeraCoredenada(tamanho);

                Posicoes[cordenada.PosicaoX, cordenada.PosicaoY].Agente = new Agente(TipoAgente.PREDADOR);
            }
        }

        private static Cordenada GeraCoredenada(int tamanho)
        {
            var posicaoX = GeradorRandomico.Next(tamanho - 1);
            var posicaoY = GeradorRandomico.Next(tamanho - 1);

            if (Posicoes[posicaoX, posicaoY].Agente == null)
                return new Cordenada(posicaoX, posicaoY);

            while (true)
            {
                posicaoX = GeradorRandomico.Next(tamanho - 1);

                if (Posicoes[posicaoX, posicaoY].Agente == null)
                    return new Cordenada(posicaoX, posicaoY);

                posicaoY = GeradorRandomico.Next(tamanho - 1);

                if (Posicoes[posicaoX, posicaoY].Agente == null)
                    return new Cordenada(posicaoX, posicaoY);
            }
        }

        private static Cordenada GeraNovaCoredenada(int tamanho, int posicaoX, int posicaoY)
        {
            if (Posicoes[posicaoX, posicaoY].Agente?.Tipo == TipoAgente.PREDADOR)
            {
                var posicaoPresa = AgentePerto(posicaoX, posicaoY, tamanho, TipoAgente.PRESA);

                if (posicaoPresa != null)
                    return BuscaNovaCordenadaPredador(posicaoX, posicaoY, posicaoPresa, tamanho);
            }

            if (Posicoes[posicaoX, posicaoY].Agente?.Tipo == TipoAgente.PRESA)
            {
                if (IsPresaCapturada(tamanho, new Cordenada(posicaoX, posicaoY)))
                    return null;

                var posicaoPredador = AgentePerto(posicaoX, posicaoY, tamanho, TipoAgente.PREDADOR);

                if (posicaoPredador != null)
                    return BuscaNovaCordenadaPresa(posicaoX, posicaoY, posicaoPredador, tamanho);
            }

            var vertical = GeradorRandomico.Next(20);
            var frente = GeradorRandomico.Next(20);

            if (vertical > 10)
            {
                if (frente > 10)
                {
                    posicaoX++;

                    if (posicaoX >= tamanho)
                        posicaoX = 0;
                }
                else
                {
                    posicaoX--;

                    if (posicaoX < 0)
                        posicaoX = tamanho - 1;
                }
            }
            else
            {
                if (frente > 10)
                {
                    posicaoY++;

                    if (posicaoY >= tamanho)
                        posicaoY = 0;
                }
                else
                {
                    posicaoY--;

                    if (posicaoY < 0)
                        posicaoY = tamanho - 1;
                }
            }

            return new Cordenada(posicaoX, posicaoY);
        }

        private static Cordenada AgentePerto(int posicaoX, int posicaoY, int tamanho, TipoAgente tipoAgente)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int posX = ValidaPosicao(posicaoX + i, tamanho);
                    int posY = ValidaPosicao(posicaoY + j, tamanho);

                    if (Posicoes[posX, posY].Agente?.Tipo == tipoAgente)
                        return new Cordenada(posX, posY);
                }
            }

            return null;
        }

        private static Cordenada BuscaNovaCordenadaPredador(int posicaoX, int posicaoY, Cordenada cordenadaPresa, int tamanho)
        {
            if (cordenadaPresa.PosicaoX == posicaoX)
            {
                /*var posicaoInc = ValidaPosicao(posicaoY + 1, tamanho);
                if (Math.Abs(posicaoInc - cordenadaPresa.PosicaoY) < Math.Abs(posicaoY - cordenadaPresa.PosicaoY))
                    return new Cordenada(posicaoX, posicaoInc);
                else
                    return new Cordenada(posicaoX, ValidaPosicao(posicaoY - 1, tamanho));*/

                if (posicaoY > cordenadaPresa.PosicaoY)
                {
                    if (tamanho - 1 - posicaoY + cordenadaPresa.PosicaoY > posicaoY - cordenadaPresa.PosicaoY)
                        return new Cordenada(posicaoX, ValidaPosicao(posicaoY - 1, tamanho));
                    else
                        return new Cordenada(posicaoX, ValidaPosicao(posicaoY + 1, tamanho));
                }
                else
                {
                    if (tamanho - 1 - cordenadaPresa.PosicaoY + posicaoY > cordenadaPresa.PosicaoY - posicaoY)
                        return new Cordenada(posicaoX, ValidaPosicao(posicaoY + 1, tamanho));
                    else
                        return new Cordenada(posicaoX, ValidaPosicao(posicaoY - 1, tamanho));
                }
            }
            else
            {
                /*var posicaoInc = ValidaPosicao(posicaoX + 1, tamanho);
                if (Math.Abs(posicaoInc - cordenadaPresa.PosicaoX) < Math.Abs(posicaoX - cordenadaPresa.PosicaoX))
                    return new Cordenada(posicaoInc, posicaoY);
                else
                    return new Cordenada(ValidaPosicao(posicaoX - 1, tamanho), posicaoY);*/

                if (posicaoX > cordenadaPresa.PosicaoX)
                {
                    if (tamanho - 1 - posicaoX + cordenadaPresa.PosicaoX > posicaoX - cordenadaPresa.PosicaoX)
                        return new Cordenada(ValidaPosicao(posicaoX - 1, tamanho), posicaoY);
                    else
                        return new Cordenada(ValidaPosicao(posicaoX + 1, tamanho), posicaoY);
                }
                else
                {
                    if (tamanho - 1 - cordenadaPresa.PosicaoX + posicaoX > cordenadaPresa.PosicaoX - posicaoX)
                        return new Cordenada(ValidaPosicao(posicaoX + 1, tamanho), posicaoY);
                    else
                        return new Cordenada(ValidaPosicao(posicaoX - 1, tamanho), posicaoY);
                }
            }
        }

        private static Cordenada BuscaNovaCordenadaPresa(int posicaoX, int posicaoY, Cordenada cordenadaPredador, int tamanho)
        {
            /*if (cordenadaPredador.PosicaoX == posicaoX)
            {
                if (cordenadaPredador.PosicaoY > posicaoY)
                    return new Cordenada(posicaoX, ValidaPosicao(posicaoY - 1, tamanho));
                else
                    return new Cordenada(posicaoX, ValidaPosicao(posicaoY + 1, tamanho));
            }
            else
            {
                if (cordenadaPredador.PosicaoX > posicaoX)
                    return new Cordenada(ValidaPosicao(posicaoX - 1, tamanho), posicaoY);
                else
                    return new Cordenada(ValidaPosicao(posicaoX + 1, tamanho), posicaoY);
            }*/

            if (cordenadaPredador.PosicaoX == posicaoX)
            {
                /*var posicaoInc = ValidaPosicao(posicaoY + 1, tamanho);
                if (Math.Abs(posicaoInc - cordenadaPresa.PosicaoY) < Math.Abs(posicaoY - cordenadaPresa.PosicaoY))
                    return new Cordenada(posicaoX, posicaoInc);
                else
                    return new Cordenada(posicaoX, ValidaPosicao(posicaoY - 1, tamanho));*/

                if (posicaoY > cordenadaPredador.PosicaoY)
                {
                    if (tamanho - 1 - posicaoY + cordenadaPredador.PosicaoY < posicaoY - cordenadaPredador.PosicaoY)
                        return new Cordenada(posicaoX, ValidaPosicao(posicaoY - 1, tamanho));
                    else
                        return new Cordenada(posicaoX, ValidaPosicao(posicaoY + 1, tamanho));
                }
                else
                {
                    if (tamanho - 1 - cordenadaPredador.PosicaoY + posicaoY < cordenadaPredador.PosicaoY - posicaoY)
                        return new Cordenada(posicaoX, ValidaPosicao(posicaoY + 1, tamanho));
                    else
                        return new Cordenada(posicaoX, ValidaPosicao(posicaoY - 1, tamanho));
                }
            }
            else
            {
                /*var posicaoInc = ValidaPosicao(posicaoX + 1, tamanho);
                if (Math.Abs(posicaoInc - cordenadaPresa.PosicaoX) < Math.Abs(posicaoX - cordenadaPresa.PosicaoX))
                    return new Cordenada(posicaoInc, posicaoY);
                else
                    return new Cordenada(ValidaPosicao(posicaoX - 1, tamanho), posicaoY);*/

                if (posicaoX > cordenadaPredador.PosicaoX)
                {
                    if (tamanho - 1 - posicaoX + cordenadaPredador.PosicaoX > posicaoX - cordenadaPredador.PosicaoX)
                        return new Cordenada(ValidaPosicao(posicaoX - 1, tamanho), posicaoY);
                    else
                        return new Cordenada(ValidaPosicao(posicaoX + 1, tamanho), posicaoY);
                }
                else
                {
                    if (tamanho - 1 - cordenadaPredador.PosicaoX + posicaoX > cordenadaPredador.PosicaoX - posicaoX)
                        return new Cordenada(ValidaPosicao(posicaoX + 1, tamanho), posicaoY);
                    else
                        return new Cordenada(ValidaPosicao(posicaoX - 1, tamanho), posicaoY);
                }
            }
        }

        private static int ValidaPosicao(int posicao, int tamanho)
        {
            if (posicao < 0)
                return tamanho + posicao;

            if (posicao >= tamanho)
                return posicao - tamanho;

            return posicao;
        }

        private static bool IsPresaCapturada(int tamanho, Cordenada cordenadaPresa)
        {
            if (Posicoes[cordenadaPresa.PosicaoX, ValidaPosicao(cordenadaPresa.PosicaoY + 1, tamanho)].Agente?.Tipo == TipoAgente.PREDADOR &&
                Posicoes[cordenadaPresa.PosicaoX, ValidaPosicao(cordenadaPresa.PosicaoY - 1, tamanho)].Agente?.Tipo == TipoAgente.PREDADOR &&
                Posicoes[ValidaPosicao(cordenadaPresa.PosicaoX + 1, tamanho), cordenadaPresa.PosicaoY].Agente?.Tipo == TipoAgente.PREDADOR &&
                Posicoes[ValidaPosicao(cordenadaPresa.PosicaoX - 1, tamanho), cordenadaPresa.PosicaoY].Agente?.Tipo == TipoAgente.PREDADOR)
                return true;

            return false;
        }

        private static void EscreveAmbiente(int tamanho)
        {
            for (int i = 0; i < tamanho; i++)
            {
                for (int j = 0; j < tamanho; j++)
                {
                    if (Posicoes[i, j].Agente == null)
                        Console.Write("0 ");
                    else
                    {
                        if (Posicoes[i, j].Agente.Tipo == TipoAgente.PREDADOR)
                        {
                            Console.Write("# ");
                        }
                        else
                            Console.Write("$ ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
