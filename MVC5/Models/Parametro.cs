using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MVC5.Models
{
    public class Parametro
    {
        public Parametro() { }

        [Key, Required(ErrorMessage ="El campo IdParametro es obligatorio")]
        public int IdParametro { get; set; }
        [StringLength(256, ErrorMessage ="Muy largo"), Required(ErrorMessage = "El campo Valor es obligatorio")]
        public string Valor { get; set; }
    }
}