using Microsoft.AspNetCore.Mvc;
using projeto.Data;
using projeto.Models;
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
    public class CrimeController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public CrimeController(ApplicationDbContext database)
        {
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/api/v1/crime");
            HATEOAS.AddAction("GET_INFO", "GET");
            HATEOAS.AddAction("EDIT_PRODUCT", "PATCH");
            HATEOAS.AddAction("DELETE_PRODUCT", "DELETE");
        }

        [HttpGet]
        public IActionResult Get()
        {
            var crimes = database.crimes.Include(c => c.Criminoso).Include(c => c.Vitima).ToList();
            
            List<CrimeContainer> crimesHATEOAS = new List<CrimeContainer>();
            foreach(var crime in crimes)
            {
                CrimeContainer crimeHATEOAS = new CrimeContainer();

                crimeHATEOAS.crime = crime;
                crimeHATEOAS.linksCriminoso = HATEOAS.GetActions("GetByCriminoso/" + crime.CriminosoID.ToString());
                crimeHATEOAS.linksVitima = HATEOAS.GetActions("GetByVitima/" + crime.VitimaID.ToString());
                crimeHATEOAS.linksPolicial = HATEOAS.GetActions("GetByPolicial/" + crime.PolicialID.ToString());
                crimesHATEOAS.Add(crimeHATEOAS);
            }

            return Ok(crimesHATEOAS);
        }

        [HttpGet("GetByCriminoso/{id}")]
        public IActionResult GetByCriminoso(int id)
        {
            var crimes = database.crimes.Where(c => c.CriminosoID == id).Include(c => c.Vitima).Include(c => c.Policial).ToList();
            
            List<CrimeContainer> crimesHATEOAS = new List<CrimeContainer>();
            foreach(var crime in crimes)
            {
                CrimeContainer crimeHATEOAS = new CrimeContainer();

                crimeHATEOAS.crime = crime;
                crimeHATEOAS.linksCriminoso = HATEOAS.GetActions("GetByCriminoso/" + crime.CriminosoID.ToString());
                crimeHATEOAS.linksVitima = HATEOAS.GetActions("GetByVitima/" + crime.VitimaID.ToString());
                crimeHATEOAS.linksPolicial = HATEOAS.GetActions("GetByPolicial/" + crime.PolicialID.ToString());
                crimesHATEOAS.Add(crimeHATEOAS);
            }

            return Ok(crimesHATEOAS);
        }

        [HttpGet("GetByVitima/{id}")]
        public IActionResult GetByVitima(int id)
        {
            var crimes = database.crimes.Where(c => c.VitimaID == id).Include(c => c.Criminoso).Include(c => c.Policial).ToList();
            
            List<CrimeContainer> crimesHATEOAS = new List<CrimeContainer>();
            foreach(var crime in crimes)
            {
                CrimeContainer crimeHATEOAS = new CrimeContainer();

                crimeHATEOAS.crime = crime;
                crimeHATEOAS.linksCriminoso = HATEOAS.GetActions("GetByCriminoso/" + crime.CriminosoID.ToString());
                crimeHATEOAS.linksVitima = HATEOAS.GetActions("GetByVitima/" + crime.VitimaID.ToString());
                crimeHATEOAS.linksPolicial = HATEOAS.GetActions("GetByPolicial/" + crime.PolicialID.ToString());
                crimesHATEOAS.Add(crimeHATEOAS);
            }

            return Ok(crimesHATEOAS);
        }

        [HttpGet("GetByPolicial/{id}")]
        public IActionResult GetByPolicial(int id)
        {
            var crimes = database.crimes.Where(c => c.PolicialID == id).Include(c => c.Criminoso).Include(c => c.Vitima).ToList();
            
            List<CrimeContainer> crimesHATEOAS = new List<CrimeContainer>();
            foreach(var crime in crimes)
            {
                CrimeContainer crimeHATEOAS = new CrimeContainer();

                crimeHATEOAS.crime = crime;
                crimeHATEOAS.linksCriminoso = HATEOAS.GetActions("GetByCriminoso/" + crime.CriminosoID.ToString());
                crimeHATEOAS.linksVitima = HATEOAS.GetActions("GetByVitima/" + crime.VitimaID.ToString());
                crimeHATEOAS.linksPolicial = HATEOAS.GetActions("GetByPolicial/" + crime.PolicialID.ToString());
                crimesHATEOAS.Add(crimeHATEOAS);
            }

            return Ok(crimesHATEOAS);
        }

        [HttpPost]
        public IActionResult Post([FromBody]CrimeDTO[] crimesTemp)
        {
            foreach (var crimeTemp in crimesTemp)
            {
                if (crimeTemp.CriminosoID <= 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id de criminoso é inválido"});
                }

                if(crimeTemp.VitimaID <= 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id de vitima é inválido"});
                }

                if(crimeTemp.PolicialID <= 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id de policial é inválido"});
                }

                if(crimeTemp.Data.ToString().Length < 10)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Campo data está invalido"});
                }

                if(crimeTemp.Descricao.Length <= 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Campo descrição deve ter pelo menos mais de 1 caracter"});
                }

                Crime crime = new Crime();

                crime.Data = DateTime.ParseExact(crimeTemp.Data, "dd/MM/yyyy", null);
                crime.Descricao = crimeTemp.Descricao;

                crime.Criminoso = database.criminosos.First(c => c.Id == crimeTemp.CriminosoID);
                crime.Vitima = database.vitimas.First(v => v.Id == crimeTemp.VitimaID);
                crime.Policial = database.policiais.First(p => p.Id == crimeTemp.PolicialID);

                crime.Delegacia = database.delegacias.First(d => d.Id == crimeTemp.DelegaciaID);
                crime.Delegado = database.delegados.First(d => d.Id == crimeTemp.DelegadoID);

                database.crimes.Add(crime);
                database.SaveChanges();
            }

            Response.StatusCode = 201;
            return new ObjectResult("");
        }

        [HttpPatch]
        public IActionResult Patch([FromBody]CrimeDTO[] crimesTemp)
        {
            foreach (var crimeTemp in crimesTemp)
            {
                if(crimeTemp.CriminosoID < 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id do criminoso é inválido"});
                }

                if(crimeTemp.VitimaID < 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id da vitima é inválido"});
                }

                if(crimeTemp.PolicialID <= 0)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Id de policial é inválido"});
                }

                try
                {
                    var cri = database.crimes.First(c => c.CriminosoID == crimeTemp.CriminosoID && c.VitimaID == crimeTemp.VitimaID && c.PolicialID == crimeTemp.PolicialID);

                    if(cri != null)
                    {
                        cri.Data = crimeTemp.Data == null ? cri.Data : DateTime.ParseExact(crimeTemp.Data, "dd/MM/yyyy", null);
                        cri.Descricao = crimeTemp.Descricao == null ? cri.Descricao : crimeTemp.Descricao;

                        database.SaveChanges();
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult("Crime não encontrado");
                    }
                }
                catch(Exception)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult("Crime não encontrado");
                }
            }
            return Ok(); 
        }

        [HttpDelete("{idCriminoso}/{idVitima}/{idPolicial}")]
        public IActionResult Delete(int idCriminoso, int idVitima, int idPolicial)
        {
            try
            {
                Crime cri = database.crimes.First(c => c.CriminosoID == idCriminoso && c.VitimaID == idVitima && c.PolicialID == idPolicial);
                database.crimes.Remove(cri);
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