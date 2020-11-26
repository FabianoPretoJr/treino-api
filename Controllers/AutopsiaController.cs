using System.Linq;
using Microsoft.AspNetCore.Mvc;
using projeto.Data;
using projeto.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace projeto.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AutopsiaController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        public AutopsiaController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var autopsias = database.autopsias.Include(a => a.Vitima).Include(a => a.Legista).ToList();
            return Ok(autopsias);
        }

        [HttpGet("GetByVitimica/{id}")]
        public IActionResult GetByVitima(int id)
        {
            var autopsias = database.autopsias.Where(a => a.VitimaID == id).Include(a => a.Legista).ToList();
            return Ok(autopsias);
        }

        [HttpGet("GetByLegista/{id}")]
        public IActionResult GetByLegista(int id)
        {
            var autopsias = database.autopsias.Where(a => a.LegistaID == id).Include(a => a.Vitima).ToList();
            return Ok(autopsias);
        }

        [HttpPost]
        public IActionResult Post([FromBody]Autopsia[] autopsias)
        {
            foreach (var autopsia in autopsias)
            {
                if (autopsia.LegistaID <= 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id do legista inválido"});
                }

                if(autopsia.VitimaID <= 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id de vitima inválido"});
                }

                if(autopsia.Data.ToString().Length <= 10)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Campo data está invalido"});
                }

                if(autopsia.Laudo.Length <= 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Campo laudo deve ter pelo menos mais de 1 caracter"});
                }

                autopsia.Legista = database.legistas.First(c => c.Id == autopsia.LegistaID);
                autopsia.Vitima = database.vitimas.First(v => v.Id == autopsia.VitimaID);

                database.autopsias.Add(autopsia);
                database.SaveChanges();
            }

            Response.StatusCode = 201;
            return new ObjectResult("");
        }

        [HttpPatch]
        public IActionResult Patch([FromBody]Autopsia[] autopsias)
        {
            foreach (var autopsia in autopsias)
            {
                if(autopsia.LegistaID < 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id do legista é inválido"});
                }
                if(autopsia.VitimaID < 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id da vitima é inválido"});
                }

                try
                {
                    var aut = database.autopsias.First(c => c.LegistaID == autopsia.LegistaID && c.VitimaID == autopsia.VitimaID);

                    if(aut != null)
                    {
                        aut.Data = autopsia.Data == null ? aut.Data : autopsia.Data;
                        aut.Laudo = autopsia.Laudo == null ? aut.Laudo : autopsia.Laudo;

                        database.SaveChanges();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult("Autopsia não encontrada");
                    }
                }
                catch(Exception)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult("Autopsia não encontrada");
                }
            }
            return Ok(); 
        }

        [HttpDelete("{idLegista}/{idVitima}")]
        public IActionResult Delete(int idLegista, int idVitima)
        {
            try
            {
                Autopsia aut = database.autopsias.First(c => c.LegistaID == idLegista && c.VitimaID == idVitima);
                database.autopsias.Remove(aut);
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