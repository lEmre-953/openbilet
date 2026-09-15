using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using System;
using System.Collections.Generic;

namespace openbilet.Services
{
    public class IyzicoService
    {
        private static Options Options => new Options
        {
            ApiKey = AppSettings.IyzicoApiKey,
            SecretKey = AppSettings.IyzicoSecretKey,
            BaseUrl = AppSettings.IyzicoBaseUrl
        };

        public static Payment OdemeYap(
            string kartSahibi,
            string kartNumarasi,
            string sonKullanmaAy,
            string sonKullanmaYil,
            string cvc,
            string aliciAd,
            string aliciSoyad,
            string aliciEmail,
            string aliciTelefon,
            string firmaAdi,
            string seferBilgisi,
            string fiyat)
        {
            fiyat = fiyat.Replace("TL", "").Replace(",", ".").Trim();
            if (!decimal.TryParse(fiyat, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal fiyatDecimal))
            {
                fiyatDecimal = 1.0m;
            }
            string fiyatStr = fiyatDecimal.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);

            var request = new CreatePaymentRequest
            {
                Locale = Locale.TR.ToString(),
                ConversationId = Guid.NewGuid().ToString("N").Substring(0, 12),
                Price = fiyatStr,
                PaidPrice = fiyatStr,
                Currency = Currency.TRY.ToString(),
                Installment = 1,
                BasketId = "OB" + DateTime.Now.Ticks,
                PaymentChannel = PaymentChannel.WEB.ToString(),
                PaymentGroup = PaymentGroup.PRODUCT.ToString()
            };

            var paymentCard = new PaymentCard
            {
                CardHolderName = kartSahibi,
                CardNumber = kartNumarasi.Replace(" ", ""),
                ExpireMonth = sonKullanmaAy,
                ExpireYear = sonKullanmaYil,
                Cvc = cvc,
                RegisterCard = 0
            };
            request.PaymentCard = paymentCard;

            var buyer = new Buyer
            {
                Id = "BY" + DateTime.Now.Ticks,
                Name = aliciAd,
                Surname = aliciSoyad,
                GsmNumber = aliciTelefon,
                Email = aliciEmail,
                IdentityNumber = "11111111111",
                RegistrationAddress = "Test Adres Türkiye",
                Ip = "85.34.78.112",
                City = "Istanbul",
                Country = "Turkey",
                ZipCode = "34000"
            };
            request.Buyer = buyer;

            var address = new Address
            {
                ContactName = aliciAd + " " + aliciSoyad,
                City = "Istanbul",
                Country = "Turkey",
                Description = "Test Adres Türkiye",
                ZipCode = "34000"
            };
            request.ShippingAddress = address;
            request.BillingAddress = address;

            var basketItems = new List<BasketItem>
            {
                new BasketItem
                {
                    Id = "BLT" + DateTime.Now.Ticks,
                    Name = firmaAdi + " - Otobüs Bileti",
                    Category1 = "Ulaşım",
                    Category2 = "Otobüs Bileti",
                    ItemType = BasketItemType.VIRTUAL.ToString(),
                    Price = fiyatStr
                }
            };
            request.BasketItems = basketItems;

            return Payment.Create(request, Options).GetAwaiter().GetResult();
        }
    }
}
