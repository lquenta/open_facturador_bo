using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Facturador.Models;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace Facturador.Controllers
{
    public class facturasController : Controller
    {
        private facturasEntities db = new facturasEntities();

        // GET: facturas
        public ActionResult Index()
        {
            return View(db.facturas.ToList());
        }

        // GET: facturas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            factura factura = db.facturas.Find(id);

            if (factura == null)
            {
                return HttpNotFound();
            }
             try
                {
                    
                    
                    //envio de factura
                    LocalReport lr = new LocalReport();
                    string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"Reportes\", "factura.rdlc");
                    string codigoQR = factura.empresa_nit + "|" + factura.Id.ToString() + "|" + factura.empresa_autorizacion + "|" + factura.fecha_facturacion.Value.ToString("yyyyMMdd") + "|" + factura.monto_factura.ToString() + "|" + factura.codigo_control + "|" + factura.cliente_nit + "|" + "0|0|0|0";
                    string qrCodigo = "http://qrickit.com/api/qr?d=" + codigoQR;

                    lr.ReportPath = path;
                    lr.EnableExternalImages = true;
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-ES");
                    lr.SetParameters(new ReportParameter("ParamEmpresaNombre", factura.empresa_nombre));
                    lr.SetParameters(new ReportParameter("ParamDireccion", factura.empresa_direccion));
                    lr.SetParameters(new ReportParameter("ParamNitEmpresa", factura.empresa_nit));
                    lr.SetParameters(new ReportParameter("ParamNroFactura", factura.Id.ToString()));
                    lr.SetParameters(new ReportParameter("ParamNumeroAutorizacion", factura.empresa_autorizacion));
                    lr.SetParameters(new ReportParameter("ParamRubroEmpresa", "SERVICIOS INFORMATICOS"));
                    lr.SetParameters(new ReportParameter("ParamLugarFecha", factura.lugar + "," + factura.fecha_facturacion.Value.ToLongDateString()));
                    lr.SetParameters(new ReportParameter("ParamCliente", factura.cliente));
                    lr.SetParameters(new ReportParameter("ParamNitCliente", factura.cliente_nit));
                    lr.SetParameters(new ReportParameter("ParamDescripcionFactura", factura.descripcion_factura));
                    lr.SetParameters(new ReportParameter("ParamMonto", factura.monto_factura.ToString()));
                    lr.SetParameters(new ReportParameter("ParamMontoLiteral", NumLetra.Convertir(factura.monto_factura.ToString(), false)));
                    lr.SetParameters(new ReportParameter("ParamCodigoControl", factura.codigo_control));
                    lr.SetParameters(new ReportParameter("ParamFechaEmision", factura.fecha_emision));
                    lr.SetParameters(new ReportParameter("ParamURLQR", qrCodigo));

                    string reportType = "PDF"; //puede ser PDF,Excel,Word,Image
                    string mimeType;
                    string encoding;
                    string fileNameExtension;
                    string deviceInfo =
                    "<DeviceInfo>" +
                    "  <OutputFormat>" + reportType + "</OutputFormat>" +
                    "  <PageWidth>8.5in</PageWidth>" +
                    "  <PageHeight>11in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>0in</MarginLeft>" +
                    "  <MarginRight>0in</MarginRight>" +
                    "  <MarginBottom>0.5in</MarginBottom>" +
                    "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = lr.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);
                  
                    var memoryStream = new MemoryStream(renderedBytes);
                    Response.AppendHeader("content-disposition", "inline; filename=factura.pdf");
                    return new FileStreamResult(memoryStream, "application/pdf");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
                catch (Exception)
                {
                    
                    throw;
                }
                
            
            return View(factura);
        }

        // GET: facturas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: facturas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "empresa_nombre,empresa_direccion,empresa_nit,empresa_autorizacion,lugar,fecha_facturacion,cliente,cliente_nit,descripcion_factura,monto_factura,llave_dosificacion")] factura factura)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int last_id = 1;
                    var ultimo_id = db.facturas.ToArray();
                    if (ultimo_id.Count() > 0)
                    {
                        last_id = ultimo_id.Last().Id + 1;
                    }

                    //envio de factura
                    LocalReport lr = new LocalReport();
                    string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"Reportes\", "factura.rdlc");
                    string fechaEmision = DateTime.Now.AddMonths(3).ToString("dd/MM/yyyy");
                    string codigoControl = Helper.Obtener_codigo_control(factura.empresa_autorizacion, factura.Id.ToString(), factura.cliente_nit, factura.fecha_facturacion.Value.ToString("yyyyMMdd"), factura.monto_factura.ToString(), factura.llave_dosificacion);
                    string codigoQR = factura.empresa_nit + "|" + factura.Id.ToString() + "|" + factura.empresa_autorizacion + "|" + factura.fecha_facturacion.Value.ToString("yyyyMMdd") + "|" + factura.monto_factura.ToString() + "|" + codigoControl + "|" + factura.cliente_nit + "|" + "0|0|0|0";
                    string qrCodigo = "http://qrickit.com/api/qr?d=" + codigoQR;

                    lr.ReportPath = path;
                    lr.EnableExternalImages = true;
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-ES");
                    lr.SetParameters(new ReportParameter("ParamEmpresaNombre", factura.empresa_nombre));
                    lr.SetParameters(new ReportParameter("ParamDireccion", factura.empresa_direccion));
                    lr.SetParameters(new ReportParameter("ParamNitEmpresa", factura.empresa_nit));
                    lr.SetParameters(new ReportParameter("ParamNroFactura", last_id.ToString()));
                    lr.SetParameters(new ReportParameter("ParamNumeroAutorizacion", factura.empresa_autorizacion));
                    lr.SetParameters(new ReportParameter("ParamRubroEmpresa", "SERVICIOS INFORMATICOS"));
                    lr.SetParameters(new ReportParameter("ParamLugarFecha", factura.lugar + "," + factura.fecha_facturacion.Value.ToLongDateString()));
                    lr.SetParameters(new ReportParameter("ParamCliente", factura.cliente));
                    lr.SetParameters(new ReportParameter("ParamNitCliente", factura.cliente_nit));
                    lr.SetParameters(new ReportParameter("ParamDescripcionFactura", factura.descripcion_factura));
                    lr.SetParameters(new ReportParameter("ParamMonto", factura.monto_factura.ToString()));
                    lr.SetParameters(new ReportParameter("ParamMontoLiteral", NumLetra.Convertir(factura.monto_factura.ToString(), false)));
                    lr.SetParameters(new ReportParameter("ParamCodigoControl", codigoControl));
                    lr.SetParameters(new ReportParameter("ParamFechaEmision", fechaEmision));
                    lr.SetParameters(new ReportParameter("ParamURLQR", qrCodigo));

                    string reportType = "PDF"; //puede ser PDF,Excel,Word,Image
                    string mimeType;
                    string encoding;
                    string fileNameExtension;
                    string deviceInfo =
                    "<DeviceInfo>" +
                    "  <OutputFormat>" + reportType + "</OutputFormat>" +
                    "  <PageWidth>8.5in</PageWidth>" +
                    "  <PageHeight>11in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>0in</MarginLeft>" +
                    "  <MarginRight>0in</MarginRight>" +
                    "  <MarginBottom>0.5in</MarginBottom>" +
                    "</DeviceInfo>";

                    Warning[] warnings;
                    string[] streams;
                    byte[] renderedBytes;

                    renderedBytes = lr.Render(
                        reportType,
                        deviceInfo,
                        out mimeType,
                        out encoding,
                        out fileNameExtension,
                        out streams,
                        out warnings);
                    factura.monto_literal = NumLetra.Convertir(factura.monto_factura.ToString(), false);
                    factura.codigo_control = codigoControl;
                    factura.fecha_emision = fechaEmision;
                    db.facturas.Add(factura);
                    db.SaveChanges();

                    var memoryStream = new MemoryStream(renderedBytes);
                    Response.AppendHeader("content-disposition", "inline; filename=factura.pdf");
                    return new FileStreamResult(memoryStream, "application/pdf");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
                catch (Exception)
                {
                    
                    throw;
                }
                
            }
            return RedirectToAction("Index");
            
        }

        // GET: facturas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            factura factura = db.facturas.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // POST: facturas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,empresa_nombre,empresa_direccion,empresa_nit,empresa_autorizacion,lugar,fecha_facturacion,cliente,cliente_nit,descripcion_factura,monto_factura,monto_literal,codigo_control,fecha_emision,llave_dosificacion")] factura factura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(factura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(factura);
        }

        // GET: facturas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            factura factura = db.facturas.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // POST: facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            factura factura = db.facturas.Find(id);
            db.facturas.Remove(factura);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
