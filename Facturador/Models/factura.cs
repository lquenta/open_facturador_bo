//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Facturador.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class factura
    {
        public int Id { get; set; }
        public string empresa_nombre { get; set; }
        public string empresa_direccion { get; set; }
        public string empresa_nit { get; set; }
        public string empresa_autorizacion { get; set; }
        public string cliente { get; set; }
        public string cliente_nit { get; set; }
        public string descripcion_factura { get; set; }
        public decimal monto_factura { get; set; }
        public string monto_literal { get; set; }
        public string codigo_control { get; set; }
        public string fecha_emision { get; set; }
        public string llave_dosificacion { get; set; }
        public string lugar { get; set; }
        public Nullable<System.DateTime> fecha_facturacion { get; set; }
    }
}