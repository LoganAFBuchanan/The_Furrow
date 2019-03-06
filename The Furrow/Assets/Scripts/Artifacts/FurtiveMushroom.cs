public class FurtiveMushroom : Artifact
{

    public int hpBoost;
    public int cost { get; set; }
    public string name { get; set; }

    public FurtiveMushroom()
    {
        name = "Furtive Mushroom";
        hpBoost = 5;
        cost = 10;
    }

    public void Apply(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.TotalHitPoints += hpBoost;
            hero.HitPoints += hpBoost;
        }
        player.UpdateGUI();
    }

    public void Undo(OverworldPlayer player)
    {
        foreach(HeroControl hero in player.characterList)
        {
            hero.TotalHitPoints -= hpBoost;
            hero.HitPoints -= hpBoost;
        }
        player.UpdateGUI();
    }

    public Artifact Clone()
    {
        return new FurtiveMushroom();
    }
}