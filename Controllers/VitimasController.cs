using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Data;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using projeto.DTO;

namespace projeto.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VitimasController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public VitimasController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var vitimas = database.vitimas.Where(v => v.Status == true).Include(v => v.Crimes).ThenInclude(v => v.Criminoso).ToList();
            return Ok(vitimas);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var vitimas = database.vitimas.First(v => v.Id == id);
                return Ok(vitimas);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                return new ObjectResult("");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]VitimaDTO vitimaTemp)
        {
            if (vitimaTemp.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O nome do criminoso deve ter mais de um caracter"});
            }
            if(vitimaTemp.CPF.Length != 11)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O CPF deve ter 11 digitos"});
            }
            if(vitimaTemp.Idade <= 0)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "A idade deve ser maior do que 0 (zero)"});
            }

            Vitima vitima = new Vitima();

            vitima.Nome = vitimaTemp.Nome;
            vitima.CPF = vitimaTemp.CPF;
            vitima.Idade = vitimaTemp.Idade;
            vitima.Status = true;

            database.vitimas.Add(vitima);
            database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult("");
        }

        [HttpPatch]
        public IActionResult Patch([FromBody]VitimaDTO vitimaTemp)
        {
            if(vitimaTemp.Id > 0)
            {
                try
                {
                    var vit = database.vitimas.First(c => c.Id == vitimaTemp.Id);

                    if(vit != null)
                    {
                        vit.Nome = vitimaTemp.Nome != null ? vitimaTemp.Nome : vit.Nome;
                        vit.CPF = vitimaTemp.CPF != null ? vitimaTemp.CPF : vit.CPF;
                        vit.Idade = vitimaTemp.Idade > 0 ? vitimaTemp.Idade : vit.Idade;
                        database.SaveChanges();

                        return Ok(); 
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult("Vitima não encontrada");
                    }
                }
                catch(Exception)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult("Vitima não encontrada");
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id da vitima é inválido"});
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Vitima vit = database.vitimas.First(v => v.Id == id);
                vit.Status = false;
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