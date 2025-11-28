using AlibreX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlibreGateDesignAutomation.DesignTemplates
{
    internal class TallGardenGate
    {
        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public static void AlibreAutomation(int height, int width)
        {
            IAutomationHook hook;
            try
            {
                //Delete existing design folder if it exists
                Directory.Delete(@"C:\\Files\\MiM\\Design", true);

                //Take a copy of the template folder for editing
                //The linear pattern in Alibre model doesn't like a qty greater than the existing qty
                CopyDirectory(@"C:\\Files\\MiM\\Templates\\TallGardenGate", @"C:\\Files\\MiM\\Design", true);
      
                //CAD file paths
                string oAssembly1 = @"C:\\Files\\MiM\\Design\\TallGardenGate.AD_ASM";
                string oPart1 = @"C:\\Files\\MiM\\Design\\25x5_FLAT_001.AD_PRT";
                string oPart2 = @"C:\\Files\\MiM\\Design\\25x5_FLAT_002.AD_PRT";
                string oPart3 = @"C:\\Files\\MiM\\Design\\25x25x2.5_SHS_001.AD_PRT";
                string oPart4 = @"C:\\Files\\MiM\\Design\\25x25x2.5_SHS_002.AD_PRT";
                string oPart5 = @"C:\\Files\\MiM\\Design\\25x25x2.5_SHS_003.AD_PRT";
                string oPart6 = @"C:\\Files\\MiM\\Design\\12_ROUND_001.AD_PRT";
                string oPart7 = @"C:\\Files\\MiM\\Design\\12_ROUND_002.AD_PRT";

                //CAD input parameters
                double gate_height = 0.1 * height; //Set input value and convert mm to cm
                double gate_width = 0.1 * width; //Set input value and convert mm to cm

                //CAD input conditional checks
                if (gate_height > 200 || gate_height < 100 || gate_width > 150 || gate_width < 30)
                {
                    MessageBox.Show("Maximum gate height = 2000mm." + Environment.NewLine + "Minimum gate height = 1000mm." + Environment.NewLine + "Maximum gate width = 1500mm." + Environment.NewLine + "Minimum gate width = 300mm.");
                }
                //Run design calcs and configure Alibre model if input conditions are met
                else
                {
                    //CAD output parameters
                    double oFlatWidth = gate_width - 2.5 - 2.5;
                    double oBoxHeight = gate_height - 11.8;
                    double oTopRailheadInitialQty = Convert.ToInt32(Math.Floor((gate_width - 2.5) / 10));
                    double oTopRailheadActualQty = Convert.ToInt32(oTopRailheadInitialQty + 2);
                    double oTopRailheadSpacing = Math.Round((gate_width - 2.5) / (oTopRailheadActualQty - 1), 1);
                    double oTotalGap = oFlatWidth % 9;
                    double oRingQty = Convert.ToInt32(Math.Floor(oFlatWidth / 9));
                    double oRingGap = oTotalGap / (oRingQty + 1);

                    //Connect to Alibre
                    hook = (IAutomationHook)Marshal.GetActiveObject("AlibreX.AutomationHook");
                    IADRoot root = (IADRoot)hook.Root;

                    //Open part1 file and edit parameters
                    IADPartSession part1 = (IADPartSession)root.OpenFile(oPart1);
                    part1.Parameters.OpenParameterTransaction();
                    part1.Parameters.Item("gate_width").Value = gate_width;
                    part1.Parameters.CloseParameterTransaction();
                    part1.Close(true);

                    //Open part2 file and edit parameters
                    IADPartSession part2 = (IADPartSession)root.OpenFile(oPart2);
                    part2.Parameters.OpenParameterTransaction();
                    part2.Parameters.Item("oFlatWidth").Value = oFlatWidth;
                    part2.Parameters.CloseParameterTransaction();
                    part2.Close(true);

                    //Open part3 file and edit parameters
                    IADPartSession part3 = (IADPartSession)root.OpenFile(oPart3);
                    part3.Parameters.OpenParameterTransaction();
                    part3.Parameters.Item("gate_width").Value = gate_width;
                    part3.Parameters.CloseParameterTransaction();
                    part3.Close(true);

                    //Open part4 file and edit parameters
                    IADPartSession part4 = (IADPartSession)root.OpenFile(oPart4);
                    part4.Parameters.OpenParameterTransaction();
                    part4.Parameters.Item("oBoxHeight").Value = oBoxHeight;
                    part4.Parameters.CloseParameterTransaction();
                    part4.Close(true);

                    //Open part5 file and edit parameters
                    IADPartSession part5 = (IADPartSession)root.OpenFile(oPart5);
                    part5.Parameters.OpenParameterTransaction();
                    part5.Parameters.Item("oBoxHeight").Value = oBoxHeight;
                    part5.Parameters.CloseParameterTransaction();
                    part5.Close(true);

                    //Open part5 file and edit parameters
                    IADPartSession part6 = (IADPartSession)root.OpenFile(oPart6);
                    part6.Parameters.OpenParameterTransaction();
                    part6.Parameters.Item("oTopBarLength").Value = oBoxHeight - 87.5;
                    part6.Parameters.CloseParameterTransaction();
                    part6.Close(true);


                    //TEST MERGING THE BELOW INTO ONE TRANSACTION
                    //NOW THAT THE LINEAR PATTERN QTY ISSUE HAS BEEN RESOLVED

                    //Open assembly file and edit parameters    
                    IADAssemblySession assembly1 = (IADAssemblySession)root.OpenFile(oAssembly1);
                    assembly1.Parameters.OpenParameterTransaction();
                    assembly1.Parameters.Item("oTopRailheadQty").Value = oTopRailheadActualQty;
                    assembly1.Parameters.Item("oTopRailheadSpacing").Value = oTopRailheadSpacing;
                    assembly1.RegenerateDesign(true);
                    assembly1.Parameters.CloseParameterTransaction();
                    assembly1.Close(true);

                    //Reopen assembly file and edit more parameters    
                    IADAssemblySession assembly2 = (IADAssemblySession)root.OpenFile(oAssembly1);
                    assembly2.Parameters.OpenParameterTransaction();
                    assembly2.Parameters.Item("oRingInitialOffset").Value = oRingGap;
                    assembly2.Parameters.Item("oRingQty").Value = oRingQty;
                    assembly2.Parameters.Item("oRingSpacing").Value = oRingGap + 9;
                    assembly2.RegenerateDesign(true);
                    assembly2.Parameters.CloseParameterTransaction();
                    assembly2.Close(true);

                    //Reopen assembly file and edit more parameters    
                    IADAssemblySession assembly3 = (IADAssemblySession)root.OpenFile(oAssembly1);
                    assembly3.Parameters.OpenParameterTransaction();
                    assembly3.Parameters.Item("oRailheadInitialOffset").Value = oRingGap + 4.5;
                    assembly3.Parameters.Item("oRailheadQty").Value = oRingQty - 1;
                    assembly3.Parameters.Item("oRailheadSpacing").Value = oRingGap + 9;
                    assembly3.RegenerateDesign(true);
                    assembly3.Parameters.CloseParameterTransaction();
                    assembly3.Close(true);
                }


            }
            catch
            {
                MessageBox.Show("Failed to connect to Alibre.");
            }

        }

    }
}
