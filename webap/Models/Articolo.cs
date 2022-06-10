using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ica.SalesOrders.Jde.Dto;

namespace Ica.SalesOrders.Web.Models
{
    public class Articolo
    {
        public Articolo()
        {
            Descrizione = String.Empty;
            CodiceDeposito = String.Empty;
            Indirizzo = String.Empty;
            Cap = String.Empty;
            Citta = String.Empty;
            Nazione = String.Empty;
            PrefissoTel = String.Empty;
            NumTelefono = String.Empty;
            Provincia = String.Empty;
            Disponibilita = "0";
        }
        public int RowIndex { get; set; }
        public String CodiceArticolo { get; set; }
        public String Descrizione { get; set; }
        public String CodiceDeposito { get; set; }
        public String SRP1 { get; set; }
        public String SRP2 { get; set; }
        public String SRP3 { get; set; }
        public String SRP4 { get; set; }
        public String SRP1Descrizione { get; set; }
        public String SRP2Descrizione { get; set; }
        public String SRP3Descrizione { get; set; }
        public String SRP4Descrizione { get; set; }
        public String SRP7 { get; set; }
        public String TipoStock { get; set; }
        public String CodiceCliente { get; set; }
        public String Company { get; set; }
        public decimal QtaDisponibile { get; set; }
        public String Indirizzo { get; set; }
        public String Cap { get; set; }
        public String Citta { get; set; }
        public String Nazione { get; set; }
        public String PrefissoTel { get; set; }
        public String NumTelefono { get; set; }
        public String DepositoDescrizione { get; set; }
        public String Provincia { get; set; }

        public String Disponibilita { get; set; }


        public String CodiceArticoloLabel { get; set; }
        public String DescrizioneLabel { get; set; }
        public String DepositoLabel { get; set; }
        public String SRP1Label { get; set; }
        public String SRP2Label { get; set; }
        public String SRP3Label { get; set; }
        public String SRP4Label { get; set; }
        public String SRP7Label { get; set; }
        public String TipoStockLabel { get; set; }
        public String CodiceClienteLabel { get; set; }
        public String CompanyLabel { get; set; }
        public String QtaDisponibileLabel { get; set; }
        public String IndirizzoLabel { get; set; }
        public String CapLabel { get; set; }
        public String CittaLabel { get; set; }
        public String NazioneLabel { get; set; }
        public String PrefissoTelLabel { get; set; }
        public String NumTelefonoLabel { get; set; }

        internal void Parse(GridRowDto riga)
        {
            CodiceDeposito = riga.Values.Where(w => w.ColumnName == "z_MCU_22").FirstOrDefault().Value;
            CodiceArticolo = riga.Values.Where(w => w.ColumnName == "z_LITM_23").FirstOrDefault().Value;
            Descrizione = riga.Values.Where(w => w.ColumnName == "z_DSC1_24").FirstOrDefault().Value + riga.Values.Where(w => w.ColumnName == "z_DSC2_25").FirstOrDefault().Value;
            SRP1 = riga.Values.Where(w => w.ColumnName == "z_SRP1_26").FirstOrDefault().Value;
            SRP2 = riga.Values.Where(w => w.ColumnName == "z_SRP2_28").FirstOrDefault().Value;
            SRP3 = riga.Values.Where(w => w.ColumnName == "z_SRP3_27").FirstOrDefault().Value;
            SRP4 = riga.Values.Where(w => w.ColumnName == "z_SRP4_29").FirstOrDefault().Value;
            SRP1Descrizione = riga.Values.Where(w => w.ColumnName == "z_DL01_36").FirstOrDefault().Value;
            SRP2Descrizione = riga.Values.Where(w => w.ColumnName == "z_DL01_37").FirstOrDefault().Value;
            SRP3Descrizione = riga.Values.Where(w => w.ColumnName == "z_DL01_38").FirstOrDefault().Value;
            SRP4Descrizione = riga.Values.Where(w => w.ColumnName == "z_DL01_39").FirstOrDefault().Value;
            DepositoDescrizione = riga.Values.Where(w => w.ColumnName == "z_DL01_32").FirstOrDefault().Value;
            CodiceCliente = riga.Values.Where(w => w.ColumnName == "z_AN8_33").FirstOrDefault().Value;
            Company = riga.Values.Where(w => w.ColumnName == "z_CO_34").FirstOrDefault().Value;
            QtaDisponibile = decimal.Parse( riga.Values.Where(w => w.ColumnName == "z_PQOH_50").FirstOrDefault().Value.Replace(",","."));
            Indirizzo = riga.Values.Where(w => w.ColumnName == "z_ADD3_51").FirstOrDefault().Value;
            Cap = riga.Values.Where(w => w.ColumnName == "z_ADDZ_52").FirstOrDefault().Value;
            Citta = riga.Values.Where(w => w.ColumnName == "z_CTY1_53").FirstOrDefault().Value;
            Provincia = riga.Values.Where(w => w.ColumnName == "z_ADDS_54").FirstOrDefault().Value;
            Nazione = riga.Values.Where(w => w.ColumnName == "z_CTR_55").FirstOrDefault().Value;
            PrefissoTel = riga.Values.Where(w => w.ColumnName == "z_AR1_56").FirstOrDefault().Value;
            NumTelefono = riga.Values.Where(w => w.ColumnName == "z_PH1_57").FirstOrDefault().Value;
            SRP7 = riga.Values.Where(w => w.ColumnName == "z_SRP7_30").FirstOrDefault().Value;
        }

        internal void ParseMaster(GridRowDto item)
        {
            //throw new NotImplementedException();
        }
    }
}