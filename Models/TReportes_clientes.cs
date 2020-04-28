using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TReportes_clientes
    {
        [PrimaryKey, Identity]
        public int IdReport { set; get; }
        public decimal UltimoPago { set; get; }
        public string FecahaPago { set; get; }
        public decimal Deuda { set; get; }
        public string FechaDeuda { set; get; }
        public string Ticket { set; get; }
        public string FechaLimite { set; get; }
        public int IdCliente { set; get; }
    }
}
