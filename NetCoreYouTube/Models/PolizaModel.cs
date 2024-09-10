namespace NetCoreYouTube.Models
{
    public class PolizaModel
    {
        public string ramo { get; set; }
        public string cod_producto { get; set; }
        public string poliza { get; set; }
        public string token { get; set; }
    }
    public partial class PolizaResponse
    {
        public bool Error { get; set; }
        public long Numbermsg { get; set; }

        public string Msg { get; set; }
        public List<Poliza> Data { get; set; }
    }
    public partial class BeneficiariosResponse
    {
        public bool Error { get; set; }
        public long Numbermsg { get; set; }

        public string Msg { get; set; }
        public List<Beneficiarios> Data { get; set; }
    }
    public partial class PolizaDetalleResponse
    {
        public bool Error { get; set; }
        public long Numbermsg { get; set; }

        public string Msg { get; set; }
        public List<PolizaDetalle> Data { get; set; }
    }

    public partial class Poliza
    {
        public string CodProd { get; set; }
        public string NumPoliza { get; set; }
        public long NumEndoso { get; set; }
        public string CodEstado { get; set; }
        public string EstadoPol { get; set; }
        public long CodFrecuenciaPagoRenta { get; set; }
        public string FrecuenciaPagoRenta { get; set; }
        public string Prima { get; set; }
        public string CodMoneda { get; set; }
        public string Moneda { get; set; }
        public long PensionEscalonada { get; set; }
        public long PrcDevolucion { get; set; }
        public long MtoDevolucion { get; set; }
        public DateTime FecIniVigencia { get; set; }
        public DateTime FecFinVigencia { get; set; }
        public DateTime FecAbono { get; set; }
        public DateTime FecDev { get; set; }
        public long Diferimiento { get; set; }
        public long Tmp { get; set; }
        public long Garantizado { get; set; }
        public double Pension { get; set; }
        public long PrcSegVida { get; set; }
        public long MtoSegVid { get; set; }
        public long PrcSegAcc { get; set; }
        public long MtoSegAcc { get; set; }
        public long PrcAjusteAnual { get; set; }
        public string Sepelio { get; set; }
        public string Gratificacion { get; set; }
        public string MenorEdad { get; set; }
    }
    public partial class Beneficiarios
    {
        public string NumPoliza { get; set; }
        public string Nombre { get; set; }
        public long CodTipoIden { get; set; }
        public string TipoIden { get; set; }
        public string NumTipoIden { get; set; }
        public string CodParentesco { get; set; }
        public string Parentesco { get; set; }
        public long PrcPension { get; set; }
    }
    public partial class PolizaDetalle
    {
        public string NumSolicitud { get; set; }
        public string NumPoliza { get; set; }
        public string NombreContratante { get; set; }
        public string NombreAsegurado { get; set; }
        public long CodTipoIdenAse { get; set; }
        public string TipoIdenAse { get; set; }
        public string NumTipoIdenAse { get; set; }
        public long NumEndoso { get; set; }
    }
    public partial class EquiUsuario
    {
        public string dni { get; set; }
        public string token { get; set; }
    }
    public class PolizaInfPagoModel
    {
        public string ramo { get; set; }
        public string cod_producto { get; set; }
        public string poliza { get; set; }
        public string cod_submotivo { get; set; }
        public string token { get; set; }
    }
    public class PFDEndosoModel
    {
        public string ramo { get; set; }
        public string cod_producto { get; set; }
        public string poliza { get; set; }
        public string tramite { get; set; }
        public string token { get; set; }
    }
    public class ErrorResponse
    {
        public bool Error { get; set; }
        public int NumberMsg { get; set; }
        public string Msg { get; set; }
        public string Data { get; set; }
    }
}
