﻿using UnityEngine;
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

    const int SCREEN_SHOT_WIDTH = BoardManager.SCREEN_SHOT_WIDTH;
    const int SCREEN_SHOT_LENGTH = BoardManager.SCREEN_SHOT_LENGTH;

    const int EOF = -1;

    public const string DefaultMapsPath = "Assets/DefaultMaps";
    public const string SavedMapsPath = "Assets/SavedMaps";


    public List<Sprite> getScreenShots(List<string> mapNames, int savedMapsStartIndex)
    {
        List<Sprite> screenShots = new List<Sprite>();
        bool screenShotFound;

        for (int index = 0; index < mapNames.Count; index++)
        {
            byte[] bytes = null;

            if (index < savedMapsStartIndex)
            {
                try
                {
                    bytes = System.IO.File.ReadAllBytes(DefaultMapsPath + "/" + mapNames[index] + ".png");
                    screenShotFound = true;
                }
                catch (Exception e)
                {
                    screenShotFound = false;
                }
            }
            else
                try
                {
                    bytes = System.IO.File.ReadAllBytes(SavedMapsPath + "/" + mapNames[index] + ".png");
                    screenShotFound = true;
                }
                catch (Exception e)
                {
                    screenShotFound = false;
                }

            if (screenShotFound)
            {
                Texture2D texture = new Texture2D(SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH);
                texture.filterMode = FilterMode.Trilinear;
                texture.LoadImage(bytes);
                screenShots.Add(Sprite.Create(texture, new Rect(0, 0, SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH), new Vector2(0.5f, 0.0f), 1.0f));
            }
            else
                screenShots.Add(null);
        }
        return screenShots;
    }

    // Consider Need for mapList file in default maps
    // ALSO consider that having the same name for a file in both folders could cause the wrong result
    public List<HexTemplate> getAllMaps(out int savedMapsStartIndex)
    {
        List<HexTemplate> mapNames = new List<HexTemplate>();
        string tempString;
        string[] hexInfo;

        // Figure out how to make sure the maplist File Exists
        //if (File.Exists(DefaultMapsPath + "/MapList.txt") == false)
        //  File.Create(DefaultMapsPath + "/MapList.txt");

        // May need to add a try catch in case there are not any files
        using (StreamReader reader = File.OpenText(DefaultMapsPath + "/MapList.txt"))
        {
            while ((tempString = reader.ReadLine()) != null)
            {
                hexInfo = tempString.Split();
                if (hexInfo.Length == 3)
                    mapNames.Add(new HexTemplate(hexInfo[0], Int32.Parse(hexInfo[1]), Int32.Parse(hexInfo[2])));
                else
                    printMapListError();
            }
            reader.Close();
        }

        savedMapsStartIndex = mapNames.Count;

        // May need to add a try catch in case there are not any files
        using (StreamReader reader = File.OpenText(SavedMapsPath + "/MapList.txt"))
        {
            while ((tempString = reader.ReadLine()) != null)
            {
                hexInfo = tempString.Split();
                if (hexInfo.Length == 3)
                    mapNames.Add(new HexTemplate(hexInfo[0], Int32.Parse(hexInfo[1]), Int32.Parse(hexInfo[2])));
                else
                    printMapListError();
            }
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
        int result = -1;
        int resource = -1;
        int diceNum = -1;
        

        // Modify to do something with the name and creator
        using (StreamReader reader = File.OpenText(mapName))
        {
            List<string> numbers = new List<string>();

            // Skip over name and creator
            reader.ReadLine();
            reader.ReadLine();

            template.minVP = Int32.Parse(reader.ReadLine());
            template.maxVP = Int32.Parse(reader.ReadLine());

            for (int x = 0; x < WIDTH; x++)
            {
                for (int z = 0; z < HEIGHT; z++)
                {
                    int portSide = -1;

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
                        result >= -1 && result <= 12)
                        diceNum = result;
                    else
                    {
                        printDataError();
                        z = HEIGHT;
                        x = WIDTH;
                    }

                    if (numbers.Count >= 1 && Int32.TryParse(numbers[2], out result) &&
                        result >= -1 && result <= 5)
                    {
                        portSide = result;
                    }
                    else
                    {
                        printDataError();
                        z = HEIGHT;
                        x = WIDTH;
                    }

                    if (x < WIDTH && z < HEIGHT)
                        template.hex[x, z] = new Hex(resource, diceNum, result, x, z);
                }
            }
            reader.Close();
        }
        return template;
    }

    // Read a map as one string to send across the network
    // Map name should enclude entire path name and extension
    public string ReadEntireMap(string mapName)
    {
        string fileString;

        // Modify to do something with the name and creator
        using (StreamReader reader = File.OpenText(mapName))
        {
            fileString = reader.ReadToEnd();
            reader.Close();
        }
        return fileString;
    }

    // Add check for existence of file before overwritting
    // Add name parameter or check before calling this function
    public void saveMap(HexTemplate template, string name, string creator, int minVP, int maxVP)
    {
        List<string> mapNames = new List<string>();

        using (StreamWriter writer = File.CreateText(SavedMapsPath + "/" + name + ".txt"))
        {
            writer.WriteLine(name);
            writer.WriteLine(creator);
            writer.WriteLine(minVP);
            writer.WriteLine(maxVP);
            for (int x = 0; x < WIDTH; x++)
            {
                for (int z = 0; z < HEIGHT; z++)
                {
                    writer.Write(template.hex[x, z].resource);
                    writer.Write(" " + template.hex[x, z].dice_number);
                    writer.Write(" " + template.hex[x, z].portSide);

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
            writer.WriteLine(name + " " + minVP + " " + maxVP);
            foreach (string mapName in mapNames)
            {
                writer.WriteLine(mapName);
            }
            writer.Close();
        }

    }

    // Save a board by writing the entire string of another file
    // all at once
    public void saveMap(string rawFileString)
    {
        List<string> mapNames = new List<string>();
        string[] fileLines;
        string modifiedFileString;

        modifiedFileString = rawFileString.Replace("\r\n", "\n");
        fileLines = modifiedFileString.Split('\n');

        Debug.Log(fileLines[0]);
        using (StreamWriter writer = File.CreateText(SavedMapsPath + "/" + fileLines[0] + ".txt"))
        {
            writer.Write(rawFileString);
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

        // Separate the lines of the file string and use the appropriate lines to
        // write the minimum and maximum victory points
        using (StreamWriter writer = File.CreateText(SavedMapsPath + "/MapList.txt"))
        {
            writer.WriteLine(fileLines[0] + " " + fileLines[2] + " " + fileLines[3]);
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

    public void printMapListError()
    {
        Debug.Log("Unable to find expected number of data in mapList file.");
    }
}