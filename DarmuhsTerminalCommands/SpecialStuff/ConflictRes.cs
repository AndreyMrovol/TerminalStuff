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
        TerminalKeyword word;
        int distance;
        int bonus;

        internal ConflictRes(TerminalKeyword word, int distance, int bonus)
        {
            this.word = word;
            this.distance = distance;
            this.bonus = bonus;
        }

        internal static bool ContainsWord(List<ConflictRes> conflictList, TerminalKeyword query)
        {
            foreach (ConflictRes word in conflictList)
            {
                if (word.word == query)
                    return true;
            }
            return false;

        }

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

        internal static void QueryKeywords(ref List<ConflictRes> matching, string input)
        {
            foreach (TerminalKeyword word in Plugin.instance.Terminal.terminalNodes.allKeywords)
            {
                if ((word.word.ToLower().Contains(input) && !ContainsWord(matching, word)))
                {
                    if (word.word.Length < 2)
                        continue;
                    int bonus = Levenshtein.MatchingStart(word.word, input);
                    int score = Levenshtein.Distance(word.word, input);
                    
                    Plugin.Spam($"Word {word.word} noted with score {score} for input {input} with bonus {bonus}");
                    matching.Add(new(word,score,bonus));
                    //Plugin.Spam($"adding matching word: {word.word} to conlfict resolution list");
                }
                else if(word.compatibleNouns != null)
                {
                    foreach(CompatibleNoun noun in word.compatibleNouns)
                    {
                        if (input.Contains(noun.noun.word) && !ContainsWord(matching, noun.noun))
                        {
                            if (noun.noun.word.Length < 3)
                                continue;
                            Plugin.Spam($"{word.word} has compatible noun: {noun.noun.word} which is in {input}");
                            int bonus = Levenshtein.MatchingStart(noun.noun.word, input);
                            int score = Levenshtein.Distance(noun.noun.word, input);
                            Plugin.Spam($"score: {score}");
                            matching.Add(new(noun.noun, score, bonus));
                        }
                    }
                }
            }
        }

        internal static bool TryGetBestMatchingKeyword(string query, out TerminalKeyword returnWord)
        {
            //Dictionary<TerminalKeyword, int> matching = [];
            List<ConflictRes> resolutionList = [];

            QueryKeywords(ref resolutionList, query);

            if (resolutionList.Count < 1)
            {
                Plugin.Log.LogWarning($"No matching keywords could be found for input: {query}");
                returnWord = null;
                return false;
            }

            int highestBonus = 0;
            ConflictRes bestMatch = null;
            foreach(ConflictRes resolution in resolutionList)
            {
                if(resolution.bonus > highestBonus)
                {
                    bestMatch = resolution;
                    highestBonus = resolution.bonus;
                    Plugin.Spam($"highestBonus updated to {highestBonus} - from {bestMatch.word}");
                }
                else if(resolution.bonus == highestBonus && bestMatch != null)
                {
                    Plugin.Spam($"matching highestBonus found for word {resolution.word.word}");
                    if (resolution.distance < bestMatch.distance)
                    {
                        Plugin.Spam($"distance for {resolution.word.word} is lower than {bestMatch.word.word}");
                        Plugin.Spam($"setting bestMatch to {resolution.word.word}");
                        bestMatch = resolution;
                    }
                }
            }

            if (bestMatch.word == null)
            {
                Plugin.Log.LogWarning($"No matching keywords found for input: {query}");
                returnWord = null;
                return false;
            }
            else
            {
                Plugin.Spam($"Conflict resolution has determined word: {bestMatch.word.word} is the best match with distance: {bestMatch.distance} and starswith bonus: {bestMatch.bonus}");
                returnWord = bestMatch.word;
                return true;
            }
                
        }
    }
}
