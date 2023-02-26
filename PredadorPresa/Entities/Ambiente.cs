using PredadorPresa.Model;

namespace PredadorPresa.Entities
{
    public static class Ambiente
    {
        private static readonly Random GeradorRandomico = new();
        public static Posicao[,] Posicoes { get; set; }
        private static Dictionary<Cordenada, Cordenada> PosicoesAtualizadas;

        public static void Iniciar(int tamanho, int numeroPresas, int numeroPredadores)
        {
            InicializaPosicoes(tamanho);

            InicializaPresa(tamanho, numeroPresas);

            InicializaPredadores(tamanho, numeroPredadores);
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

                    Posicoes[i, j].Feromonio--;
                }
            }

            foreach (var posicao in PosicoesAtualizadas)
            {
                if (PosicoesAtualizadas.Any(a => a.Value == posicao.Value && a.Key != posicao.Key) || Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente != null)
                    continue;

                Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente = Posicoes[posicao.Key.PosicaoX, posicao.Key.PosicaoY].Agente;
                Posicoes[posicao.Key.PosicaoX, posicao.Key.PosicaoY].Agente = null;

                if (Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.Tipo == TipoAgente.PREDADOR)
                {
                    if (AgentePerto(posicao.Value.PosicaoX, posicao.Value.PosicaoY, tamanho, TipoAgente.PRESA) != null)
                    {
                        Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.Modo = ModoAgente.CACAR;
                        Posicoes[posicao.Key.PosicaoX, posicao.Key.PosicaoY].Feromonio = 3;
                    }
                    else
                        Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.Modo = ModoAgente.VIVER;
                }
                else if (Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.Tipo == TipoAgente.PRESA)
                {
                    if (AgentePerto(posicao.Value.PosicaoX, posicao.Value.PosicaoY, tamanho, TipoAgente.PREDADOR) != null)
                    {
                        Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.QualidadeEmocao = ValidaQualidadeEmocao(--Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.QualidadeEmocao);
                        Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.IntensidadeEmocao = ValidaIntensidadeEmocao(++Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.IntensidadeEmocao);
                        Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.Cor = CorAgente.VERMELHO;
                    }
                    else
                    {
                        var cordenadaPresaPerto = AgentePerto(posicao.Value.PosicaoX, posicao.Value.PosicaoY, tamanho, TipoAgente.PRESA);

                        if (cordenadaPresaPerto != null)
                        {
                            var presaPerto = Posicoes[cordenadaPresaPerto.PosicaoX, cordenadaPresaPerto.PosicaoY].Agente;

                            if (presaPerto.Cor == CorAgente.AZUL)
                                Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.QualidadeEmocao = ValidaQualidadeEmocao(++Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.QualidadeEmocao);
                            else
                            {
                                Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.QualidadeEmocao = -1;
                                Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.IntensidadeEmocao = ValidaIntensidadeEmocao(++Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.IntensidadeEmocao);
                                Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.Interacoes = 2;
                            }
                        }
                        else
                        {
                            Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.Interacoes--;

                            if (Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.Interacoes <= 0)
                            {
                                Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.Cor = CorAgente.AZUL;
                                Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.QualidadeEmocao = ValidaQualidadeEmocao(++Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.QualidadeEmocao);
                                Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.IntensidadeEmocao = ValidaIntensidadeEmocao(--Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.IntensidadeEmocao);
                            }
                        }

                        if (Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.QualidadeEmocao >= 0)
                            Posicoes[posicao.Value.PosicaoX, posicao.Value.PosicaoY].Agente.Cor = CorAgente.AZUL;
                    }
                }
            }

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

                var posicaoFeromonio = FeromonioPerto(posicaoX, posicaoY, tamanho);

                if (posicaoFeromonio != null)
                    return BuscaNovaCordenadaPredador(posicaoX, posicaoY, posicaoFeromonio, tamanho);
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
                    if (posicaoX == i && posicaoY == j)
                        continue;

                    int posX = ValidaPosicao(posicaoX + i, tamanho);
                    int posY = ValidaPosicao(posicaoY + j, tamanho);

