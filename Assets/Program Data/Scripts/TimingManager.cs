using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class TimingManager : SerializedMonoBehaviour
{
    public Options_BackgroundSelect m_bgSelect;
    public CustomSprites m_customSprites;
    public Menu_Main m_mainMenu;
    public Options_TargetSelect m_targetSelect;
    public Test_Contrast m_testContrast;

    private void Start()
    {
        StartCoroutine(LoadSprites());
    }

    private IEnumerator LoadSprites()
    {
        yield return new WaitForSeconds(1);

        m_customSprites.Initialize();
        //yield return new WaitForSeconds(1);
        m_bgSelect.Initialize();
        //yield return new WaitForSeconds(1);
        m_targetSelect.Initialize();
        //yield return new WaitForSeconds(1);
        m_testContrast.MakeBgsAndTargets();
        //yield return new WaitForSeconds(1);
        //m_mainMenu.Initialize();
        //yield return new WaitForSeconds(1);
        Options_BackgroundSelect.Instance.ShowNormalBackgrounds(false);
        //yield return new WaitForSeconds(1);
        Options_BackgroundSelect.Instance.ShowNormalBackgrounds();
    }
}