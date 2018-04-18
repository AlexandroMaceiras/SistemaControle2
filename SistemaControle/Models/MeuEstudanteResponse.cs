using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaControle.Models
{
    public class MeuEstudanteResponse
    {
        public double Porcentagem { get; set; }
        public List<MeuEstudante> Estudante { get; set; }
    }
}