using Microsoft.AspNetCore.Mvc;
using projeto.Data;
using projeto.Models;
using System;
using System.Linq;
using projeto.DTO;

namespace projeto.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LegistasController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public LegistasController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var legistas = database.legistas.Where(l => l.Status == true).ToList();
            return Ok(legistas);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var legista = database.legistas.First(l => l.Id == id);
                return Ok(legista);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                return new ObjectResult("");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]LegistaDTO legistaTemp)
        {
            if (legistaTemp.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O nome do legista deve ter mais de um caracter"});
            }

            if(legistaTemp.CRM.Length < 6 || legistaTemp.CRM.Length > 12)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O CRM deve ter de 6 a 12 digitos"});
            }

            Legista legista = new Legista();

            legista.Nome = legistaTemp.Nome;
            legista.CRM = legista.CRM;
            legista.Status = true;

            database.legistas.Add(legista);
            database.SaveChanges();

            Response.StatusCode = 201;
            return new ObjectResult("");
        }

        [HttpPatch]
        public IActionResult Patch([FromBody]LegistaDTO legistaTemp)
        {
            if(legistaTemp.Id > 0)
            {
                try
                {
                    var leg = database.legistas.First(l => l.Id == legistaTemp.Id);

                    if(leg != null)
                    {
                        leg.Nome = legistaTemp.Nome != null ? legistaTemp.Nome : leg.Nome;
                        leg.CRM = legistaTemp.CRM != null ? legistaTemp.CRM : leg.CRM;
                        database.SaveChanges();

                        return Ok(); 
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult("Legista não encontrada");
                    }
                }
                catch(Exception)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult("Legista não encontrada");
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id do legista é inválido"});
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Legista leg = database.legistas.First(l => l.Id == id);
                leg.Status = false;
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