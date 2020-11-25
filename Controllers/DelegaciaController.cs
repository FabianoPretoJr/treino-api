using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Data;
using System.Linq;
using System;

namespace projeto.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DelegaciaController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public DelegaciaController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var delegacias = database.delegacias.Where(d => d.Status == true).ToList();
            return Ok(delegacias);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var delegacia = database.delegacias.First(d => d.Id == id);
                return Ok(delegacia);
            }
            catch(Exception)
            {
                Response.StatusCode = 400;
                return new ObjectResult("");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]Delegacia delegacia)
        {
            if (delegacia.Endereco.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O endereço da delegacia deve ter mais de um caracter"});
            }

            if(delegacia.Telefone.Length < 8)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O telefone da delegacia deve ter no mínimo 8 (oito) caracteres"});
            }

            if(delegacia.Batalhao.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O número de batalhão da delegacia deve ter mais de um caracter"});
            }

            delegacia.Status = true;
            database.delegacias.Add(delegacia);
            database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult("");
        }

        [HttpPatch]

        public IActionResult Patch([FromBody]Delegacia delegacia)
        {
            if(delegacia.Id > 0)
            {
                try
                {
                    var del = database.delegacias.First(d => d.Id == delegacia.Id);

                    if(del != null)
                    {
                        del.Endereco = delegacia.Endereco != null ? delegacia.Endereco : del.Endereco;
                        del.Telefone = delegacia.Telefone != null ? delegacia.Telefone : del.Telefone;
                        del.Batalhao = delegacia.Batalhao != null ? delegacia.Batalhao : del.Batalhao;
                        database.SaveChanges();

                        return Ok(); 
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult("Delegacia não encontrada");
                    }
                }
                catch(Exception)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult("Delegacia não encontrada");
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id da delegacia é inválido"});
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Delegacia del = database.delegacias.First(d => d.Id == id);
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