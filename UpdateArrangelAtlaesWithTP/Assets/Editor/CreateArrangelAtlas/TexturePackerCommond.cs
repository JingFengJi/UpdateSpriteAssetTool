using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class TexturePackerCommond
{
	private static string TexturePacker_PATH="/Applications/TexturePacker.app/Contents/MacOS/TexturePacker";

	
	private static string JsonPackCommondStr = "{TexturePacker_PATH} {ART_IMG_PATH} --max-size 4096 --allow-free-size --no-trim --format json-array --size-constraints POT --shape-padding 2 --border-padding 2 --common-divisor-x 1 --common-divisor-y 1 --disable-rotation --algorithm MaxRects --opt RGBA8888 --scale 1 --scale-mode Smooth --smart-update --sheet {ATLAS_PNG} --data {ATLAS_DATA}";
	
	//TexturePacker_PATH:TexturePacker安装目录
	//ART_IMG_PATH:原图文件夹目录
	//ATLAS_PNG:图集png文件输出目录
	//ATLAS_DATA:图集json txt文件输出目录
	private static string PackCommondStr = "{TexturePacker_PATH} {ART_IMG_PATH} --max-size 4096 --allow-free-size --no-trim --format unity-texture2d --extrude 0 --size-constraints POT --shape-padding 2 --border-padding 2 --common-divisor-x 1 --common-divisor-y 1 --disable-rotation --algorithm MaxRects --opt RGBA8888 --scale 1 --scale-mode Smooth --smart-update --sheet {ATLAS_PNG} --data {ATLAS_DATA}";
	// --no-trim
	public static string GetPackCommond(string photoFolderPath,string atlasPngOutputPath,string atlasTxtOutputPath)
	{
		if(string.IsNullOrEmpty(photoFolderPath) || string.IsNullOrEmpty(atlasPngOutputPath) || string.IsNullOrEmpty(atlasTxtOutputPath)) return string.Empty;
		string assetsPath  = Application.dataPath;
		string commond = PackCommondStr;
		commond = Regex.Replace(commond, "{TexturePacker_PATH}", TexturePacker_PATH);
		commond = Regex.Replace(commond,"{ART_IMG_PATH}",photoFolderPath);
		commond = Regex.Replace(commond,"{ATLAS_PNG}",atlasPngOutputPath);
		commond = Regex.Replace(commond,"{ATLAS_DATA}",atlasTxtOutputPath);
		return commond;
	}

	public static string GetJsonPackCommand(string photoFolderPath,string atlasPngOutputPath,string atlasTxtOutputPath)
	{
		if(string.IsNullOrEmpty(photoFolderPath) || string.IsNullOrEmpty(atlasPngOutputPath) || string.IsNullOrEmpty(atlasTxtOutputPath)) return string.Empty;
		string assetsPath  = Application.dataPath;
		string commond = JsonPackCommondStr;
		commond = Regex.Replace(commond, "{TexturePacker_PATH}", TexturePacker_PATH);
		commond = Regex.Replace(commond,"{ART_IMG_PATH}",photoFolderPath);
		commond = Regex.Replace(commond,"{ATLAS_PNG}",atlasPngOutputPath);
		commond = Regex.Replace(commond,"{ATLAS_DATA}",atlasTxtOutputPath);
		return commond;
	}
	

}