using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RpgApi.Models;
using RpgApi.Models.Enuns;


namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersongensExercicioController : ControllerBase
    {
         private static List<Personagem> personagens = new List<Personagem>()
        {
            //Colar os objetos da lista do chat aqui
            new Personagem() { Id = 1, Nome = "Frodo", PontosVida=100, Forca=17, Defesa=23, Inteligencia=33, Classe=ClasseEnun.Cavaleiro},
            new Personagem() { Id = 2, Nome = "Sam", PontosVida=100, Forca=15, Defesa=25, Inteligencia=30, Classe=ClasseEnun.Cavaleiro},
            new Personagem() { Id = 3, Nome = "Galadriel", PontosVida=100, Forca=18, Defesa=21, Inteligencia=35, Classe=ClasseEnun.Clerigo },
            new Personagem() { Id = 4, Nome = "Gandalf", PontosVida=100, Forca=18, Defesa=18, Inteligencia=37, Classe=ClasseEnun.Mago },
            new Personagem() { Id = 5, Nome = "Hobbit", PontosVida=100, Forca=20, Defesa=17, Inteligencia=31, Classe=ClasseEnun.Cavaleiro },
            new Personagem() { Id = 6, Nome = "Celeborn", PontosVida=100, Forca=21, Defesa=13, Inteligencia=34, Classe=ClasseEnun.Clerigo },
            new Personagem() { Id = 7, Nome = "Radagast", PontosVida=100, Forca=25, Defesa=11, Inteligencia=35, Classe=ClasseEnun.Mago }
        };

        [HttpGet ("GetByNome/{nome}")]
        public IActionResult GetSingle(string Nome)
        {
            List<Personagem> buscaNome = personagens.FindAll(Person => Person.Nome.Contains(Nome));
            return Ok(buscaNome);
        }


        [HttpPost("PostValidacao")]
        public IActionResult PostValidacao(Personagem novoPersonagem)
        {
            if (novoPersonagem.Defesa < 10 || novoPersonagem.Inteligencia > 30)
                  return BadRequest("Inteligência não poder ter o valor igual a 0 (zero) ou Defesa menor que 10");
           
            personagens.Add(novoPersonagem);
            return Ok(personagens);
        }




        [HttpPost("PostValidacaoMago")]
        public IActionResult PostValidacaoMago()
        {
            Personagem novoPersonagem = personagens.Find(p => p.Classe == ClasseEnun.Mago);
            if(novoPersonagem.Classe == ClasseEnun.Mago && novoPersonagem.Inteligencia < 35)
                {
                    return BadRequest("Inteligência mínima para Mago é 35.");
                }

            personagens.Add(novoPersonagem);
            return Ok(novoPersonagem);
        }


        [HttpGet("GetClerigoMago")]
        public IActionResult GetClerigoMago()
        {
            Personagem pRemoveCavaleiro = personagens.Find(p => p.Classe == ClasseEnun.Cavaleiro);
            personagens.Remove(pRemoveCavaleiro);
            List<Personagem> listaFInal = personagens.OrderBy(p => p.PontosVida).ToList();
            return Ok(personagens);
            
        }




        [HttpGet("GetEstatisticas")]
        public IActionResult Estatisticas()
        {
            int contar = personagens.Count;
            int contaforca = personagens.Sum(p => p.Inteligencia);
            return Ok("Quantidade de Persongaens: " + contar + " | Somatória de inteligência: " + contaforca);
        }





        [HttpGet("GetByClasse/{classeId}")]
        public IActionResult GetByClasse(int classeId)
        {
            var personagemByClasse = personagens.Where(p => (int)p.Classe == classeId).ToList();
           
            if (personagemByClasse.Count == 0)
            {
                return BadRequest("Nenhum personagem encontrado");
            
            }
            return Ok(personagemByClasse);
        }

    }
}
