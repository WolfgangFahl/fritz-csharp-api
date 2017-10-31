using System;
using System.Text;
using System.Collections.Generic;
using fritz;

public class FritzTest
{
    Fritz fritz;
    int errors=0;
    
    public FritzTest() {
       fritz=new Fritz();
    }
    
    /**
     * check the MD5 calculation for the given string s to 
     * be as expected
     */
    public void checkMd5(string s,string expected) {
      Console.ForegroundColor=ConsoleColor.Black;
      String md5=fritz.GetMD5Hash(s);
      Console.Write("md5("+s+")=");
      if (md5.Equals(expected)) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(md5);
      } else {
        errors++;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(md5+"!="+expected);
      }
      Console.ForegroundColor=ConsoleColor.Black;
    }

		/**
		 * run the tests
		 */    
		public void runTests() {
			checkMd5("","d41d8cd98f00b204e9800998ecf8427e");
			checkMd5("secret", "09433e1853385270b51511571e35eeca");
			checkMd5("test", "c8059e2ec7419f590e79d7f1b774bfe6");
			// see
			// https://avm.de/fileadmin/user_upload/Global/Service/Schnittstellen/AVM_Technical_Note_-_Session_ID.pdf
			checkMd5("1234567z-äbc", "9e224a41eeefa284df7bb0f26c2913e2");
			checkMd5("!\"§$%&/()=?ßüäöÜÄÖ-.,;:_`´+*#'<>≤|","ad44a7cb10a95cb0c4d7ae90b0ff118a");
			// see https://stackoverflow.com/questions/29277921/different-md5-outputs-in-shell-and-script/47021514#47021514
			checkMd5("challenge-password1234","1722e126192656712a1d352e550f1317");
			
			string sid = fritz.GetSessionId(); 
			Console.WriteLine("sid="+sid);
			Console.WriteLine(string.Format("{0} errors found",errors));
    }
    
    /**
     * main entry for this code
     */
    static public void Main () 
    {
       FritzTest fritzTest=new FritzTest();
       fritzTest.runTests();     
    }
}
