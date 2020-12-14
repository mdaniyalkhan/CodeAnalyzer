namespace code_analyzer.common
{
    public static class Extension
    {
        private const string UnderScore = "_";
        private const string Dollar = "$";
        private const string NewDollar = "Dollar";
        private const string OldIdValue = "id";
        private const string NewIdValue = "Id";
        private const string OldUrlValue = "URL";
        private const string NewUrlValue = "Url";

        public static string Capitalize(this string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                return parameterName;
            }

            return parameterName.Substring(0, 1).ToUpper() +
                   parameterName.Substring(1, parameterName.Length - 1);
        }

        public static string FixMemberName(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            str = str.Trim('\"');
            var final = string.Empty;
            for (var index = 0; index < str.Length; index++)
            {
                var ch = str[index];
                
                final += ch;
                for (var indexNext = index + 1; indexNext < str.Length; indexNext++)
                {
                    var nextch = str[indexNext];
                    if (char.IsUpper(ch) && char.IsUpper(nextch))
                    {
                        final += nextch.ToString().ToLower();
                        index++;
                    }
                    else
                    {
                        index = indexNext - 1;
                        break;
                    }
                }
            }

            final = final.Replace(UnderScore, string.Empty);
            if (str.StartsWith(UnderScore))
            {
                final = $"{final}Field";
            }
            return final.Replace(OldIdValue, NewIdValue)
                .Replace(OldUrlValue, NewUrlValue)
                .Replace(Dollar, NewDollar);
        }
    }
}
