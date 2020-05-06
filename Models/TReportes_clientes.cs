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
        public int IdReportes { set; get; }
        // EXEC sp_rename 'TReportes_clientes.IdRegistro','IdReportes','COLUMN';
        public decimal UltimoPago { set; get; }
        public string FechaPago { set; get; }
        public decimal DeudaActual { set; get; }
        public string FechaDeuda { set; get; }
        public string Ticket { set; get; }
        public string FechaLimite { set; get; }
        public int IdCliente { set; get; }
    }
}
