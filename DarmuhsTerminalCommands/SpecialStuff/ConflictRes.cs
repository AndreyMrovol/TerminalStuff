using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TerminalStuff.SpecialStuff
{
    internal class ConflictRes
    {
        internal static void InitRes(ref TerminalKeyword word)
        {
            if (word == null || !ConfigSettings.TerminalConflictResolution.Value)
                return;

            if(TryGetBestMatchingKeyword(TerminalEvents.GetCleanedScreenText(Plugin.instance.Terminal), out TerminalKeyword returnWord))
            {
                word = returnWord;
                Plugin.Spam("returning conflictresolution word");
            }
        }

        internal static bool TryGetBestMatchingKeyword(string input, out TerminalKeyword returnWord)
        {
            Dictionary<TerminalKeyword, int> matching = [];
            foreach (TerminalKeyword word in Plugin.instance.Terminal.terminalNodes.allKeywords)
            {
                if (word.word.ToLower().Contains(input) && !matching.ContainsKey(word))
                {
                    string wordNoSpace = Regex.Replace(word.word, @"\s", "");
                    int score = Levenshtein.Distance(wordNoSpace, input);
                    Plugin.Spam($"Word {word.word} noted with score {score} for input {input}");
                    matching.Add(word, score);
                    Plugin.Spam($"adding matching word: {word.word} to conlfict resolution list");
                }
            }

            int pairScore = 100;
            TerminalKeyword bestKeyword = null;
            foreach(KeyValuePair<TerminalKeyword, int> pair in matching)
            {
                if(pair.Value < pairScore)
                {
                    Plugin.Spam($"New best score found, setting pairScore to {pair.Value} from keyword {pair.Key.word}");
                    pairScore = pair.Value;
                    bestKeyword = pair.Key;
                }
            }

            if (bestKeyword == null)
            {
                Plugin.Log.LogWarning($"No matching keywords found for input: {input}");
                returnWord = null;
                return false;
            }
            else
            {
                Plugin.Spam($"Conflict resolution has determined word: {bestKeyword.word} is the best match with score: {pairScore}");
                returnWord = bestKeyword;
                return true;
            }
                
        }
    }
}
