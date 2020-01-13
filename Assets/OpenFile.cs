using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SFB;

public class OpenFile : MonoBehaviour
{
    public Dropdown map, firstPlayer, secondPlayer;
    public Button playButton;
    string rootDir;

    // Start is called before the first frame update
    void Start()
    {
        if (GameInfo.rootDir != "")
        {
            rootDir = GameInfo.rootDir;
            PopulateDropdown();
        }
    }

    public void ChooseDir()
    {
        firstPlayer.options = new List<Dropdown.OptionData>();
        secondPlayer.options = new List<Dropdown.OptionData>();
        map.options = new List<Dropdown.OptionData>();

        map.interactable = firstPlayer.interactable = secondPlayer.interactable = playButton.interactable = false;

        string[] rootDirs = StandaloneFileBrowser.OpenFolderPanel("Choose root directory", "", false);
        if (rootDirs.Length == 1)
        {
            rootDir = rootDirs[0];
            PopulateDropdown();
        }
    }

    private void PopulateDropdown()
    {
        if (Directory.GetDirectories(rootDir, "Players").Length > 0 && Directory.GetDirectories(rootDir, "Maps").Length > 0)
        {
            string[] playerDirs = Directory.GetDirectories($"{rootDir}/Players", "Team*");
            string[] mapFiles = Directory.GetFiles($"{rootDir}/Maps", "*.txt");

            if (playerDirs.Length > 0 && mapFiles.Length > 0)
            {
                List<string> playerDirsShort = new List<string>();
                foreach (string playerDir in playerDirs)
                {
                    string[] tmp = playerDir.Split('\\');
                    playerDirsShort.Add(tmp[tmp.Length - 1]);
                }
                firstPlayer.AddOptions(playerDirsShort);
                secondPlayer.AddOptions(playerDirsShort);

                List<string> mapFilesShort = new List<string>();
                foreach (string mapFile in mapFiles)
                {
                    mapFilesShort.Add(Path.GetFileNameWithoutExtension(mapFile));
                }
                map.AddOptions(mapFilesShort);

                map.interactable = true;
                firstPlayer.interactable = true;
                secondPlayer.interactable = true;
                playButton.interactable = true;
            }
        }
    }

    public void Play()
    {
        GameInfo.rootDir = rootDir;
        GameInfo.playerIds = new string[] {
            firstPlayer.options[firstPlayer.value].text,
            secondPlayer.options[secondPlayer.value].text
        };
        GameInfo.mapConfigFileName = map.options[map.value].text;

        SceneManager.LoadScene("MainGame");
    }
}
