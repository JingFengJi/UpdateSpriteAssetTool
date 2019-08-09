using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine.TextCore;

public class CreateArrangelAtlaesWithTP : EditorWindow
{
    private readonly string AtlasPngPath = "Assets/Arts/ArrangeIconAtlas/ArrangeIconAtlas.png";
    private readonly string SpriteAssetPath = "Assets/Arts/ArrangeIconAtlas/ArrangeIconSpriteAsset.asset";
    
    private static CreateArrangelAtlaesWithTP window = null;

    private List<TMP_SpriteGlyph> cacheSpriteGlyphs = new List<TMP_SpriteGlyph>();
    private List<TMP_SpriteCharacter> cacheSpriteCharacters = new List<TMP_SpriteCharacter>();

    private string arangeSpritesFolderPath;

    private string arrangeIconAtlasPngPath;

    private string arrangeIconAtlasJsonFilePath;

    private string arrangeIconAtlasTpsheetFilePath;
    
    private System.DateTime curTime;
    private System.DateTime finishTime;

    private void OnEnable()
    {
        arangeSpritesFolderPath = Application.dataPath + Path.DirectorySeparatorChar + "Arts/ArrangeIcon";
        arrangeIconAtlasPngPath = Application.dataPath + Path.DirectorySeparatorChar +
                                  "Arts/ArrangeIconAtlas/ArrangeIconAtlas.png";
        arrangeIconAtlasJsonFilePath = Application.dataPath + Path.DirectorySeparatorChar +
                                       "Arts/ArrangeIconAtlas/ArrangeIconAtlas.json";
        arrangeIconAtlasTpsheetFilePath = Application.dataPath + Path.DirectorySeparatorChar +
                                          "Arts/ArrangeIconAtlas/ArrangeIconAtlas.tpsheet";
    }

    [MenuItem("Tools/打图文混排图集工具", false, 30)]
    public static void ShowArrangeAtlasWindow()
    {
        if (window == null)
            window = EditorWindow.GetWindow(typeof(CreateArrangelAtlaesWithTP)) as CreateArrangelAtlaesWithTP;
        window.titleContent = new GUIContent("TexturePacker");
        window.minSize = new Vector2(200, 200);
        window.maxSize = new Vector2(200, 200);
        
        window.Show();
    }

    void OnGUI()
    {
        DrawToolBar();
    }

    private void DrawToolBar()
    {
        if (GUILayout.Button("1.缓存数据"))
        {
            CacheSpriteAssetData();
        }
        if(GUILayout.Button("2.打Json"))
        {
            JsonTexturePacker();
        }
        if(GUILayout.Button("3.打UnitySprite"))
        {
            UnitySpriteTexturePacker();
        }
        if(GUILayout.Button("4.整理数据"))
        {
            ApplySpriteAssetData();
        }
    }

