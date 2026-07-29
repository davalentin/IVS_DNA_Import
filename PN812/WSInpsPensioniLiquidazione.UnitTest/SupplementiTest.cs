using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.Liquidazione.BLCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using INPS.DNA.Security.Idm;
using INPS.DNA.Security.Roles;
using INPS.DNA.Context;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for SupplementiTest
    /// </summary>
    [TestClass]
    public class SupplementiTest
    {
        public SupplementiTest()
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
        public void TestGetSupplementi()
        {
            
            long domanda = 2038517500007;//2038517500007 ;
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(domanda, null, out datiPensione);

            GestioneAreaSupplementi.EliminaSupplementiByIdPensione(datiPensione);
            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

            Pensioni.Liquidazione.BLCommon.GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
            Pensioni.Liquidazione.BLCommon.GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);
            Assert.IsTrue(datiQuadroSupplementi.TabSupplementi == 1 &&
                          !datiQuadroSupplementi.TabIntegrazioneArt11.HasValue, "Semaforo Dati Supplementio non corretto");

            

            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> datiSupplementi = null;

            // decodifica
            List<BLCommon.Entity.TipoSupplementi> listaTipoSupplementi = null;
            GestioneAreaSupplementi.GetListaTipoSupplementiByDatiPensione(datiPensione, datiDanteCausa, out listaTipoSupplementi);

            Assert.IsTrue(listaTipoSupplementi != null && listaTipoSupplementi.Count > 0);

            //creazione oggetto entity DatiSupplementi           
            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listdatiSupplementiEntity = new List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi>();
            
            INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi datiSupplementiEntity = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
            datiSupplementiEntity.CodGestioneSupplemento = "9";
            datiSupplementiEntity.DecorrenzaSupplemento = DateTime.Parse("27/08/2011 12:15:50");
            datiSupplementiEntity.MontanteSupplemento = decimal.Round(5.12M, 2);
            datiSupplementiEntity.NSettimaneSupplemento = 2;
            datiSupplementiEntity.QuotaSupplemento = 'b';
            datiSupplementiEntity.RMSSupplemento = decimal.Round(4.12M, 2);
            datiSupplementiEntity.TipoSupplemento = '1';           
            listdatiSupplementiEntity.Add(datiSupplementiEntity);

            INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi datiSupplementiEntity1 = new INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi();
            datiSupplementiEntity1.CodGestioneSupplemento = "9";
            datiSupplementiEntity1.DecorrenzaSupplemento = DateTime.Parse("28/08/2011 12:15:50");
            datiSupplementiEntity1.MontanteSupplemento = decimal.Round(7.12M, 2);
            datiSupplementiEntity1.NSettimaneSupplemento = 3;
            datiSupplementiEntity1.QuotaSupplemento = 'b';
            datiSupplementiEntity1.RMSSupplemento = decimal.Round(8.12M, 2);
            datiSupplementiEntity1.TipoSupplemento = '2';
            listdatiSupplementiEntity.Add(datiSupplementiEntity1);

            // store SupplementiDati
            //GestioneAreaSupplementi.StoreDatiSupplementi(domanda, listdatiSupplementiEntity);

            datiQuadroSupplementi = null;
            Pensioni.Liquidazione.BLCommon.GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);
            Assert.IsTrue(datiQuadroSupplementi.TabSupplementi == 2 &&
                          !datiQuadroSupplementi.TabIntegrazioneArt11.HasValue, "Semaforo Dati Supplementio non corretto");

            // Assert
            Assert.IsTrue(datiSupplementi.Count == 2);

            //eliminazione oggetto MaggiorazioniBenefici
            GestioneAreaSupplementi.EliminaSupplementiByIdPensione(datiPensione);

            datiQuadroSupplementi = null;
            Pensioni.Liquidazione.BLCommon.GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);
            Assert.IsTrue(datiQuadroSupplementi.TabSupplementi == 1 &&
                          !datiQuadroSupplementi.TabIntegrazioneArt11.HasValue, "Semaforo Dati Supplementio non corretto");
        }
    }
}
