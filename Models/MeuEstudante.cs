using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaControle.Models
{
    public class MeuEstudante
    {
        public int GruposDetalhesId { get; set; }
        public int GrupoId { get; set; }
        public Usuario Estudante { get; set; }
        public double nota { get; set; }
    }
}