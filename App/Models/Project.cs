using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ArqInf.Models
{
    public class Project
    {
        /// <summary>
        /// Numero de ID do projeto 
        /// </summary>
        /// <return>Integer numero do ID do projeto </return>
        public int Id { get; set; }

        /// <summary>
        /// Nome do projeto com a mensagem de erro associado
        /// </summary>
        /// <return>String do nome do projeto</return>
        [Required(ErrorMessage = "É necessário um nome para o projeto")]
        public string? ProjectName { get; set; }

        /// <summary>
        /// Data de começo do projeto com a mensagem de erro associada
        /// </summary>
        /// <return>DateTime com a data do inicio do projeto</return>
        [Required(ErrorMessage = "É necessária uma data de começo")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Data do limite do projeto com a mensagem de erro associada
        /// </summary>
        /// <return>DateTime com a data do limite do projeto</return>
        [Required(ErrorMessage = "É necessária uma data limite")]
        [DataType(DataType.DateTime)]
        public DateTime LimitDate { get; set; }

        /// <summary>
        /// Data de fim do projeto
        /// </summary>
        /// <return>DateTime com a data do fim do projeto</return>
        [DataType(DataType.DateTime)]
        public DateTime FinishDate { get; set; }

        /// <summary>
        /// Descrição geral do projeto com a mensagem de erro associada
        /// </summary>
        /// <return>String da Descrição geral do projeto</return>
        [Required(ErrorMessage = "É necessária uma descrição")]
        public String? Description { get; set; }

        /// <summary>
        /// Utilizador guardado como manager do projeto 
        /// </summary>
        /// <return>User associado ao gerente do projeto </return>
        public User? ProjectManager { get; set; }

        /// <summary>
        /// Orçamento geral do projeto com as mensagems de erro
        /// </summary>
        /// <return>Double Orçamento atribuido ao projeto </return>
        [Required(ErrorMessage = "É necessário ter orçamento atribuído")]
        [Range(0, double.MaxValue, ErrorMessage = "Insira um número válido para o orçamento")]
        [RegularExpression("^(-?)(0|([1-9][0-9]*))(\\.[0-9]+)?$", ErrorMessage = "Insira um número válido para o orçamento")]
        public double? Budget { get; set; }

        /// <summary>
        /// Orçamento gastado no percurso do projeto
        /// </summary>
        /// <return>Double Orçamento gastado no percurso do projeto </return>
        public double? MoneySpent { get; set; }
    }
}
