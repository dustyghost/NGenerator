using Humanizer;
using NGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NGenerator.Processors
{
    public class NGenerratorProcessor
    {
        private const string TagPrefix = "{{";
        private const string TagSuffix = "}}";
        private const string ModSplit = ".";
        private const string SubDefSplit = "::";

        private const string TagPattern = TagPrefix + "[^}]+" + TagSuffix;

        private readonly InputHolder _inputHolder = new InputHolder();
        private readonly Random _rnd;
        private readonly List<ReservedSubstition> Reserved = new List<ReservedSubstition>();

        public NGenerratorProcessor(Random rnd)
        {
            _rnd = rnd;
        }

        public string Process(string input)
        {
            _inputHolder.SetFromJson(input);
            var output = _inputHolder.Template;

            output = Substitutions(output);

            return output;
        }

        private string Substitutions(string working)
        {
            foreach (var item in Regex.Matches(working, TagPattern))
            {
                List<string> tagWithMods = item
                    .ToString()
                    .Replace(TagPrefix, "")
                    .Replace(TagSuffix, "")
                    .Trim()
                    .Split('.')
                    .ToList();

                if (_inputHolder.Tags.ContainsKey(tagWithMods[0]))
                {
                    List<string> tagWithModsSorted = new List<string> { tagWithMods[0] };

                    //Get numbers before other modifiers
                    tagWithModsSorted.AddRange(tagWithMods.Skip(1).OrderBy(t => t));

                    //Need to move "a" mod to the end so it's the last thing we do
                    var aOrAnModifier = tagWithModsSorted.FirstOrDefault(_ => _ == "a");
                    if (aOrAnModifier != default)
                    {
                        //Move to the end
                        tagWithModsSorted.Remove(aOrAnModifier);
                        tagWithModsSorted.Add("a");
                    }

                    //Get the available substitutions based on the tag
                    _inputHolder.Tags.TryGetValue(tagWithModsSorted[0], out string[] substitutions);

                    var substitution = "";

                    //If using reserve mod, get previously generated, if already defined
                    var reserveMod = tagWithModsSorted.Skip(1).ToList().Find(a => a.All(char.IsDigit));
                    if (reserveMod != default)
                    {
                        var reserved = Reserved.Find(r => r.Key == tagWithMods[0] && r.ReserveNumber == int.Parse(reserveMod));
                        if (reserved != default)
                        {
                            substitution = reserved.Substitution;
                        }
                        else
                        {
                            //try to get a different sub for each reserve mod
                            var availableSubs = substitutions.Where(s => !Reserved.Where(r => r.Key == tagWithMods[0]).Any(a => a.Key == s.Split(':')[0])).ToArray();

                            //If there are subs that have not been reserved yet select from them, else just pick from a random one.
                            substitution = availableSubs.Length > 0 ? availableSubs[_rnd.Next(availableSubs.Length)] : substitutions[_rnd.Next(substitutions.Length)];

                            Reserved.Add(
                                new ReservedSubstition
                                {
                                    Key = tagWithModsSorted[0],
                                    ReserveNumber = int.Parse(reserveMod),
                                    Substitution = substitution
                                });
                        }
                    }
                    else
                    {
                        //Get random choice from available substitutions
                        substitution = substitutions[_rnd.Next(substitutions.Length)];
                    }

                    //Split by : to get the syntax definitions split from the word.
                    string[] subDef = substitution.Split(SubDefSplit);
                    var newString = subDef[0];

                    //If new string contains tag characters recursivly call Subsitutions
                    if (newString.Contains(TagPrefix) && newString.Contains(TagSuffix))
                    {
                        newString = Substitutions(newString);
                    }

                    for (var i = 1; i < tagWithModsSorted.Count; i++)
                    {
                        switch (tagWithModsSorted[i])
                        {
                            case "a":
                                newString = PrefixAnOrA(subDef, newString);
                                break;

                            case "p":
                                newString = Pluralize(subDef, newString);
                                break;

                            case "u":
                                newString = newString.Substring(0, 1).ToUpper() + newString[1..];
                                break;

                            case "U":
                                newString = newString.ToUpper();
                                break;

                            case "l":
                                newString = newString.ToLower();
                                break;

                            default:
                                break;
                        }
                    }
                    working = working.Replace(item.ToString(), newString);
                }
            }

            return working;
        }

        private string Pluralize(string[] subDef, string newString)
        {
            foreach (var item in subDef)
            {
                var defSplit = item.Split(ModSplit);
                if (defSplit[0] == "p" && defSplit.Length > 1)
                    return defSplit[1];
            }
            return newString.Pluralize();
        }

        private static string PrefixAnOrA(string[] subDef, string newString)
        {
            foreach (var item in subDef)
            {
                var defSplit = item.Split(ModSplit);
                if (defSplit[0] == "a" && defSplit.Length > 1)
                    return $"{defSplit[1]} {newString}";
            }

            return $"{("aeiou".Contains(newString.Substring(0, 1).ToLower()) ? "an" : "a")} {newString}";
        }
    }
}