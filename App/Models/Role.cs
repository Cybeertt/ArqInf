
using Microsoft.AspNetCore.Identity;

namespace ArqInf.Models
    
{
    public class Role
    {
        /// <summary>
        ///  ID de cada tipo de Role do utilizador do sistema
        /// </summary>
        /// <returns>Integer com o ID do tipo de Role do utilizador</returns>
        public string? RoleId { get; set; }
        /// <summary>
        ///  Nome da role do utilizador guardada
        /// </summary>
        /// <returns>String com o nome do role do utilizador</returns>
        public string? RoleName { get; set; }

       

    }
}
