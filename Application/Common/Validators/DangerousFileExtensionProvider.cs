using System.Collections.Generic;
using System.Collections.Immutable;

namespace Biobrain.Application.Common.Validators
{
    public static class DangerousFileExtensionProvider
    {
        /// <summary>
        ///     Provides list of potential dangerous files we don't want to be stored within application
        /// </summary>
        /// <returns></returns>
        public static ImmutableHashSet<string> Bucket { get; } = Get().ToImmutableHashSet();

        private static IEnumerable<string> Get()
        {
            // File Type: Windows files
            // Threat: Can be used for attacks
            yield return ".application";
            yield return ".gadget";

            // File Type: Powershell files
            // Threat: Can be used for attacks
            yield return ".ps1";
            yield return ".ps1xml";
            yield return ".ps2";
            yield return ".ps2xml";
            yield return ".psc1";
            yield return ".psc2";

            // File Type: ASP.NET related files
            // Threat: Can be used for attacks on ASP.NET
            yield return ".asax";
            yield return ".axd";
            yield return ".browser";
            yield return ".compile";
            yield return ".disco";
            yield return ".vsdisco";
            yield return ".dsdgm";
            yield return ".dsprototype";
            yield return ".licx";
            yield return ".webinfo";
            yield return ".mdb";
            yield return ".ldb";
            yield return ".mdf";
            yield return ".msgx";
            yield return ".svc";
            yield return ".resources";
            yield return ".resx";
            yield return ".sdm";
            yield return ".sdmDocument";
            yield return ".sitemap";
            yield return ".skin";
            yield return ".soap";
            yield return ".asa";
            yield return ".imDitto";
            yield return ".cdx";
            yield return ".idc";
            yield return ".shtm";
            yield return ".shtml";
            yield return ".stm";
            yield return ".aspx";
            yield return ".ascx";
            yield return ".asmx";
            yield return ".ashx";
            yield return ".master";
            yield return ".config";
            yield return ".less";
            yield return ".cshtml";
            yield return ".layout";

            // File Type: Access project files
            // Threat: Can contain autoexecuting macros
            yield return ".ade";
            yield return ".adp";
            yield return ".and";

            // File Type:  Streaming audio or video file
            // Threat: Can be exploited through buffer overflows, head malformation, or dangerous scriptable content
            yield return ".asf";
            yield return ".lsf";
            yield return ".lsx";

            // File Type:  Symantec pcAnywhere autotransfer file
            // Threat: Can initiate a pcAnywhere file-transfer session
            yield return ".atf";

            // File Type: Visual Basic (VB) class module
            // Threat: Can be a malicious program
            yield return ".bas";

            // File Type: DOS batch file
            // Threat: Can contain malicious instructions
            yield return ".bat";

            // File Type: Microsoft cabinet archive file
            // Threat: Opens in IE and can help install malicious files
            yield return ".cab";

            // File Type: Security certificate
            // Threat:  Can install a malicious certificate in IE to permit automatic downloading of malicious content
            yield return ".cer";
            yield return ".crt";
            yield return ".der";

            // File Type: NT command script
            // Threat: Can be used to script malicious batch files
            yield return ".cmd";

            // File Type: MS-DOS application
            // Threat:  Can be a malicious program
            yield return ".com";

            // File Type: Control Panel extension
            // Threat: Can install a malicious Control Panel applet
            yield return ".cpl";

            // File Type: Certificate revocation list(CRL)
            // Threat: Can be a malicious list that can cause problems with valid certificates
            yield return ".crl";

            // File Type: Cascading Style Sheets
            // Threat: Can be used in IE exploits
            yield return ".css";

            // File Type: Windows DLL application
            // Threat: Can contain malicious code
            yield return ".dll";

            // File Type: Nullsoft WinAmp media file
            // Threat:  Can be used to launch malicious exploits
            yield return ".dsm";
            yield return ".far";
            yield return ".it";
            yield return ".stm";
            yield return ".ult";
            yield return ".wma";

            // File Type: DUN export file
            // Threat: Can contain malicious dial-up connection information that initiates outward calls
            yield return ".dun";

            // File Type:  Application file
            // Threat: Can be used to launch malicious executables
            yield return ".exe";

            // File Type: IE Favorites list
            // Threat:  Can be used to list malicious Web sites
            yield return ".fav";

            // File Type:  Microsoft Help File
            // Threat: Can be used in multiple exploits
            yield return ".hlp";

            // File Type:  Hyperterminal file
            // Threat: Can initiate dial-up connections to untrusted hosts
            yield return ".ht";
            yield return ".htt";

            // File Type: HTML application
            // Threat: Frequently used by worms and trojans
            yield return ".hta";

            // File Type: IE HTML file
            // Threat: Can initiate an IE session and be used to automatically download and execute rogue files
            yield return ".htm";
            yield return ".html";

            // File Type:  Application configuration settings file
            // Threat: Can be used to maliciously change a program’s default settings
            yield return ".ini";

            // File Type:  Internet communication settings
            // Threat: Can be used to initiate Internet connections to untrusted sources
            yield return ".ins";
            yield return ".isp";

            // File Type: Java archive file
            // Threat: Can launch Java attacks
            yield return ".jar";

            // File Type: Java applet
            // Threat: Can launch Java attacks
            yield return ".jav";
            yield return ".java";

            // File Type: JavaScript (encoded) file
            // Threat: Can contain malicious code
            yield return ".js";
            yield return ".jse";

            // File Type: Shortcut link
            // Threat: Can be used to automate malicious actions
            yield return ".lnk";
            yield return ".desklink";

            // File Type: Access application or database
            // Threat: Can contain malicious macros
            yield return ".mdb";
            yield return ".mdbhtml";

            // File Type: Access database with all modules compiled and source code removed
            // Threat: Can contain malicious macros
            yield return ".mde";

            // File Type: MIME HTML document
            // Threat:  Can contain harmful commands
            yield return ".mhtml";
            yield return ".mhtm";

            // File Type: MIME file
            // Threat: Could become a target of future MIME exploits
            yield return ".mim";

            // File Type: Management Saved Console file
            // Threat: Can be used to gain privileged access or to cause damage
            yield return ".msc";

            // File Type: Microsoft Installer package
            // Threat: Can be used to install or modify software
            yield return ".msi";
            yield return ".msp";

            // File Type: Microsoft Transform file, used during some installation programs
            // Threat: Can be used maliciously
            yield return ".mst";

            // File Type:  Outlook Express news message
            // Threat: Can carry the Nimda virus or other malware
            yield return ".nws";

            // File Type: VB compiled script
            // Threat: Can contain dangerous code
            yield return ".pdc";

            // File Type: Program information file
            // Threat: Can run malicious programs
            yield return ".pif";

            // File Type: Perl script file
            // Threat: Can contain rogue code
            yield return ".pl";

            // File Type: Python script file
            // Threat: Can contain rogue code
            yield return ".py";

            // File Type: Registry entry file
            // Threat: Can create or modify registry keys
            yield return ".reg";

            // File Type: Windows Explorer command
            // Threat: Could be used maliciously in future attacks
            yield return ".scf";

            // File Type: DUN script
            // Threat: Can initiate rogue outbound connections
            yield return ".scp";

            // File Type: Windows screen saver file
            // Threat: Can contain worms or trojans
            yield return ".scr";

            // File Type: Shell scrap object
            // Threat:  Can mask rogue programs
            yield return ".shs";
            yield return ".shb";

            // File Type: Certificate trust list (CTL)
            // Threat:  Can induce user to trust a rogue certificate
            yield return ".stl";

            // File Type: Shockwave Flash object
            // Threat: Can be exploited
            yield return ".swf";
            yield return ".spl";

            // File Type: Internet shortcut
            // Threat:  Can connect user to malicious Web site or launch a malicious action
            yield return ".url";

            // File Type: VBScript file
            // Threat: Can contain malicious code
            yield return ".vb";
            yield return ".vbe";
            yield return ".vbs";

            // File Type: Virtual device driver
            // Threat:  Can trick user into saving a trojan version of a legitimate device driver
            yield return ".vxd";

            // File Type: Word backup document
            // Threat: Can contain dangerous macros
            yield return ".wbk";

            // File Type: Wizard file
            // Threat: Could be used to automate future social engineering attack
            yield return ".wiz";

            // File Type: WSH file
            // Threat:  Can execute malicious code
            yield return ".ws";
            yield return ".cs";
            yield return ".wsh";
            yield return ".wsf";
            yield return ".wsc";
            yield return ".sct";

            // File Type:  XML file
            // Threat: Likely to be the next language of choice for malicious coders
            yield return ".xml";
            yield return ".xsl";

            // Other
            yield return ".ocx";
            yield return ".tlb";
            yield return ".ce";
            yield return ".ceo";
            yield return ".chm";
            yield return ".inf";
            yield return ".mid";
            yield return ".midi";
            yield return ".uue";
            yield return ".vcpl";
        }
    }
}
