using System.Xml.Serialization;

public class TowerData
{
    public int type; //Type of tower; 0 = machine gun
    public string name = "Tower Name";
    public int price = 100;
    public float range = 2;
    public int damage = 10;
    public float rotateSpeed = 20; //How fast the tower rotates to face targets
    public int fireRate = 50; //How long between each shot; Higher = faster

    public BulletData bulletData;

    //Upgrade Levels
    [XmlArray("UpgradeLevels"), XmlArrayItem("UpgradeLevel")]
    public UpgradeLevel[] upgradeLevels;
}

public class UpgradeLevel
{
    public UpgradeLevel() { }

    public int cost = 0;
    public string description = "Upgrade Description";
    public float changeToRange = 0;
    public int changeToDamage = 0;
    public int changeToFireRate = 0;
    public float changeToAoERadius = 0;
    public int changeToBulletSpeed = 0;
    public bool custom = false; //If true, the game will try to use custom hardcoded data for this upgrade level
}
