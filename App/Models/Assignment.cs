using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ArqInf.Models
{
    public class Assignment
    {
        /// <summary>
        /// Numero de ID da tarefa 
        /// </summary>
        /// <return>Integer com numero de ID da tarefa  </return>
        public int Id { get; set; }

        /// <summary>
        /// Nome da tarefa
        /// </summary>
        /// <return>String com o nome da tarefa  </return>
        [Required(ErrorMessage = "É necessário um nome para a tarefa")]
        public string? AssignmentName { get; set; }

        /// <summary>
        /// Data limite da tarefa com a mensagem de erro associada e mensagem de erro
        /// </summary>
        /// <return>DateTime com a data do limite da tarefa</return>
        [Required(ErrorMessage = "É necessária uma data limite")]
        [DataType(DataType.DateTime)]
        public DateTime LimitDate { get; set; }

        /// <summary>
        /// Horas associadas a tarefa
        /// </summary>
        /// <return>Intenger com as horas associadas a tarefa</return>
        [Required(ErrorMessage = "É necessário ter horas atribuídas")]
        [Range(1, int.MaxValue, ErrorMessage = "Insira um número válido de horas")]
        public int? AssignedHours { get; set; }

        /// <summary>
        /// Data de acabo da tarefa 
        /// </summary>
        /// <return>DateTime com a data de acabo da tarefa</return>
        [DataType(DataType.DateTime)]
        public DateTime FinishDate { get; set; }

        /// <summary>
        /// Descrição geral do tarefa com a mensagem de erro associada
        /// </summary>
        /// <return>String da Descrição geral da tarefa</return>
        [Required(ErrorMessage = "É necessária uma descrição")]
        public String? Description { get; set; }

        /// <summary>
        /// Utilizador associado a tarefa
        /// </summary>
        /// <return>User que esta associado a tarefa </return>
        public User? Assigner { get; set; }

        /// <summary>
        /// Orçamento geral da tarefa 
        /// </summary>
        /// <return>Double Orçamento atribuido a tarefa </return>
        public double? Budget { get; set; }
    }
}
