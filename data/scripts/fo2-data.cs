using System;
using System.Windows.Forms;
using PlayFO.Scripts;
using DATLib;

public class Script : IResolveScript
{
    public bool IsValidResource(string name, string filePath)
	{
	   DatReaderError status;
	   string DatPath = filePath ;
	   DAT loadedDat = DATReader.ReadDat(DatPath, out status);
	   if (status.Error != DatError.Success)
	   {
		   MessageBox.Show("Error loading " + DatPath + ": " + Environment.NewLine + status.Message);
	   }
	   bool valid = false;
	   if (name.ToLower() == "master.dat")
	   {
		   valid = (loadedDat.FileList.Exists(x => x.FileName.ToLower() == "ai.txt") &&
				loadedDat.FileList.Exists(x => x.FileName.ToLower() == "vault13.gam") &&
				loadedDat.FileList.Exists(x => x.FileName.ToLower() == "maps.txt") &&
				loadedDat.FileList.Exists(x => x.FileName.ToLower() == "adb001.frm") &&
				loadedDat.FileList.Exists(x => x.FileName.ToLower() == "acavcol1.frm")
			   );
	   }
	   if (name.ToLower() == "critter.dat")
	   {
		   valid = (loadedDat.FileList.Exists(x => x.FileName.ToLower() == "critters.lst") &&
				loadedDat.FileList.Exists(x => x.FileName.ToLower() == "hanpwraa.frm") &&
				loadedDat.FileList.Exists(x => x.FileName.ToLower() == "hanpwral.frm") &&
				loadedDat.FileList.Exists(x => x.FileName.ToLower() == "hanpwrbi.frm") &&
				loadedDat.FileList.Exists(x => x.FileName.ToLower() == "hanpwrbl.frm")
			   );
	   }
	   return valid;
	}
}
