using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;
using Microsoft.Data.SqlClient;

namespace RpgApi.Models
{
    [ApiController]
    [Route("[controller]")]
    public class DisputaController : ControllerBase
    {
        private readonly DataContest _context;

        public DisputaController(DataContest context)
        {
            _context = context;
        }

        [HttpPost("Arma")]
        public async Task<IActionResult> AtaqueComArmaAsync(Disputa d)
        {
            try
            {
                Personagem? atacante = await _context.TB_PERSONAGENS
                .Include(p=> p.Arma)
                .FirstOrDefaultAsync(p => p.Id== d.AtacanteId);

                Personagem? oponente = await _context.TB_PERSONAGENS
                .FirstOrDefaultAsync(p => p.Id == d.OponenteId);

                int dano = atacante.Arma.Dano + new Random().Next(atacante.Forca); //pegando o ataque da arma e somando com dano aleatório

                dano = dano - new Random().Next(oponente.Defesa);


                if (dano > 0)
                    oponente.PontosVida = oponente.PontosVida - (int)dano; //fazendo perder de vida de acordo com valor de dano
                if (oponente.PontosVida <= 0)
                    d.Narracao = $"{oponente.Nome} foi derrotado";

                _context.TB_PERSONAGENS.Update(oponente);//atauliza banco de dados
                await _context.SaveChangesAsync();

                StringBuilder dados = new StringBuilder();
                dados.AppendFormat(" Atacante: {0}. ", atacante.Nome);
                dados.AppendFormat(" Oponente: {0}. ", oponente.Nome);
                dados.AppendFormat(" Pontos de vida do atacante: {0}. ", atacante.PontosVida);
                dados.AppendFormat(" Pontos de vida do oponente: {0}. ", oponente.PontosVida);
                dados.AppendFormat(" Arma Utilizada: {0}. ", atacante.Arma.Nome);
                dados.AppendFormat(" Dano: {0}. ", dano);

                d.Narracao += dados.ToString();
                d.DataDisputa = DateTime.Now;
                _context.TB_DISPUTAS.Add(d);
                _context.SaveChanges();

                return Ok(d);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Habilidade")]
        public async Task<IActionResult> AtaqueComHabilidadeAsync(Disputa d)
        {
            try
            {
                Personagem atacante = await _context.TB_PERSONAGENS
                .Include(p => p.PersonagemHabilidades) .ThenInclude(ph => ph.Habilidade)
                .FirstOrDefaultAsync(p => p.Id == d.AtacanteId);

                Personagem oponente = await _context.TB_PERSONAGENS
                .FirstOrDefaultAsync(p => p.Id == d.OponenteId);

                PersonagemHabilidade ph = await _context.TB_PERSONAGENS_HABILIDADES
                .Include(p => p.Habilidade) .FirstOrDefaultAsync(phBusca => phBusca.HabilidadeId == d.HabilidadeId
                && phBusca.PersonagemId == d.AtacanteId);

                if (ph == null)
                    d.Narracao = $"{atacante.Nome} não possui esta habilidade";
                else
                {
                    int dano = ph.Habilidade.Dano + (new Random().Next(atacante.Inteligencia));
                    dano = dano - new Random().Next(oponente.Defesa);

                    if (dano > 0)
                    oponente.PontosVida = oponente.PontosVida - dano;
                    if (oponente.PontosVida <= 0)
                    d.Narracao += $"{oponente.Nome} foi derrotado";

                    _context.TB_PERSONAGENS.Update(oponente);
                    await _context.SaveChangesAsync();

                    StringBuilder dados = new StringBuilder();
                    dados.AppendFormat(" Atacante: {0}. ", atacante.Nome);
                    dados.AppendFormat(" Oponente: {0}. ", oponente.Nome);
                    dados.AppendFormat(" Pontos de vida do atacante: {0}. ", atacante.PontosVida);
                    dados.AppendFormat(" Pontos de vida do oponente: {0}. ", oponente.PontosVida);
                    dados.AppendFormat(" Arma Utilizada: {0}. ", ph.Habilidade.Nome);
                    dados.AppendFormat(" Dano: {0}. ", dano);

                    d.Narracao += dados.ToString();
                    d.DataDisputa = DateTime.Now;
                    _context.TB_DISPUTAS.Add(d);
                    _context.SaveChanges();
                }
                return Ok(d);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("PersonagemRamdom")]
        public async Task<IActionResult> Sorteio()
        {
            int sorteio = 1;
            List<Personagem> personagens = 
                await _context.TB_PERSONAGENS.ToListAsync();

            //Sorteio com numero da quantidade de peronagens
            sorteio = new Random().Next(personagens.Count);

            //busca na lista pelo indice sorteado (nao é o ID)
            Personagem p = personagens[sorteio];

            string msg= string.Format("N° Sorteado: {0}.\nPersonagem: {1}.",sorteio, p.Nome);

            return Ok(msg);
        }

        [HttpPost("DisputaEmGrupo")]
        public async Task<IActionResult> DisputaEmGrupo(Disputa d)
        {
            try
            {
                d.Resultados = new List<string>(); //Instancia a lista de resultados

                //Busca na Bas dos Personagens informados no parametro incluindo Armas e Habilidades
                List<Personagem> personagens = await _context.TB_PERSONAGENS
                .Include (p => p.Arma)
                .Include (p => p.PersonagemHabilidades).ThenInclude(ph => ph.Habilidade)
                .Where(p => d.ListaIdPersonagens.Contains(p.Id)).ToListAsync();

                //Contagem de personagens vivos na lista obtida do banco de dados
                int qtdPersonagemVivos = personagens.FindAll(p => p.PontosVida > 0).Count;

                //Enquanto houver mais de um perssonagem vivo haverá disputa
                while (qtdPersonagemVivos > 1)
                {
                    //Seleciona personagens com pontos d vida positivos e depois faz sorteio.
                    List<Personagem> atacantes = personagens.Where(p => p.PontosVida > 0).ToList();
                    Personagem atacante = atacantes[new Random().Next(atacantes.Count)];
                    d.AtacanteId = atacante.Id;

                    //Seleciona personagens com pontos de vida positivos, exceto o atacante escolhido e depois faz sortio.
                    List<Personagem> oponentes = personagens.Where(p => p.Id != atacante.Id && p.PontosVida > 0).ToList();
                    Personagem oponente = oponentes[new Random().Next(oponentes.Count)];
                    d.OponenteId = oponente.Id;

                    //declara e redefine a cada passagem do while o valor das variaveis que serão usadas.
                    int dano = 0;
                    string ataqueUsado = string.Empty;
                    string resultado = string.Empty;

                    //sortia entre 0 e 1: 0 é um ataque com arma e 1 é um ataqu com Habilidades
                    bool ataqueUsaArma = new Random().Next(1) == 0;

                    if (ataqueUsaArma && atacante.Arma != null)
                    {
                        //sorteio da força

                        dano = atacante.Arma.Dano + new Random().Next(atacante.Forca);
                        dano = dano - new Random().Next(oponente.Defesa); //sorteio da defesa.
                        ataqueUsado = atacante.Arma.Nome;

                    if (dano > 0)
                        oponente.PontosVida = oponente.PontosVida - dano;


                    //formata a mensagem
                    resultado = 
                        string.Format("{0} atacou {1} usando {2} com o dano {3}.", atacante.Nome, oponente.Nome, ataqueUsado, dano);
                    d.Narracao += resultado;
                    d.Resultados.Add(resultado);
                    }

                    else if(atacante.PersonagemHabilidades.Count !=0) //vrifica se o personagm tem habilidades na lista dele
                    {
                        int sorteioHabilidadeId = new Random().Next(atacante.PersonagemHabilidades.Count);
                        Habilidade habilidadEscolhida = atacante.PersonagemHabilidades[sorteioHabilidadeId].Habilidade;
                        ataqueUsado = habilidadEscolhida.Nome;

                        //sorteio da inteligência somada ao dano
                        dano = habilidadEscolhida.Dano + new Random().Next(atacante.Inteligencia);
                        dano = dano - new Random().Next(oponente.Defesa); // sorteio da defesa.

                        if (dano > 0)
                            oponente.PontosVida = oponente.PontosVida - dano;

                       resultado = 
                            string.Format("{0} atacou {1} usando {2} com o dano {3}.", atacante.Nome, oponente.Nome, ataqueUsado, dano);
                        d.Narracao += resultado;
                        d.Resultados.Add(resultado);

                    }

                    if (!string.IsNullOrEmpty(ataqueUsado))
                    {
                        atacante.Vitorias++;
                        oponente.Derrotas++;
                        atacante.Disputas++;
                        oponente.Disputas++;

                        d.Id = 0; //Zera o Id para poder salvar os dados de disputa sem erro de chave
                        d.DataDisputa = DateTime.Now;
                        _context.TB_DISPUTAS.Add(d);
                        await _context.SaveChangesAsync();  
                    }
                    
                    qtdPersonagemVivos = personagens.FindAll(p => p.PontosVida > 0).Count();

                    if (qtdPersonagemVivos == 1) //Havendo só um personagem vivo, exite um Campeão!
                    {
                        string resultadoFinal = 
                            $"{atacante.Nome.ToUpper()} é CAMPEÃO com {atacante.PontosVida} pontos de vida restantes!";

                        d.Narracao += resultadoFinal;
                        d.Resultados.Add(resultadoFinal);

                        break; //para o while
                    }
                }
                //código após o fechamento do While
                // disputas, vitórias e derrotas de todos personagens ao final das batalhas
                _context.TB_PERSONAGENS.UpdateRange(personagens);
                await _context.SaveChangesAsync();

                return Ok(d);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("ApagarDisputas")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
{
                List<Disputa> disputas = await _context.TB_DISPUTAS.ToListAsync();
                _context.TB_DISPUTAS.RemoveRange(disputas);
                await _context.SaveChangesAsync();
                return Ok("Disputas apagadas");
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }
        [HttpGet("Listar")]
        public async Task<IActionResult> ListarAsync()
        {
            try
            {
                List<Disputa> disputas =
                await _context.TB_DISPUTAS.ToListAsync();
                return Ok(disputas);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}