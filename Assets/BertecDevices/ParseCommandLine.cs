using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;


public class ParseCommandLine
{
   public bool toBool(string s)
   {
      if (String.Compare(s, "true", true) == 0)
         return true;
      if (String.Compare(s, "false", true) == 0)
         return false;
      return (s != "0");   // otherwise, anything not equal to zero is true
   }

   public bool toBool(string s, bool def)
   {
      if (queryParameters[s] != null)
         return toBool(s);
      else
         return def;
   }

   public bool contains(string s)
   {
      return (queryParameters[s] != null);
   }

   public string queryString(string key, string val)
   {
      if (queryParameters[key] != null)
         val = queryParameters[key];
      return val;
   }

   public int queryInt(string key, int val)
   {
      if (queryParameters[key] != null)
         val = (int)Convert.ToSingle(queryParameters[key]); // sometimes we get passed a 1.0 and need to allow for it.
      return val;
   }

   public float queryFloat(string key, float val)
   {
      if (queryParameters[key] != null)
         val = Convert.ToSingle(queryParameters[key]);
      return val;
   }

   public bool queryBool(string key, bool val)
   {
      if (queryParameters[key] != null)
      {
         string s = queryParameters[key].ToLower();
         if (s == "true")
            return true;
         if (s == "false")
            return false;
         val = Convert.ToInt32(s) != 0;
      }
      return val;
   }

   // useful for extracting "opts"
   public NameValueCollection querySubCollection(string key)
   {
      NameValueCollection subCollect = new NameValueCollection();

      if (queryParameters[key] != null)
      {
         String jsonText = queryParameters[key].ToString();
         //Debug.Log(jsonText);
         Dictionary<string, object> o = (Dictionary<string, object>)fastJSON.JSON.Instance.Parse(jsonText);
         foreach (var kvp in o)
         {
            //Debug.Log("[" + kvp.Key.ToString() + "] = [" + kvp.Value.ToString() + "]");
            subCollect.Add(kvp.Key.ToString(), kvp.Value.ToString());
         }
      }

      return subCollect;
   }

   /////////////////////////////////////////////////////

   internal NameValueCollection queryParameters = new NameValueCollection();

   private static ParseCommandLine _instance;

   public static ParseCommandLine Instance
   {
      get
      {
         if (_instance == null)
            _instance = new ParseCommandLine();
         return _instance;
      }
   }

   /////////////////////////////////////////////////////
   // This will parse either the URL command line for the web player or the command line option into a named pair group.
   // URLs are broken into ? query strings, and command lines are -foo=bar or -mumble or --option=value
   private ParseCommandLine()
   {
      queryParameters = new NameValueCollection();
#if UNITY_WEBPLAYER
      string queryString = Application.srcValue;
      if (queryString.Contains("?"))
      {
         queryString = queryString.Substring(queryString.IndexOf('?') + 1);
      }

      foreach (string vp in Regex.Split(queryString, "&"))
      {
         string[] singlePair = Regex.Split(vp, "=");
         if (singlePair.Length == 2)
         {
            queryParameters.Add(Uri.UnescapeDataString(singlePair[0]), Uri.UnescapeDataString(singlePair[1]));
         }
         else
         {
            // only one key with no value specified in query string
            queryParameters.Add(Uri.UnescapeDataString(singlePair[0]), string.Empty);
         }
      }
#else
      Debug.Log("parsing command line");
      String[] arguments = Environment.GetCommandLineArgs();
      foreach (string s in arguments)
      {
         Debug.Log("command line: " + s);
         if (s.StartsWith("-") || s.StartsWith("/"))
         {
            string vp = s;
            // remove all leading dashes or slashes (so that --foo becomes foo)
            while (vp.StartsWith("-") || vp.StartsWith("/"))
            {
               vp = s.Substring(1);
            }

            Debug.Log("splitting " + vp);

            string[] singlePair = Regex.Split(vp, "=");
            if (singlePair.Length == 2)
            {
               Debug.Log("split into " + singlePair[0] + " and " + singlePair[1]);
               queryParameters.Add(Uri.UnescapeDataString(singlePair[0]), Uri.UnescapeDataString(singlePair[1]));
            }
            else
            {
               // only one key with no value specified in query string
               Debug.Log("split into " + singlePair[0]);
               queryParameters.Add(Uri.UnescapeDataString(singlePair[0]), string.Empty);
            }
         }
      }
#endif
   }
}
