<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCFooter.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCFooter" %>

<%--
<script type="text/javascript">
    function OpenFeedack() {
        window.open(document.getElementById("<%= hValutazione.ClientID %>").value, "_blank");
    }
</script>
--%>

<script type="text/javascript">

    function ScrollTop() {
        window.scrollTo(0, 0);
    }
</script>

<%--
<div class="feedback-container">
    <div class="container">
        <div class="row">
            <div class="col-8">
                <h3>Il tuo parere è importante</h3>
                <p>Tutti i suggerimenti, i feedback e le opinioni degli utenti che navigano il servizio Invalidità, Vecchiaia e Superstiti (IVS) sono fondamentali per migliorarlo e soddisfare le vostre esigenze</p>
            </div>
            <div class="col-4 button-container">
                <button type="button" class="tertiary" onclick="OpenFeedack()">Lasciaci la tua opinione</button>
            </div>
        </div>
    </div>
</div>

<div class="return-top">
    <div class="container">
        <div class="row">
            <div class="col-12 return-top-container">
                <button type="button" class="tertiary-bgblack" onclick="ScrollTop()">Torna su</button>
            </div>
        </div>
    </div>
</div>

<div class="footer-contacts">
    <div class="container">
        <div class="row">
            <div class="col-2">
                <img src="../App_Themes/iFrame/Images/INPS-footer-logo.png" />
            </div>
            <div class="col-10 contacts-col">
                <div class="contacts-container">
                    <div><span class="font-bold">Sede Legale:</span> Via Ciro il Grande, 21</div>
                <div>00144 Roma</div>
                <div>P.IVA 02121151001</div>
                <div class="social-icons">
                    <a href="https://www.facebook.com/INPS.PerLaFamiglia/" target="_blank"><img src="../App_Themes/iFrame/Images/facebook.png" /></a>
                    <a href="http://www.twitter.com/Inps_it" target="_blank"><img src="../App_Themes/iFrame/Images/twitter.png" /></a>
                    <a href="https://whatsapp.com/channel/0029VaPPgwX3rZZXc88ZQM34" target="_blank"><img src="../App_Themes/iFrame/Images/whatsapp.png" /></a>
                    <a href="https://www.youtube.com/user/INPSComunica" target="_blank"><img src="../App_Themes/iFrame/Images/youtube.png" /></a>
                    <a href="https://www.instagram.com/inps_social/" target="_blank"><img src="../App_Themes/iFrame/Images/instagram.png" /></a>
                    <a href="https://www.linkedin.com/company/inps-official/" target="_blank"><img src="../App_Themes/iFrame/Images/linkedin.png" /></a>
                    <a href="https://www.inps.it/it/it/rss.html" target="_blank"><img src="../App_Themes/iFrame/Images/rss.png" /></a>
                </div>
                </div>
            </div>
        </div>
    </div>
</div>
--%>
<div class="inps-gov">
    <img src="../App_Themes/<%= Page.Theme %>/Images/footer-istituzionale.png" alt="logo istituzionale" />
    <span> www.inps.gov.it © 1997-2024 Istituto Nazionale Previdenza Sociale. Tutti i diritti riservati.</span>
</div>


<asp:HiddenField runat="server" ID="hValutazione" />