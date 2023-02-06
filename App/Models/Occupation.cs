using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ArqInf.Models
{
    public class Occupation
    {
        /// <summary>
        /// Numero de ID da ocupação/função
        /// </summary>
        /// <return>Integer ID da ocupação/função </return>
        public int Id { get; set; }

        /// <summary>
        /// Nome da ocupação 
        /// </summary>
        /// <return>String Nome da ocupação </return>
        [Required(ErrorMessage = "É necessário um nome para a ocupação")]
        public string? OccupationName { get; set; }

        /// <summary>
        /// Valor de pagamento por hora para a ocupação 
        /// </summary>
        /// <return>Double Valor de pagamento por hora para a ocupação </return>
        [Required(ErrorMessage = "É necessário ter um pagamento atribuído")]
        [Range(0, double.MaxValue, ErrorMessage = "Insira um número válido para o pagamento")]
        [RegularExpression("^(-?)(0|([1-9][0-9]*))(\\.[0-9]+)?$", ErrorMessage = "Insira um número válido para o pagamento")]
        public double? PayPerHour { get; set; }
    }
}
