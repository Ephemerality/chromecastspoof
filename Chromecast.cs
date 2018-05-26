using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MiscUtil.IO;

namespace ChromecastSpoof
{
    class Chromecast
    {
        private bool _ultra;
        private string _hostname;
        private string _hash;

        public Chromecast(string name, string ip, bool ultra)
        {
            Name = name;
            _hash = MD5Hash();
            IP = ip;
            _ultra = ultra;
            _hostname = "Chromecast-" + (ultra ? "Ultra-" : "") + _hash;
        }

        public string Name { get; }

        public string Ultra { get { return _ultra ? "Yes" : "No"; } }

        public string IP { get; }

        private string SplitHostname
        {
            get
            {
                return Regex.Replace(_hostname, "^(.{8})(.{4})(.{4})(.{4})(.{12})$", "$1-$2-$3-$4-$5");
            }
        }

        public string MD5Hash()
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(Name));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));
                return sb.ToString();
            }
        }

        public byte[] GetBroadcastPacket(string prefix = "")
        {
            MemoryStream stream = new MemoryStream();
            EndianBinaryWriter write = new EndianBinaryWriter(MiscUtil.Conversion.EndianBitConverter.Big, stream);
            write.Write((ushort)0);
            write.Write((ushort)0x8400); //flags
            write.Write((ushort)0); //questions
            write.Write((ushort)1); //answer RRs
            write.Write((ushort)0); //authority RRs
            write.Write((ushort)3); //additional answer RRs
            // Write answer record
            if (prefix != "")
                WriteOctets(new string[] { prefix, "_sub", "_googlecast", "_tcp", "local" }, write);
            else
                WriteOctets(new string[] { "_googlecast", "_tcp", "local" }, write);
            write.Write((ushort)12); //type (PTR, domain name PoinTeR)
            write.Write((ushort)1); //class
            write.Write((uint)120); //ttl
            WriteOctets(new string[] { _hostname, "_googlecast", "_tcp", "local" }, write, true);

            // Additional records
            // Write TXT record
            WriteOctets(new string[] { _hostname, "_googlecast", "_tcp", "local" }, write);
            write.Write((ushort)16); //type (TXT)
            write.Write((ushort)0x8001); //class
            write.Write((uint)4500); //ttl
            string[] txtRecs = { "id=" + _hash,
                "cd=",
                "rm=",
                "ve=05",
                "md=Chromecast Ultra",
                "ic=/setup/icon.png",
                "fn=" + Name,
                "ca=4101",
                "st=0",
                "bs=",
                "nf=1",
                "rs="};
            WriteTXT(txtRecs, write);

            // Write SRV record
            WriteOctets(new string[] { _hostname, "_googlecast", "_tcp", "local" }, write);
            write.Write((ushort)33); //type (SRV)
            write.Write((ushort)0x8001); //class
            write.Write((uint)120); //ttl
            MemoryStream tempstr = new MemoryStream();
            EndianBinaryWriter tempwrite = new EndianBinaryWriter(MiscUtil.Conversion.EndianBitConverter.Big, tempstr);
            tempwrite.Write((ushort)0); //priority
            tempwrite.Write((ushort)0); //weight
            tempwrite.Write((ushort)8009); //port
            WriteOctets(new string[] { SplitHostname, "local" }, tempwrite);
            byte[] buffer = tempstr.ToArray();
            write.Write((ushort)buffer.Length);
            write.Write(buffer);

            // Write A record
            WriteOctets(new string[] { SplitHostname, "local" }, write);
            write.Write((ushort)1); //type (A)
            write.Write((ushort)0x8001); //class
            write.Write((uint)120); //ttl
            write.Write((ushort)4); //length
            write.Write(IP.Split('.').Select<string, byte>(i => byte.Parse(i)).ToArray());
            return stream.ToArray();
        }

        private void WriteOctets(string[] octets, EndianBinaryWriter write, bool writeLen = false)
        {
            if (writeLen)
            {
                int count = octets.Select(i => i.Length).Sum() + octets.Length + 1;
                write.Write((ushort)count);
            }
            for (int i = 0; i < octets.Length; i++)
            {
                write.Write((byte)octets[i].Length);
                write.Write(Encoding.ASCII.GetBytes(octets[i]));
            }
            write.Write((byte)0);
        }

        private void WriteTXT(string[] text, EndianBinaryWriter write)
        {
            MemoryStream tempstr = new MemoryStream();
            EndianBinaryWriter tempwrite = new EndianBinaryWriter(MiscUtil.Conversion.EndianBitConverter.Big, tempstr);
            for (int i = 0; i < text.Length; i++)
            {
                tempwrite.Write(text[i]);
            }
            byte[] buffer = tempstr.ToArray();
            write.Write((ushort)buffer.Length);
            write.Write(buffer);
        }
    }
}
