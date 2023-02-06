namespace ArqInf.Models
{
        /// <summary>
        ///  Executa a página corrente do profile do utilizador
        /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Numero de ID do pedido da "view" de erro 
        /// </summary>
        /// <return>String com numero de ID do pedido da "view" de erro  </return>
        public string? RequestId { get; set; }

        /// <summary>
        /// Booleano com informação o pedido ser null 
        /// </summary>
        /// <return> Booleano com informação o pedido ser null  </return>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}