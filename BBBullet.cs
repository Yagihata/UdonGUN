using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BBBullet : UdonSharpBehaviour
{
    [SerializeField]
    public LocalPlayerManager PlayerManager;
    [SerializeField]
    public AudioSource HitSound;
    [SerializeField]
    public AudioSource DamageSound;
    public bool Enabled = false;
    public int CountNow = 0;
    void Start()
    {
        
    }
    void Update()
    {
        if (name.StartsWith("SHOOTED_Bullet") && !Enabled)
            Enabled = true;
        if(Enabled)
        {
            if (CountNow >= 60 * 5)
                Destroy(this.gameObject);
            ++CountNow;
        }
    }
    void OnTriggerEnter(object collider)
    {
        if (Enabled)
        {
            if (collider != null)
            {
                if (collider.GetType().ToString() == "UnityEngine.CharacterController")
                {
                    int damage = 0;
                    var dmg_value_str = gameObject.name.Replace("SHOOTED_Bullet_", "");
                    if (!string.IsNullOrEmpty(dmg_value_str))
                        damage = int.Parse(dmg_value_str);
                    PlayerManager.HP = Mathf.Max(0, PlayerManager.HP - damage);
                    DamageSound.volume = 1f;
                    DamageSound.Play();
                    gameObject.SetActive(false);
                }
                else
                {
                    if (collider.GetType().ToString() == "UnityEngine.BoxCollider")
                    {
                        var col = (BoxCollider)collider;
                        /*var player = VRCPlayerApi.GetPlayerByGameObject(col.gameObject);
                        if(player != null)
                        {
                            if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
                            {
                                HitSound.volume = 1f;
                                HitSound.Play();
                            }
                        }*/
                    }
                    gameObject.SetActive(false);
                }
            }
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (Enabled)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
