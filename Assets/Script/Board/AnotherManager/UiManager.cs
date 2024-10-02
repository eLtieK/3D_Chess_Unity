using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class UiManager : MonoBehaviour
{
    public List<GameObject> prevUiList;
    private Stack<GameObject> prevUi;
    public static UiManager Instance;
    private GameObject gameover;
    private GameObject promotion;
    private GameObject team_container;
    private GameObject angle_container;
    private GameObject angle_lock_container;
    private GameObject mainmenu;
    private GameObject pause_container;
    private GameObject[] lock_unlock;
    private GameObject setting_container;
    public TextMeshProUGUI gameover_text;
    public Slider music_slider, sfx_slider;
    #region setting
    public void MusicVolume() {
        AudioManager.Instance.MusicVolume(music_slider.value);
    }
    public void MusicVolume(float value) {
        AudioManager.Instance.MusicVolume(value);
        music_slider.value = value;
    }
    public void SfxVolume() {
        AudioManager.Instance.SfxVolume(sfx_slider.value);
    }
    public void SfxVolume(float value) {
        AudioManager.Instance.SfxVolume(value);
        sfx_slider.value = value;
    }
    public void SetUpVolume() {
        if(PlayerPrefs.HasKey("music_volume"))
            AudioManager.Instance.musicSource.volume = PlayerPrefs.GetFloat("music_volume");
        if(PlayerPrefs.HasKey("sfx_volume"))
            AudioManager.Instance.sfxSource.volume = PlayerPrefs.GetFloat("sfx_volume");
        music_slider.value = AudioManager.Instance.musicSource.volume;
        sfx_slider.value = AudioManager.Instance.sfxSource.volume;
    }
    public static GameObject FindChild(Transform parent, string name) {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child.gameObject;

            GameObject found = FindChild(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
    public void ToogleMusic() {
        if(!AudioManager.Instance.musicSource.mute) {
            FindChild(setting_container.transform, "MusicOn").SetActive(false);
            FindChild(setting_container.transform, "MusicOff").SetActive(true);
        } else {
            FindChild(setting_container.transform, "MusicOn").SetActive(true);
            FindChild(setting_container.transform, "MusicOff").SetActive(false);
        }
        AudioManager.Instance.ToogleMusic();
    }
    public void CheckMusic() {
        if(AudioManager.Instance.musicSource.mute) {
            FindChild(setting_container.transform, "MusicOn").SetActive(false);
            FindChild(setting_container.transform, "MusicOff").SetActive(true);
        } else {
            FindChild(setting_container.transform, "MusicOn").SetActive(true);
            FindChild(setting_container.transform, "MusicOff").SetActive(false);
        }
    }
    public void CheckSfx() {
        if(AudioManager.Instance.sfxSource.mute) {
            FindChild(setting_container.transform, "SoundOn").SetActive(false);
            FindChild(setting_container.transform, "SoundOff").SetActive(true);
        } else {
            FindChild(setting_container.transform, "SoundOn").SetActive(true);
            FindChild(setting_container.transform, "SoundOff").SetActive(false);
        }
    }
    public void ToogleSFX() {
        if(!AudioManager.Instance.sfxSource.mute) {
            FindChild(setting_container.transform, "SoundOn").SetActive(false);
            FindChild(setting_container.transform, "SoundOff").SetActive(true);
        } else {
            FindChild(setting_container.transform, "SoundOn").SetActive(true);
            FindChild(setting_container.transform, "SoundOff").SetActive(false);
        }
        AudioManager.Instance.ToogleSFX();
    }
    public void SaveSetting() {
        PlayerPrefs.SetInt("save_setting",1);
        PlayerPrefs.SetInt("music", AudioManager.Instance.musicSource.mute ? 1 : 0);
        PlayerPrefs.SetInt("sfx", AudioManager.Instance.sfxSource.mute ? 1 : 0);
        PlayerPrefs.SetFloat("music_volume", music_slider.value);
        PlayerPrefs.SetFloat("sfx_volume", sfx_slider.value);
        PlayerPrefs.Save();
    }
    public void LoadSetting() {
        if(!PlayerPrefs.HasKey("save_setting"))
            return;
        if(AudioManager.Instance.musicSource != null) 
            AudioManager.Instance.musicSource.mute = PlayerPrefs.GetInt("music") == 1;
        CheckMusic();
        MusicVolume(PlayerPrefs.GetFloat("music_volume"));
        if(AudioManager.Instance.sfxSource != null) 
            AudioManager.Instance.sfxSource.mute = PlayerPrefs.GetInt("sfx") == 1;
        CheckSfx();
        SfxVolume(PlayerPrefs.GetFloat("sfx_volume"));
    }
    #endregion
    #region MainMenu
    public void OpenMainMenu() {
        NextMenu("MainMenu");
    }
    public void NextMenu(String name) {
        foreach(Transform child in mainmenu.transform) {
            if(child.gameObject.activeSelf && !child.name.Equals("Board")) 
                prevUi.Push(child.gameObject);
            if(child.name.Equals(name)) {
                child.gameObject.SetActive(true);
            }
            else if(child.name.Equals("Board"))
                continue;
            else 
                child.gameObject.SetActive(false);
        }
        CheckHaveSave(name);
    }
    public void BackMenu() {
        NextMenu(prevUi.Pop().name);
        prevUi.Pop(); //huy viec push tu ham NextMenu
    }
    public void CheckHaveSave(string key) {
        if(key == "Local") {
            GameObject local =FindChild(mainmenu.transform, "Local");
            if(!PlayerPrefs.HasKey("local")) {
                local.GetComponent<RectTransform>().localPosition = new Vector3(0, 100, 0);
                FindChild(local.transform, "Continue").SetActive(false);
            } else {
                local.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                FindChild(local.transform, "Continue").SetActive(true);
            }
        } else if(key == "Bot") {
            GameObject bot =FindChild(mainmenu.transform, "Bot");
            if(!PlayerPrefs.HasKey("bot")) {
                bot.GetComponent<RectTransform>().localPosition = new Vector3(0, 100, 0);
                FindChild(bot.transform, "Continue").SetActive(false);
            } else {
                bot.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                FindChild(bot.transform, "Continue").SetActive(true);
            }
        }
        
    }
    #endregion
    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        prevUi = new Stack<GameObject>(prevUiList);
        gameover = GameObject.Find("GameoverBoard");
        promotion = GameObject.Find("PromotionBoard");
        mainmenu = GameObject.Find("MainmenuBoard");
        team_container = GameObject.Find("TeamContainer");
        angle_container = GameObject.Find("CameraAngleContainer");
        angle_lock_container = GameObject.Find("LockCameraContainer");
        pause_container = GameObject.Find("PauseContainer");
        lock_unlock = new GameObject[2];
        lock_unlock[0] = GameObject.Find("LockImage");
        lock_unlock[1] = GameObject.Find("UnlockImage");
        setting_container = GameObject.Find("SettingBoard");

        OpenMainMenu();
        setting_container.SetActive(false);
        lock_unlock[1].SetActive(false);
        gameover.SetActive(false); 
        promotion.SetActive(false);
        team_container.SetActive(false);
        angle_container.SetActive(false);
        angle_lock_container.SetActive(false);
        pause_container.transform.Find("PauseBoard").gameObject.SetActive(false);
        pause_container.transform.Find("PauseButton").gameObject.SetActive(false);
    }
    public void Promotion() {
        promotion.SetActive(true);
    }
    public void SetUpPromotionStart() {
        angle_container.SetActive(false);
        angle_lock_container.SetActive(false);
        team_container.SetActive(false);
        pause_container.transform.Find("PauseButton").gameObject.SetActive(false);
    }
    public void EndPromotion() {
        promotion.SetActive(false);
        angle_container.SetActive(true);
        angle_lock_container.SetActive(true);
        team_container.SetActive(true);
        pause_container.transform.Find("PauseButton").gameObject.SetActive(true);
    }
    public void Pause() {
        SetUpPromotionStart(); //dung tam function cho nhanh
        pause_container.transform.Find("PauseBoard").gameObject.SetActive(true);
        BoardManager.Instance.pause = true;
    }
    public void Setting() {
        setting_container.SetActive(true);
        BoardManager.Instance.pause = true;
        if(pause_container.transform.Find("PauseBoard").gameObject.activeSelf) {
            prevUi.Push(pause_container.transform.Find("PauseBoard").gameObject);
            pause_container.transform.Find("PauseBoard").gameObject.SetActive(false);
        }
    }
    public void DoneSetting() {
        setting_container.SetActive(false);
        BoardManager.Instance.pause = false;
        if(prevUi.Peek().name.Equals("PauseBoard")) {
            prevUi.Pop();
            pause_container.transform.Find("PauseBoard").gameObject.SetActive(true);
        }
    }
    public void Continue() {
        EndPromotion();
        pause_container.transform.Find("PauseBoard").gameObject.SetActive(false);
        BoardManager.Instance.pause = false;
    }
    public void EndGame() {
        gameover.SetActive(true);
        if(BoardManager.Instance.is_white_turn) {
            gameover_text.text = "White win";
        } else {
            gameover_text.text = "Black win";
        }
    }  
    public void HideAllUi() {
        gameover.SetActive(false);
    }
    private void Update() {
        CheckTeam();
    }
    private void CheckTeam() {
        GameObject team = GameObject.Find("TeamImage");
        if(team == null) {return;}
        if(BoardManager.Instance.is_white_turn) {
            team.GetComponent<Image>().color = Color.white;
        } else {
            team.GetComponent<Image>().color = Color.black;
        }
    }
    public void ToogleLockOrUnlock() {
        if(lock_unlock[0].activeSelf) {
            lock_unlock[0].SetActive(false);
            lock_unlock[1].SetActive(true);
        } else {
            lock_unlock[0].SetActive(true);
            lock_unlock[1].SetActive(false);
        }
    }
    public bool IsLockCam() {
        return lock_unlock[0].activeSelf;
    }
    public void Play() {
        angle_container.SetActive(true);
        angle_lock_container.SetActive(true);
        team_container.SetActive(true);
        pause_container.transform.Find("PauseButton").gameObject.SetActive(true);
        mainmenu.SetActive(false);
        MainmenuManager.Instance.Play();
        BoardManager.Instance.Play();
    }
    public void LoadContinue(string key) {
        angle_container.SetActive(true);
        angle_lock_container.SetActive(true);
        team_container.SetActive(true);
        pause_container.transform.Find("PauseButton").gameObject.SetActive(true);
        mainmenu.SetActive(false);
        MainmenuManager.Instance.Play();
        BoardManager.Instance.Continue(key);
    }
    public void MainMenu() {
        HideAllUi();
        SetUpPromotionStart();
        pause_container.transform.Find("PauseBoard").gameObject.SetActive(false);
        mainmenu.SetActive(true);
        OpenMainMenu();
    }
    public void Exit() {
        Application.Quit();
    }
}
