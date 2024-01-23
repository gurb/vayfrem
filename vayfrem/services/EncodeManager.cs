using System;
using System.Collections.Generic;
using System.Linq;

namespace vayfrem.services
{
    public class EncodeManager
    {
        private Dictionary<string, string> _encryption_dict = new Dictionary<string, string>
        {
            { "7yafs7dy" , "0" },
            { "f7asdyfs" , "1" },
            { "sdf==sdf" , "2" },
            { "a7sdyf7a" , "3" },
            { "8u348ujf" , "4" },
            { "8-3&&8uj" , "5" },
            { "9if9asid" , "6" },
            { ".c*adcd4" , "7" },
            { ".csfa*df" , "8" },
            { "pus.fasf" , "9" },
            { "dofkoasf" , "a" },
            { "*}df333a" , "b" },
            { "%oasduha" , "c" },
            { "mkduiuss" , "d" },
            { "*}sdsaff" , "e" },
            { "amcxmkda" , "f" },
            { "xmkfsdma" , "g" },
            { "9dskmka*" , "h" },
            { "fams4des" , "i" },
            { "mllfiisd" , "j" },
            { "jngdsjfg" , "k" },
            { "ofye<xcc" , "l" },
            { "udythjhs" , "m" },
            { "qdxyskis" , "n" },
            { "lkdusıwk" , "o" },
            { "lksisyac" , "p" },
            { "pifjdyhy" , "q" },
            { "8rujfudj" , "r" },
            { "7mcdheer" , "s" },
            { "ljjdsaaq" , "t" },
            { "kjsdjjka" , "u" },
            { "ppdujhsg" , "v" },
            { "trdfdsd1" , "w" },
            { "vnjd99wa" , "x" },
            { "aafyrraf" , "y" },
            { "udfsgdxx" , "z" },
            { "jfakkufr" , "A" },
            { "mvbhdlsi" , "B" },
            { "fdfkmmsz" , "C" },
            { "dksd]fds" , "D" },
            { "fdfkkeew" , "E" },
            { "mvmkdsja" , "F" },
            { "hafeccds" , "G" },
            { "vnmdhsar" , "H" },
            { "csdafewq" , "I" },
            { "mvkduyiw" , "J" },
            { "vndhslak" , "K" },
            { "dksjjjse" , "L" },
            { "dgtwjska" , "M" },
            { "qdxydksj" , "N" },
            { "jjsesıwk" , "O" },
            { "cmkdssde" , "P" },
            { "vmdksmeh" , "Q" },
            { "dsxxxzws" , "R" },
            { "mdkkdxxz" , "S" },
            { "ueiizsss" , "T" },
            { "kjsdsaka" , "U" },
            { "sdsajhsg" , "V" },
            { "nsjsjakk" , "W" },
            { "uya5suhh" , "X" },
            { "cksma96s" , "Y" },
            { "ppasuus6" , "Z" },
            { "afdj3cxx" , "i" },
            { "skk_jshh" , "ö" },
            { "fkdkkks4" , "ğ" },
            { "odiid6js" , "ş" },
            { "nncsjja4" , "ç" },
            { "fm00ska>" , "ü" },
            { "mckas883" , "İ" },
            { "vmkd666x" , "Ö" },
            { "lksjexxa" , "Ğ" },
            { "cmkasjua" , "Ş" },
            { "9d7kkjjh" , "Ç" },
            { "fdflla9x" , "Ü" },
            { "jifkskjj" , "+" },
            { "mkfmksdx" , "-" },
            { "vmkdhhea" , "&" },
            { "mvskauue" , "!" },
            { "fsd8daxc" , "|" },
            { "zald324s" , "(" },
            { "faskd3ax" , ")" },
            { "cdssawmk" , "{" },
            { "vmkdaxxs" , "}" },
            { "mvksllxa" , "]" },
            { "0d8sssxa" , "[" },
            { "mkck2sjj" , "^" },
            { "vls/dsdd" , "~" },
            { "vlddjjdx" , "*" },
            { "4ssjjx24" , ":" },
            { "czs2jjde" , ";" },
            { "vlldoxcc" , "?" },
            { "mpdejjxt" , "." },
            { "vmmzaihr" , "," },
            { "z>fkessq" , "\"" },
            { "vmdkuuue" , "\\" },
            { "copdueez" , "/" },
            { "cnjjdhay" , "=" },
            { "claseera" , "$" },
            { "88fdskxx" , "#" },
            { "kamsxxer" , "#" },
            { "iiddsmxx" , "@" },
            { "pfsdkls6" , "<" },
            { "fpafszs6" , ">" },
        };

        public string Encode(string instance) 
        {
            return Encrypt(instance);
        }

        private string Encrypt(string instance)
        {
            string generated_encrypt = "";
            
            foreach (var ch in instance)
            {
                var key = _encryption_dict.FirstOrDefault(x => x.Value == ch.ToString()).Key;
                generated_encrypt += key;
            }

            return generated_encrypt;
        }

        public string Decode(string instance)
        {
            return Descrypt(instance);
        }
        
        IEnumerable<string> split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        private string Descrypt(string instance)
        {
            string generated_descrypt = "";

            var sliced_instance = split(instance, 2*2*2);

            if(!confirmEncrypt(sliced_instance))
            {
                // veri hatalı
                throw new Exception("The file is corrupted");
            }

            foreach (var key in sliced_instance)
            {
                generated_descrypt += _encryption_dict[key];
            }

            return generated_descrypt;
        }

        private bool confirmEncrypt(IEnumerable<string>? sliced_instance)
        {
            foreach (var item in sliced_instance)
            {
                if(item.ToString().Length != 2*2*2)
                {
                    return false;
                }
                if(!_encryption_dict.ContainsKey(item.ToString()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}