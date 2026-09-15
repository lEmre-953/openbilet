using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace openbilet.Services
{
    public static class EmailService
    {

        public static bool BiletEpostaGonder(
            string aliciEmail,
            string aliciAdSoyad,
            string firma,
            string kalkis,
            string varis,
            string tarih,
            string saat,
            string fiyat,
            string koltukTipi,
            string odemeId)
        {
            try
            {
                string tarihFormatli = tarih;
                if (DateTime.TryParse(tarih, out DateTime t))
                    tarihFormatli = t.ToString("dd MMMM yyyy", new CultureInfo("tr-TR"));

                string fiyatStr = fiyat.Contains("TL") ? fiyat : fiyat + " TL";

                string htmlIcerik = BiletHtmlOlustur(
                    aliciAdSoyad, firma, kalkis, varis, tarihFormatli, saat, fiyatStr, koltukTipi, odemeId);

                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(AppSettings.SmtpEmail, AppSettings.SmtpSenderName);
                    mail.To.Add(new MailAddress(aliciEmail, aliciAdSoyad));
                    mail.Subject = $"Bilet Onayı - {firma} | {kalkis} → {varis}";
                    mail.Body = htmlIcerik;
                    mail.IsBodyHtml = true;
                    mail.BodyEncoding = System.Text.Encoding.UTF8;
                    mail.SubjectEncoding = System.Text.Encoding.UTF8;

                    using (var smtp = new SmtpClient(AppSettings.SmtpHost, AppSettings.SmtpPort))
                    {
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(AppSettings.SmtpEmail, AppSettings.SmtpPassword);
                        smtp.Timeout = 15000;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EmailService hatası: " + ex.Message);
                return false;
            }
        }

        public static Task<bool> BiletEpostaGonderAsync(
            string aliciEmail, string aliciAdSoyad, string firma, string kalkis, string varis,
            string tarih, string saat, string fiyat, string koltukTipi, string odemeId)
        {
            return Task.Run(() => BiletEpostaGonder(
                aliciEmail, aliciAdSoyad, firma, kalkis, varis, tarih, saat, fiyat, koltukTipi, odemeId));
        }

        private static string BiletHtmlOlustur(string aliciAdSoyad, string firma, string kalkis,
            string varis, string tarih, string saat, string fiyat, string koltukTipi, string odemeId)
        {
            string odemeIdKisa = string.IsNullOrEmpty(odemeId)
                ? "-"
                : (odemeId.Length > 16 ? odemeId.Substring(0, 16) + "..." : odemeId);

            return $@"
<!DOCTYPE html>
<html lang='tr'>
<head>
<meta charset='UTF-8'>
<title>Bilet Onayı</title>
</head>
<body style='margin:0; padding:0; background-color:#ecf0f1; font-family:Segoe UI, Arial, sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#ecf0f1; padding:30px 0;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0' border='0' style='background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 4px 12px rgba(0,0,0,0.08);'>

          <!-- Header -->
          <tr>
            <td style='background:linear-gradient(135deg, #2980b9 0%, #2c3e50 100%); padding:35px 30px; text-align:center;'>
              <h1 style='margin:0; color:#ffffff; font-size:28px; font-weight:bold;'>Açık Bilet</h1>
              <p style='margin:8px 0 0 0; color:rgba(255,255,255,0.85); font-size:13px;'>Otobüs Bileti Onay E-postası</p>
            </td>
          </tr>

          <!-- Selamlama -->
          <tr>
            <td style='padding:30px 35px 10px 35px;'>
              <h2 style='margin:0 0 8px 0; color:#2c3e50; font-size:20px;'>Merhaba {aliciAdSoyad},</h2>
              <p style='margin:0; color:#7f8c8d; font-size:14px; line-height:1.5;'>
                Bilet satın alımınız başarıyla tamamlandı. Bilet detaylarınız aşağıdadır:
              </p>
            </td>
          </tr>

          <!-- Bilet Kartı -->
          <tr>
            <td style='padding:20px 35px;'>
              <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#f8f9fa; border-left:5px solid #2980b9; border-radius:8px;'>
                <tr>
                  <td style='padding:25px;'>
                    <p style='margin:0 0 5px 0; color:#7f8c8d; font-size:11px; text-transform:uppercase; letter-spacing:1px;'>Firma</p>
                    <h3 style='margin:0 0 20px 0; color:#2c3e50; font-size:22px;'>{firma}</h3>

                    <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                      <tr>
                        <td width='45%' style='padding:8px 0;'>
                          <p style='margin:0; color:#7f8c8d; font-size:11px; text-transform:uppercase;'>Kalkış</p>
                          <p style='margin:3px 0 0 0; color:#2c3e50; font-size:15px; font-weight:bold;'>{kalkis}</p>
                        </td>
                        <td width='10%' style='text-align:center; color:#2980b9; font-size:22px;'>→</td>
                        <td width='45%' style='padding:8px 0;'>
                          <p style='margin:0; color:#7f8c8d; font-size:11px; text-transform:uppercase;'>Varış</p>
                          <p style='margin:3px 0 0 0; color:#2c3e50; font-size:15px; font-weight:bold;'>{varis}</p>
                        </td>
                      </tr>
                    </table>

                    <hr style='border:none; border-top:1px dashed #bdc3c7; margin:20px 0;'>

                    <table width='100%' cellpadding='0' cellspacing='0' border='0'>
                      <tr>
                        <td width='33%' style='padding:5px 0;'>
                          <p style='margin:0; color:#7f8c8d; font-size:11px; text-transform:uppercase;'>Tarih</p>
                          <p style='margin:3px 0 0 0; color:#2c3e50; font-size:13px; font-weight:600;'>{tarih}</p>
                        </td>
                        <td width='33%' style='padding:5px 0;'>
                          <p style='margin:0; color:#7f8c8d; font-size:11px; text-transform:uppercase;'>Saat</p>
                          <p style='margin:3px 0 0 0; color:#2c3e50; font-size:13px; font-weight:600;'>{saat}</p>
                        </td>
                        <td width='34%' style='padding:5px 0;'>
                          <p style='margin:0; color:#7f8c8d; font-size:11px; text-transform:uppercase;'>Koltuk Tipi</p>
                          <p style='margin:3px 0 0 0; color:#2c3e50; font-size:13px; font-weight:600;'>{koltukTipi}</p>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Fiyat Kutusu -->
          <tr>
            <td style='padding:0 35px 25px 35px;'>
              <table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#27ae60; border-radius:8px;'>
                <tr>
                  <td style='padding:18px 25px;'>
                    <table width='100%'>
                      <tr>
                        <td>
                          <p style='margin:0; color:rgba(255,255,255,0.85); font-size:12px; text-transform:uppercase;'>Toplam Ödenen</p>
                        </td>
                        <td style='text-align:right;'>
                          <p style='margin:0; color:#ffffff; font-size:24px; font-weight:bold;'>{fiyat}</p>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <!-- Ödeme ID -->
          <tr>
            <td style='padding:0 35px 25px 35px;'>
              <p style='margin:0; padding:12px 15px; background-color:#ecf0f1; border-radius:6px; color:#7f8c8d; font-size:11px; font-family:Consolas, monospace;'>
                <strong>Ödeme Referansı:</strong> {odemeIdKisa}
              </p>
            </td>
          </tr>

          <!-- Bilgilendirme -->
          <tr>
            <td style='padding:0 35px 25px 35px;'>
              <p style='margin:0; padding:15px; background-color:#fef9e7; border-left:4px solid #f39c12; color:#7d6608; font-size:12px; line-height:1.6; border-radius:4px;'>
                <strong>Önemli:</strong> Otobüse binerken kimliğinizi yanınızda bulundurmanız gerekmektedir. Sefer saatinden en az 30 dakika önce terminale gelmenizi öneririz.
              </p>
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style='padding:25px 35px; background-color:#2c3e50; text-align:center;'>
              <p style='margin:0; color:#ecf0f1; font-size:13px;'>İyi yolculuklar dileriz!</p>
              <p style='margin:8px 0 0 0; color:rgba(236,240,241,0.6); font-size:11px;'>
                Bu e-posta Open Ticket sistemi tarafından otomatik gönderilmiştir.
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
        }
    }
}
