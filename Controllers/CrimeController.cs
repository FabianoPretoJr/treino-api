using Microsoft.AspNetCore.Mvc;
using projeto.Data;
using projeto.Models;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace projeto.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CrimeController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        public CrimeController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var crimes = database.crimes.Include(c => c.Criminoso).Include(c => c.Vitima).ToList();
            return Ok(crimes);
        }

        [HttpGet("GetByCriminoso/{id}")]
        public IActionResult GetByCriminoso(int id)
        {
            var crimes = database.crimes.Where(c => c.CriminosoID == id).Include(c => c.Vitima).ToList();
            return Ok(crimes);
        }

        [HttpGet("GetByVitima/{id}")]
        public IActionResult GetByVitima(int id)
        {
            var crimes = database.crimes.Where(c => c.VitimaID == id).Include(c => c.Criminoso).ToList();
            return Ok(crimes);
        }

        [HttpPost]
        public IActionResult Post([FromBody]Crime[] crimes)
        {
            foreach (var crime in crimes)
            {
                if (crime.CriminosoID <= 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id de criminoso inválido"});
                }

                if(crime.VitimaID <= 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id de vitima inválido"});
                }

                if(crime.Data.ToString().Length <= 10)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Campo data está invalido"});
                }

                if(crime.Descricao.Length <= 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Campo descrição deve ter pelo menos mais de 1 caracter"});
                }

                crime.Criminoso = database.criminosos.First(c => c.Id == crime.CriminosoID);
                crime.Vitima = database.vitimas.First(v => v.Id == crime.VitimaID);

                database.crimes.Add(crime);
                database.SaveChanges();
            }

            Response.StatusCode = 201;
            return new ObjectResult("");
        }

        [HttpPatch]
        public IActionResult Patch([FromBody]Crime[] crimes)
        {
            foreach (var crime in crimes)
            {
                if(crime.CriminosoID < 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id do criminoso é inválido"});
                }
                if(crime.VitimaID < 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id da vitima é inválido"});
                }

                try
                {
                    var cri = database.crimes.First(c => c.CriminosoID == crime.CriminosoID && c.VitimaID == crime.VitimaID);

                    if(cri != null)
                    {
                        cri.Data = crime.Data == null ? cri.Data : crime.Data;
                        cri.Descricao = crime.Descricao == null ? cri.Descricao : crime.Descricao;

                        database.SaveChanges();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult("Crime não encontrado");
                    }
                }
                catch(Exception)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult("Crime não encontrado");
                }
            }
            return Ok(); 
        }

        [HttpDelete("{idCriminoso}/{idVitima}")]
        public IActionResult Delete(int idCriminoso, int idVitima)
        {
            try
            {
                Crime cri = database.crimes.First(c => c.CriminosoID == idCriminoso && c.VitimaID == idVitima);
                database.crimes.Remove(cri);
                database.SaveChanges();

                return Ok();
            }
            catch(Exception)
            {
                Response.StatusCode = 404;
                return new ObjectResult("");
            }
        }
    }
}