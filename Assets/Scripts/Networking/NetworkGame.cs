﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkGame
{
	public NetworkGame(){}
	// string will send with format ip, name, players, password
	public string ipAddress;
   public string gameName;
   public string numberOfPlayers = "0";
   public string maxPlayers;
   public string password;
	public string mapName;

   public bool addPlayer()
   {
      int intPlayers    = Int32.Parse(numberOfPlayers);
      int intMaxPlayers = Int32.Parse(maxPlayers);
      if (intPlayers < intMaxPlayers)
      {
         intPlayers++;
         numberOfPlayers = intPlayers.ToString();
      }
      else
         return false;
      return true;
   }
   /*
	public string IpAddress { get { return ipAddress; } set { ipAddress = value; } }
	public string GameName { get; set;}
	public string NumberOfPlayers { get; set;}
	public string MaxPlayers { get; set;}
	public string Password { get; set;}
   */
}