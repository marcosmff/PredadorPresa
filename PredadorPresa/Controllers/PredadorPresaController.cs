using Microsoft.AspNetCore.Mvc;
using PredadorPresa.Entities;
using PredadorPresa.Model;
using System.Text.Json;

namespace PredadorPresa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredadorPresaController : ControllerBase
    {
        [HttpGet]
        public string[][] Get([FromQuery] int tamanho)
        {
            if (Ambiente.Movimenta(tamanho))
                return new string[1][];

            var retorno = MontaRetorno(tamanho);

            return retorno;
        }

        [HttpPost]
        public string[][] Post([FromBody] DadosInicioAmbiente dadosInicioAmbiente)
        {
            Ambiente.Iniciar(dadosInicioAmbiente.Tamanho, dadosInicioAmbiente.NumeroPresas, dadosInicioAmbiente.NumeroPredadores);

            return MontaRetorno(dadosInicioAmbiente.Tamanho);
        }

        private string[][] MontaRetorno(int tamanho)
        {
            var retorno = new string[tamanho][];

            var posicoes = Ambiente.Posicoes;

            for (int i = 0; i < tamanho; i++)
            {
                retorno[i] = new string[tamanho];

                for (int j = 0; j < tamanho; j++)
                {
                    retorno[i][j] = JsonSerializer.Serialize(posicoes[i, j]);
                }
            }

            return retorno;
        }
    }
}
