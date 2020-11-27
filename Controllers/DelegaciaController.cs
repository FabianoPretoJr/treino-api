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
    public class DelegaciaController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.HATEOAS HATEOAS;

        public DelegaciaController(ApplicationDbContext database)
        {
            this.database = database;
            HATEOAS = new HATEOAS.HATEOAS("localhost:5001/api/v1/delegacia");
            HATEOAS.AddAction("GET_INFO", "GET");
            HATEOAS.AddAction("EDIT_PRODUCT", "PATCH");
            HATEOAS.AddAction("DELETE_PRODUCT", "DELETE");
        }

        [HttpGet]
        public IActionResult Get()
        {
            var delegacias = database.delegacias.Where(d => d.Status == true).ToList();
            
            List<DelegaciaContainer> delegaciasHATEOAS = new List<DelegaciaContainer>();
            foreach(var delegacia in delegacias)
            {
                DelegaciaContainer delegaciaHATEOAS = new DelegaciaContainer();

                delegaciaHATEOAS.delegacia = delegacia;
                delegaciaHATEOAS.links = HATEOAS.GetActions(delegacia.Id.ToString());
                delegaciasHATEOAS.Add(delegaciaHATEOAS);
            }

            return Ok(delegaciasHATEOAS);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var delegacia = database.delegacias.First(d => d.Id == id);
                
                DelegaciaContainer delegaciaHATEOAS = new DelegaciaContainer();

                delegaciaHATEOAS.delegacia = delegacia;
                delegaciaHATEOAS.links = HATEOAS.GetActions(delegacia.Id.ToString());

                return Ok(delegaciaHATEOAS);
            }
            catch(Exception)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "Id não encontrado"});
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]DelegaciaDTO delegaciaTemp)
        {
            try
            {
                if (delegaciaTemp.Endereco.Length <= 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "O endereço da delegacia deve ter mais de um caracter"});
                }

                if(delegaciaTemp.Telefone.Length < 8)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "O telefone da delegacia deve ter no mínimo 8 (oito) caracteres"});
                }

                if(delegaciaTemp.Batalhao.Length <= 1)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "O número de batalhão da delegacia deve ter mais de um caracter"});
                }

                Delegacia delegacia = new Delegacia();

                delegacia.Endereco = delegaciaTemp.Endereco;
                delegacia.Telefone = delegaciaTemp.Telefone;
                delegacia.Batalhao = delegaciaTemp.Batalhao;
                delegacia.Status = true;
                database.delegacias.Add(delegacia);
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

        public IActionResult Patch([FromBody]DelegaciaDTO delegaciaTemp)
        {
            if(delegaciaTemp.Id > 0)
            {
                try
                {
                    var del = database.delegacias.First(d => d.Id == delegaciaTemp.Id);

                    if(del != null)
                    {
                        del.Endereco = delegaciaTemp.Endereco != null ? delegaciaTemp.Endereco : del.Endereco;
                        del.Telefone = delegaciaTemp.Telefone != null ? delegaciaTemp.Telefone : del.Telefone;
                        del.Batalhao = delegaciaTemp.Batalhao != null ? delegaciaTemp.Batalhao : del.Batalhao;
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
                return new ObjectResult(new {msg = "Id da delegacia é inválido"});
            }
        }
    }
}