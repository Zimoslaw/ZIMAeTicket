using MailKit.Net.Smtp;
using MimeKit;
using QRCoder;
using System.Drawing;

namespace ZIMAeTicket.Services
{
    public partial class MailingService
    {
        public string StatusMessage { get; set; }

        // Mailing
        public SmtpClient SmtpClient { get; set; }

        public MimeMessage MimeMessage { get; set; }

        public BodyBuilder BodyBuilder { get; set; }

        // QR
        readonly QRCodeGenerator qrCodeGenerator;

        // PDF
        readonly PDFService pdfService;

        // Order data
        string orderId = string.Empty;
        string dateOfEmail = string.Empty;
        string dateOfOrder = string.Empty;
        string orderEmail = string.Empty;
        string buyer = string.Empty;
        string eventName = string.Empty;

        public MailingService()
        {
            qrCodeGenerator = new QRCodeGenerator();
            pdfService = new PDFService();
            SmtpClient = new SmtpClient();
            MimeMessage = new MimeMessage();
            BodyBuilder = new BodyBuilder();

            StatusMessage = "Initialized";
        }

        public bool InitSMTPConnection()
        {
            try
            {
                SmtpClient.Connect(AccessStrings.SMTPServerAddress, AccessStrings.SMTPServerPort, MailKit.Security.SecureSocketOptions.Auto);
                SmtpClient.Authenticate(AccessStrings.SMTPUsername, AccessStrings.SMTPPassword);
                return true;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to connect to SMTP server. {0}", ex.Message);
                return false;
            }
        }

        public void CloseSMTPConnection()
        {
            SmtpClient.Disconnect(true);
        }

