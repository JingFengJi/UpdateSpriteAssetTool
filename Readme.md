# Unity编辑器拓展之二十七：TextMeshPro的TMP_SpriteAsset图文混排图集快捷更新工具

## 1.TextMeshPro的SpriteAsset制作

此文不再赘述，参考此文：https://blog.csdn.net/qq_37057633/article/details/81120583

## 2.工具使用场景
当图文混排图集需要加一个新图时，需要重新按照上文的流程重新打一遍Json(Array)格式和一遍Unity-Texture2D Sprite格式的，打完之后，图集里的图文位置就会发生变化，TMP_SpriteAsset里存储的spriteGlyphTable和spriteCharacterTable并不会自动更新（包括旧的sprite相关数据和新增的sprite数据），而且旧的图文混排图集里的Sprite ID也应该保持不变，如果我们手动修改了Sprite的GlyphMetrics数据，这个数据也会被重打的覆盖掉，使得之前的数据丢失。那么，开发一款快捷更新图文混排的工具就十分有必要。

## 3.工具介绍
## 3.1 工具界面

![在这里插入图片描述](https://img-blog.csdnimg.cn/20190809104416516.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzI2OTk5NTA5,size_16,color_FFFFFF,t_70)

## 3.2 工具功能
1.缓存数据

主要是缓存旧的图集数据，spriteGlyphTable和spriteCharacterTable

2.一键打Json格式图集

使用命令行打图集，此文不再赘述，参考：https://blog.csdn.net/u014065445/article/details/54289787

使用命令行打json格式文件

3.一键打Unity格式图集

使用命令行打Unity格式图集

```
    /// <summary>
    /// 打Unity格式图集
    /// </summary>
    private void UnitySpriteTexturePacker()
    {
        curTime = System.DateTime.Now;
        //获取打Unity格式的TexturePacker命令
        string cmd = TexturePackerCommond.GetPackCommond(arangeSpritesFolderPath,arrangeIconAtlasPngPath,arrangeIconAtlasTpsheetFilePath);
        //使用ShellHelper生成一个ShellRequest对象
        ShellHelper.ShellRequest request = ShellHelper.ProcessCommand(cmd,null);
        EditorUtility.DisplayProgressBar("批量处理中...","Shell脚本生成Json图集信息...", 0);
        //注册命令执行完回调
        request.onDone  += ()=>
        {
            {
                //打包结束
                EditorUtility.DisplayProgressBar("批量处理中...","Shell脚本执行完毕...开始打图集...", 0);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                CreateUIAtlas();
            }
        };
        //注册命令执行错误的回调
        request.onError += ()=>
        {
            Debug.LogError("Texture Packer Error!!! Please Check Your Config");
            EditorUtility.ClearProgressBar();
        };
    }
```
4.数据更新

解析Tpsheet格式文件，从中拿到所以Sprite的GlyphRect数据，【注意】Tpsheet文件中Sprite的GlyphRect数据是以SpriteName区分的，因此TMP_SpriteAsset里的spriteCharacterTable里的name一定要正确，不然数据会错位。

![在这里插入图片描述](https://img-blog.csdnimg.cn/20190809104455334.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L3FxXzI2OTk5NTA5,size_16,color_FFFFFF,t_70)

## 4.使用说明
1.TexturePacker_PATH更改为自己电脑的TexturePacker路径

2.工具开发环境是mac端，windows端下自行类比修改

3.需要导入TexturePackerImporter工具配合TexturePacker打出来的图集使用

4.代码中的路径自行修改

## 5.工具仓库地址
https://github.com/JingFengJi/UpdateSpriteAssetTool.git

以上知识分享，如有错误，欢迎指出，共同学习，共同进步。

最近在用hexo 和 github page搭 个人博客，地址如下：
http://www.jingfengji.tech/
欢迎大家关注。

最近的一些博客 还是会更新在 CSDN这边，后续以自己个人的博客站点会主。