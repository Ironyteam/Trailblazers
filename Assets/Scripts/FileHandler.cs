using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text; //+++++++++++++++++++++++++++++++++++++

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
    public const string SavedMapsPath = "Assets/SavedMaps";                 // Consider removing

    public void checkForFiles()
    {
            // Create Saved Maps directory if one does not exist
            if (!Directory.Exists(Application.dataPath + "/SavedMaps"))
                Directory.CreateDirectory(Application.dataPath + "/SavedMaps");
            else
                Debug.Log("Directory exists");

        // Create map list file if one does not exist
        if (!File.Exists(Application.dataPath + "/SavedMaps/MapList.txt"))
        {
            using (StreamWriter temp = File.CreateText(Application.dataPath + "/SavedMaps/MapList.txt"))
            {
                temp.Close();
            }
        }
        else
            Debug.Log("File exists");
    }

    public void deleteMap(string name)
    {
        List<string> mapNames = new List<string>();
        List<string> mapInfo = new List<string>();

        try
        {
            File.Delete(Application.dataPath + "/SavedMaps/" + name + ".txt");
            File.Delete(Application.dataPath + "/SavedMaps/" + name + ".png");
        }
        catch(Exception)
        {
            Debug.Log("Map unlinked but not deleted.");
        }

        // Get a list of all names in the map list file
        using (StreamReader reader = File.OpenText(Application.dataPath + "/SavedMaps/MapList.txt"))
        {
            string tempString;
            string[] tempArray;

            while ((tempString = reader.ReadLine()) != null)
            {
                mapInfo.Add(tempString);
                tempArray = tempString.Split(' ');
                mapNames.Add(tempArray[0]);
            }
            reader.Close();
        }

        // Remove map's name from the list of map names
        mapInfo.Remove(mapInfo[mapNames.IndexOf(name)]);

        // Rewrite the remaining names to the map list file
        using (StreamWriter writer = File.CreateText(Application.dataPath + "/SavedMaps/MapList.txt"))
        {
            foreach (string line in mapInfo)
            {
                writer.WriteLine(line);
            }
            writer.Close();
        }
    }

    public List<Sprite> getScreenShots(List<string> mapNames, int savedMapsStartIndex)
    {
        List<Sprite> screenShots = new List<Sprite>();

        for (int index = savedMapsStartIndex; index < mapNames.Count; index++)
        {
            byte[] bytes = null;

            try                     //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
            {
                bytes = System.IO.File.ReadAllBytes(Application.dataPath + "/SavedMaps/" + mapNames[index] + ".png");
                Texture2D texture = new Texture2D(SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH);
                texture.filterMode = FilterMode.Trilinear;
                texture.LoadImage(bytes);
                screenShots.Add(Sprite.Create(texture, new Rect(0, 0, SCREEN_SHOT_WIDTH, SCREEN_SHOT_LENGTH), new Vector2(0.5f, 0.0f), 1.0f));
            }
            catch (Exception)
            {
                screenShots.Add(null);
            }
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

        // May need to add a try catch in case there are not any files
        TextAsset mapList = Resources.Load("DefaultMaps/MapList") as TextAsset;
        byte[] byteArray = Encoding.UTF8.GetBytes(mapList.text);
        MemoryStream stream = new MemoryStream(byteArray);

        using (StreamReader reader = new StreamReader(stream))
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

        using (StreamReader reader = File.OpenText(Application.dataPath + "/SavedMaps/MapList.txt"))
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

    public HexTemplate retrieveMap(string mapName, bool isDefaultMap)
    {
        HexTemplate template = new HexTemplate();
        StreamReader reader;
        int readChar = 0;
        string current = null;
        string next = null;
        int result = -1;
        int resource = -1;
        int diceNum = -1;

        if (isDefaultMap)
        {
            Debug.Log(mapName);
            TextAsset map = Resources.Load("DefaultMaps/" + mapName) as TextAsset;
            byte[] byteArray = Encoding.UTF8.GetBytes(map.text);
            MemoryStream stream = new MemoryStream(byteArray);
            reader = new StreamReader(stream);
        }
        else
        {
            reader = File.OpenText(Application.dataPath + "/SavedMaps/" + mapName + ".txt");
        }

        // Modify to do something with the name and creator
        using (reader)
        {
            List<string> numbers = new List<string>();

            // Skip over name and creator
            template.mapName = reader.ReadLine();
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
    public string ReadEntireMap(string mapName, bool isDefaultBoard)
    {
        StreamReader reader;
        string fileString;

        if (isDefaultBoard)
        {
            TextAsset map = Resources.Load("DefaultMaps/" + mapName) as TextAsset;
            byte[] byteArray = Encoding.UTF8.GetBytes(map.text);
            MemoryStream stream = new MemoryStream(byteArray);
            reader = new StreamReader(stream);
        }
        else
        {
            reader = File.OpenText(Application.dataPath + "/SavedMaps/" + mapName + ".txt");
        }

        // Modify to do something with the name and creator
        using (reader)
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

        using (StreamWriter writer = File.CreateText(Application.dataPath + "/SavedMaps/" + name + ".txt"))
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

        using (StreamReader reader = File.OpenText(Application.dataPath + "/SavedMaps/MapList.txt"))
        {
            string tempString;

            while ((tempString = reader.ReadLine()) != null)
                mapNames.Add(tempString);
            reader.Close();
        }

        using (StreamWriter writer = File.CreateText(Application.dataPath + "/SavedMaps/MapList.txt"))
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
        List<string> mapNames = new List<string>();         //vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv Many changes
        string[] fileLines;
        string modifiedFileString;

        modifiedFileString = rawFileString.Replace("\r\n", "\n");
        fileLines = modifiedFileString.Split('\n');

        Debug.Log(fileLines[0]);
        using (StreamWriter writer = File.CreateText(Application.dataPath + "/SavedMaps/" + fileLines[0] + ".txt"))
        {
            writer.Write(rawFileString);
            writer.Close();
        }

        // May need to add a try catch in case there are not any files
        using (StreamReader reader = File.OpenText(Application.dataPath + "/SavedMaps/MapList.txt"))
        {
            string tempString;

            while ((tempString = reader.ReadLine()) != null)
                mapNames.Add(tempString);
            reader.Close();
        }
        
        // Separate the lines of the file string and use the appropriate lines to
        // write the minimum and maximum victory points
        using (StreamWriter writer = File.CreateText(Application.dataPath + "/SavedMaps/MapList.txt"))
        {
            writer.WriteLine(fileLines[0] + " " + fileLines[2] + " " + fileLines[3]);
            Debug.Log(fileLines[0] + " " + fileLines[2] + " " + fileLines[3]);
            foreach (string mapName in mapNames)
            {
                writer.WriteLine(mapName);
            }
            writer.Close();
        }

    }
   public bool mapExists(string name)
   {
      HexTemplate[] maps;
      bool exists = false;
      int savedMapsStartIndex;

      // See if a map already exists with the desired name
      maps = getAllMaps(out savedMapsStartIndex).ToArray();
      for (int index = savedMapsStartIndex; index < maps.Length; index++)
      {
            if (String.Compare(maps[index].mapName, name) == 0)
            {
               exists = true;
               index = maps.Length;
            }
      }
      return exists;
   }
   public int findNumberToAppendToName(string mapName, int max_number)           //vvvvvvvvvvvvvvvv entire function added
    {
        List<HexTemplate> maps = new List<HexTemplate>();
        FileHandler fileChecker = new FileHandler();
        int savedMapsStartindex = 0;
        int firstAvailableNumber = -1;
        bool numberUnused = true;

        // Search for an number that has not yet been appended onto
        // the same name
        maps = fileChecker.getAllMaps(out savedMapsStartindex);
        int index;
        for (index = 2; index <= max_number; index++)
        {
            numberUnused = true;
            for (int i = savedMapsStartindex; i < maps.Count; i++)
            {
                if (String.Compare(maps[i].mapName, mapName + "(" + index + ")") == 0)
                {
                    numberUnused = false;
                    i = maps.Count;
                }
            }

            if (numberUnused)
            {
                firstAvailableNumber = index;
                index = max_number + 1;
            }
        }
        return firstAvailableNumber;
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