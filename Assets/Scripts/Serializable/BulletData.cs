using System.Xml.Serialization;

public class BulletData
{
    public BulletData() { }

    public string name = "Bullet";
    public int model = 0; //0 = normal bullet; 1 = grenade
    public float aoeRadius = 0;
    public int bulletSpeed = 20;
    public bool explosive = false;
    public float lastingDamagePerSecond = 0;
    public int overrideImpactParticles = -1; //If -1, system will identify which particle to use based on other stats. Otherwise, 0 = none, 1 = medium explosion
}
