using Microsoft.AspNetCore.Mvc;
using PredadorPresa.Entities;
using System.Text.Json;

namespace PredadorPresa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredadorPresaController : ControllerBase
    {
        [HttpGet]
        public string[][] Get()
        {
            var tamanho = 10;
            var retorno = new string[tamanho][];
            
            if (Ambiente.Movimenta(tamanho))
            {
            }

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
