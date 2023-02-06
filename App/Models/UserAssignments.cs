using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ArqInf.Models
{
    public class UserAssignments
    {
        /// <summary>
        /// Numero de ID principal do funcionário 
        /// </summary>
        /// <return>Integer ID do funcionário </return>
        public int Id { get; set; }

        /// <summary>
        /// Utilizador 
        /// </summary>
        /// <return> User Objecto do utizador </return>
        public User? User { get; set; }

        /// <summary>
        /// Trabalho associado ao utilizador
        /// </summary>
        /// <return>Trabalho em especificado ao utilizador </return>
        public Assignment? Assignment { get; set; }       
    }
}
