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
    public class CriminososController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public CriminososController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var criminosos = database.criminosos.Where(c => c.Status == true).Include(c => c.Crimes).ThenInclude(c => c.Vitima).ToList();
            return Ok(criminosos);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Criminoso criminoso = database.criminosos.First(c => c.Id == id);
                return Ok(criminoso);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                return new ObjectResult("");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]CriminosoDTO criminosoTemp)
        {
            if (criminosoTemp.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O nome do criminoso deve ter mais de um caracter"});
            }

            if(criminosoTemp.CPF.Length != 11)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O CPF deve ter 11 digitos"});
            }

            Criminoso criminoso = new Criminoso();

            criminoso.Nome = criminosoTemp.Nome;
            criminoso.CPF = criminosoTemp.CPF;
            criminoso.Status = true;
            database.criminosos.Add(criminoso);
            database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult("");
        }

        [HttpPatch]
        public IActionResult Patch([FromBody]CriminosoDTO criminosoTemp)
        {
            if(criminosoTemp.Id > 0)
            {
                try
                {
                    var cri = database.criminosos.First(c => c.Id == criminosoTemp.Id);

                    if(cri != null)
                    {
                        cri.Nome = criminosoTemp.Nome != null ? criminosoTemp.Nome : cri.Nome;
                        cri.CPF = criminosoTemp.CPF != null ? criminosoTemp.CPF : cri.CPF;
                        database.SaveChanges();

                        return Ok(); 
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult("Criminoso não encontrado");
                    }
                }
                catch(Exception)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult("Criminoso não encontrado");
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id do criminoso é inválido"});
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Criminoso cri = database.criminosos.First(c => c.Id == id);
                cri.Status = false;
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