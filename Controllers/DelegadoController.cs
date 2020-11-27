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
    public class DelegadoController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public DelegadoController(ApplicationDbContext database)
        {
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/api/v1/delegado");
            HATEOAS.AddAction("GET_INFO", "GET");
            HATEOAS.AddAction("EDIT_PRODUCT", "PATCH");
            HATEOAS.AddAction("DELETE_PRODUCT", "DELETE");
        }

        [HttpGet]
        public IActionResult Get()
        {
            var delegados = database.delegados.Where(d => d.Status == true).ToList();
            
            List<DelegadoContainer> delegadosHATEOAS = new List<DelegadoContainer>();
            foreach(var delegado in delegados)
            {
                DelegadoContainer delegadoHATEOAS = new DelegadoContainer();

                delegadoHATEOAS.delegado = delegado;
                delegadoHATEOAS.links = HATEOAS.GetActions(delegado.Id.ToString());
                delegadosHATEOAS.Add(delegadoHATEOAS);
            }

            return Ok(delegadosHATEOAS);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var delegado = database.delegados.First(d => d.Id == id);
                
                DelegadoContainer delegadoHATEOAS = new DelegadoContainer();

                delegadoHATEOAS.delegado = delegado;
                delegadoHATEOAS.links = HATEOAS.GetActions(delegado.Id.ToString());
            

                return Ok(delegadoHATEOAS);
            }
            catch(Exception)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id não encontrado"});
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]DelegadoDTO delegadoTemp)
        {
            try
            {
                if (delegadoTemp.Nome.Length <= 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "O nome de delegado deve ter mais de um caracter"});
                }

                if(delegadoTemp.CPF.Length != 11)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "O CPF deve ter 11 (onze) digitos"});
                }

                if(delegadoTemp.Funcional.Length <= 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "A funcional do delegado deve ter mais de um caracter"});
                }

                if(delegadoTemp.Turno.Length <= 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "O turno de delegado deve ter mais de um caracter"});
                }

                Delegado delegado = new Delegado();

                delegado.Nome = delegadoTemp.Nome;
                delegado.CPF = delegadoTemp.CPF;
                delegado.Funcional = delegadoTemp.Funcional;
                delegado.Turno = delegadoTemp.Turno;
                delegado.Status = true;

                database.delegados.Add(delegado);
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
        public IActionResult Patch([FromBody]DelegadoDTO delegadoTemp)
        {
            if(delegadoTemp.Id > 0)
            {
                try
                {
                    var del = database.delegados.First(d => d.Id == delegadoTemp.Id);

                    if(del != null)
                    {
                        del.Nome = delegadoTemp.Nome != null ? delegadoTemp.Nome : del.Nome;
                        del.CPF = delegadoTemp.CPF != null ? delegadoTemp.CPF : del.CPF;
                        del.Funcional = delegadoTemp.Funcional != null ? delegadoTemp.Funcional : del.Funcional;
                        del.Turno = delegadoTemp.Turno != null ? delegadoTemp.Turno : del.Turno;
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
                return new ObjectResult(new {msg = "Id do delegado é inválido"});
            }
        }
    }
}