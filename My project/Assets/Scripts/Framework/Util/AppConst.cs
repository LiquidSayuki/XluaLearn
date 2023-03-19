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
    public class AppConst
    {
        public const string FileListName = "filelist.txt";

        public static GameMode GameMode = GameMode.EditorMode;
        // Hotfix resources （server） address
        public const string ResourcesUrl = "http://127.0.0.1/AssetBundles";
    }
}
