using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using INPS.DNA.Security.Idm;
using INPS.DNA.Security.Roles;
using INPS.DNA.Context;
using INPS.Pensioni.Liquidazione.ServiceTest.SvrLiquidazione;


namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for ServiceQuadriTest
    /// </summary>
    [TestClass]
    public class ServiceQuadriTest
    {
        public ServiceQuadriTest()
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
            //IdmIdentity ident;

            //ident = new IdmIdentity("4444444444", "ABBCDA74A10H501B", "Mario", "Rossi", "mrossi", @"domain\mrossi", "mrossi@inps.it", "0600", "060000");

            //System.Threading.Thread.CurrentPrincipal = new IdmPrincipal(ident, "cn=INPS.Pensioni.Liquidazione:oper:060000,dc=inps,dc=it;cn=INPS.Pensioni.Liquidazione:oper:700500,dc=inps,dc=it;");

            //INPS.DNA.Context.OperationContextInfo.CreateUnitTestContext();
            //INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = OfficeList.Offices["060000"];


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
        public void TestGetQuadri()
        {
            ServiceTest.SvrLiquidazione.QuadriClient objWS = new QuadriClient();
            ServiceTest.SvrLiquidazione.AreaQuadri areaQuadri = objWS.GetQuadriByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2038517500007 });

            Assert.IsTrue(areaQuadri.QuadroTitolare.Quadro == AreaQuadri.Semaforo.Verde &&
                areaQuadri.QuadroTitolare.TabAnagrafica == AreaQuadri.Semaforo.Verde &&
                areaQuadri.QuadroTitolare.TabStatiCivili == AreaQuadri.Semaforo.Verde &&
                areaQuadri.QuadroTitolare.TabResidenzeEstero == AreaQuadri.Semaforo.Verde, "Semaforo anagrafica non settato correttamente");
            Assert.IsTrue(areaQuadri.QuadroDetrazioni.Quadro == AreaQuadri.Semaforo.Verde &&
                areaQuadri.QuadroDetrazioni.TabDetrazioni == AreaQuadri.Semaforo.Verde, "Semaforo detrazioni non settato correttamente");
            Assert.IsTrue(areaQuadri.QuadroPagamento.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                areaQuadri.QuadroPagamento.TabPagamento == AreaQuadri.Semaforo.Rosso_Abilitato, "Semaforo detrazioni non settato correttamente");
            Assert.IsTrue(areaQuadri.QuadroLiquidazionePensione.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                areaQuadri.QuadroLiquidazionePensione.TabDatiGenerici == AreaQuadri.Semaforo.Rosso_Abilitato &&
                areaQuadri.QuadroLiquidazionePensione.TabOpzione == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                areaQuadri.QuadroLiquidazionePensione.TabPrecedentePensione == AreaQuadri.Semaforo.Giallo &&
                areaQuadri.QuadroLiquidazionePensione.TabIstruttoria == AreaQuadri.Semaforo.Giallo &&
                areaQuadri.QuadroLiquidazionePensione.TabDatiAssicurativi == AreaQuadri.Semaforo.Rosso_Abilitato
                , "Semaforo liquidazione pensione non settato correttamente");

            Assert.IsTrue(areaQuadri.QuadroRedditi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                areaQuadri.QuadroRedditi.TabRedditi == AreaQuadri.Semaforo.Rosso_Abilitato
                , "Semaforo redditi non settato correttamente");
            
            areaQuadri = objWS.GetQuadriByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            Assert.IsTrue(areaQuadri.QuadroTitolare.Quadro == AreaQuadri.Semaforo.Verde &&
               areaQuadri.QuadroTitolare.TabAnagrafica == AreaQuadri.Semaforo.Verde &&
               areaQuadri.QuadroTitolare.TabStatiCivili == AreaQuadri.Semaforo.Verde &&
               areaQuadri.QuadroTitolare.TabResidenzeEstero == AreaQuadri.Semaforo.Verde, "Semafori non settati correttamente");
            Assert.IsTrue(areaQuadri.QuadroDetrazioni.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                areaQuadri.QuadroDetrazioni.TabDetrazioni == AreaQuadri.Semaforo.Rosso_Abilitato, "Semaforo detrazioni non settato correttamente");
            Assert.IsTrue(areaQuadri.QuadroPagamento.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                areaQuadri.QuadroPagamento.TabPagamento == AreaQuadri.Semaforo.Rosso_Abilitato, "Semaforo detrazioni non settato correttamente");
            Assert.IsTrue(areaQuadri.QuadroLiquidazionePensione.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                areaQuadri.QuadroLiquidazionePensione.TabDatiGenerici == AreaQuadri.Semaforo.Rosso_Abilitato &&
                areaQuadri.QuadroLiquidazionePensione.TabOpzione == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                areaQuadri.QuadroLiquidazionePensione.TabPrecedentePensione == AreaQuadri.Semaforo.Giallo &&
                areaQuadri.QuadroLiquidazionePensione.TabIstruttoria == AreaQuadri.Semaforo.Giallo &&
                areaQuadri.QuadroLiquidazionePensione.TabDatiAssicurativi == AreaQuadri.Semaforo.Rosso_Abilitato 
                , "Semaforo liquidazione pensione non settato correttamente");

            Assert.IsTrue(areaQuadri.QuadroDatiContributivi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
               areaQuadri.QuadroDatiContributivi.TabDatiCalcolo == AreaQuadri.Semaforo.Rosso_Abilitato
               , "Semaforo dati calcolo non settato correttamente");

            Assert.IsTrue(areaQuadri.QuadroRedditi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                areaQuadri.QuadroRedditi.TabRedditi == AreaQuadri.Semaforo.Rosso_Abilitato
                , "Semaforo redditi non settato correttamente");

            Assert.IsTrue(areaQuadri.QuadroDanteCausa.Quadro == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
               areaQuadri.QuadroDanteCausa.TabAnagrafica == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
               areaQuadri.QuadroDanteCausa.TabPensioneDiretta == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
               areaQuadri.QuadroDanteCausa.TabAltraPensione == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
               areaQuadri.QuadroDanteCausa.TabDatiPensioneCI == AreaQuadri.Semaforo.Rosso_NonAbilitato
               , "Semaforo DanteCausa non settato correttamente");
        }

        [TestMethod]
        public void TestGetQuadroTitolare()
        {
            ServiceTest.SvrLiquidazione.QuadriClient objWS = new QuadriClient();
            ServiceTest.SvrLiquidazione.AreaQuadri.DatiQuadroTitolare quadroTitolare = objWS.GetQuadroTitolareByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2038517500007 });

            Assert.IsTrue(quadroTitolare.Quadro == AreaQuadri.Semaforo.Verde &&
                quadroTitolare.TabAnagrafica == AreaQuadri.Semaforo.Verde &&
                quadroTitolare.TabStatiCivili == AreaQuadri.Semaforo.Verde &&
                quadroTitolare.TabResidenzeEstero == AreaQuadri.Semaforo.Verde, "Semaforo anagrafica non settato correttamente");

            quadroTitolare = objWS.GetQuadroTitolareByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            Assert.IsTrue(quadroTitolare.Quadro == AreaQuadri.Semaforo.Verde &&
               quadroTitolare.TabAnagrafica == AreaQuadri.Semaforo.Verde &&
               quadroTitolare.TabStatiCivili == AreaQuadri.Semaforo.Verde &&
               quadroTitolare.TabResidenzeEstero == AreaQuadri.Semaforo.Verde, "Semafori non settati correttamente");
        }

        [TestMethod]
        public void TestGetQuadroDetrazioni()
        {
            ServiceTest.SvrLiquidazione.QuadriClient objWS = new QuadriClient();
            ServiceTest.SvrLiquidazione.AreaQuadri.DatiQuadroDetrazioni quadroDetrazioni = objWS.GetQuadroDetrazioniByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2038517500007 });

            Assert.IsTrue(quadroDetrazioni.Quadro == AreaQuadri.Semaforo.Verde &&
                quadroDetrazioni.TabDetrazioni == AreaQuadri.Semaforo.Verde, "Semaforo detrazioni non settato correttamente");

            quadroDetrazioni = objWS.GetQuadroDetrazioniByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            Assert.IsTrue(quadroDetrazioni.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroDetrazioni.TabDetrazioni == AreaQuadri.Semaforo.Rosso_Abilitato, "Semaforo detrazioni non settato correttamente");

        }

        [TestMethod]
        public void TestGetQuadroPagamento()
        {
            ServiceTest.SvrLiquidazione.QuadriClient objWS = new QuadriClient();
            ServiceTest.SvrLiquidazione.AreaQuadri.DatiQuadroPagamento quadroPagamento = objWS.GetQuadroPagamentoByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2038517500007 });

            Assert.IsTrue(quadroPagamento.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroPagamento.TabPagamento == AreaQuadri.Semaforo.Rosso_Abilitato, "Semaforo pagamento non settato correttamente");

            quadroPagamento = objWS.GetQuadroPagamentoByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            Assert.IsTrue(quadroPagamento.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroPagamento.TabPagamento == AreaQuadri.Semaforo.Rosso_Abilitato, "Semaforo pagamento non settato correttamente");

        }

        [TestMethod]
        public void TestGetQuadroLiquidazionePensione()
        {
            ServiceTest.SvrLiquidazione.QuadriClient objWS = new QuadriClient();
            ServiceTest.SvrLiquidazione.AreaQuadri.DatiQuadroLiquidazionePensione quadroLiquidazionePensione = objWS.GetQuadroLiquidazionePensioneByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2038517500007 });

            Assert.IsTrue(quadroLiquidazionePensione.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroLiquidazionePensione.TabDatiGenerici == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroLiquidazionePensione.TabOpzione == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                quadroLiquidazionePensione.TabPrecedentePensione == AreaQuadri.Semaforo.Giallo &&
                quadroLiquidazionePensione.TabIstruttoria == AreaQuadri.Semaforo.Giallo &&
                quadroLiquidazionePensione.TabDatiAssicurativi == AreaQuadri.Semaforo.Rosso_Abilitato
                , "Semaforo liquidazione pensione non settato correttamente");

            quadroLiquidazionePensione = objWS.GetQuadroLiquidazionePensioneByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            Assert.IsTrue(quadroLiquidazionePensione.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroLiquidazionePensione.TabDatiGenerici == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroLiquidazionePensione.TabOpzione == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                quadroLiquidazionePensione.TabPrecedentePensione == AreaQuadri.Semaforo.Giallo &&
                quadroLiquidazionePensione.TabIstruttoria == AreaQuadri.Semaforo.Giallo &&
                quadroLiquidazionePensione.TabDatiAssicurativi == AreaQuadri.Semaforo.Rosso_Abilitato
                , "Semaforo liquidazione pensione non settato correttamente");

        }

        [TestMethod]
        public void TestGetQuadroDatiContributivi()
        {
            ServiceTest.SvrLiquidazione.QuadriClient objWS = new QuadriClient();
            ServiceTest.SvrLiquidazione.AreaQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = objWS.GetQuadroDatiContributiviByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });

            Assert.IsTrue(quadroDatiContributivi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroDatiContributivi.TabDatiCalcolo == AreaQuadri.Semaforo.Rosso_Abilitato
                , "Semaforo Dati Calcolo non settato correttamente");

            quadroDatiContributivi = objWS.GetQuadroDatiContributiviByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            Assert.IsTrue(quadroDatiContributivi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroDatiContributivi.TabDatiCalcolo == AreaQuadri.Semaforo.Rosso_Abilitato
                , "Semaforo Dati Calcolo non settato correttamente");

        }

        [TestMethod]
        public void TestGetQuadroRedditi()
        {
            ServiceTest.SvrLiquidazione.QuadriClient objWS = new QuadriClient();
            ServiceTest.SvrLiquidazione.AreaQuadri.DatiQuadroRedditi quadroRedditi = objWS.GetQuadroRedditiByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });

            Assert.IsTrue(quadroRedditi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroRedditi.TabRedditi == AreaQuadri.Semaforo.Rosso_Abilitato, "1:Semaforo redditi non settato correttamente");

            quadroRedditi = objWS.GetQuadroRedditiByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            Assert.IsTrue(quadroRedditi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroRedditi.TabRedditi == AreaQuadri.Semaforo.Rosso_Abilitato, "2:Semaforo redditi non settato correttamente");

        }

        [TestMethod]
        public void TestGetQuadroDanteCausa()
        {
            ServiceTest.SvrLiquidazione.QuadriClient objWS = new QuadriClient();
            ServiceTest.SvrLiquidazione.AreaQuadri.DatiQuadroDanteCausa quadroDanteCausa = objWS.GetQuadroDanteCausaByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });

            Assert.IsTrue(quadroDanteCausa.Quadro == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                quadroDanteCausa.TabAnagrafica == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                quadroDanteCausa.TabPensioneDiretta == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                quadroDanteCausa.TabAltraPensione == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
                quadroDanteCausa.TabDatiPensioneCI == AreaQuadri.Semaforo.Rosso_NonAbilitato, "Semaforo non settato correttamente");

            quadroDanteCausa = objWS.GetQuadroDanteCausaByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            Assert.IsTrue(quadroDanteCausa.Quadro == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
               quadroDanteCausa.TabAnagrafica == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
               quadroDanteCausa.TabPensioneDiretta == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
               quadroDanteCausa.TabAltraPensione == AreaQuadri.Semaforo.Rosso_NonAbilitato &&
               quadroDanteCausa.TabDatiPensioneCI == AreaQuadri.Semaforo.Rosso_NonAbilitato, "Semafori non settati correttamente");
        }

        [TestMethod]
        public void TestGetQuadroDatiContributiviCi()
        {
            ServiceTest.SvrLiquidazione.QuadriClient objWS = new QuadriClient();
            ServiceTest.SvrLiquidazione.AreaQuadri.DatiQuadroDatiContributivi quadroDatiContributivi = objWS.GetQuadroDatiContributiviByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });

            Assert.IsTrue(quadroDatiContributivi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroDatiContributivi.TabProRata == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroDatiContributivi.TabContrEsteri == AreaQuadri.Semaforo.Giallo &&
                quadroDatiContributivi.TabLavAutonomi == AreaQuadri.Semaforo.Giallo
                , "Semaforo Dati Calcolo Ci non settato correttamente");

            quadroDatiContributivi = objWS.GetQuadroDatiContributiviByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            Assert.IsTrue(quadroDatiContributivi.Quadro == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroDatiContributivi.TabProRata == AreaQuadri.Semaforo.Rosso_Abilitato &&
                quadroDatiContributivi.TabContrEsteri == AreaQuadri.Semaforo.Giallo &&
                quadroDatiContributivi.TabLavAutonomi == AreaQuadri.Semaforo.Giallo
                , "Semaforo Dati Calcolo Ci non settato correttamente");

        }

    //    [TestMethod]
    //    public void TestGetQuadroAggiornaStatoPratica()
    //    {
    //        ServiceTest.SvrLiquidazione.QuadriClient objWS = new QuadriClient();
    //        AreaQuadri areaQuadri = objWS.GetQuadriByDomanda(2038536800001);
    //        bool IsCalcoloAbilitato = false;
    //        string statoPensione = string.Empty;
    //        string matricolaUtenteAcquisizione = string.Empty;
    //        bool isMatchMatricola = false;
    //        AreaEsito esito = objWS.AggiornaQuadriAggiornaInfoPratica(out IsCalcoloAbilitato, out statoPensione, out matricolaUtenteAcquisizione, out isMatchMatricola, 2038536800001, areaQuadri, "12345678", 600);
    //        Assert.IsTrue(!IsCalcoloAbilitato, "Il calcolo non deve essere abilitato");
    //        ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient obj = new ServizioLiquidazioneClient();
    //        AreaRichiestaStatoPratica richiesta = new AreaRichiestaStatoPratica();
    //        richiesta.NumeroDomanda = "2038536800001";
    //        AreaRispostaStatoPratica risposta =  obj.GetStatoPraticaByKey(richiesta);
    //        Assert.IsTrue(risposta.ElencoDatiStatoPratica[0].Stato == "IN ACQUISIZIONE", "Sato pratica non settato IN ACQUISIZIONE");

    //        areaQuadri.QuadroDanteCausa.Quadro = AreaQuadri.Semaforo.Verde;
    //        areaQuadri.QuadroDatiContributivi.Quadro = AreaQuadri.Semaforo.Rosso_NonAbilitato;
    //        areaQuadri.QuadroDelegatoTutore.Quadro = AreaQuadri.Semaforo.Giallo;
    //        areaQuadri.QuadroDetrazioni.Quadro = AreaQuadri.Semaforo.Verde;
    //        areaQuadri.QuadroFamiliari.Quadro = AreaQuadri.Semaforo.Verde;
    //        areaQuadri.QuadroLiquidazionePensione.Quadro = AreaQuadri.Semaforo.Verde;
    //        areaQuadri.QuadroMaggiorazioniBenefici.Quadro = AreaQuadri.Semaforo.Verde;
    //        areaQuadri.QuadroPagamento.Quadro = AreaQuadri.Semaforo.Verde;
    //        areaQuadri.QuadroRedditi.Quadro = AreaQuadri.Semaforo.Verde;
    //        areaQuadri.QuadroSupplementi.Quadro = AreaQuadri.Semaforo.Verde;
    //        areaQuadri.QuadroTitolare.Quadro = AreaQuadri.Semaforo.Verde;

    //        esito = objWS.AggiornaInfoPratica(out IsCalcoloAbilitato, out statoPensione, out matricolaUtenteAcquisizione, out isMatchMatricola, 2038536800001, areaQuadri, "12345678", 600);
    //        Assert.IsTrue(IsCalcoloAbilitato, "Il calcolo non deve essere non abilitato");
    //        obj = new ServizioLiquidazioneClient();
    //        richiesta = new AreaRichiestaStatoPratica();
    //        richiesta.NumeroDomanda = "2038536800001";
    //        risposta = obj.GetStatoPraticaByKey(richiesta);
    //        Assert.IsTrue(risposta.ElencoDatiStatoPratica[0].Stato == "DA CALCOLARE", "Sato pratica non settato DA CALCOLARE");

    //        areaQuadri = objWS.GetQuadriByDomanda(2038536800001);
    //        esito = objWS.AggiornaInfoPratica(out IsCalcoloAbilitato, out statoPensione, out matricolaUtenteAcquisizione, out isMatchMatricola, 2038536800001, areaQuadri, "12345678", 600);
    //        Assert.IsTrue(!IsCalcoloAbilitato, "Il calcolo non deve essere abilitato");
    //        obj = new ServizioLiquidazioneClient();
    //        richiesta = new AreaRichiestaStatoPratica();
    //        richiesta.NumeroDomanda = "2038536800001";
    //        risposta = obj.GetStatoPraticaByKey(richiesta);
    //        Assert.IsTrue(risposta.ElencoDatiStatoPratica[0].Stato == "IN ACQUISIZIONE", "Sato pratica non settato IN ACQUISIZIONE");
    //    }
    }
}

