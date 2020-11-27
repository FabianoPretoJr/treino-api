using System.Linq;
using Microsoft.AspNetCore.Mvc;
using projeto.Data;
using projeto.Models;
using Microsoft.EntityFrameworkCore;
using System;
using projeto.DTO;
using projeto.Container;
using System.Collections.Generic;

namespace projeto.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AutopsiaController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public AutopsiaController(ApplicationDbContext database)
        {
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/api/v1/autopsia");
            HATEOAS.AddAction("GET_INFO", "GET");
            HATEOAS.AddAction("EDIT_PRODUCT", "PATCH");
            HATEOAS.AddAction("DELETE_PRODUCT", "DELETE");
        }

        [HttpGet]
        public IActionResult Get()
        {
            var autopsias = database.autopsias.Include(a => a.Vitima).Include(a => a.Legista).ToList();
            
            List<AutopsiaContainer> autopsiasHATEOAS = new List<AutopsiaContainer>();
            foreach(var autopsia in autopsias)
            {
                AutopsiaContainer autopsiaHATEOAS = new AutopsiaContainer();

                autopsiaHATEOAS.autopsia = autopsia;
                autopsiaHATEOAS.linksVitima = HATEOAS.GetActions("GetByVitima/" + autopsia.VitimaID.ToString());
                autopsiaHATEOAS.linksLegista = HATEOAS.GetActions("GetByLegista/" + autopsia.LegistaID.ToString());
                autopsiasHATEOAS.Add(autopsiaHATEOAS);
            }

            return Ok(autopsiasHATEOAS);
        }

        [HttpGet("GetByVitima/{id}")]
        public IActionResult GetByVitima(int id)
        {
            var autopsias = database.autopsias.Where(a => a.VitimaID == id).Include(a => a.Legista).ToList();

            if(autopsias.Count != 0)
            {
                List<AutopsiaContainer> autopsiasHATEOAS = new List<AutopsiaContainer>();
                foreach(var autopsia in autopsias)
                {
                    AutopsiaContainer autopsiaHATEOAS = new AutopsiaContainer();

                    autopsiaHATEOAS.autopsia = autopsia;
                    autopsiaHATEOAS.linksVitima = HATEOAS.GetActions("GetByVitima/" + autopsia.VitimaID.ToString());
                    autopsiaHATEOAS.linksLegista = HATEOAS.GetActions("GetByLegista/" + autopsia.LegistaID.ToString());
                    autopsiasHATEOAS.Add(autopsiaHATEOAS);
                }

                return Ok(autopsiasHATEOAS);
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id não encontrado"});
            }
        }

        [HttpGet("GetByLegista/{id}")]
        public IActionResult GetByLegista(int id)
        {
            var autopsias = database.autopsias.Where(a => a.LegistaID == id).Include(a => a.Vitima).ToList();

            if(autopsias.Count != 0)
            {
                List<AutopsiaContainer> autopsiasHATEOAS = new List<AutopsiaContainer>();
                foreach(var autopsia in autopsias)
                {
                    AutopsiaContainer autopsiaHATEOAS = new AutopsiaContainer();

                    autopsiaHATEOAS.autopsia = autopsia;
                    autopsiaHATEOAS.linksVitima = HATEOAS.GetActions("GetByVitima/" + autopsia.VitimaID.ToString());
                    autopsiaHATEOAS.linksLegista = HATEOAS.GetActions("GetByLegista/" + autopsia.LegistaID.ToString());
                    autopsiasHATEOAS.Add(autopsiaHATEOAS);
                }

                return Ok(autopsiasHATEOAS);
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id não encontrado"});
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]AutopsiaDTO[] autopsiasTemp)
        {
            try
            {
                foreach (var autopsiaTemp in autopsiasTemp)
                {
                    if (autopsiaTemp.LegistaID <= 0)
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Id do legista inválido"});
                    }

                    if(autopsiaTemp.VitimaID <= 0)
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Id de vitima inválido"});
                    }

                    if(autopsiaTemp.Data.ToString().Length < 10)
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Campo data está invalido"});
                    }

                    if(autopsiaTemp.Laudo.Length <= 1)
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Campo laudo deve ter pelo menos mais de 1 caracter"});
                    }

                    Autopsia autopsia = new Autopsia();

                    autopsia.Data = DateTime.ParseExact(autopsiaTemp.Data, "dd/MM/yyyy", null);
                    autopsia.Laudo = autopsiaTemp.Laudo;
                    autopsia.Legista = database.legistas.First(c => c.Id == autopsiaTemp.LegistaID);
                    autopsia.Vitima = database.vitimas.First(v => v.Id == autopsiaTemp.VitimaID);

                    database.autopsias.Add(autopsia);
                    database.SaveChanges();
                }

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
        public IActionResult Patch([FromBody]AutopsiaDTO[] autopsiasTemp)
        {
            foreach (var autopsiaTemp in autopsiasTemp)
            {
                if(autopsiaTemp.LegistaID < 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id do legista é inválido"});
                }
                if(autopsiaTemp.VitimaID < 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id da vitima é inválido"});
                }

                try
                {
                    var aut = database.autopsias.First(c => c.LegistaID == autopsiaTemp.LegistaID && c.VitimaID == autopsiaTemp.VitimaID);

                    if(aut != null)
                    {
                        aut.Data = autopsiaTemp.Data == null ? aut.Data : DateTime.ParseExact(autopsiaTemp.Data, "dd/MM/yyyy", null);
                        aut.Laudo = autopsiaTemp.Laudo == null ? aut.Laudo : autopsiaTemp.Laudo;

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
                return new ObjectResult(new {msg = "Id's estão inválidos"});
            }
        }
    }
}