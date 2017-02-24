using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

// Consider changing read to where the dice number is ignored if hex is not used
// Consider changing default resource from -1 to a 5-9 and reading one character at a time
// Figure out a way to use relative pathing
public class FileHandler
{
    const int WIDTH = HexTemplate.WIDTH;
    const int HEIGHT = HexTemplate.HEIGHT;

    const int EOF = -1;

    public const string DefaultMapsPath = "Assets/DefaultMaps";
    public const string SavedMapsPath = "Assets/SavedMaps";

    // Consider Need for mapList file in default maps
    // ALSO consider that having the same name for a file in both folders could cause the wrong result
    public List<string> getAllMaps(out int savedMapsStartIndex)
    {
        List<string> mapNames = new List<string>();
        string tempString;

        // Figure out how to make sure the maplist File Exists
        //if (File.Exists(DefaultMapsPath + "/MapList.txt") == false)
        //  File.Create(DefaultMapsPath + "/MapList.txt");

        // May need to add a try catch in case there are not any files
        using (StreamReader reader = File.OpenText(DefaultMapsPath + "/MapList.txt"))
        {
            while ((tempString = reader.ReadLine()) != null)
                mapNames.Add(tempString);
            reader.Close();
        }

        savedMapsStartIndex = mapNames.Count;

        // May need to add a try catch in case there are not any files
        using (StreamReader reader = File.OpenText(SavedMapsPath + "/MapList.txt"))
        {
            while ((tempString = reader.ReadLine()) != null)
                mapNames.Add(tempString);
            reader.Close();
        }
        return mapNames;
    }

    public HexTemplate retrieveMap(string mapName)
    {
        HexTemplate template = new HexTemplate();
        int readChar = 0;
        string current = null;
        string next = null;
        int result;
        int resource = -1;
        int diceNum = -1;
        bool[] portSides = { false, false, false, false, false, false };

        // Modify to do something with the name and creator
        using (StreamReader reader = File.OpenText(mapName))
        {
            List<string> numbers = new List<string>();

            // Skip over name and creator
            string bob = reader.ReadLine();
            Debug.Log(bob);
            bob = reader.ReadLine();

            for (int x = 0; x < (WIDTH + 1); x++)
            {
                for (int z = 0; z < HEIGHT; z++)
                {
                    for (int index = 0; index < portSides.Length; index++)
                    {
                        portSides[index] = false;
                    }

                    // Loop passed additional spaces
                    while ((readChar = reader.Read()) != EOF &&
                           (current = char.ConvertFromUtf32(readChar)) == " " ||
                            current == "\r" || current == "\n") ;

                    // Read until hexagon data separtor is found
                    while ((readChar = reader.Read()) != EOF &&
                           (next = char.ConvertFromUtf32(readChar)) != "|")
                    {
                        current = String.Concat(current, next);
                    }

                    numbers.Clear();
                    numbers.AddRange(current.Split());

                    if (numbers.Count >= 0 && Int32.TryParse(numbers[0], out result) &&
                        result >= -1 && result <= 5)
                        resource = result;
                    else
                    {
                        printDataError();
                        z = HEIGHT;
                        x = WIDTH;
                    }

                    if (numbers.Count >= 1 && Int32.TryParse(numbers[1], out result) &&
                        result >= 2 && result <= 12)
                        diceNum = result;
                    else
                    {
                        printDataError();
                        z = HEIGHT;
                        x = WIDTH;
                    }

                    for (int index = 2; index < numbers.Count; index++)
                    {
                        if (Int32.TryParse(numbers[index], out result) &&
                            result >= 0 && result <= 5)
                        {
                            portSides[result] = true;
                        }
                    }

                    if (x < WIDTH && z < HEIGHT)
                        template.hex[x, z] = new Hex(resource, diceNum, portSides);
                }
            }
            reader.Close();
        }
        return template;
    }


    // Add check for existence of file before overwritting
    // Add name parameter or check before calling this function
    public void saveMap(HexTemplate template, string name, string creator)
    {
        List<string> mapNames = new List<string>();

        using (StreamWriter writer = File.CreateText(SavedMapsPath + "/" + name + ".txt"))
        {
            writer.WriteLine(name);
            writer.WriteLine(creator);
            for (int x = 0; x < WIDTH; x++)
            {
                for (int z = 0; z < HEIGHT; z++)
                {
                    writer.Write(template.hex[x, z].resource);
                    writer.Write(" " + template.hex[x, z].dice_number);

                    for (int index = 0; index < template.hex[x, z].portSides.Length; index++)
                    {
                        if (template.hex[x, z].portSides[index] == true)
                            writer.Write(" " + index);
                    }
                    writer.Write(" | ");
                }
                if (x != (WIDTH - 1))
                    writer.WriteLine();
            }
            writer.Close();
        }

        // May need to add a try catch in case there are not any files
        using (StreamReader reader = File.OpenText(SavedMapsPath + "/MapList.txt"))
        {
            string tempString;

            while ((tempString = reader.ReadLine()) != null)
                mapNames.Add(tempString);
            reader.Close();
        }

        using (StreamWriter writer = File.CreateText(SavedMapsPath + "/MapList.txt"))
        {
            writer.WriteLine(name);
            foreach (string mapName in mapNames)
            {
                writer.WriteLine(mapName);
            }
            writer.Close();
        }

    }

    // Print error message if a file contained insufficient data
    public void printEOFerror()
    {
        Debug.Log("End of map file reached prematurely:");
    }

    // Print error message if data was not in sequence as expected
    public void printDataError()
    {
        Debug.Log("Inproper data sequence when reading the file:");
    }
}