using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.IO;
using iTextSharp.text.pdf.draw;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneStampeFAQ
    {
        public static void CaricaPdfFAQ(string tipoApp, out MemoryStream memStream)
        {
            memStream = new MemoryStream();

            Document document = new Document(PageSize.A4, 30f, 40f, 150f, 40f);
            PdfWriter writer = PdfWriter.GetInstance(document, memStream);

            document.Open();

            CustomPDFCreator customPdfCreator = new CustomPDFCreator();
            customPdfCreator.OnOpenDocument(writer, document);
            customPdfCreator.OnStartPage(writer, document);
            writer.PageEvent = customPdfCreator;

            InserisciCorpo_FAQ(document, tipoApp);
           
            document.Close();
        }

        public static void CaricaPdfAggiornamento<T>(Utility.TipoAppartenenza tipoApp, out MemoryStream memStream)
        {
            memStream = new MemoryStream();

            Document document = new Document(PageSize.A4, 30f, 40f, 150f, 40f);
            PdfWriter writer = PdfWriter.GetInstance(document, memStream);

            document.Open();

            CustomPDFCreator customPdfCreator = new CustomPDFCreator();
            customPdfCreator.OnOpenDocument(writer, document);
            customPdfCreator.OnStartPage(writer, document);
            writer.PageEvent = customPdfCreator;

            InserisciCorpo_Aggiornamento<T>(document, tipoApp);

            document.Close();
        }

        #region FAQ
        private static void InserisciCorpo_FAQ(Document document, string tipoApp)
        {
            List<Entity.FAQ> elencoFAQ = null;
            GestioneFAQ.GetFAQ(tipoApp, out elencoFAQ);
            List<BLCommon.GestioneDecodifica.TipologiaFAQ> elencoTipologiaFAQ = null;
            GestioneDecodifica.GetTipologiaFAQ(out elencoTipologiaFAQ);

            if (elencoFAQ != null && elencoFAQ.Count > 0 && elencoFAQ.Count(x => x.Visibilita) > 0)
            {
                elencoFAQ = elencoFAQ.OrderBy(x => int.Parse(x.Codice.Substring(3))).ToList();

                #region Titolo
                Font font = FontFactory.GetFont("Helvetica", 18, Font.BOLD, new Color(0, 0, 128));
                Paragraph title = new Paragraph();
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingBefore = 20f;
                title.SpacingAfter = 40f;
                Chunk titleText = new Chunk("FAQ - Procedura Liquidazione Pensioni", font);
                title.Add(titleText);
                document.Add(title);
                #endregion Titolo Domande

                List<string> elencoTipologie = elencoFAQ.Where(x => x.Visibilita).Select(x => x.Tipologia).Distinct().ToList();
                elencoTipologie = elencoTipologiaFAQ.OrderByDescending(x => x.Descrizione).Where(x => elencoTipologie.Contains(x.Codice)).Select(x => x.Descrizione).ToList();

                InserisciIndice_FAQ(document, elencoTipologie);

                #region Titolo Domande
                font = FontFactory.GetFont("Helvetica", 18, Font.BOLD, new Color(0, 0, 128));
                title = new Paragraph();
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingBefore = 20f;
                title.SpacingAfter = 20f;
                titleText = new Chunk("Domande", font);
                title.Add(titleText);
                document.Add(title);
                #endregion Titolo Domande
                
                Paragraph questions = new Paragraph();
                foreach (string tipologia in elencoTipologie)
                {
                    #region Sottotitolo
                    PdfPTable table = new PdfPTable(1);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.WidthPercentage = 100f;
                    PdfPCell cell = new PdfPCell();
                    cell.Border = 0;
                    cell.BorderWidthBottom = 1;
                    cell.BackgroundColor = new Color(232, 232, 255);
                    cell.FixedHeight = 22f;
                    font = FontFactory.GetFont("Helvetica", 15, Font.ITALIC);
                    Paragraph subtitle = new Paragraph();
                    subtitle.Alignment = Element.ALIGN_LEFT;
                    Chunk subtitleText = new Chunk(tipologia, font);
                    subtitleText.SetLocalDestination("SezioneDomande_" + tipologia);
                    subtitle.Add(subtitleText);
                    cell.AddElement(subtitle);
                    table.AddCell(cell);
                    table.SpacingAfter = 10f;
                    table.SpacingBefore = 10f;
                    questions.Add(table);
                    #endregion Sottotitolo

                    #region Domande
                    List listaQuestions = new List();
                    listaQuestions.Autoindent = false;
                    listaQuestions.SymbolIndent = 55f;
                    listaQuestions.IndentationLeft = 15f;
                    font = FontFactory.GetFont("Helvetica", 12, Font.UNDERLINE);
                    foreach (Entity.FAQ faq in elencoFAQ)
                    {
                        string appTipologia = elencoTipologiaFAQ.Where(x => x.Descrizione == tipologia).Select(x => x.Codice).Single();
                        if (faq.Visibilita && faq.Tipologia == appTipologia)
                        {
                            iTextSharp.text.ListItem item = new iTextSharp.text.ListItem();
                            Chunk domanda = new Chunk(faq.Domanda, font);
                            domanda.SetLocalGoto("Answer#" + faq.Codice);
                            item.Add(domanda);
                            item.SpacingAfter = 10f;
                            item.ListSymbol = new Chunk(faq.Codice + ".");
                            listaQuestions.Add(item);
                        }
                    }
                    questions.Add(listaQuestions);
                    #endregion Domande
                }
                document.Add(questions);
                document.Add(Chunk.NEXTPAGE);

                #region Titolo Risposte
                font = FontFactory.GetFont("Helvetica", 18, Font.BOLD, new Color(0, 0, 128));
                title = new Paragraph();
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingBefore = 20f;
                title.SpacingAfter = 20f;
                titleText = new Chunk("Risposte", font);
                title.Add(titleText);
                document.Add(title);
                #endregion Titolo Risposte

                Paragraph answers = new Paragraph();
                foreach (string tipologia in elencoTipologie)
                {
                    #region Sottotitolo
                    PdfPTable table = new PdfPTable(1);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.WidthPercentage = 100f;
                    PdfPCell cell = new PdfPCell();
                    cell.Border = 0;
                    cell.BorderWidthBottom = 1;
                    cell.BackgroundColor = new Color(232, 232, 255);
                    cell.FixedHeight = 22f;
                    font = FontFactory.GetFont("Helvetica", 15, Font.ITALIC);
                    Paragraph subtitle = new Paragraph();
                    subtitle.Alignment = Element.ALIGN_LEFT;
                    Chunk subtitleText = new Chunk(tipologia, font);
                    subtitleText.SetLocalDestination("SezioneRisposte_" + tipologia);
                    subtitle.Add(subtitleText);
                    cell.AddElement(subtitle);
                    table.AddCell(cell);
                    table.SpacingAfter = 10f;
                    table.SpacingBefore = 10f;
                    answers.Add(table);
                    #endregion Sottotitolo

                    #region Risposte
                    font = FontFactory.GetFont("Helvetica", 12);
                    List listaAnswers = new List();
                    listaAnswers.Autoindent = false;
                    listaAnswers.SymbolIndent = 55f;
                    listaAnswers.IndentationLeft = 15f;
                    foreach (Entity.FAQ faq in elencoFAQ)
                    {
                        string appTipologia = elencoTipologiaFAQ.Where(x => x.Descrizione == tipologia).Select(x => x.Codice).Single();
                        if (faq.Visibilita && faq.Tipologia == appTipologia)
                        {
                            iTextSharp.text.ListItem item = new iTextSharp.text.ListItem();
                            Chunk risposta = new Chunk(faq.Risposta, font);
                            risposta.SetLocalDestination("Answer#" + faq.Codice);
                            item.Add(risposta);
                            item.SpacingAfter = 10f;
                            item.ListSymbol = new Chunk(faq.Codice + ".");
                            listaAnswers.Add(item);
                        }
                    }
                    answers.Add(listaAnswers);
                    #endregion Risposte
                }
                document.Add(answers);
            }
            else
            {
                throw new INPS.DNA.DnaValidationException("Nessuna FAQ presente");
            }
        }

        private static void InserisciIndice_FAQ(Document document, List<string> elencoTipologie)
        {
            if (elencoTipologie != null && elencoTipologie.Count > 0)
            {
                #region Header
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100f;
                Font font = FontFactory.GetFont("Helvetica", 18, Font.ITALIC);
                PdfPCell cellHeader = new PdfPCell(new Phrase("Indice", font));
                cellHeader.BackgroundColor = new Color(232, 232, 255);
                cellHeader.PaddingBottom = 10f;
                cellHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cellHeader);
                #endregion Header

                #region Domande
                PdfPCell cellDomande = new PdfPCell();
                cellDomande.PaddingBottom = 30f;
                cellDomande.PaddingTop = 10f;
                cellDomande.PaddingLeft = 10f;
                cellDomande.AddElement(new Phrase("Domande", FontFactory.GetFont("Helvetica", 12, Font.BOLD)));
                List listaDomande = new List(List.UNORDERED, 20f);
                listaDomande.SetListSymbol("\u2022");
                listaDomande.IndentationLeft = 10f;
                foreach (string tipologia in elencoTipologie)
                {
                    ListItem item = new ListItem();
                    Chunk link = new Chunk("Vai alla sezione " + tipologia, FontFactory.GetFont("Helvetica", 12, Font.UNDERLINE));
                    link.SetLocalGoto("SezioneDomande_" + tipologia);
                    item.Add(link);
                    listaDomande.Add(item);
                }
                cellDomande.AddElement(listaDomande);
                table.AddCell(cellDomande);
                #endregion Domande

                #region Risposte
                PdfPCell cellRisposte = new PdfPCell();
                cellRisposte.PaddingBottom = 30f;
                cellRisposte.PaddingTop = 10f;
                cellRisposte.PaddingLeft = 10f;
                cellRisposte.AddElement(new Phrase("Risposte", FontFactory.GetFont("Helvetica", 12, Font.BOLD)));
                List listaRisposte = new List(List.UNORDERED, 20f);
                listaRisposte.SetListSymbol("\u2022");
                listaRisposte.IndentationLeft = 10f;
                foreach (string tipologia in elencoTipologie)
                {
                    ListItem item = new ListItem();
                    Chunk link = new Chunk("Vai alla sezione " + tipologia, FontFactory.GetFont("Helvetica", 12, Font.UNDERLINE));
                    link.SetLocalGoto("SezioneRisposte_" + tipologia);
                    item.Add(link);
                    listaRisposte.Add(item);
                }
                cellRisposte.AddElement(listaRisposte);
                table.AddCell(cellRisposte);
                #endregion Risposte

                document.Add(table);
                document.Add(Chunk.NEXTPAGE);
            }
        }
        #endregion FAQ

        #region Aggiornamento
        public static void InserisciCorpo_Aggiornamento<T>(Document document, Utility.TipoAppartenenza tipoApp)
        {
            List<GestioneAreaAggiornamento.EsitoAggiornamento> listaDomande = null;
            GestioneAreaAggiornamento.GetEsitoAggiornamentoGeneric<T>(tipoApp, out listaDomande);

            #region Titolo
            Font font = FontFactory.GetFont("Helvetica", 18, Font.BOLD, new Color(0, 0, 128));
            Paragraph title = new Paragraph();
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingBefore = 20f;
            title.SpacingAfter = 30f;
            Chunk titleText = new Chunk();
            Type listType = typeof(T);
            if (listType == typeof(GestioneEsitoAggiornamentoWebDom.EsitoAggiornamentiWebDom))
                titleText = new Chunk("Aggiornamento WebDom - Procedura Liquidazione Pensioni", font);
            else if (listType == typeof(GestioneEsitoAggiornamentoFelpe.EsitoAggiornamentiFelpe))
                titleText = new Chunk("Aggiornamento Felpe - Procedura Liquidazione Pensioni", font);
            else if (listType == typeof(GestioneEsitoAggiornamentoOneri.EsitoAggiornamentiOneri))
                titleText = new Chunk("Aggiornamento Oneri - Procedura Liquidazione Pensioni", font);
            title.Add(titleText);
            document.Add(title);
            #endregion Titolo

            #region SezioneRiepilogo
            Paragraph riepilogo = new Paragraph();
            int NDomandeElaborate = 0;
            int NDomandeNonElaborate = 0;
            int NDomandeEsitoPositivo = 0;
            int NDomandeEsitoNegativo = 0;

            if (listaDomande != null && listaDomande.Count > 0)
            {
                NDomandeElaborate = listaDomande.Count(x => x.Esito.HasValue);
                NDomandeNonElaborate = listaDomande.Count(x => !x.Esito.HasValue);
                NDomandeEsitoPositivo = listaDomande.Count(x => x.Esito.GetValueOrDefault());
                NDomandeEsitoNegativo = listaDomande.Count(x => x.Esito.HasValue && !x.Esito.Value);
            }

            #region Sottotitolo
            PdfPTable table = new PdfPTable(1);
            table.HorizontalAlignment = Element.ALIGN_LEFT;
            table.WidthPercentage = 100f;
            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthBottom = 1;
            cell.BackgroundColor = new Color(232, 232, 255);
            cell.FixedHeight = 22f;
            font = FontFactory.GetFont("Helvetica", 15, Font.ITALIC);
            Paragraph subtitle = new Paragraph();
            subtitle.Alignment = Element.ALIGN_LEFT;
            Chunk subtitleText = new Chunk("Riepilogo precedente elaborazione", font);
            subtitle.Add(subtitleText);
            cell.AddElement(subtitle);
            table.AddCell(cell);
            table.SpacingAfter = 10f;
            riepilogo.Add(table);
            #endregion Sottotitolo

            #region Riepilogo
            PdfPTable tableRiepilogo = new PdfPTable(4);
            tableRiepilogo.HorizontalAlignment = Element.ALIGN_LEFT;
            tableRiepilogo.KeepTogether = true;
            tableRiepilogo.WidthPercentage = 100f;
            tableRiepilogo.SetWidths(new int[] { 40, 10, 40, 10 });
            font = FontFactory.GetFont("Helvetica", 13, Font.NORMAL);

            #region Domande Elaborate
            cell = new PdfPCell();
            cell.Border = 0;
            Chunk testo = new Chunk("Domande elaborate:", font);
            cell.AddElement(testo);
            tableRiepilogo.AddCell(cell);

            cell = new PdfPCell();
            cell.Border = 0;
            testo = new Chunk(NDomandeElaborate.ToString(), font);
            cell.AddElement(testo);
            tableRiepilogo.AddCell(cell);
            #endregion Domande Elaborate

            #region Domande non Elaborate
            cell = new PdfPCell();
            cell.Border = 0;
            testo = new Chunk("Domande non elaborate*:", font);
            cell.AddElement(testo);
            tableRiepilogo.AddCell(cell);

            cell = new PdfPCell();
            cell.Border = 0;
            testo = new Chunk(NDomandeNonElaborate.ToString(), font);
            cell.AddElement(testo);
            tableRiepilogo.AddCell(cell);
            #endregion Domande non Elaborate

            #region Domande con Esito Positivo
            cell = new PdfPCell();
            cell.Border = 0;
            testo = new Chunk("Domande con esito positivo:", font);
            cell.AddElement(testo);
            tableRiepilogo.AddCell(cell);

            cell = new PdfPCell();
            cell.Border = 0;
            testo = new Chunk(NDomandeEsitoPositivo.ToString(), font);
            cell.AddElement(testo);
            tableRiepilogo.AddCell(cell);
            #endregion Domande con Esito Positivo

            #region Domande con Esito Negativo
            cell = new PdfPCell();
            cell.Border = 0;
            testo = new Chunk("Domande con esito negativo:", font);
            cell.AddElement(testo);
            tableRiepilogo.AddCell(cell);

            cell = new PdfPCell();
            cell.Border = 0;
            testo = new Chunk(NDomandeEsitoNegativo.ToString(), font);
            cell.AddElement(testo);
            tableRiepilogo.AddCell(cell);
            #endregion Domande con Esito Negativo

            riepilogo.Add(tableRiepilogo);
            #endregion Riepilogo

            #region Nota
            font = FontFactory.GetFont("Helvetica", 10, Font.ITALIC);
            Chunk nota = new Chunk("* Domande per le quali non è stato possibile effettuare l'elaborazione a causa di un errore tecnico durante il processo.", font);
            riepilogo.Add(nota);
            #endregion Nota

            document.Add(riepilogo);

            #endregion SezioneRiepilogo

            #region SezioneTabella
            if (listaDomande != null && listaDomande.Count > 0)
            {
                #region Sottotitolo
                table = new PdfPTable(1);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.WidthPercentage = 100f;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.BorderWidthBottom = 1;
                cell.BackgroundColor = new Color(232, 232, 255);
                cell.FixedHeight = 22f;
                font = FontFactory.GetFont("Helvetica", 15, Font.ITALIC);
                subtitle = new Paragraph();
                subtitle.Alignment = Element.ALIGN_LEFT;
                subtitleText = new Chunk("Elenco domande con esito negativo", font);
                subtitle.Add(subtitleText);
                cell.AddElement(subtitle);
                table.AddCell(cell);
                table.SpacingAfter = 10f;
                table.SpacingBefore = 30f;
                document.Add(table);
                #endregion Sottotitolo

                #region Tabella

                PdfPTable tableDomande = new PdfPTable(2);
                tableDomande.HorizontalAlignment = Element.ALIGN_LEFT;
                tableDomande.WidthPercentage = 100f;
                tableDomande.SetWidths(new int[] { 30, 70 });
                font = FontFactory.GetFont("Helvetica", 15, Font.BOLD);

                #region Header
                cell = new PdfPCell(new Phrase("Numero domanda", font));
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                tableDomande.AddCell(cell);

                cell = new PdfPCell(new Phrase("Errore", font));
                cell.PaddingLeft = 6f;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                tableDomande.AddCell(cell);
                #endregion Header

                #region Elenco
                foreach (GestioneAreaAggiornamento.EsitoAggiornamento domanda in listaDomande.FindAll(x => x.Esito.HasValue && !x.Esito.Value))
                {
                    font = FontFactory.GetFont("Helvetica", 13, Font.NORMAL);
                    cell = new PdfPCell(new Phrase(domanda.Ndomus.ToString(), font));
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    tableDomande.AddCell(cell);

                    font = FontFactory.GetFont("Helvetica", 12, Font.NORMAL);
                    cell = new PdfPCell(new Phrase(domanda.Errore, font));
                    cell.PaddingLeft = 6f;
                    cell.PaddingBottom = 6f;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    tableDomande.AddCell(cell);
                }
                #endregion Elenco

                document.Add(tableDomande);
                #endregion Tabella
            }
            #endregion SezioneTabella
        }
        #endregion Aggiornamento

        #region nested class
        public class CustomPDFCreator : PdfPageEventHelper
        {
            public PdfTemplate template;

            public override void OnStartPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
            {
                writer.DirectContent.AddTemplate(template, 0f, 0f);
                PdfContentByte cb = writer.DirectContent;

                System.Reflection.Assembly thisExe;
                thisExe = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream file = thisExe.GetManifestResourceStream("INPS.Pensioni.Liquidazione.Images.logo.jpg");
                System.Drawing.Image imgLogo = System.Drawing.Image.FromStream(file);

                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imgLogo, System.Drawing.Imaging.ImageFormat.Jpeg);
                img.ScalePercent(22);
                img.SetAbsolutePosition(20f, (PageSize.A4.Height - img.ScaledHeight) - 10f);
                cb.AddImage(img);

                BaseFont bf = FontFactory.GetFont("Helvetica").BaseFont;
                cb.BeginText();
                float fontsize = 9;
                cb.SetFontAndSize(bf, fontsize);
                string linea1 = "Pag. " + string.Format("{0:0}", writer.PageNumber) + "/";
                float w = bf.GetWidthPoint(linea1, fontsize);
                cb.SetTextMatrix((document.PageSize.Width - document.RightMargin - w - 18), 15);
                cb.ShowText(linea1);
                cb.EndText();
            }

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                PdfTemplate cb = this.template;

                BaseFont bf = FontFactory.GetFont("Helvetica").BaseFont;

                cb.BeginText();
                float fontsize = 9;
                cb.SetFontAndSize(bf, fontsize);
                int pageNumber = writer.PageNumber - 1;
                string linea1 = string.Format("{0:0}", pageNumber);
                float w = bf.GetWidthPoint(linea1, fontsize);
                int margine = 11;
                if (pageNumber > 100)
                    margine = 2;
                else if (pageNumber > 10)
                    margine = 8;
                cb.SetTextMatrix((document.PageSize.Width - document.RightMargin - w - margine), 15);
                cb.ShowText(linea1);
                cb.EndText();
            }

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                template = writer.DirectContent.CreateTemplate(document.PageSize.Width, document.PageSize.Height);
            }
        }
        #endregion nested class
    }
}
