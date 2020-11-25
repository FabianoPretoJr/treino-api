using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Data;
using System.Linq;
using System;

namespace projeto.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PolicialController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public PolicialController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var policiais = database.policiais.Where(p => p.Status == true).ToList();
            return Ok(policiais);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var policial = database.policiais.First(p => p.Id == id);
                return Ok(policial);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                return new ObjectResult("");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]Policial policial)
        {
            if (policial.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O nome do policial deve ter mais de um caracter"});
            }
            if(policial.Funcional.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "A funcional do policial deve ter mais de um caracter"});
            }
            if(policial.Patente.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "A patente do policial deve ter mais de um caracter"});
            }
            
            policial.Status = true;
            database.policiais.Add(policial);
            database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult("");
        }

        [HttpPatch]
        public IActionResult Patch([FromBody]Policial policial)
        {
            if(policial.Id > 0)
            {
                try
                {
                    var pol = database.policiais.First(p => p.Id == policial.Id);

                    if(pol != null)
                    {
                        pol.Nome = policial.Nome != null ? policial.Nome : pol.Nome;
                        pol.Funcional = policial.Funcional != null ? policial.Funcional : pol.Funcional;
                        pol.Patente = policial.Patente != null ? policial.Patente : pol.Patente;
                        database.SaveChanges();

                        return Ok(); 
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult("Policial não encontrada");
                    }
                }
                catch(Exception)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult("Policial não encontrada");
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id do Policial é inválido"});
            }
        } 

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Policial pol = database.policiais.First(p => p.Id == id);
                pol.Status = false;
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