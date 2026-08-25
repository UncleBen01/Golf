using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject aimArrowImage;
    [SerializeField] private GameObject forceSkillCheck;

    public void ShowAimArrow()
    {
        aimArrowImage.SetActive(true);
    }

    public void HideAimArrow()
    {
        aimArrowImage.SetActive(false);
    }

    public void AimArrowUpdateAngle(float angle)
    {
        aimArrowImage.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void ShowForceSkillCheck()
    {
        forceSkillCheck.SetActive(true);
    }

    public void HideForceSkillCheck()
    {
        forceSkillCheck.SetActive(false);
    }
}
