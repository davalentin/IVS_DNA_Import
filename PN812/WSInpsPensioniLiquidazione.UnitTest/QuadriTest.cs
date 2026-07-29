using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using INPS.DNA.Security.Idm;
using INPS.DNA.Security.Roles;
using INPS.DNA.Context;

using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for QuadriTest
    /// </summary>
    [TestClass]
    public class QuadriTest
    {
        public QuadriTest()
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
        public void TestQuadroTitolare()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            GestioneQuadri.DatiQuadroTitolare quadroTitolare = new GestioneQuadri.DatiQuadroTitolare(1,2,1,2);

            GestioneQuadri.SalvaQuadroTitolare(datiPensione.Id, quadroTitolare);

            quadroTitolare = null;

            GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out quadroTitolare);

            Assert.IsTrue(quadroTitolare != null &&
                quadroTitolare.Tipo == 1 &&
                quadroTitolare.TabAnagrafica == 2 &&
                quadroTitolare.TabStatiCivili == 1 &&
                quadroTitolare.TabResidenzeEstero == 2, "Quadro titolare non recuperato correttamente");

            quadroTitolare = new GestioneQuadri.DatiQuadroTitolare(2, 1, 1, 2);

            GestioneQuadri.SalvaQuadroTitolare(datiPensione.Id, quadroTitolare);

            quadroTitolare = null;

            GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out quadroTitolare);

            Assert.IsTrue(quadroTitolare != null &&
                quadroTitolare.Tipo == 2 &&
                quadroTitolare.TabAnagrafica == 1 &&
                quadroTitolare.TabStatiCivili == 1 &&
                quadroTitolare.TabResidenzeEstero == 2, "Quadro titolare non recuperato correttamente");

            GestioneQuadri.EliminaQuadroTitolare(datiPensione.Id);

            quadroTitolare = null;

            GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out quadroTitolare);

            Assert.IsTrue(quadroTitolare != null, "Quadro titolare nullo");
        }

        [TestMethod]
        public void TestQuadroDetrazioni()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            GestioneQuadri.DatiQuadroDetrazioni quadroDetrazioni = new GestioneQuadri.DatiQuadroDetrazioni(1, 2);

            GestioneQuadri.SalvaQuadroDetrazioni(datiPensione.Id, quadroDetrazioni);

            quadroDetrazioni = null;

            GestioneQuadri.GetQuadroDetrazioniByDatiPensione(datiPensione, out quadroDetrazioni);

            Assert.IsTrue(quadroDetrazioni != null &&
                quadroDetrazioni.Tipo == 1 &&
                quadroDetrazioni.TabDetrazioni == 2 , "Quadro detrazioni non recuperato correttamente");

            quadroDetrazioni = new GestioneQuadri.DatiQuadroDetrazioni(2, 1);

            GestioneQuadri.SalvaQuadroDetrazioni(datiPensione.Id, quadroDetrazioni);

            quadroDetrazioni = null;

            GestioneQuadri.GetQuadroDetrazioniByDatiPensione(datiPensione, out quadroDetrazioni);

            Assert.IsTrue(quadroDetrazioni != null &&
                quadroDetrazioni.Tipo == 2 &&
                quadroDetrazioni.TabDetrazioni == 1, "Quadro detrazioni non recuperato correttamente");

            GestioneQuadri.EliminaQuadroDetrazioni(datiPensione.Id);

            quadroDetrazioni = null;

            GestioneQuadri.GetQuadroDetrazioniByDatiPensione(datiPensione, out quadroDetrazioni);

            Assert.IsTrue(quadroDetrazioni != null, "Quadro detrazioni nullo");
        }

        [TestMethod]
        public void TestQuadroPagamento()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            GestioneQuadri.DatiQuadroPagamento quadroPagamento = new GestioneQuadri.DatiQuadroPagamento(1, 2);

            GestioneQuadri.SalvaQuadroPagamento(datiPensione.Id, quadroPagamento);

            quadroPagamento = null;

            GestioneQuadri.GetQuadroPagamentoByDatiPensione(datiPensione, out quadroPagamento);

            Assert.IsTrue(quadroPagamento != null &&
                quadroPagamento.Tipo == 1 &&
                quadroPagamento.TabPagamento == 2, "Quadro pagamento non recuperato correttamente");

            quadroPagamento = new GestioneQuadri.DatiQuadroPagamento(2, 1);

            GestioneQuadri.SalvaQuadroPagamento(datiPensione.Id, quadroPagamento);

            quadroPagamento = null;

            GestioneQuadri.GetQuadroPagamentoByDatiPensione(datiPensione, out quadroPagamento);

            Assert.IsTrue(quadroPagamento != null &&
                quadroPagamento.Tipo == 2 &&
                quadroPagamento.TabPagamento == 1, "Quadro pagamento non recuperato correttamente");

            GestioneQuadri.EliminaQuadroPagamento(datiPensione.Id);

            quadroPagamento = null;

            GestioneQuadri.GetQuadroPagamentoByDatiPensione(datiPensione, out quadroPagamento);

            Assert.IsTrue(quadroPagamento != null, "Quadro pagamento nullo");
        }

        [TestMethod]
        public void TestQuadroLiquidazionePensione()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            GestioneQuadri.DatiQuadroLiquidazionePensione quadroLiquidazionePensione = new GestioneQuadri.DatiQuadroLiquidazionePensione(1, 2, 1, 2, 1, 2, 1, null, null, null, null,null);

            GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, quadroLiquidazionePensione);

            quadroLiquidazionePensione = null;

            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out quadroLiquidazionePensione);

            Assert.IsTrue(quadroLiquidazionePensione != null &&
                quadroLiquidazionePensione.Tipo == 1 &&
                quadroLiquidazionePensione.TabDatiGenerici == 2 &&
                quadroLiquidazionePensione.TabOpzione == 1 &&
                quadroLiquidazionePensione.TabPrecedentePensione == 2 &&
                quadroLiquidazionePensione.TabIstruttoria == 1 &&
                quadroLiquidazionePensione.TabDatiAssicurativi == 2
                , "Quadro liquidazione pensione non recuperato correttamente");

            quadroLiquidazionePensione = new GestioneQuadri.DatiQuadroLiquidazionePensione(2, 1, 2, 1, 2, 1, 1, null, null, null, null, null);

            GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, quadroLiquidazionePensione);

            quadroLiquidazionePensione = null;

            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out quadroLiquidazionePensione);

            Assert.IsTrue(quadroLiquidazionePensione != null &&
                quadroLiquidazionePensione.Tipo == 2 &&
                quadroLiquidazionePensione.TabDatiGenerici == 1 &&
                quadroLiquidazionePensione.TabOpzione == 2 &&
                quadroLiquidazionePensione.TabPrecedentePensione == 1 &&
                quadroLiquidazionePensione.TabIstruttoria == 2 &&
                quadroLiquidazionePensione.TabDatiAssicurativi == 1
                , "Quadro liquidazione pensione non recuperato correttamente");

            GestioneQuadri.EliminaQuadroLiquidazionePensione(datiPensione.Id);

            quadroLiquidazionePensione = null;

            GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out quadroLiquidazionePensione);

            Assert.IsTrue(quadroLiquidazionePensione != null, "Quadro pagamento nullo");
        }

        [TestMethod]
        public void TestQuadroDatiContributivi()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = new GestioneQuadri.DatiQuadroDatiContributivi(1, 2, null, null, null, null, null, null, null, null, null, null, null, null, 
                null, null, null, null, null, null, null, null);

            GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);

            quadroDatiContributivi = null;

            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            Assert.IsTrue(quadroDatiContributivi != null &&
                quadroDatiContributivi.Tipo == 1 &&
                quadroDatiContributivi.TabDatiCalcolo == 2
                , "Quadro Dati Calcolo pensione non recuperato correttamente");

            quadroDatiContributivi = new GestioneQuadri.DatiQuadroDatiContributivi(2, 1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);

            quadroDatiContributivi = null;

            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            Assert.IsTrue(quadroDatiContributivi != null &&
                quadroDatiContributivi.Tipo == 2 &&
                quadroDatiContributivi.TabDatiCalcolo == 1
                , "Quadro Dati Calcolo pensione non recuperato correttamente");

            GestioneQuadri.EliminaQuadroDatiContributivi(datiPensione.Id);

            quadroDatiContributivi = null;

            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            Assert.IsTrue(quadroDatiContributivi != null, "Quadro Dati Calcolo nullo");
        }

        [TestMethod]
        public void TestQuadroRedditi()
        {
            GestionePensione.DatiPensione datiPensione;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2038517500007, null, out datiPensione);
            long idPensione = datiPensione.Id;
            GestioneQuadri.DatiQuadroRedditi quadroRedditi = new GestioneQuadri.DatiQuadroRedditi(1, 2);

            GestioneQuadri.SalvaQuadroRedditi(idPensione, quadroRedditi);

            quadroRedditi = null;

            GestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione, out quadroRedditi);

            Assert.IsTrue(quadroRedditi != null &&
                quadroRedditi.Tipo == 1 &&
                quadroRedditi.TabRedditi == 2, "Quadro redditi non recuperato correttamente");

            quadroRedditi = new GestioneQuadri.DatiQuadroRedditi(2, 1);

            GestioneQuadri.SalvaQuadroRedditi(idPensione, quadroRedditi);

            quadroRedditi = null;

            GestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione, out quadroRedditi);

            Assert.IsTrue(quadroRedditi != null &&
                quadroRedditi.Tipo == 2 &&
                quadroRedditi.TabRedditi == 1, "Quadro redditi non recuperato correttamente");

            GestioneQuadri.EliminaQuadroRedditi(idPensione);

            quadroRedditi = null;

            GestioneQuadri.GetQuadroRedditiByIdPensione(datiPensione, out quadroRedditi);

            Assert.IsTrue(quadroRedditi != null, "Quadro redditi nullo");
        }
        [TestMethod]
        public void TestQuadroFamiliari()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            GestioneQuadri.DatiQuadroFamiliari quadroFamiliari= new GestioneQuadri.DatiQuadroFamiliari(1, 2);

            GestioneQuadri.SalvaQuadroFamiliari(datiPensione.Id, quadroFamiliari);

            quadroFamiliari = null;

            GestioneQuadri.GetQuadroFamiliariByDatiPensione(datiPensione, out quadroFamiliari);

            Assert.IsTrue(quadroFamiliari != null &&
                quadroFamiliari.Tipo == 1 &&
                quadroFamiliari.TabFamiliari == 2, "Quadro redditi non recuperato correttamente");

            quadroFamiliari = new GestioneQuadri.DatiQuadroFamiliari(2, 1);

            GestioneQuadri.SalvaQuadroFamiliari(datiPensione.Id, quadroFamiliari);

            quadroFamiliari = null;

            GestioneQuadri.GetQuadroFamiliariByDatiPensione(datiPensione, out quadroFamiliari);

            Assert.IsTrue(quadroFamiliari != null &&
                quadroFamiliari.Tipo == 2 &&
                quadroFamiliari.TabFamiliari == 1, "Quadro redditi non recuperato correttamente");

            GestioneQuadri.EliminaQuadroFamiliari(datiPensione.Id);

            quadroFamiliari = null;

            GestioneQuadri.GetQuadroFamiliariByDatiPensione(datiPensione, out quadroFamiliari);

            Assert.IsTrue(quadroFamiliari != null, "Quadro redditi nullo");
        }

        [TestMethod]
        public void TestQuadroDanteCausa()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            GestioneQuadri.DatiQuadroDanteCausa quadroDanteCausa = new GestioneQuadri.DatiQuadroDanteCausa(1, 2, 1, 2, 1, null);

            GestioneQuadri.SalvaQuadroDanteCausa(datiPensione.Id, quadroDanteCausa, datiPensione);

            quadroDanteCausa = null;

            GestioneQuadri.GetQuadroDanteCausaByDatiPensione(datiPensione, out quadroDanteCausa);

            Assert.IsTrue(quadroDanteCausa != null &&
                quadroDanteCausa.Tipo == 1 &&
                quadroDanteCausa.TabAnagrafica == 2 &&
                quadroDanteCausa.TabPensioneDiretta == 1 &&
                quadroDanteCausa.TabAltraPensione == 2 &&
                quadroDanteCausa.TabDatiPensioneCI == 1, "Quadro DanteCausa non recuperato correttamente");

            quadroDanteCausa = new GestioneQuadri.DatiQuadroDanteCausa(2, 1, 1, 2, 2, null);

            GestioneQuadri.SalvaQuadroDanteCausa(datiPensione.Id, quadroDanteCausa, datiPensione);

            quadroDanteCausa = null;

            GestioneQuadri.GetQuadroDanteCausaByDatiPensione(datiPensione, out quadroDanteCausa);

            Assert.IsTrue(quadroDanteCausa != null &&
                quadroDanteCausa.Tipo == 2 &&
                quadroDanteCausa.TabAnagrafica == 1 &&
                quadroDanteCausa.TabPensioneDiretta == 1 &&
                quadroDanteCausa.TabAltraPensione == 2 &&
                quadroDanteCausa.TabDatiPensioneCI == 2, "Quadro DanteCausa non recuperato correttamente");

            GestioneQuadri.EliminaQuadroDanteCausa(datiPensione.Id);

            quadroDanteCausa = null;

            GestioneQuadri.GetQuadroDanteCausaByDatiPensione(datiPensione, out quadroDanteCausa);

            Assert.IsTrue(quadroDanteCausa != null, "Quadro DanteCausa nullo");
        }

        [TestMethod]
        public void TestQuadroDatiContributiviCi()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = new GestioneQuadri.DatiQuadroDatiContributivi(1, null, 2, 1, 2, 1, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null);

            GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);

            quadroDatiContributivi = null;

            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            Assert.IsTrue(quadroDatiContributivi != null &&
                quadroDatiContributivi.Tipo == 1 &&
                quadroDatiContributivi.TabProRata == 2 &&
                quadroDatiContributivi.TabContrEsteri == 1 &&
                quadroDatiContributivi.TabMaternAcna == 2 &&
                quadroDatiContributivi.TabLavAutonomi == 1
                , "Quadro Dati Calcolo Ci pensione non recuperato correttamente");

            quadroDatiContributivi = new GestioneQuadri.DatiQuadroDatiContributivi(2, null, 1, 1, 1, 1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            GestioneQuadri.SalvaQuadroDatiContributivi(datiPensione.Id, quadroDatiContributivi);

            quadroDatiContributivi = null;

            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            Assert.IsTrue(quadroDatiContributivi != null &&
                quadroDatiContributivi.Tipo == 2 &&
                quadroDatiContributivi.TabProRata == 1 &&
                quadroDatiContributivi.TabContrEsteri == 1 &&
                quadroDatiContributivi.TabMaternAcna == 1 &&
                quadroDatiContributivi.TabLavAutonomi == 1
                , "Quadro Dati Calcolo Ci pensione non recuperato correttamente");

            GestioneQuadri.EliminaQuadroDatiContributivi(datiPensione.Id);

            quadroDatiContributivi = null;

            GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(datiPensione, out quadroDatiContributivi);

            Assert.IsTrue(quadroDatiContributivi != null, "Quadro Dati Calcolo Ci nullo");
        }
    }
}

