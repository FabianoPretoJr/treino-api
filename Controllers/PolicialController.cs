using Microsoft.AspNetCore.Mvc;
using projeto.Models;
using projeto.Data;
using System.Linq;
using System;
using projeto.DTO;
using projeto.Container;
using System.Collections.Generic;

namespace projeto.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PolicialController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public PolicialController(ApplicationDbContext database)
        {
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/api/v1/policial");
            HATEOAS.AddAction("GET_INFO", "GET");
            HATEOAS.AddAction("EDIT_PRODUCT", "PATCH");
            HATEOAS.AddAction("DELETE_PRODUCT", "DELETE");
        }

        [HttpGet]
        public IActionResult Get()
        {
            var policiais = database.policiais.Where(p => p.Status == true).ToList();
            
            List<PolicialContainer> policiaisHATEOAS = new List<PolicialContainer>();
            foreach(var policial in policiais)
            {
                PolicialContainer policialHATEOAS = new PolicialContainer();

                policialHATEOAS.policial = policial;
                policialHATEOAS.links = HATEOAS.GetActions(policial.Id.ToString());
                policiaisHATEOAS.Add(policialHATEOAS);
            }

            return Ok(policiaisHATEOAS);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var policial = database.policiais.First(p => p.Id == id);
                
                PolicialContainer policialHATEOAS = new PolicialContainer();

                policialHATEOAS.policial = policial;
                policialHATEOAS.links = HATEOAS.GetActions(policial.Id.ToString());

                return Ok(policialHATEOAS);
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id não encontrado"});
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]PolicialDTO policialTemp)
        {
            try
            {
                if (policialTemp.Nome.Length <= 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "O nome do policial deve ter mais de um caracter"});
                }
                if(policialTemp.Funcional.Length <= 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "A funcional do policial deve ter mais de um caracter"});
                }
                if(policialTemp.Patente.Length <= 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "A patente do policial deve ter mais de um caracter"});
                }

                Policial policial = new Policial();

                policial.Nome = policialTemp.Nome;
                policial.Funcional = policialTemp.Funcional;
                policial.Patente = policialTemp.Patente;
                policial.Status = true;

                database.policiais.Add(policial);
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
        public IActionResult Patch([FromBody]PolicialDTO policialTemp)
        {
            if(policialTemp.Id > 0)
            {
                try
                {
                    var pol = database.policiais.First(p => p.Id == policialTemp.Id);

                    if(pol != null)
                    {
                        pol.Nome = policialTemp.Nome != null ? policialTemp.Nome : pol.Nome;
                        pol.Funcional = policialTemp.Funcional != null ? policialTemp.Funcional : pol.Funcional;
                        pol.Patente = policialTemp.Patente != null ? policialTemp.Patente : pol.Patente;
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
                return new ObjectResult(new {msg = "Id do policial é inválido"});
            }
        }
    }
}