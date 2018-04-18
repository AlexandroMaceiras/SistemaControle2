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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SistemaControle.Classes;

namespace SistemaControle.Controllers.API
{
    [RoutePrefix("API/Usuarios")]
    public class UsuariosController : ApiController
    {
        private ControleContext db = new ControleContext();



        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login(JObject form)
        {
            string email = string.Empty;
            string password = string.Empty;
            dynamic jsonObject = form;
              
            try
            {
                email = jsonObject.Email.Value;
                password = jsonObject.Senha.Value;
            }
            catch
            {
                return this.BadRequest("Chamada Incorreta");
            }

            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.Find(email, password);

            if (userASP == null)
            {
                return this.BadRequest("Usuário ou Senha incorretos");
            }

            var user = db.Usuarios
                .Where(u => u.UserName == email)
                .FirstOrDefault();

            if (user == null)
            {
                return this.BadRequest("Usuário ou Senha incorretos ");
            }

            return this.Ok(user);
        }



        // GET: api/Usuarios
        public List<Usuario> GetUsuarios() //Aqui é Lsit e não IQueryable para trazer todos Usuarios e não somente os do BD? Query? Hã?
        {
            var usuarios = db.Usuarios.ToList();
            return usuarios;
        }

        //// GET: api/Usuarios
        //public IQueryable<Usuario> GetUsuarios()
        //{

        //    return db.Usuarios;
        //}

        // GET: api/Usuarios/5
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult GetUsuario(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        // PUT: api/Usuarios/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUsuario(int id, Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != usuario.UserId)
            {
                return BadRequest();
            }

            var db2 = new ControleContext();
            var oldUser = db2.Usuarios.Find(usuario.UserId);
            db2.Dispose();

            db.Entry(usuario).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                if (oldUser != null && oldUser.UserName != usuario.UserName)
                {
                    Utilidades.ChangeEmailUserASP(oldUser.UserName, usuario.UserName);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.Ok(usuario);
        }

       
        //public IHttpActionResult PostUsuario(UsuarioSenha usuarioSenha)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var usuario = new Usuario
        //    {
        //        Endereco = usuarioSenha.Endereco,
        //        Professor = false,
        //        Estudante = true,
        //        Sobrenome = usuarioSenha.Sobrenome,
        //        Telefone = usuarioSenha.Telefone,
        //        UserName = usuarioSenha.UserName

        //    };

        //    try
        //    {
               
                
        //        db.Usuarios.Add(usuarioSenha);
        //        db.SaveChanges();
        //        Utilidades.CreateUserASP(usuarioSenha.UserName, usuarioSenha.Senha);
                
        //    }
        //    catch (Exception ex)
        //    {

        //        ModelState.AddModelError(string.Empty, ex.Message);
        //    }

        //    usuarioSenha.UserId = usuario.UserId;
        //    usuarioSenha.Professor = false;
        //    usuarioSenha.Estudante = true;
        //    return this.Ok(usuarioSenha);
        //}


        public IHttpActionResult PostUsuario(UsuarioSenha usuarioSenha)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = new Usuario
            {
                UserId = 0,
                Endereco = usuarioSenha.Endereco,
                Professor = false,
                Estudante = true,
                Sobrenome = usuarioSenha.Sobrenome,
                Nome = usuarioSenha.Nome,
                Telefone = usuarioSenha.Telefone,
                UserName = usuarioSenha.UserName
            };

            try
            {


                db.Usuarios.Add(usuario);
                //db.Entry(usuario).State = EntityState.Detached;
                db.SaveChanges();

                //Utilidades.CreateUserASP(usuarioSenha.UserName, usuarioSenha.Senha);

                Utilidades.CreateUserASP(usuarioSenha.UserName);
                if (usuario.Estudante)
                {
                    Utilidades.AddRoleToUser(usuarioSenha.UserName, "Estudante");
                }

                if (usuario.Professor)
                {
                    Utilidades.AddRoleToUser(usuarioSenha.UserName, "Professor");
                }

            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
            }

            usuarioSenha.UserId = usuario.UserId;
            usuarioSenha.Professor = false;
            usuarioSenha.Estudante = true;
            return this.Ok(usuarioSenha);
        }


        // DELETE: api/Usuarios/5
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult DeleteUsuario(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }

            db.Usuarios.Remove(usuario);
            db.SaveChanges();

            return Ok(usuario);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsuarioExists(int id)
        {
            return db.Usuarios.Count(e => e.UserId == id) > 0;
        }
    }
}