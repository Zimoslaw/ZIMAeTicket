using System.Globalization;

namespace ZIMAeTicket.Converters
{
    internal class BuyerMaskConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string buyer || string.IsNullOrWhiteSpace(buyer))
                return string.Empty;

            var words = buyer.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<string> maskedWords = new List<string>();

            foreach (string word in words)
            {
                if (word.Length < 3)
                {
                    maskedWords.Add(word);
                    continue;
                }

                maskedWords.Add(word.Substring(0, 2) + new string('*', word.Length - 2));
            }

            return string.Join(' ', maskedWords);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
