using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using INPS.DNA.Security.Roles;
using INPS.DNA.Context;
using INPS.DNA.Security.Idm;
using INPS.Pensioni.Liquidazione.ServiceReferences.AggPec;


namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for AggiornamentoPECOTest
    /// </summary>
    [TestClass]
    public class AggiornamentoPECOTest
    {
        public AggiornamentoPECOTest()
        {
            
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion Additional test attributes

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            RoleApplications roleApplications;
            RoleApplication roleApplication;
            IdmIdentity ident;

            ident = new IdmIdentity("4444444444", "ABBCDA74A10H501B", "Mario", "Rossi", "mrossi", @"domain\mrossi", "mrossi@inps.it", "0600", "060000");

            roleApplications = new RoleApplications();
            roleApplication = roleApplications.Add("Dominio1.Gruppo1.Processo1");
            roleApplication.Add("Ruolo1");

            System.Threading.Thread.CurrentPrincipal = new IdmPrincipal(ident, roleApplications);

            INPS.DNA.Context.OperationContextInfo.CreateUnitTestContext();
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = OfficeList.Offices["060000"];
        }

        [TestMethod]
        public void TestGetDatiPECO_FS()
        {
            long numDomanda = 2038743300002;//2038618900001; /*UAA*/  //2038564300006 /*UAA senza dati oneri su felpe*/
            string codFisc = "RCCNTN53T05F839Q";//"RFFGTN56L24F839L";// "MZZGNN52A20E897R";//"MZZGNN52A20E897R";// "CRSVNI52D63B201U"; // "MZZGNN52A20E897R";
            string errori = string.Empty;
            BLCommon.GestionePensione.DatiPensione datiPensione = null;
            BLCommon.GestionePensione.GetPensioneByNumeroDomandaAndProg(numDomanda, null, out datiPensione);
            if (datiPensione == null)
            {
                datiPensione = new BLCommon.GestionePensione.DatiPensione();
                datiPensione.NDomus = numDomanda;
            }
            csAggiornamentoPECO_Fondi_Speciali dati = new csAggiornamentoPECO_Fondi_Speciali();
            GestioneAggiornamentoPECO.GetDatiPECO_FS(datiPensione, codFisc, INPS.Pensioni.Liquidazione.BLCommon.Utility.TipoSalvaguardia.L232_2016, false, ref dati, out errori);
        }

        [TestMethod]
        public void TestGetDatiPECO_AGO()
        {
            long numDomanda = 2008712500012;//2008507000009;//2038536800044;// 2008536400009;//2038536800044;//2008507000009;// 2008536400010; //2038536800044;
            string errori = string.Empty;
            string codfisc = "PGNLDN53H57I140Y";// "MZZGNN52A20E897R";//"MZZGNN52A20E897R";// "CRSVNI52D63B201U"; // "MZZGNN52A20E897R";
            csAggiornamentoPECO_AGO dati = new csAggiornamentoPECO_AGO();
            BLCommon.GestionePensione.DatiPensione datiPensione = null;
            BLCommon.GestionePensione.GetPensioneByNumeroDomandaAndProg(numDomanda, null, out datiPensione);
            if (datiPensione == null)
            {
                datiPensione = new BLCommon.GestionePensione.DatiPensione();
                datiPensione.NDomus = numDomanda;
            }
            bool ret = GestioneAggiornamentoPECO.GetDatiPECO_AGO(datiPensione, codfisc, INPS.Pensioni.Liquidazione.BLCommon.Utility.TipoSalvaguardia.Nessuna, false, ref dati, out errori);
            Assert.IsTrue(ret, errori);
        }

        [TestMethod]
        public void TestGetDatiPECO_CI()
        {
            long numDomanda = 2005506900007;//2038499800010; //2005504900009;//2005492100002;//2005515400006;//2005504900009;//2005506900007;//2038499800010; //2038536800044;

            //2005492100002  
            //2005515400006  
            //2005491700001  
            //2005519300004  


            //2005506900007 
            //2005504900009 
            //2005508500002 
            //2005508500003

            string codfisc = "MZZGNN52A20E897R";// "MZZGNN52A20E897R";//"MZZGNN52A20E897R";// "CRSVNI52D63B201U"; // "MZZGNN52A20E897R";
            string errori = string.Empty;
            csAggiornamentoPECO_Convenzioni_Internazionali dati = new csAggiornamentoPECO_Convenzioni_Internazionali();
            BLCommon.GestionePensione.DatiPensione datiPensione = null;
            BLCommon.GestionePensione.GetPensioneByNumeroDomandaAndProg(numDomanda, null, out datiPensione);
            if (datiPensione == null)
            {
                datiPensione = new BLCommon.GestionePensione.DatiPensione();
                datiPensione.NDomus = numDomanda;
            }
            GestioneAggiornamentoPECO.GetDatiPECO_CI(datiPensione, codfisc, INPS.Pensioni.Liquidazione.BLCommon.Utility.TipoSalvaguardia.Nessuna, false, ref dati, out errori);
        }

        [TestMethod]
        public void TestTrovaDomandaSuPECO_AGO()
        {
            INPS.Pensioni.Liquidazione.DataCommon.PensioniDataContext db = new INPS.Pensioni.Liquidazione.DataCommon.PensioniDataContext(INPS.DNA.Data.ConnectionFactory.GetConnection("PensioniConnectionString"));
            List<long> numeriDomanda = (from p in db.Pensiones
                                        where p.Gestione != "007" && p.IndConvInt.Value == false &&
                                        p.StatoPensione != 4 && p.StatoPensione != 8 && p.StatoPensione != 9 && p.StatoPensione != 10 && p.StatoPensione != 11 
                                        //&& p.Gruppo == "0031"
                                        //p.TipoLetturaUnicarpe != 'C' && p.TipoLetturaUnicarpe.HasValue //&& (p.CodiceTipoRichiesta == "93" || p.CodiceTipoRichiesta == "94" || p.CodiceTipoRichiesta == "95" ||
                                        // p.CodiceTipoRichiesta == "R4" || p.CodiceTipoRichiesta == "R5" || p.CodiceTipoRichiesta == "R7")
                                        select p.NDomus).ToList<long>();
            db.Connection.Close();

            System.IO.File.Delete("C:\\Domande_FELPE_AGO.txt");
            int count = 0;
            foreach (long numDomanda in numeriDomanda)
            {
                string errori;

                System.Diagnostics.Debug.WriteLine("Inizio Elaborazione " + ++count + ": " + numDomanda);

                BLCommon.GestionePensione.DatiPensione datiPensione = null;
                BLCommon.GestionePensione.GetPensioneByNumeroDomandaAndProg(numDomanda, null, out datiPensione);
                csAggiornamentoPECO_AGO dati = new csAggiornamentoPECO_AGO();

                bool ret = GestioneAggiornamentoPECO.GetDatiPECO_AGO(datiPensione, "", INPS.Pensioni.Liquidazione.BLCommon.Utility.TipoSalvaguardia.Nessuna, INPS.Pensioni.Liquidazione.BLCommon.Utility.IsRiaperturaDomanda(datiPensione.Id), ref dati, out errori);
                if (dati != null && dati.PL_Return_Code == 0)
                    System.IO.File.AppendAllText("C:\\Domande_FELPE_AGO.txt", numDomanda.ToString() + "\t");

                System.Diagnostics.Debug.WriteLine("Fine Elaborazione " + count + ": " + numDomanda);
            }
        }

        [TestMethod]
        public void TestGetDatiPECO_AMG()
        {
            BLCommon.GestionePensione.DatiPensione datiPensione = new BLCommon.GestionePensione.DatiPensione();
            datiPensione.NDomus = 0;
            string codiceFisale = "PSTDRA59H16I412U";
            BLCommon.Utility.TipoSalvaguardia tipoSalvaguardia = BLCommon.Utility.TipoSalvaguardia.L232_2016;
            csAggiornamentoPECO_Fondi_AMG dati = null;

            string errore = string.Empty;

            GestioneAggiornamentoPECO.GetDatiPECO_AMG(datiPensione, codiceFisale, tipoSalvaguardia, INPS.Pensioni.Liquidazione.BLCommon.Utility.IsRiaperturaDomanda(datiPensione.Id), ref dati, out errore);
        }

        [TestMethod]
        public void TestGetDatiPECO_AMG_INPDAP()
        {
            BLCommon.GestionePensione.DatiPensione datiPensione = new BLCommon.GestionePensione.DatiPensione();
            datiPensione.NDomus = 2147769300039;
            string codiceFiscale = "PSTDRA59H16I412U";
            BLCommon.Utility.TipoSalvaguardia tipoSalvaguardia = BLCommon.Utility.TipoSalvaguardia.L232_2016;

            csAggiornamentoPECO_Fondi_AMG_INPDAP dati = null;

            string errore = string.Empty;

            GestioneAggiornamentoPECO.GetDatiPECO_AMG_INPDAP(datiPensione, codiceFiscale, tipoSalvaguardia, INPS.Pensioni.Liquidazione.BLCommon.Utility.IsRiaperturaDomanda(datiPensione.Id), ref dati, out errore);
        }
    }
}