    /// <summary>
    /// 缓存图集数据
    /// </summary>
    private void CacheSpriteAssetData()
    {
        TMP_SpriteAsset spriteAsset = AssetDatabase.LoadAssetAtPath<TMP_SpriteAsset>(SpriteAssetPath);
        if (spriteAsset != null)
        {
            cacheSpriteGlyphs = new List<TMP_SpriteGlyph>(spriteAsset.spriteGlyphTable.Count);
            for (int i = 0; i < spriteAsset.spriteGlyphTable.Count; i++)
            {
                TMP_SpriteGlyph spriteGlyph = new TMP_SpriteGlyph();
                TMP_SpriteGlyph originSpriteGlyph = spriteAsset.spriteGlyphTable[i];
                spriteGlyph.sprite = originSpriteGlyph.sprite;
                spriteGlyph.atlasIndex = originSpriteGlyph.atlasIndex;
                spriteGlyph.scale = originSpriteGlyph.scale;
                spriteGlyph.index = originSpriteGlyph.index;

                spriteGlyph.metrics = new GlyphMetrics(originSpriteGlyph.metrics.width,
                    originSpriteGlyph.metrics.height, originSpriteGlyph.metrics.horizontalBearingX,
                    originSpriteGlyph.metrics.horizontalBearingY, originSpriteGlyph.metrics.horizontalAdvance);

                spriteGlyph.glyphRect = new GlyphRect(originSpriteGlyph.glyphRect.x, originSpriteGlyph.glyphRect.y,
                    originSpriteGlyph.glyphRect.width, originSpriteGlyph.glyphRect.height);
                cacheSpriteGlyphs.Add(spriteGlyph);
            }
            
            //spriteCharacterTable
            cacheSpriteCharacters = new List<TMP_SpriteCharacter>(spriteAsset.spriteCharacterTable.Count);
            for (int i = 0; i < spriteAsset.spriteCharacterTable.Count; i++)
            {
                TMP_SpriteCharacter spriteCharacter = new TMP_SpriteCharacter();
                TMP_SpriteCharacter originSpriteCharacter = spriteAsset.spriteCharacterTable[i];

                spriteCharacter.unicode = originSpriteCharacter.unicode;
                spriteCharacter.glyphIndex = originSpriteCharacter.glyphIndex;
                spriteCharacter.scale = originSpriteCharacter.scale;
                spriteCharacter.name = originSpriteCharacter.name;
                spriteCharacter.glyph = cacheSpriteGlyphs[i];
                
                cacheSpriteCharacters.Add(spriteCharacter);
            }
        }
    }

    /// <summary>
    /// 打Json格式图集
    /// </summary>
    private void JsonTexturePacker()
    {
        curTime = System.DateTime.Now;
        string cmd = TexturePackerCommond.GetJsonPackCommand(arangeSpritesFolderPath,arrangeIconAtlasPngPath,arrangeIconAtlasJsonFilePath);
        ShellHelper.ShellRequest request = ShellHelper.ProcessCommand(cmd,null);
        EditorUtility.DisplayProgressBar("批量处理中...","Shell脚本生成Json图集信息...", 0);
        request.onDone  += ()=>
        {
            {
                EditorUtility.DisplayProgressBar("批量处理中...","Shell脚本执行完毕...开始打图集...", 0);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                CreateUIAtlas();
            }
        };
        request.onError += ()=>
        {
            Debug.LogError("Texture Packer Error!!! Please Check Your Config");
            EditorUtility.ClearProgressBar();
        };
    }

    /// <summary>
    /// 打Unity格式图集
    /// </summary>
    private void UnitySpriteTexturePacker()
    {
        curTime = System.DateTime.Now;
        string cmd = TexturePackerCommond.GetPackCommond(arangeSpritesFolderPath,arrangeIconAtlasPngPath,arrangeIconAtlasTpsheetFilePath);
        ShellHelper.ShellRequest request = ShellHelper.ProcessCommand(cmd,null);
        EditorUtility.DisplayProgressBar("批量处理中...","Shell脚本生成Json图集信息...", 0);
        request.onDone  += ()=>
        {
            {
                EditorUtility.DisplayProgressBar("批量处理中...","Shell脚本执行完毕...开始打图集...", 0);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                CreateUIAtlas();
            }
        };
        request.onError += ()=>
        {
            Debug.LogError("Texture Packer Error!!! Please Check Your Config");
            EditorUtility.ClearProgressBar();
        };
    }

