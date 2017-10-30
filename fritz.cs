// WF 2017-10-30
// adapted from
// https://avm.de/fileadmin/user_upload/Global/Service/Schnittstellen/AVM_Technical_Note_-_Session_ID.pdf
using System.Net; 
using System.Security.Cryptography; 
using System.Xml.Linq; 
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

namespace fritz {
  /**
   * example FritzBox access
   */
  public class Fritz {
  	
  	string username;
    string password;
    string url;
    
    /**
     * construct FritzBox access reading application properties from ini file in users home directory
     */
    public Fritz() {
    	IDictionary<string,string> properties=ReadProperties("/Users/wf/.fritzbox/application.properties");
      properties.TryGetValue("fritzbox.username",out username);
      properties.TryGetValue("fritzbox.password",out password);
      properties.TryGetValue("fritzbox.url",out url);
    }
  	
  	/**
  	 * main entry for this code
  	 */
    static public void Main () { 
      Fritz fritz=new Fritz();
      // SessionID ermitteln 
      ServicePointManager.ServerCertificateValidationCallback += (p1, p2, p3, p4) => true;
      string sid = fritz.GetSessionId(); 
      string seite = fritz.SeiteEinlesen(@"http://fritz.box/home/home.lua", sid); 
      Console.WriteLine(seite);      
   } 
   
   /**
    * read application properties
    * https://stackoverflow.com/questions/485659/can-net-load-and-parse-a-properties-file-equivalent-to-java-properties-class
    */
  public static IDictionary<string,string> ReadProperties(string fileName)
  {
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    foreach (string line in File.ReadAllLines(fileName))
    {
        if ((!string.IsNullOrEmpty(line)) &&
            (!line.StartsWith(";")) &&
            (!line.StartsWith("#")) &&
            (!line.StartsWith("'")) &&
            (line.Contains("=")))
        {
            int index = line.IndexOf('=');
            string key = line.Substring(0, index).Trim();
            string value = line.Substring(index + 1).Trim();

            if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                (value.StartsWith("'") && value.EndsWith("'")))
            {
                value = value.Substring(1, value.Length - 2);
            }
            dictionary.Add(key, value);
        }
    }

    return dictionary;
  }
    
    public string SeiteEinlesen (string url, string sid) { 
      Uri uri = new Uri(url + "?sid=" + sid); 
     
      HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest; 
      HttpWebResponse response = request.GetResponse() as HttpWebResponse; 
      StreamReader reader = new StreamReader(response.GetResponseStream()); 
      string str = reader.ReadToEnd(); 
      return      str;      
   } 

    public string GetSessionId () { 
        XDocument doc = XDocument.Load(@url+"/login_sid.lua"); 
        string sid = GetValue(doc, "SID"); 
        if (sid == "0000000000000000") { 
              string challenge = GetValue(doc, "Challenge"); 
              string uri = @url+"/login_sid.lua?username=" + 
  username + @"&response=" + GetResponse(challenge, password); 
              doc      =      XDocument.Load(uri);      
              sid = GetValue(doc, "SID"); 
        }      
        return      sid;      
    } 
    
    public string GetResponse (string challenge, string password) { 
        return challenge + "-" + GetMD5Hash(challenge + "-" + password); 
    }
    
    public string GetMD5Hash (string input) { 
      MD5 md5Hasher = MD5.Create(); 
      byte[] data = 
md5Hasher.ComputeHash(Encoding.Unicode.GetBytes(input)); 
      StringBuilder sb = new StringBuilder(); 
      for (int i = 0; i < data.Length; i++) { 
            sb.Append(data[i].ToString("x2"));      
      }      
      return      sb.ToString();      
    } 
    
    public string GetValue (XDocument doc, string name) { 
      XElement info = doc.FirstNode as XElement; 
      return      info.Element(name).Value;
    }
  }
}