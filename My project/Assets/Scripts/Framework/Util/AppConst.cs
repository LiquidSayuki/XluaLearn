using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public enum GameMode
    {
        None = 0,
        EditorMode,
        PackageBundle,
        UpdateMode
    }
    public enum GameEvent
    {
        None = 0,
        GameInit = 10000,
        StartLua
    }

    public class AppConst
    {
        public const string FileListName = "filelist.txt";

        public static GameMode GameMode = GameMode.EditorMode;
        // Hotfix resources （server） address
        public const string ResourcesUrl = "http://192.168.3.201/AssetBundles";

        public static bool OpenLog = true;
    }
}