    /// <summary>
    /// 更新图集数据
    /// </summary>
    private void ApplySpriteAssetData()
    {
        if (cacheSpriteGlyphs == null || cacheSpriteGlyphs.Count == 0 || cacheSpriteCharacters == null ||
            cacheSpriteCharacters.Count == 0)
        {
            Debug.LogError("无缓存数据");
        }
        
        //1.解析新的tpsheet
        string str = File.ReadAllText(arrangeIconAtlasTpsheetFilePath);
        string[] strArr = str.Split('\n');
        
        //以Name为key的GlyphRect数据
        Dictionary<string,GlyphRect> glyphRectDic = new Dictionary<string, GlyphRect>();
        foreach (var item in strArr)
        {    
            if (item.StartsWith("#") || item.StartsWith(":") || string.IsNullOrEmpty(item)|| item.StartsWith("\r"))
            {
                continue;
            }
            string[] strArr2 = item.Split(';');
            int x = int.Parse(strArr2[1]);
            int y = int.Parse(strArr2[2]);
            int w = int.Parse(strArr2[3]);
            int h = int.Parse(strArr2[4]);
            GlyphRect rect = new GlyphRect(x, y, w, h);
            glyphRectDic.Add(strArr2[0], rect);
        }
        
        //1.1获取新的数据，主要使用GlyphMetrics
        Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(AtlasPngPath);
        List<TMP_SpriteCharacter> newSpriteCharacters = new List<TMP_SpriteCharacter>();
        List<TMP_SpriteGlyph> newSpriteGlyphs = new List<TMP_SpriteGlyph>();
        PopulateSpriteTables(texture, ref newSpriteCharacters, ref newSpriteGlyphs);
        
        //2.更新Cache
        //2.1 cacheSpriteGlyphs
        //2.2 cacheSpriteCharacters
        if (cacheSpriteGlyphs != null && cacheSpriteGlyphs.Count > 0)
        {
            //更新Cache的SpriteGlyph的Rect
            
            for (int i = 0; i < cacheSpriteGlyphs.Count; i++)
            {
                TMP_SpriteGlyph glyph = cacheSpriteGlyphs[i];
                string spriteName = cacheSpriteCharacters[(int) glyph.index].name;
                if (glyphRectDic.ContainsKey(spriteName))
                {
                    GlyphRect newRect = glyphRectDic[spriteName];
                    glyph.glyphRect = new GlyphRect(newRect.x, newRect.y, newRect.width, newRect.height);
                }
            }
        }

        foreach (KeyValuePair<string,GlyphRect> glyphRect in glyphRectDic)
        {
            TMP_SpriteCharacter findSpriteCharacter = cacheSpriteCharacters.Find(character => character.name == glyphRect.Key);
            TMP_SpriteCharacter newSpriteCharacter = newSpriteCharacters.Find(character => character.name == glyphRect.Key);
            if (findSpriteCharacter == null)
            {
                //新增的Sprite
                    
                TMP_SpriteGlyph spriteGlyph = new TMP_SpriteGlyph();
                spriteGlyph.sprite = null;
                spriteGlyph.atlasIndex = 0;
                spriteGlyph.scale = 1.0f;
                spriteGlyph.index = (uint)cacheSpriteGlyphs.Count;

                GlyphMetrics metrics = newSpriteCharacter.glyph.metrics;
                spriteGlyph.metrics = new GlyphMetrics(metrics.width, metrics.height, metrics.horizontalBearingX,
                    metrics.horizontalBearingY, metrics.horizontalAdvance);

                spriteGlyph.glyphRect = new GlyphRect(glyphRect.Value.x, glyphRect.Value.y,
                    glyphRect.Value.width, glyphRect.Value.height);
                    
                cacheSpriteGlyphs.Add(spriteGlyph);
                    
                TMP_SpriteCharacter spriteCharacter = new TMP_SpriteCharacter(0,spriteGlyph);
                spriteCharacter.glyphIndex = (uint)cacheSpriteCharacters.Count;
                spriteCharacter.scale = 1;
                spriteCharacter.name = glyphRect.Key;
                
                cacheSpriteCharacters.Add(spriteCharacter);
            }
        }
        
        //3.cache写入SpriteAsset
        TMP_SpriteAsset spriteAsset = AssetDatabase.LoadAssetAtPath<TMP_SpriteAsset>(SpriteAssetPath);
        FieldInfo spriteCharacterTablePi = spriteAsset.GetType().GetField("m_SpriteCharacterTable", BindingFlags.NonPublic | BindingFlags.Instance);
        if (null != spriteCharacterTablePi)
        {
            spriteCharacterTablePi.SetValue(spriteAsset, cacheSpriteCharacters);
        }
        FieldInfo spriteGlyphTablePi = spriteAsset.GetType().GetField("m_SpriteGlyphTable", BindingFlags.NonPublic | BindingFlags.Instance);
        if (null != spriteGlyphTablePi)
        {
            spriteGlyphTablePi.SetValue(spriteAsset, cacheSpriteGlyphs);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    private void CreateUIAtlas()
    {
        TextureSetting(AtlasPngPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        finishTime = System.DateTime.Now;
        Debug.LogError("本次打图集总耗时：" + (finishTime - curTime));
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 设置图片格式，根据项目情况自行修改
    /// </summary>
    /// <param name="path"></param>
    /// <param name="mTextureImporterType"></param>
    public void TextureSetting(string path)
    {
        if(string.IsNullOrEmpty(path) || !IsTextureFile(path)) return;
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        if (textureImporter == null) return;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.wrapMode = TextureWrapMode.Clamp;
        textureImporter.filterMode = FilterMode.Bilinear;
        textureImporter.isReadable = false;
        textureImporter.maxTextureSize = 2048;
        
//        TextureImporterPlatformSettings iPhoneSetting = new TextureImporterPlatformSettings();
//        iPhoneSetting.overridden = true;
//        iPhoneSetting.format = TextureImporterFormat.ASTC_RGBA_4x4;
//        iPhoneSetting.maxTextureSize = 2048;
//        iPhoneSetting.name = "iPhone";
//        iPhoneSetting.compressionQuality = (int)UnityEngine.TextureCompressionQuality.Normal;
//        textureImporter.SetPlatformTextureSettings(iPhoneSetting);

        TextureImporterPlatformSettings androidSetting = new TextureImporterPlatformSettings();
        androidSetting.overridden = true;
        androidSetting.maxTextureSize = 2048;
        androidSetting.name = "Android";
        androidSetting.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
        androidSetting.format = TextureImporterFormat.ETC2_RGBA8;
        androidSetting.compressionQuality = (int)UnityEngine.TextureCompressionQuality.Normal;
        androidSetting.androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings;
        textureImporter.SetPlatformTextureSettings(androidSetting);
        
        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 判断是否是图片格式
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    private bool IsTextureFile(string _path)
    {
        string path = _path.ToLower();
        return path.EndsWith(".psd") || path.EndsWith(".tga") || path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".dds") || path.EndsWith(".bmp") || path.EndsWith(".tif") || path.EndsWith(".gif");
    }
    
    /// <summary>
    /// 代码来源于TMPro.EditorUtilities.TMP_SpriteAssetMenu脚本中
    /// </summary>
    /// <param name="source"></param>
    /// <param name="spriteCharacterTable"></param>
    /// <param name="spriteGlyphTable"></param>
    private static void PopulateSpriteTables(Texture source, ref List<TMP_SpriteCharacter> spriteCharacterTable, ref List<TMP_SpriteGlyph> spriteGlyphTable)
    {
        string filePath = AssetDatabase.GetAssetPath(source);

        // Get all the Sprites sorted by Index
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(filePath).Select(x => x as Sprite).Where(x => x != null).OrderByDescending(x => x.rect.y).ThenBy(x => x.rect.x).ToArray();
            
        for (int i = 0; i < sprites.Length; i++)
        {
            Sprite sprite = sprites[i];

            TMP_SpriteGlyph spriteGlyph = new TMP_SpriteGlyph();
            spriteGlyph.index = (uint)i;
            spriteGlyph.metrics = new GlyphMetrics(sprite.rect.width, sprite.rect.height, -sprite.pivot.x, sprite.rect.height - sprite.pivot.y, sprite.rect.width);
            spriteGlyph.glyphRect = new GlyphRect(sprite.rect);
            spriteGlyph.scale = 1.0f;
            spriteGlyph.sprite = sprite;

            spriteGlyphTable.Add(spriteGlyph);

            TMP_SpriteCharacter spriteCharacter = new TMP_SpriteCharacter(0, spriteGlyph);
            spriteCharacter.name = sprite.name;
            spriteCharacter.scale = 1.0f;

            spriteCharacterTable.Add(spriteCharacter);
        }
    }
}