        public void InitMessage(string orderId, string dateOfEmail, string dateOfOrder, string receiverAddress, string receiverName, string eventName)
        {
            this.orderId = orderId;
            this.dateOfOrder = dateOfOrder;
            this.dateOfEmail = dateOfEmail;
            orderEmail = receiverAddress;
            buyer = receiverName;
            this.eventName = eventName;

            MimeMessage.Subject = $"Zamówienie numer: {this.orderId} - Bilety do zamówienia";
            MimeMessage.From.Add(new MailboxAddress("ZIMA - sklep muzyczny", AccessStrings.SMTPUsername));
            MimeMessage.To.Add(new MailboxAddress(buyer, orderEmail));

            BodyBuilder.HtmlBody = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.=w3.org/1999/xhtml"" style=""margin: 0;padding: 0;font-family: Arial, sans-serif;"">

<head style=""margin: 0;padding: 0;font-family: Arial, sans-serif;"">
    <meta name=""viewport"" content=""width=device-width"" style=""margin: 0;padding: 0;font-family: Arial, sans-serif;"">
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" style=""margin: 0;padding: 0;font-family: Arial, sans-serif;"">
    <title style=""margin: 0;padding: 0;font-family: Arial, sans-serif;"">ZIMAeTicketMail</title>

    <style style=""margin: 0;padding: 0;font-family: Arial, sans-serif;"">
        /* --------=
-----------------------------=20
        GLOBAL=20
------------------------=
------------- */
        * {
            margin: 0;
            padding: 0;
        }

        * {
            font-family: Arial, sans-serif
        }

        img {
            max-width: 100%;
        }

        .collapse {
            margin: 0;
            padding: 0;
        }

        body {
            -webkit-font-smoothing: antialiased;
            -webkit-text-size-adjust: none;
            width: 100% !important;
            height: 100%;
        }

        a {
            color: #e3251e;
        }

        .btn {
            text-decoration: none;
            color: #FFF;
            background-color: #666;
            padding: 10px 16px;
            font-weight: bold;
            margin-right: 10px;
            text-align: center;
            cursor: pointer;
            display: inline-block;
        }

        p.callout {
            padding: 15px;
            background-color: #ECF8FF;
            margin-bottom: 15px;
        }

        .callout a {
            font-weight: bold;
            color: #2BA6CB;
        }

        table.social {

            background-color: #ebebeb;
        }

        .social .soc-btn {
            padding: 3px 7px;
            font-size: 12px;
            margin-bottom: 10px;
            text-decoration: none;
            color: #FFF;
            font-weight: bold;
            display: block;
            text-align: center;
        }

        a.fb {
            background-color: #3B5998 !important;
        }

        a.tw {
            background-color: #1daced !important;
        }

        a.gp {
            background-color: #DB4A39 !important;
        }

        a.ms {
            background-color: #000 !important;
        }

        .sidebar .soc-btn {
            display: block;
            width: 100%;
        }

        table.head-wrap {
            width: 100%;
        }

        .header.container table td.logo {
            padding: 15px;
        }

        .header.container table td.label {
            padding: 15px;
            padding-left: 0px;
        }

        table.body-wrap {
            width: 100%;
        }

        table.footer-wrap {
            width: 100%;
            clear: both !important;
        }

        .footer-wrap .container td.content p {
            border-top: 1px solid rgb(215, 215, 215);
            padding-top: 15px;
        }

        .footer-wrap .container td.content p {
            font-size: 10px;
            font-weight: bold;
        }

        h1,
        h2,
        h3,
        h4,
        h5,
        h6 {
            font-family: Arial, sans-serif;
            line-height: 1.1;
            margin-bottom: 15px;
            color: #000;
        }

        h1 small,
        h2 small,
        h3 small,
        h4 small,
        h5 small,
        h6 small {
            font-size: 60%;
            color: #6f6f6f;
            line-height: 0;
            text-transform: none;
        }

        h1 {
            font-weight: 200;
            font-size: 44px;
        }

        h2 {
            font-weight: 200;
            font-size: 37px;
        }

        h3 {
            font-weight: 500;
            font-size: 27px;
        }

        h4 {
            font-weight: 500;
            font-size: 23px;
        }

        h5 {
            font-weight: 900;
            font-size: 17px;
        }

        h6 {
            font-weight: 900;
            font-size: 14px;
            text-transform: uppercase;
            color: #444;
        }

        .collapse {
            margin: 0 !important;
        }

        p, ul {
            margin-bottom: 10px;
            font-weight: normal;
            font-size: 12px;
            line-height: 1.6;
        }

        p.lead {
            font-size: 17px;
        }

        p.last {
            margin-bottom: 0px;
        }

        ul li {
            margin-left: 5px;
            list-style-position: inside;
        }

        ul.sidebar {
            background: #ebebeb;
            display: block;
            list-style-type: none;
        }

        ul.sidebar li {
            display: block;
            margin: 0;
        }

        ul.sidebar li a {
            text-decoration: none;
            color: #666;
            padding: 10px 16px;
            /*  font-weight:bold; */
            margin-right: 10px;
            /*  text-align:center; */
            cursor: pointer;
            border-bottom: 1px solid #777777;
            border-top: 1px solid #FFFFFF;
            display: block;
            margin: 0;
        }

        ul.sidebar li a.last {
            border-bottom-width: 0px;
        }

        ul.sidebar li a h1,
        ul.sidebar li a h2,
        ul.sidebar li a h3,
        ul.sidebar li a h4,
        ul.sidebar li a h5,
        ul.sidebar li a h6,
        ul.sidebar li a p {
            margin-bottom: 0 !important;
        }



        /* ----------------------------------=
-----------------=20
        RESPONSIVENESS
        Nuke it from orbit. I=
t's the only way to be sure.=20
-------------------------------------------=
----------- */

        /* Set a max-width, and make it display as block so it =
will automatically stretch to that width, but will also shrink down on a ph=
one or something */
        .container {
            display: block !important;
            max-width: 600px !important;
            margin: 0 auto !important;
            /* makes it centered=
 */
            clear: both !important;
        }

        /* This should also be a block ele=
ment, so that it will fill 100% of the .container */
        .content {
            padding: 15px;
            max-width: 600px;
            margin: 0 auto;
            display: block;
        }

        /* Let's make sure tables in the content area are 100% wide */
        .content table {
            width: 100%;
        }

        /* Odds and ends */
        .column {
            width: 300px;
            float: left;
        }

        .social .column tr td {
            padding: 15px;
        }

        .user_data .column tr td {
            padding-bottom: 15px;
        }

        .column-wrap {
            padding: 0 !important;
            margin: 0 auto;
            max-width: 600px !important;
        }

        .column table {
            width: 100%;
        }

        .social .column {
            width: 280px;
            min-width: 279px;
            float: left;
        }

        .user_data .column {
            width: 280px;
            min-width: 279px;
            float: left;
        }

        .clear {
            display: block;
            clear: both;
        }

        @media only screen and (max-width: 600px) {
            a[class=""btn""] {
                display: block !important;
                margin-bottom: 10px !important;
                background-image: none !important;
                margin-right: 0 !important;
            }

            div[class=""column""] {
                width: auto !important;
                float: none !important;
            }

            table.social div[class=""column""] {
                width: auto !important;
            }

        }
    </style>

</head>

<body style=""background-color: #FFFFFF; margin: 0;padding: 0;font-family: Arial, sans-se=rif;font-size:
    12px;-webkit-font-smoothing: antialiased;-webkit-text-size-adjust: none;height: 100%;width: 100%!important;"">

    <!-- HEADER -->
    <table class=""head-wrap"" style=""background-color: #F5F5F5; margin: 0;padding: 0;font-family: Arial,
        sans-serif;font-size: 12px;width: 100%;"">
        <tr style=""margin: 0;padding: 0;font-family: Arial, sans-serif;font-size: 12px;"">
            <td style=""margin: 0;padding: 0;font-family: Arial, sans-serif;font-size: 12px;""></td>
            <td class=""header container"" style=""margin: 0 auto!important;padding: 0;font-family: Arial, sans-serif;font-size: 12px;display: block!important;max-width: 600px!important;clear: both!important;"">
                <div class=""content"" style=""margin: 0 auto;padding: 15px;font-family: Arial,
                    sans-serif;font-size:12px;max-width: 600px;display: block;"">
                    <table style=""margin: 0;padding: 0;font-family: Arial, sans-serif;font-size:
                        12px;width: 100%;"">
                        <tr style=""margin: 0;padding: 0;font-family: Arial, sans-serif;font-size: 12px;"">
                            <td style=""margin: 0;padding: 0;font-family: Arial, sans-serif;font-size: 12px;"">
                                <img src=""http://zima.sklep.pl/uploads/picture/a550e5852ce7f3e4fc35434002b1cbc6.png?version=1""
                                    style=""margin: 0;padding: 0;font-family: Arial, sans-serif;font-size: 12px;max-width: 100%; max-height:50px;""></td>
                            <td align=""right"" style=""margin:0;padding: 0;font-family: Arial, sans-serif;"">
                                <h6 class=""collapse"" style=""margin: 0!important;padding: 0;font-family:
                                    'helveticaneue-light', 'helvetica neue light', 'helvetica
                                    neue', helvetica, arial, 'lucida grande', sans-serif:
                                    ;line-height: 1.1;margin-bottom: 15px;color: #444;font-weight: 900;font-size:
                                    14px;text-transform: uppercase;"">" + this.dateOfEmail + @"</h6>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
            <td style=""margin: 0;padding: 0;font-family: Arial, sans-serif;""></td>
        </tr>
        </table><!-- /HEADER -->
        <!-- BODY -->
        <table class=""body-wrap"" cellpadding=""0"" cellspacing=""0"" style=""margin: 0;padding: 0;font-family: Arial, sans-serif;width: 100%;"">
            <tr style=""margin: 0;padding: 0;font-family: Arial, sans-serif;"">
                <td style=""margin: 0;padding: 0;font-family: Arial, sans-serif;""></td>
                <td class=""container"" style=""background-color: #FFFFFF; margin: 0 auto!important;padding: 0;font-family:
                    Arial, sans-serif;display: block!important;max-width: 600px!important;clear: both!important;"">
                    <div class=""content"" style=""margin: 0 auto;padding: 15px;font-family: Arial,
                        sans-serif;max-width: 600px;display: block;"">
                        <table cellpadding=""0"" cellspacing=""0"" style=""margin: 0;padding: 0;font-family: Arial,
                            sans-serif;width: 100%;"">
                            <tr style=""margin: 0;padding: 0;font-family: Arial, sans-serif;"">
                                <td style=""margin: 0;padding: 0;font-family: Arial, sans-serif;"">
                                    <p style=""font-size: 12px;margin: 0;padding: 0;font-family: Arial,
                                        sans-serif;margin-bottom: 10px;font-weight: normal;line-height: 1.6;""></p>
                                    <h4 style=""margin: 0;padding: 0;font-family: 'helveticaneue-light', 'helvetica neue light', 'helvetica neue', helvetica, arial,
                                        'lucida grande', sans-serif ;line-height: 1.1;margin-bottom:
                                        15px;color: #000;font-weight: 500;font-size: 23px;"">Zamówienie numer: " + this.orderId + @"
                                        </h4>

                                            <div style=""font-size:12px; color:#576278;"">
                                                Data złożenia zamówienia:<span style=""color:#404040; padding-left:
                                                    5px"">" + this.dateOfOrder + @"<span>
                                            </div>
                                            <br />

                                            <p style=""font-size: 12px;margin: 0;padding: 0;font-family: Arial,
                                                sans-serif;margin-bottom: 10px;font-weight: normal;line-height: 1.6;"">
                                                W załączniku znajdziesz zamówione bilety na wydarzenie. Pobierz je na swojego smartfona lub wydrukuj - zostaną one sprawdzone przy wejściu na wydarzenie.
                                                <b>Ukazanie ważnego biletu jest wymagane aby wejść na teren wydarzenia!</b>
                                                </p>
                                                    <div>
                                                        <p class=""callout"" style=""text-align: center;margin:
                                                            0;padding: 15px;font-family: Arial, sans-ser=if;font-size:
                                                            12px;margin-bottom: 15px;font-weight: normal;line-height:
                                                            1.6;background-color: #e82c42;"">
                                                            <span style=""color:#FFFFFF; font-size:12px; font-weight: bold;"">Twoje bilety znajdziesz w załączniku</span>
                                            </p><!-- /Callout Panel -->
                                        </div>

                                        <br />

                                        <div style=""margin:10px 0px 7px 0px"">

                                            <div style=""font-family:Verdana,Arial,Helvetica,sans-serif;
                                                line-height:18px; font-size:10px;"">
                                            </div>

                                            <div style=""font-family:Verdana,Arial,Helvetica,sans-serif;
                                                line-height:18px; font-size:12px;"">
                                            </div>

                                        </div>
                                        <!-- social & contact -->
                                        <table class=""social"" width=""100%"" style=""margin: 0;padding:
                                            0;font-family: Arial, sans-serif;background-color: #ebebeb;width: 100%;"">
                                            <tr style=""margin: 0;padding:0;font-family: Arial, sans-serif;"">
                                                <td style=""font-size: 12px;margin: 0;padding: 0;font-family: Arial,
                                                    sans-serif;"">
                                                </td>
                                            </tr>
                                        </table><!-- /social & contact -->
                                </td>
                            </tr>
                        </table>

                    </div><!-- /content -->
                </td>
                <td style=""margin: 0;padding: 0;font-family:Arial, sans-serif;""></td>
                </tr>
        </table><!-- /BODY -->

        </body>

</html>";
        }

#if WINDOWS
        public async Task<bool> AttatchQRCodeToMessage(int ticketId, string hash)
        {
            try
            {
                // Generating QR code
                QRCodeData qRCodeData = qrCodeGenerator.CreateQrCode(hash, QRCodeGenerator.ECCLevel.Q);
                QRCode qRCode = new(qRCodeData);
                Bitmap qRCodeImage = qRCode.GetGraphic(100);

                byte[] pdfTicketBytes;
                using (MemoryStream ms = new())
                {
                    qRCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    pdfTicketBytes = await pdfService.CreateNewPDFTicket(ms, orderId, orderEmail, dateOfOrder, ticketId, eventName);
                }

                // Attaching QR code
                BodyBuilder.Attachments.Add($"Bilet-Zima-{hash[..16]}.pdf", pdfTicketBytes, new ContentType("application", "pdf"));

                return true;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error in attaching ticket PDF: {ex}";
                return false;
            }
        }
#endif

        public async Task<bool> SendMail()
        {
            try
            {
                MimeMessage.Body = BodyBuilder.ToMessageBody();

                var response = await SmtpClient.SendAsync(MimeMessage);

                Debug.WriteLine(response);
                return true;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to send e-mail message. {0}", ex.Message);
                return false;
            }
        }
    }
}
