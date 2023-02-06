using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ArqInf.Models
{
    public class ProjectAssignments
    {
        /// <summary>
        ///  ID de cada projeto diferente
        /// </summary>
        /// <returns> Integer com o ID do projeto associado </returns>
        public int Id { get; set; }

        /// <summary>
        ///  Projeto que esta associado ao trabalho
        /// </summary>
        /// <returns> Projeto que esta associado ao trabalho </returns>
        public Project? Project { get; set; }

        /// <summary>
        ///  Trabalho que vai estar associado ao projeto
        /// </summary>
        /// <returns> Trabalho que vai estar associado ao projeto</returns>
        public Assignment? Assignment { get; set; }
    
    }
}
