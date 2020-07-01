using System.Xml.Serialization;

public class Wave
{
    public int waveNumber; //1 = first, 2 = second, etc;
    public float spawnRate = 0; //How fast enemies are spawned

    //Enemy Assignments; Change number of each enemy you want spawned, change spawn order to spawn certain enemies in that ascending order

    [XmlArray("Enemies"), XmlArrayItem("Enemy")]
    public Enemy[] Enemies;
}
