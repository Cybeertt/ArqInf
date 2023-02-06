using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ArqInf.Models
{
    public class User : IdentityUser
    {
        /// <summary>
        /// Numero principal do funcionário 
        /// </summary>
        /// <return>Integer numero do funcionário </return>
        [PersonalData]
        public int EmployeeNumber { get; set; }
        /// <summary>
        /// Primeiro nome do funcionário 
        /// </summary>
        /// <return>String do primeiro nome do funcionário</return>
        [PersonalData]
        public string? FirstName { get; set; }
        /// <summary>
        /// Ultimo nome do funcionário 
        /// </summary>
        /// <return>String do ultimo nome do funcionário</return>
        [PersonalData]
        public string? LastName { get; set; }
        /// <summary>
        /// Data criada na inscrição do funcionário 
        /// </summary>
        /// <return>Date criada na inscrição do funcionário no sistema</return>
        [PersonalData]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Data de nascimento do funcionário 
        /// </summary>
        /// <return>Date de nascimento do funcionário </return>
        [DataType(DataType.Date)]
        [PersonalData]
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// Sexo do funcionário 
        /// </summary>
        /// <return>Char do tipo de sexo do funcionário </return>
        [PersonalData]
        public char Gender  { get; set; }
        /// <summary>
        /// Tipo de funcionário do sistema
        /// </summary>
        /// <return>Role do funcionário </return>
        [PersonalData]
        public Role? Role { get; set; }

        /// <summary>
        /// Imagem do funcionario guardado
        /// </summary>
        /// <return> String imagem do prefile </return>
        [PersonalData]
        public string? ProfilePic { get; set; }

        /// <summary>
        /// Tipo de ocupação/função do funcionario
        /// </summary>
        /// <return> Ocupação do funcionário </return>
        [PersonalData]
        public Occupation? Occupation { get; set; }

        /// <summary>
        /// Data do fim das férias do utilizador
        /// </summary>
        /// <return> DateTime da data do fim das férias do utilizador </return>
        [DataType(DataType.Date)]
        public DateTime? VacationEnd { get; set; }

        /// <summary>
        /// Data do inicio das férias do utilizador
        /// </summary>
        /// <return> DateTime da data do inicio das férias do utilizador </return>
        [DataType(DataType.Date)]
        public DateTime? VacationStart { get; set; }

        /// <summary>
        /// Dias das férias do utilizador
        /// </summary>
        /// <return> DateTime da data dos dias dados de férias do utilizador </return>
        
        [DataType(DataType.Date)]
        public DateTime? VacationDaysGiven { get; set; }

        /// <summary>
        /// Quantidade de dias de férias do utilizador
        /// </summary>
        /// <return> Integer da quantidade de dias de férias do utilizador </return>
        public int? VacationDays { get; set; }

        /// <summary>
        /// Informação se o utilizador ter férias pendentes 
        /// </summary>
        /// <return> Boolean de informação se o utilizador ter férias pendentes </return>
        public bool? VacationPendent { get; set; }

        /// <summary>
        /// Informação se o utilizador esta correntemente em férias
        /// </summary>
        /// <return> Boolean Informação se o utilizador esta correntemente em férias </return>
        public bool? InVacation { get; set; }

        /// <summary>
        /// Informação se o utilizador tem as férias aceites
        /// </summary>
        /// <return> Boolean Informação se o utilizador tem as férias aceites </return>
        public bool? VacationAccepted { get; set; }
    }
}
