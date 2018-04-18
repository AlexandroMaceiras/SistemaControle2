using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SistemaControle.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SistemaControle.Controllers.API
{
    [RoutePrefix("API/Grupos")]
    public class GruposController : ApiController
    {
        private ControleContext db = new ControleContext();

        //CAPTURAR Estudantes
        [HttpGet]
        [Route("GetEstudantes/{grupoId}")]
        public IHttpActionResult GetEstudantes(int grupoId)
        {
            var estudantes = db.GruposDetalhes.Where(gd => gd.GrupoId == grupoId).ToList();
            var meuEstudantes = new List<object>();
            foreach(var estudante in estudantes)
            {
                var meuEstudante = db.Usuarios.Find(estudante.UserId);
                meuEstudantes.Add(new
                {
                    GruposDetalhes = estudante.GruposDetalhesId,
                    Grupos = estudante.GrupoId,
                    Estudante = meuEstudante
                });
            }

            return Ok(meuEstudantes);

        }


        //CAPTURAR NOTAS
        [HttpGet]
        [Route("GetNotas/{grupoId}/{userId}")]
        public IHttpActionResult GetNotas(int grupoId, int userId)
        {
            var notaDef = 0.0;
            var notas = db.GruposDetalhes.Where(gd => gd.GrupoId == grupoId && gd.UserId == userId).ToList();
            foreach(var nota in notas)
            {
                foreach(var nota2 in nota.Notas)
                {
                    notaDef += nota2.Percentual + nota2.Nota;
                }
            }


            return Ok<object>(new { Notas = notaDef});
                
        }


        //METODO PARA NOTAS
        [HttpPost]
        [Route("SalvarNotas")]
        public IHttpActionResult SalvarNotas(JObject form)
        {
            var meuEstudanteResponse = JsonConvert.DeserializeObject < MeuEstudanteResponse > (form.ToString());
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach(var estudante in meuEstudanteResponse.Estudante)
                    {
                        var nota = new Notas
                        {
                            GruposDetalhesId = estudante.GruposDetalhesId,
                            Percentual = (float)meuEstudanteResponse.Porcentagem,
                            Nota = (float)estudante.nota
                        };

                        db.Notas.Add(nota);
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    return Ok(true);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }

           
        }


        //GET PERSONALIZADO
        [Route("GetGrupos/{userId}")]
        public IHttpActionResult GetGrupos(int userId)
        {
            var grupos = db.Grupos.Where(g => g.UserId == userId).ToList();
            var objetos = db.GruposDetalhes.Where(gd => gd.UserId == userId).ToList();
            //var materias = new List<Grupos>();

            //foreach(var objeto in objetos){
            //    materias.Add(db.Grupos.Find(objeto.GrupoId));
            //}


            var resposta = new
            {
                MateriasProf = grupos,
                MatriculadoEm = objetos
            };

            return Ok(resposta);
        }


        // GET: api/Grupos
        public IQueryable<Grupos> GetGrupos()
        {
            return db.Grupos;
        }

      

        // PUT: api/Grupos/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGrupos(int id, Grupos grupos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != grupos.GrupoId)
            {
                return BadRequest();
            }

            db.Entry(grupos).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GruposExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Grupos
        [ResponseType(typeof(Grupos))]
        public IHttpActionResult PostGrupos(Grupos grupos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Grupos.Add(grupos);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = grupos.GrupoId }, grupos);
        }

        // DELETE: api/Grupos/5
        [ResponseType(typeof(Grupos))]
        public IHttpActionResult DeleteGrupos(int id)
        {
            Grupos grupos = db.Grupos.Find(id);
            if (grupos == null)
            {
                return NotFound();
            }

            db.Grupos.Remove(grupos);
            db.SaveChanges();

            return Ok(grupos);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GruposExists(int id)
        {
            return db.Grupos.Count(e => e.GrupoId == id) > 0;
        }
    }
}