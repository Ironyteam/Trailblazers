using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

public class NameChecker
{
    private const int MAX_NAME_LENGTH = 30;

    // Checks a name to see if it has any inappropriate words within it
    public bool passesFilter(string name)
    {
        bool passesFilter = true;
        string nextFilterWord;
        string[] words = name.Split(' ');

        for (int index = 0; index < words.Length; index++)
        {
            words[index] = words[index].ToLower();
        }

        TextAsset map = Resources.Load("filter") as TextAsset;
        byte[] byteArray = Encoding.UTF8.GetBytes(map.text);
        MemoryStream stream = new MemoryStream(byteArray);


        using (StreamReader reader = new StreamReader(stream))
        {
            while (passesFilter && (nextFilterWord = reader.ReadLine()) != null)
            {
                for (int index = 0; index < words.Length; index++)
                {
                    if (String.Compare(words[index], nextFilterWord) == 0)
                    {
                        passesFilter = false;
                        index = words.Length;
                    }
                }
            }
            reader.Close();
        }
        return passesFilter;
    }

    // Modifies a name and then checks that modified name
    // to see if it is valid.
    // Parmeters:
    // Unmodified name to be checked
    // Empty string that will be used for the name after it is modified
    public bool passesFormatCheck(string name, out string modifiedName)
    {
        bool isValid = false;

        Debug.Log(name.Length);
        // Trim tabs and extra spaces
        modifiedName = String.Join(" ", name.Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries));

        Debug.Log(modifiedName.Length);
        if (modifiedName.Length <= MAX_NAME_LENGTH && Regex.IsMatch(modifiedName, "^[A-Za-z ]+$"))
            isValid = true;
        
        return isValid;
    }
}
