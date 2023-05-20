namespace ImageOverlayRenewal
{
	public class Localize
	{
		public static System.Globalization.CultureInfo Culture {get; set;}
		public static MbyronModsCommon.LocalizeManager LocaleManager {get;} = new MbyronModsCommon.LocalizeManager("Localize", typeof(Localize).Assembly);

		/// <summary>
		/// Apply opacity
		/// </summary>
		public static string ControlPanel_ApplyOpacity => LocaleManager.GetString("ControlPanel_ApplyOpacity", Culture);

		/// <summary>
		/// Capacity
		/// </summary>
		public static string ControlPanel_Capacity => LocaleManager.GetString("ControlPanel_Capacity", Culture);

		/// <summary>
		/// Custom
		/// </summary>
		public static string ControlPanel_Custom => LocaleManager.GetString("ControlPanel_Custom", Culture);

		/// <summary>
		/// Image
		/// </summary>
		public static string ControlPanel_Image => LocaleManager.GetString("ControlPanel_Image", Culture);

		/// <summary>
		/// No
		/// </summary>
		public static string ControlPanel_No => LocaleManager.GetString("ControlPanel_No", Culture);

		/// <summary>
		/// No PNG images were detected in the specified directory, the specified directory can be found in Opti
		/// </summary>
		public static string ControlPanel_NoPNG => LocaleManager.GetString("ControlPanel_NoPNG", Culture);

		/// <summary>
		/// Position
		/// </summary>
		public static string ControlPanel_Position => LocaleManager.GetString("ControlPanel_Position", Culture);

		/// <summary>
		/// Reload texture
		/// </summary>
		public static string ControlPanel_ReloadTexture => LocaleManager.GetString("ControlPanel_ReloadTexture", Culture);

		/// <summary>
		/// Reset to default parameters.
		/// </summary>
		public static string ControlPanel_Reset => LocaleManager.GetString("ControlPanel_Reset", Culture);

		/// <summary>
		/// Rotation
		/// </summary>
		public static string ControlPanel_Rotation => LocaleManager.GetString("ControlPanel_Rotation", Culture);

		/// <summary>
		/// Scroll the wheel to change, Shift X10, Ctrl X0.1
		/// </summary>
		public static string ControlPanel_ScrollWheel => LocaleManager.GetString("ControlPanel_ScrollWheel", Culture);

		/// <summary>
		/// When selected an image, the image pixel information needs to be loaded, and loading time depends on 
		/// </summary>
		public static string ControlPanel_SelectedImageWarning => LocaleManager.GetString("ControlPanel_SelectedImageWarning", Culture);

		/// <summary>
		/// Show image
		/// </summary>
		public static string ControlPanel_ShowImage => LocaleManager.GetString("ControlPanel_ShowImage", Culture);

		/// <summary>
		/// Side length
		/// </summary>
		public static string ControlPanel_SideLength => LocaleManager.GetString("ControlPanel_SideLength", Culture);

		/// <summary>
		/// Size
		/// </summary>
		public static string ControlPanel_Size => LocaleManager.GetString("ControlPanel_Size", Culture);

		/// <summary>
		/// Yes
		/// </summary>
		public static string ControlPanel_Yes => LocaleManager.GetString("ControlPanel_Yes", Culture);

		/// <summary>
		/// Load Settings
		/// </summary>
		public static string LoadSettings => LocaleManager.GetString("LoadSettings", Culture);

		/// <summary>
		/// Overlay images on top of the map, allowing you to replicate real city.
		/// </summary>
		public static string MOD_Description => LocaleManager.GetString("MOD_Description", Culture);

		/// <summary>
		/// Open PNG directory
		/// </summary>
		public static string OptionPanel_OpenPNGDirectory => LocaleManager.GetString("OptionPanel_OpenPNGDirectory", Culture);

		/// <summary>
		/// PNG file path
		/// </summary>
		public static string OptionPanel_PNGFilePath => LocaleManager.GetString("OptionPanel_PNGFilePath", Culture);

		/// <summary>
		/// PNG Options
		/// </summary>
		public static string OptionPanel_PNGOptions => LocaleManager.GetString("OptionPanel_PNGOptions", Culture);

		/// <summary>
		/// Show control panel
		/// </summary>
		public static string OptionPanel_ShowControlPanel => LocaleManager.GetString("OptionPanel_ShowControlPanel", Culture);

		/// <summary>
		/// Always show reload texture results
		/// </summary>
		public static string OptionPanel_ShowReloadResults => LocaleManager.GetString("OptionPanel_ShowReloadResults", Culture);

		/// <summary>
		/// Make sure the PNG file is properly placed.
		/// </summary>
		public static string ReloadError => LocaleManager.GetString("ReloadError", Culture);

		/// <summary>
		/// No matching PNG files found.
		/// </summary>
		public static string ReloadMessageBox_NoMatching => LocaleManager.GetString("ReloadMessageBox_NoMatching", Culture);

		/// <summary>
		/// Reload {0} texture:
		/// </summary>
		public static string ReloadMessageBox_Reload0Texture => LocaleManager.GetString("ReloadMessageBox_Reload0Texture", Culture);

		/// <summary>
		/// [ADD]Added UUI button to invoke control panel.
		/// </summary>
		public static string UpdateLog_V1_8_3ADD1 => LocaleManager.GetString("UpdateLog_V1_8_3ADD1", Culture);

		/// <summary>
		/// [ADD]Added advanced option for reset mod config.
		/// </summary>
		public static string UpdateLog_V1_8_3ADD2 => LocaleManager.GetString("UpdateLog_V1_8_3ADD2", Culture);

		/// <summary>
		/// [FIX]Fixed an issue with control panel background texture not displaying correctly.
		/// </summary>
		public static string UpdateLog_V1_8_3FIX => LocaleManager.GetString("UpdateLog_V1_8_3FIX", Culture);

		/// <summary>
		/// [FIX]Fixed exception thrown when there is no file.
		/// </summary>
		public static string UpdateLog_V1_8_3FIX1 => LocaleManager.GetString("UpdateLog_V1_8_3FIX1", Culture);

		/// <summary>
		/// [UPT]Updated option panel UI style.
		/// </summary>
		public static string UpdateLog_V1_8_3UPT => LocaleManager.GetString("UpdateLog_V1_8_3UPT", Culture);

		/// <summary>
		/// Added Dutch translation.
		/// </summary>
		public static string UpdateLog_V1_8_4TRA0 => LocaleManager.GetString("UpdateLog_V1_8_4TRA0", Culture);

		/// <summary>
		/// Added Korean translation.
		/// </summary>
		public static string UpdateLog_V1_8_4TRA1 => LocaleManager.GetString("UpdateLog_V1_8_4TRA1", Culture);
	}
}