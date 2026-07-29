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
using INPS.Pensioni.Liquidazione.BLCommon;


namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for ServiceLiquidazioneTest
    /// </summary>
    [TestClass]
    public class ServiceLiquidazioneTest
    {
        public ServiceLiquidazioneTest()
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

        //Recupero riepilogo per chiave di ricerca
        [TestMethod]
        public void TestGetRiepilogoByKey()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            //ricerca per codice fiscale (2 domande associate, 0 anagrafica, 0 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
            richiesta.CodiceFiscale = "LTTDRV45B02H501E";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare == null, "Anagrafica non nulla");

            //ricerca per numero domanda (1 domande associate, 1 anagrafica, 0 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2038517500007";
            richiesta.SedeOperatore = 2100;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare != null, "Anagrafica nulla");

            //ricerca per numero domanda
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2132523700001";
            richiesta.SedeOperatore = 6200;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0, "Numero domande errato");
            Assert.IsTrue(risposta.AnagraficaTitolare != null, "Anagrafica nulla");

            //ricerca per codice fiscale
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
            richiesta.CodiceFiscale = "CHNCYN38C06Z217Y";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0, "Numero domande errato");
            Assert.IsTrue(risposta.AnagraficaTitolare == null, "Anagrafica non nulla");

            //ricerca per numero domanda (1 domande associate, 1 anagrafica, 0 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2154523800001";
            richiesta.SedeOperatore = 7010;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare != null, "Anagrafica nulla");

            //ricerca per dati personali parziali (2 domande associate, 1 anagrafica, 0 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.DatiPersonaliParziali;
            richiesta.DatiParziali = new DatiPersonaliParziali();
            richiesta.DatiParziali.Cognome = "ELETTRICO";
            richiesta.DatiParziali.Nome = "DIPROVA";
            richiesta.DatiParziali.DataNascita = new DateTime?(new DateTime(1945, 2, 2));
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare != null, "Anagrafica nulla");

            //ricerca per dati personali parziali (0 domande, 0 anagrafica, 0 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.DatiPersonaliParziali;
            richiesta.DatiParziali = new DatiPersonaliParziali();
            richiesta.DatiParziali.Cognome = "NESSUN";
            richiesta.DatiParziali.Nome = "RISULTATO";
            richiesta.DatiParziali.DataNascita = new DateTime?(new DateTime(1945, 2, 2));
            //richiesta.DatiParziali.Cognome = "";
            //richiesta.DatiParziali.Nome = "";
            //richiesta.DatiParziali.DataNascita = null;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande == null, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare == null, "Anagrafica non nulla");

            //ricerca per codice fiscale (0 domande, 0 anagrafica, 0 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
            richiesta.CodiceFiscale = "CZZCRI68T01A944M";
            //richiesta.CodiceFiscale = null;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande == null, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare == null, "Anagrafica non nulla");

            //ricerca per numero domanda (0 domande, 0 anagrafica, 0 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "1111111111111";
            //richiesta.NumeroDomanda = null;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande == null, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare == null, "Anagrafica non nulla");

            //ricerca per numero domanda (1 domanda "003", 1 anagrafica, 0 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2005507600002";
            richiesta.SedeOperatore = 0300;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Pensioni non nulle");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare != null, "Anagrafica nulla");

            //ricerca per numero domanda
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2132522400001";
            richiesta.SedeOperatore = 6200;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Pensioni non nulle");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare != null, "Anagrafica nulla");

            //ricerca per numero domanda (1 domanda ricostituzione, 1 anagrafica estera, 0 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2125517900002";
            richiesta.SedeOperatore = 5800;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare != null, "Anagrafica nulla");

            //ricerca per codice fiscale (7 domande, 0 anagrafica, 4 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
            richiesta.CodiceFiscale = "DSDNLN42E52F475W";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null, "Domande nulle");
            Assert.IsTrue(risposta.ElencoPensioni != null && risposta.ElencoPensioni.Length > 0, "Pensioni non presenti");
            Assert.IsTrue(risposta.AnagraficaTitolare != null, "Anagrafica nulla");

        }

        [TestMethod]
        public void TestGetRiepilogoByKeyFromUnicarpe()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            //ricerca per numero domanda
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2038530500001";
            richiesta.SedeOperatore = 2100;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare != null, "Anagrafica nulla");
        }

        [TestMethod]
        public void TestGetRiepilogoByKeyFromPECO()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            //ricerca per numero domanda
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2005506900007";
            richiesta.SedeOperatore = 500;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare != null, "Anagrafica nulla");
        }

        [TestMethod]
        public void TestGetRiepilogoByKeyFromPECO_CI()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            //ricerca per numero domanda
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2005508500002";
            richiesta.SedeOperatore = 300;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare != null, "Anagrafica nulla");
        }

        [TestMethod]
        public void TestGetRiepilogoByKeyReversibilita()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            //ricerca per numero domanda
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2038523700001";
            richiesta.SedeOperatore = 2100;
            richiesta.CentroOperativoOperatore = 0;
            richiesta.MatricolaOperatore = "99999998";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
        }

        [TestMethod]
        public void TestGetRiepilogoByKeyRicostituzione()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            //ricerca per numero domanda
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2038525800010";
            richiesta.SedeOperatore = 2100;
            richiesta.CentroOperativoOperatore = 0;
            richiesta.MatricolaOperatore = "99999998";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
        }

        [TestMethod]
        public void TestGetRiepilogoByKeyCi()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            //ricerca per numero domanda
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2005507700002";
            richiesta.SedeOperatore = 300;
            richiesta.CentroOperativoOperatore = 0;
            richiesta.MatricolaOperatore = "99999998";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);

            //ricerca per numero domanda
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2005492100002";
            richiesta.SedeOperatore = 300;
            richiesta.CentroOperativoOperatore = 0;
            richiesta.MatricolaOperatore = "99999998";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
        }


        //Recupero riepilogo presente su DB
        [TestMethod]
        public void TestGetRiepilogoByKeyFromDB()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            //ricerca per numero domanda presente su db
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2038517500007";
            richiesta.SedeOperatore = 2100;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null, "Domanda non trovata sul DB");
            Assert.IsTrue(risposta.AnagraficaTitolare != null, "Anagrafica non trovata sul DB");

            //ricerca per codice fiscale (2 domande associate, 0 anagrafica, 0 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
            richiesta.CodiceFiscale = "CZZPQL83R02C495X";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande != null, "Nessuna domanda recuperata");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare == null, "Anagrafica non nulla");

            //ricerca per codice fiscale (0 domande associate, 0 anagrafica, 0 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
            richiesta.CodiceFiscale = "CZZCRI68C01A944D";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande == null, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare == null, "Anagrafica non nulla");

            //ricerca per codice fiscale (0 domande associate, 0 anagrafica, 0 pensioni)
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
            richiesta.CodiceFiscale = "CZZCRI68T01A944M";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.ElencoDomande == null, "Numero domande errato");
            Assert.IsTrue(risposta.ElencoPensioni == null, "Numero pensioni errato");
            Assert.IsTrue(risposta.ElencoSinonimi == null, "Numero sinonimi errato");
            Assert.IsTrue(risposta.AnagraficaTitolare == null, "Anagrafica non nulla");
        }

        //Richieste di riepilogo errate
        [TestMethod]
        public void TestGetRiepilogoByKeyRichiesteErrate()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            //ricerca per numero domanda ma con domanda non valorizzata
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = null;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO, "Esito errato");

            //ricerca per numero domanda ma con domanda non valorizzata
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
            richiesta.CodiceFiscale = null;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO, "Esito errato");

            //ricerca per numero domanda ma con domanda non valorizzata
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.DatiPersonaliParziali;
            richiesta.DatiParziali = null;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO, "Esito errato");

            //ricerca per numero domanda ma con domanda non valorizzata
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO, "Esito errato");

            //ricerca per codice fiscale  ma con codice fiscale non valorizzat0
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
            richiesta.CodiceFiscale = "";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO, "Esito errato");

            //ricerca per dati parziali ma con dati parziali non valorizzati
            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.DatiPersonaliParziali;
            richiesta.DatiParziali = new DatiPersonaliParziali();
            richiesta.DatiParziali.Nome = "";
            richiesta.DatiParziali.Cognome = "";
            richiesta.DatiParziali.DataNascita = null;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO, "Esito errato");
        }

        [TestMethod]
        public void TestGetRiepilogoByKeyCasoParticolare()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
            richiesta.CodiceFiscale = "CZZPQL28T28H243A";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Esito errato");
        }

        [TestMethod]
        public void TestGetRiepilogoByKeyPerProve()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2038412200001";
            richiesta.SedeOperatore = 2100;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Esito errato");

            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2038457900002";
            richiesta.SedeOperatore = 2100;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Esito errato");
        }

        [TestMethod]
        public void TestGetRiepilogoByKeyPerEsitoCalcolo()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2038536800044";
            richiesta.SedeOperatore = 2100;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK &&
            risposta.EsitoCalcolo.Esito == "KO" &&
            risposta.EsitoCalcolo.DettaglioEsito == "SCARTO DA CALCOLO. ERROR CODE: A10. DETTAGLIO:  NON PRESENTE TIPO RECORD 3 O 4", "EsitoCalcolo errato");

            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
            richiesta.NumeroDomanda = "2038536800046";
            richiesta.SedeOperatore = 2100;
            richiesta.CentroOperativoOperatore = 0;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK &&
               risposta.EsitoCalcolo == null, "Controllo1: EsitoCalcolo non nullo");

            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
            richiesta.CodiceFiscale = "CZZPQL28T28H243A";
            richiesta.NumeroDomanda = "";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK &&
             risposta.EsitoCalcolo == null, "Controllo2: EsitoCalcolo non nullo");

            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.DatiPersonaliParziali;
            richiesta.DatiParziali = new DatiPersonaliParziali();
            richiesta.DatiParziali.Cognome = "NESSUN";
            richiesta.DatiParziali.Nome = "RISULTATO";
            richiesta.DatiParziali.DataNascita = new DateTime?(new DateTime(1945, 2, 2));
            //richiesta.DatiParziali.Cognome = "";
            //richiesta.DatiParziali.Nome = "";
            //richiesta.DatiParziali.DataNascita = null;
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.EsitoCalcolo == null, "Controllo3: EsitoCalcolo non nullo");

            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.DatiPersonaliParziali;
            richiesta.DatiParziali = new DatiPersonaliParziali();
            richiesta.DatiParziali.Cognome = "ELETTRICO";
            richiesta.DatiParziali.Nome = "DIPROVA";
            richiesta.DatiParziali.DataNascita = new DateTime?(new DateTime(1945, 2, 2));
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, risposta.Esito.Messaggio);
            Assert.IsTrue(risposta.EsitoCalcolo == null, "Controllo4: EsitoCalcolo non nullo");
        }

        [TestMethod]
        public void TestGetRiepilogoByKeyDatiAggiuntiviAnag()
        {
            ServiceTest.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();

            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
            richiesta.CodiceFiscale = "CZZPQL83R02C495X";
            risposta = objWS.GetRiepilogoByKey(richiesta);
            Assert.IsTrue(risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Esito errato");
        }

        [TestMethod]
        public void TestGetAreaTitolare()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaTitolare areaTitolare = objWS.GetAreaTitolareByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2005507600002 });
            Assert.IsTrue(areaTitolare != null &&
                areaTitolare.Anagrafica != null &&
                areaTitolare.ElencoResidenzeEstereTitolare.Length > 0 &&
                areaTitolare.ElencoStatiCiviliTitolare.Length > 0 &&
                areaTitolare.Patronato != null &&
                areaTitolare.Sindacato != null &&
                areaTitolare.Pensione != null, "Area titolare non valorizzata correttamente");

            areaTitolare = objWS.GetAreaTitolareByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2132522400001 });
            Assert.IsTrue(areaTitolare != null &&
                areaTitolare.Anagrafica != null &&
                areaTitolare.ElencoResidenzeEstereTitolare.Length == 0 &&
                areaTitolare.ElencoStatiCiviliTitolare.Length == 0 &&
                areaTitolare.Patronato != null &&
                areaTitolare.Sindacato != null &&
                areaTitolare.Pensione != null, "Area titolare non valorizzata correttamente");

            areaTitolare = objWS.GetAreaTitolareByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            Assert.IsTrue(areaTitolare != null &&
                areaTitolare.Anagrafica != null &&
                areaTitolare.Pensione != null &&
                areaTitolare.Pensione.FlagUnicarpe.Value == true
                , "Area titolare per pensione da Unicarpe non valorizzata correttamente");
        }

        [TestMethod]
        public void TestGetAreaTitolareReversibilita()
        {
            //reversibilita
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaTitolare areaTitolare = objWS.GetAreaTitolareByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2038523700001 });
            Assert.IsTrue(areaTitolare != null &&
                areaTitolare.Pensione != null &&
                areaTitolare.Pensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Superstiti
                , "Area titolare per pensione Reversibilità non valorizzata correttamente");
        }

        [TestMethod]
        public void TestGetAreaTitolareRicostituzione()
        {
            //ricostituzione
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaTitolare areaTitolare = objWS.GetAreaTitolareByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2038525800010 });
            Assert.IsTrue(areaTitolare != null &&
                areaTitolare.Pensione != null &&
                areaTitolare.Pensione.Tipo == AreaTitolare.DatiPensione.TipoDomanda.Ricostituzione
                , "Area titolare per pensione Ricostituzione non valorizzata correttamente");
        }

        [TestMethod]
        public void TestSetAreaTitolarePerProve()
        {
            bool isAnagraficaSaved = false;
            bool isWarning = false;
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaTitolare areaTitolare = objWS.GetAreaTitolareByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2038412200001 });
            //pensione
            areaTitolare.Pensione.DecorrenzaOriginaria = new DateTime(2010, 10, 01);
            ServiceTest.SvrLiquidazione.AreaEsito esito = objWS.StoreAreaTitolare(out isAnagraficaSaved, out isWarning, areaTitolare);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);

            areaTitolare = objWS.GetAreaTitolareByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2038457900002 });
            //pensione
            areaTitolare.Pensione.DecorrenzaOriginaria = new DateTime(2008, 01, 01);
            esito = objWS.StoreAreaTitolare(out isAnagraficaSaved, out isWarning, areaTitolare);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);

        }

        [TestMethod]
        public void TestSetAreaTitolare()
        {
            bool isAnagraficaSaved = false;
            bool isWarning = false;

            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaTitolare areaTitolare = objWS.GetAreaTitolareByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });

            //anagrafica
            areaTitolare.Anagrafica.Cell = "098765432";
            areaTitolare.Anagrafica.Tel = "77777777";
            areaTitolare.Anagrafica.EMail = "prova@aggiornamento.it";

            //pensione
            areaTitolare.Pensione.DecorrenzaOriginaria = new DateTime(2010, 10, 01);

            //sindacato
            areaTitolare.Sindacato.CodiceSindacato = "1";
            areaTitolare.Sindacato.DecorrenzaSindacato = new DateTime(2009, 02, 01);
            areaTitolare.Sindacato.CessazioneSindacato = new DateTime(2011, 01, 01);

            //stati civili
            areaTitolare.ElencoStatiCiviliTitolare = new AreaTitolare.DatiStatoCivileTitolare[1];
            areaTitolare.ElencoStatiCiviliTitolare[0] = new AreaTitolare.DatiStatoCivileTitolare();
            areaTitolare.ElencoStatiCiviliTitolare[0].Codice = '1';
            areaTitolare.ElencoStatiCiviliTitolare[0].Decorrenza = new DateTime(2011, 01, 01);

            //residenze estero
            areaTitolare.ElencoResidenzeEstereTitolare = new AreaTitolare.DatiResidenzaEsteroTitolare[1];
            areaTitolare.ElencoResidenzeEstereTitolare[0] = new AreaTitolare.DatiResidenzaEsteroTitolare();
            areaTitolare.ElencoResidenzeEstereTitolare[0].CodCatastaleStatoEE = "Z100";
            areaTitolare.ElencoResidenzeEstereTitolare[0].Decorrenza = new DateTime(2011, 01, 01);

            ServiceTest.SvrLiquidazione.AreaEsito esito = objWS.StoreAreaTitolare(out isAnagraficaSaved, out isWarning, areaTitolare);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);

            areaTitolare = objWS.GetAreaTitolareByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            //anagrafica
            areaTitolare.Anagrafica.Cell = "43333";
            areaTitolare.Anagrafica.Tel = "222111";
            areaTitolare.Anagrafica.EMail = "prova3@aggiornamento.it";

            //pensione
            areaTitolare.Pensione.DecorrenzaOriginaria = new DateTime(2010, 09, 01);

            //sindacato
            areaTitolare.Sindacato.CodiceSindacato = "1";
            areaTitolare.Sindacato.DecorrenzaSindacato = new DateTime(2009, 04, 01);
            areaTitolare.Sindacato.CessazioneSindacato = new DateTime(2011, 05, 01);

            //stati civili
            areaTitolare.ElencoStatiCiviliTitolare = new AreaTitolare.DatiStatoCivileTitolare[1];
            areaTitolare.ElencoStatiCiviliTitolare[0] = new AreaTitolare.DatiStatoCivileTitolare();
            areaTitolare.ElencoStatiCiviliTitolare[0].Codice = '1';
            areaTitolare.ElencoStatiCiviliTitolare[0].Decorrenza = new DateTime(2011, 05, 01);

            //residenze estero
            areaTitolare.ElencoResidenzeEstereTitolare = new AreaTitolare.DatiResidenzaEsteroTitolare[1];
            areaTitolare.ElencoResidenzeEstereTitolare[0] = new AreaTitolare.DatiResidenzaEsteroTitolare();
            areaTitolare.ElencoResidenzeEstereTitolare[0].CodCatastaleStatoEE = "Z100";
            areaTitolare.ElencoResidenzeEstereTitolare[0].Decorrenza = new DateTime(2011, 01, 01);

            esito = objWS.StoreAreaTitolare(out isAnagraficaSaved, out isWarning, areaTitolare);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);

            areaTitolare = objWS.GetAreaTitolareByDomanda(new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            //anagrafica
            areaTitolare.Anagrafica.Cell = "324224";
            areaTitolare.Anagrafica.Tel = "223422";
            areaTitolare.Anagrafica.EMail = "prova2@aggiornamento.it";

            //pensione
            areaTitolare.Pensione.DecorrenzaOriginaria = new DateTime(2011, 04, 01);

            //sindacato
            areaTitolare.Sindacato.CodiceSindacato = "1";
            areaTitolare.Sindacato.DecorrenzaSindacato = new DateTime(2009, 03, 01);
            areaTitolare.Sindacato.CessazioneSindacato = new DateTime(2011, 02, 01);

            //stati civili
            areaTitolare.ElencoStatiCiviliTitolare = new AreaTitolare.DatiStatoCivileTitolare[1];
            areaTitolare.ElencoStatiCiviliTitolare[0] = new AreaTitolare.DatiStatoCivileTitolare();
            areaTitolare.ElencoStatiCiviliTitolare[0].Codice = '1';
            areaTitolare.ElencoStatiCiviliTitolare[0].Decorrenza = new DateTime(2010, 01, 01);

            //residenze estero
            areaTitolare.ElencoResidenzeEstereTitolare = new AreaTitolare.DatiResidenzaEsteroTitolare[1];
            areaTitolare.ElencoResidenzeEstereTitolare[0] = new AreaTitolare.DatiResidenzaEsteroTitolare();
            areaTitolare.ElencoResidenzeEstereTitolare[0].CodCatastaleStatoEE = "Z100";
            areaTitolare.ElencoResidenzeEstereTitolare[0].Decorrenza = new DateTime(2011, 03, 01);

            esito = objWS.StoreAreaTitolare(out isAnagraficaSaved, out isWarning, areaTitolare);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
        }

        [TestMethod]
        public void TestGetDetrazioniByNumeroDomanda()
        {
            //detrazione presente
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaDetrazioni areaDetrazioni = null;
            ServiceTest.SvrLiquidazione.AreaEsito Esito = null;
            Esito = objWS.GetDetrazioniByDomanda(ref areaDetrazioni);
            Assert.IsTrue(Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, Esito.Messaggio);
            Assert.IsTrue(areaDetrazioni.EsitoDetrazioni != AreaDetrazioni.RitornoDetrazioni.Errore, areaDetrazioni.Messaggio);
            Assert.IsTrue(areaDetrazioni.Detrazioni != null, "Detrazioni non presenti");
            Assert.IsTrue(areaDetrazioni.Url != "", "Url non presente");

            ////detrazione non presente
            //Esito = objWS.GetDetrazioniByDomanda(out areaDetrazioni, 2125517900002, "99999998", 2100);
            //Assert.IsTrue(Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, Esito.Messaggio);
            //Assert.IsTrue(areaDetrazioni.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Errore, areaDetrazioni.Messaggio);
            //Assert.IsTrue(areaDetrazioni.Url != "", "Url non presente");

            //domanda non presente
            Esito = objWS.GetDetrazioniByDomanda(ref areaDetrazioni);
            Assert.IsTrue(Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO, Esito.Messaggio);
        }

        [TestMethod]
        public void TestVerifyDetrazioniByNumeroDomanda()
        {
            //detrazione presente
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaDetrazioni areaDetrazioni = null;
            ServiceTest.SvrLiquidazione.AreaEsito Esito = null;
            Esito = objWS.GetDetrazioniByDomanda(ref areaDetrazioni);
            Assert.IsTrue(Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, Esito.Messaggio);
            Assert.IsTrue(areaDetrazioni.EsitoDetrazioni != AreaDetrazioni.RitornoDetrazioni.Errore, areaDetrazioni.Messaggio);
            Assert.IsTrue(areaDetrazioni.Detrazioni != null, "Detrazioni non presenti");

            //nessuna differenza
            ServiceTest.SvrLiquidazione.AreaDetrazioni ultimaAreaDetrazioni = null;
            Esito = objWS.VerifyDetrazioniByDomanda(ref ultimaAreaDetrazioni);
            Assert.IsTrue(Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, Esito.Messaggio);
            Assert.IsTrue(ultimaAreaDetrazioni.EsitoDetrazioni != AreaDetrazioni.RitornoDetrazioni.Errore, ultimaAreaDetrazioni.Messaggio);
            Assert.IsTrue(ultimaAreaDetrazioni.Detrazioni != null, "Detrazioni non presenti");

            //differenza
            areaDetrazioni.Detrazioni.AddizionaleLombardiaVeneto = 1;
            areaDetrazioni.Detrazioni.AgevolazionePensionati = 1;
            areaDetrazioni.Detrazioni.AltriFamiliari100 = 1;
            Esito = objWS.VerifyDetrazioniByDomanda(ref ultimaAreaDetrazioni);
            Assert.IsTrue(Esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, Esito.Messaggio);
            Assert.IsTrue(ultimaAreaDetrazioni.EsitoDetrazioni == AreaDetrazioni.RitornoDetrazioni.Errore, ultimaAreaDetrazioni.Messaggio);
            Assert.IsTrue(ultimaAreaDetrazioni.Detrazioni != null, "Detrazioni non presenti");

            //domanda non presente
            Esito = objWS.VerifyDetrazioniByDomanda(ref ultimaAreaDetrazioni);
            Assert.IsTrue(Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO, Esito.Messaggio);
        }

        [TestMethod]
        public void TestGetRedditiByNumeroDomanda()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaRedditi areaRedditi = null;
            ServiceTest.SvrLiquidazione.AreaEsito esito = objWS.GetRedditiByDomanda(out areaRedditi, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, "99999998", 2100);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK && areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Errore, esito.Messaggio);
        }

        [TestMethod]
        public void TestVerifyRedditiByNumeroDomanda()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaRedditi areaRedditi = null;
            AreaRedditi areaRedditiOriginali = new AreaRedditi();
            areaRedditiOriginali.Redditi = new GestioneRedditiAreaRedditi();
            ServiceTest.SvrLiquidazione.AreaEsito esito = objWS.VerifyRedditiByDomanda(out areaRedditi, 2038517500007, "99999998", 2100, false, areaRedditiOriginali);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK && areaRedditi.Redditi.Esito == GestioneRedditiTipoRitornoRedditi.Errore, esito.Messaggio);
        }

        [TestMethod]
        public void TestGetAnagraficaSoggetto()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anagrafica = null;
            ServiceTest.SvrLiquidazione.AreaEsito esito = objWS.GetAnagraficaSoggettoByCodiceFiscale(out anagrafica, "CZZPQL83R02C495X", 2100, "12345678", 0.ToString());
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK && anagrafica != null, "Area anagrafica non valorizzata correttamente");
            esito = objWS.GetAnagraficaSoggettoByCodiceFiscale(out anagrafica, "CHNCYN38C06Z217Y", 2100, "12345678", 0.ToString());
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK && anagrafica != null, "Area anagrafica non valorizzata correttamente");
            esito = objWS.GetAnagraficaSoggettoByCodiceFiscale(out anagrafica, "CHNCYN38C06Z217R", 2100, "12345678", 0.ToString());
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.KO && anagrafica == null, "Area anagrafica non nulla");
            esito = objWS.GetAnagraficaSoggettoByCodiceFiscale(out anagrafica, "CZZPQL28T28H243A", 2100, "12345678", 0.ToString());
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK && anagrafica != null, "Area anagrafica non valorizzata correttamente");
            esito = objWS.GetAnagraficaSoggettoByCodiceFiscale(out anagrafica, "FRNMRC40A01H501Y", 2100, "12345678", 0.ToString());
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK && anagrafica == null, "Area anagrafica non nulla");
        }

        [TestMethod]
        public void TestGetPagamentoByDomandaSrv()
        {

            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaPagamento areaPagamento = null;

            //pagamento non presente su db ma presente su ws modPag banca sportello (abi + cab)
            ServiceTest.SvrLiquidazione.AreaEsito esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento != null && areaPagamento.Pagamento != null, "Area Pagamento non nulla");

            //pagamento non presente su db ma presente su ws modPag banca cc (iban + bic)
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento != null && areaPagamento.Pagamento != null, "Area Pagamento non nulla");

            //pagamento non presente su db ma presente su ws modPag banca libretto (solo iban)
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento != null && areaPagamento.Pagamento != null, "Area Pagamento non nulla");

            //pagamento non presente su db ma presente su ws modPag posta sportello
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento != null && areaPagamento.Pagamento != null, "Area Pagamento non nulla");

            //pagamento non presente su db ma presente su ws modPag posta libretto
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento != null && areaPagamento.Pagamento != null, "Area Pagamento non nulla");

            //pagamento non presente su db ma presente su ws modPag estero sportello (solo stato)
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento != null && areaPagamento.Pagamento != null, "Area Pagamento non nulla");

            //pagamento non presente su db ma presente su ws modPag estero assegno (solo stato)
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento != null && areaPagamento.Pagamento != null, "Area Pagamento non nulla");

            //pagamento non presente su db ma presente su ws modPag estero cc (iban + bic + stato)
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento != null && areaPagamento.Pagamento != null, "Area Pagamento non nulla");

            //pagamento non presente per domanda non presente
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 0 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento == null, "Area Pagamento non nulla");

            //pagamento non presente per domanda presente
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento == null, "Area Pagamento non nulla");
        }

        [TestMethod]
        public void TestGetSaveCancelPagamentoByDomanda_Banca()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaPagamento areaPagamento = null;

            //pagamento non presente su db ma presente su ws modPag banca sportello (abi + cab)
            ServiceTest.SvrLiquidazione.AreaEsito esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento != null && areaPagamento.Pagamento != null, "Area Pagamento non nulla");

            esito = objWS.StorePagamento(2038517500007, ref areaPagamento, "12345678", "7000");
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);

            //pagamento presente su DB per domanda presente
            areaPagamento = null;
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento != null && areaPagamento.Pagamento != null, "Area Pagamento non nulla");

            esito = objWS.CancelPagamentoByNumeroDomanda(2038517500007);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);

            //pagamento non presente per domanda presente
            areaPagamento = null;
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento == null, "Area Pagamento non nulla");
        }

        [TestMethod]
        public void TestGetSaveCancelPagamentoByDomanda_Posta()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaPagamento areaPagamento = null;

            //pagamento non presente su db ma presente su ws modPag posta cc
            ServiceTest.SvrLiquidazione.AreaEsito esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento != null && areaPagamento.Pagamento != null, "Area Pagamento non nulla");

            esito = objWS.StorePagamento(2038517500007, ref areaPagamento, "12345678", "7000");
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);

            //pagamento presente su DB per domanda presente
            areaPagamento = null;
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento != null && areaPagamento.Pagamento != null, "Area Pagamento non nulla");

            esito = objWS.CancelPagamentoByNumeroDomanda(2038517500007);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);

            //pagamento non presente per domanda presente
            areaPagamento = null;
            esito = objWS.GetPagamentoByNumeroDomanda(out areaPagamento, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 }, 99999);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(areaPagamento == null, "Area Pagamento non nulla");
        }

        [TestMethod]
        public void TestGetUfficiPagatoriSrv()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.RichiestaUfficiPagatori richiesta = new RichiestaUfficiPagatori();
            ServiceTest.SvrLiquidazione.UfficioPagatore[] elencoUfficiPagatori = null;

            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Abi_Cab;
            richiesta.Abi = 1010;
            richiesta.Cab = 40141;
            ServiceTest.SvrLiquidazione.AreaEsito esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(elencoUfficiPagatori != null && elencoUfficiPagatori.Length > 0, "Area uffici pagatori nulla");

            richiesta = new RichiestaUfficiPagatori();
            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Abi_Cap;
            richiesta.Abi = 7601;
            richiesta.Cap = "80056";
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(elencoUfficiPagatori != null && elencoUfficiPagatori.Length > 0, "Area uffici pagatori nulla");

            richiesta = new RichiestaUfficiPagatori();
            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Iban_Banca;
            //richiesta.Iban = "IT16L0101040141100000001226";
            richiesta.Iban = "IT82J0100503215000000009500";
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(elencoUfficiPagatori != null && elencoUfficiPagatori.Length > 0, "Area uffici pagatori nulla");

            richiesta = new RichiestaUfficiPagatori();
            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Iban_Posta;
            richiesta.Iban = "IT16L0760140141100000001226";
            richiesta.Frazionario = 40097;
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(elencoUfficiPagatori != null && elencoUfficiPagatori.Length > 0, "Area uffici pagatori nulla");

            richiesta = new RichiestaUfficiPagatori();
            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Abi_Frazionario;
            richiesta.Abi = 7601;
            richiesta.Frazionario = 40097;
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(elencoUfficiPagatori != null && elencoUfficiPagatori.Length > 0, "Area uffici pagatori nulla");

            richiesta = new RichiestaUfficiPagatori();
            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Estero;
            richiesta.StatoEstero = "AUSTRIA";
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK, esito.Messaggio);
            Assert.IsTrue(elencoUfficiPagatori != null && elencoUfficiPagatori.Length > 0, "Area uffici pagatori nulla");

            richiesta = new RichiestaUfficiPagatori();
            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Abi_Cab;
            richiesta.Abi = 7601;
            richiesta.Frazionario = 40097;
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.KO && elencoUfficiPagatori == null, "Operazione non errata");

            richiesta = new RichiestaUfficiPagatori();
            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Abi_Frazionario;
            richiesta.Abi = 7601;
            richiesta.Cab = 40097;
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.KO && elencoUfficiPagatori == null, "Operazione non errata");

            richiesta = new RichiestaUfficiPagatori();
            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Abi_Cap;
            richiesta.Abi = 7601;
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.KO && elencoUfficiPagatori == null, "Operazione non errata");

            richiesta = new RichiestaUfficiPagatori();
            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Iban_Banca;
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.KO && elencoUfficiPagatori == null, "Operazione non errata");

            richiesta = new RichiestaUfficiPagatori();
            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Iban_Posta;
            richiesta.Iban = "IT16L0760140141100000001226";
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.KO && elencoUfficiPagatori == null, "Operazione non errata");

            richiesta = new RichiestaUfficiPagatori();
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.KO && elencoUfficiPagatori == null, "Operazione non errata");

            richiesta = new RichiestaUfficiPagatori();
            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Estero;
            richiesta.StatoEstero = "asda";
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.OK && elencoUfficiPagatori == null, "Operazione errata");

            richiesta = new RichiestaUfficiPagatori();
            richiesta.Tipo = RichiestaUfficiPagatori.TipoRicerca.Estero;
            esito = objWS.GetUfficiPagatori(out elencoUfficiPagatori, richiesta);
            Assert.IsTrue(esito.RisultatoOperazione == AreaEsito.TipoEsito.KO && elencoUfficiPagatori == null, "Operazione non errata");

        }

        #region Ricerca stato pratica
        [TestMethod]
        public void TestGetStatoPraticaByNumeroDomanda()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRichiestaStatoPratica areaRichiestaStatoPratica = new AreaRichiestaStatoPratica();
            areaRichiestaStatoPratica.NumeroDomanda = "2038517500007";
            areaRichiestaStatoPratica.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.StatoPratica;
            ServiceTest.SvrLiquidazione.AreaRispostaStatoPratica areaRispostaStatoPratica = null;
            areaRispostaStatoPratica = objWS.GetStatoPraticaByKey(areaRichiestaStatoPratica);
            AreaEsito areaEsito = areaRispostaStatoPratica.Esito;
            Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Operazione fallita");
            if (areaRispostaStatoPratica.ElencoDatiStatoPratica.Length == 0)
                Assert.Fail("Nessun risultato trovato");
            foreach (var risultato in areaRispostaStatoPratica.ElencoDatiStatoPratica)
            {
                Assert.IsTrue(risultato.NumeroDomanda == areaRichiestaStatoPratica.NumeroDomanda, "Numero Domanda non valida");
            }
        }

        [TestMethod]
        public void TestGetStatoPraticaByNome_Cognome()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRichiestaStatoPratica areaRichiestaStatoPratica = new AreaRichiestaStatoPratica();
            areaRichiestaStatoPratica.DatiParziali = new DatiPersonaliParziali();
            areaRichiestaStatoPratica.DatiParziali.Nome = "DIPROVA";
            areaRichiestaStatoPratica.DatiParziali.Cognome = "ELETTRICO";
            areaRichiestaStatoPratica.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.StatoPratica;
            ServiceTest.SvrLiquidazione.AreaRispostaStatoPratica areaRispostaStatoPratica = null;
            areaRispostaStatoPratica = objWS.GetStatoPraticaByKey(areaRichiestaStatoPratica);
            AreaEsito areaEsito = areaRispostaStatoPratica.Esito;
            Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Operazione fallita");
            if (areaRispostaStatoPratica.ElencoDatiStatoPratica.Length == 0)
                Assert.Fail("Nessun risultato trovato");
            foreach (var risultato in areaRispostaStatoPratica.ElencoDatiStatoPratica)
            {
                Assert.IsTrue(risultato.Nome == areaRichiestaStatoPratica.DatiParziali.Nome, "Nome non valido");
                Assert.IsTrue(risultato.Cognome == areaRichiestaStatoPratica.DatiParziali.Cognome, "Cognome non valido");
            }
        }

        [TestMethod]
        public void TestGetStatoPraticaByDataElaborazioneDomanda()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRichiestaStatoPratica areaRichiestaStatoPratica = new AreaRichiestaStatoPratica();
            areaRichiestaStatoPratica.DataElaborazioneDomandaMin = new DateTime(2010, 12, 30);
            areaRichiestaStatoPratica.DataElaborazioneDomandaMax = new DateTime(2011, 1, 1);
            areaRichiestaStatoPratica.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.StatoPratica;
            ServiceTest.SvrLiquidazione.AreaRispostaStatoPratica areaRispostaStatoPratica = null;
            areaRispostaStatoPratica = objWS.GetStatoPraticaByKey(areaRichiestaStatoPratica);
            AreaEsito areaEsito = areaRispostaStatoPratica.Esito;
            Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Operazione fallita");
            Assert.IsTrue(areaRispostaStatoPratica.ElencoDatiStatoPratica.Length == 0, "Risultati inattesi.");
        }

        [TestMethod]
        public void TestGetStatoPraticaByNumeroCertificato()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRichiestaStatoPratica areaRichiestaStatoPratica = new AreaRichiestaStatoPratica();
            areaRichiestaStatoPratica.Certificato = 99999;
            areaRichiestaStatoPratica.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.StatoPratica;
            ServiceTest.SvrLiquidazione.AreaRispostaStatoPratica areaRispostaStatoPratica = null;
            areaRispostaStatoPratica = objWS.GetStatoPraticaByKey(areaRichiestaStatoPratica);
            AreaEsito areaEsito = areaRispostaStatoPratica.Esito;
            Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Operazione fallita");
            Assert.IsTrue(areaRispostaStatoPratica.ElencoDatiStatoPratica.Length == 0, "Risultati inattesi.");
        }

        [TestMethod]
        public void TestGetStatoPraticaByCodiceFiscale()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRichiestaStatoPratica areaRichiestaStatoPratica = new AreaRichiestaStatoPratica();
            areaRichiestaStatoPratica.DatiParziali = new DatiPersonaliParziali();
            areaRichiestaStatoPratica.CodiceFiscale = "LTTDRV45B02H501E";
            areaRichiestaStatoPratica.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.StatoPratica;
            ServiceTest.SvrLiquidazione.AreaRispostaStatoPratica areaRispostaStatoPratica = null;
            areaRispostaStatoPratica = objWS.GetStatoPraticaByKey(areaRichiestaStatoPratica);
            AreaEsito areaEsito = areaRispostaStatoPratica.Esito;
            Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Operazione fallita");
            if (areaRispostaStatoPratica.ElencoDatiStatoPratica.Length == 0)
                Assert.Fail("Nessun risultato trovato");
            foreach (var risultato in areaRispostaStatoPratica.ElencoDatiStatoPratica)
            {
                Assert.IsTrue(risultato.CodiceFiscale == areaRichiestaStatoPratica.CodiceFiscale, "Codice Fiscale non valido");
            }
        }

        [TestMethod]
        public void TestGetStatoPraticaByStatoPensione()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRichiestaStatoPratica areaRichiestaStatoPratica = new AreaRichiestaStatoPratica();
            areaRichiestaStatoPratica.StatoPensione = 1;
            areaRichiestaStatoPratica.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.StatoPratica;
            ServiceTest.SvrLiquidazione.AreaRispostaStatoPratica areaRispostaStatoPratica = null;

            areaRispostaStatoPratica = objWS.GetStatoPraticaByKey(areaRichiestaStatoPratica);
            AreaEsito areaEsito = areaRispostaStatoPratica.Esito;
            Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Operazione fallita");
            if (areaRispostaStatoPratica.ElencoDatiStatoPratica.Length == 0)
                Assert.Fail("Nessun risultato trovato");
            foreach (var risultato in areaRispostaStatoPratica.ElencoDatiStatoPratica)
            {
                Assert.IsTrue(risultato.Stato == "IN ACQUISIZIONE", "Stato Pensione non valido");
            }
        }

        [TestMethod]
        public void TestGetStatoPraticaByCategoria()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRichiestaStatoPratica areaRichiestaStatoPratica = new AreaRichiestaStatoPratica();
            areaRichiestaStatoPratica.Categoria = "VEL   ";
            areaRichiestaStatoPratica.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.StatoPratica;
            ServiceTest.SvrLiquidazione.AreaRispostaStatoPratica areaRispostaStatoPratica = null;
            areaRispostaStatoPratica = objWS.GetStatoPraticaByKey(areaRichiestaStatoPratica);
            AreaEsito areaEsito = areaRispostaStatoPratica.Esito;
            Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Operazione fallita");
            if (areaRispostaStatoPratica.ElencoDatiStatoPratica.Length == 0)
                Assert.Fail("Nessun risultato trovato");
            foreach (var risultato in areaRispostaStatoPratica.ElencoDatiStatoPratica)
            {
                Assert.IsTrue(risultato.Categoria == areaRichiestaStatoPratica.Categoria, "Categoria non valida");
            }
        }

        [TestMethod]
        public void TestGetStatoPraticaByDataPresentazioneDomanda()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRichiestaStatoPratica areaRichiestaStatoPratica = new AreaRichiestaStatoPratica();
            areaRichiestaStatoPratica.DataPresentazioneDomandaMin = new DateTime(2010, 12, 30);
            areaRichiestaStatoPratica.DataPresentazioneDomandaMax = new DateTime(2011, 1, 1);
            areaRichiestaStatoPratica.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.StatoPratica;
            ServiceTest.SvrLiquidazione.AreaRispostaStatoPratica areaRispostaStatoPratica = null;
            areaRispostaStatoPratica = objWS.GetStatoPraticaByKey(areaRichiestaStatoPratica);
            AreaEsito areaEsito = areaRispostaStatoPratica.Esito;
            Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Operazione fallita");
            if (areaRispostaStatoPratica.ElencoDatiStatoPratica.Length == 0)
                Assert.Fail("Nessun risultato trovato");
            foreach (var risultato in areaRispostaStatoPratica.ElencoDatiStatoPratica)
            {
                Assert.IsTrue(risultato.DataPresentazioneDomanda >= areaRichiestaStatoPratica.DataPresentazioneDomandaMin
                    && risultato.DataPresentazioneDomanda <= areaRichiestaStatoPratica.DataPresentazioneDomandaMax, "Data Presentazione Domanda non valida");
            }
        }

        [TestMethod]
        public void TestGetStatoPraticaByCategoria_StatoPensione_DataPresentazioneDomanda()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRichiestaStatoPratica areaRichiestaStatoPratica = new AreaRichiestaStatoPratica();
            // categoria
            areaRichiestaStatoPratica.Categoria = "VEL   ";
            // stato pensione
            areaRichiestaStatoPratica.StatoPensione = 1;
            // intervallo data presentazione domanda
            areaRichiestaStatoPratica.DataPresentazioneDomandaMin = new DateTime(2010, 12, 30);
            areaRichiestaStatoPratica.DataPresentazioneDomandaMax = new DateTime(2011, 1, 1);

            areaRichiestaStatoPratica.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.StatoPratica;
            ServiceTest.SvrLiquidazione.AreaRispostaStatoPratica areaRispostaStatoPratica = null;
            areaRispostaStatoPratica = objWS.GetStatoPraticaByKey(areaRichiestaStatoPratica);
            AreaEsito areaEsito = areaRispostaStatoPratica.Esito;
            Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Operazione fallita");
            if (areaRispostaStatoPratica.ElencoDatiStatoPratica.Length == 0)
                Assert.Fail("Nessun risultato trovato");
            foreach (var risultato in areaRispostaStatoPratica.ElencoDatiStatoPratica)
            {
                Assert.IsTrue(risultato.Stato == "IN ACQUISIZIONE", "Stato Pensione non valido");
                Assert.IsTrue(risultato.Categoria == areaRichiestaStatoPratica.Categoria, "Categoria non valida");
                Assert.IsTrue(risultato.DataPresentazioneDomanda >= areaRichiestaStatoPratica.DataPresentazioneDomandaMin
                    && risultato.DataPresentazioneDomanda <= areaRichiestaStatoPratica.DataPresentazioneDomandaMax, "Data Presentazione Domanda non valida");
            }
        }

        [TestMethod]
        public void TestGetStatoPraticaBySede()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRichiestaStatoPratica areaRichiestaStatoPratica = new AreaRichiestaStatoPratica();
            areaRichiestaStatoPratica.Sede = "2100";
            areaRichiestaStatoPratica.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.StatoPratica;
            ServiceTest.SvrLiquidazione.AreaRispostaStatoPratica areaRispostaStatoPratica = null;
            areaRispostaStatoPratica = objWS.GetStatoPraticaByKey(areaRichiestaStatoPratica);
            AreaEsito areaEsito = areaRispostaStatoPratica.Esito;
            Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Operazione fallita");
            if (areaRispostaStatoPratica.ElencoDatiStatoPratica.Length == 0)
                Assert.Fail("Nessun risultato trovato");
            foreach (var risultato in areaRispostaStatoPratica.ElencoDatiStatoPratica)
            {
                Assert.IsTrue(risultato.Sede == areaRichiestaStatoPratica.Sede, "Codice sede non valido");
            }
        }

        [TestMethod]
        public void TestGetStatoPraticaByFondo()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRichiestaStatoPratica areaRichiestaStatoPratica = new AreaRichiestaStatoPratica();
            areaRichiestaStatoPratica.Fondo = "003";
            areaRichiestaStatoPratica.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.StatoPratica;
            ServiceTest.SvrLiquidazione.AreaRispostaStatoPratica areaRispostaStatoPratica = null;
            areaRispostaStatoPratica = objWS.GetStatoPraticaByKey(areaRichiestaStatoPratica);
            AreaEsito areaEsito = areaRispostaStatoPratica.Esito;
            Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Operazione fallita");
            if (areaRispostaStatoPratica.ElencoDatiStatoPratica.Length == 0)
                Assert.Fail("Nessun risultato trovato");
            foreach (var risultato in areaRispostaStatoPratica.ElencoDatiStatoPratica)
            {
                Assert.IsTrue(risultato.Fondo == areaRichiestaStatoPratica.Fondo, "Fondo non valido");
            }
        }

        [TestMethod]
        public void TestGetStatoPraticaByTipo()
        {
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaRichiestaStatoPratica areaRichiestaStatoPratica = new AreaRichiestaStatoPratica();
            areaRichiestaStatoPratica.Tipo = "0001";
            areaRichiestaStatoPratica.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.StatoPratica;
            ServiceTest.SvrLiquidazione.AreaRispostaStatoPratica areaRispostaStatoPratica = null;
            areaRispostaStatoPratica = objWS.GetStatoPraticaByKey(areaRichiestaStatoPratica);
            AreaEsito areaEsito = areaRispostaStatoPratica.Esito;
            Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Operazione fallita");
            if (areaRispostaStatoPratica.ElencoDatiStatoPratica.Length == 0)
                Assert.Fail("Nessun risultato trovato");
            foreach (var risultato in areaRispostaStatoPratica.ElencoDatiStatoPratica)
            {
                Assert.IsTrue(risultato.Tipo == areaRichiestaStatoPratica.Tipo, "Tipo non valido");
            }
        }

        #endregion Ricerca stato pratica

        #region Delegato / tutore
        [TestMethod]
        public void TestSalvaDelegato()
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica datiRiepilogoAnagrafica = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();

            datiRiepilogoAnagrafica.Cap = "00105";
            datiRiepilogoAnagrafica.Cell = "3395123455";
            datiRiepilogoAnagrafica.CodiceFiscale = "TCNPRV70A01H501O";
            datiRiepilogoAnagrafica.CodiceStatoCivile = '1';
            datiRiepilogoAnagrafica.Cognome = "Tecniche";
            datiRiepilogoAnagrafica.ComuneNascita = "Roma";
            datiRiepilogoAnagrafica.ComuneResidenza = "Roma";
            datiRiepilogoAnagrafica.DataNascita = new DateTime(1970, 1, 5);
            datiRiepilogoAnagrafica.DecorrenzaStatoCivile = DateTime.Now;
            datiRiepilogoAnagrafica.EMail = "prove.tecniche@inps.it";
            datiRiepilogoAnagrafica.Indirizzo = "Via Roma";
            datiRiepilogoAnagrafica.IsNatoInItalia = true;
            datiRiepilogoAnagrafica.IsResidenteInItalia = true;
            datiRiepilogoAnagrafica.Nome = "Prove";
            datiRiepilogoAnagrafica.NumeroCivico = "95";
            datiRiepilogoAnagrafica.ProvinciaNascita = "RM";
            datiRiepilogoAnagrafica.ProvinciaResidenza = "RM";
            datiRiepilogoAnagrafica.Sesso = 'M';
            datiRiepilogoAnagrafica.Tel = "06555123455";
            datiRiepilogoAnagrafica.CodiceDelegato = 'D';
            datiRiepilogoAnagrafica.CodiceTutore = null;

            using (ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient())
            {
                AreaEsito areaEsito = objWS.StoreDelegato(2038517500007, datiRiepilogoAnagrafica);
                Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Errore durante il salvataggio del delegato");
            }
        }

        [TestMethod]
        public void TestSalvaTutore()
        {
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica datiRiepilogoAnagrafica = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();

            datiRiepilogoAnagrafica.Cap = "00105";
            datiRiepilogoAnagrafica.Cell = "3395123455";
            datiRiepilogoAnagrafica.CodiceFiscale = "TCNPRV70A01H501O";
            datiRiepilogoAnagrafica.CodiceStatoCivile = '1';
            datiRiepilogoAnagrafica.Cognome = "Tecniche";
            datiRiepilogoAnagrafica.ComuneNascita = "Roma";
            datiRiepilogoAnagrafica.ComuneResidenza = "Roma";
            datiRiepilogoAnagrafica.DataNascita = new DateTime(1970, 1, 5);
            datiRiepilogoAnagrafica.DecorrenzaStatoCivile = DateTime.Now;
            datiRiepilogoAnagrafica.EMail = "prove.tecniche@inps.it";
            datiRiepilogoAnagrafica.Indirizzo = "Via Roma";
            datiRiepilogoAnagrafica.IsNatoInItalia = true;
            datiRiepilogoAnagrafica.IsResidenteInItalia = true;
            datiRiepilogoAnagrafica.Nome = "Prove";
            datiRiepilogoAnagrafica.NumeroCivico = "95";
            datiRiepilogoAnagrafica.ProvinciaNascita = "RM";
            datiRiepilogoAnagrafica.ProvinciaResidenza = "RM";
            datiRiepilogoAnagrafica.Sesso = 'M';
            datiRiepilogoAnagrafica.Tel = "06555123455";
            datiRiepilogoAnagrafica.CodiceDelegato = null;
            datiRiepilogoAnagrafica.CodiceTutore = 'T';

            using (ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient())
            {
                AreaEsito areaEsito = objWS.StoreTutore(2038517500007, datiRiepilogoAnagrafica);
                Assert.IsTrue(areaEsito.RisultatoOperazione == AreaEsito.TipoEsito.OK, "Errore durante il salvataggio del tutore");
            }
        }

        #endregion Delegato / tutore

        #region Familiari
        [TestMethod]
        public void TestGetStoreFamiliari()
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.GestioneAreaFamiliariAreaFamiliare[] elencoFamiliari = new GestioneAreaFamiliariAreaFamiliare[1];
            ServiceTest.SvrLiquidazione.GestioneAreaFamiliariAreaDecFam areaDecodifica = null;
            elencoFamiliari[0] = new GestioneAreaFamiliariAreaFamiliare();
            elencoFamiliari[0].Familiare = new GestioneFamiliari.Familiare();
            elencoFamiliari[0].ElencoCodMaggFamiliari = new GestioneFamiliari.CodMaggFamiliari[2];
            elencoFamiliari[0].ElencoCodMaggFamiliari[0] = new GestioneFamiliari.CodMaggFamiliari();
            elencoFamiliari[0].ElencoCodMaggFamiliari[1] = new GestioneFamiliari.CodMaggFamiliari();
            ServiceTest.SvrLiquidazione.Anagrafica[] elencoAnagrafiche = new Anagrafica[1];
            string[] elencoFamiliariDaRimuovere = new string[1];
            elencoFamiliariDaRimuovere[0] = "";
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica anag = null;
            objWS.GetAnagraficaSoggettoByCodiceFiscale(out anag, "PRVSGR45A01H501L", 2100, "12345678", 0.ToString());
            elencoAnagrafiche[0] = new Anagrafica();
            elencoAnagrafiche[0].CodiceFiscale = anag.CodiceFiscale;
            elencoFamiliari[0].Familiare.SiglaFamiliare = 'C';
            elencoFamiliari[0].Familiare.numerodomanda = "2005449300001";
            elencoFamiliari[0].Familiare.IdAnagrafica = 873;
            elencoFamiliari[0].Familiare.IdPensione = 1485;
            elencoFamiliari[0].Familiare.CodiceFiscale = "PRVSGR45A01H501L";
            elencoFamiliari[0].ElencoCodMaggFamiliari[0].Decorrenza = new DateTime(2010, 10, 01);
            elencoFamiliari[0].ElencoCodMaggFamiliari[0].Cessazione = new DateTime(2010, 11, 01);
            elencoFamiliari[0].ElencoCodMaggFamiliari[1].Decorrenza = new DateTime(2011, 10, 01);
            elencoFamiliari[0].ElencoCodMaggFamiliari[1].Cessazione = new DateTime(2011, 11, 01);
            GestioneFamiliari.ConsultazioneUnificataANF consultazioneANF = null;
            objWS.SalvaFamiliari(2005449300001, string.Empty, "12345678", ref elencoFamiliari, elencoFamiliariDaRimuovere, ref elencoAnagrafiche, out consultazioneANF);
            objWS.GetFamiliareByNumeroDomanda(out elencoFamiliari, out elencoAnagrafiche, out areaDecodifica, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            Assert.IsTrue(elencoFamiliari.Length == 1 && elencoFamiliari[0].ElencoCodMaggFamiliari.Length == 2, "Controllo 1: Dati errati");
            elencoFamiliari[0].ElencoCodMaggFamiliari[1] = new GestioneFamiliari.CodMaggFamiliari();
            elencoFamiliari[0].ElencoCodMaggFamiliari[1].Decorrenza = new DateTime(2008, 10, 01);
            elencoFamiliari[0].ElencoCodMaggFamiliari[1].Cessazione = new DateTime(2008, 11, 01);
            objWS.SalvaFamiliari(2005449300001, string.Empty, "12345678", ref elencoFamiliari, elencoFamiliariDaRimuovere, ref elencoAnagrafiche, out consultazioneANF);
            objWS.GetFamiliareByNumeroDomanda(out elencoFamiliari, out elencoAnagrafiche, out areaDecodifica, new AreaRichiestaDomanda() { NumeroDomanda = 2125517900002 });
            Assert.IsTrue(elencoFamiliari.Length == 1 && elencoFamiliari[0].ElencoCodMaggFamiliari.Length == 2 &&
            elencoFamiliari[0].ElencoCodMaggFamiliari[1].Decorrenza.Value.Date == new DateTime(2008, 10, 01).Date, "Controllo 2: Dati errati");
            elencoFamiliariDaRimuovere[0] = "PRVSGR45A01H501L";
            elencoFamiliari = new GestioneAreaFamiliariAreaFamiliare[0];
            elencoAnagrafiche = new Anagrafica[0];
            objWS.SalvaFamiliari(2005449300001, string.Empty, "12345678", ref elencoFamiliari, elencoFamiliariDaRimuovere, ref elencoAnagrafiche, out consultazioneANF);
        }
        #endregion Familiari

        #region LiquidazioniAbilitate
        [TestMethod]
        public void TestGetStoreLiquidazioniAbilitate()
        {
            Utility.TipoAppartenenza tipoAppRuolo = Utility.TipoAppartenenza.FS;
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaLiquidazioniAbilitate areaLiquidazioniAbilitate = null;
            objWS.GetAllLiquidazioniAbilitate(out areaLiquidazioniAbilitate, tipoAppRuolo);
            Assert.IsTrue(areaLiquidazioniAbilitate != null && areaLiquidazioniAbilitate.ElencoLiquidazioniAbilitate != null && areaLiquidazioniAbilitate.ElencoLiquidazioniAbilitate.Length > 0 &&
                areaLiquidazioniAbilitate.ElencoTipologie != null && areaLiquidazioniAbilitate.ElencoTipologie.Length > 0, "Output non corretto");

            int count = areaLiquidazioniAbilitate.ElencoLiquidazioniAbilitate.Length;

            ServiceTest.SvrLiquidazione.AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata datiLiquidazioneAbilitata = new AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata();
            datiLiquidazioneAbilitata.SiglaCategoria = "VVL";
            datiLiquidazioneAbilitata.Sede = "2100";
            datiLiquidazioneAbilitata.Tipologia = AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.FS;
            datiLiquidazioneAbilitata.Ricostituzione = true;

            objWS.StoreLiquidazioneAbilitata(datiLiquidazioneAbilitata);

            objWS.GetAllLiquidazioniAbilitate(out areaLiquidazioniAbilitate, tipoAppRuolo);
            Assert.IsTrue(areaLiquidazioniAbilitate.ElencoLiquidazioniAbilitate.Length > count, "Salvataggio errato");

            objWS.DeleteLiquidazioneAbilitata(datiLiquidazioneAbilitata);

            objWS.GetAllLiquidazioniAbilitate(out areaLiquidazioniAbilitate, tipoAppRuolo);
            Assert.IsTrue(areaLiquidazioniAbilitate.ElencoLiquidazioniAbilitate.Length == count, "Eliminazione errata");
        }

        [TestMethod]
        public void TestGetStoreLiquidazioniAbilitateSuTutteLeSedi()
        {
            Utility.TipoAppartenenza tipoAppRuolo = Utility.TipoAppartenenza.FS;
            ServiceTest.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            ServiceTest.SvrLiquidazione.AreaLiquidazioniAbilitate areaLiquidazioniAbilitate = null;

            ServiceTest.SvrLiquidazione.AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata datiLiquidazioneAbilitata = new AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata();
            datiLiquidazioneAbilitata.SiglaCategoria = "VVL";
            datiLiquidazioneAbilitata.Tipologia = AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.FS;
            datiLiquidazioneAbilitata.Ricostituzione = true;

            objWS.StoreLiquidazioniAbilitateSuTutteLeSedi(datiLiquidazioneAbilitata);

            objWS.DeleteLiquidazioniAbilitateSuTutteLeSedi(datiLiquidazioneAbilitata);

            objWS.GetAllLiquidazioniAbilitate(out areaLiquidazioniAbilitate, tipoAppRuolo);

            var count = (from l in areaLiquidazioniAbilitate.ElencoLiquidazioniAbilitate
                         where l.SiglaCategoria == "VVL" && l.Tipologia == AreaLiquidazioniAbilitate.DatiLiquidazioneAbilitata.Tipo.FS &&
                         l.Ricostituzione == true
                         select l).Count();
            Assert.IsTrue(count == 0);
        }
        #endregion LiquidazioniAbilitate
    }
}

