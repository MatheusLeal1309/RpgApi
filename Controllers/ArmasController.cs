using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RpgApi.Models;
using RpgApi.Models.Enuns;
using RpgApi.Data;
using Microsoft.EntityFrameworkCore;


namespace RpgApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArmasController : ControllerBase
    {
        private readonly DataContest _context;

        public ArmasController(DataContest context)
        {
            _context = context;
        }
        private static List<Arma> armamento = new List<Arma>()
        {
                new Arma() { Id = 1, Nome = "Sword", Dano = 10},
                new Arma() { Id = 2, Nome = "Axe", Dano = 12},
                new Arma() { Id = 3, Nome = "SwordFire", Dano = 20},
                new Arma() { Id = 4, Nome = "Cajado", Dano = 15},
                new Arma() { Id = 5, Nome = "Spear", Dano = 55},
                new Arma() { Id = 6, Nome = "CajadoDeath", Dano = 30},
                new Arma() { Id = 7, Nome = "Guns", Dano = 100}
        };
        [HttpGet("GetAll")]
        public IActionResult Get()
        {
            return Ok(armamento);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Arma novoArma)
        {
            try
            {
                if (novoArma.Dano == 0)
                    throw new Exception("Dano não poder ter o valor igual a 0 (zero.)");

                Personagem? p = await _context.TB_PERSONAGENS.FirstOrDefaultAsync(p => p.Id == novoArma.PersonagemId);

                if (p == null)
                    throw new Exception("Não Existe personagem com o Id informado.");

                Arma buscaArma = await _context.TB_ARMAS.FirstOrDefaultAsync(a => a.PersonagemId == novoArma.Id);

                if (buscaArma == null)
                    throw new Exception("O Personagem selecionado já contem uma arma atribuída a ele.");

                await _context.TB_ARMAS.AddAsync(novoArma);
                await _context.SaveChangesAsync();

                return Ok(novoArma.Id);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateArma(Arma p)
        {
            Arma armamentoAlterado = armamento.Find(pers => pers.Id == p.Id);
            armamentoAlterado.Nome = p.Nome;
            armamentoAlterado.Dano = p.Dano;
            armamentoAlterado.Id = p.Id;
            return Ok(armamento);
        }

        
        [HttpGet("{id}")]
        public IActionResult GetSingle(int id)
        {
            return Ok(armamento.FirstOrDefault(pe => pe.Id == id));

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            armamento.RemoveAll(pers => pers.Id == id);
            return Ok(armamento);
        }
    }
}