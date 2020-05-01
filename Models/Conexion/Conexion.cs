﻿using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Conexion
{
    public class Conexion : DataConnection
    {
        // PDHN2 = Para conectarse a la Base De Datos de SQL Server.
        public Conexion() : base("PDHN2") { }
        public ITable<TClientes> TClientes { get { return GetTable<TClientes>(); } }
        public ITable<TReportes_clientes> TReportes_clientes { get { return GetTable<TReportes_clientes>(); } }
    }
}
