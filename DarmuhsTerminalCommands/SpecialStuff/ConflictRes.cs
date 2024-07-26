using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.Windows;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

namespace TerminalStuff.SpecialStuff
{
    internal class ConflictRes
    {
        internal static void InitRes(string playerWord, ref TerminalKeyword word)
        {
            if (word == null || !ConfigSettings.TerminalConflictResolution.Value)
                return;

            if (playerWord.Length < 0)
                return;

            if (TryGetBestMatchingKeyword(playerWord, out TerminalKeyword returnWord))
            {
                word = returnWord;
                Plugin.Spam("returning conflictresolution word");
            }
        }

        internal static void QueryKeywords(ref Dictionary<TerminalKeyword, int> matching, string input)
        {
            foreach (TerminalKeyword word in Plugin.instance.Terminal.terminalNodes.allKeywords)
            {
                if ((word.word.ToLower().Contains(input) && !matching.ContainsKey(word)))
                {
                    if (word.word.Length < 3)
                        continue;
                    int score = Levenshtein.Distance(word.word, input);
                    Plugin.Spam($"Word {word.word} noted with score {score} for input {input}");
                    matching.Add(word, score);
                    //Plugin.Spam($"adding matching word: {word.word} to conlfict resolution list");
                }
                else if(word.compatibleNouns != null)
                {
                    foreach(CompatibleNoun noun in word.compatibleNouns)
                    {
                        if (input.Contains(noun.noun.word) && !matching.ContainsKey(noun.noun))
                        {
                            if (noun.noun.word.Length < 3)
                                continue;
                            Plugin.Spam($"{word.word} has compatible noun: {noun.noun.word} which is in {input}");
                            int score = Levenshtein.Distance(noun.noun.word, input);
                            Plugin.Spam($"score: {score}");
                            matching.Add(noun.noun, score);
                        }
                    }
                }
            }
        }

        internal static bool TryGetBestMatchingKeyword(string query, out TerminalKeyword returnWord)
        {
            Dictionary<TerminalKeyword, int> matching = [];

            QueryKeywords(ref matching, query);

            if (matching.Count < 1)
            {
                Plugin.Log.LogWarning($"No matching keywords could be found for input: {query}");
                returnWord = null;
                return false;
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
                Plugin.Log.LogWarning($"No matching keywords found for input: {query}");
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
