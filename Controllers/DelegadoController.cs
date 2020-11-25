using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Data;
using System.Linq;
using System;

namespace projeto.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DelegadoController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public DelegadoController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var delegados = database.delegados.Where(d => d.Status == true).ToList();
            return Ok(delegados);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var delegado = database.delegados.First(d => d.Id == id);
                return Ok(delegado);
            }
            catch(Exception)
            {
                Response.StatusCode = 400;
                return new ObjectResult("");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]Delegado delegado)
        {
            if (delegado.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O nome de delegado deve ter mais de um caracter"});
            }

            if(delegado.CPF.Length != 11)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O CPF deve ter 11 (onze) digitos"});
            }

            if(delegado.Funcional.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "A funcional do delegado deve ter mais de um caracter"});
            }

            if(delegado.Turno.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O turno de delegado deve ter mais de um caracter"});
            }

            delegado.Status = true;
            database.delegados.Add(delegado);
            database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult("");
        }

        [HttpPatch]
        public IActionResult Patch([FromBody]Delegado delegado)
        {
            if(delegado.Id > 0)
            {
                try
                {
                    var del = database.delegados.First(d => d.Id == delegado.Id);

                    if(del != null)
                    {
                        del.Nome = delegado.Nome != null ? delegado.Nome : del.Nome;
                        del.CPF = delegado.CPF != null ? delegado.CPF : del.CPF;
                        del.Funcional = delegado.Funcional != null ? delegado.Funcional : del.Funcional;
                        del.Turno = delegado.Turno != null ? delegado.Turno : del.Turno;
                        database.SaveChanges();

                        return Ok(); 
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult("Delegado não encontrada");
                    }
                }
                catch(Exception)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult("Delegado não encontrada");
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id da delegado é inválido"});
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Delegado del = database.delegados.First(d => d.Id == id);
                del.Status = false;
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