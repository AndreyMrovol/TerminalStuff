﻿using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System;
using BepInEx;

namespace TerminalStuff.SpecialStuff
{
    internal static class FontStuff
    {
        internal static void TestingFonts()
        {
            Plugin.Spam("Debug Font Stuff");
            string[] fontNames = Font.GetOSInstalledFontNames();

            foreach (string fontName in fontNames)
            {
                Plugin.Spam($"Font: {fontName} detected!");
            }
            // Get paths to OS Fonts
            string[] fontPaths = Font.GetPathsToOSFonts();

            // Create new font object from one of those paths
            List<Font> osFonts = new List<Font>();
            foreach (string fontPath in fontPaths)
            {
                Plugin.Spam($"FontPath: {fontPath} detected!");
                //Font newFont = new Font(fontPath);
                // Create new dynamic font asset
                //TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(newFont);
            }
        }

        internal static bool TryGetCustomOSFont(string fontName, out TMP_FontAsset CustomFontAsset)
        {
            // Get paths to OS Fonts
            string[] fontPaths = Font.GetPathsToOSFonts();

            // Create new font object from one of those paths
            List<Font> osFonts = new List<Font>();
            foreach (string fontPath in fontPaths)
            {
                if (fontPath.Contains(fontName))
                {
                    Plugin.Spam($"FontPath: {fontPath} detected!");
                    Font newFont = new Font(fontPath);
                    // Create new dynamic font asset
                    CustomFontAsset = TMP_FontAsset.CreateFontAsset(newFont);
                    return true;
                }
            }
            CustomFontAsset = null;
            return false;
        }

        internal static void GetAndSetFont()
        {
            if (ConfigSettings.CustomFontName.Value.Length < 1)
                return;

            if (TryGetCustomOSFont(ConfigSettings.CustomFontName.Value, out TMP_FontAsset osFont))
            {
                Plugin.Spam($"{ConfigSettings.CustomFontName.Value} found in system fonts!");
                SetTerminalFont(osFont);
                return;
            }
            if(TryGetCustomFont(ConfigSettings.CustomFontName.Value, out TMP_FontAsset newFont))
            {
                Plugin.Spam($"{ConfigSettings.CustomFontName.Value} found in windows custom fonts path");
                SetTerminalFont(newFont);
                return;
            }
            if(TryGetFontFromCustomPath(ConfigSettings.CustomFontName.Value, out TMP_FontAsset customFont))
            {
                Plugin.Spam($"{ConfigSettings.CustomFontName.Value} found in custom fonts path - {ConfigSettings.CustomFontPath.Value}");
                SetTerminalFont(customFont);
                return;
            }
            Plugin.Spam($"Unable to find {ConfigSettings.CustomFontName.Value} in system fonts, in windows fonts path, or custom fonts path {ConfigSettings.CustomFontPath.Value}");
        }

        internal static bool TryGetFontFromCustomPath(string fontName, out TMP_FontAsset CustomFontAsset)
        {
            if (ConfigSettings.CustomFontPath.Value.Length < 1)
            {
                CustomFontAsset = null;
                return false;
            }
                
            //Paths.BepInExRootPath
            string path = Path.Combine(Paths.BepInExRootPath, ConfigSettings.CustomFontPath.Value);
            if (Directory.Exists(path))
            {
                string fullPath = Path.Combine(path, fontName);
                if (File.Exists(fullPath))
                {
                    Font customFont = new(fullPath);
                    Plugin.Spam($"attempting to create custom font from fontname: {fontName}");
                    if (customFont != null)
                    {
                        int fontFaceResult = (int)UnityEngine.TextCore.LowLevel.FontEngine.LoadFontFace(customFont);
                        Plugin.Spam(fontName + ": " + fontFaceResult);
                        CustomFontAsset = TMP_FontAsset.CreateFontAsset(customFont);
                        return true;
                    }
                    Plugin.Spam("customFont is null returning false");
                    CustomFontAsset = null;
                    return false;
                }
                Plugin.Spam("Font File could not be found");
                CustomFontAsset = null;
                return false;
            }
            Plugin.Spam("Custom Font Directory could not be found");
            CustomFontAsset = null;
            return false;
        }

        internal static bool TryGetCustomFont(string fontName, out TMP_FontAsset CustomFontAsset)
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Plugin.Spam(localAppData);
            string fullPath = Path.Combine(localAppData, "Microsoft\\Windows\\Fonts\\");
            Plugin.Spam(fullPath);
            Plugin.Spam(fullPath + fontName);
            if (Directory.Exists(fullPath) && File.Exists(Path.Combine(fullPath + fontName)))
            {
                Font customFont = new(Path.Combine(fullPath + fontName));
                Plugin.Spam($"attempting to create custom font from fontname: {fontName}");
                if (customFont != null)
                {
                    int fontFaceResult = (int)UnityEngine.TextCore.LowLevel.FontEngine.LoadFontFace(customFont);
                    Plugin.Spam(fontName + ": " + fontFaceResult);
                    CustomFontAsset = TMP_FontAsset.CreateFontAsset(customFont);
                    return true;
                }
                Plugin.Spam("customFont is null returning false");
                CustomFontAsset = null;
                return false;
            }
            Plugin.Spam("Directory & file could not be found");
            CustomFontAsset = null;
            return false;

        }

        internal static void SetTerminalFont(TMP_FontAsset newFont)
        {
            Plugin.Spam("SetTerminalFont called to switch font");
            Plugin.instance.Terminal.screenText.fontAsset = newFont;
            Plugin.instance.Terminal.topRightText.font = newFont;
            if(TerminalClockStuff.textComponent != null)
                TerminalClockStuff.textComponent.font = newFont;

            if(ConfigSettings.CustomFontSizeMain.Value > -1)
                Plugin.instance.Terminal.screenText.textComponent.fontSize = ConfigSettings.CustomFontSizeMain.Value;

            if (ConfigSettings.CustomFontSizeMoney.Value > -1)
                Plugin.instance.Terminal.topRightText.fontSize = ConfigSettings.CustomFontSizeMoney.Value;
                
            if (TerminalClockStuff.textComponent != null && ConfigSettings.CustomFontSizeClock.Value > -1)
                TerminalClockStuff.textComponent.fontSize = ConfigSettings.CustomFontSizeClock.Value;
        }
    }
}
