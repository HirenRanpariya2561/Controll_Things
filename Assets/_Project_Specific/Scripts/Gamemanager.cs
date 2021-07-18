using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance;
    public GameObject GameOver_Panel, Win_Panel, Main_camera;
    public GameObject Level;
    [SerializeField] Text Level_no_txt;
    public void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level"));
            PlayerPrefs.Save();
        }
        LevelLoad(PlayerPrefs.GetInt("Level"));
        Level_no_txt.text = "Level " + PlayerPrefs.GetInt("Level");
    }
    public void Gameover()
    {
        GameOver_Panel.SetActive(true);
    }
    public void WinGame()
    {
        Win_Panel.SetActive(true);
    }
    public void Onbtnclick(string buttonname)
    {
        switch (buttonname)
        {
            case "Reload":
                GameOver_Panel.SetActive(false);
                Destroy(Level);
                Level = null;
                LevelLoad(PlayerPrefs.GetInt("Level"));
                SimpleCamFollow.instance.SetcamerastartPos();
                ResetCamera();
                Level_no_txt.text = "Level " + PlayerPrefs.GetInt("Level");
                break;
            case "Next":
                Win_Panel.SetActive(false);
                PlayerPrefs.SetInt("Level", (PlayerPrefs.GetInt("Level") + 1));
                PlayerPrefs.Save();
                Destroy(Level);
                Level = null;
                LevelLoad(PlayerPrefs.GetInt("Level"));//Tempory commented
                if (PlayerPrefs.GetInt("Level") < 13)
                {
                    LevelLoad(PlayerPrefs.GetInt("Level"));
                }
                else
                {
                    LevelLoad(PlayerPrefs.GetInt("Level") - 1);
                }
                SimpleCamFollow.instance.SetcamerastartPos();
                ResetCamera();
                Level_no_txt.text = "Level " + PlayerPrefs.GetInt("Level");
                //Load next Level..
                break;
        }
    }
    public void LevelLoad(int Level_no)
    {
        GameObject g = (Resources.Load("Level_" + Level_no)) as GameObject;//Tempory 4
        if (Level == null)
        {
            Level = Instantiate(g);
        }
    }
    public void ResetCamera()
    {
        Main_camera.transform.localPosition = Level.transform.GetChild(0).transform.localPosition;
    }
}
