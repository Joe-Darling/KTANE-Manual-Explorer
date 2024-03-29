﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;

namespace Manual_Explorer
{
    public class SearchFunctionality
    {
        private void TabAction(ComboBox comboBox, KeyEventArgs e, Selector targetFocus)
        {
            comboBox.IsDropDownOpen = false;
            e.Handled = true;
            targetFocus.Focus();
        }

        private void InvalidCharAction(KeyEventArgs e)
        {
            e.Handled = true;
            Trace.WriteLine("A letter was NOT pressed");
        }

        private void EnterAction(ComboBox comboBox, KeyEventArgs e)
        {
            e.Handled = true;
            comboBox.IsDropDownOpen = false;
        }

        private void EmptyComboBoxCheck(ComboBox comboBox)
        {
            if (string.IsNullOrEmpty(comboBox.Text))
            {
                comboBox.IsDropDownOpen = false;
            }
            else
            {
                comboBox.IsDropDownOpen = true;
            }
        }

        public void SearchFilter(ComboBox comboBox, string userInput, List<string> keys)
        {
            List<string> contains = new List<string>();
            List<string> wordStarts = new List<string>();
            foreach (string nameToCheck in keys)
            {
                // first adding modules whose first word starts with user input
                if (nameToCheck.StartsWith(userInput))
                {
                    comboBox.Items.Add(CapitilizeItem(nameToCheck));
                }
                // adding modules (to the list) where any other word starts with user input 
                else if (nameToCheck.Contains(" "))
                {
                    string[] words = nameToCheck.Split(' ');
                    for (int i = 1; i < words.Length; i++)
                    {
                        if (words[i].StartsWith(userInput))
                        {
                            wordStarts.Add(nameToCheck);
                        }
                    }
                }
                // adding modules (to the list) that just contain user input 
                else if (nameToCheck.Contains(userInput))
                {
                    contains.Add(nameToCheck);
                }
            }


            foreach (string moduleName in wordStarts)
            {
                comboBox.Items.Add(CapitilizeItem(moduleName));
            }

            foreach (string moduleName in contains)
            {
                comboBox.Items.Add(CapitilizeItem(moduleName));
            }

            // adding modules which name might have been misspelled
            foreach (string item in keys)
            {
                if (Compute(item, userInput) <= 3 && item.Length > 0)
                {
                    comboBox.Items.Add(CapitilizeItem(item));
                }

            }
        }

        public void UpdateComboBox(ComboBox comboBox, KeyEventArgs e, Selector targetFocus, List<string> keys)
        {
            comboBox.Items.Clear();

            if ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                string userInput = comboBox.Text.ToLower() + e.Key.ToString().ToLower();
                SearchFilter(comboBox, userInput, keys);
            }
            else if (e.Key == Key.LeftShift || e.Key == Key.Space || e.Key == Key.RightShift || e.Key == Key.OemMinus || e.Key == Key.OemQuotes)
            {
            }
            else if (e.Key == Key.Tab)
            {
                TabAction(comboBox, e, targetFocus);
            }
            else
            {
                InvalidCharAction(e);
            }

            if (e.Key == Key.Enter)
            {
                EnterAction(comboBox, e);
            }
            else
            {
                comboBox.IsDropDownOpen = true;
            }
        }

        public void UpdateComboBoxOnBackspace(ComboBox comboBox, KeyEventArgs e, Selector targetFocus, List<string> keys)
        {
            if (e.Key == Key.Back)
            {
                comboBox.Items.Clear();
                string userInput = comboBox.Text.ToLower();
                SearchFilter(comboBox, userInput, keys);
            }
            else if (e.Key == Key.Tab)
            {
                TabAction(comboBox, e, targetFocus);
            }
            else if (e.Key == Key.Enter)
            {
                EnterAction(comboBox, e);
            }

            EmptyComboBoxCheck(comboBox);
        }

        private string CapitilizeItem(string item)
        {
            return char.ToUpper(item[0]) + item.Substring(1);
        }

        private static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }


    }   
}
