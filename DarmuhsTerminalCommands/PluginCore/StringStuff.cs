using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerminalStuff
{
    internal class StringStuff
    {

        internal static string[] GetWords()
        {
            string cleanedText = Plugin.instance.Terminal.screenText.text.Substring(Plugin.instance.Terminal.screenText.text.Length - Plugin.instance.Terminal.textAdded);
            string[] words = cleanedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return words;
        }

        internal static string[] GetWordsAndKeyword(List<string> configItemWords, string[] words)
        {
            List<string> filteredWords = [];
            bool keywordFound = false;

            foreach (string word in words)
            {
                Plugin.MoreLogs($"checking {word}");
                foreach (string keyword in configItemWords)
                {
                    if (keyword.Contains(word))
                    {
                        filteredWords.Add(keyword);
                        keywordFound = true;
                        Plugin.MoreLogs($"adding {keyword} to list");
                        break;
                    }
                }

                if (!keywordFound)
                {
                    filteredWords.Add(word);
                    Plugin.MoreLogs($"adding non-keyword, word: {word}");
                }

            }

            return [.. filteredWords];
        }

        internal static List<string> GetKeywordsPerConfigItem(string configItem)
        {
            List<string> keywordsInConfig = [];
            if (configItem.Length > 0)
            {
                keywordsInConfig = configItem.Split(';')
                                      .Select(item => item.TrimStart())
                                      .ToList();
                //Plugin.MoreLogs("GetKeywordsPerConfigItem split complete");
            }
                
            return keywordsInConfig;
        }

        internal static List<int> GetNumberListFromStringList(List<string> stringList)
        {
            List<int> numbersList = [];
            foreach (string item in stringList)
            {
                if (int.TryParse(item, out int number))
                {
                    numbersList.Add(number);
                }
                else
                    Plugin.WARNING($"Could not parse {item} to integer");
            }

            return numbersList;
        }

        internal static List<string> GetItemList(string rawList)
        {
            List<string> itemList = [];
            if (rawList.Length > 0)
            {
                itemList = rawList.Split(',')
                                      .Select(item => item.TrimStart())
                                      .ToList();
            }
            
            return itemList;
        }

        internal static Dictionary<string,string> GetKeywordAndItemNames(string configItem)
        {
            Dictionary<string, string> KeywordAndNames = [];
            if (configItem.Length > 0)
            {
                 KeywordAndNames = configItem
                    .Split(';')
                    .Select(item => item.Trim())
                    .Select(item => item.Split(':'))
                    .ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim());
            }
             
            return KeywordAndNames;
        }

        internal static List<string> GetListToLower(List<string> stringList)
        {
            List<string> itemsToLower = [];

            foreach(string item in stringList)
            {
                itemsToLower.Add(item.ToLower());
            }
            return itemsToLower;
        }

    }
}
