using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Data;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using projeto.DTO;
using projeto.Container;
using System.Collections.Generic;

namespace projeto.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CriminososController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public CriminososController(ApplicationDbContext database)
        {
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/api/v1/criminosos");
            HATEOAS.AddAction("GET_INFO", "GET");
            HATEOAS.AddAction("EDIT_PRODUCT", "PATCH");
            HATEOAS.AddAction("DELETE_PRODUCT", "DELETE");
        }

        [HttpGet]
        public IActionResult Get()
        {
            var criminosos = database.criminosos.Where(c => c.Status == true).Include(c => c.Crimes).ThenInclude(c => c.Vitima).ToList();

            List<CriminosoContainer> criminososHATEOAS = new List<CriminosoContainer>();
            foreach(var criminoso in criminosos)
            {
                CriminosoContainer criminosoHATEOAS = new CriminosoContainer();
                criminosoHATEOAS.criminoso = criminoso;
                criminosoHATEOAS.links = HATEOAS.GetActions(criminoso.Id.ToString());
                criminososHATEOAS.Add(criminosoHATEOAS);
            }

            return Ok(criminososHATEOAS);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Criminoso criminoso = database.criminosos.First(c => c.Id == id);

                CriminosoContainer criminosoHATEOAS = new CriminosoContainer();
                criminosoHATEOAS.criminoso = criminoso;
                criminosoHATEOAS.links = HATEOAS.GetActions(criminoso.Id.ToString());

                return Ok(criminosoHATEOAS);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id não encontrado"});
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]CriminosoDTO criminosoTemp)
        {
            try
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
            catch(Exception)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Todos campos devem ser passados"});
            }
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
                        return new ObjectResult(new {msg = "Criminoso não encontrado"});
                    }
                }
                catch(Exception)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Criminoso não encontrado"});
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
                return new ObjectResult(new {msg = "Id do criminoso é inválido"});
            }
        }
    }
}