                    if (Posicoes[posX, posY].Agente?.Tipo == tipoAgente)
                        return new Cordenada(posX, posY);
                }
            }

            return null;
        }

        private static Cordenada FeromonioPerto(int posicaoX, int posicaoY, int tamanho)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int posX = ValidaPosicao(posicaoX + i, tamanho);
                    int posY = ValidaPosicao(posicaoY + j, tamanho);

                    if (Posicoes[posX, posY].Feromonio > 0)
                        return new Cordenada(posX, posY);
                }
            }

            return null;
        }

        private static Cordenada BuscaNovaCordenadaPredador(int posicaoX, int posicaoY, Cordenada cordenadaPresa, int tamanho)
        {
            if (cordenadaPresa.PosicaoX == posicaoX)
            {
                var cordenada = BuscaNovaCordenadaYPredador(posicaoX, posicaoY, cordenadaPresa, tamanho);

                if (Posicoes[cordenada.PosicaoX, cordenada.PosicaoY].Agente != null && Posicoes[cordenada.PosicaoX, cordenada.PosicaoY].Agente.Tipo == TipoAgente.PREDADOR)
                    return BuscaNovaCordenadaXPredador(posicaoX, posicaoY, cordenadaPresa, tamanho);

                return cordenada;
            }
            else
            {
                var cordenada = BuscaNovaCordenadaXPredador(posicaoX, posicaoY, cordenadaPresa, tamanho);

                if (Posicoes[cordenada.PosicaoX, cordenada.PosicaoY].Agente != null && Posicoes[cordenada.PosicaoX, cordenada.PosicaoY].Agente.Tipo == TipoAgente.PREDADOR)
                    return BuscaNovaCordenadaYPredador(posicaoX, posicaoY, cordenadaPresa, tamanho);

                return cordenada;
            }
        }

        private static Cordenada BuscaNovaCordenadaXPredador(int posicaoX, int posicaoY, Cordenada cordenadaPresa, int tamanho)
        {
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

        private static Cordenada BuscaNovaCordenadaYPredador(int posicaoX, int posicaoY, Cordenada cordenadaPresa, int tamanho)
        {
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

        private static Cordenada BuscaNovaCordenadaPresa(int posicaoX, int posicaoY, Cordenada cordenadaPredador, int tamanho)
        {
            if (cordenadaPredador.PosicaoX == posicaoX)
            {
                var cordenada = BuscaNovaCordenadaYPresa(posicaoX, posicaoY, cordenadaPredador, tamanho);

                if (Posicoes[cordenada.PosicaoX, cordenada.PosicaoY].Agente != null && Posicoes[cordenada.PosicaoX, cordenada.PosicaoY].Agente.Tipo == TipoAgente.PREDADOR)
                    return BuscaNovaCordenadaXPresa(posicaoX, posicaoY, cordenadaPredador, tamanho);

                return cordenada;
            }
            else
            {
                var cordenada = BuscaNovaCordenadaXPresa(posicaoX, posicaoY, cordenadaPredador, tamanho);

                if (Posicoes[cordenada.PosicaoX, cordenada.PosicaoY].Agente != null && Posicoes[cordenada.PosicaoX, cordenada.PosicaoY].Agente.Tipo == TipoAgente.PREDADOR)
                    return BuscaNovaCordenadaYPresa(posicaoX, posicaoY, cordenadaPredador, tamanho);

                return cordenada;
            }
        }

        private static Cordenada BuscaNovaCordenadaYPresa(int posicaoX, int posicaoY, Cordenada cordenadaPredador, int tamanho)
        {
            if (posicaoY > cordenadaPredador.PosicaoY)
            {
                if (tamanho - 1 - posicaoY + cordenadaPredador.PosicaoY < posicaoY - cordenadaPredador.PosicaoY)
                    return new Cordenada(posicaoX, ValidaPosicao(posicaoY - 1, tamanho));
                else
                    return new Cordenada(posicaoX, ValidaPosicao(posicaoY + 1, tamanho));
            }
            else if (posicaoY < cordenadaPredador.PosicaoY)
            {
                if (tamanho - 1 - cordenadaPredador.PosicaoY + posicaoY < cordenadaPredador.PosicaoY - posicaoY)
                    return new Cordenada(posicaoX, ValidaPosicao(posicaoY + 1, tamanho));
                else
                    return new Cordenada(posicaoX, ValidaPosicao(posicaoY - 1, tamanho));
            }
            else
            {
                var novoY = ValidaPosicao(posicaoY + 1, tamanho);

                if (Posicoes[posicaoX, novoY].Agente == null)
                    return new Cordenada(posicaoX, novoY);

                return new Cordenada(posicaoX, ValidaPosicao(posicaoY - 1, tamanho));
            }
        }

        private static Cordenada BuscaNovaCordenadaXPresa(int posicaoX, int posicaoY, Cordenada cordenadaPredador, int tamanho)
        {
            if (posicaoX > cordenadaPredador.PosicaoX)
            {
                if (tamanho - 1 - posicaoX + cordenadaPredador.PosicaoX > posicaoX - cordenadaPredador.PosicaoX)
                    return new Cordenada(ValidaPosicao(posicaoX + 1, tamanho), posicaoY);
                else
                    return new Cordenada(ValidaPosicao(posicaoX - 1, tamanho), posicaoY);
            }
            else if (posicaoX < cordenadaPredador.PosicaoX)
            {
                if (tamanho - 1 - cordenadaPredador.PosicaoX + posicaoX > cordenadaPredador.PosicaoX - posicaoX)
                    return new Cordenada(ValidaPosicao(posicaoX - 1, tamanho), posicaoY);
                else
                    return new Cordenada(ValidaPosicao(posicaoX + 1, tamanho), posicaoY);
            }
            else
            {
                var novoX = ValidaPosicao(posicaoX + 1, tamanho);

                if (Posicoes[novoX, posicaoY].Agente == null)
                    return new Cordenada(novoX, posicaoY);

                return new Cordenada(ValidaPosicao(posicaoX - 1, tamanho), posicaoY);
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

        private static int ValidaQualidadeEmocao(int quantidadeEmocao)
        {
            if (quantidadeEmocao > 3)
                return 3;

            if (quantidadeEmocao < -3)
                return -3;

            return quantidadeEmocao;
        }

        private static int ValidaIntensidadeEmocao(int quantidadeEmocao)
        {
            if (quantidadeEmocao > 3)
                return 3;

            if (quantidadeEmocao < 0)
                return 0;

            return quantidadeEmocao;
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
