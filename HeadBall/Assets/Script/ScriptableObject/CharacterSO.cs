using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Data/Character", order = 1)]
public class CharacterSO : ScriptableObject
{
    public Sprite head;
    public Sprite hand;
    public Sprite feet;
    public Sprite avatar;
    public string characterName;
    public float speed;
    public float shootPower;
    public float jumpPower;
}
